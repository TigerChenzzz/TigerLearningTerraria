using Terraria.DataStructures;
using Terraria.GameContent;

namespace TigerLearning.Documents;

public partial class Document {
    public class TeleportPylonsSystem_cls {
        public static void ShowTeleportPylonsSystem() {
            #region params
            int necessaryNPCCount = 2;
            Player player = null;
            Point16 centerPoint = player.Center.ToTileCoordinates16();
            #endregion
            TeleportPylonsSystem.DoesPositionHaveEnoughNPCs(necessaryNPCCount, centerPoint);    //对应位置附近是否有足够的NPC
        }
    }
}
