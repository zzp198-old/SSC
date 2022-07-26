using Terraria;
using Terraria.ModLoader;
using Terraria.Social;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public string SteamID;
    public bool Sended;
    public int Cooldown;

    public override void OnEnterWorld(Player player)
    {
        var packet = Mod.GetPacket();
        packet.Write((byte)PID.SteamID);
        packet.Write(SocialAPI.Friends.GetUsername());
        packet.Send();
    }
}