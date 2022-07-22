using System.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public override void Load()
    {
        base.Load();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        base.HandlePacket(reader, whoAmI);
    }

    public override void Unload()
    {
        base.Unload();
    }
}

public enum PKG_ID
{
    
    
}