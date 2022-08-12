using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class UISystem : ModSystem
{
    internal static UserInterface UI;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            UI = new UserInterface();
        }
    }

    public override void Unload()
    {
        UI = null;
    }
}