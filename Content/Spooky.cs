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
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player self, ref int index)
    {
        self.statLife = 0;
        self.statMana = 0;
        self.dead = true;
        self.ghost = true;
        self.buffTime[index] = 198;
    }

    public override void Load()
    {
        On.Terraria.Player.Ghost += On_Player_Ghost; // 幽灵化后依旧更新来保证0血0魔
    }

    public override void Unload()
    {
        On.Terraria.Player.Ghost -= On_Player_Ghost;
    }

    private void On_Player_Ghost(On.Terraria.Player.orig_Ghost invoke, Player self)
    {
        var i = self.FindBuffIndex(Type);
        if (i != -1)
        {
            Update(self, ref i);
        }

        invoke(self);
    }
}