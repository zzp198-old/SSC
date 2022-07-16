using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC
{
    public class SSC : Mod
    {
        readonly string SavePath = Path.Combine(Main.SavePath, "SSC");

        public override void HandlePacket(BinaryReader b, int _)
        {
            switch (Main.netMode)
            {
                case NetmodeID.MultiplayerClient:
                    ClientPacket(b, _);
                    break;
                case NetmodeID.Server:
                    ServerPacket(b, _);
                    break;
            }

            // var type = (PacketID)b.Read();
            // if (type == PacketID.SSCList)
            // {
            //     if (Main.netMode == NetmodeID.Server)
            //     {
            //         var ps = Directory.GetFiles(Path.Combine(SavePath, b.ReadString()), "*.plr");
            //         var p = GetPacket();
            //         p.Write((int)PacketID.SSCList);
            //         p.Write(ps.Length);
            //         foreach (var plr in ps)
            //         {
            //             p.Write(Path.GetFileNameWithoutExtension(plr));
            //         }
            //
            //         p.Send(_);
            //         return;
            //     }
            //
            //     if (Main.netMode == NetmodeID.MultiplayerClient)
            //     {
            //         var count = b.Read();
            //         if (count == 0)
            //         {
            //             Main.NewText("You don't have any SSC.");
            //         }
            //         else
            //         {
            //             Main.NewText("SSC List: ");
            //             for (var i = 0; i < count; i++)
            //             {
            //                 Main.NewText($"{i}: {b.ReadString()}");
            //             }
            //         }
            //
            //         return;
            //     }
            //
            //     throw new Exception("SSCList");
            // }
        }

        public void ClientPacket(BinaryReader b, int _)
        {
        }

        public void ServerPacket(BinaryReader b, int _)
        {
        }
    }

    public enum PacketID
    {
        SSCList, // C-S-C
        SSCData, // C-S-C
    }
}