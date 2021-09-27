using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSC : Mod
    {
        private static string SavePath => Path.Combine(Main.SavePath, "SSC");
        public static SSCConfig SSCConfig => ModContent.GetInstance<SSCConfig>();

        public override void Load()
        {
            IL.Terraria.NetMessage.SendData += IL_NetMessage_SendData;
        }

        public override void Unload()
        {
            IL.Terraria.NetMessage.SendData -= IL_NetMessage_SendData;
        }

        private static void IL_NetMessage_SendData(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.GotoNext(i => i.MatchLdsfld(typeof(NPC), nameof(NPC.downedPlantBoss)));
            cursor.GotoNext(i => i.MatchCallvirt(typeof(BinaryWriter), nameof(BinaryWriter.Write)));
            cursor.EmitDelegate<Func<byte, byte>>(i => (byte) (i + 64));
        }

        public override void HandlePacket(BinaryReader reader, int _)
        {
            var messageID = (SSCMessageID) reader.ReadByte();
            if (messageID == SSCMessageID.ClientChange)
            {
                var i = reader.ReadInt32();
                var tagCompound = reader.ReadTagCompound();

                var player = Main.player[i];
                var modPlayer = player.GetModPlayer<SSCPlayer>();
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");
                if (tagCompound.ContainsKey("taxMoney"))
                    player.taxMoney = tagCompound.GetInt("taxMoney");

                // if (!clone.Player..Equals(Player.hbLocked))
                //     tagCompound.Set("hbLocked", Player.hbLocked);
                // if (!clone.Player.hideInfo.SequenceEqual(Player.hideInfo))
                //     tagCompound.Set("hideInfo", Player.hideInfo.ToList());
                // if (!clone.Player.anglerQuestsFinished.Equals(Player.anglerQuestsFinished))
                //     tagCompound.Set("anglerQuestsFinished", Player.anglerQuestsFinished);
                // if (!clone.Player.DpadRadial.Bindings.SequenceEqual(Player.DpadRadial.Bindings))
                //     tagCompound.Set("DpadRadial", Player.DpadRadial.Bindings);
                // if (!clone.Player.builderAccStatus.SequenceEqual(Player.builderAccStatus))
                //     tagCompound.Set("builderAccStatus", Player.builderAccStatus);
                // if (!clone.Player.bartenderQuestLog.Equals(Player.bartenderQuestLog))
                //     tagCompound.Set("bartenderQuestLog", Player.bartenderQuestLog);
                // if (!clone.Player.golferScoreAccumulated.Equals(Player.golferScoreAccumulated))
                //     tagCompound.Set("golferScoreAccumulated", Player.golferScoreAccumulated);

                return;
            }
        }
    }

    public enum SSCMessageID : byte
    {
        ClientChange,
    }
}