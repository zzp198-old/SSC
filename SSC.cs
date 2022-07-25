using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = (PID)b.ReadByte();
        Logger.Debug($"NetMode: {Main.netMode} id:{Main.myPlayer} receive {type} from {_}");
    }
}

public enum PID : byte
{
}