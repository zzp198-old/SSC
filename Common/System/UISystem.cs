using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SSC.Common.System;

public class UISystem : ModSystem
{
    public Terraria.UI.UserInterface UI;
    public Terraria.UI.UIState UIState;
    GameTime UIGameTime;

    public override void Load()
    {
        if (Main.dedServ) return;
        UI = new Terraria.UI.UserInterface();
        UIState = new Content.UI.UIState();
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

    public override void ModifyInterfaceLayers(List<Terraria.UI.GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new Terraria.UI.LegacyGameInterfaceLayer("SSC: UISystem", () =>
            {
                if (UIGameTime != null && UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, UIGameTime);
                }

                return true;
            }, Terraria.UI.InterfaceScaleType.UI));
        }
    }
}