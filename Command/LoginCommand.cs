using System;
using Terraria;
using Terraria.ModLoader;

namespace SSC.Command
{
    public class LoginCommand : ModCommand
    {
        public override string Command => "login";

        // FIXME
        public override CommandType Type => CommandType.Chat | CommandType.Server;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Console.WriteLine(Main.netMode);
            Console.WriteLine(caller.Player.name);
        }
    }
}