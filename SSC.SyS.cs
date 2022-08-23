using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

public class SSCSyS : ModSystem
{
    internal UserInterface UI;
    internal TagCompound Database;

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

        new DirectoryInfo(Path.Combine(Main.SavePath, "SSC")).GetDirectories().ToList().ForEach(i =>
        {
            Database.Set(i.Name, new List<TagCompound>());
            i.GetFiles("*.plr").ToList().ForEach(j =>
            {
                var data = Player.LoadPlayer(j.FullName, false);
                Database.Get<List<TagCompound>>(i.Name).Add(new TagCompound
                {
                    { "name", data.Player.name },
                    { "difficulty", data.Player.difficulty },
                });
            });
        });
        TagIO.Write(Database, b);
    }

    public override void NetReceive(BinaryReader b)
    {
        Database = TagIO.Read(b);
    }

    #endregion
}