using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace TigerLearning.Learning;

public class 物块基础 {
    /// <summary>
    /// 一些物块相关的属性和方法
    /// </summary>
    public static void ShowTile() {
        #region params
        Tile tile = default;
        #endregion
        #region 物块ID
        Show(tile.TileType);
        Show(TileID.Dirt);//仅限原版
        Show(ModContent.TileType<ModTile>());
        #endregion

    }
    public static string 帧图 = """
        物块都是以16 X 16的规格等分成小块, 每个小块之间都要有2像素的间隔
        物块的帧图无论是竖的还是横的, 都是可以的
        """;
    /// <summary>
    /// 自制物块的物品
    /// </summary>
    public class ExampleTileItem : ModItem {
        public static string 参考 = "Kid: 物块属性讲解一: 基本物块 https://fs49.org/2022/08/26/kid%ef%bc%9a%e7%89%a9%e5%9d%97%e5%b1%9e%e6%80%a7%e8%ae%b2%e8%a7%a3%e4%b8%80%ef%bc%9a%e5%9f%ba%e6%9c%ac%e7%89%a9%e5%9d%97/";
        public override void SetDefaults() {
            //...
            Item.consumable = true;     //当然, 你想不消耗也行, 把consumable改成false就好了
            Item.createTile = ModContent.TileType<ExampleTile>();
            Item.placeStyle = 0;    //决定了放置的物块处于哪个帧, 一般没有多帧的物块都设为0, 但是如果有多个帧, 那么帧数从0开始, 第一个是0, 第二个是1, 以此类推
            //...
        }
    }
    /// <summary>
    /// 自制物块
    /// </summary>
    public class ExampleTile : ModTile {
        public static string 单例 = "每一种ModTile只会存在一个实例, 所有的这种物块统一由这个实例管理";
        public static string 参考 = ExampleTileItem.参考 + "\nKid：物块属性讲解二：TileObjectData https://fs49.org/2022/10/16/kid%ef%bc%9a%e7%89%a9%e5%9d%97%e5%b1%9e%e6%80%a7%e8%ae%b2%e8%a7%a3%e4%ba%8c%ef%bc%9atileobjectdata/";
        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;            //是不是实心的, 能不能被玩家, 弹幕等穿透, 默认false
            Main.tileSolidTop[Type] = true;         //顶端能否站人, 默认false
            Main.tileNoAttach[Type] = false;        //意思是能否在物块附近放东西, 如在上面放个工作台, 旁边放火把等等, 默认false
            Main.tileTable[Type] = false;           //这个物块是不是当做桌子, 这样可以在上面放上瓶子, 旁边放椅子还可以合成各种表, 默认false
            Main.tileLavaDeath[Type] = true;        //物块会不会被岩浆破坏, 被岩浆一冲就会蹦出物品 默认false, 如果有TileObjectData会被其LavaDeath替代
            Main.tileWaterDeath[Type] = true;       //物块是否会被水破坏, 如果有TileObjectData会被其WaterDeath替代
            Main.tileFrameImportant[Type] = true;   //帧对齐, 如果是false, 放置和敲击时就会随机选帧, 所以除非是泥土块之类的单块物块, 一定要把这个值设为true, 默认false
            Main.tileCut[Type] = true;              //会不会被武器, 弹幕破坏, 默认false
            Main.tileBlockLight[Type] = true;       //是否会挡住光源传播 一般物块都会阻挡光源传播 默认false
            Main.tileSpelunker[Type] = true;        //会不会被洞穴探险药水高亮, 默认false
            Main.tileOreFinderPriority[Type] = 114; //金属探测器探测的优先级, 越大越优先, 如果用了这个, 就算 TileID.Sets.Ore[Type] = false; 还是会被金属探测器探测到的哦
            Main.tileShine2[Type] = true;           //会不会被洞穴探险荧光棒高亮, 默认false
            Main.tileShine[Type] = 514;             //闪出小白星的频率, 数字越高越频繁
            Main.tileMergeDirt[Type] = true;        //会不会和土黏在一起, 一般矿石用这个属性, 默认false
            Main.tileNoFail[Type] = true;           //待测试
            Main.tileObsidianKill[Type] = true;     //是否会因为黑曜石在此格生成而被摧毁
            Main.tileLighted[Type] = true;          //是否自发光, 默认false
            Main.tileLighted[Type] = true;          //让他发光, 并且会改变照在它上面的光照, 要配合ModifyLight用
            //ItemDrop = ItemID.DirtBlock;          //挖掉以后出来的物品, 现在会自动设置
            TileID.Sets.Ore[Type] = true;           //这个方块是不是矿石, true的话会被金属探测器探测到, 上面显示的名字是方块的地图名, 默认false, 但若设置为true会固定为1x1物块的读取方式
            TileID.Sets.HasOutlines[Type] = true;   //是否有指示能够交互的轮廓线, 若为true需要有对应图片(图片名字字符串用ModTile.HighlightTexture设置, 默认[物块名]_Highlight)
            TileID.Sets.InteractibleByNPCs[Type] = true;    //NPC是否与之互动(待测试)

