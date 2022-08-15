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
    internal static string SavePath => Path.Combine(Main.SavePath, "SSC");
    internal static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    public override void HandlePacket(BinaryReader b, int whoAmI)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(ID)type}({b.BaseStream.Length}) from {whoAmI}");

        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                var id = b.ReadUInt64();
                Directory.CreateDirectory(Path.Combine(SavePath, id.ToString()));
                SSCUtils.SendSSCList(id);
                break;
            }
            case ID.SSCList:
            {
                var num = b.ReadInt32();
                var data = new List<(string, byte)>();
                for (var i = 0; i < num; i++)
                {
                    data.Add((b.ReadString(), b.ReadByte()));
                }

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
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(msg), Color.Red, whoAmI);
                    return;
                }

                // 防止同名注册
                if (Directory.GetFiles(SavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, whoAmI);
                    return;
                }

                // 创建人物
                var data = new PlayerFileData(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = mode }
                };
                SSCUtils.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                SSCUtils.InternalSavePlayer(data);

                SSCUtils.SendSSCList(id);
                break;
            }
            case ID.RemoveSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                // 删除人物
                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.tplr"));

                SSCUtils.SendSSCList(id);
                break;
            }
            case ID.ChooseSSC:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();

                // 防止在线玩家名称重复
                if (Netplay.Clients.Any(x => whoAmI != x.Id && x.IsActive && Main.player[x.Id].name == id.ToString()))
                {
                    SSCUtils.Boot(whoAmI, NetworkText.FromKey(Lang.mp[5].Key, id.ToString()).ToString());
                    return;
                }

                var player = Player.LoadPlayer(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false).Player;

                // 防止旅程模式误入导致的隐患和崩溃
                if (player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, whoAmI);
                    return;
                }

                if (player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, whoAmI);
                    return;
                }

                // 服务端加载SSC玩家,此时玩家没有幽灵化DB.
                Main.player[whoAmI] = player;

                // 压缩玩家为byte[],客户端同步SSC玩家
                var data = SSCUtils.Player2ByteArray(id, player);

                var mp = SSCUtils.GetPacket(ID.SSCBinary);
                mp.Write(whoAmI);
                mp.Write(id);
                mp.Write(data.Length);
                mp.Write(data);
                mp.Send();
                break;
            }
            case ID.SSCBinary:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                    {
                        var i = b.ReadInt32();
                        var id = b.ReadUInt64();
                        var array = b.ReadBytes(b.ReadInt32());

                        Main.player[i] = SSCUtils.ByteArray2Player(id, array);

                        if (i == Main.myPlayer)
                        {
                            UISystem.UI.SetState(null);
                        }

                        break;
                    }
                    case NetmodeID.Server:
                    {
                        break;
                    }
                }

                break;
            }
            default:
                SSCUtils.Boot(whoAmI, $"Unexpected Package ID: {type}");
                break;
        }
    }

    public enum ID
    {
        SSCInit,
        SSCList,
        CreateSSC,
        RemoveSSC,
        ChooseSSC,
        SSCBinary,
    }
}