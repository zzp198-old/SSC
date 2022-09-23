using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace QOS.Common.Configs;

public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

 
    public SSCProperty SSCProperty = new();

    [SeparatePage]
    public List<ItemDefinition> Test;
}

public class SSCProperty
{
    [JsonIgnore]
    [Label("云存档")]
    [Tooltip("因为设计原因,云存档的开启会让工作量成倍增加,不得不禁止修改")]
    public bool SSC => true;

    [Label("初始血量")] [Tooltip("")] [Slider, Range(1, 500), DefaultValue(100)]
    public int InitLife = 100;

    [Label("初始魔力")] [Tooltip("")] [Slider, Range(0, 200), DefaultValue(20)]
    public int InitMana = 20;

    [Label("保存到world里")] [Tooltip("")] [Slider, Range(0, 200), DefaultValue(20)]
    public bool EveryWorld;

    [Label("展示提示信息")] [Tooltip("")] [Slider, Range(0, 200), DefaultValue(20)]
    public bool ShowMessage;

    public override string ToString()
    {
        return SSC ? "启用" : "禁用";
    }
}