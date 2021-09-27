using System;
using System.IO;
using SSC.Extension;
using Terraria;
using Terraria.ModLoader.IO;

namespace SSC.Component
{
    public static class SSCStats
    {
        public static string Cache;

        public static void Reset()
        {
            Main.LocalPlayer.statLife = Main.LocalPlayer.statLifeMax = 100;
            Main.LocalPlayer.statMana = Main.LocalPlayer.statManaMax = 20;
        }

        public static void Request()
        {
            var socket = SSC.Instance.GetPacket();
            socket.Write((byte) SSCMessageID.RequestStats);
            socket.Write(Main.myPlayer);
            socket.Write(Main.clientUUID);
            socket.Send();
        }

        public static TagCompound Extract(string uuid)
        {
            var dir = Path.Combine(SSC.BasePath, Main.worldName, uuid);
            Directory.CreateDirectory(dir);
            var name = Path.Combine(dir, "Stats.dat");
            return File.Exists(name) ? TagIO.FromFile(name) : new TagCompound();
        }

        public static void Receive(Player player, TagCompound compound)
        {
            if (compound.ContainsKey("StatLife"))
            {
                player.statLife = compound.GetInt("StatLife");
            }

            if (compound.ContainsKey("StatLifeMax"))
            {
                player.statLifeMax = compound.GetInt("StatLifeMax");
            }

            if (compound.ContainsKey("StatMana"))
            {
                player.statMana = compound.GetInt("StatMana");
            }

            if (compound.ContainsKey("StatManaMax"))
            {
                player.statManaMax = compound.GetInt("StatManaMax");
            }
        }

        public static void RequestStorage()
        {
            var compound = new TagCompound();
            compound.Set("StatLife", Main.LocalPlayer.statLife);
            compound.Set("StatLifeMax", Main.LocalPlayer.statLifeMax);
            compound.Set("StatMana", Main.LocalPlayer.statMana);
            compound.Set("StatManaMax", Main.LocalPlayer.statManaMax);

            var message = TagIO.Serialize(compound).ToString();
            if (string.CompareOrdinal(Cache, message) == 0) return;
            Cache = message;
            var socket = SSC.Instance.GetPacket();
            socket.Write((byte) SSCMessageID.StorageStats);
            socket.Write(Main.clientUUID);
            socket.Write(compound);
            socket.Send();
        }

        public static void Storage(string uuid, TagCompound compound)
        {
            var name = Path.Combine(SSC.BasePath, Main.worldName, uuid, "Stats.dat");
            TagIO.ToFile(compound, name);
        }
    }
}