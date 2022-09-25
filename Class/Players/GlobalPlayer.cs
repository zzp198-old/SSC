using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QOS.Class.Players;

public class GlobalPlayer : ModPlayer
{
    internal int DeathCount;

    public override void SaveData(TagCompound tag)
    {
        tag.Set(nameof(DeathCount), DeathCount);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(DeathCount))) DeathCount = tag.Get<int>(nameof(DeathCount));
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        base.Kill(damage, hitDirection, pvp, damageSource);
    }
}