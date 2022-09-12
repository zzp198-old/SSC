using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace QOS.Common.Systems;

public class SSCSystem : ModSystem
{
    internal static UserInterface UI;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return QOS.SC.SSC;
    }

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
        }

        IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData; // 加入世界时无视GameMode
        IL.Terraria.MessageBuffer.GetData += IL_MessageBuffer_GetData; // 初始化SSC
        IL.Terraria.Main.DrawInterface += IL_Main_DrawInterface; // 处于幽灵化时,隐藏资源和其他mod界面
        IL.Terraria.Main.DoUpdate_AutoSave += IL_Main_DoUpdate_AutoSave; // 缩短自动保存间隔
        IL.Terraria.Player.InternalSavePlayerFile += IL_Player_InternalSavePlayerFile; // 发送后缀为SSC的存档
        IL.Terraria.Player.KillMeForGood += IL_Player_KillMeForGood; // 硬核死亡删除云存档
    }

    public override void Unload()
    {
        UI = null;
        IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
        IL.Terraria.MessageBuffer.GetData -= IL_MessageBuffer_GetData;
        IL.Terraria.Main.DrawInterface -= IL_Main_DrawInterface;
        IL.Terraria.Main.DoUpdate_AutoSave -= IL_Main_DoUpdate_AutoSave;
        IL.Terraria.Player.InternalSavePlayerFile -= IL_Player_InternalSavePlayerFile;
        IL.Terraria.Player.KillMeForGood -= IL_Player_KillMeForGood;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (UI?.CurrentState != null)
        {
            UI.Update(gameTime);
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

    public override void NetSend(BinaryWriter bin)
    {
        var binary = new TagCompound();
        new DirectoryInfo(QOS.SSCSavePath).GetDirectories().ToList().ForEach(i =>
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
                    Utils.LogAndConsoleErrorMessage(e.ToString());
                }
            });
        });
        TagIO.ToStream(binary, bin.BaseStream);
    }

    public override void NetReceive(BinaryReader bin)
    {
        var tag = TagIO.FromStream(bin.BaseStream);
        if (QOS.My.ghost && UI.CurrentState == null)
        {
            UI.SetState(new Views.SSCView());
        }

        ((Views.SSCView)UI.CurrentState)?.FlushList(tag);
    }

    private void IL_NetMessage_SendData(ILContext il)
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

    private void IL_MessageBuffer_GetData(ILContext il)
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
                // 在接收到3之前,客户端已经Reload完成.后续不会再进行重加载导致Player被覆盖.
                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player
                    {
                        name = QOS.ClientID.ToString(), difficulty = i.ReadByte(),
                        statLife = 0, dead = true, ghost = true, immune = true, immuneTime = int.MaxValue,
                        savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                    }
                };
                data.MarkAsServerSide();
                data.SetAsActive();
            }
            else
            {
                i.ReadByte();
            }
        });
    }

    private void IL_Main_DrawInterface(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        c.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (QOS.My.ghost)
            {
                foreach (var layer in layers)
                {
                    layer.Active = layer.Name switch
                    {
                        _ => layer.Name.StartsWith("Vanilla")
                    };
                }
            }

            return layers;
        });
    }

    private void IL_Main_DoUpdate_AutoSave(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 30000);
    }

    private void IL_Player_InternalSavePlayerFile(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");

                QOSKit.InternalSavePlayer(new PlayerFileData(name, false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = data.Player
                });

                var binary = QOSKit.Plr2Byte(name);

                var mp = QOS.Mod.GetPacket();
                mp.Write((byte)QOS.ID.SaveSSC);
                mp.Write(QOS.ClientID);
                mp.Write(data.Player.name);
                mp.Write(binary.Length);
                mp.Write(binary);
                mp.Send();
            }
        });
    }

    private void IL_Player_KillMeForGood(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ActivePlayerFileData)));
        c.EmitDelegate<Func<PlayerFileData, PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                var mp = QOS.Mod.GetPacket();
                mp.Write((byte)QOS.ID.RemoveSSC);
                mp.Write(QOS.ClientID);
                mp.Write(data.Player.name);
                mp.Send();
                Main.ActivePlayerFileData = new PlayerFileData();
            }

            return data;
        });
    }
}