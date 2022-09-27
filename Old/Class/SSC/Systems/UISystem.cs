using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using QOS.Class.SSC.Configs;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace QOS.Class.SSC.Systems;

public class UISystem : ModSystem
{
    internal static UserInterface UI;

    public override void Load()
    {
        if (!Main.dedServ) UI = new UserInterface();
    }

    public override void Unload()
    {
        UI = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (UI?.CurrentState != null) UI.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
            layers.Insert(index, new LegacyGameInterfaceLayer("Vanilla: SSC", () =>
            {
                if (UI?.CurrentState != null) UI.Draw(Main.spriteBatch, Main.gameTimeCache);

                return true;
            }, InterfaceScaleType.UI));
    }
}