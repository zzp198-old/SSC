using System.IO;

namespace SSC;

public partial class SSC
{
    public enum ID
    {
        AddSSC,
        DelSSC,
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        switch ((ID)type)
        {
            case ID.AddSSC:
                Handle_AddSSC(b, from);
                break;
            case ID.DelSSC:
                break;
            default:
                // TODO
                break;
        }
    }

    private void Handle_AddSSC(BinaryReader b, int from)
    {
        var id = b.ReadUInt64();
    }
}