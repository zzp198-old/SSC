using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SSC;

public static class SSCUtils
{
    public static void Kick(int i, string tip)
    {
        NetMessage.TrySendData(MessageID.Kick, i, -1, NetworkText.FromLiteral(tip));
    }
}