using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public override void Load()
    {
        Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", "Temp"));
    }

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        switch ((SSCID)type)
        {
            case SSCID.Connect:
            {
                var i = b.ReadInt32();
                var id = b.ReadUInt64();

                if (Main.netMode == NetmodeID.Server)
                {
                    if (!ModContent.GetInstance<SSCConfig>().RepeatConnect)
                    {
                        if (Main.player.Any(who => who.active && who.GetModPlayer<SSCPlayer>().SteamID == id))
                        {
                            SSCTools.Boot(i, "Repeat connect. Can be modified on the config.");
                            return;
                        }
                    }

                    var p = GetPacket();
                    p.Write((byte)SSCID.Connect);
                    p.Write(i);
                    p.Write(id);
                    p.Send(i);
                    p.Send();
                }

                Main.player[i] = new Player
                {
                    name = id.ToString(), difficulty = (byte)Main.GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                Main.player[i].GetModPlayer<SSCPlayer>().SteamID = id;

                if (i == Main.myPlayer)
                {
                    var data = new PlayerFileData
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = Main.player[i]
                    };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                break;
            }
            default:
                SSCTools.Boot(_, $"Unexpected packet ID: {type}");
                break;
        }
    }
}