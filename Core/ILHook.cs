using System.IO;
using MonoMod.Cil;
using Steamworks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC.Core;

public class ILHook : ILoadable
{
    public void Load(Mod mod)
    {
        IL.Terraria.MessageBuffer.GetData += InitPlayer;
    }

    public void Unload()
    {
        IL.Terraria.MessageBuffer.GetData -= InitPlayer;
    }

    private static void InitPlayer(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(
            i => i.MatchLdsfld<Netplay>(nameof(Netplay.Connection)),
            i => i.MatchLdcI4(2),
            i => i.MatchStfld<RemoteServer>(nameof(RemoteServer.State))
        );
        c.EmitDelegate(() =>
        {
            var data = Misc.NewSSC(SteamUser.GetSteamID().ToString());
            data.Player.difficulty = Main.LocalPlayer.difficulty;
            data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198);
            data.SetAsActive();
        });
    }
}