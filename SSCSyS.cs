using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

public class SSCSyS : ModSystem
{
    internal static UserInterface UI;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
        }

        IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData; // 发送GameMode
        IL.Terraria.MessageBuffer.GetData += IL_MessageBuffer_GetData; // 接收GameMode并初始化SSC
        IL.Terraria.Main.DoUpdate_AutoSave += IL_Main_DoUpdate_AutoSave; // 缩短自动保存间隔
        IL.Terraria.Player.InternalSavePlayerFile += IL_Player_InternalSavePlayerFile; // 发送后缀为SSC的存档
        IL.Terraria.IngameOptions.Draw += IL_IngameOptions_Draw; // 设置界面展示版本号
        IL.Terraria.Main.DrawInterface += IL_Main_DrawInterface; // 处于幽灵化时,隐藏资源和其他mod界面
        IL.Terraria.Player.KillMeForGood += IL_Player_KillMeForGood; // 硬核死亡删除云存档
    }

    public override void Unload()
    {
        UI = null;

        IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
        IL.Terraria.MessageBuffer.GetData -= IL_MessageBuffer_GetData;
        IL.Terraria.Main.DoUpdate_AutoSave -= IL_Main_DoUpdate_AutoSave;
        IL.Terraria.Player.InternalSavePlayerFile -= IL_Player_InternalSavePlayerFile;
        IL.Terraria.IngameOptions.Draw -= IL_IngameOptions_Draw;
        IL.Terraria.Main.DrawInterface -= IL_Main_DrawInterface;
        IL.Terraria.Player.KillMeForGood -= IL_Player_KillMeForGood;
    }

    #region IL Patch

    private static void IL_NetMessage_SendData(ILContext il)
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

    private static void IL_MessageBuffer_GetData(ILContext il)
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
                // 检测本地是否有非法mod
                foreach (var mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
                {
                    if (mod.Side is ModSide.Client or ModSide.NoSync)
                    {
                        if (!ModContent.GetInstance<SSCSet>().LocalModWhiteList.Contains(mod.Name))
                        {
                            SSCKit.Boot(0, $"{mod.Name} mod is not allowed by the server.");
                            return;
                        }
                    }
                }

                // 在发送3之前,客户端已经Reload并重加载Player,ClientID.plr并不会被实用.
                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{SSC.ClientID}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player
                    {
                        name = SSC.ClientID.ToString(), difficulty = i.ReadByte(),
                        savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                    }
                };
                data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198); // 只有这里会设置Spooky状态
                data.MarkAsServerSide();
                data.SetAsActive();
            }
            else
            {
                i.ReadByte();
            }
        });
    }

    private static void IL_Main_DoUpdate_AutoSave(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 30000);
    }

    private static void IL_Player_InternalSavePlayerFile(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");

                // 此时为Temp/[Time].plr,不满足.SSC,所以只会保存到本地,不会向云端发送
                SSCKit.InternalSavePlayer(new PlayerFileData(name, false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = data.Player
                });

                var bytes = SSCKit.Plr2Byte(name);

                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.SaveSSC);
                mp.Write(SSC.ClientID);
                mp.Write(data.Player.name);
                mp.Write(bytes.Length);
                mp.Write(bytes);
                mp.Send();
            }
        });
    }

    private static void IL_IngameOptions_Draw(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdstr("GameUI.SettingsMenu"),
            i => i.MatchCall(typeof(Language), nameof(Language.GetTextValue))
        );
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC Ver.{SSC.Mod.Version.ToString()})");
    }

    private static void IL_Main_DrawInterface(ILContext il)
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

    private static void IL_Player_KillMeForGood(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ActivePlayerFileData)));
        c.EmitDelegate<Func<PlayerFileData, PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.RemoveSSC);
                mp.Write(SSC.ClientID);
                mp.Write(data.Player.name);
                mp.Send();
                Main.ActivePlayerFileData = new PlayerFileData();
            }

            return data;
        });
    }

    #endregion

    #region Fixed UI

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

    #endregion

    #region Net Sync

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

    #endregion
}