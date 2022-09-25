// using System.ComponentModel;
// using System.IO;
// using System.Linq;
// using Terraria;
// using Terraria.Enums;
// using Terraria.Localization;
// using Terraria.ModLoader.Config;
//
// namespace QOS.Common.Configs;
//
// [Label("$Mods.QOS.Config.ServerConfig")]
// public class ServerConfig : ModConfig
// {
//     public override ConfigScope Mode => ConfigScope.ServerSide;
//
//     [SeparatePage]
//     [Label("$Mods.QOS.Config.StartItemConfig.Label"),
//      Tooltip("$Mods.QOS.Config.StartItemConfig.Tooltip")]
//     public StartItemConfig StartItemConfig = new();
//
//     [SeparatePage]
//     [Label("$Mods.QOS.Config.SSCConfig.Label"),
//      Tooltip("$Mods.QOS.Config.SSCConfig.Tooltip")]
//     public SSCConfig SSCConfig = new();
//
//
//     [Label("$Mods.QOS.Config.ReviveSeal.Label"),
//      Tooltip("$Mods.QOS.Config.ReviveSeal.Tooltip")]
//     [DefaultValue(false)]
//     public bool ReviveSeal = false;
//
//     [Label("$Mods.QOS.Config.Observer.Label"),
//      Tooltip("$Mods.QOS.Config.Observer.Tooltip")]
//     [DefaultValue(false)]
//     public bool Observer = false;
//
//     [Label("$Mods.QOS.Config.ReinforcedTile.Label"),
//      Tooltip("$Mods.QOS.Config.ReinforcedTile.Tooltip")]
//     [DefaultValue(false)]
//     public bool ReinforcedTile = false;
//
//     [Label("$Mods.QOS.Config.CreativeShock.Label"),
//      Tooltip("$Mods.QOS.Config.CreativeShock.Tooltip")]
//     [DefaultValue(false)]
//     public bool CreativeShock = false;
//
//     [Label("$Mods.QOS.Config.AutoJoinTeam.Label"),
//      Tooltip("$Mods.QOS.Config.AutoJoinTeam.Tooltip")]
//     [DefaultValue(Team.None)]
//     public Team AutoJoinTeam = Team.None;
//
//     [Label("$Mods.QOS.Config.ForceHostile.Label"),
//      Tooltip("$Mods.QOS.Config.ForceHostile.Tooltip")]
//     [DefaultValue(false)]
//     public bool ForceHostile = false;
//
//     public override bool AcceptClientChanges(ModConfig obj, int i, ref string message)
//     {
//         var admin = File.ReadLines(QOS.AdminSavePath).Select(name => name.Trim());
//         if (admin.Any(name => name == Main.player[i].name))
//         {
//             return true;
//         }
//
//         message = Language.GetTextValue("Mods.QOS.Config.RejectClientChange");
//         return false;
//     }
//
//     public override bool NeedsReload(ModConfig _)
//     {
//         if (_ is not ServerConfig obj)
//         {
//             return true;
//         }
//
//         if (SSCConfig.SSC != obj.SSCConfig.SSC)
//         {
//             return true;
//         }
//
//         if (ReviveSeal != obj.ReviveSeal)
//         {
//             return true;
//         }
//
//         if (Observer != obj.Observer)
//         {
//             return true;
//         }
//
//         return false;
//     }
// }

