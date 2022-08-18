using System.IO;
using MonoMod.Cil;
using Steamworks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC.Common;

public class SpookyHook : ILoadable
{
    public void Load(Mod mod) => IL.Terraria.MessageBuffer.GetData += Hook;
    public void Unload() => IL.Terraria.MessageBuffer.GetData -= Hook;

    private static void Hook(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(
            i => i.MatchLdsfld(typeof(Netplay), nameof(Netplay.Connection)),
            i => i.MatchLdcI4(2),
            i => i.MatchStfld(typeof(RemoteServer), nameof(RemoteServer.State))
        );
        c.EmitDelegate(() =>
        {
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{SteamUser.GetSteamID()}.plr"), true)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = SteamUser.GetSteamID().ToString(),
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.MarkAsServerSide();
            data.Player.difficulty = Main.LocalPlayer.difficulty;
            data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198);
            data.SetAsActive();
        });
    }
}