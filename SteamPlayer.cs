using System;
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
        var packet = Mod.GetPacket();
        packet.Write((byte)PID.SteamID);
        packet.Write(SocialAPI.Friends.GetUsername());
        packet.Send();
    }
}