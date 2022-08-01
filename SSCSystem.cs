using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SSC.UI.State;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class SSCSystem : ModSystem
{
    internal static UserInterface UI;

    public override void Load()
    {
        UI = new UserInterface();
    }

    public override void Unload()
    {
        UI = null;
    }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NETCore.CS_ErasePLR(Main.myPlayer, SteamUser.GetSteamID().m_SteamID);
        }
    }

    public override void UpdateUI(GameTime time) => UI?.Update(time);

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: SSCSystem", delegate
                {
                    UI?.Draw(Main.spriteBatch, Main.gameTimeCache);
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}