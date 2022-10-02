using Terraria.ModLoader.Config;

namespace SSC.Common.Configs.Elements;

public class ItemsDefinition
{
    public ItemDefinition Item = new();
    public int Stack = 0;
    public override string ToString() => $"{(Item.Mod == nameof(Terraria) ? Item.Name : Item)} ({Stack})";
}