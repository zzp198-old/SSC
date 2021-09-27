using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using SSC.Component;
using SSC.Extension;

// XXX Real-Time Synchronization. TODO There is a flaw in real-time synchronization.
// When the player sends a partial data request to save, the server will read the remaining part locally.
// At this time, if the player disconnects, it is not guaranteed that the local data can be read correctly.
namespace SSC
{
    public class SSC : Mod
    {
        public static string BasePath => Path.Combine(Main.SavePath, "SSC");
        public static SSC Instance => ModContent.GetInstance<SSC>();
        public static SSCConfig SSCConfig => ModContent.GetInstance<SSCConfig>();

        public override void Load() => IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData;
        public override void Unload() => IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;

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
            if (messageID == SSCMessageID.RequestStats)
            {
                var i = reader.ReadInt32();
                var uuid = reader.ReadString();
                var compound = SSCStats.Extract(uuid);

                var socket = GetPacket();
                socket.Write((byte) SSCMessageID.ReceiveStats);
                socket.Write(i);
                socket.Write(compound);
                socket.Send();
                SSCStats.Receive(Main.player[i], compound);
            }
            else if (messageID == SSCMessageID.ReceiveStats)
            {
                var i = reader.ReadInt32();
                var compound = reader.ReadTagCompound();
                SSCStats.Receive(Main.player[i], compound);
                if (i == Main.myPlayer) SSCPlayer.SSCReady++;
            }
            else if (messageID == SSCMessageID.StorageStats)
            {
                var uuid = reader.ReadString();
                var compound = reader.ReadTagCompound();
                SSCStats.Storage(uuid, compound);
            }
        }
    }

    public enum SSCMessageID : byte
    {
        RequestStats,
        ReceiveStats,
        StorageStats,
    }
}