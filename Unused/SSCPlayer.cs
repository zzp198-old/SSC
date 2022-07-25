// using Terraria;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.ModLoader;
// using Terraria.Social;
//
// namespace SSC;
//
// public class SSCPlayer : ModPlayer
// {
//     public string SteamID;
//
//     public override void OnEnterWorld(Player player)
//     {
//         SteamID = null;
//         if (Main.netMode == NetmodeID.MultiplayerClient)
//         {
//             new PlayerFileData
//             {
//                 Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
//                 Player = new Player
//                 {
//                     name = SocialAPI.Friends.GetUsername(),
//                     difficulty = Player.difficulty,
//                     dead = true,
//                     ghost = true,
//                 }
//             }.SetAsActive();
//
//             var packet = Mod.GetPacket();
//             packet.Write((byte)PID.SyncSteamID);
//             packet.Write(SocialAPI.Friends.GetUsername());
//             packet.Send();
//         }
//     }
// }