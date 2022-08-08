using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    internal ulong SteamID = 0;

    internal bool Record;
    internal FileStream Stream;

    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
        }
    }

    public override void PreUpdate()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        if (Record == false)
        {
            Player.dead = true;
            Player.ghost = true;
        }
    }
}