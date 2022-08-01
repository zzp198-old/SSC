using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;

namespace SSC;

public static class Utils
{
    public static void Boot(int id, string tip)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = tip;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(id, NetworkText.FromLiteral(tip));
                break;
            default:
                throw new Exception("Boot can only be used in C/S");
        }
    }

    public static Player ErasePLR(int whoAmI, string name, byte GameMode)
    {
        Main.player[whoAmI] = new Player
        {
            name = name, difficulty = GameMode,
            savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
        };

        if (whoAmI == Main.myPlayer)
        {
            var data = new PlayerFileData { Player = Main.player[whoAmI] };
            data.MarkAsServerSide();
            data.SetAsActive();
        }

        return Main.player[whoAmI];
    }

    public static List<string> GetPlayerList(ulong SteamID, string searchPattern)
    {
        var directory = Path.Combine(SSC.SavePath, SteamID.ToString());
        return Directory.GetFiles(directory, searchPattern).ToList();
    }
}