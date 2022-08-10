using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    internal ulong SteamID;
    internal int SaveTime;

    public override void OnEnterWorld(Player player)
    {
        base.OnEnterWorld(player);
    }

    public override void PostUpdate()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            
            
            
        }
        
        if (SaveTime > ModContent.GetInstance<SSCConfig>().SaveTime)
        {
        }
    }
}