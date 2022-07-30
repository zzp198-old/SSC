using System.Diagnostics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    // Client SSC/[SteamID]/[WorldID].map
    // Server SSC/[SteamID]/[PlayerName].plr
    internal readonly static string SavePath = Path.Combine(Main.SavePath, "SSC");

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(PID)type} from {_}");

        switch ((PID)type)
        {
            case PID.Steam:
            {
                Debug.Assert(Main.netMode == NetmodeID.Server);

                var whoAmI = b.ReadInt32();
                var SteamID = b.ReadUInt64();
                var GameMode = Main.GameMode == 3 ? (byte)3 : (byte)0;

                Main.player[whoAmI] = new Player { name = SteamID.ToString(), difficulty = GameMode };

                var p = GetPacket();
                p.Write((byte)PID.Erase);
                p.Write(whoAmI);
                p.Write(SteamID);
                p.Write(GameMode);
                Netplay.Clients.Where(i => i.IsConnected()).ToList().ForEach(i => p.Send(i.Id));
            }
                break;
            case PID.Erase:
            {
                var whoAmI = b.ReadInt32();
                var SteamID = b.ReadUInt64();
                var GameMode = b.ReadByte();

                Main.player[whoAmI] = new Player
                {
                    name = SteamID.ToString(), difficulty = GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                if (whoAmI == Main.myPlayer)
                {
                    new PlayerFileData()
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = Main.player[whoAmI],
                    }.SetAsActive();
                }
            }
                break;
            default:
                Utils.Boot(_, $"Unexpected PacketID: {type}");
                break;
        }
    }
}

public enum PID : byte
{
    Steam, // Client only. whoAmI, SteamID
    Erase, // Server only. whoAmI, SteamID, GameMode
}