using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QOS.Class.SSC.Systems;
public class NetCodeSystem : ModSystem
{
    public override void NetSend(BinaryWriter binary)
    {
        var tag = new TagCompound();
        var directory = new DirectoryInfo(Path.Combine(QOS.SavePath, "SSC"));
        if (directory.Exists)
        {
            directory.GetDirectories().ToList().ForEach(i =>
            {
                tag.Set(i.Name, new List<TagCompound>());
                i.GetFiles("*.plr").ToList().ForEach(j =>
                {
                    try
                    {
                        var data = Player.LoadPlayer(j.FullName, false);
                        tag.Get<List<TagCompound>>(i.Name).Add(new TagCompound
                        {
                            { "name", data.Player.name },
                            { "difficulty", data.Player.difficulty }
                        });
                    }
                    catch (Exception e)
                    {
                        Mod.Logger.Error(e);
                    }
                });
            });
        }

        TagIO.ToStream(tag, binary.BaseStream);
    }

    public override void NetReceive(BinaryReader bin)
    {
        var tag = TagIO.FromStream(bin.BaseStream);
        if (UISystem.UI.CurrentState != null)
        {
            ((Views.SSCView)UISystem.UI.CurrentState)?.Redraw(tag);
        }
    }

    public static void CreateSSC(BinaryReader bin, int plr)
    {
        var id = bin.ReadUInt64();
        var name = bin.ReadString();
        var t = bin.ReadBytes(bin.ReadInt32());
        var tml = bin.ReadBytes(bin.ReadInt32());

        if (id.ToString().Equals(name))
        {
            // TODO
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("你的steamid是内部保留名字,禁止使用"), Color.Red, plr);
            return;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, plr);
            return;
        }

        if (name.Length > Player.nameLen)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, plr);
            return;
        }

        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            // TODO
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.InvalidFileName"), Color.Red, plr);
            return;
        }

        var directory = new DirectoryInfo(Path.Combine(QOS.SavePath, "SSC"));
        if (directory.GetFiles($"{name}.plr", SearchOption.AllDirectories).Length > 0) // 防止同名注册
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
            return;
        }

        Utils.TryCreatingDirectory(Path.Combine(QOS.SavePath, "SSC", id.ToString()));
        File.WriteAllBytes(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.plr"), t);
        File.WriteAllBytes(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.tplr"), tml);

        ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("创建成功"), Color.Yellow, plr);

        NetMessage.TrySendData(MessageID.WorldData, plr);
    }

    public static void RemoveSSC(BinaryReader bin, int plr)
    {
        var id = bin.ReadUInt64();
        var name = bin.ReadString();

        File.Delete(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.plr"));
        File.Delete(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.tplr"));

        ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("删除成功"), Color.Yellow, plr);

        NetMessage.TrySendData(MessageID.WorldData, plr);
    }

    public static void ChooseSSC(BinaryReader bin, int plr)
    {
        var id = bin.ReadUInt64();
        var name = bin.ReadString();

        if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name)) // 防止在线玩家重复
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
            return;
        }

        var data = Player.LoadPlayer(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.plr"), false);

        if (data.Player.difficulty == PlayerDifficultyID.Creative && !Main.GameModeInfo.IsJourneyMode)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, plr);
            return;
        }

        if (data.Player.difficulty != PlayerDifficultyID.Creative && Main.GameModeInfo.IsJourneyMode)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, plr);
            return;
        }

        if (!SystemLoader.CanWorldBePlayed(data, Main.ActiveWorldFileData, out var mod)) // 兼容其他mod的游玩规则
        {
            var message = mod.WorldCanBePlayedRejectionMessage(data, Main.ActiveWorldFileData);
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(message), Color.Red, plr);
            return;
        }

        // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)

        var t = File.ReadAllBytes(data.Path);
        var tml = File.ReadAllBytes(Path.ChangeExtension(data.Path, ".tplr"));

        var mp = QOS.Mod.GetPacket();
        mp.Write((byte)QOS.PID.LoadSSC);
        mp.Write(t.Length);
        mp.Write(t);
        mp.Write(tml.Length);
        mp.Write(tml);
        mp.Send(plr);

        // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
        NetMessage.SendData(MessageID.PlayerInfo, plr);
    }

    public static void LoadSSC(BinaryReader bin, int plr)
    {
        var t = bin.ReadBytes(bin.ReadInt32());
        var tml = bin.ReadBytes(bin.ReadInt32());

        var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");
        File.WriteAllBytes(name, t);
        File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), tml);

        var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.SSC"), false) // 只有这里会设置后缀为SSC
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = Player.LoadPlayer(name, false).Player
        };
        data.MarkAsServerSide();
        data.SetAsActive();

        data.Player.Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields,设置临时物品
        try
        {
            Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
            throw;
        }
        finally
        {
            UISystem.UI.SetState(null);
        }
    }

    public static void SaveSSC(BinaryReader bin, int plr)
    {
        var id = bin.ReadUInt64();
        var name = bin.ReadString();
        var t = bin.ReadBytes(bin.ReadInt32());
        var tml = bin.ReadBytes(bin.ReadInt32());

        if (File.Exists(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.plr")))
        {
            File.WriteAllBytes(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.plr"), t);
            File.WriteAllBytes(Path.Combine(QOS.SavePath, "SSC", id.ToString(), $"{name}.tplr"), tml);

            if (Configs.SSCConfig.Instance.DEBUG)
            {
                ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("保存成功"), Color.Yellow, plr);
            }
        }
        else
        {
            // TODO
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.FileNotFound"), Color.Red, plr);
        }
    }
}