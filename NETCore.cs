using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
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

        Utils.ErasePLR(whoAmI, SteamID.ToString(), GameMode);

        if (Main.netMode == NetmodeID.Server)
        {
            CS_ErasePLR(whoAmI, SteamID, GameMode, _);
            CS_ErasePLR(whoAmI, SteamID, GameMode);
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

        InternalSavePlayerFile.Invoke(null, new object[]
        {
            new PlayerFileData(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = name, difficulty = GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            }
        });
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

        File.Delete(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.plr"));
        File.Delete(Path.Combine(SSC.SavePath, SteamID.ToString(), $"{name}.tplr"));
    }

    internal static void C_ObtainPLR(ulong SteamID, string name)
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

        foreach (var path in Directory.GetFiles(Path.Combine(SSC.SavePath, SteamID.ToString()), $"{name}.?plr"))
        {
            CS_ByteArray(SteamID, Path.GetFileName(path), File.ReadAllBytes(path), _);
        }
    }

    // 用于Client和Server互相传递byte[]
    internal static void CS_ByteArray(ulong SteamID, string FileName, byte[] data, int toClient = -1)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ByteArray);
        p.Write(SteamID);
        p.Write(FileName);
        p.Write(data.Length);
        p.Write(data);
        p.Send(toClient);
    }

    internal static void H_ByteArray(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var FileName = b.ReadString();
        var data = b.ReadBytes(b.ReadInt32());

        File.WriteAllBytes(Path.Combine(SSC.SavePath, SteamID, FileName), data);
    }
}