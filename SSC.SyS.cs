using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

public class SSCSyS : ModSystem
{
    internal UserInterface UI;
    internal static TagCompound Database;

    #region UI SyS

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

    public override void UpdateUI(GameTime time)
    {
        if (UI?.CurrentState != null)
        {
            UI.Update(time);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer("Vanilla: SSC", () =>
            {
                if (UI?.CurrentState != null)
                {
                    UI.Draw(Main.spriteBatch, Main.gameTimeCache);
                }

                return true;
            }, InterfaceScaleType.UI));
        }
    }

    #endregion

    #region Net SyS

    public override void NetSend(BinaryWriter b)
    {
        Database = new TagCompound();

        var root = new DirectoryInfo(Path.Combine(Main.SavePath, "SSC"));
        foreach (var plr in root.GetFiles("*.plr", SearchOption.AllDirectories))
        {
            if (plr.Directory == null) return;

            var id = plr.Directory.Name;
            var data = Player.LoadPlayer(plr.FullName, false);
            if (!Database.ContainsKey(id)) Database.Add(id, new List<TagCompound>());
            Database.Get<List<TagCompound>>(id).Add(new TagCompound
            {
                { "name", data.Player.name },
                { "mode", data.Player.difficulty },
            });
        }

        TagIO.Write(Database, b);
    }

    public override void NetReceive(BinaryReader b)
    {
        Database = TagIO.Read(b);
    }

    #endregion
}