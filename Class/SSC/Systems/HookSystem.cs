using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Chat;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace QOS.Class.SSC.Systems;

public class HookSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return Configs.SSCConfig.Instance != null;
    }

    public override void Load()
    {
        Debug.WriteLine($"{GetType()} Loading......");
        IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData; // 加入世界时无视GameMode
        IL.Terraria.MessageBuffer.GetData += IL_MessageBuffer_GetData; // SSC初始化
        IL.Terraria.Main.DrawInterface += IL_Main_DrawInterface; // SSC界面下,隐藏无关界面
        IL.Terraria.Main.DoUpdate_AutoSave += IL_Main_DoUpdate_AutoSave; // 缩短自动保存间隔
        IL.Terraria.Player.InternalSavePlayerFile += IL_Player_InternalSavePlayerFile; // 重定向保存位置
        IL.Terraria.Player.KillMeForGood += IL_Player_KillMeForGood; // 硬核死亡删除云存档
    }

    public override void Unload()
    {
        IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
        IL.Terraria.MessageBuffer.GetData -= IL_MessageBuffer_GetData;
        IL.Terraria.Main.DrawInterface -= IL_Main_DrawInterface;
        IL.Terraria.Main.DoUpdate_AutoSave -= IL_Main_DoUpdate_AutoSave;
        IL.Terraria.Player.InternalSavePlayerFile -= IL_Player_InternalSavePlayerFile;
        IL.Terraria.Player.KillMeForGood -= IL_Player_KillMeForGood;
    }

    private static void IL_NetMessage_SendData(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            i => i.MatchLdloc(3), i => i.MatchLdarg(1), i => i.MatchConvU1(),
            i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)),
            i => i.MatchLdloc(3), i => i.MatchLdcI4(0),
            i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write))
        );
        ilCursor.Emit(OpCodes.Ldloc_3);
        ilCursor.EmitDelegate<Action<BinaryWriter>>(i => i.Write((byte)Main.GameMode));
    }

    private static void IL_MessageBuffer_GetData(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            i => i.MatchLdarg(0),
            i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
            i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadByte)),
            i => i.MatchStloc(out _),
            i => i.MatchLdarg(0),
            i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
            i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadBoolean)),
            i => i.MatchStloc(out _)
        );
        ilCursor.Emit(OpCodes.Ldarg_0);
        ilCursor.Emit(OpCodes.Ldfld, typeof(MessageBuffer).GetField(nameof(MessageBuffer.reader)));
        ilCursor.EmitDelegate<Action<BinaryReader>>(i =>
        {
            var mode = i.ReadByte();
            if (Netplay.Connection.State != 2)
            {
                return;
            }

            // 在接收到3之前,客户端已经Reload完成.后续不会再进行重加载,ActivePlayerFileData会覆盖Player并修改whoAmI/myPlayer.
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = QOS.ClientID.ToString(), difficulty = mode,
                    // StatLift由16同步,Ghost由13同步.Dead由12和16计算得出.
                    statLife = 0, statMana = 0, dead = true, ghost = true,
                    // 避免因为AdjustRespawnTimerForWorldJoining导致Spawn时误修改Dead.不然尽管设置了Ghost,还是会被弹幕判断击杀并重置Dead.
                    respawnTimer = int.MaxValue, lastTimePlayerWasSaved = long.MaxValue,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.MarkAsServerSide();
            data.SetAsActive();
            UISystem.UI.SetState(new Views.SSCView()); // 唯一设置,界面只会在此时启用
        });
    }

    private static void IL_Main_DrawInterface(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        ilCursor.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (UISystem.UI.CurrentState != null) // SSC界面下禁止其他操作,避免影响服务端同步和条件判断
            {
                layers.ForEach(layer => layer.Active = layer.Name switch
                {
                    "Vanilla: Map / Minimap" => false,
                    "Vanilla: Resource Bars" => false,
                    // "Vanilla: Player Chat" => false,
                    _ => layer.Name.StartsWith("Vanilla")
                });
            }

            return layers;
        });
    }

    private static void IL_Main_DoUpdate_AutoSave(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        ilCursor.EmitDelegate<Func<long, long>>(_ => 30000); // 30秒保存间隔
    }

    private static void IL_Player_InternalSavePlayerFile(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        // 放行[SSC].SSC
        ilCursor.GotoNext(MoveType.After, i => i.MatchLdarg(0),
            i => i.MatchCallvirt(typeof(PlayerFileData), "get_ServerSideCharacter"));
        ilCursor.Emit(OpCodes.Ldarg_0); // PlayerFileData
        ilCursor.EmitDelegate<Func<bool, PlayerFileData, bool>>((b, data) => b && !data.Path.EndsWith("SSC"));
        // 拦截[SSC].SSC并转发到云端
        ilCursor.GotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.IO.PlayerIO", "SaveData"));
        var label = ilCursor.DefineLabel();
        ilCursor.Emit(OpCodes.Ldarg_0); // PlayerFileData
        ilCursor.EmitDelegate<Func<PlayerFileData, bool>>(data =>
            data.ServerSideCharacter && data.Path.EndsWith("SSC"));
        ilCursor.Emit(OpCodes.Brfalse, label);
        ilCursor.Emit(OpCodes.Ldloc_3); // array
        ilCursor.EmitDelegate<Action<TagCompound, byte[]>>((tml, t) =>
        {
            Console.WriteLine(tml);
            Console.WriteLine(t);
        });
        ilCursor.Emit(OpCodes.Ret);
        ilCursor.MarkLabel(label);
    }

    private static void IL_Player_KillMeForGood(ILContext il)
    {
        // var c = new ILCursor(il);
        // c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ActivePlayerFileData)));
        // c.EmitDelegate<Func<PlayerFileData, PlayerFileData>>(data =>
        // {
        //     if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
        //     {
        //         // var mp = QOS.Mod.GetPacket();
        //         // mp.Write((byte)QOS.ID.RemoveSSC);
        //         // mp.Write(QOS.ClientID);
        //         // mp.Write(data.Player.name);
        //         // mp.Send();
        //     }
        //
        //     return data;
        // });
    }

    // public static void adfsadf()
    // {
    //     Player.SavePlayer();
    //
    //     var uiCharacterCreation = new UICharacterCreation(new Player());
    //     var invoke =  typeof(UICharacterCreation).GetMethod("SetupPlayerStatsAndInventoryBasedOnDifficulty", (BindingFlags)36);
    //     var dummy =  (Player)typeof(UICharacterCreation).GetField("_player", (BindingFlags)36)?.GetValue(uiCharacterCreation);
    //     invoke?.Invoke(uiCharacterCreation, System.Array.Empty<object>());
    //     dummy.name = "123";
    //     dummy.difficulty = 1;
    // }
}