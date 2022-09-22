// using System.IO;
// using Steamworks;
// using Terraria;
// using Terraria.ModLoader;
//
// namespace QOS;
//
// public partial class QOS : Mod
// {
//     internal static string QOSSavePath => Path.Combine(Main.SavePath, nameof(QOS));
//     internal static string AdminSavePath => Path.Combine(QOSSavePath, "Admin.txt");
//     internal static Mod Mod => ModContent.GetInstance<QOS>();
//     internal static Common.Configs.ClientConfig CC => ModContent.GetInstance<Common.Configs.ClientConfig>();
//     internal static Common.Configs.ServerConfig SC => ModContent.GetInstance<Common.Configs.ServerConfig>();
//     internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;
//
//     public override void Load()
//     {
//         if (Main.dedServ)
//         {
//             Utils.TryCreatingDirectory(QOSSavePath);
//             if (!File.Exists(AdminSavePath))
//             {
//                 File.CreateText(AdminSavePath).Close();
//             }
//         }
//     }
//
//     public override void HandlePacket(BinaryReader bin, int plr)
//     {
//         switch ((ID)bin.ReadByte())
//         {
//             case ID.CreateSSC:
//                 ModContent.GetInstance<Common.Systems.SSCSystem>().HandlePacket_CreateSSC(bin, plr);
//                 break;
//             case ID.RemoveSSC:
//                 ModContent.GetInstance<Common.Systems.SSCSystem>().HandlePacket_RemoveSSC(bin, plr);
//                 break;
//             case ID.ChooseSSC:
//                 ModContent.GetInstance<Common.Systems.SSCSystem>().HandlePacket_ChooseSSC(bin, plr);
//                 break;
//             case ID.LoadSSC:
//                 ModContent.GetInstance<Common.Systems.SSCSystem>().HandlePacket_LoadSSC(bin, plr);
//                 break;
//             case ID.SaveSSC:
//                 ModContent.GetInstance<Common.Systems.SSCSystem>().HandlePacket_SaveSSC(bin, plr);
//                 break;
//             default:
//                 QOSKit.Boot(plr, "Invalid Package ID");
//                 break;
//         }
//     }
// }