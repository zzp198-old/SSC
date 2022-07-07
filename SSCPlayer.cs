using System;
using System.IO;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;

namespace ServerMod;

public class SSCPlayer : ModPlayer
{
    public static bool SSCReady;
    public static int SaveCountdown;

    public override void Load()
    {
        On.Terraria.Player.KillMe += On_Player_KillMe;
        IL.Terraria.GameContent.Creative.CreativeUI.SacrificeItem_refItem_refInt32_bool +=
            IL_GameContent_Creative_CreativeUI_SacrificeItem_refItem_refInt32_bool;
        On.Terraria.WorldGen.SaveAndQuit += On_WorldGen_SaveAndQuit;
    }

    private void On_WorldGen_SaveAndQuit(WorldGen.orig_SaveAndQuit orig, Action callback)
    {
        SendSavePacket();
        orig(callback);
    }

    public override void Unload()
    {
        On.Terraria.Player.KillMe -= On_Player_KillMe;
        IL.Terraria.GameContent.Creative.CreativeUI.SacrificeItem_refItem_refInt32_bool -=
            IL_GameContent_Creative_CreativeUI_SacrificeItem_refItem_refInt32_bool;
        On.Terraria.WorldGen.SaveAndQuit -= On_WorldGen_SaveAndQuit;
    }

    public override void OnEnterWorld(Terraria.Player player)
    {
        SSCPlayer.SSCReady = false;
        if (Terraria.Main.netMode != NetmodeID.MultiplayerClient)
            return;
        SSCIO.Reset(player);
        ModPacket packet = this.Mod.GetPacket();
        packet.Write((byte)0);
        packet.Write(Terraria.Main.myPlayer);
        packet.Write(Terraria.Main.clientUUID);
        packet.Send();
    }

    public override void PostUpdate()
    {
        if (!SSCPlayer.SSCReady || ++SSCPlayer.SaveCountdown < 1800)
            return;
        SSCPlayer.SendSavePacket();
        SSCPlayer.SaveCountdown = 0;
    }

    private static void SendSavePacket()
    {
        MemoryStream memoryStream = new MemoryStream();
        TagIO.ToStream(SSCIO.ClientTagCompound(Terraria.Main.LocalPlayer), (Stream)memoryStream);
        byte[] array = memoryStream.ToArray();
        ModPacket packet = ModContent.GetInstance<ServerMod>().GetPacket();
        packet.Write((byte)2);
        packet.Write(Terraria.Main.myPlayer);
        packet.Write(Terraria.Main.clientUUID);
        packet.Write(array.Length);
        packet.Write(array);
        packet.Send();
    }

    private static void On_Player_KillMe(
        On.Terraria.Player.orig_KillMe orig,
        Terraria.Player self,
        PlayerDeathReason damageSource,
        double dmg,
        int hitDirection,
        bool pvp)
    {
        orig(self, damageSource, dmg, hitDirection, pvp);
        if (!SSCPlayer.SSCReady)
            return;
        SSCPlayer.SendSavePacket();
        SSCPlayer.SaveCountdown = 0;
    }

    private static void IL_GameContent_Creative_CreativeUI_SacrificeItem_refItem_refInt32_bool(ILContext il)
    {
        ILCursor ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            (Func<Instruction, bool>)(i => i.MatchLdsfld(typeof(Terraria.Main), "ServerSideCharacter")));
        ilCursor.EmitDelegate<Func<bool, bool>>((Func<bool, bool>)(_ => false));
    }
}