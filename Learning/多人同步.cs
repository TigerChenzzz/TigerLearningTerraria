using System.IO;
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
        number: ChangeType, x, y, tileType, style (byte, short, short, short, byte)
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

        // ???
        NetMessage.SendTileSquare(-1, i, j);
    }

    #region ModPacket
    public static void ShowModPacket() {
        // 获取 ModPacket
        ModPacket packet = ModInstance.GetPacket();

        // 写入数据
        packet.Write((byte)MessageTypes.ShowModPacket);
        packet.Write(true);
        packet.Write(20);
        packet.Write7BitEncodedInt(777);

        // 发包
        int toClient = -1, ignoreClient = -1; // 默认值
        packet.Send(toClient, ignoreClient);

        // 发包到其他端后需要重载 Mod.HandlePacket 以处理此包
        // 一般发 ModPacket 会首先额外写入一个 byte 代表包类型, 用枚举记录
        // 然后在 Mod.HandlePacket 中读取这个包类型并做相应处理
    }

    // in Mod.HandlePacket
    // whoAmI: 从哪个端接受到的包, 只在服务端有用, 客户端上此值始终为 255
    public static void HandlePacket(BinaryReader reader, int whoAmI) {
        MessageTypes type = (MessageTypes)reader.ReadByte();
        if (type == MessageTypes.ShowModPacket) {
            reader.ReadBoolean();
            reader.ReadInt32();
            reader.Read7BitEncodedInt();
        }
        // ...

        // 可以使用 switch 书写, 或者使用一个函数数组 List<Action<BinaryReader, int>> 处理
    }
    #endregion

    #region ModPlayer 同步
    public class NetSyncExampleModPlayer : ModPlayer {
        public static readonly string References = """
            https://github.com/tModLoader/tModLoader/wiki/Basic-Netcode
            https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode
            """;

        public int IntValueToBeSync { get; set; }

        /// <summary>
        /// 主要用于新玩家加入时的玩家同步 (全量同步)
        /// 包括将加入的玩家数据同步到服务器和其他客户端, 以及将其他客户端的玩家同步到正在加入的客户端
        /// </summary>
        /// <param name="toWho"></param>
        /// <param name="fromWho"></param>
        /// <param name="newPlayer">在当前客户端玩家加入游戏时为 <see langword="true"/></param>
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            Show((object)base.SyncPlayer);
            SendPacketToSync(toWho, fromWho);
        }
        // 以下两个方法绑定, 需要同时重载

        /// <summary>
        /// <br/>TML 会在本地存一个副本 <see cref="Main.clientPlayer"/>
        /// <br/>当尝试同步玩家时 (<see cref="Main.TrySyncingMyPlayer"/>)
        /// <br/>会调用 <see cref="ModPlayer.SendClientChanges(ModPlayer)"/> 以发送需要的数据,
        /// <br/>然后再使用 <see cref="ModPlayer.CopyClientState(ModPlayer)"/>
        /// <br/>将需要的数据放入 <see cref="Main.clientPlayer"/> 中.
        /// <br/>也就是说在 <see cref="ModPlayer.SendClientChanges(ModPlayer)"/> 中需要
        /// <br/>将当前 <see cref="ModPlayer"/> 与参数内的上一次同步后的玩家做比较,
        /// <br/>然后按需要自行发包同步
        /// </summary>
        public override void CopyClientState(ModPlayer targetCopy) {
            Show((object)base.CopyClientState);
            ((NetSyncExampleModPlayer)targetCopy).IntValueToBeSync = IntValueToBeSync;
        }
        public override void SendClientChanges(ModPlayer clientPlayer) {
            Show((object)base.SendClientChanges);
            if (((NetSyncExampleModPlayer)clientPlayer).IntValueToBeSync != IntValueToBeSync) {
                SendPacketToSync();
            }
        }

        /// <summary>
        /// 使用发包以同步
        /// </summary>
        private static void SendPacketToSync(int toClient = -1, int ignoreClient = -1) {
            ModPacket packet = ModInstance.GetPacket();
            packet.Write((byte)MessageTypes.SyncNetSyncExampleModPlayer);
            // packet.Write(...);
            packet.Send(toClient, ignoreClient);
        }
    }
    #endregion

    public enum MessageTypes : byte {
        ShowModPacket,
        SyncNetSyncExampleModPlayer,
    }
}
