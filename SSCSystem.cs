using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class SSCSystem : ModSystem
{
    internal static UserInterface UI;
    internal static GameTime LastGameTime;
    internal static SSCLayout SSCLayout;

    public override void Load()
    {
        UI = new UserInterface();
        SSCLayout = new SSCLayout();
    }

    public override void Unload()
    {
        SSCLayout = null;
        UI = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        LastGameTime = gameTime;
        if (UI?.CurrentState != null)
        {
            UI.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: SSCSystem", () =>
            {
                if (LastGameTime != null && UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, LastGameTime);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var p = Mod.GetPacket();
            p.Write((byte)PID.SteamAccount);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
        }
    }
}