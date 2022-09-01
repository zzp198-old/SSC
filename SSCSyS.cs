using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

public partial class SSCSyS : ModSystem
{
    internal UserInterface UI;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
        }

        IL.Terraria.NetMessage.SendData += SendGameMode; // 发送GameMode
        IL.Terraria.MessageBuffer.GetData += ReceiveGameMode; // 接收GameMode并初始化SSC
        IL.Terraria.Main.DoUpdate_AutoSave += ShortAutoSaveInterval; // 缩短自动保存间隔
        IL.Terraria.Player.InternalSavePlayerFile += SendLocalSSC; // 发送后缀为SSC的存档
        IL.Terraria.IngameOptions.Draw += ShowVersion; // 设置界面展示版本号
        IL.Terraria.Main.DrawInterface += HideModeUI; // 处于幽灵化时,隐藏资源和其他mod界面
        IL.Terraria.Player.KillMeForGood += HardcoreDead; // 硬核死亡删除云存档
    }

    public override void Unload()
    {
        IL.Terraria.NetMessage.SendData -= SendGameMode;
        IL.Terraria.MessageBuffer.GetData -= ReceiveGameMode;
        IL.Terraria.Main.DoUpdate_AutoSave -= ShortAutoSaveInterval;
        IL.Terraria.Player.InternalSavePlayerFile -= SendLocalSSC;
        IL.Terraria.IngameOptions.Draw -= ShowVersion;
        IL.Terraria.Main.DrawInterface -= HideModeUI;
        IL.Terraria.Player.KillMeForGood -= HardcoreDead;
    }

    public override void UpdateUI(GameTime time)
    {
        if (UI?.CurrentState != null)
        {
            UI.Update(time);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("Vanilla: SSC", () =>
            {
                if (UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, Main.gameTimeCache);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    #region IL Patch

    private static void SendGameMode(ILContext il)
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

    private static void ReceiveGameMode(ILContext il)
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
            if (Netplay.Connection.State == 2)
            {
                var id = SteamUser.GetSteamID().m_SteamID;
                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{id}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player
                    {
                        name = id.ToString(), difficulty = i.ReadByte(),
                        savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                    }
                };
                data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198);
                data.MarkAsServerSide();
                data.SetAsActive();
            }
            else
            {
                i.ReadByte();
            }
        });
    }

    private static void ShortAutoSaveInterval(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 30000);
    }

    private static void SendLocalSSC(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var name = Path.Combine(Path.GetTempPath(), $"{SteamUser.GetSteamID().m_SteamID}.plr");

                SSCKit.InternalSavePlayer(new PlayerFileData(name, false) // 此时为Temp/[id].plr,不满足id.SSC,所以不会向云端发送
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = data.Player
                });

                // 压缩玩家为byte[],客户端同步SSC玩家
                var memoryStream = new MemoryStream();
                TagIO.ToStream(new TagCompound
                {
                    { "PLR", File.ReadAllBytes(name) },
                    { "TPLR", File.ReadAllBytes(Path.ChangeExtension(name, ".tplr")) },
                }, memoryStream);
                var array = memoryStream.ToArray();

                // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.SaveSSC);
                mp.Write(SteamUser.GetSteamID().m_SteamID);
                mp.Write(data.Player.name);
                mp.Write(array.Length);
                mp.Write(array);
                mp.Send();
            }
        });
    }

    private static void ShowVersion(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdstr("GameUI.SettingsMenu"),
            i => i.MatchCall(typeof(Language), nameof(Language.GetTextValue))
        );
        c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<string, string>>(i => $"{i} (SSC Ver.{SSC.Mod.Version.ToString()})");
    }

    private static void HideModeUI(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        c.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (Main.LocalPlayer.HasBuff<Content.Spooky>())
            {
                foreach (var layer in layers)
                {
                    layer.Active = layer.Name switch
                    {
                        "Vanilla: Resource Bars" => false,
                        _ => layer.Name.StartsWith("Vanilla")
                    };
                }
            }

            return layers;
        });
    }

    private static void HardcoreDead(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ActivePlayerFileData)));
        c.EmitDelegate<Func<PlayerFileData, PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.RemoveSSC);
                mp.Write(SteamUser.GetSteamID().m_SteamID);
                mp.Write(data.Player.name);
                mp.Send();
            }

            return data;
        });
    }

    #endregion

    public override void NetSend(BinaryWriter bin)
    {
        Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC"));
        var binary = new TagCompound();
        new DirectoryInfo(Path.Combine(Main.SavePath, "SSC")).GetDirectories().ToList().ForEach(i =>
        {
            binary.Set(i.Name, new List<TagCompound>());
            i.GetFiles("*.plr").ToList().ForEach(j =>
            {
                try
                {
                    var data = Player.LoadPlayer(j.FullName, false);
                    binary.Get<List<TagCompound>>(i.Name).Add(new TagCompound
                    {
                        { "name", data.Player.name },
                        { "difficulty", data.Player.difficulty },
                    });
                }
                catch (Exception e)
                {
                    Mod.Logger.Error(e);
                }
            });
        });
        TagIO.ToStream(binary, bin.BaseStream);
    }

    public override void NetReceive(BinaryReader bin)
    {
        var data = TagIO.FromStream(bin.BaseStream);

        if (Main.LocalPlayer.HasBuff<Content.Spooky>() && UI.CurrentState == null)
        {
            UI.SetState(new SSCView());
        }

        ((SSCView)UI.CurrentState)?.RedrawList(data);
    }
}