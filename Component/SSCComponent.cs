using Terraria;
using Terraria.ModLoader.IO;

namespace SSC.Component
{
    public abstract class SSCComponent
    {
        public abstract void Reset(Player player);
        public abstract TagCompound ClientSave(Player player);
        public abstract void ClientLoad(Player player, TagCompound compound);
        public abstract void ServerSave(TagCompound tag);
        public abstract TagCompound ServerLoad();
    }
}