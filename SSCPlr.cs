using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlr : ModPlayer
{
    private static int _deadTime;
    private static List<int> _lastFrame;

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (Player.whoAmI == Main.myPlayer)
        {
            _lastFrame = new List<int>();
            var rectangle = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            rectangle.Inflate(5000, 5000);
            foreach (var boss in Main.npc.Where(npc => npc.boss && npc.active))
            {
                if (boss.getRect().Intersects(rectangle))
                {
                    _lastFrame.Add(boss.whoAmI);
                }
            }
        }
    }

    public override void ResetEffects()
    {
        if (Player.whoAmI == Main.myPlayer)
        {
            _deadTime = 0;
            _lastFrame = new List<int>();
        }
    }

    public override void UpdateDead()
    {
        if (Player.whoAmI == Main.myPlayer)
        {
            _deadTime++;
            _lastFrame = (from i in _lastFrame let npc = Main.npc[i] where npc.boss && npc.active select i).ToList();
            if (ModContent.GetInstance<SSCSet>().NoSpawnWhenBossFight)
            {
                if (_lastFrame.Count > 0 && Player.respawnTimer < 180)
                {
                    Player.respawnTimer++;
                }
            }
        }
    }

    public override void ModifyScreenPosition()
    {
        if (Player.whoAmI == Main.myPlayer)
        {
            if (_deadTime > 100 && ModContent.GetInstance<SSCSet>().SpectatorMode)
            {
                foreach (var i in _lastFrame)
                {
                    if (Main.npc[i].boss && Main.npc[i].active)
                    {
                        var pos = Main.npc[i].Center;
                        if (Main.npc[i].HasPlayerTarget)
                        {
                            var who = Main.player[Main.npc[i].target];
                            if (!who.dead && !who.ghost)
                            {
                                pos = Main.player[Main.npc[i].target].Center;
                            }
                        }

                        var coordinate = pos.ToTileCoordinates();
                        if (WorldGen.InWorld(coordinate.X, coordinate.Y))
                        {
                            if (Vector2.Distance(Player.Center, pos) < 50) // 减轻因为延迟造成的短距离抖动
                            {
                                Player.Center = Vector2.Lerp(Player.Center, pos, 0.05f);
                            }
                            else if (Vector2.Distance(Player.Center, pos) < Main.ScreenSize.ToVector2().Length() / 2) // 在半个屏幕内平滑
                            {
                                Player.Center = Vector2.Lerp(Player.Center, pos, 0.1f);
                            }
                            else // 太远则直接闪现
                            {
                                Player.Center = pos;
                            }
                        }

                        return;
                    }
                }
            }
        }
    }

    public override void Load()
    {
        IL.Terraria.Main.DrawInterface_35_YouDied += IL_Main_DrawInterface_35_YouDied;
    }

    public override void Unload()
    {
        IL.Terraria.Main.DrawInterface_35_YouDied -= IL_Main_DrawInterface_35_YouDied;
    }

    private static void IL_Main_DrawInterface_35_YouDied(ILContext il)
    {
        var c = new ILCursor(il);

        c.GotoNext(MoveType.After, i => i.MatchLdcR4(-60));
        c.EmitDelegate<Func<float, float>>(i => i + Main.screenHeight * 0.2f);
        c.GotoNext(MoveType.After,
            i => i.MatchLdsfld(typeof(Lang), nameof(Lang.inter)),
            i => i.MatchLdcI4(38),
            i => i.MatchLdelemRef(),
            i => i.MatchCallvirt(typeof(LocalizedText), "get_Value")
        );
        c.EmitDelegate<Func<string, string>>(i => Language.ActiveCulture == GameCulture.FromName("zh-Hans") ? "菜" : i);
    }
}