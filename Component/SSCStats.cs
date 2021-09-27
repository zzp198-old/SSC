using Terraria;
using Terraria.ModLoader.IO;

namespace SSC.Component
{
    public class SSCStats : SSCComponent
    {
        public static string Cache;

        public override void Reset(Player player)
        {
            player.statLife = player.statLifeMax = 100;
            player.statMana = player.statManaMax = 20;
        }

        public override TagCompound ClientSave(Player player)
        {
            var compound = new TagCompound();
            compound.Set("StatLife", player.statLife);
            compound.Set("StatLifeMax", player.statLifeMax);
            compound.Set("StatMana", player.statMana);
            compound.Set("StatManaMax", player.statManaMax);

            return compound;
        }

        public override void ClientLoad(Player player, TagCompound compound)
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

        public override void ServerSave(TagCompound tag)
        {
        }

        public override TagCompound ServerLoad()
        {
            return TagIO.FromFile("");
        }
    }
}