using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Social;

namespace SSC;

public class SteamPlayer : ModPlayer
{
    public string SteamID;

    public override void OnEnterWorld(Player player)
    {
        // new PlayerFileData
        // {
        //     Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
        //     Player = new Player
        //     {
        //         name = SocialAPI.Friends.GetUsername() + "(Anonymous)",
        //         difficulty = (byte)Main.GameMode,
        //         dead = true,
        //         ghost = true,
        //         savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
        //     }
        // }.SetAsActive();
        // Main.LocalPlayer.Spawn(PlayerSpawnContext.SpawningIntoWorld);

        var packet = Mod.GetPacket();
        packet.Write((byte)PID.SteamID);
        packet.Write(SocialAPI.Friends.GetUsername());
        packet.Send();
    }
}