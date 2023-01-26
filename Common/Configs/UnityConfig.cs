using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Terraria;
using Terraria.ModLoader.Config;

namespace SSC.Common.Configs;

public class UnityConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public List<string> AdminUser = new()
    {
        "Admin"
    };

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
    {
        if (AdminUser.Count == 0)
        {
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