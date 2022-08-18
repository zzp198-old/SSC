using Terraria.ModLoader;

namespace SSC.Common;

public class SaveHook : ILoadable
{
    public void Load(Mod mod)
    {
        // private static void DoUpdate_AutoSave()
        // {
        //     if (!Main.gameMenu && Main.netMode == 1)
        //     {
        //         if (!Main.saveTime.IsRunning)
        //             Main.saveTime.Start();
        //         if (Main.saveTime.ElapsedMilliseconds <= 300000L)
        //             return;
        //         Main.saveTime.Reset();
        //         WorldGen.saveToonWhilePlaying();
        //     }
        //     else if (!Main.gameMenu && (Main.autoSave || Main.netMode == 2))
        //     {
        //         if (!Main.saveTime.IsRunning)
        //             Main.saveTime.Start();
        //         if (Main.saveTime.ElapsedMilliseconds <= 600000L)
        //             return;
        //         Main.saveTime.Reset();
        //         if (Main.netMode != 2)
        //             WorldGen.saveToonWhilePlaying();
        //         WorldGen.saveAndPlay();
        //     }
        //     else
        //     {
        //         if (!Main.saveTime.IsRunning)
        //             return;
        //         Main.saveTime.Stop();
        //     }
        // }
    }

    public void Unload()
    {
     
    }
}