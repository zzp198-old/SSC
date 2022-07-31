using System;
using System.IO;
using System.Linq;
using System.Reflection;
using On.Terraria.Net;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public static class NETCore
{
    static Mod Mod => ModContent.GetInstance<SSC>();

    static MethodInfo InternalSavePlayerFile => typeof(Player).GetMethod("InternalSavePlayerFile",
        BindingFlags.NonPublic | BindingFlags.Static);

    internal static void ErasePLR(int whoAmI, string name, byte GameMode = byte.MaxValue, int toClient = -1)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ErasePLR);
        p.Write(whoAmI);
        p.Write(name);
        p.Write(GameMode);
        p.Send(toClient);

        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                p.Send();
                break;
            case NetmodeID.Server:

                break;
            default:
                throw new Exception();
        }
    }

    internal static void HErasePLR(BinaryReader b, int _)
    {
        var whoAmI = b.ReadInt32();
        var name = b.ReadString();
        var GameMode = b.ReadByte();
        GameMode = GameMode == byte.MaxValue ? (byte)Main.GameMode : GameMode;

        Utils.ErasePLR(whoAmI, name, GameMode);

        if (Main.netMode == NetmodeID.Server)
        {
            Netplay.Clients.Where(i => i.IsConnected()).ToList().ForEach(i => ErasePLR(whoAmI, name, GameMode, i.Id));
        }
    }

    internal static void CreatePLR(ulong SteamID, string name, byte GameMode)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.CreatePLR);
        p.Write(SteamID);
        p.Write(name);
        p.Write(GameMode);
        p.Send();
    }

    internal static void HCreatePLR(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var name = b.ReadString();
        var GameMode = b.ReadByte();

        InternalSavePlayerFile.Invoke(null, new object[]
        {
            new PlayerFileData(Path.Combine(SSC.SavePath, SteamID, $"{name}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player()
                {
                    name = name, difficulty = GameMode,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            }
        });
    }

    internal static void DeletePLR(ulong SteamID, string name)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.DeletePLR);
        p.Write(SteamID);
        p.Write(name);
        p.Send();
    }

    internal static void HDeletePLR(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var name = b.ReadString();

        File.Delete(Path.Combine(SSC.SavePath, SteamID, $"{name}.plr"));
        File.Delete(Path.Combine(SSC.SavePath, SteamID, $"{name}.tplr"));
    }

    internal static void ObtainPLR(ulong SteamID, string name)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ObtainPLR);
        p.Write(SteamID);
        p.Write(name);
        p.Send();
    }

    internal static void HObtainPLR(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var name = b.ReadString();
        name = name == "" ? "*" : name;

        foreach (var path in Directory.GetFiles(Path.Combine(SSC.SavePath, SteamID), $"{name}.plr"))
        {
            ByteArray(SteamID, Path.GetFileName(path), File.ReadAllBytes(path), _);
        }
    }

    // 用于Client和Server互相传递byte[]
    internal static void ByteArray(ulong SteamID, string name, byte[] data, int toClient = -1)
    {
        var p = Mod.GetPacket();
        p.Write((byte)PID.ByteArray);
        p.Write(SteamID);
        p.Write(name);
        p.Write(data.Length);
        p.Write(data);
        p.Send(toClient);
    }

    internal static void HByteArray(BinaryReader b, int _)
    {
        var SteamID = b.ReadUInt64().ToString();
        var name = b.ReadString();
        var data = b.ReadBytes(b.ReadInt32());

        File.WriteAllBytes(Path.Combine(SSC.SavePath, SteamID, name), data);
    }
}