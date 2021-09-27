using System.IO;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace SSC
{
    public static class Extension
    {
        public static byte GetHairDye(this Player player)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            SSCUtil.PlayerIO_Invoke("WriteByteVanillaHairDye", player.hairDye, writer);
            writer.Close();
            return stream.ToArray()[0];
        }

        public static byte[] Save(this CreativeUnlocksTracker tracker)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            tracker.ItemSacrifices.Save(writer);
            writer.Close();
            return stream.ToArray();
        }

        public static byte[] SaveToPlayer(this CreativePowerManager manager, Player player)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            manager.SaveToPlayer(player, writer);
            writer.Close();
            return stream.ToArray();
        }

        public static void WriteTagCompound(this BinaryWriter writer, TagCompound tagCompound)
        {
            var stream = new MemoryStream();
            TagIO.ToStream(tagCompound, stream);
            var bytes = stream.ToArray();
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static TagCompound ReadTagCompound(this BinaryReader reader)
        {
            var stream = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
            return TagIO.FromStream(stream);
        }
    }
}