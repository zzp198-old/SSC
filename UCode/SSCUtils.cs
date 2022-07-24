// using System;
// using System.IO;
// using System.Security.Cryptography;
// using System.Text;
// using Terraria;
// using Terraria.IO;
// using Terraria.ModLoader.IO;
// using Terraria.Utilities;
//
// namespace SSC;
//
// public static class SSCUtils
// {
//     public static void SavePLR(string dir, string name, TagCompound compound)
//     {
//         if (!Directory.Exists(dir))
//         {
//             // 确定引用类型不为null,可添加!忽略
//             Directory.CreateDirectory(dir!);
//         }
//
//         var plr = Path.Combine(dir, $"{name}.plr");
//         if (compound.ContainsKey("PLR") && File.Exists(plr))
//         {
//             File.WriteAllBytes(plr, compound.GetByteArray("PLR"));
//         }
//
//         var tplr = Path.Combine(dir, $"{name}.tplr");
//         if (compound.ContainsKey("TPLR") && File.Exists(tplr))
//         {
//             File.WriteAllBytes(tplr, compound.GetByteArray("TPLR"));
//         }
//     }
//
//     public static TagCompound LoadPLR(string dir, string name)
//     {
//         var compound = new TagCompound();
//         var plr = Path.Combine(dir, $"{name}.plr");
//         if (File.Exists(plr))
//         {
//             compound.Set("PLR", File.ReadAllBytes(plr));
//         }
//
//         var tplr = Path.Combine(dir, $"{name}.tplr");
//         if (File.Exists(tplr))
//         {
//             compound.Set("TPLR", File.ReadAllBytes(tplr));
//         }
//
//         return compound;
//     }
// }