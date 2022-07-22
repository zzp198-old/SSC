using Terraria.ModLoader;

namespace SSC;

public class SSCCommand : ModCommand
{
    public override string Command => "SSC";
    public override CommandType Type => CommandType.Server;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var type = args[0];
        if (type == "add")
        {
            
        }
        else if (type == "del")
        {
            
        }
        else if (type == "rename")
        {
            
        }
        else if (type == "use")
        {
            
        }


        // bool flag2 = false;
        // if (Netplay.Clients[this.whoAmI].State < 10)
        // {
        //     for (int index = 0; index < (int) byte.MaxValue; ++index)
        //     {
        //         if (index != number3 && player2.name == Main.player[index].name && Netplay.Clients[index].IsActive)
        //             flag2 = true;
        //     }
        // }
        // if (flag2)
        // {
        //     NetMessage.TrySendData(2, this.whoAmI, text: NetworkText.FromKey(Lang.mp[5].Key, (object) player2.name));
        //     break;
        // }
        // if (player2.name.Length > Player.nameLen)
        // {
        //     NetMessage.TrySendData(2, this.whoAmI, text: NetworkText.FromKey("Net.NameTooLong"));
        //     break;
        // }
        // if (player2.name == "")
        // {
        //     NetMessage.TrySendData(2, this.whoAmI, text: NetworkText.FromKey("Net.EmptyName"));
        //     break;
        // }
        // if (player2.difficulty == (byte) 3 && !Main.GameModeInfo.IsJourneyMode)
        // {
        //     NetMessage.TrySendData(2, this.whoAmI, text: NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"));
        //     break;
        // }
        // if (player2.difficulty != (byte) 3 && Main.GameModeInfo.IsJourneyMode)
        // {
        //     NetMessage.TrySendData(2, this.whoAmI, text: NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"));
        //     break;
        // }
    }
}