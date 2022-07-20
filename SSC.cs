using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using NetMessage = IL.Terraria.NetMessage;

namespace SSC;

public partial class SSC : Mod
{
    public static readonly string SavePath = Path.Combine(Main.SavePath, "SSC");
    public static readonly string CachePath = Path.Combine(SavePath, "Cache");
    public static readonly string OtherPath = Path.Combine(SavePath, "Other");

    public override void HandlePacket(BinaryReader br, int _)
    {
        var type = (PKG_ID)br.Read();
        if (type == PKG_ID.RequestPlayerList)
        {
            var uuid = br.ReadString();
            var path = Path.Combine(SavePath, uuid);
            var files = Directory.GetFiles(path, "*.plr");

            var P = GetPacket();
            P.Write((int)PKG_ID.PlayerList);
            P.Write(files.Length);
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var tag = new TagCompound
                {
                    { "plr", File.ReadAllBytes(file) },
                };
                if (File.Exists(Path.ChangeExtension(file, ".tplr")))
                {
                    tag.Add("tplr", File.ReadAllBytes(Path.ChangeExtension(file, ".tplr")));
                }

                TagIO.WriteTag(name, tag, P);
            }

            P.Send(_);
        }

        if (type == PKG_ID.PlayerList)
        {
            if (Directory.Exists(CachePath))
            {
                Directory.Delete(CachePath, true);
            }

            Directory.CreateDirectory(CachePath);
            var count = br.Read();
            for (var i = 0; i < count; i++)
            {
                var tag = (TagCompound)TagIO.ReadTag(br, out var name);
                File.WriteAllBytes(Path.Combine(CachePath, name + ".plr"), tag.GetByteArray("plr"));
                if (tag.ContainsKey("tplr"))
                {
                    File.WriteAllBytes(Path.Combine(CachePath, name + ".tplr"), tag.GetByteArray("tplr"));
                }
            }

            var UISystem = ModContent.GetInstance<UISystem>();
            UISystem.UI.SetState(UISystem.SSCUI);
        }

        if (type == PKG_ID.DeletePlayer)
        {
            var uuid = br.ReadString();
            var player_name = br.ReadString();
            if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr")))
            {
                File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr"));
            }

            if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr")))
            {
                File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr"));
            }

            var path = Path.Combine(SavePath, uuid);
            var files = Directory.GetFiles(path, "*.plr");

            var P = GetPacket();
            P.Write((int)PKG_ID.PlayerList);
            P.Write(files.Length);
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var tag = new TagCompound
                {
                    { "plr", File.ReadAllBytes(file) },
                };
                if (File.Exists(Path.ChangeExtension(file, ".tplr")))
                {
                    tag.Add("tplr", File.ReadAllBytes(Path.ChangeExtension(file, ".tplr")));
                }

                TagIO.WriteTag(name, tag, P);
            }

            P.Send(_);
        }

        if (type == PKG_ID.PlayPlayer)
        {
            var uuid = br.ReadString();
            var player_name = br.ReadString();

            var tag = new TagCompound();
            if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr")))
            {
                tag.Add("plr", File.ReadAllBytes(Path.Combine(SavePath, uuid, player_name + ".plr")));
            }

            if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr")))
            {
                tag.Add("tplr", File.ReadAllBytes(Path.Combine(SavePath, uuid, player_name + ".tplr")));
            }

            var data = Player.LoadPlayer(Path.Combine(SavePath, uuid, player_name + ".plr"), false);
            Main.player[_] = data.Player;
            var P = GetPacket();
            P.Write((int)PKG_ID.CallbackPlayPlayer);
            P.Write(_);
            TagIO.WriteTag(player_name, tag, P);
            P.Send();
        }

        if (type == PKG_ID.CallbackPlayPlayer)
        {
            var who = br.Read();
            var tag = (TagCompound)TagIO.ReadTag(br, out var name);

            if (!Directory.Exists(OtherPath))
            {
                Directory.CreateDirectory(OtherPath);
            }

            File.WriteAllBytes(Path.Combine(OtherPath, name + ".plr"), tag.GetByteArray("plr"));
            if (tag.ContainsKey("tplr"))
            {
                File.WriteAllBytes(Path.Combine(OtherPath, name + ".tplr"), tag.GetByteArray("tplr"));
            }

            var data = Player.LoadPlayer(Path.Combine(OtherPath, name + ".plr"), false);
            if (Main.myPlayer == who)
            {
                data.SetAsActive();
            }
            else
            {
                Main.player[who] = data.Player;
            }
        }

        if (type == PKG_ID.CreatePlayer)
        {
            // TODO
        }
    }

    public enum PKG_ID
    {
        RequestPlayerList,
        PlayerList,
        DeletePlayer,
        PlayPlayer,
        CallbackPlayPlayer,
        CreatePlayer,
    }
}