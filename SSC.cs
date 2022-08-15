using System;
using System.Collections.Generic;
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

public class SSC : Mod
{
    public enum ID
    {
        SSCInit,
        SSCList,
        CreateSSC,
        RemoveSSC,
        ChooseSSC,
        SSCBinary
    }

    internal static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    internal static string SavePath()
    {
        return Path.Combine(Main.SavePath, "SSC");
    }

    internal static string SavePath(ulong id)
    {
        return Path.Combine(SavePath(), id.ToString());
    }

    private static string SavePath(ulong id, string name, bool tml = false)
    {
        return Path.Combine(SavePath(id), $"{name}.{(tml ? "tplr" : "plr")}");
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(ID)type}({b.BaseStream.Length}) from {from}");

        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                var id = b.ReadUInt64();
                Directory.CreateDirectory(SavePath(id));
                SSCUtils.SendSSCList(id, from);
                break;
            }
            case ID.SSCList:
            {
                var num = b.ReadInt32();
                var data = new List<(string, byte)>();
                for (var i = 0; i < num; i++) data.Add((b.ReadString(), b.ReadByte()));

                UISystem.View.RedrawList(data);
                break;
            }
            case ID.CreateSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();
                var mode = b.ReadByte();

                // 基本名称校验
                if (!SSCUtils.CheckName(name, out var msg))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(msg), Color.Red, from);
                    return;
                }

                // 防止同名注册
                if (Directory.GetFiles(SavePath(), $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, from);
                    return;
                }

                // 创建人物
                var data = new PlayerFileData(SavePath(id, name), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = mode }
                };

                try
                {
                    SSCUtils.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                    SSCUtils.InternalSavePlayer(data);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, from);
                }

                SSCUtils.SendSSCList(id, from);
                break;
            }
            case ID.RemoveSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                try
                {
                    // 删除人物
                    File.Delete(SavePath(id, name));
                    File.Delete(SavePath(id, name, true));
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, from);
                }

                SSCUtils.SendSSCList(id, from);
                break;
            }
            case ID.ChooseSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                // 防止在线玩家名称重复
                if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, from);
                    return;
                }

                var data = Player.LoadPlayer(SavePath(id, name), false);

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

                // 服务端加载SSC玩家,此时玩家没有幽灵化DB.
                Main.player[from] = data.Player;

                // 压缩玩家为byte[],客户端同步SSC玩家
                var memoryStream = new MemoryStream();
                TagIO.ToStream(new TagCompound
                {
                    { "PLR", File.ReadAllBytes(SavePath(id, name)) },
                    { "TPLR", File.ReadAllBytes(SavePath(id, name, true)) }
                }, memoryStream);
                var array = memoryStream.ToArray();

                var mp = SSCUtils.GetPacket(ID.SSCBinary);
                mp.Write(from);
                mp.Write(array.Length);
                mp.Write(array);
                mp.Send();
                break;
            }
            case ID.SSCBinary:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                    {
                        var whoAmI = b.ReadInt32();
                        var array = b.ReadBytes(b.ReadInt32());

                        Main.player[whoAmI] = SSCUtils.ByteArray2Player(array);

                        if (whoAmI == Main.myPlayer)
                        {
                            Main.player[whoAmI].Spawn(PlayerSpawnContext.SpawningIntoWorld);
                            try
                            {
                                PlayerLoader.OnEnterWorld(whoAmI);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                            }
                            finally
                            {
                                UISystem.UI.SetState(null);
                            }
                        }

                        break;
                    }
                    case NetmodeID.Server:
                    {
                        var id = b.ReadUInt64();
                        var name = b.ReadString();
                        var compound = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                        if (!File.Exists(SavePath(id, name)))
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Player does not exist or has been deleted."),
                                Color.Red, from);
                            return;
                        }

                        try
                        {
                            File.WriteAllBytes(SavePath(id, name), compound.GetByteArray("PLR"));
                            File.WriteAllBytes(SavePath(id, name, true), compound.GetByteArray("TPLR"));
                        }
                        catch (Exception e)
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, from);
                        }

                        break;
                    }
                }

                break;
            }
            default:
                SSCUtils.Boot(from, $"Unexpected Package ID: {type}");
                break;
        }
    }
}