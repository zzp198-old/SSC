using System;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Social;
using Terraria.Utilities;

namespace QSMod;

public class SSCSystem : ModSystem
{
    public override void PostUpdatePlayers()
    {
        Main.ServerSideCharacter = Main.netMode == NetmodeID.MultiplayerClient;

        FileUtilities.ProtectedInvoke((Action)(() => Player.InternalSavePlayerFile(playerFile)));
    }
}