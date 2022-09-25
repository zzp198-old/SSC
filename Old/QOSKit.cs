// using System;
// using System.IO;
// using System.Reflection;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.Localization;
// using Terraria.ModLoader.IO;
// using Terraria.Utilities;
//
// namespace QOS;
//
// public static class QOSKit
// {
//     public static void Boot(int plr, string msg)
//     {
//         switch (Main.netMode)
//         {
//             case NetmodeID.MultiplayerClient:
//                 Netplay.Disconnect = true;
//                 Main.statusText = msg;
//                 Main.menuMode = MenuID.MultiplayerJoining;
//                 break;
//             case NetmodeID.Server:
//                 NetMessage.BootPlayer(plr, NetworkText.FromLiteral(msg));
//                 break;
//             default:
//                 throw new Exception("Boot can only be used in C/S.");
//         }
//     }
//
//     public static void InternalSavePlayer(PlayerFileData data)
//     {
//         var invoke = typeof(Player).GetMethod("InternalSavePlayerFile", (BindingFlags)40);
//         FileUtilities.ProtectedInvoke(() => invoke?.Invoke(null, new object[] { data }));
//     }
//
//     public static byte[] Plr2Byte(string name)
//     {
//         var tag = new TagCompound { { "plr", File.ReadAllBytes(name) } };
//         if (File.Exists(Path.ChangeExtension(name, ".tplr")))
//         {
//             tag.Set("tplr", File.ReadAllBytes(Path.ChangeExtension(name, ".tplr")));
//         }
//
//         var memory = new MemoryStream();
//         TagIO.ToStream(tag, memory);
//         return memory.ToArray();
//     }
//
//     public static void Byte2Plr(byte[] data, string name)
//     {
//         var tag = TagIO.FromStream(new MemoryStream(data));
//         File.WriteAllBytes(name, tag.GetByteArray("plr"));
//         if (tag.ContainsKey("tplr"))
//         {
//             File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), tag.GetByteArray("tplr"));
//         }
//     }
//

// }

