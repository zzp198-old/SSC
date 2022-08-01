using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSC : Mod
{
    public static string SavePath => Path.Combine(Main.SavePath, "SSC");

    public override void Load()
    {
        Main.runningCollectorsEdition = true;
        On.Terraria.Player.KillMeForGood += KillMeForGood;
    }

    void KillMeForGood(On.Terraria.Player.orig_KillMeForGood invoke, Player self)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && self.whoAmI == Main.myPlayer)
        {
            var p = GetPacket();
            p.Write((byte)PID.DeletePLR);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Write(Main.LocalPlayer.name);
            p.Send();
        }

        invoke(self);
    }

    public override void Unload()
    {
        On.Terraria.Player.KillMeForGood -= KillMeForGood;
    }

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(PID)type} from {_}");

        switch ((PID)type)
        {
            case PID.CleanPLR:
            {
                var whoAmI = b.ReadInt32();
                var SteamID = b.ReadUInt64();
                var GameMode = b.ReadByte();
                GameMode = GameMode == byte.MaxValue ? (byte)Main.GameMode : GameMode;
                Main.player[whoAmI] = new Player
                {
                    name = SteamID.ToString(), difficulty = GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                if (whoAmI == Main.myPlayer)
                {
                    var data = new PlayerFileData(Path.Combine(SSC.SavePath, $"{SteamID}.plr"), false)
                    {
                        Player = Main.player[whoAmI]
                    };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    var p = GetPacket();
                    p.Write((byte)PID.CleanPLR);
                    p.Write(whoAmI);
                    p.Write(SteamID);
                    p.Write(GameMode);
                    p.Send(whoAmI);
                    p.Send();
                }
            }
                break;
            case PID.PLRList:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.Server:
                    {
                        var SteamID = b.ReadUInt64();
                        var directory = Path.Combine(SavePath, SteamID.ToString());
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        var FileList = Directory.GetFiles(directory, "*.plr").Select(i => Player.LoadPlayer(i, false)).ToList();
                        var p = GetPacket();
                        p.Write((byte)PID.PLRList);
                        p.Write(FileList.Count);
                        FileList.ForEach(i => TagIO.Write(new TagCompound
                        {
                            { "name", i.Player.name }, { "GameMode", i.Player.difficulty }, { "PlayTime", i.GetPlayTime().Ticks }
                        }, p));
                        p.Send(_);
                        break;
                    }
                    case NetmodeID.MultiplayerClient:
                    {
                        var count = b.ReadInt32();
                        SSCState.PlayerList.Clear();
                        for (var i = 0; i < count; i++)
                        {
                            var compound = TagIO.Read(b);
                            var data = new PlayerFileData
                            {
                                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                                Player = new Player
                                {
                                    name = compound.GetString("name"), difficulty = compound.GetByte("GameMode")
                                }
                            };
                            data.SetPlayTime(new TimeSpan(compound.GetLong("PlayTime")));
                            SSCState.PlayerList.Add(data);
                        }

                        SSCState.Refresh();
                    }
                        break;
                }
            }
                break;
            case PID.CreatePLR:
            {
                var SteamID = b.ReadUInt64();
                var name = b.ReadString();
                var GameMode = b.ReadByte();

                if (name == "")
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, _);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, _);
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Illegal characters in name"), Color.Red, _);
                    return;
                }

                if (!Directory.Exists(Path.Combine(SavePath, SteamID.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(SavePath, SteamID.ToString()));
                }

                if (Directory.GetFiles(SavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already occupied"), Color.Red, _);
                    return;
                }

                var data = new PlayerFileData(Path.Combine(SavePath, SteamID.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player
                    {
                        name = name, difficulty = GameMode,
                        savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                    }
                };
                try
                {
                    Utils.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                    Utils.InternalSavePlayerFile.Invoke(null, new object[] { data });
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Created successfully"), Color.Green, _);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
                }
            }
                break;
            case PID.DeletePLR:
            {
                var SteamID = b.ReadUInt64();
                var name = b.ReadString();
                try
                {
                    File.Delete(Path.Combine(SavePath, SteamID.ToString(), $"{name}.plr"));
                    File.Delete(Path.Combine(SavePath, SteamID.ToString(), $"{name}.tplr"));
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Successfully deleted"), Color.Green, _);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
                    throw;
                }
            }
                break;
            case PID.SelectPLR:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.Server:
                    {
                        var SteamID = b.ReadUInt64();
                        var name = b.ReadString();

                        if (Main.player.Any(player => player.active && player.GetModPlayer<SSCPlayer>().SteamID == SteamID))
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Current steam user is logged in"),
                                Color.Red, _);
                            return;
                        }

                        var data = Player.LoadPlayer(Path.Combine(SavePath, SteamID.ToString(), $"{name}.plr"), false);

                        if (data.Player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"),
                                Color.Red, _);
                            return;
                        }

                        if (data.Player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"),
                                Color.Red, _);
                            return;
                        }

                        Main.player[_] = data.Player;
                        Main.player[_].GetModPlayer<SSCPlayer>().SteamID = SteamID;

                        var path = Path.Combine(SavePath, SteamID.ToString(), $"{name}.plr");
                        var compound = new TagCompound();
                        compound.Set("Terraria", File.ReadAllBytes(path));
                        if (File.Exists(Path.ChangeExtension(path, ".tplr")))
                        {
                            compound.Set("tModLoader", File.ReadAllBytes(path));
                        }

                        var p = GetPacket();
                        p.Write((byte)PID.SelectPLR);
                        p.Write(_);
                        p.Write(SteamID);
                        p.Write(name);
                        TagIO.Write(compound, p);
                        p.Send();
                    }
                        break;
                    case NetmodeID.MultiplayerClient:
                    {
                        var whoAmI = b.ReadInt32();
                        var SteamID = b.ReadUInt64();
                        var name = b.ReadString();
                        var compound = TagIO.Read(b);

                        var directory = Path.Combine(Main.SavePath, "Cache");
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        var path = Path.Combine(directory, $"{name}.plr");
                        File.WriteAllBytes(path, compound.GetByteArray("Terraria"));
                        if (compound.ContainsKey("tModLoader"))
                        {
                            File.WriteAllBytes(Path.ChangeExtension(path, ".tplr"), compound.GetByteArray("tModLoader"));
                        }

                        var data = Player.LoadPlayer(path, false);
                        Main.player[whoAmI] = data.Player;
                        if (whoAmI == Main.myPlayer)
                        {
                            data.MarkAsServerSide();
                            data.SetAsActive();
                            Main.player[whoAmI].Spawn(PlayerSpawnContext.SpawningIntoWorld);
                            Main.player[whoAmI].GetModPlayer<SSCPlayer>().Selected = true;
                            SSCSystem.UI.SetState(null);
                        }
                    }
                        break;
                }
            }
                break;
            case PID.SavePLR:
            {
                var SteamID = b.ReadUInt64();
                var name = b.ReadString();
                var compound = TagIO.Read(b);

                var path = Path.Combine(SavePath, SteamID.ToString(), $"{name}.plr");
                if (!File.Exists(path))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("User does not exist, save failed"), Color.Red, _);
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
            }
                break;
            default:
                throw new Exception($"Unexpected packet id: {type}");
        }
    }
}

public enum PID : byte
{
    CleanPLR,
    PLRList,
    CreatePLR,
    DeletePLR,
    SelectPLR,
    SavePLR,
}