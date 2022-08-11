using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace SSC;

public class SSC : Mod
{
    public enum ID
    {
        Clean,
        AskList,
        List,
        AskCreate,
        AskDelete,
        AskSelect,
        Select,
        AskSave,
    }

    public override void Load()
    {
        Main.runningCollectorsEdition = ModContent.GetInstance<MainConfig>().CollectorsEdition;
        Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", "Temp"));
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive packet {(ID)type} from {from}");

        switch ((ID)type)
        {
            case ID.Clean:
            {
                var i = b.ReadInt32();
                var id = b.ReadUInt64();

                Main.player[i] = new Player
                {
                    name = id.ToString(), difficulty = (byte)Main.GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                if (i == Main.myPlayer)
                {
                    var data = new PlayerFileData
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = Main.player[i]
                    };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", id.ToString()));
                    if (!ModContent.GetInstance<MainConfig>().RepeatConnect)
                    {
                        if (Main.player.Any(who => who.active && who.GetModPlayer<MainPlayer>().SteamID == id))
                        {
                            SSCTools.Boot(i, "Repeat connect. Can be modified on the config.");
                            return;
                        }
                    }

                    Main.player[i].GetModPlayer<MainPlayer>().SteamID = id;

                    var p = GetPacket();
                    p.Write((byte)SSC.ID.Clean);
                    p.Write(i);
                    p.Write(id);
                    p.Send(i);
                    p.Send();
                }

                break;
            }
            case ID.AskList:
            {
                var id = b.ReadUInt64();
                var dir = Path.Combine(Main.SavePath, "SSC", id.ToString());
                var array = Directory.GetFiles(dir, "*.plr").ToList();

                var p = ModContent.GetInstance<SSC>().GetPacket();
                p.Write((byte)ID.List);
                p.Write(array.Count);
                array.ForEach(i =>
                {
                    var data = Player.LoadPlayer(i, false);
                    p.Write(data.Player.name);
                    p.Write(data.Player.difficulty);
                    p.Write(data.GetPlayTime().Ticks);
                });
                p.Send(from);
                break;
            }
            case ID.List:
            {
                var count = b.ReadInt32();
                var data = new List<(string, byte, long)>();
                for (var i = 0; i < count; i++)
                {
                    data.Add((b.ReadString(), b.ReadByte(), b.ReadInt64()));
                }

                MainLayout.ResetListItem(data);
                break;
            }
            case ID.AskCreate:
            {
                var id = b.ReadUInt64();
                var dir = Path.Combine(Main.SavePath, "SSC");
                var name = b.ReadString();
                var difficulty = b.ReadByte();

                if (name == "")
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, from);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, from);
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Illegal name"), Color.Red, from);
                    return;
                }

                if (Directory.GetFiles(dir, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, from);
                    return;
                }

                var data = new PlayerFileData(Path.Combine(dir, id.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = difficulty }
                };
                SSCTools.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);

                FileUtilities.ProtectedInvoke(() => typeof(Player).GetMethod("InternalSavePlayerFile",
                    (BindingFlags)40)?.Invoke(null, new object[] { data }));

                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"{id} created a new player: {name}"),
                    Color.Green);

                dir = Path.Combine(dir, id.ToString());
                var array = Directory.GetFiles(dir, "*.plr").ToList();

                var p = ModContent.GetInstance<SSC>().GetPacket();
                p.Write((byte)ID.List);
                p.Write(array.Count);
                array.ForEach(i =>
                {
                    data = Player.LoadPlayer(i, false);
                    p.Write(data.Player.name);
                    p.Write(data.Player.difficulty);
                    p.Write(data.GetPlayTime().Ticks);
                });
                p.Send(from);
                break;
            }
            case ID.AskDelete:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();
                try
                {
                    File.Delete(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr"));
                    File.Delete(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.tplr"));
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Successfully deleted"), Color.Green,
                        from);

                    var dir = Path.Combine(Main.SavePath, "SSC", id.ToString());
                    var array = Directory.GetFiles(dir, "*.plr").ToList();

                    var p = ModContent.GetInstance<SSC>().GetPacket();
                    p.Write((byte)ID.List);
                    p.Write(array.Count);
                    array.ForEach(i =>
                    {
                        var data = Player.LoadPlayer(i, false);
                        p.Write(data.Player.name);
                        p.Write(data.Player.difficulty);
                        p.Write(data.GetPlayTime().Ticks);
                    });
                    p.Send(from);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, from);
                    throw;
                }

                break;
            }
            case ID.AskSelect:
            {
                var id = b.ReadUInt64();
                var dir = Path.Combine(Main.SavePath, "SSC", id.ToString());
                var name = b.ReadString();

                if (Main.player.Any(player => player.active && player.name == name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Current user already in world"),
                        Color.Red, from);
                    return;
                }

                var data = Player.LoadPlayer(Path.Combine(dir, $"{name}.plr"), false);

                if (data.Player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"),
                        Color.Red, from);
                    return;
                }

                if (data.Player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"),
                        Color.Red, from);
                    return;
                }

                Main.player[from] = data.Player;
                Main.player[from].GetModPlayer<MainPlayer>().SteamID = id;

                var path = Path.Combine(dir, $"{name}.plr");
                var compound = new TagCompound();
                compound.Set("Terraria", File.ReadAllBytes(path));
                if (File.Exists(Path.ChangeExtension(path, ".tplr")))
                {
                    compound.Set("tModLoader", File.ReadAllBytes(path));
                }

                var memoryStream = new MemoryStream();
                TagIO.ToStream(compound, memoryStream);
                var bytes = memoryStream.ToArray();

                var p = GetPacket();
                p.Write((byte)SSC.ID.Select);
                p.Write(from);
                p.Write(id);
                p.Write(name);
                p.Write(bytes.Length);
                p.Write(bytes);
                p.Send();
                break;
            }
            case ID.Select:
            {
                var i = b.ReadInt32();
                var id = b.ReadUInt64();
                var name = b.ReadString();
                var compound = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));
                var dir = Path.Combine(Main.SavePath, "SSC", "Temp");

                var path = Path.Combine(dir, $"{name}.plr");
                File.WriteAllBytes(path, compound.GetByteArray("Terraria"));
                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(Path.ChangeExtension(path, ".tplr"), compound.GetByteArray("tModLoader"));
                }

                var data = Player.LoadPlayer(path, false);
                Main.player[i] = data.Player;
                if (i == Main.myPlayer)
                {
                    data.MarkAsServerSide();
                    data.SetAsActive();
                    Main.player[i].Spawn(PlayerSpawnContext.SpawningIntoWorld);
                    Main.player[i].GetModPlayer<MainPlayer>().SteamID = id;
                    MainSystem.UI.SetState(null);
                }

                break;
            }
            case ID.AskSave:
            {
                var id = b.ReadUInt64();
                var name = b.ReadString();
                var compound = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                var path = Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr");
                if (!File.Exists(path))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("User does not exist, saving failed"),
                        Color.Red, from);
                    return;
                }

                if (compound.ContainsKey("Terraria"))
                {
                    File.WriteAllBytes(path, compound.GetByteArray("Terraria"));
                }

                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(Path.ChangeExtension(path, ".tplr"), compound.GetByteArray("tModLoader"));
                }

                break;
            }
            default:
                SSCTools.Boot(from, $"Unexpected packet ID: {type}");
                break;
        }
    }
}