﻿using System;
using System.Collections.Generic;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class ILSystem : ModSystem
{
    public override void Load()
    {
        // 此时已经选择玩家,并对其他玩家进行了初始化.通过IL挂载可确保仅被执行一次.
        IL.Terraria.Netplay.InnerClientLoop += ILHook1;
        // 隐藏其他无关紧要的UI,防止因为提前操作导致数据丢失.
        IL.Terraria.Main.DrawInterface += ILHook2;
        // 显示SSC是否启用
        IL.Terraria.Main.DrawInventory += ILHook3;
    }

    public override void Unload()
    {
        IL.Terraria.Netplay.InnerClientLoop -= ILHook1;
        IL.Terraria.Main.DrawInterface -= ILHook2;
        IL.Terraria.Main.DrawInventory -= ILHook3;
    }

    private static void ILHook1(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(i => i.MatchLdstr("Net.FoundServer"));
        c.EmitDelegate(() =>
        {
            Main.statusText = "SSC Hooking...";
            var data = new PlayerFileData(Path.Combine(SSC.SavePath, $"{SSC.SteamID}.plr"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = DateTime.UtcNow.Ticks.ToString(),
                    // name = SSC.SteamID.ToString(), TODO
                    difficulty = Main.LocalPlayer.difficulty,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules(),
                }
            };
            data.Player.AddBuff(ModContent.BuffType<Content.Spooky>(), 198); // 重要,用于后续的限制
            data.MarkAsServerSide();
            data.SetAsActive();
        });
    }

    private static void ILHook2(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCall(typeof(SystemLoader), nameof(SystemLoader.ModifyInterfaceLayers)));
        c.EmitDelegate<Func<List<GameInterfaceLayer>, List<GameInterfaceLayer>>>(layers =>
        {
            if (Main.LocalPlayer.HasBuff<Content.Spooky>())
            {
                foreach (var layer in layers)
                {
                    switch (layer.Name)
                    {
                        case "Vanilla: Map / Minimap":
                        case "Vanilla: Resource Bars":
                            layer.Active = false;
                            break;
                        default:
                            layer.Active = layer.Name.StartsWith("Vanilla");
                            break;
                    }
                }
            }

            return layers;
        });
    }

    private static void ILHook3(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchCallvirt(typeof(LocalizedText), "get_Value"));
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC)");
    }
}