using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class ObserveSystem : ModSystem
{
    internal const int Interval = 150;
    internal static int Ticks;
    internal static bool Type;
    internal static int Index;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return QOS.SC.Observer;
    }

    public override void Load()
    {
        On.Terraria.Player.GetDeathAlpha += On_Player_GetDeathAlpha; // 死亡字体透明度跟随Ticks
    }

    public override void Unload()
    {
        On.Terraria.Player.GetDeathAlpha -= On_Player_GetDeathAlpha;
    }

    public override void PostUpdatePlayers()
    {
        if (QOS.My.dead && !QOS.My.ghost) // 幽灵模式不允许观战,只能死亡时.Ticks用来记录死亡时间.
        {
            Ticks++;
        }
        else
        {
            Ticks = 0;
        }

        if (Ticks < Interval)
        {
            return;
        }

        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            Index++;
        }

        if (Main.mouseRight && Main.mouseRightRelease)
        {
            Type = !Type;
        }

        var npcList = new List<NPC>();
        if (Main.CurrentFrameFlags.AnyActiveBossNPC) // 四柱和外星探测器会触发,但不会被包含进去
        {
            npcList = Main.npc.Where(npc => npc.active && npc.boss).ToList();
        }

        if (QOS.SC.ReviveSeal)
        {
            npcList = ReviveSealSystem.Sealers.ToList();
        }

        if (npcList.Count == 0) // 可观站的BOSS在复活前都无了,继续显示死亡文本
        {
            Ticks = 0;
            return;
        }

        if (Index >= npcList.Count)
        {
            Index = 0;
        }

        var npc = npcList[Index]; // 筛选出要观战的BOSS
        var targetCenter = npc.Center;

        if (Type && npc.HasValidTarget) // 选择观战Boss的目标玩家
        {
            targetCenter = npc.GetTargetData().Center;
        }

        var coordinate = targetCenter.ToTileCoordinates();
        if (!WorldGen.InWorld(coordinate.X, coordinate.Y))
        {
            return;
        }

        if (Vector2.Distance(QOS.My.Center, targetCenter) < 50) // 平滑处理
        {
            targetCenter = Vector2.Lerp(QOS.My.Center, targetCenter, 0.02f);
        }
        else if (Vector2.Distance(QOS.My.Center, targetCenter) < 100)
        {
            targetCenter = Vector2.Lerp(QOS.My.Center, targetCenter, 0.05f);
        }
        else if (Vector2.Distance(QOS.My.Center, targetCenter) < Main.ScreenSize.ToVector2().Length())
        {
            targetCenter = Vector2.Lerp(QOS.My.Center, targetCenter, 0.1f);
        }

        QOS.My.Center = targetCenter;
    }

    private Color On_Player_GetDeathAlpha(On.Terraria.Player.orig_GetDeathAlpha invoke, Player self, Color newColor)
    {
        return Ticks < Interval ? invoke(self, newColor) : Color.Transparent;
    }
}