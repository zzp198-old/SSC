using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, "SSC");
    static MethodInfo InternalSavePlayerFile => typeof(Player).GetMethod("InternalSavePlayerFile", (BindingFlags)40);

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(PID)type} from {_}");

        switch ((PID)type)
        {

            default:
                throw new Exception($"Unexpected packet id: {type}");
        }
    }
}

public enum PID : byte
{
    SteamInit,
}