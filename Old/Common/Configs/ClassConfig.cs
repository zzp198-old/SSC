using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

public class ClassConfig : ModConfig
{
    public static ClassConfig Instance;

    public List<string> AllowMods = new();

    [DefaultValue(false)] [ReloadRequired]
    public bool SSC = false;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        if (SSC && Class.SSC.Configs.SSCConfig.Instance == null)
        {
            var type = typeof(Class.SSC.Configs.SSCConfig);
            Mod.AddConfig(type.Name, (ModConfig)Activator.CreateInstance(type, true));
        }
    }

    public override void OnChanged() // After ModConfig.Load()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            foreach (var mod in ModLoader.Mods.Where(mod => mod.Side is ModSide.Client or ModSide.NoSync))
            {
                if (AllowMods.Contains(mod.Name) || mod.Name == nameof(ModLoader))
                {
                    continue;
                }

                QOSKit.BootPlayer(Main.myPlayer, $"你被踢出了游戏,因为你启用了本地mod {mod.Name}\n" +
                                                 "如果你想正常游玩,请关闭此mod,或联系管理员.\n" +
                                                 $"将 {mod.Name} 添加到全局配置中的mod白名单里");
                return;
            }
        }
    }
}