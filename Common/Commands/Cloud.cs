using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Social;

namespace SSC.Common.Commands;

public class Cloud : ModCommand
{
    public List<string> Cache;

    public override string Command => "Cloud";
    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        switch (args.Length)
        {
            case 0:
            {
                Cache = SocialAPI.Cloud.GetFiles().ToList();
                for (var i = 0; i < Cache.Count; i++)
                {
                    ChatHelper.DisplayMessage(NetworkText.FromLiteral($"{i}. {Cache[i]}"), Color.White, byte.MaxValue);
                }

                break;
            }
            default:
            {
                if (Cache == null)
                {
                    ChatHelper.DisplayMessage(NetworkText.FromLiteral("Cache invalid."), Color.Yellow, byte.MaxValue);
                    return;
                }

                foreach (var arg in args)
                {
                    if (!int.TryParse(arg, out var i))
                    {
                        continue;
                    }

                    if (SocialAPI.Cloud.Delete(Cache[i]))
                    {
                        ChatHelper.DisplayMessage(NetworkText.FromLiteral("RemoveCloud"), Color.Yellow, byte.MaxValue);
                    }
                }

                Cache = null;
                break;
            }
        }
    }
}