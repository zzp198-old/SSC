using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using QOS.Class.SSC.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

[Label("全局配置")]
public class ClassConfig : ModConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static ClassConfig Instance;

    [Label("本地模组白名单")] //
    [Tooltip("阿巴阿巴......")]
    public List<string> AllowMods = new();

    [Label("云存档")] //
    [Tooltip("阿巴阿巴......")]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool SSC = false;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override bool Autoload(ref string name)
    {
        return true;
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

    internal void DynamicModify()
    {
        if (SSC && SSCConfig.Instance == null)
        {
            var type = typeof(SSCConfig);
            Mod.AddConfig(type.Name, (ModConfig)Activator.CreateInstance(type, true));
        }
    }
}