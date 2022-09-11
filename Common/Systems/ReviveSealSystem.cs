using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class ReviveSealSystem : ModSystem
{
    internal static HashSet<NPC> Sealers;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return !Main.dedServ && QOS.SC.ReviveSeal; // 仅在本地客户端执行
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
            QOS.My.respawnTimer = 180; // 避免一些mod因为缩短复活时间导致绕过
        }
    }

    public override void PostUpdateNPCs()
    {
        if (Main.GameUpdateCount % 60 != 0)
        {
            return;
        }

        Sealers = Sealers.Where(npc => npc.active && (npc.boss || NPCID.Sets.DangerThatPreventsOtherDangers[npc.type])).ToHashSet();
        if (!Main.CurrentFrameFlags.AnyActiveBossNPC || QOS.My.DeadOrGhost) // 函数调用前刚好为CurrentFrameFlags赋值
        {
            return;
        }

        foreach (var npc in Main.npc.Where(npc => npc.active && (npc.boss || NPCID.Sets.DangerThatPreventsOtherDangers[npc.type])))
        {
            if (Sealers.Contains(npc))
            {
                continue;
            }

            var box = Utils.CenteredRectangle(QOS.My.Hitbox.Center(), Main.ScreenSize.ToVector2());
            box.Inflate(5000, 5000);
            if (!box.Intersects(npc.Hitbox))
            {
                continue;
            }

            Sealers.Add(npc);
        }
    }
}