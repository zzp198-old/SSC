using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class MainSystem : ModSystem
{
    internal static GameTime GameTime;
    internal static UserInterface UI;
    internal static MainLayout MainLayout;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
            MainLayout = new MainLayout();
        }
    }

    public override void Unload()
    {
        MainLayout = null;
        UI = null;
    }

    public override void UpdateUI(GameTime time)
    {
        GameTime = time;
        if (UI?.CurrentState != null)
        {
            UI?.Update(time);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: MainSystem", () =>
            {
                if (GameTime != null && UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, GameTime);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)SSC.ID.Clean);
            p.Write(Main.myPlayer);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
        }
    }
}