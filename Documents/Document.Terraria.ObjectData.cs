using Terraria.ObjectData;

namespace TigerLearning.Documents;

public partial class Document {
    public class TileObjectData_cls {
        public static TileObjectData tileObjectData;
        public static void NewTile1x1() {
            int tileType = 0;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(tileType);
        }
        public static void NewTile2x2() {
            int tileType = 0;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
            TileObjectData.addTile(tileType);
        }
    }
}
