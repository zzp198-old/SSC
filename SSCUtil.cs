using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public static class SSCUtil
    {
        public static object PlayerIO_Invoke(string name, params object[] args)
        {
            try
            {
                var type = Assembly.Load("tModLoader").GetType("Terraria.ModLoader.IO.PlayerIO");
                var method = type?.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                return method?.Invoke(null, args);
            }
            catch (Exception e)
            {
                ModContent.GetInstance<SSC>().Logger.Error(e);
                throw;
            }
        }

        public static TagCompound SaveItemWithSlot(Item item, int i)
        {
            var tagCompound = ItemIO.Save(item);
            tagCompound.Set("slot", (short) i);
            return tagCompound;
        }

        public static byte[] TagCompound2Bytes(TagCompound tagCompound)
        {
            var stream = new MemoryStream();
            TagIO.ToStream(tagCompound, stream);
            return stream.ToArray();
        }

        public static TagCompound Bytes2TagCompound(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return TagIO.FromStream(stream);
        }
    }
}