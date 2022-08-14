using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class UISystem : ModSystem
{
    internal static UserInterface UI;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
        }
    }

    public override void Unload()
    {
        UI = null;
    }

    public override void UpdateUI(GameTime time)
    {
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
            layers.Insert(index, new LegacyGameInterfaceLayer("Vanilla: SSC", () =>
            {
                if (UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, Main.gameTimeCache);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    public override void OnWorldLoad()
    {
        var mp = SSCUtils.GetPacket(SSC.ID.SSCInit);
        mp.Write(SSC.SteamID);
        mp.Send();
    }
}