using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using QOS.Class.SSC.Configs;
using QOS.Class.SSC.Views;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QOS.Class.SSC.Systems;

public class NetCodeSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        Console.WriteLine("IsLoadingEnabled NetCodeSystem");
        return SSCConfig.Instance != null;
    }

    public override void NetSend(BinaryWriter binary)
    {
        var tag = new TagCompound();
        new DirectoryInfo(SSCKit.SavePath(SSCConfig.Instance.EveryWorld)).GetDirectories().ToList().ForEach(i =>
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
        TagIO.ToStream(tag, binary.BaseStream);
    }

    public override void NetReceive(BinaryReader bin)
    {
        var tag = TagIO.FromStream(bin.BaseStream);
        if (UISystem.UI.CurrentState != null) ((SSCView)UISystem.UI.CurrentState)?.Redraw(tag);
    }

    public static void CreateSSC(BinaryReader binary, int plr)
    {
        var id = binary.ReadUInt64();
        var name = binary.ReadString();
        var t = binary.ReadBytes(binary.ReadInt32());
        var tml = binary.ReadBytes(binary.ReadInt32());

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

        var directory = new DirectoryInfo(SSCKit.SavePath(SSCConfig.Instance.EveryWorld));
        if (directory.GetFiles($"{name}.plr", SearchOption.AllDirectories).Length > 0) // 防止同名注册
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
            return;
        }

        File.WriteAllBytes(SSCKit.SavePath(SSCConfig.Instance.EveryWorld, id, $"{name}.plr"), t);
        File.WriteAllBytes(SSCKit.SavePath(SSCConfig.Instance.EveryWorld, id, $"{name}.tplr"), tml);

        NetMessage.TrySendData(MessageID.WorldData, plr);
    }
}