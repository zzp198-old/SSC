using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class ReviveSealSystem : ModSystem
{
    internal static HashSet<NPC> Sealers;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return QOS.SC.ReviveSeal;
    }

    public override void OnWorldLoad()
    {
        Sealers = new HashSet<NPC>();
    }

    public override void OnWorldUnload()
    {
        Sealers = null;
    }

    public override void PostUpdatePlayers()
    {
        if (QOS.My.dead && Sealers.Count > 0 && QOS.My.respawnTimer < 180)
        {
            QOS.My.respawnTimer = 180; // 不使用++,避免一些mod加速缩短绕过
        }
    }

    public override void PostUpdateNPCs()
    {
        if (Main.GameUpdateCount % 60 != 0)
        {
            return;
        }

        Sealers = Sealers.Where(npc => npc.active && npc.boss).ToHashSet();
        if (!Main.CurrentFrameFlags.AnyActiveBossNPC || QOS.My.DeadOrGhost) // 节省运算,虽然四柱和外星探测器会绕过快速过滤,但无伤大雅.不会因为SSC漏帧.
        {
            return;
        }

        foreach (var npc in Main.npc.Where(npc => npc.active && npc.boss))
        {
            var box = Utils.CenteredRectangle(QOS.My.Hitbox.Center(), Main.ScreenSize.ToVector2());
            box.Inflate(5000, 5000);
            if (box.Intersects(npc.Hitbox))
            {
                Sealers.Add(npc);
            }
        }
    }
}