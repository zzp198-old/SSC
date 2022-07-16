using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SSC.Content.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC.Common.System;

public class UISystem : ModSystem
{
    public UserInterface UI;
    public UIState UIState;
    GameTime UIGameTime;

    public override void Load()
    {
        if (Main.dedServ) return;
        UI = new UserInterface();
        UIState = new SSC_GUI();
        UIState.Activate();
    }

    public override void Unload()
    {
        UIState = null;
        UI = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        UIGameTime = gameTime;
        if (UI?.CurrentState != null)
        {
            UI.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: UISystem", () =>
            {
                if (UIGameTime != null && UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, UIGameTime);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }
}