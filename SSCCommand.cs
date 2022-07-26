using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Creative;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSCCommand : ModCommand
{
    public override string Command => "SSC";
    public override CommandType Type => CommandType.Server;

    public override string Usage => "/SSC [action] [arg...]\n" +
                                    "e.g: \n" +
                                    "/SSC list\n" +
                                    "/SSC add [name] [difficulty](0/classic,1/mediumcore,2/hardcore,3/journey)\n" +
                                    "/SSC use [id](from list)\n" +
                                    "/SSC del [id](from list)\n";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        var id = caller.Player.GetModPlayer<SSCPlayer>().SteamID;
        if (string.IsNullOrEmpty(id))
        {
            NetMessage.BootPlayer(caller.Player.whoAmI, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
            return;
        }

        switch (args[0])
        {
            // 获取SteamID名下的所有Player并返回简述
            case "list":
            {
                var dir = Path.Combine(SSC.SavePath, "Server", id);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.plr");
                    if (files.Length > 0)
                    {
                        for (var i = 0; i < files.Length; i++)
                        {
                            var data = Player.LoadPlayer(files[i], false);
                            var text = data.Player.difficulty switch
                            {
                                0 => "classic",
                                1 => "mediumcore",
                                2 => "hardcore",
                                3 => "journey",
                                _ => "error"
                            };
                            caller.Reply(
                                $"{i}. name: {data.Name}  " +
                                $"difficulty: [c/{data.Player.ChatColor().Hex3()}:{text}]  " +
                                $"time: {data.GetPlayTime():hh\\:mm\\:ss}"
                            );
                        }
                    }
                }

                caller.Reply("You don't have any SSC.", Color.Yellow);
            }
                break;
            // 在SteamID名下创建新角色
            case "add":
            {
                var name = args[1];
                var player = new Player();

                if (!Directory.Exists(Path.Combine(SSC.SavePath, "Server", id)))
                {
                    Directory.CreateDirectory(Path.Combine(SSC.SavePath, "Server", id));
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    caller.Reply("Name contains illegal char.", Color.Red);
                    return;
                }

                if (Directory.GetDirectories(Path.Combine(SSC.SavePath, "Server"))
                    .Select(i => Directory.GetFiles(i, $"{name}.plr"))
                    .Any(i => i.Length > 0))
                {
                    caller.Reply("Name already exists.", Color.Red);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    caller.Reply(NetworkText.FromKey("Net.NameTooLong").ToString(), Color.Red);
                    break;
                }

                if (name == "")
                {
                    caller.Reply(NetworkText.FromKey("Net.EmptyName").ToString(), Color.Red);
                    break;
                }

                player.name = name;
                var difficulty = args[2];
                if (difficulty == "0" || difficulty.ToLower() == "classic")
                {
                    player.difficulty = 0;
                }
                else if (difficulty == "1" || difficulty.ToLower() == "mediumcore")
                {
                    player.difficulty = 1;
                }
                else if (difficulty == "2" || difficulty.ToLower() == "hardcore")
                {
                    player.difficulty = 2;
                }
                else if (difficulty == "3" || difficulty.ToLower() == "journey")
                {
                    player.difficulty = 3;
                }
                else
                {
                    caller.Reply($"Unexpected difficulty value: {difficulty}", Color.Red);
                    break;
                }

                SetupPlayerStatsAndInventoryBasedOnDifficulty(player);

                var data = new PlayerFileData(Path.Combine(SSC.SavePath, "Server", id, name + ".plr"), false)
                {
                    Name = name,
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = player
                };
                try
                {
                    typeof(Player).GetMethod("InternalSavePlayerFile", BindingFlags.NonPublic | BindingFlags.Static)
                        .Invoke(null, new object[] { data });
                    ChatHelper.BroadcastChatMessage(
                        NetworkText.FromLiteral($"{id} successfully created a new player named {name}"),
                        Color.Green);
                }
                catch (Exception e)
                {
                    caller.Reply(e.ToString(), Color.Red);
                }
            }
                break;
            // 使用SteamID名下的角色/地图同步
            case "use":
            {
                if (!int.TryParse(args[1], out var i))
                {
                    caller.Reply("Parameter must be numeric.", Color.Red);
                    return;
                }

                if (caller.Player.GetModPlayer<SSCPlayer>().State)
                {
                    caller.Reply("The current steam account has selected player, please exit and reselect.", Color.Red);
                    return;
                }

                var dir = Path.Combine(SSC.SavePath, "Server", id);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.plr");
                    if (i < files.Length)
                    {
                        var data = Player.LoadPlayer(files[i], false);
                        if (data.Player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                        {
                            caller.Reply(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative").ToString(),
                                Color.Red);
                            break;
                        }

                        if (data.Player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                        {
                            caller.Reply(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative").ToString(),
                                Color.Red);
                            break;
                        }

                        // 发送并本地应用player
                        var compound = new TagCompound();
                        compound.Set("Terraria", File.ReadAllBytes(files[i]));
                        if (File.Exists(Path.ChangeExtension(files[i], ".tplr")))
                        {
                            compound.Set("tModLoader", File.ReadAllBytes(Path.ChangeExtension(files[i], ".tplr")));
                        }


                        Main.player[caller.Player.whoAmI] = data.Player;
                        Main.player[caller.Player.whoAmI].GetModPlayer<SSCPlayer>().SteamID = id;
                        Main.player[caller.Player.whoAmI].GetModPlayer<SSCPlayer>().State = true;
                        Main.player[caller.Player.whoAmI].Spawn(PlayerSpawnContext.SpawningIntoWorld);

                        var packet = Mod.GetPacket();
                        packet.Write((byte)PID.LoadSSC);
                        packet.Write(caller.Player.whoAmI);
                        TagIO.WriteTag(Path.GetFileNameWithoutExtension(files[i]), compound, packet);
                        packet.Send();

                        // 发送地图数据
                        compound = new TagCompound();
                        var map_name = Path.Combine(SSC.SavePath, "Server", id,
                            Main.ActiveWorldFileData.UniqueId + ".map");
                        if (File.Exists(map_name))
                        {
                            compound.Set("Terraria", File.ReadAllBytes(map_name));
                        }

                        if (File.Exists(Path.ChangeExtension(map_name, ".tmap")))
                        {
                            compound.Set("tModLoader", File.ReadAllBytes(Path.ChangeExtension(map_name, ".tmap")));
                        }

                        packet = Mod.GetPacket();
                        packet.Write((byte)PID.LoadMap);
                        TagIO.Write(compound, packet);
                        packet.Send(caller.Player.whoAmI);
                        return;
                    }
                }

                caller.Reply("Player/index does not exist.", Color.Yellow);
            }
                break;
            // 删除SteamID名下的指定角色
            case "del":
            {
                if (!int.TryParse(args[1], out var i))
                {
                    caller.Reply("Parameter must be numeric.", Color.Red);
                    return;
                }

                var dir = Path.Combine(SSC.SavePath, "Server", id);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.plr");
                    if (i < files.Length)
                    {
                        File.Delete(files[i]);
                        if (File.Exists(Path.ChangeExtension(files[i], ".tplr")))
                        {
                            File.Delete(Path.ChangeExtension(files[i], ".tplr"));
                        }

                        caller.Reply($"Player {Path.GetFileNameWithoutExtension(files[i])} deleted successfully.",
                            Color.Green);
                        return;
                    }
                }

                caller.Reply("Player/index does not exist.", Color.Yellow);
            }
                break;
            default:
                caller.Reply(Usage, Color.Red);
                break;
        }
    }

    // 修改自UICharacterCreation.SetupPlayerStatsAndInventoryBasedOnDifficulty
    static void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player player)
    {
        var index1 = 0;
        int num1;
        if (player.difficulty == 3)
        {
            player.statLife = player.statLifeMax = 100;
            player.statMana = player.statManaMax = 20;
            player.inventory[index1].SetDefaults(6);
            Item[] inventory1 = player.inventory;
            int index2 = index1;
            int index3 = index2 + 1;
            inventory1[index2].Prefix(-1);
            player.inventory[index3].SetDefaults(1);
            Item[] inventory2 = player.inventory;
            int index4 = index3;
            int index5 = index4 + 1;
            inventory2[index4].Prefix(-1);
            player.inventory[index5].SetDefaults(10);
            Item[] inventory3 = player.inventory;
            int index6 = index5;
            int index7 = index6 + 1;
            inventory3[index6].Prefix(-1);
            player.inventory[index7].SetDefaults(7);
            Item[] inventory4 = player.inventory;
            int index8 = index7;
            int index9 = index8 + 1;
            inventory4[index8].Prefix(-1);
            player.inventory[index9].SetDefaults(4281);
            Item[] inventory5 = player.inventory;
            int index10 = index9;
            int index11 = index10 + 1;
            inventory5[index10].Prefix(-1);
            player.inventory[index11].SetDefaults(8);
            Item[] inventory6 = player.inventory;
            int index12 = index11;
            int index13 = index12 + 1;
            inventory6[index12].stack = 100;
            player.inventory[index13].SetDefaults(965);
            Item[] inventory7 = player.inventory;
            int index14 = index13;
            int num2 = index14 + 1;
            inventory7[index14].stack = 100;
            Item[] inventory8 = player.inventory;
            int index15 = num2;
            int num3 = index15 + 1;
            inventory8[index15].SetDefaults(50);
            Item[] inventory9 = player.inventory;
            int index16 = num3;
            num1 = index16 + 1;
            inventory9[index16].SetDefaults(84);
            player.armor[3].SetDefaults(4978);
            player.armor[3].Prefix(-1);
            player.AddBuff(216, 3600);
        }
        else
        {
            player.inventory[index1].SetDefaults(3507);
            Item[] inventory10 = player.inventory;
            int index17 = index1;
            int index18 = index17 + 1;
            inventory10[index17].Prefix(-1);
            player.inventory[index18].SetDefaults(3509);
            Item[] inventory11 = player.inventory;
            int index19 = index18;
            int index20 = index19 + 1;
            inventory11[index19].Prefix(-1);
            player.inventory[index20].SetDefaults(3506);
            Item[] inventory12 = player.inventory;
            int index21 = index20;
            num1 = index21 + 1;
            inventory12[index21].Prefix(-1);
        }

        if (Main.runningCollectorsEdition)
        {
            Item[] inventory = player.inventory;
            int index22 = num1;
            int num4 = index22 + 1;
            inventory[index22].SetDefaults(603);
        }

        player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
        PlayerLoader.SetStartInventory(player,
            PlayerLoader.GetStartingItems(player,
                player.inventory.Where((Func<Item, bool>)(item => !item.IsAir))
                    .Select((Func<Item, Item>)(x => x.Clone()))));
    }
}