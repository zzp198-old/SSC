using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC.Command;

public class SSCCommand : ModCommand
{
    public override string Command => "SSC";
    public override CommandType Type => CommandType.Server;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var id = caller.Player.GetModPlayer<SSCPlayer>().SteamID;
        if (string.IsNullOrEmpty(id))
        {
            NetMessage.BootPlayer(caller.Player.whoAmI, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
            return;
        }

        if (args[0] == "list")
        {
            if (Directory.Exists(Path.Combine(Main.SavePath, "SSC", id)))
            {
                var plrs = Directory.GetFiles(Path.Combine(Main.SavePath, "SSC", id), "*.plr");

                if (plrs.Length > 0)
                {
                    for (var i = 0; i < plrs.Length; i++)
                    {
                        var data = Player.LoadPlayer(plrs[i], false);
                        caller.Reply($"{i++}. {data.Player.name} {data.Player.difficulty}");
                    }

                    return;
                }
            }

            caller.Reply($"You don't have any SSC.", Color.Yellow);
            return;
        }

        if (args[0] == "add")
        {
            if (!Directory.Exists(Path.Combine(Main.SavePath, "SSC", id)))
            {
                Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", id));
            }

            var name = args[1];
            var difficulty = byte.Parse(args[2]);
            var data = new PlayerFileData(Path.Combine(Main.SavePath, "SSC", id, name + ".plr"), false)
            {
                Name = name,
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = name,
                    difficulty = difficulty
                }
            };
            typeof(Player).GetMethod("InternalSavePlayerFile", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, new object[] { data });
        }
    }
}