using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace SSC.Common.Configs;

[Label("SSC Main Config")]
public class UnityConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("SSC")] //
    [Slider] //
    [SliderColor(200, 0, 0)]
    [Range(1, 500)]
    [DefaultValue(100)]
    [ReloadRequired]
    public int StartingHealth = 100;

    [Slider] //
    [SliderColor(0, 0, 200)]
    [Range(1, 200)]
    [DefaultValue(20)]
    [ReloadRequired]
    public int StartingMana = 20;

    public List<string> AdminUser = new();

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
    {
        if (AdminUser.Count == 0)
        {
            // TODO
            message = "修改成功,请添加管理员名称到AdminUser以确保配置不会被随意更改";
            return true;
        }

        if (AdminUser.Contains(Main.player[whoAmI].name))
        {
            message = "修改成功,来自管理员:" + Main.player[whoAmI].name;
            return true;
        }

        return false;
    }
}