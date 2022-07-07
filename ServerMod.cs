using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ServerMod
{
    public class ServerMod : Mod
    {
        private static string SavePath => Path.Combine(Main.SavePath, nameof(ServerMod));

        public static SSCConfig SSCConfig => ModContent.GetInstance<SSCConfig>();

        public override void Load()
        {
            On.Terraria.Main.DrawFPS += delegate(On.Terraria.Main.orig_DrawFPS orig, Main self)
            {
                Main.debugWords = "SSC(" + (SSCPlayer.SSCReady ? "ON" : "OFF") + ")";
                orig(self);
            };

            IL.Terraria.NetMessage.SendData += delegate(ILContext il)
            {
                var c = new ILCursor(il);
                c.GotoNext(i => i.MatchLdsfld(typeof(NPC), nameof(NPC.downedPlantBoss)));
                c.GotoNext(i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)));
                c.EmitDelegate<Func<byte, byte>>(i => (byte)(i + 64));
            };
        }

        public override void HandlePacket(BinaryReader reader, int _)
        {
            switch ((SSCMessageID)reader.ReadByte())
            {
                case SSCMessageID.Reset:
                    int index1 = reader.ReadInt32();
                    string str1 = reader.ReadString();
                    string path1 = Path.Combine(SavePath, "SSC", Terraria.Main.worldName, str1 + ".dat");
                    MemoryStream memoryStream1 = new MemoryStream();
                    TagIO.ToStream(File.Exists(path1) ? TagIO.FromFile(path1) : new TagCompound(),
                        (Stream)memoryStream1);
                    byte[] array = memoryStream1.ToArray();
                    for (int toClient = 0; toClient < (int)byte.MaxValue; ++toClient)
                    {
                        if (Terraria.Main.player[index1].name == "")
                            return;
                        ModPacket packet = this.GetPacket();
                        packet.Write((byte)1);
                        packet.Write(index1);
                        packet.Write(array.Length);
                        packet.Write(array);
                        packet.Send(toClient);
                    }

                    SSCIO.Load(Terraria.Main.player[index1], TagIO.FromStream((Stream)memoryStream1));
                    break;
                case SSCMessageID.Load:
                    int index2 = reader.ReadInt32();
                    MemoryStream memoryStream2 = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
                    SSCIO.Load(Terraria.Main.player[index2], TagIO.FromStream((Stream)memoryStream2));
                    if (index2 != Terraria.Main.myPlayer)
                        break;
                    SSCPlayer.SSCReady = true;
                    SSCPlayer.SaveCountdown = 0;
                    break;
                case SSCMessageID.Save:
                    Directory.CreateDirectory(Path.Combine(SavePath, "SSC", Terraria.Main.worldName));
                    int index3 = reader.ReadInt32();
                    string str2 = reader.ReadString();
                    string path2 = Path.Combine(SavePath, "SSC", Terraria.Main.worldName, str2 + ".dat");
                    TagCompound tagCompound =
                        TagIO.FromStream((Stream)new MemoryStream(reader.ReadBytes(reader.ReadInt32())));
                    SSCIO.ServerTagCompound(Terraria.Main.player[index3], tagCompound);
                    TagIO.ToFile(tagCompound, path2);
                    File.WriteAllText(Path.Combine(SavePath, "SSC", Terraria.Main.worldName, str2 + ".txt"),
                        TagIO.Serialize((object)tagCompound).ToString());
                    break;
            }
        }

        public enum SSCMessageID : byte
        {
            Reset,
            Load,
            Save,
        }
    }
}