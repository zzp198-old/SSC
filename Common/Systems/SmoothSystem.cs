using Terraria;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class SmoothSystem : ModSystem
{
    public override void Load()
    {
        On.Terraria.Main.DrawDust += On_Main_DrawDust; // 只是隐藏,不影响生成,后续关闭仍可见原有特效
        On.Terraria.Main.DrawGore += On_Main_DrawGore;
        On.Terraria.Main.DrawProj += On_Main_DrawProj;
    }

    public override void Unload()
    {
        On.Terraria.Main.DrawDust -= On_Main_DrawDust;
        On.Terraria.Main.DrawGore -= On_Main_DrawGore;
        On.Terraria.Main.DrawProj -= On_Main_DrawProj;
    }

    private void On_Main_DrawDust(On.Terraria.Main.orig_DrawDust invoke, Main self)
    {
        if (QOS.CC.Smooth && Main.CurrentFrameFlags.AnyActiveBossNPC)
        {
            return;
        }

        invoke(self);
    }

    private void On_Main_DrawGore(On.Terraria.Main.orig_DrawGore invoke, Main self)
    {
        if (QOS.CC.Smooth && Main.CurrentFrameFlags.AnyActiveBossNPC)
        {
            return;
        }

        invoke(self);
    }

    private void On_Main_DrawProj(On.Terraria.Main.orig_DrawProj invoke, Main self, int i)
    {
        if (QOS.CC.Smooth && Main.CurrentFrameFlags.AnyActiveBossNPC)
        {
            var proj = Main.projectile[i];
            if (proj.friendly && proj.owner != Main.myPlayer && proj.owner != byte.MaxValue) // 友军弹幕
            {
                if (QOS.My.dead) // 特例,死亡时可见
                {
                    invoke(self, i);
                }

                return;
            }
        }

        invoke(self, i);
    }
}