            MineResist = 11.4f;                     //要抡几下稿子才能挖掉
            MinPick = 514;                          //最小多少点稿力能挖掉
            DustType = DustID.Stone;                //敲击时出来的粒子,可以用ModContent.DustType<模组粒子>()来设置模组粒子
            AdjTiles = new int[] { TileID.WorkBenches };    //视作哪些工作站

            AddMapEntry(new Color(114, 514, 114), Language.GetText("Mods.ExampleMod.Tiles.ExampleTile.MapEntry"));  //在地图上显示的颜色和名字
            AddMapEntry(new Color(200, 200, 200));  //在地图上显示的颜色


            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top); //自动设置成只能挂在物块下, 并且放置物块的时候鼠标对于物块的位置是上面一格, 并且上面的物块被敲掉, 这个也会掉落
            TileObjectData.newTile.Width             = 3;                //把物块变成 3x1 格
            TileObjectData.newTile.Height            = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };  //每一块的高度
            TileObjectData.newTile.DrawYOffset       = 0;                     //如果你想把底部嵌入下面的方块的话, 就要使用DrawYOffset, 意思是绘制时向下偏移多少像素
            TileObjectData.newTile.DrawXOffset       = 0;                     //这个十分有趣, 在光标显示中是你填的偏移像素, 但是, 你一放下就会变回原样
            TileObjectData.newTile.CoordinateWidth   = 16;                    //所有小块横长全部改为16
            TileObjectData.newTile.CoordinatePadding = 2;                     //意思是你的物块帧图每16格就空2像素
                                                                                // ...
            #region TileObjectData扩展
            TileObjectData.newTile.UsesCustomCanPlace = false;  //这个属性是决定这个物块上是否不能再放其他方块
            TileObjectData.newTile.StyleHorizontal = true;      //true设置是横向帧图, false设置是竖向帧图
            /*
            物块贴图横向有多少帧, 大于这个数字贴图里的帧读取时就会换行读取, 运行时根据这个值确定目标贴图在哪行哪列
            什么？你问我这个有什么用？答案是：tr的图片横向不能超过2048像素, 这样可以保持横向不超2048像素, 
            tr的旗帜就用到了这个属性, 一般你们是用不到这个属性的
            */
            TileObjectData.newTile.StyleWrapLimit = 144;
            /*
            用于设置物块的原点, 也就是放置物块的时候鼠标对于物块的位置
            默认为new Point16(0, 0);也就是从左上角开始
            设为(2, 0)就是从左上角开始, 向左移2格, 即32像素
            */
            TileObjectData.newTile.Origin = Point16.Zero;
            /*
            决定 上/下 方是个什么样的物块才符合放置条件
            第二个参数是一般设为TileObjectData.newTile.Width, 用于配合最后一个参数起始点, 都是物块坐标
            */
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            #endregion
            TileObjectData.addTile(Type);   //把做的更改添加进去

            //在用TileObjectData加进去后再做修改
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;  //是否被岩浆摧毁
        }
        /// <summary>
        /// 修改在摧毁时生成小动物的概率
        /// </summary>
        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance) {
            wormChance = 8;     //具体单位待测试,这里抄的ExMod
        }
        /// <summary>
        /// 修改敲击时出现的粒子个数
        /// </summary>
        public override void NumDust(int i, int j, bool fail, ref int num) {
            //如果fail为真,则代表不是真的在摧毁物块,通常是蠕虫,弹幕等使其产生粒子
            num = fail ? 1 : 3;
        }
        /// <summary>
        /// 修改物块发光强度和颜色<br/>
        /// 应该需要<see cref="Main.tileLighted"/>对应项为<see langword="true"/>
        /// </summary>
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            //i,j: 物块坐标
            //修改rgb来修改其发光强度(0 ~ 1)
            r = g = b = 1f;
        }
        /// <summary>
        /// 当玩家靠近时会发生什么<br/>
        /// <paramref name="closer"/>为<see langword="true"/>时代表音乐盒,时钟等的范围, 否则代表营火, 心灯等的范围(更大)
        /// </summary>
        public override void NearbyEffects(int i, int j, bool closer) {
            base.NearbyEffects(i, j, closer);
        }
        /// <summary>
        /// 当鼠标在物块上时会发生什么<br/>
        /// 一般用来在鼠标上显示点东西
        /// </summary>
        public override void MouseOver(int i, int j) {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }
        /// <summary>
        /// 是否会被智能选择选中, 比如箱子, 默认<see langword="false"/>
        /// </summary>
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        /// <summary>
        /// 在右键时发生什么<br/>
        /// 若完成了物块交互, 返回<see langword="true"/>以避免发生其他右键点击事件(如设置召唤物目标)<br/>
        /// 默认返回<see langword="false"/>
        /// </summary>
        public override bool RightClick(int i, int j) {
            return base.RightClick(i, j);
        }
        /// <summary>
        /// 当电线通过时会发生什么<br/>
        /// TBT 应该是在通电时会发生什么
        /// </summary>
        public override void HitWire(int i, int j) {
            base.HitWire(i, j);
        }
        /// <summary>
        /// 控制物块的帧动画
        /// </summary>
        public override void AnimateTile(ref int frame, ref int frameCounter) {
            //每4游戏帧换一动画帧, 动画帧在8帧中循环
            if(++frameCounter >= 4) {
                frameCounter = 0;
                frame = ++frame % 8;
            }
        }
    }
    public static class 金属探测器优先级 {
        public const int 罐子 = 100;
        public const int 化石 = 150;
        public const int 铜矿 = 200;
        public const int 锡矿 = 210;
        public const int 铁矿 = 220;
        public const int 铅矿 = 230;
        public const int 银矿 = 240;
        public const int 钨矿 = 250;
        public const int 金矿 = 260;
        public const int 铂金矿 = 270;
        public const int 魔矿 = 300;
        public const int 猩红矿 = 310;
        public const int 陨石 = 400;
        public const int 任意宝箱 = 500;
        public const int 生命水晶 = 550;
        public const int 生命水晶巨石 = 550;
        public const int 修复的魔力水晶 = 550;
        public const int 钴矿 = 600;
        public const int 钯金矿 = 610;
        public const int 秘银矿 = 620;
        public const int 山铜矿 = 630;
        public const int 精金矿 = 640;
        public const int 钛金矿 = 650;
        public const int 明胶水晶 = 675;
        public const int 叶绿矿 = 700;
        public const int 奇异植物 = 750;
        public const int 发光郁金香 = 760;
        public const int 生命果 = 810;
    }
}