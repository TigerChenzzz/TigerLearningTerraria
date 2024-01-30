using Terraria.DataStructures;

namespace TigerLearning.Learning;

public class 多人同步 {
    public static void 同步物品() {
        #region params
        IEntitySource source = null;
        Vector2 pos = default;
        Rectangle rect = default;
        int itemID = 0, stack = 1;
        #endregion
        #region 超级标准的写法
        int itemWai = Item.NewItem(source, rect, itemID, stack, noBroadcast: false, prefixGiven: -1); //生成一个物品, 在客户端不会自动同步, 但在服务端会, 设置noBroadcast为true以让服务端也不同步
        if(Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemWai); //同步世界中特定物品(itemWai为物品在Main.item数组中的下标, 也是item.whoAmI)
        }
        #endregion

        //在玩家位置生成一个物品, 在多人客户端可以直接调用而不用处理同步
        Main.LocalPlayer.QuickSpawnItem(source, itemID, stack);
    }
    public static void 同步物块() {
        #region params
        int i = 0, j = 0, tileType = 0, style = 0, changeType = 0, pickPower = 0;
        bool fail = false, noItem = false;
        Player player = null;
        #endregion
        /*
        number: ChangeType, x, y, tileType, style
        ChangeType WorldGen.{
            KillTile = 0, PlaceTile = 1, KillWall = 2, PlaceWall = 3, KillTileNoItem = 4,
            PlaceWire = 5, KillWire = 6, PoundTile = 7, PlaceActuator = 8, KillActuator = 9
            PlaceWire2 = 10, KillWire2 = 11, PlaceWire3 = 12, KillWire3 = 13, SlopeTile = 14,
            FrameTrack = 15, PlaceWire4 = 16, KillWire4 = 17 }
        */
        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, changeType, i, j, tileType, style);

        //破坏物块
        #region 破坏物块
        //fail: 是否失败, 若失败则会有粒子效果等影响, 但物块本身不动, 也不需要同步
        //noItem: 是否不会有物品掉落
        WorldGen.KillTile(i, j, fail, false, noItem);
        if(!fail) {     //一般fail是一个指定值, 所以其实是根据fail的值来判断是否写下面这句
            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
        }

        //模拟玩家以多少点镐力来敲这个方块
        //若player是Main.LocalPlayer且非服务端, 则不用同步
        //但是会占用玩家的
        player.PickTile(i, j, pickPower);
        #endregion

        //放置物块
        if(WorldGen.PlaceTile(i, j, tileType, mute: false, forced: false, plr: -1, style)) {  //若forced为false则在对应位置有物块时不会放置且返回false
            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, i, j, tileType, style);
        }
    }
}
