using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SSC;

public partial class SSC
{
    public override void Load()
    {
        // 设置界面展示版本号
        IL.Terraria.IngameOptions.Draw += Hook1;
        // 客户端初次发送玩家数据前进行幽灵化
        IL.Terraria.NetMessage.SendData += Hook2;
        IL.Terraria.MessageBuffer.GetData += Hook3;
        // 新的欢迎公告
        IL.Terraria.NetMessage.greetPlayer += Hook4;
        // 缩短自动保存间隔
        IL.Terraria.Main.DoUpdate_AutoSave += Hook5;
        // 当触发保存时重定向至云存储
        IL.Terraria.Player.InternalSavePlayerFile += Hook6;
        // 接收到SystemData后,显示选择角色界面
        IL.Terraria.MessageBuffer.GetData += Hook7;
    }

    public override void Unload()
    {
        IL.Terraria.IngameOptions.Draw -= Hook1;
        IL.Terraria.NetMessage.SendData -= Hook2;
        IL.Terraria.MessageBuffer.GetData -= Hook3;
        IL.Terraria.NetMessage.greetPlayer -= Hook4;
        IL.Terraria.Main.DoUpdate_AutoSave -= Hook5;
        IL.Terraria.Player.InternalSavePlayerFile -= Hook6;
        IL.Terraria.MessageBuffer.GetData -= Hook7;
    }

    private static void Hook1(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdstr("GameUI.SettingsMenu"),
            i => i.MatchCall(typeof(Language), nameof(Language.GetTextValue))
        );
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC Ver.{SSC.Mod.Version.ToString()})");
    }

    private static void Hook2(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdloc(3),
            i => i.MatchLdarg(1),
            i => i.MatchConvU1(),
            i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)),
            i => i.MatchLdloc(3),
            i => i.MatchLdcI4(0),
            i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write))
        );
        c.Emit(OpCodes.Ldloc_3);
        c.EmitDelegate<Action<BinaryWriter>>(i => i.Write((byte)Main.GameMode));
    }

    private static void Hook3(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdarg(0),
            i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
            i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadByte)),
            i => i.MatchStloc(out _),
            i => i.MatchLdarg(0),
            i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
            i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadBoolean)),
            i => i.MatchStloc(out _)
        );
        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(MessageBuffer).GetField(nameof(MessageBuffer.reader)));
        c.EmitDelegate<Action<BinaryReader>>(i =>
        {
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{SteamUser.GetSteamID()}.SSC"), true)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = SSC.Sid.ToString(), difficulty = i.ReadByte(),
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198);
            data.MarkAsServerSide();
            data.SetAsActive();
        });
    }

    private static void Hook4(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<int>>(whoAmI =>
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(Main.motd switch
            {
                "" => $"{Lang.mp[18].Value} {Main.worldName}! \n云存档(SSC)已开启,祝您开荒愉快!",
                _ => Main.motd
            }), new Color(byte.MaxValue, 240, 20), whoAmI);
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(
                "Game.JoinGreeting",
                string.Join(", ", from i in Main.player.Where(i => i.active) select i.name)
            ), new Color(byte.MaxValue, 240, 20), whoAmI);
        });
        c.Emit(OpCodes.Ret);
    }

    private static void Hook5(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 30000);
    }

    private static void Hook6(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                FileUtilities.ProtectedInvoke(() =>
                {
                    // TODO
                });
            }
        });
    }

    private static void Hook7(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdsfld(typeof(Netplay), nameof(Netplay.Connection)),
            i => i.MatchLdfld(typeof(RemoteServer), nameof(RemoteServer.State)),
            i => i.MatchLdcI4(4),
            i => i.MatchBle(out _),
            i => i.MatchLdarg(0),
            i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
            i => i.MatchCall("Terraria.ModLoader.IO.WorldIO", "ReceiveModData")
        );
        c.EmitDelegate(() =>
        {
            if (Main.LocalPlayer.HasBuff<Content.Spooky>() && ModContent.GetInstance<SSCSyS>().UI.CurrentState == null)
            {
                ModContent.GetInstance<SSCSyS>().UI.SetState(new SSCView());
            }
        });
    }
}