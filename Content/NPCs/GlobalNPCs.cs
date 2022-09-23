using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Content.NPCs;

public class GlobalNPCs : GlobalNPC
{
    public override void ModifyNPCNameList(NPC npc, List<string> nameList)
    {
        nameList.Add("zzp198");
    }
}