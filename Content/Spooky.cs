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
        self.statLife = 0; // 不为0会让Boss召唤陷入死循环
        self.statMana = 0;
        self.dead = true;
        self.ghost = true;
        self.buffTime[index] = 198;
    }
}