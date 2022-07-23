using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public class SSCCommand : ModCommand
{
    public override string Command => "SSC";
    public override CommandType Type => CommandType.Server;
    public override string Usage => "";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var id = caller.Player.GetModPlayer<SSCPlayer>().SteamID;
        if (string.IsNullOrWhiteSpace(id))
        {
            SSCUtils.Kick(caller.Player.whoAmI, $"Unexpected SteamID: {id}");
            return;
        }

        var action = args[0];
        if (action == "list")
        {
            var i = 0;
            Directory.GetFiles(Path.Combine(SSC.ServerSavePath, id), "*.plr").ToList().ForEach(plr =>
            {
                var data = Player.LoadPlayer(plr, false);
                caller.Reply($"{i++}. name: {data.Name}  mode: {data.Player.difficulty}");
            });
        }

        if (action == "add")
        {
            var name = args[1];
            var diff = int.Parse(args[2]);
        }
    }
}