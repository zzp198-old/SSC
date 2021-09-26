using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSCPlayer : ModPlayer
    {
        public static bool SSCReady;
        public static int SaveCountdown;

        public override void Load()
        {
            On.Terraria.Player.KillMe += On_Player_KillMe;
            IL.Terraria.GameContent.Creative.CreativeUI.SacrificeItem += IL_GameContent_Creative_CreativeUI_SacrificeItem;
        }

        public override void Unload()
        {
            On.Terraria.Player.KillMe -= On_Player_KillMe;
            IL.Terraria.GameContent.Creative.CreativeUI.SacrificeItem -= IL_GameContent_Creative_CreativeUI_SacrificeItem;
        }

        public override void OnEnterWorld(Player player)
        {
            SSCReady = false;
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            SSCIO.Reset(player);

            var packet = Mod.GetPacket();
            packet.Write((byte) SSCMessageID.Reset);
            packet.Write(Main.myPlayer);
            packet.Write(Main.clientUUID);
            packet.Send();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if (!SSCReady) return;
            if (++SaveCountdown < 1800) return;

            SendSavePacket();

            SaveCountdown = 0;
        }

        private static void SendSavePacket()
        {
            var memoryStream = new MemoryStream();
            TagIO.ToStream(SSCIO.ClientTagCompound(Main.LocalPlayer), memoryStream);
            var bytes = memoryStream.ToArray();

            var packet = ModContent.GetInstance<SSC>().GetPacket();
            packet.Write((byte) SSCMessageID.Save);
            packet.Write(Main.myPlayer);
            packet.Write(Main.clientUUID);
            packet.Write(bytes.Length);
            packet.Write(bytes);
            packet.Send();
        }

        private static void On_Player_KillMe(On.Terraria.Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
        {
            orig(self, damageSource, dmg, hitDirection, pvp);
            if (SSCReady)
            {
                SendSavePacket();
                SaveCountdown = 0;
            }
        }

        // CreativeTracker Local Save
        private static void IL_GameContent_Creative_CreativeUI_SacrificeItem(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.ServerSideCharacter)));
            cursor.EmitDelegate<Func<bool, bool>>(_ => false);
        }
    }
}