using System.IO;
using SSC.Common.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSC : Mod
    {
        public static string SavePath = Path.Combine(Main.SavePath, "SSC");
        public static string CachePath = Path.Combine(SavePath, "Cache");

        public override void HandlePacket(BinaryReader br, int _)
        {
            switch (Main.netMode)
            {
                case NetmodeID.MultiplayerClient:
                    ClientPacket(br, _);
                    break;
                case NetmodeID.Server:
                    ServerPacket(br, _);
                    break;
            }
        }

        void ClientPacket(BinaryReader br, int _)
        {
            var type = (PacketID)br.Read();
            if (type == PacketID.SSCList)
            {
                var cacheDir = Path.Combine(SavePath, "Cache");
                var count = br.Read();
                Directory.Delete(cacheDir, true);
                Directory.CreateDirectory(cacheDir);
                for (var i = 0; i < count; i++)
                {
                    var compound = (TagCompound)TagIO.ReadTag(br, out var name);
                    File.WriteAllBytes(Path.Combine(cacheDir, name + ".plr"), compound.GetByteArray("plr"));
                    if (compound.ContainsKey("tplr"))
                    {
                        File.WriteAllBytes(Path.Combine(cacheDir, name + ".tplr"), compound.GetByteArray("tplr"));
                    }
                }

                var UISystem = ModContent.GetInstance<UISystem>();
                UISystem.UI.SetState(UISystem.UIState);
                return;
            }
        }

        void ServerPacket(BinaryReader br, int _)
        {
            var type = (PacketID)br.Read();
            if (type == PacketID.UUID)
            {
                var players = Directory.GetFiles(Path.Combine(SavePath, br.ReadString()), "*.plr");

                var packet = GetPacket();
                packet.Write((int)PacketID.SSCList);
                packet.Write(players.Length);
                foreach (var player in players)
                {
                    var compound = new TagCompound
                    {
                        { "plr", File.ReadAllBytes(player) }
                    };
                    var tplr = Path.ChangeExtension(player, ".tplr");
                    if (File.Exists(tplr))
                    {
                        compound.Add("tplr", File.ReadAllBytes(tplr));
                    }

                    TagIO.WriteTag(Path.GetFileNameWithoutExtension(player), compound, packet);
                }

                packet.Send(_);
                return;
            }
        }
    }

    public enum PacketID
    {
        UUID, // C-S, UUID
        SSCList, // S-C, Count,[Size, TagCompoent]
    }
}