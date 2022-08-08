using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public override void Load()
    {
        var dir = Path.Combine(Main.SavePath, "SSC", "Cache");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(PID)type} from {from}");

        switch ((PID)type)
        {
            case PID.SteamAccount:
            {
                var steamID = b.ReadUInt64();
                if (ModContent.GetInstance<SSCConfig>().MultipleOnline)
                {
                    var activeList = Main.player.Where(i => i.active).ToList();
                    if (activeList.Any(i => i.GetModPlayer<SSCPlayer>().SteamID == steamID))
                    {
                        SSCUtils.Boot(from, $"Restrict: Prohibit repeated login.");
                        return;
                    }
                }

                Main.player[from] = new Player { name = steamID.ToString() };
                Main.player[from].GetModPlayer<SSCPlayer>().SteamID = steamID;

                var p = GetPacket();
                p.Write((byte)PID.ClearPLR);
                p.Write(from);
                p.Write(Main.player[from].name);
                ModLoader.GetMod("StreamPacket").Call("SSC", p, from);
                ModLoader.GetMod("StreamPacket").Call("SSC", p);

                break;
            }
            case PID.ClearPLR:
            {
                var whoAmI = b.ReadInt32();
                Main.player[whoAmI] = new Player
                {
                    name = b.ReadString(),
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };

                if (whoAmI == Main.myPlayer)
                {
                    var data = new PlayerFileData
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = Main.player[whoAmI]
                    };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                break;
            }
            case PID.Test:
            {
                var count = b.ReadInt32();

                var data = b.ReadBytes(count);

                File.WriteAllBytes(
                    @"C:\Users\Administrator\Documents\My Games\Terraria\tModLoader\Players\zzp198\42492281-e9e2-4262-9fa9-197372fc1392.map",
                    data);

                break;
            }
            default:
                break;
        }
    }
}