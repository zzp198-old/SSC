// using System;
// using System.IO;
// using MonoMod.Cil;
// using Terraria.ModLoader;
// using Terraria.ModLoader.IO;
// using Terraria.Utilities;
//
// namespace SSC;
//
// public class SSCSystem : ModSystem
// {
//     public override void Load()
//     {
//         IL.Terraria.Player.LoadPlayer += SupportByteArray;
//         On.Terraria.Utilities.FileUtilities.Exists += OnExists;
//         On.Terraria.Utilities.FileUtilities.ReadAllBytes += ReadAllBytes;
//     }
//
//     private bool OnExists(On.Terraria.Utilities.FileUtilities.orig_Exists orig, string path, bool cloud)
//     {
//         if (path.StartsWith(SSC.BASE64_KEY))
//         {
//             return true;
//         }
//
//         return orig(path, cloud);
//     }
//
//     private byte[] ReadAllBytes(On.Terraria.Utilities.FileUtilities.orig_ReadAllBytes orig, string path, bool cloud)
//     {
//         if (path.StartsWith(SSC.BASE64_KEY))
//         {
//             return Convert.FromBase64String(path[SSC.BASE64_KEY.Length..]);
//         }
//
//         return orig(path, cloud);
//     }
//
//     private void SupportByteArray(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(i => i.MatchCall(typeof(FileUtilities), nameof(FileUtilities.ReadAllBytes)));
//         c.Remove();
//         c.EmitDelegate<Func<string, bool, byte[]>>((data, _) =>
//         {
//             try
//             {
//                 var compound = TagIO.FromStream(new MemoryStream(Convert.FromBase64String(data)));
//                 return compound.GetByteArray("PLR");
//             }
//             catch (Exception e)
//             {
//                 return FileUtilities.ReadAllBytes(data, _);
//             }
//         });
//     }
//
//     public override void Unload()
//     {
//         IL.Terraria.Player.LoadPlayer -= SupportByteArray;
//         On.Terraria.Utilities.FileUtilities.Exists -= OnExists;
//         On.Terraria.Utilities.FileUtilities.ReadAllBytes -= ReadAllBytes;
//     }
// }