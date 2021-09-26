using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSC : Mod
    {
        private static string SavePath => Path.Combine(Main.SavePath, "SSC");
        public static SSCConfig SSCConfig => ModContent.GetInstance<SSCConfig>();

        public override void Load()
        {
            On.Terraria.Main.DrawFPS += On_Main_DrawFPS;
            IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawFPS -= On_Main_DrawFPS;
            IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
        }

        private void On_Main_DrawFPS(On.Terraria.Main.orig_DrawFPS orig, Main self)
        {
            var state = SSCPlayer.SSCReady ? "ON" : "OFF";
            Main.debugWords = $"SSC({state})";
            orig(self);
        }

        private static void IL_NetMessage_SendData(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.GotoNext(i => i.MatchLdsfld(typeof(NPC), nameof(NPC.downedPlantBoss)));
            cursor.GotoNext(i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)));
            cursor.EmitDelegate<Func<byte, byte>>(i => (byte) (i + 64));
        }

        public override void HandlePacket(BinaryReader reader, int _)
        {
            var messageID = (SSCMessageID) reader.ReadByte();
            if (messageID == SSCMessageID.Reset)
            {
                var i = reader.ReadInt32();
                var uuid = reader.ReadString();
                var path = Path.Combine(SavePath, Main.worldName, $"{uuid}.dat");
                var memoryStream = new MemoryStream();
                TagIO.ToStream(File.Exists(path) ? TagIO.FromFile(path) : new TagCompound(), memoryStream);
                var bytes = memoryStream.ToArray();

                // FIXME can't dispatch to clients without for.
                for (var j = 0; j < byte.MaxValue; ++j)
                {
                    if (Main.player[i].name == "") return;
                    var packet = GetPacket();
                    packet.Write((byte) SSCMessageID.Load);
                    packet.Write(i);
                    packet.Write(bytes.Length);
                    packet.Write(bytes);
                    packet.Send(j);
                }

                SSCIO.Load(Main.player[i], TagIO.FromStream(memoryStream));
            }
            else if (messageID == SSCMessageID.Load)
            {
                var i = reader.ReadInt32();
                var memoryStream = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
                SSCIO.Load(Main.player[i], TagIO.FromStream(memoryStream));
                if (i != Main.myPlayer) return;
                SSCPlayer.SSCReady = true;
                SSCPlayer.SaveCountdown = 0;
            }
            else if (messageID == SSCMessageID.Save)
            {
                Directory.CreateDirectory(Path.Combine(SavePath, Main.worldName));
                var i = reader.ReadInt32();
                var uuid = reader.ReadString();
                var path = Path.Combine(SavePath, Main.worldName, $"{uuid}.dat");
                var memoryStream = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
                var tagCompound = TagIO.FromStream(memoryStream);
                SSCIO.ServerTagCompound(Main.player[i], tagCompound);
                TagIO.ToFile(tagCompound, path);
                File.WriteAllText(Path.Combine(SavePath, Main.worldName, $"{uuid}.txt"), TagIO.Serialize(tagCompound).ToString());
            }
        }
    }

    public enum SSCMessageID : byte
    {
        Reset,
        Load,
        Save,
    }
}