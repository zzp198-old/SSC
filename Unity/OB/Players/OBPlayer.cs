using System;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Unity.OB.Players;

public class OBPlayer : ModPlayer
{
    internal static int DeathTime;
    internal static float DeathAlpha;
    internal static bool WatchPlayer;
    internal static int Watch;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return Common.QOS.SC.OB;
    }

    public override void Load()
    {
        IL.Terraria.Main.DrawInterface_35_YouDied += IL_Main_DrawInterface_35_YouDied; // 字体位置下调
        On.Terraria.Player.GetDeathAlpha += On_Player_GetDeathAlpha; // 隐藏字体
    }

    public override void Unload()
    {
        IL.Terraria.Main.DrawInterface_35_YouDied -= IL_Main_DrawInterface_35_YouDied;
        On.Terraria.Player.GetDeathAlpha -= On_Player_GetDeathAlpha;
    }

    public override void ResetEffects()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        DeathTime = 0;
        DeathAlpha = 0;
    }

    public override void UpdateDead()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        if (DeathTime < 150)
        {
            DeathTime++;
            return;
        }

        if (Common.QOS.SC.ReviveSeal && Player.GetModPlayer<ReviveSeal.Players.ReviveSealPlayer>().Sealers.Count > 0)
        {
            DeathAlpha = Utils.Clamp(DeathAlpha + 1, 0, 100);
        }
        else
        {
            DeathAlpha = Utils.Clamp(DeathAlpha - 1, 0, 100);
        }

        if (!Common.Systems.FlagSystem.AnyActiveBoss)
        {
            return;
        }

        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            Watch++;
        }

        if (Main.mouseRight && Main.mouseRightRelease)
        {
            WatchPlayer = !WatchPlayer;
        }

        if (Watch >= Common.Systems.FlagSystem.ActiveBoss.Count)
        {
            Watch = 0;
        }

        var npc = Common.Systems.FlagSystem.ActiveBoss[Watch]; // 筛选出要观战的BOSS
        var targetCenter = npc.Center;

        if (WatchPlayer && npc.HasValidTarget) // 选择观战Boss的目标玩家
        {
            targetCenter = npc.GetTargetData().Center;
        }

        var coordinate = targetCenter.ToTileCoordinates();
        if (!WorldGen.InWorld(coordinate.X, coordinate.Y))
        {
            return;
        }

        if (Vector2.Distance(Player.Center, targetCenter) < 5)
        {
            targetCenter = Player.Center;
        }
        else if (Vector2.Distance(Player.Center, targetCenter) < 100) // 平滑处理
        {
            targetCenter = Vector2.Lerp(Player.Center, targetCenter, 0.05f);
        }
        else if (Vector2.Distance(Player.Center, targetCenter) < Main.ScreenSize.ToVector2().Length())
        {
            targetCenter = Vector2.Lerp(Player.Center, targetCenter, 0.1f);
        }

        Player.Center = targetCenter;
    }

    private static void IL_Main_DrawInterface_35_YouDied(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcR4(-60));
        c.EmitDelegate<Func<float, float>>(_ => 60);
    }

    private static Color On_Player_GetDeathAlpha(On.Terraria.Player.orig_GetDeathAlpha invoke, Player self, Color newColor)
    {
        return invoke(self, newColor) * (1 - DeathAlpha / 100);
    }
}