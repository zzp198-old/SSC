using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC.Common.Systems;

public class HookSystem : ModSystem
{
    public const string Base64 = @"[A-Za-z\d+/]*[=]{0,2}";
    private static readonly Regex Regex = new($@"^SSC:(?<plr>{Base64}):(?<tplr>{Base64}):\.(?<type>plr|tplr)$");

    public override void Load()
    {
        IL.Terraria.NetMessage.SendData += Hook1; // 加入世界时无视GameMode
        IL.Terraria.MessageBuffer.GetData += Hook2; // SSC初始化
        IL.Terraria.Main.DrawInterface += Hook3; // SSC界面下隐藏其他界面
        On.Terraria.Utilities.FileUtilities.Exists += Hook4; // 允许加载SSC特殊格式
        On.Terraria.Utilities.FileUtilities.ReadAllBytes += Hook5; // 允许加载SSC特殊格式
        IL.Terraria.Main.DoUpdate_AutoSave += Hook6; // 缩短自动保存间隔
        IL.Terraria.Player.InternalSavePlayerFile += Hook7; // 重定向保存位置
        On.Terraria.Player.KillMeForGood += Hook8; // 硬核死亡删除云存档
    }

    private static void Hook1(ILContext il)
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

    private static void Hook2(ILContext il)
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
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{SSCKit.ClientID}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = SSCKit.ClientID.ToString(), difficulty = mode,
                    // StatLift由16同步,Ghost由13同步.Dead由12和16计算得出.
                    statLife = 0, statMana = 0, dead = true, ghost = true,
                    // 避免因为AdjustRespawnTimerForWorldJoining导致Spawn时误修改Dead.不然尽管设置了Ghost,还是会被弹幕判断击杀并重置Dead.
                    respawnTimer = int.MaxValue, lastTimePlayerWasSaved = long.MaxValue,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.MarkAsServerSide();
            data.SetAsActive();
            ViewSystem.View.SetState(new Views.SSCView()); // 唯一设置,界面只会在此时启用
        });
    }

    private static void Hook3(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        ilCursor.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (ViewSystem.View.CurrentState != null)
            {
                layers.ForEach(layer => layer.Active = layer.Name switch
                {
                    "Vanilla: Map / Minimap" => false,
                    "Vanilla: Resource Bars" => false,
                    _ => layer.Name.StartsWith("Vanilla")
                });
            } // SSC界面下禁止其他操作,避免影响服务端同步和条件判断

            return layers;
        });
    }

    private static bool Hook4(On.Terraria.Utilities.FileUtilities.orig_Exists invoke, string name, bool _)
    {
        return Regex.Match(name).Success || invoke(name, _);
    }

    private static byte[] Hook5(On.Terraria.Utilities.FileUtilities.orig_ReadAllBytes invoke, string name, bool _)
    {
        var obj = Regex.Match(name);
        return obj.Success ? Convert.FromBase64String(obj.Groups[obj.Groups["type"].Value].Value) : invoke(name, _);
    }

    private static void Hook6(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        ilCursor.EmitDelegate<Func<long, long>>(_ => 60000);
    }

    private static void Hook7(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        // 放行[SSC].SSC
        ilCursor.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchCallvirt(typeof(PlayerFileData), "get_ServerSideCharacter"));
        ilCursor.Emit(OpCodes.Ldarg_0); // PlayerFileData
        ilCursor.EmitDelegate<Func<bool, PlayerFileData, bool>>((b, data) => b && !data.Path.EndsWith("SSC"));
        // 拦截[SSC].SSC并转发到云端
        ilCursor.GotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.IO.PlayerIO", "SaveData"));
        var label = ilCursor.DefineLabel();
        ilCursor.Emit(OpCodes.Ldarg_0); // PlayerFileData
        ilCursor.EmitDelegate<Func<PlayerFileData, bool>>(data => data.ServerSideCharacter && data.Path.EndsWith("SSC"));
        ilCursor.Emit(OpCodes.Brfalse, label);
        ilCursor.Emit(OpCodes.Ldloc_3); // array
        ilCursor.Emit(OpCodes.Ldarg_0); // PlayerFileData
        ilCursor.EmitDelegate<Action<TagCompound, byte[], PlayerFileData>>((obj, t, data) =>
        {
            // SSC, SSC.SSC     Create
            // SSC, 7818.SSC    Save
            var memory = new MemoryStream();
            TagIO.ToStream(obj, memory);
            var tml = memory.ToArray();

            var mp = ModContent.GetInstance<SSC>().GetPacket();
            mp.Write((byte)(data.Path.EndsWith("SSC.SSC") ? SSC.PID.CreateSSC : SSC.PID.SaveSSC));
            mp.Write(SSCKit.ClientID);
            mp.Write(data.Player.name);
            mp.Write(t.Length);
            mp.Write(t);
            mp.Write(tml.Length);
            mp.Write(tml);
            mp.Send();
        });
        ilCursor.Emit(OpCodes.Ret);
        ilCursor.MarkLabel(label);
    }

    private static void Hook8(On.Terraria.Player.orig_KillMeForGood invoke, Player self)
    {
        var data = Main.ActivePlayerFileData;
        if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
        {
            var mp = ModContent.GetInstance<SSC>().GetPacket();
            mp.Write((byte)SSC.PID.RemoveSSC);
            mp.Write(SSCKit.ClientID);
            mp.Write(data.Player.name);
            mp.Send();
        }

        invoke(self);
    }
}