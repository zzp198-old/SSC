using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSC : Mod
    {
        // tModLoader/SSC/UUID/WID/
        public static string SavePath = Path.Combine(Main.SavePath, "SSC");

        public override void HandlePacket(BinaryReader br, int _)
        {
            var type = (PacketID)br.Read();
            if (type == PacketID.UUID)
            {
                var ds = Directory.GetFiles(Path.Combine(SavePath, br.ReadString()), "*.plr");
                foreach (var d in ds)
                {
                     
                }


                return;
            }

            if (type == PacketID.SSCList)
            {
                Main.NewText("123");
                return;
            }
        }
    }

    public enum PacketID
    {
        UUID, //C-S, UUID
        SSCList, //S-C ,
    }
}