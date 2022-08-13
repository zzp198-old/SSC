using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public class HookSystem : ModSystem
{
    public override void Load()
    {
        // 用来判断是否启用SSC
        IL.Terraria.Main.DrawInventory += Hook0;
        IL.Terraria.Netplay.InnerClientLoop += Hook2;
        // 在原版需要保存的地方挂载,可以满足大部分情况.(尤其是死亡和退出时,但如果出现崩溃会保存失败,还需要一个定时保存来减少损失)
        On.Terraria.Player.SavePlayer += Hook1;
        // 硬核死亡时,哪怕是云端保存也得给你删干净(坏笑)
        On.Terraria.Player.KillMeForGood += Hook3;
    }

    private void Hook2(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(i => i.MatchCall(typeof(NetMessage), nameof(NetMessage.SendData)));
        c.EmitDelegate(() =>
        {
            // 已经选择了角色并SetAsActive,路径为固定设置,变相实现所有角色共用同一世界地图.
            var data = new PlayerFileData(Path.Combine(Main.SavePath, "SSC", $"{SSC.SteamID}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = SSC.SteamID.ToString(), difficulty = Main.LocalPlayer.difficulty,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                }
            };
            data.MarkAsServerSide();
            data.SetAsActive();
        });
    }

    public override void Unload()
    {
        IL.Terraria.Main.DrawInventory -= Hook0;
        IL.Terraria.Netplay.InnerClientLoop -= Hook2;
        On.Terraria.Player.SavePlayer -= Hook1;
        On.Terraria.Player.KillMeForGood -= Hook3;
    }

    private static void Hook0(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCallvirt(typeof(LocalizedText), "get_Value"));
        c.EmitDelegate<Func<string, string>>(_ => _ + " (SSC)");
    }

    private static void Hook1(On.Terraria.Player.orig_SavePlayer invoke, PlayerFileData data, bool skip)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && SSCSystem.Selected)
        {
            SSC.SendSaveSSC(SSC.SteamID, data.Player);
        }

        invoke(data, skip);
    }

    public override void PreSaveAndQuit()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && SSCSystem.Selected)
        {
            SSC.SendSaveSSC(SSC.SteamID, Main.LocalPlayer);
        }
    }

    private static void Hook3(On.Terraria.Player.orig_KillMeForGood invoke, Player self)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && self.whoAmI == Main.myPlayer)
        {
            var mp = SSC.GetPacket(SSC.ID.RemoveSSC);
            mp.Write(SSC.SteamID);
            mp.Write(self.name);
            mp.Send();
        }

        invoke(self);
    }
}