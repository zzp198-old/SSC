using Terraria;
using Terraria.ModLoader;

namespace SSC.Content;

public class Spooky : ModBuff
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Spooky");
        Description.SetDefault("Spooky by SSC");

        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player self, ref int index)
    {
    }
}