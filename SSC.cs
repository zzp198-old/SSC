using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, "SSC");

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(PID)type} from {_}");

        switch ((PID)type)
        {
            case PID.ErasePLR:
                NETCore.HErasePLR(b, _);
                break;
            case PID.CreatePLR:
                NETCore.HCreatePLR(b, _);
                break;
            case PID.DeletePLR:
                NETCore.HDeletePLR(b, _);
                break;
            case PID.ObtainPLR:
                NETCore.HObtainPLR(b, _);
                break;
            default:
                Utils.Boot(_, $"Unexpected packet: {type}");
                throw new Exception();
        }
    }
}

public enum PID : byte
{
    ErasePLR,
    CreatePLR,
    DeletePLR,
    ObtainPLR,
    ByteArray,
    
}