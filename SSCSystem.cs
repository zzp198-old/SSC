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
    internal static SSCState SSCState;

    public override void Load()
    {
        UI = new UserInterface();
        SSCState = new SSCState();
    }

    public override void Unload()
    {
        SSCState = null;
        UI = null;
    }

    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var p = Mod.GetPacket();
            p.Write((byte)PID.CleanPLR);
            p.Write(Main.myPlayer);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Write(byte.MaxValue);
            p.Send();
        }
    }

    public override void UpdateUI(GameTime time) => UI?.Update(time);

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("SSC: SSCSystem", () =>
            {
                UI?.Draw(Main.spriteBatch, Main.gameTimeCache);
                return true;
            }, InterfaceScaleType.UI));
        }
    }
}