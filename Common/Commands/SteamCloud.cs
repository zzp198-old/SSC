using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Social;

namespace QOS.Common.Commands;

public class SteamCloud : ModCommand
{
    internal static List<string> Files;

    public override string Command => "SteamCloud";
    public override CommandType Type => CommandType.Chat;
    public override string Usage => "SteamCloud 查询所有文件\nSteamCloud [id id id...] 删除指定编号的文件(可复选)";
    public override string Description => "可以帮你查看并删除Steam云存档文件";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length == 0)
        {
            Files = SocialAPI.Cloud.GetFiles().ToList();
            for (var i = 0; i < Files.Count; i++)
            {
                ChatHelper.DisplayMessage(NetworkText.FromLiteral($"{i}. {Files[i]}"), Color.White, byte.MaxValue);
            }
        }
        else
        {
            if (Files == null)
            {
                ChatHelper.DisplayMessage(NetworkText.FromLiteral($"缓存已失效,请先查询后执行删除"), Color.Yellow, byte.MaxValue);
                return;
            }

            foreach (var arg in args)
            {
                if (!int.TryParse(arg, out var i))
                {
                    continue;
                }

                if (SocialAPI.Cloud.Delete(Files[i]))
                {
                    ChatHelper.DisplayMessage(NetworkText.FromLiteral($"文件 {Files[i]} 删除成功"), Color.White, byte.MaxValue);
                }
            }

            Files = null;
        }
    }
}