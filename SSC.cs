using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace SSC
{
    public class SSC : Mod
    {
        // this._player.name = "";
        // Main.clrInput();
        // UIVirtualKeyboard state = new UIVirtualKeyboard(Lang.menu[45].Value, "", new UIVirtualKeyboard.KeyboardSubmitEvent(this.OnFinishedNamingAndCreating), new Action(this.OnCancledNaming));
        // state.SetMaxInputLength(20);
        // Main.MenuUI.SetState((UIState) state);
        
        public static string SavePath = Path.Combine(Main.SavePath, "SSC");
        public static string CachePath = Path.Combine(SavePath, "Cache");

        public override void HandlePacket(BinaryReader br, int _)
        {
        }
    }

    public enum PKG_ID
    {
        UUID, // C-S, UUID
        SSCList, // S-C, Count,[Size, TagCompoent]
    }
}