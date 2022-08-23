using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public partial class SSC
{
    public enum ID
    {
        CreateSSC,
        RemoveSSC,
        SelectSSC,
        LoadSSC,
        SaveSSC,
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        switch ((ID)type)
        {
            case ID.CreateSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();
                var mode = b.ReadByte();

                #region Check Name

                if (string.IsNullOrWhiteSpace(name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, from);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, from);
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Illegal characters in name"), Color.Red, from);
                    return;
                }

                // 防止同名注册
                if (Directory.GetFiles(SavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, from);
                    return;
                }

                #endregion

                #region Create SSC

                var data = new PlayerFileData(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = mode }
                };
                SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                InternalSavePlayer(data);

                #endregion

                NetMessage.TrySendData(MessageID.WorldData, from);
                break;
            }
            case ID.RemoveSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.tplr"));

                NetMessage.TrySendData(MessageID.WorldData, from);
                break;
            }
            case ID.SelectSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                #region Check Data

                // 防止在线玩家名称重复
                if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, from);
                    return;
                }

                var data = Player.LoadPlayer(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false);
                // 防止旅程模式误入导致的隐患和崩溃

                if (data.Player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, from);
                    return;
                }

                if (data.Player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, from);
                    return;
                }

                #endregion

                // 压缩玩家为byte[],客户端同步SSC玩家
                var memoryStream = new MemoryStream();
                TagIO.ToStream(new TagCompound
                {
                    { "PLR", File.ReadAllBytes(Path.Combine(SavePath, id.ToString(), $"{name}.plr")) },
                    { "TPLR", File.ReadAllBytes(Path.Combine(SavePath, id.ToString(), $"{name}.tplr")) },
                }, memoryStream);
                var array = memoryStream.ToArray();

                // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
                var mp = GetPacket();
                mp.Write((byte)ID.LoadSSC);
                mp.Write(array.Length);
                mp.Write(array);
                mp.Send(from);

                // 其他Client者通过原生代码同步必要的数据,包括mod的net同步.
                // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
                NetMessage.SendData(MessageID.PlayerInfo, from);
                break;
            }
            case ID.LoadSSC:
            {
                var compound = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                var name = Path.Combine(Path.GetTempPath(), $"{Sid}.plr");
                File.WriteAllBytes(name, compound.GetByteArray("PLR"));
                File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), compound.GetByteArray("TPLR"));

                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{Sid}.SSC"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = Player.LoadPlayer(name, false).Player
                };
                data.MarkAsServerSide();
                data.SetAsActive();

                Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields
                try
                {
                    Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    throw;
                }
                finally
                {
                    ModContent.GetInstance<SSCSyS>().UI.SetState(null);
                }

                break;
            }
            case ID.SaveSSC:
            {
                var id = b.ReadUInt64();
                var name = Path.Combine(SavePath, id.ToString(), $"{b.ReadString()}.plr");
                var compound = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                if (File.Exists(name))
                {
                    File.WriteAllBytes(name, compound.GetByteArray("PLR"));
                    File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), compound.GetByteArray("TPLR"));
                }

                break;
            }
            default:
                break;
        }
    }
}