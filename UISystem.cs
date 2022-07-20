using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class UISystem : ModSystem
{
    public UserInterface UI;
    public UIState SSCUI;
    GameTime UITime;

    public override void Load()
    {
        if (Main.dedServ) return;
        UI = new UserInterface();
        SSCUI = new SSCUI();
    }

    public override void Unload()
    {
        SSCUI = null;
        UI = null;
    }

    #region UI

    public override void UpdateUI(GameTime time)
    {
        UITime = time;
        if (UI?.CurrentState != null)
        {
            UI.Update(time);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: UISystem", () =>
            {
                if (UITime != null && UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, UITime);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    #endregion
}