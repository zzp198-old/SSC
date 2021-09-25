using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace SSC
{
    public static class SSCIO
    {
        public static void Load(Player player, TagCompound tagCompound)
        {
            if (tagCompound.ContainsKey("Hair"))
            {
                player.hair = tagCompound.GetInt("Hair");
            }

            if (tagCompound.ContainsKey("HairDye"))
            {
                player.hairDye = tagCompound.GetByte("HairDye");
            }

            if (tagCompound.ContainsKey("HairDye"))
            {
                player.hideVisibleAccessory = tagCompound.GetList<bool>("HairDye").ToArray();
            }

            if (tagCompound.ContainsKey("HairDye"))
            {
                player.hideVisibleAccessory = tagCompound.GetList<bool>("HairDye").ToArray();
            }
        }
        
 
    }
}