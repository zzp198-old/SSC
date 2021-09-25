using System.IO;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace SSC
{
    public class SSC : Mod
    {
        public static SSCConfig SSCConfig => ModContent.GetInstance<SSCConfig>();

        public override void Load()
        {
            IL.Terraria.MessageBuffer.GetData += IL_MessageBuffer_GetData;
        }

        public override void Unload()
        {
            IL.Terraria.MessageBuffer.GetData -= IL_MessageBuffer_GetData;
        }

        private static void IL_MessageBuffer_GetData(ILContext il)
        {
            var cursor = new ILCursor(il);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var messageID = (SSCMessageID) reader.ReadByte();
            if (messageID == SSCMessageID.RequestSSC)
            {
                return;
            }
        }
    }

    public enum SSCMessageID : byte
    {
        RequestSSC,
    }
}