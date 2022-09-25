using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace QOS.Class.SSC;

public static class SSCKit
{
    internal static string DifficultyTextValue(byte difficulty)
    {
        return Language.GetTextValue(difficulty switch
        {
            0 => "UI.Softcore",
            1 => "UI.Mediumcore",
            2 => "UI.Hardcore",
            3 => "UI.Creative",
            _ => "Unknown"
        });
    }

    internal static Color DifficultyTextColor(byte difficulty)
    {
        return difficulty switch
        {
            1 => Main.mcColor,
            2 => Main.hcColor,
            3 => Main.creativeModeColor,
            _ => Color.White
        };
    }

    internal static string SavePath(bool world = false, ulong id = 0, string name = "")
    {
        var path = Path.Combine(QOS.SavePath, "SSC");
        if (world) path = Path.Combine(path, Main.ActiveWorldFileData.UniqueId.ToString());

        if (id != 0) path = Path.Combine(path, id.ToString());

        if (name != "") path = Path.Combine(path, name);

        return path;
    }
}