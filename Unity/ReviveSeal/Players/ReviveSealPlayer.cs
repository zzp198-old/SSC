using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QOS.Unity.ReviveSeal.Players;

public class ReviveSealPlayer : ModPlayer
{
    internal HashSet<NPC> Sealers = new();

    public override bool IsLoadingEnabled(Mod mod)
    {
        return Common.QOS.SC.ReviveSeal;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(Sealers)))
        {
            Sealers = tag.GetIntArray(nameof(Sealers)).Select(i => Main.npc[i]).ToHashSet();
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Set(nameof(Sealers), Sealers.Select(npc => npc.whoAmI).ToArray());
    }

    public override void UpdateDead()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        Sealers = Sealers.Where(npc => npc.active && npc.boss).ToHashSet();
        if (Sealers.Count > 0 && Player.respawnTimer < 180)
        {
            Player.respawnTimer = 180; // 不使用++,避免一些mod加速缩短绕过
        }
    }

    public override void PostUpdate()
    {
        if (Main.GameUpdateCount % 60 != 0 || Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        Sealers = Sealers.Where(npc => npc.active && npc.boss).ToHashSet();
        if (!Common.Systems.FlagSystem.AnyActiveBoss || Player.DeadOrGhost)
        {
            return;
        }

        foreach (var npc in Common.Systems.FlagSystem.ActiveBoss)
        {
            var box = Utils.CenteredRectangle(Player.Hitbox.Center(), Main.ScreenSize.ToVector2());
            box.Inflate(5000, 5000);
            if (box.Intersects(npc.Hitbox))
            {
                Sealers.Add(npc);
            }
        }
    }
}