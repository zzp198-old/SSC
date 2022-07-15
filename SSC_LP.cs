using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSC_LP : ModPlayer
{
    public override void OnEnterWorld(Player player)
    {
        Main.ActivePlayerFileData = new PlayerFileData();
        Main.ActivePlayerFileData.SetAsActive();
        Main.LocalPlayer.Spawn(PlayerSpawnContext.SpawningIntoWorld);

        var p = Mod.GetPacket();
        p.Write((int)PacketID.UUID);
        p.Write(Main.clientUUID);
        p.Send();
    }
}