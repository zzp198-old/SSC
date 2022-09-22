// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using Mono.Cecil.Cil;
// using MonoMod.Cil;
// using Terraria;
// using Terraria.Chat;
// using Terraria.GameContent.Creative;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.Localization;
// using Terraria.ModLoader;
// using Terraria.ModLoader.IO;
// using Terraria.UI;
//
// namespace QOS.Common.Systems;
//
// public class SSCSystem : ModSystem
// {
//     internal static string SSCSavePath => Path.Combine(QOS.QOSSavePath, "SSC");
//     internal static UserInterface UI;
//
//     public override bool IsLoadingEnabled(Mod mod)
//     {
//         return QOS.SC.SSCConfig.SSC;
//     }
//
//     public override void Load()
//     {
//         if (!Main.dedServ)
//         {
//             UI = new UserInterface();
//         }
//         else
//         {
//             Utils.TryCreatingDirectory(SSCSavePath);
//         }
//
//         IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData; // 加入世界时无视GameMode
//         IL.Terraria.MessageBuffer.GetData += IL_MessageBuffer_GetData; // 初始化SSC
//         IL.Terraria.Main.DrawInterface += IL_Main_DrawInterface; // 处于幽灵化时,隐藏资源和其他mod界面
//         IL.Terraria.Main.DoUpdate_AutoSave += IL_Main_DoUpdate_AutoSave; // 缩短自动保存间隔
//         IL.Terraria.Player.InternalSavePlayerFile += IL_Player_InternalSavePlayerFile; // 发送后缀为SSC的存档
//         IL.Terraria.Player.KillMeForGood += IL_Player_KillMeForGood; // 硬核死亡删除云存档
//     }
//
//     public override void Unload()
//     {
//         UI = null;
//         IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
//         IL.Terraria.MessageBuffer.GetData -= IL_MessageBuffer_GetData;
//         IL.Terraria.Main.DrawInterface -= IL_Main_DrawInterface;
//         IL.Terraria.Main.DoUpdate_AutoSave -= IL_Main_DoUpdate_AutoSave;
//         IL.Terraria.Player.InternalSavePlayerFile -= IL_Player_InternalSavePlayerFile;
//         IL.Terraria.Player.KillMeForGood -= IL_Player_KillMeForGood;
//     }
//
//     public override void UpdateUI(GameTime gameTime)
//     {
//         if (UI?.CurrentState != null)
//         {
//             UI.Update(gameTime);
//         }
//     }
//
//     public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
//     {
//         var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
//         if (index != -1)
//         {
//             layers.Insert(index, new LegacyGameInterfaceLayer("Vanilla: SSC", () =>
//             {
//                 if (UI?.CurrentState != null)
//                 {
//                     UI.Draw(Main.spriteBatch, Main.gameTimeCache);
//                 }
//
//                 return true;
//             }, InterfaceScaleType.UI));
//         }
//     }
//
//     public override void NetSend(BinaryWriter bin)
//     {
//         var binary = new TagCompound();
//         new DirectoryInfo(SSCSavePath).GetDirectories().ToList().ForEach(i =>
//         {
//             binary.Set(i.Name, new List<TagCompound>());
//             i.GetFiles("*.plr").ToList().ForEach(j =>
//             {
//                 try
//                 {
//                     var data = Player.LoadPlayer(j.FullName, false);
//                     binary.Get<List<TagCompound>>(i.Name).Add(new TagCompound
//                     {
//                         { "name", data.Player.name },
//                         { "difficulty", data.Player.difficulty }
//                     });
//                 }
//                 catch (Exception e)
//                 {
//                     Mod.Logger.Error(e);
//                 }
//             });
//         });
//         TagIO.ToStream(binary, bin.BaseStream);
//     }
//
//     public override void NetReceive(BinaryReader bin)
//     {
//         var tag = TagIO.FromStream(bin.BaseStream);
//         if (Main.LocalPlayer.ghost && UI.CurrentState == null)
//         {
//             UI.SetState(new Views.SSCView());
//         }
//
//         ((Views.SSCView)UI.CurrentState)?.FlushList(tag);
//     }
//
//     private void IL_NetMessage_SendData(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After,
//             i => i.MatchLdloc(3),
//             i => i.MatchLdarg(1),
//             i => i.MatchConvU1(),
//             i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)),
//             i => i.MatchLdloc(3),
//             i => i.MatchLdcI4(0),
//             i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write))
//         );
//         c.Emit(OpCodes.Ldloc_3);
//         c.EmitDelegate<Action<BinaryWriter>>(i => i.Write((byte)Main.GameMode));
//     }
//
//     private void IL_MessageBuffer_GetData(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After,
//             i => i.MatchLdarg(0),
//             i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
//             i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadByte)),
//             i => i.MatchStloc(out _),
//             i => i.MatchLdarg(0),
//             i => i.MatchLdfld(typeof(MessageBuffer), nameof(MessageBuffer.reader)),
//             i => i.MatchCallvirt(typeof(BinaryReader), nameof(BinaryReader.ReadBoolean)),
//             i => i.MatchStloc(out _)
//         );
//         c.Emit(OpCodes.Ldarg_0);
//         c.Emit(OpCodes.Ldfld, typeof(MessageBuffer).GetField(nameof(MessageBuffer.reader)));
//         c.EmitDelegate<Action<BinaryReader>>(i =>
//         {
//             var mode = i.ReadByte();
//             if (Netplay.Connection.State == 2)
//             {
//                 // 在接收到3之前,客户端已经Reload完成.后续不会再进行重加载,ActivePlayerFileData会覆盖Player并修改whoAmI/myPlayer.
//                 var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.plr"), false)
//                 {
//                     Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
//                     Player = new Player
//                     {
//                         name = QOS.ClientID.ToString(), difficulty = mode,
//                         // StatLift由16同步,Ghost由13同步.Dead由12和16计算得出.
//                         statLife = 0, dead = true, ghost = true,
//                         // 避免因为AdjustRespawnTimerForWorldJoining导致Spawn时误修改Dead.不然尽管设置了Ghost,还是会被弹幕判断击杀并重置Dead.
//                         respawnTimer = int.MaxValue, lastTimePlayerWasSaved = long.MaxValue,
//                         savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
//                     }
//                 };
//                 data.MarkAsServerSide();
//                 data.SetAsActive();
//             }
//         });
//     }
//
//     private void IL_Main_DrawInterface(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
//         c.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
//         {
//             if (Main.LocalPlayer.ghost) // 幽灵模式下禁止其他操作,影响服务端同步和条件判断
//             {
//                 layers.ForEach(layer => layer.Active = layer.Name switch
//                 {
//                     _ => layer.Name.StartsWith("Vanilla")
//                 });
//             }
//
//             return layers;
//         });
//     }
//
//     private void IL_Main_DoUpdate_AutoSave(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
//         c.EmitDelegate<Func<long, long>>(_ => 30000); // 30秒保存间隔
//     }
//
//     private void IL_Player_InternalSavePlayerFile(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.Emit(OpCodes.Ldarg_0);
//         c.EmitDelegate<Action<PlayerFileData>>(data =>
//         {
//             if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
//             {
//                 var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");
//
//                 QOSKit.InternalSavePlayer(new PlayerFileData(name, false)
//                 {
//                     Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
//                     Player = data.Player
//                 });
//                 var binary = QOSKit.Plr2Byte(name);
//
//                 var mp = QOS.Mod.GetPacket();
//                 mp.Write((byte)QOS.ID.SaveSSC);
//                 mp.Write(QOS.ClientID);
//                 mp.Write(data.Player.name);
//                 mp.Write(binary.Length);
//                 mp.Write(binary);
//                 mp.Send();
//             }
//         });
//     }
//
//     private void IL_Player_KillMeForGood(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ActivePlayerFileData)));
//         c.EmitDelegate<Func<PlayerFileData, PlayerFileData>>(data =>
//         {
//             if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
//             {
//                 var mp = QOS.Mod.GetPacket();
//                 mp.Write((byte)QOS.ID.RemoveSSC);
//                 mp.Write(QOS.ClientID);
//                 mp.Write(data.Player.name);
//                 mp.Send();
//
//                 data = new PlayerFileData();
//                 data.MarkAsServerSide();
//             }
//
//             return data;
//         });
//     }
//
//     public void HandlePacket_CreateSSC(BinaryReader bin, int plr)
//     {
//         var id = bin.ReadUInt64();
//         var name = bin.ReadString();
//         var difficulty = bin.ReadByte();
//
//         if (string.IsNullOrWhiteSpace(name))
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, plr);
//             return;
//         }
//
//         if (name.Length > Player.nameLen)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, plr);
//             return;
//         }
//
//         if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.InvalidFileName"), Color.Red, plr);
//             return;
//         }
//
//         if (Directory.GetFiles(SSCSavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0) // 防止同名注册
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
//             return;
//         }
//
//         try
//         {
//             Directory.CreateDirectory(Path.Combine(SSCSavePath, id.ToString()));
//             var data = new PlayerFileData(Path.Combine(SSCSavePath, id.ToString(), $"{name}.plr"), false)
//             {
//                 Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
//                 Player = new Player { name = name, difficulty = difficulty }
//             };
//
//             data.Player.statLife = data.Player.statLifeMax = QOS.SC.SSCConfig.StartLife;
//             data.Player.statMana = data.Player.statManaMax = QOS.SC.SSCConfig.StartMana;
//             switch (data.Player.difficulty)
//             {
//                 case PlayerDifficultyID.SoftCore:
//                 case PlayerDifficultyID.MediumCore:
//                 case PlayerDifficultyID.Hardcore:
//                 {
//                     data.Player.inventory[0].SetDefaults(ItemID.CopperShortsword);
//                     data.Player.inventory[1].SetDefaults(ItemID.CopperPickaxe);
//                     data.Player.inventory[2].SetDefaults(ItemID.CopperAxe);
//                     data.Player.inventory[3].SetDefaults(ItemID.Carrot);
//                     break;
//                 }
//                 case PlayerDifficultyID.Creative:
//                 {
//                     data.Player.inventory[0].SetDefaults(ItemID.IronShortsword);
//                     data.Player.inventory[1].SetDefaults(ItemID.IronPickaxe);
//                     data.Player.inventory[2].SetDefaults(ItemID.IronAxe);
//                     data.Player.inventory[3].SetDefaults(ItemID.IronHammer);
//                     data.Player.inventory[4].SetDefaults(ItemID.BabyBirdStaff);
//                     data.Player.inventory[5].SetDefaults(ItemID.Torch);
//                     data.Player.inventory[5].stack = 100;
//                     data.Player.inventory[6].SetDefaults(ItemID.Rope);
//                     data.Player.inventory[6].stack = 100;
//                     data.Player.inventory[7].SetDefaults(ItemID.MagicMirror);
//                     data.Player.inventory[8].SetDefaults(ItemID.GrapplingHook);
//                     data.Player.inventory[9].SetDefaults(ItemID.Carrot);
//                     data.Player.armor[3].SetDefaults(ItemID.CreativeWings);
//                     data.Player.AddBuff(BuffID.BabyBird, 3600);
//                     break;
//                 }
//             }
//
//             data.Player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
//             CreativePowerManager.Instance.ResetDataForNewPlayer(data.Player);
//             var items = PlayerLoader.GetStartingItems(data.Player, data.Player.inventory.Where(x => !x.IsAir).Select(x => x.Clone()));
//             PlayerLoader.SetStartInventory(data.Player, items);
//
//             // 保存并不一定成功,如果其他mod的数据涉及到服务端未加载的内容,会导致异常并且丢失所有mod数据
//             QOSKit.InternalSavePlayer(data);
//         }
//         catch (Exception e)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.CreateException"), Color.Yellow, plr);
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.CreateExceptionSolution"), Color.Yellow, plr);
//             throw;
//         }
//         finally
//         {
//             NetMessage.TrySendData(MessageID.WorldData, plr);
//         }
//     }
//
//     public void HandlePacket_RemoveSSC(BinaryReader bin, int plr)
//     {
//         var id = bin.ReadUInt64();
//         var name = Path.Combine(SSCSavePath, id.ToString(), $"{bin.ReadString()}.plr");
//
//         try
//         {
//             File.Delete(name);
//             File.Delete(Path.ChangeExtension(name, ".tplr"));
//             File.Delete(Path.ChangeExtension(name, ".txt"));
//         }
//         finally
//         {
//             NetMessage.TrySendData(MessageID.WorldData, plr);
//         }
//     }
//
//     public void HandlePacket_ChooseSSC(BinaryReader bin, int plr)
//     {
//         var id = bin.ReadUInt64();
//         var name = bin.ReadString();
//
//         if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name)) // 防止在线玩家重复
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
//             return;
//         }
//
//         var data = Player.LoadPlayer(Path.Combine(SSCSavePath, id.ToString(), $"{name}.plr"), false);
//
//         if (data.Player.difficulty == PlayerDifficultyID.Creative && !Main.GameModeInfo.IsJourneyMode)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, plr);
//             return;
//         }
//
//         if (data.Player.difficulty != PlayerDifficultyID.Creative && Main.GameModeInfo.IsJourneyMode)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, plr);
//             return;
//         }
//
//         if (!SystemLoader.CanWorldBePlayed(data, Main.ActiveWorldFileData, out var mod)) // 兼容其他mod的游玩规则
//         {
//             var message = mod.WorldCanBePlayedRejectionMessage(data, Main.ActiveWorldFileData);
//             ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(message), Color.Red, plr);
//             return;
//         }
//
//         // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
//         var binary = QOSKit.Plr2Byte(data.Path);
//         var mp = Mod.GetPacket();
//         mp.Write((byte)QOS.ID.LoadSSC);
//         mp.Write(binary.Length);
//         mp.Write(binary);
//         mp.Send(plr);
//
//         // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
//         NetMessage.SendData(MessageID.PlayerInfo, plr);
//     }
//
//     public void HandlePacket_LoadSSC(BinaryReader bin, int plr)
//     {
//         var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");
//
//         QOSKit.Byte2Plr(bin.ReadBytes(bin.ReadInt32()), name);
//
//         var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.SSC"), false) // 只有这里会设置后缀为SSC
//         {
//             Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
//             Player = Player.LoadPlayer(name, false).Player
//         };
//         data.MarkAsServerSide();
//         data.SetAsActive();
//
//         data.Player.Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields,设置临时物品
//         try
//         {
//             Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
//         }
//         catch (Exception e)
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
//             throw;
//         }
//         finally
//         {
//             UI.SetState(null);
//         }
//     }
//
//     public void HandlePacket_SaveSSC(BinaryReader bin, int plr)
//     {
//         var id = bin.ReadUInt64();
//         var name = Path.Combine(SSCSavePath, id.ToString(), $"{bin.ReadString()}.plr");
//         var binary = bin.ReadBytes(bin.ReadInt32());
//
//         if (File.Exists(name))
//         {
//             QOSKit.Byte2Plr(binary, name);
//         }
//         else
//         {
//             ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.FileNotFound"), Color.Red, plr);
//         }
//     }
// }