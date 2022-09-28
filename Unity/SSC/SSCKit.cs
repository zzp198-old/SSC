using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace QOS.Unity.SSC;

internal static class SSCKit
{
    internal static string SavePath => Path.Combine(Common.QOSKit.SavePath, "SSC");

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
}