using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QOS.Common.Systems;

public class SmoothSystem : ModSystem
{
    // public override bool IsLoadingEnabled(Mod mod)
    // {
    //     return !Main.dedServ;
    // }

    public override void Load()
    {
        // TODO
    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void PostUpdatePlayers()
    {
    }
}