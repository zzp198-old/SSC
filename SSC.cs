using System;
using System.IO;
using System.Security.Cryptography;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;
using Terraria.ModLoader.IO;
using Terraria.Social;
using Terraria.Utilities;

namespace SSC;

public class SSC : Mod
{
    public static readonly string SavePath = Path.Combine(Main.SavePath, "SSC");

    public override void Load()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
    }

    public override void Unload()
    {
    }

    // public override void HandlePacket(BinaryReader br, int _)
    // {
    //     var type = (PKG_ID)br.Read();
    //     if (type == PKG_ID.RequestPlayerList)
    //     {
    //         var uuid = br.ReadString();
    //         var path = Path.Combine(SavePath, uuid);
    //         var files = Directory.GetFiles(path, "*.plr");
    //
    //         var P = GetPacket();
    //         P.Write((int)PKG_ID.PlayerList);
    //         P.Write(files.Length);
    //         foreach (var file in files)
    //         {
    //             var name = Path.GetFileNameWithoutExtension(file);
    //             var tag = new TagCompound
    //             {
    //                 { "plr", File.ReadAllBytes(file) },
    //             };
    //             if (File.Exists(Path.ChangeExtension(file, ".tplr")))
    //             {
    //                 tag.Add("tplr", File.ReadAllBytes(Path.ChangeExtension(file, ".tplr")));
    //             }
    //
    //             TagIO.WriteTag(name, tag, P);
    //         }
    //
    //         P.Send(_);
    //     }
    //
    //     if (type == PKG_ID.PlayerList)
    //     {
    //         if (Directory.Exists(CachePath))
    //         {
    //             Directory.Delete(CachePath, true);
    //         }
    //
    //         Directory.CreateDirectory(CachePath);
    //         var count = br.Read();
    //         for (var i = 0; i < count; i++)
    //         {
    //             var tag = (TagCompound)TagIO.ReadTag(br, out var name);
    //             File.WriteAllBytes(Path.Combine(CachePath, name + ".plr"), tag.GetByteArray("plr"));
    //             if (tag.ContainsKey("tplr"))
    //             {
    //                 File.WriteAllBytes(Path.Combine(CachePath, name + ".tplr"), tag.GetByteArray("tplr"));
    //             }
    //         }
    //
    //         var UISystem = ModContent.GetInstance<UISystem>();
    //         UISystem.UI.SetState(UISystem.SSCUI);
    //         UISystem.SSCUI.Activate();
    //     }
    //
    //     if (type == PKG_ID.DeletePlayer)
    //     {
    //         var uuid = br.ReadString();
    //         var player_name = br.ReadString();
    //         if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr")))
    //         {
    //             File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr"));
    //         }
    //
    //         if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr")))
    //         {
    //             File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr"));
    //         }
    //
    //         var path = Path.Combine(SavePath, uuid);
    //         var files = Directory.GetFiles(path, "*.plr");
    //
    //         var P = GetPacket();
    //         P.Write((int)PKG_ID.PlayerList);
    //         P.Write(files.Length);
    //         foreach (var file in files)
    //         {
    //             var name = Path.GetFileNameWithoutExtension(file);
    //             var tag = new TagCompound
    //             {
    //                 { "plr", File.ReadAllBytes(file) },
    //             };
    //             if (File.Exists(Path.ChangeExtension(file, ".tplr")))
    //             {
    //                 tag.Add("tplr", File.ReadAllBytes(Path.ChangeExtension(file, ".tplr")));
    //             }
    //
    //             TagIO.WriteTag(name, tag, P);
    //         }
    //
    //         P.Send(_);
    //     }
    //
    //     if (type == PKG_ID.PlayPlayer)
    //     {
    //         var uuid = br.ReadString();
    //         var player_name = br.ReadString();
    //
    //         var tag = new TagCompound();
    //         if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".plr")))
    //         {
    //             tag.Add("plr", File.ReadAllBytes(Path.Combine(SavePath, uuid, player_name + ".plr")));
    //         }
    //
    //         if (File.Exists(Path.Combine(SavePath, uuid, player_name + ".tplr")))
    //         {
    //             tag.Add("tplr", File.ReadAllBytes(Path.Combine(SavePath, uuid, player_name + ".tplr")));
    //         }
    //
    //         var data = Player.LoadPlayer(Path.Combine(SavePath, uuid, player_name + ".plr"), false);
    //         Main.player[_] = data.Player;
    //         var P = GetPacket();
    //         P.Write((int)PKG_ID.CallbackPlayPlayer);
    //         P.Write(_);
    //         TagIO.WriteTag(player_name, tag, P);
    //         P.Send();
    //     }
    //
    //     if (type == PKG_ID.CallbackPlayPlayer)
    //     {
    //         var who = br.Read();
    //         var tag = (TagCompound)TagIO.ReadTag(br, out var name);
    //
    //         if (!Directory.Exists(OtherPath))
    //         {
    //             Directory.CreateDirectory(OtherPath);
    //         }
    //
    //         File.WriteAllBytes(Path.Combine(OtherPath, name + ".plr"), tag.GetByteArray("plr"));
    //         if (tag.ContainsKey("tplr"))
    //         {
    //             File.WriteAllBytes(Path.Combine(OtherPath, name + ".tplr"), tag.GetByteArray("tplr"));
    //         }
    //
    //         var data = Player.LoadPlayer(Path.Combine(OtherPath, name + ".plr"), false);
    //         if (Main.myPlayer == who)
    //         {
    //             data.SetAsActive();
    //         }
    //         else
    //         {
    //             Main.player[who] = data.Player;
    //         }
    //     }
    //
    //     if (type == PKG_ID.CreatePlayer)
    //     {
    //         var uuid = br.ReadString();
    //         var tag = (TagCompound)TagIO.ReadTag(br, out var player_name);
    //         if (tag.ContainsKey("plr"))
    //         {
    //             File.WriteAllBytes(Path.Combine(SavePath, uuid, player_name + ".plr"), tag.GetByteArray("plr"));
    //         }
    //
    //         if (tag.ContainsKey("tplr"))
    //         {
    //             File.WriteAllBytes(Path.Combine(SavePath, uuid, player_name + ".tplr"), tag.GetByteArray("tplr"));
    //         }
    //
    //         var path = Path.Combine(SavePath, uuid);
    //         var files = Directory.GetFiles(path, "*.plr");
    //
    //         var P = GetPacket();
    //         P.Write((int)PKG_ID.PlayerList);
    //         P.Write(files.Length);
    //         foreach (var file in files)
    //         {
    //             var name = Path.GetFileNameWithoutExtension(file);
    //             var new_tag = new TagCompound
    //             {
    //                 { "plr", File.ReadAllBytes(file) },
    //             };
    //             if (File.Exists(Path.ChangeExtension(file, ".tplr")))
    //             {
    //                 new_tag.Add("tplr", File.ReadAllBytes(Path.ChangeExtension(file, ".tplr")));
    //             }
    //
    //             TagIO.WriteTag(name, new_tag, P);
    //         }
    //
    //         P.Send(_);
    //     }
    // }

    public enum PKG_ID
    {
        UUID,
        LIST,
        SAVE,
        ADD,
    }
}