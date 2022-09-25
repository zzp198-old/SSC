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

