using System;
using System.Collections.Generic;
using System.IO;
using IL.Terraria;
using MonoMod.Cil;
using SSC.Content;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

namespace SSC;

public class ILSystem : ModSystem
{
    public override void Load()
    {
        // 此时已经选择玩家,并对其他玩家进行了初始化.通过IL挂载可确保仅被执行一次.
        Netplay.InnerClientLoop += ILHook1;
        // 隐藏其他无关紧要的UI,防止因为提前操作导致数据丢失.
        Main.DrawInterface += ILHook2;
        // 显示SSC是否启用
        Main.DrawInventory += ILHook3;
        // 用于显示UI,但是不用TML提供的接口以免被重复调用.并在应用SSC后重复调用一遍TML的接口确保新角色数据正常.
        Player.Hooks.EnterWorld += ILHook4;
        // 每当多人模式客户端游戏中请求保存时触发(死亡,保存并退出).(其他mod请求此方法时也可触发保存,完美)
        Player.SavePlayer += ILHook5;
        // 缩短自动保存的时间间隔
        Main.DoUpdate_AutoSave += ILHook6;
        // 硬核死亡删除云存档
        Player.KillMeForGood += ILHook7;
    }

    public override void Unload()
    {
        Netplay.InnerClientLoop -= ILHook1;
        Main.DrawInterface -= ILHook2;
        Main.DrawInventory -= ILHook3;
        Player.Hooks.EnterWorld -= ILHook4;
        WorldGen.saveToonWhilePlayingCallBack -= ILHook5;
        Main.DoUpdate_AutoSave -= ILHook6;
        Player.KillMeForGood -= ILHook7;
    }

    private static void ILHook1(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(i => i.MatchLdstr("Net.FoundServer"));
        c.EmitDelegate(() =>
        {
            Terraria.Main.statusText = "SSC Hooking...";
            var data = new PlayerFileData(Path.Combine(SSC.SavePath(), $"{SSC.SteamID}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Terraria.Player
                {
                    name = SSC.SteamID.ToString(), difficulty = Terraria.Main.LocalPlayer.difficulty,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Terraria.Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.Player.AddBuff(ModContent.BuffType<Spooky>(), 198); // 幽灵化
            data.MarkAsServerSide();
            data.SetAsActive();
        });
    }

    private static void ILHook2(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        c.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (Terraria.Main.LocalPlayer.HasBuff<Spooky>())
                foreach (var layer in layers)
                    switch (layer.Name)
                    {
                        case "Vanilla: Map / Minimap":
                        case "Vanilla: Resource Bars":
                            layer.Active = false;
                            break;
                        default:
                            layer.Active = layer.Name.StartsWith("Vanilla");
                            break;
                    }

            return layers;
        });
    }

    private static void ILHook3(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCallvirt(typeof(LocalizedText), "get_Value"));
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC)");
    }

    private static void ILHook4(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(PlayerLoader), nameof(PlayerLoader.OnEnterWorld)));
        c.EmitDelegate(() =>
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient && Terraria.Main.LocalPlayer.HasBuff<Spooky>())
                UISystem.UI.SetState(UISystem.View);
        });
    }

    private static void ILHook5(ILContext il)
    {
        // 原方法会成功保存Map,剩下的内容由此Hook继续下去.
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(FileUtilities), nameof(FileUtilities.ProtectedInvoke)));
        c.EmitDelegate(() =>
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient && !Terraria.Main.LocalPlayer.HasBuff<Spooky>())
                SSCUtils.SendSSCBinary2Server(SSC.SteamID, Terraria.Main.LocalPlayer);
        });
    }

    private static void ILHook6(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 60000);
    }

    private static void ILHook7(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(i => i.MatchLdsfld(typeof(Terraria.Main), nameof(Terraria.Main.ActivePlayerFileData)));
        c.EmitDelegate(() =>
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var mp = SSCUtils.GetPacket(SSC.ID.RemoveSSC);
                mp.Write(SSC.SteamID);
                mp.Write(Terraria.Main.LocalPlayer.name);
                mp.Send();
            }
        });
    }
}