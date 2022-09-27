using System;
using System.Text.RegularExpressions;
using Terraria.ModLoader;

namespace QOS.Unity.SSC.Systems;

public class HookSystem : ModSystem
{
    private static readonly Regex Regex =
        new(@"^SSC:(?<plr>[A-Za-z\d+/]*[=]{0,2}):(?<tplr>[A-Za-z\d+/]*[=]{0,2}):\.(?<type>plr|tplr)$");

    public override void Load()
    {
        On.Terraria.Utilities.FileUtilities.Exists += On_FileUtilities_Exists;
        On.Terraria.Utilities.FileUtilities.ReadAllBytes += On_FileUtilities_ReadAllBytes;
    }

    private static bool On_FileUtilities_Exists(On.Terraria.Utilities.FileUtilities.orig_Exists invoke, string name,
        bool _)
    {
        return Regex.Match(name).Success || invoke(name, _);
    }

    private static byte[] On_FileUtilities_ReadAllBytes(On.Terraria.Utilities.FileUtilities.orig_ReadAllBytes invoke,
        string name, bool _)
    {
        var obj = Regex.Match(name);
        return obj.Success ? Convert.FromBase64String(obj.Groups[obj.Groups["type"].Value].Value) : invoke(name, _);
    }
}