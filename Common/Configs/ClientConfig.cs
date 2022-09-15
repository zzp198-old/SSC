using System.ComponentModel;
using Terraria.Enums;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

[Label("$Mods.QOS.Config.ClientConfig")]
public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("$Mods.QOS.Config.Smooth.Label"),
     Tooltip("$Mods.QOS.Config.Smooth.Tooltip")]
    [DefaultValue(false)]
    public bool Smooth;

    [Label("$Mods.QOS.Config.JoinTeam.Label"),
     Tooltip("$Mods.QOS.Config.JoinTeam.Tooltip")]
    [DefaultValue(Team.Red)]
    public Team JoinTeam;
}