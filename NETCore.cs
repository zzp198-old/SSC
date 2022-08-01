using System;
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

namespace SSC;

public static class NETCore
{
    static Mod Mod => ModContent.GetInstance<SSC>();
    static MethodInfo InternalSavePlayerFile => typeof(Player).GetMethod("InternalSavePlayerFile", (BindingFlags)40);

    internal static void CS_ErasePLR(int whoAmI, ulong SteamID, byte GameMode = byte.MaxValue, int toClient = -1)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ErasePLR);
        p.Write(whoAmI);
        p.Write(SteamID);
        p.Write(GameMode);
        p.Send(toClient);
    }

    internal static void H_ErasePLR(BinaryReader b, int _)
    {
        var whoAmI = b.ReadInt32();
        var SteamID = b.ReadUInt64();
        var GameMode = b.ReadByte();
        GameMode = GameMode == byte.MaxValue ? (byte)Main.GameMode : GameMode;

        if (SteamID == 0)
        {
            Utils.Boot(whoAmI, $"Unexpected SteamID: {SteamID}");
            return;
        }

        Utils.ErasePLR(whoAmI, SteamID.ToString(), GameMode);

        if (Main.netMode == NetmodeID.Server)
        {
            CS_ErasePLR(whoAmI, SteamID, GameMode, _);
            CS_ErasePLR(whoAmI, SteamID, GameMode);
        }

        if (Main.netMode == NetmodeID.Server || whoAmI == Main.myPlayer)
        {
            var dir = Path.Combine(SSC.SavePath, SteamID.ToString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    internal static void C_CreatePLR(ulong SteamID, string name, byte GameMode)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.CreatePLR);
        p.Write(SteamID);
        p.Write(name);
        p.Write(GameMode);
        p.Send();
    }

    internal static void H_CreatePLR(BinaryReader b, int _)
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
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("名字中有非法字符"), Color.Red, _);
            return;
        }

        if (Directory.GetFiles(SSC.SavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("名字已被占用"), Color.Red, _);
            return;
        }

        var data = new PlayerFileData(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.plr"), false)
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = new Player
            {
                name = name, difficulty = GameMode,
                savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
            }
        };
        Utils.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
        try
        {
            InternalSavePlayerFile.Invoke(null, new object[] { data });
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("创建成功"), Color.Red, _);
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
        }
    }

    internal static void C_DeletePLR(ulong SteamID, string name)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.DeletePLR);
        p.Write(SteamID);
        p.Write(name);
        p.Send();
    }

    internal static void H_DeletePLR(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64();
        var name = b.ReadString();

        try
        {
            File.Delete(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.plr"));
            File.Delete(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.tplr"));
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("删除成功"), Color.Green, _);
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
            throw;
        }
    }

    internal static void C_ObtainPLR(ulong SteamID, string name = "")
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ObtainPLR);
        p.Write(SteamID);
        p.Write(name);
        p.Send();
    }

    internal static void H_ObtainPLR(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64();
        var name = b.ReadString();
        name = name == "" ? "*" : name;

        try
        {
            Directory.GetFiles(Path.Combine(SSC.SavePath, SteamID.ToString()), $"{name}.*plr").ToList()
                .ForEach(i => CS_ByteArray(SteamID, Path.GetFileName(i), File.ReadAllBytes(i), _));
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
            throw;
        }
    }

    internal static void CS_ByteArray(ulong SteamID, string name, byte[] data, int toClient = -1)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ByteArray);
        p.Write(SteamID);
        p.Write(name);
        p.Write(data.Length);
        p.Write(data);
        p.Send(toClient);
    }

    internal static void H_ByteArray(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var FileName = b.ReadString();
        var data = b.ReadBytes(b.ReadInt32());

        try
        {
            File.WriteAllBytes(Path.Combine(SSC.SavePath, SteamID, FileName), data);
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, _);
            throw;
        }
    }
}