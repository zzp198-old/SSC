using System.IO;
using Terraria.ModLoader.IO;

namespace SSC.Extension
{
    public static class BinaryIO
    {
        public static void Write(this BinaryWriter writer, TagCompound compound)
        {
            var stream = new MemoryStream();
            TagIO.ToStream(compound, stream);
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