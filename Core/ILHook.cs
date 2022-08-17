using System.IO;
using MonoMod.Cil;
using Steamworks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC.Core;

public class ILHook : ModSystem
{
    public override void Load()
    {
        IL.Terraria.MessageBuffer.GetData += InitPlayer;
    }

    public override void Unload()
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
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = SteamUser.GetSteamID().ToString(),
                    difficulty = Main.LocalPlayer.difficulty,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules(),
                },
            };
            data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198);
            data.MarkAsServerSide();
            data.SetAsActive();
        });
    }
}