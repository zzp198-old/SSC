using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

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
                        caller.Reply($"{i++}.  {data.Player.name}  {data.Player.difficulty}  {data.GetPlayTime()}");
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
                    difficulty = difficulty,
                }
            };
            SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);

            typeof(Player).GetMethod("InternalSavePlayerFile", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, new object[] { data });
        }

        if (args[0] == "use")
        {
            if (caller.Player.GetModPlayer<SSCPlayer>().Sended)
            {
                caller.Reply("Log out if you want to change player.");
                return;
            }

            var who = int.Parse(args[1]);
            var plr = Directory.GetFiles(Path.Combine(Main.SavePath, "SSC", id), "*.plr")[who];

            var data = Player.LoadPlayer(plr, false);
            Main.player[who] = data.Player;
            Main.player[who].GetModPlayer<SSCPlayer>().SteamID = id;
            caller.Player.GetModPlayer<SSCPlayer>().Sended = true;

            var compound = new TagCompound();

            compound.Set("Terraria", File.ReadAllBytes(plr));
            compound.Set("tModLoader", File.ReadAllBytes(Path.ChangeExtension(plr, ".tplr")));

            var packet = Mod.GetPacket();
            packet.Write((byte)PID.ApplySSC);
            packet.Write(caller.Player.whoAmI);
            TagIO.WriteTag(Path.GetFileNameWithoutExtension(plr), compound, packet);
            packet.Send();
        }
    }

    private void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player _player)
    {
        int index1 = 0;
        int num1;
        if (_player.difficulty == (byte)3)
        {
            _player.statLife = _player.statLifeMax = 100;
            _player.statMana = _player.statManaMax = 20;
            _player.inventory[index1].SetDefaults(6);
            Item[] inventory1 = _player.inventory;
            int index2 = index1;
            int index3 = index2 + 1;
            inventory1[index2].Prefix(-1);
            _player.inventory[index3].SetDefaults(1);
            Item[] inventory2 = _player.inventory;
            int index4 = index3;
            int index5 = index4 + 1;
            inventory2[index4].Prefix(-1);
            _player.inventory[index5].SetDefaults(10);
            Item[] inventory3 = _player.inventory;
            int index6 = index5;
            int index7 = index6 + 1;
            inventory3[index6].Prefix(-1);
            _player.inventory[index7].SetDefaults(7);
            Item[] inventory4 = _player.inventory;
            int index8 = index7;
            int index9 = index8 + 1;
            inventory4[index8].Prefix(-1);
            _player.inventory[index9].SetDefaults(4281);
            Item[] inventory5 = _player.inventory;
            int index10 = index9;
            int index11 = index10 + 1;
            inventory5[index10].Prefix(-1);
            _player.inventory[index11].SetDefaults(8);
            Item[] inventory6 = _player.inventory;
            int index12 = index11;
            int index13 = index12 + 1;
            inventory6[index12].stack = 100;
            _player.inventory[index13].SetDefaults(965);
            Item[] inventory7 = _player.inventory;
            int index14 = index13;
            int num2 = index14 + 1;
            inventory7[index14].stack = 100;
            Item[] inventory8 = _player.inventory;
            int index15 = num2;
            int num3 = index15 + 1;
            inventory8[index15].SetDefaults(50);
            Item[] inventory9 = _player.inventory;
            int index16 = num3;
            num1 = index16 + 1;
            inventory9[index16].SetDefaults(84);
            _player.armor[3].SetDefaults(4978);
            _player.armor[3].Prefix(-1);
            _player.AddBuff(216, 3600);
        }
        else
        {
            _player.inventory[index1].SetDefaults(3507);
            Item[] inventory10 = _player.inventory;
            int index17 = index1;
            int index18 = index17 + 1;
            inventory10[index17].Prefix(-1);
            _player.inventory[index18].SetDefaults(3509);
            Item[] inventory11 = _player.inventory;
            int index19 = index18;
            int index20 = index19 + 1;
            inventory11[index19].Prefix(-1);
            _player.inventory[index20].SetDefaults(3506);
            Item[] inventory12 = _player.inventory;
            int index21 = index20;
            num1 = index21 + 1;
            inventory12[index21].Prefix(-1);
        }

        if (Main.runningCollectorsEdition)
        {
            Item[] inventory = _player.inventory;
            int index22 = num1;
            int num4 = index22 + 1;
            inventory[index22].SetDefaults(603);
        }

        _player.savedPerPlayerFieldsThatArentInThePlayerClass = new Terraria.Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(_player);
        PlayerLoader.SetStartInventory(_player,
            (IList<Item>)PlayerLoader.GetStartingItems(_player,
                ((IEnumerable<Item>)_player.inventory).Where<Item>((Func<Item, bool>)(item => !item.IsAir))
                .Select<Item, Item>((Func<Item, Item>)(x => x.Clone()))));
    }
}