using ReLogic.OS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Skies;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.Initializers;
using Terraria.UI;

namespace TigerLearning.Documents;

public partial class Document {
    public class Entity_cls {
        public static Entity entity;
        public static string intro = "Player, Item, NPC的基类";
        public static void ShowEntity() {
            #region params
            Vector2 position = default;
            Rectangle hitbox = default;
            #endregion
            Show(entity.velocity);          //速度
            Show(entity.position);          //位置(世界坐标), 似乎是左上角(TBT)
            Show(entity.active);            //代表此实体是否存在于游戏世界中, 在遍历Main的各实体数组时都需先检查此值是否为真
            Show(entity.Hitbox);            //碰撞箱
            entity.Hitbox.Intersects(hitbox);//碰撞检测
            Show(entity.Center);            //中心位置
            Show(entity.Distance(position));//到点position(世界坐标)的像素距离
            Show(entity.DistanceSQ(position));//距离的平方
            Show(entity.Size);              //大小
            Show(entity.whoAmI);            //在Main的对应实体数组中的序号(Item不适用), 包括Projectile, NPC 和 Player
        }
    }
    public class Player_cls {
        public static Player player;
        public static void ShowPlayer() {
            #region params
            int i = 0, j = 0, distance = 10, pickPower = 0, itemID = 0, buffIndex = 0, buffID = 0, time = 60;
            int minionProjectileId = 0, originalDamageNotScaledByMinionDamage = 0;
            float knockBack = 0;
            Vector2 point = default;
            IEntitySource source = null;
            #endregion
            Show(player.whoAmI);            //玩家在Main.player数组中的下标
            Show(player.inventory);         //库存数组, 包括第一排, 钱币和弹药, 但不包括垃圾桶
            string specialInventorySlot = """
                player.inventory[58]代指鼠标上拿起的物品, 由Main.mouseItem克隆而来。
                    实际上克隆发生在Player.dropItemCheck中, 而这个在失去焦点或者在不能使用物品时不会执行(参见Player.Update)
                如果要对鼠标上的东西作出修改, 不能修改这里, 而需要修改Main.mouseItem。
                另外当鼠标上有物品时, player.HeldItem也是player.inventory[58], 所以
                此时对player.HeldItem作出修改也是没用的(但其他时候就可以对它作出修改)。
                """;
            Show(player.HeldItem);
            string playerHeldItemIntro = """
                player.HeldItem为玩家手上的物品
                (在第一排中选中的, 如果鼠标上有物品, 则是鼠标上的)
                实际上是player.inventory[player.selectedItem]的简称
                """;
            Show(player.numberOfDeathsPVE); //PVE死亡数
            Show(player.numberOfDeathsPVP); //PVP死亡数
            #region 是否拥有物品
            player.HasItem(itemID);
            player.HasItemInAnyInventory(itemID);
            player.HasItemInInventoryOrOpenVoidBag(itemID);
            #endregion
            player.IsWithinSnappngRangeToTile(i, j, distance);      //是否在某物块的一定距离内, i, j为物块坐标, distance为像素距离
            player.Distance(point);         //玩家到点的距离
            player.PickTile(i, j, pickPower);
            #region 控制
            Show(player.controlUp);         //上
            Show(player.controlDown);       //下
            Show(player.controlLeft);       //左
            Show(player.controlRight);      //右
            Show(player.controlJump);       //跳
            Show(player.controlDownHold);   //按住下
            Show(player.controlHook);       //钩子(E)
            Show(player.controlMount);      //坐骑(R)
            Show(player.controlInv);        //背包(待测试Esc)
            Show(player.controlMap);        //地图(M)
            Show(player.controlQuickHeal);  //快速治疗
            Show(player.controlQuickMana);  //快速回魔
            Show(player.controlUseItem);    //使用物品
            Show(player.controlUseTile);    //使用物块
            #endregion
            #region 附近的物块和液体
            Show(player.adjWater);
            Show(player.adjLava);
            Show(player.adjHoney);
            Show(player.adjShimmer);
            Show(player.adjTile[TileID.Sinks]);
            #endregion
            #region 位置
            Show(player.ZoneDungeon);           //地牢
            Show(player.ZoneCorrupt);           //腐化
            Show(player.ZoneHallow);            //神圣
            Show(player.ZoneMeteor);            //陨石
            Show(player.ZoneJungle);            //丛林
            Show(player.ZoneSnow);              //雪原
            Show(player.ZoneCrimson);           //猩红
            Show(player.ZoneWaterCandle);       //水蜡烛
            Show(player.ZonePeaceCandle);       //和平蜡烛
            Show(player.ZoneTowerSolar);        //以下四条为四柱
            Show(player.ZoneTowerVortex);
            Show(player.ZoneTowerNebula);
            Show(player.ZoneTowerStardust);
            Show(player.ZoneDesert);            //沙漠
            Show(player.ZoneGlowshroom);        //发光蘑菇
            Show(player.ZoneUndergroundDesert); //地下沙漠
            Show(player.ZoneSkyHeight);         //太空

            Show(player.ZoneOverworldHeight);   //地上
            Show(player.ZoneDirtLayerHeight);   //地下
            Show(player.ZoneRockLayerHeight);   //洞穴
            Show(player.ZoneUnderworldHeight);  //地狱
            string 更多关于高度的信息 ="参见Main_cls.ShowMain()中的关于高度信息区域";

            Show(player.ZoneBeach);             //沙滩
            Show(player.ZoneRain);              //下雨
            Show(player.ZoneSandstorm);         //沙尘暴
            Show(player.ZoneOldOneArmy);        //旧日军团
            Show(player.ZoneGranite);           //花岗岩
            Show(player.ZoneMarble);            //大理石
            Show(player.ZoneHive);              //蜂巢
            Show(player.ZoneGemCave);           //宝石洞穴
            Show(player.ZoneLihzhardTemple);    //神庙
            Show(player.ZoneGraveyard);         //墓地

            Show(player.ShoppingZone_Forest);   //森林购物区??
            #endregion
            #region Buff
            Show(player.buffTime[buffIndex]);   //某个buff的持续时间
            Show(player.buffImmune[buffID]);    //是否免疫某buff
            Show(player.buffType[buffIndex]);   //获得某个buff的ID
            player.AddBuff(buffID, time);       //增加一个buff, 自动处理免疫, 不同模式下时间增幅, 和重添加逻辑
            player.DelBuff(buffIndex);          //删除对应下标的buff
            player.ClearBuff(buffID);           //删除对应类型的buff
            #endregion
            Show(player.fallStart); //开始掉落的位置, 物块坐标Y
            Show(player.talkNPC);   //在对话的NPC在npc数组中的下标, -1表示没有对话的NPC
            Show(player.TalkNPC);   //在对话的NPC, 若没有则为null
            Show(player.currentShoppingSettings.PriceAdjustment);   //当前商店的定价调整, 1f代表正常, <= 0.9表示足够快乐
            player.SpawnMinionOnCursor(source, player.whoAmI, minionProjectileId, originalDamageNotScaledByMinionDamage, knockBack);
        }
    }
    public class Item_cls {
        public static Item item;
        public static string newItem_func = """
            int NewItem(IEntitySource source, position, type, stack = 1, noBroadcast = false, ...)
            以在世界中新建一个物品, position可以为: Vector2 position; Vector2 pos, Vector2 randomBox, Vector2 pos, int Width, int Height;
            int X, int Y, int Width, int Height 当传入矩形范围时似乎会生成在正中间.
            不应该在多人模式的客户端被调用, 若想要在客户端的代码中生成, 可以用 player.QuickSpawnItem(source, item, stack = 1),
            它会处理多人模式的同步需要.     返回生成的物品在Main.item中的序号
            """;
        public static string constructor = """
            new Item()什么都不会做
            item = new Item(itemId)会设置item的各种基本信息(Item基本的SetDefaults)
            会设置其ModItem, 调用ModItem的AutoDefaults和SetDefaults, 然后是
            关联的GlobalItems的SetDefaults
            """;//构造函数
        public static void ShowItem() {
            #region params
            int x = 0, y = 0, width = 0, height = 0;
            Entity entity = null;
            int itemId = 0;
            Mod mod = null;
            #endregion
            Item.NewItem(new EntitySource_DropAsItem(entity), x, y, width, height, itemId);  //在世界中新建一个物品
            Show(item.type);                //物品id(非原版物品的id会因加载不同的模组产生变化)
            Show(item.ModItem);             //获取对应的ModItem
            Show(item.maxStack);            //最大堆叠数
            item.rare = ItemRarityID.White; //稀有度
            Show(item.value);               //值多少铜币, 为商人的卖价, 由玩家卖出只能获得其 1/5 的钱
            Show(item.Name);                //展示的名字
            Show(item.AffixName());         //带有前缀的名字
            Show(item.HoverName);           //鼠标放在上面显示的名字, 带有前缀和个数
            #region 可使用
            Show(item.useTime);                     //使用时间, 设为item.useAnimation的三分之一可让物品三连发
            Show(item.useAnimation);                //使用动画时间, 一般和使用时间相同(或更大?)
            Show(item.reuseDelay);                  //在useAnimation过完后还需要多久才能重新使用
            Show(item.useTurn);                     //使用时是否可以转身
            item.useStyle = ItemUseStyleID.Swing;   //使用方法
            item.UseSound = SoundID.Item1;          //使用声音
            #endregion
            #region 工具武器
            Show(item.width);                       //碰撞箱宽度
            Show(item.height);                      //碰撞箱高度
            Show(item.damage);                      //伤害
            item.DamageType = DamageClass.Melee;    //伤害类型, 默认DamageClass.Default
            Show(item.knockBack);                   //击退
            Show(item.autoReuse);                   //是否自动连用, 默认false
            Show(item.shoot);                       //射出弹幕的弹幕id
            Show(item.shootSpeed);                  //射出弹幕的速度
            Show(item.ammo);                        //消耗弹药的物品id
            #endregion
            #region 消耗品
            Show(item.consumable);                  //是否可消耗
            Show(item.createTile);                  //放置方块的id
            #endregion
            #region 药水食物
            Show(item.buffType);                    //buff的id
            Show(item.buffTime);                    //buff持续时间
            #endregion
            #region 饰品
            Show(item.accessory);                   //是否是饰品, 影响其是否可装入饰品栏
            Show(item.vanity);                      //是否可放在装饰栏位
            Show(item.defense);                     //增加防御, 装备的防御也是这个
#if Terraria143
            Show(item.canBePlacedInVanityRegardlessOfConditions);   //是否可不计条件的装在装饰栏位
#endif
            #endregion
            #region 商店
            Show(item.buyOnce);             //若为真, 则购买时会消耗商店中物品的stack, 消耗完时就不能买了
            Show(item.shopCustomPrice);     //商店中的自定义价格, 但还会受NPC心情影响
            #endregion
            #region 其他
            Item.buyPrice(0, 0, 50, 0);     //会直接转换为对应的铜币数
            Item.sellPrice(0, 0, 10, 0);    //得到的值为同样参数的buyPrice的五倍
            Show(item.tileWand);                    //作为物品法杖消耗物品的id, 默认-1
            Show(item.IsACoin);                     //是否是钱币
            item.StatsModifiedBy.Add(mod);          //告诉游戏此mod对此物品做出了修改
            ContentSamples.ItemsByType.TryGetValue(itemId, out item);   //当只是要查某个值时用这个而不用new一个item出来
            #endregion
        }
        /// <summary>
        /// 关于用Item.NewItem(...)时的联机同步问题
        /// </summary>
        /// <param name="prefixGiven">当为-1时会自动生成前缀</param>
        /// <param name="noBroadcast">在服务端以外无用, 控制在服务端生成的item是否不通过<see cref="NetMessage.SendData"/>
        /// 和<see cref="MessageID.SyncItem"/>同步. 若为真, 可以在此后调整item了之后再用<see cref="MessageID.SyncItem"/>同步
        /// 在多人联机客户端总是需要手动同步的, 在<see cref="Player.QuickSpawnItem"/>中可以看到例子
        /// Controls whether an item spawned on a server is synced using Terraria.NetMessage.SendData
        /// and Terraria.ID.MessageID.SyncItem. If false, the item will be synced. If true,
        /// the calling code can modify the item instance and then sync the item with Terraria.ID.MessageID.SyncItem.
        /// Has no effect except on server. NewItem running on multiplayer clients always
        /// needs to manually sync, see Terraria.Player.QuickSpawnItem(Terraria.DataStructures.IEntitySource,System.Int32,System.Int32)
        /// source code for an example.
        /// </param>
        public static void NewItemSync(IEntitySource source, Vector2 position, int itemId, int stack = 1, int prefixGiven = 0, bool noBroadcast = false, bool noGrabDelay = true) {
            #region 标准的产生物品并同步
            int itemIndex = Item.NewItem(source, position, itemId, stack, noBroadcast: false, prefixGiven, noGrabDelay);
            //NewItemInner里如果是服务器就会在noBroadcast为false时同步, 在客户端就不会[恼]
            if(Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, noGrabDelay.ToInt());
            }
            #endregion
            #region 在产生物品后做一些修改然后再同步
            int itemIndex2 = Item.NewItem(source, position, itemId, stack, noBroadcast: true, prefixGiven, noGrabDelay);
            Do(Main.item[itemIndex2]);  //在这里可以对此item做一些可以被同步的修改
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex2, noGrabDelay.ToInt());
            #endregion
            #region 在已有一个Item时让它产生在世界中
            Item item = default;    //假设这是从另外一个地方得到的Item
            int itemIndex3 = Item.NewItem(source, position, item, noBroadcast: false, noGrabDelay);
            if(Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex3, noGrabDelay.ToInt());
            }
            //实际上这样产生的物品是由item.Clone()克隆出来的Item
            #endregion

            #region 官方轮子
            //在玩家处产生一个物品并自动同步
            Main.LocalPlayer.QuickSpawnItem(source, itemId, stack);
            #endregion
        }
    }
    public class Projectile_cls {
        public static Projectile projectile;
        public static void ShowProjectile() {
            #region params
            Vector2 worldCoordinates = default, position = default, velocity = default;
            int radius = 0, minI = 0, maxI = 0, minJ = 0, maxJ = 0, damage = 10;
            float knockBack = 3.5f;
            IEntitySource source = null;
            Player player = null;
            #endregion
            Show(projectile.friendly);      //是否能对敌方造成伤害
            Show(projectile.hostile);       //是否能对友善NPC以及玩家造成伤害, 与上一个属性可以共存
            Show(projectile.DamageType);    //伤害类型
            Show(projectile.ignoreWater);   //在水中是否不被减速
            Show(projectile.timeLeft);      //存在时间(以帧计)
            Show(projectile.tileCollide);   //是否不能穿墙
            Show(projectile.aiStyle);       //ai行为, 若自行定义AI()需设置为-1
            Show(projectile.extraUpdates);  //额外更新次数, 弹幕每帧会调用AI()总共 1 + extraUpdates 次
            Projectile.NewProjectile(source, position, velocity, ProjectileID.WoodenArrowFriendly, damage, knockBack, player.whoAmI);
            projectile.ExplodeTiles(worldCoordinates, radius, minI, maxI, minJ, maxJ, projectile.ShouldWallExplode(worldCoordinates, radius, minI, maxI, minJ, maxJ));//包括min和max
        }
    }
    public class NPC_cls {
        public static NPC npc;
        public static void ShowNPC() {
            #region params
            int buffIndex = 0, buffID = 0, time = 0, npcID = 0;
            #endregion
            Show(npc.lifeMax);      //最大生命
            Show(npc.value);        //掉落钱币(?)
            Show(npc.friendly);     //是否友好
            Show(npc.boss);         //是否是boss

            Show(npc.position);     //位置
            #region Buff
            Show(npc.buffTime[buffIndex]);      //某个buff的持续时间
            Show(npc.buffImmune[buffID]);       //是否免疫某buff
            Show(npc.buffType[buffIndex]);      //获得某个buff的ID
            npc.AddBuff(buffID, time);          //增加一个buff, 自动处理免疫, 不同模式下时间增幅, 和重添加逻辑
            npc.DelBuff(buffIndex);             //删除对应下标的buff
            npc.RequestBuffRemoval(buffID);     //删除对应类型的buff
            #endregion
            #region 世界进度
            Show(WorldGen.shadowOrbSmashed);    //是否打碎过暗影珠
            Show(NPC.downedGoblins);            //哥布林
            Show(NPC.downedSlimeKing);          //史王
            Show(NPC.downedBoss1);              //克眼
            Show(NPC.downedBoss2);              //克脑/世吞
            Show(DD2Event.DownedInvasionT1);    //旧日军团T1
            Show(NPC.downedQueenBee);           //蜂后
            Show(NPC.downedBoss3);              //骷髅王
            Show(NPC.downedDeerclops);          //独眼巨鹿
            //Show(Main.hardMode);                //肉后
            Show(NPC.downedPirates);            //海盗
            Show(NPC.downedClown);              //小丑
            Show(DD2Event.DownedInvasionT2);    //旧日军团T2
            Show(NPC.downedQueenSlime);         //史莱姆皇后
            Show(NPC.downedMechBossAny);        //任意新三王
            Show(NPC.downedMechBoss2);          //双子魔眼
            Show(NPC.downedMechBoss1);          //毁灭者
            Show(NPC.downedMechBoss3);          //机械骷髅王
            Show(NPC.downedFishron);            //猪龙鱼公爵
            Show(NPC.downedPlantBoss);          //世花
            Show(DD2Event.DownedInvasionT3);    //旧日军团T3
            Show(NPC.downedEmpressOfLight);     //光之女皇
            Show(NPC.downedGolemBoss);          //石巨人
            Show(NPC.downedMartians);           //火星人
            Show(NPC.downedAncientCultist);     //邪教徒
            Show(NPC.downedFrost);              //冰雪军团
            Show(NPC.downedTowerNebula);        //星云柱
            Show(NPC.downedTowerSolar);         //日耀柱
            Show(NPC.downedTowerStardust);      //星尘柱
            Show(NPC.downedTowerVortex);        //星漩柱
            Show(NPC.downedTowers);             //四柱
            Show(NPC.downedMoonlord);           //月后
            #endregion
            Show(npc.IsShimmerVariant);     //是否为微光转化状态(好像只能给城镇NPC用)
            Show(NPC.AnyNPCs(npcID));       //世界上是否有对应id的npc
            Show(NPC.CountNPCS(npcID));     //世界上有对应id的npc的数量
            Show(NPC.FindFirstNPC(npcID));  //找到NPC在Main.npc中的下标, 若没找到返回-1
        }
    }
    public class Tile_cls {
        public static Tile tile;
        public static void ShowTile() {

        }
    }
    public class Main_cls {
        public static Main main;
        public static void ShowMain() {
            Show(Main.hardMode);                //是否到达肉后
            #region Player
            Show(Main.player);                  //玩家数组
            Show(Main.player[Main.myPlayer]);   //玩家自己
            Show(Main.LocalPlayer);             //玩家自己, 相当于上面那行的简写
            Show(Main.mouseItem);               //鼠标上拿着的物品
            Show(Main.CurrentFrameFlags.SleepingPlayersCount);  //正在睡觉的玩家数量
            Show(Main.CurrentFrameFlags.ActivePlayersCount);    //玩家数量
            #endregion
            #region UI
            Show(Main.playerInventory);         //是否在打开背包的状态
            #endregion
            Show(Main.npc);                     //npc数组
            Show(Main.projectile);              //弹幕数组
            Show(Main.item);                    //在世界中的掉落物数组
            #region 时间
            Show(Main.time);            //自开始一个白天/夜晚经过了多长时间
            Show(Main.dayRate);         //Main.time每帧增加的量
            Show(Main.dayTime);         //是否是白天
            Show(Main.dayLength);       //白天时Main.time的限制值
            Show(Main.nightLength);     //夜晚时Main.time的限制值
            Show(Utils.GetDayTimeAs24FloatStartingFromMidnight());    //获得24小时制时间, 如上午四点半为4.5, 晚上七点半为19.5
            #region 月相
            Show(Main.moonPhase);
            Show(Main.GetMoonPhase());
            Show(MoonPhase.Empty);                  //新月
            Show(MoonPhase.QuarterAtRight);         //峨眉月
            Show(MoonPhase.HalfAtRight);            //上弦月
            Show(MoonPhase.ThreeQuartersAtRight);   //渐盈凸月
            Show(MoonPhase.Full);                   //满月
            Show(MoonPhase.ThreeQuartersAtLeft);    //渐亏凸月
            Show(MoonPhase.HalfAtLeft);             //下弦月
            Show(MoonPhase.QuarterAtLeft);          //残月
            #endregion
            #endregion
            #region 世界模式
            Show(Main.expertMode);              //专家模式, 设置需使用Main.GameModeInfo.IsExpertMode
            Show(Main.masterMode);              //大师模式, 设置需使用Main.GameModeInfo.IsMasterMode
            Show(Main.GameModeInfo.IsJourneyMode);  //旅行模式
            #region Game mode info
            Show(Main.GameModeInfo.DebuffTimeMultiplier);       //debuff时间乘数
            Show(Main.GameModeInfo.EnemyDamageMultiplier);      //敌人伤害乘数
            Show(Main.GameModeInfo.EnemyDefenseMultiplier);     //敌人防御乘数
            Show(Main.GameModeInfo.EnemyMaxLifeMultiplier);     //敌人最大生命乘数
            Show(Main.GameModeInfo.EnemyMoneyDropMultiplier);   //敌人掉钱乘数
            Show(Main.GameModeInfo.KnockbackToEnemiesMultiplier);//对敌人的击退乘数
            Show(Main.GameModeInfo.TownNPCDamageMultiplier);    //城镇NPC伤害乘数
            Show(Main.GameModeInfo.IsJourneyMode);              //旅行模式
            Show(Main.GameModeInfo.IsExpertMode);               //专家模式
            Show(Main.GameModeInfo.IsMasterMode);               //大师模式
            #endregion
            #endregion
            #region tile
            Show(Main.tile);                    //tile二维数组, 用Main.tile[i, j]访问
            Show(Main.tileSolid);               //是否为实心块
            Show(Main.tileSolidTop);            //是否为顶端可站(需对于Main.tileSolid为true?)
            Show(Main.tileMergeDirt);           //是否与土块融合
            Show(Main.tileBlockLight);          //是否发光(?)
            Show(Main.tileLighted);             //是否发光(?)
            Show(Main.tileShine);               //反映微小粒子在物块旁边出现的频率, 此值越大频率越小
            Show(Main.tileFrameImportant);      //是否区分不同帧(?) 作用于用Style的物块(?)
            #endregion
            #region 事件
            Show(Main.invasionType = InvasionID.GoblinArmy);    //哥布林军队
            Show(Main.invasionType = InvasionID.SnowLegion);    //雪人军团
            Show(Main.invasionType = InvasionID.PirateInvasion);//海盗
            Show(Main.invasionType = InvasionID.MartianMadness);//火星人
            Show(Main.raining);                 //下雨
            Show(Main.pumpkinMoon);             //南瓜月
            Show(Main.snowMoon);                //雪月
            Show(Main.slimeRain);               //史莱姆雨
            Show(BirthdayParty.PartyIsUp);      //派对
            Show(BirthdayParty.GenuineParty || BirthdayParty.ManualParty);  //派对
            Show(Sandstorm.Happening);          //沙尘暴
            Show(DD2Event.Ongoing);             //旧日军团
            Show(Main.forceHalloweenForToday);  //强制万圣节
            Show(Main.forceXMasForToday);       //强制圣诞节
            Show(Main.fastForwardTimeToDawn);   //日晷
            Show(Main.sundialCooldown);         //日晷冷却
            Show(Main.fastForwardTimeToDusk);   //月晷
            Show(Main.moondialCooldown);        //月晷冷却
            Show(Main.xMas);                    //圣诞
            Show(Main.halloween);               //万圣
            Show(Main.eclipse);                 //日食
            Show(Main.IsItStorming);            //暴雨(泰拉有这东西? 待测试)(是不是说沙暴啊)
            Show(LanternNight.LanternsUp);      //灯笼夜
            Show(Main.IsItAHappyWindyDay);      //快乐大风天(没见过的东西)
            #endregion
            #region 世界种子
            Show(Main.drunkWorld);              //醉酒
            Show(Main.bottomWorld);             //颠倒
            Show(Main.zenithWorld);             //天顶
            Show(Main.getGoodWorld);            //for the worthy
            Show(Main.tenthAnniversaryWorld);   //十周年
            Show(Main.dontStarveWorld);         //饥荒
            Show(Main.notTheBeesWorld);         //没有蜜蜂
            Show(Main.remixWorld);              //混合
            Show(Main.noTrapsWorld);            //没有陷阱
            #endregion
            #region 关于高度信息
            Show(Main.worldSurface * 0.35); //分隔天空和地上(Sky and Overworld)
            Show(Main.worldSurface);    //地面高度, 分隔地上和地下(Overworld and DirLayer)
            Show(Main.rockLayer);       //分隔泥土层和岩石层(DirtLayer and RockLayer)
            Show(Main.maxTilesY - 200); //分隔岩石层和地狱(RockLayer and Underworld)    (地狱固定200格还是有点欠考虑...不过就这样了)
            #endregion
            #region 其他
            Main.instance.MouseText("some text");   //在鼠标上显示信息(持续一帧)
            #endregion
        }
        public static void ShowMainUpdate() {
            #region params
            //请折叠此段
            Main main = null;
            Game _base = null;
            GameTime gameTime = null;
            bool IsEnginePreloaded = false;
            Action OnEnginePreload = null;
            bool _isDrawingOrUpdating = false;
            bool GameAskedToQuit = false;
            Action Main_OnTickForThirdPartySoftwareOnly = null;
            bool _hasPendingNetmodeChange = false;
            int _targetNetMode = 0;
            float logoRotation = 0f;
            float logoRotationSpeed = 0f;
            float logoScale = 1f;
            Func<bool> Main_CanPauseGame = null;
            Action Main_OnTickForInternalCodeOnly = null;
            double _partialWorldEventUpdates = 0.0;
            Stopwatch _worldUpdateTimeTester = default;
            float cameraLerp = 0f;
            int cameraLerpTimer = 0;
            int cameraLerpTimeToggle = 0;
            #endregion
            //Main.Update(GameTime gameTime)的完整逻辑
            if(!/*main.*/IsEnginePreloaded) {    //Main.IsEnginePreloaded
                IsEnginePreloaded = true;
                /*Main.*/OnEnginePreload?.Invoke(); //Main.OnEnginePreload
            }

            if(!/*main.*/_isDrawingOrUpdating) {
                _isDrawingOrUpdating = true;
                Do(/*main.DoUpdate(ref gameTime)*/() => {
                    Main.gameTimeCache = gameTime;
                    if(Main.showSplash) {
                        ShowMainUpdateAudio();
                        Main.GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                        ChromaInitializer.UpdateEvents();
                        Main.Chroma.Update(Main.GlobalTimeWrappedHourly);
                        return;
                    }

                    PartySky.MultipleSkyWorkaroundFix = true;
                    Main.LocalPlayer.cursorItemIconReversed = false;
                    if(!Main.GlobalTimerPaused) {
                        Main.GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                    }
                    Do(/*main.UpdateCreativeGameModeOverride()*/() => {

                    });
                    Do(/*main.UpdateWorldPreparationState()*/() => {

                    });
                    if(Player.BlockInteractionWithProjectiles > 0 && !Main.mouseRight && Main.mouseRightRelease) {
                        Player.BlockInteractionWithProjectiles--;
                    }
                    PlayerInput.SetZoom_UI();
                    for(int num = Main.DelayedProcesses.Count - 1; num >= 0; num--) {
                        IEnumerator enumerator = Main.DelayedProcesses[num];
                        if(!enumerator.MoveNext())
                            Main.DelayedProcesses.Remove(enumerator);
                    }

                    if(!Main.gameMenu || Main.menuMode != MenuID.FancyUI) {
                        Main.MenuUI.SetState(null);
                    }
                    else {
                        Main.InGameUI.SetState(null);
                    }

                    Main.CurrentInputTextTakerOverride = null;
                    if(!Main.dedServ) {
                        Main.AchievementAdvisor.Update();
                    }

                    PlayerInput.SetZoom_Unscaled();
                    Dos(/*main.MouseOversTryToClear()*/);
                    PlayerInput.ResetInputsOnActiveStateChange();
                    if(!Main.dedServ) {
                        Main_OnTickForThirdPartySoftwareOnly?.Invoke(); //Main.OnTickForThirdPartySoftwareOnly
                    }
                    if(/*Main.*/_hasPendingNetmodeChange) {
                        Main.netMode = /*Main.*/_targetNetMode;
                        _hasPendingNetmodeChange = false;
                    }

                    if(CaptureManager.Instance.IsCapturing) {
                        return;
                    }

                    Main.ActivePlayerFileData?.UpdatePlayTimer();

                    Netplay.UpdateInMainThread();
                    Main.gameInactive = !_base.IsActive;
                    if(Main.changeTheTitle) {
                        Main.changeTheTitle = false;
                        Dos(/*main.SetTitle()*/);
                    }

                    Dos(/*_worldUpdateTimeTester.Restart()*/);
                    if(!WorldGen.gen) {
                        WorldGen.destroyObject = false;
                    }

                    if(Main.gameMenu) {
                        Main.mapFullscreen = false;
                    }

                    Dos(/*main.UpdateSettingUnlocks()*/);
                    if(Main.dedServ) {
                        if(Main.dedServFPS) {
                            Main.updatesCountedForFPS++;
                            if(!Main.fpsTimer.IsRunning)
                                Main.fpsTimer.Restart();

                            if(Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                                Main.dedServCount1 += Main.updatesCountedForFPS;
                                Main.dedServCount2++;
                                float num2 = Main.dedServCount1 / (float)Main.dedServCount2;
                                Console.WriteLine(Main.updatesCountedForFPS + "  (" + num2 + ")");
                                Main.updatesCountedForFPS = 0;
                                Main.fpsTimer.Restart();
                            }
                        }
                        else {
                            if(Main.fpsTimer.IsRunning)
                                Main.fpsTimer.Stop();

                            Main.updatesCountedForFPS = 0;
                        }
                    }

                    Dos(/*LocalizationLoader.Update()*/);
                    Dos(/*main.DoUpdate_AutoSave()*/);
                    if(!Main.dedServ) {
                        ChromaInitializer.UpdateEvents();
                        Main.Chroma.Update(Main.GlobalTimeWrappedHourly);
                        if(Main.superFast) {
                            _base.IsFixedTimeStep = false;
                            Main.graphics.SynchronizeWithVerticalRetrace = false;
                        }
                        else {
                            if(Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                                if(_base.IsActive)
                                    _base.IsFixedTimeStep = false;
                                else
                                    _base.IsFixedTimeStep = true;
                            }
                            else {
                                _base.IsFixedTimeStep = true;
                                Main.graphics.SynchronizeWithVerticalRetrace = true;
                            }

                            Main.graphics.SynchronizeWithVerticalRetrace = true;
                        }

                        if(Main.showSplash) {
                            return;
                        }

                        Main.updatesCountedForFPS++;
                        if(Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                            if(Main.fpsCount >= 30f + 30f * Main.gfxQuality) {
                                Main.gfxQuality += Main.gfxRate;
                                Main.gfxRate += 0.005f;
                            }
                            else if(Main.fpsCount < 29f + 30f * Main.gfxQuality) {
                                Main.gfxRate = 0.01f;
                                Main.gfxQuality -= 0.1f;
                            }

                            if(Main.gfxQuality < 0f)
                                Main.gfxQuality = 0f;

                            if(Main.gfxQuality > 1f)
                                Main.gfxQuality = 1f;

                            if(Main.maxQ && _base.IsActive) {
                                Main.gfxQuality = 1f;
                                Main.maxQ = false;
                            }

                            Main.updateRate = Main.uCount;
                            Main.frameRate = Main.fpsCount;
                            Main.fpsCount = 0;
                            Main.fpsTimer.Restart();
                            Main.updatesCountedForFPS = 0;
                            Main.drawsCountedForFPS = 0;
                            Main.uCount = 0;
                            if(Main.gfxQuality < 0.8f)
                                Main.mapTimeMax = (int)((1f - Main.gfxQuality) * 60f);
                            else
                                Main.mapTimeMax = 0;
                        }

                        if(Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                            Main.UpdateTimeAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
                            if(Main.UpdateTimeAccumulator < 0.01666666753590107 && !Main.superFast) {
                                if(Main.FrameSkipMode == FrameSkipMode.Subtle)
                                    Main.instance.SuppressDraw();

                                return;
                            }

                            gameTime = new GameTime(gameTime.TotalGameTime, new TimeSpan(166666L));
                            Main.UpdateTimeAccumulator -= 0.01666666753590107;
                            Main.UpdateTimeAccumulator = Math.Min(Main.UpdateTimeAccumulator, 0.01666666753590107);
                        }

                        Main.uCount++;
                        Main.drawSkip = false;
                        PlayerInput.AllowExecutionOfGamepadInstructions = true;
                        Dos(/*main.TryPlayingCreditsRoll()*/);
                        PlayerInput.SetZoom_UI();
                        Dos(/*UpdateUIStates(gameTime)*/);
                        PlayerInput.SetZoom_Unscaled();
                        Terraria.Graphics.Effects.Filters.Scene.Update(gameTime);
                        Overlays.Scene.Update(gameTime);
                        LiquidRenderer.Instance.Update(gameTime);
                        ShowMainUpdateAudio();
                        InGameNotificationsTracker.Update();
                        ItemSlot.UpdateInterface();
                        if(Main.teamCooldown > 0)
                            Main.teamCooldown--;

                        Do(/*main.DoUpdate_AnimateBackgrounds()*/() => {

                        });
                        Animation.UpdateAll();
                        if(Main.qaStyle == 1)
                            Main.gfxQuality = 1f;
                        else if(Main.qaStyle == 2)
                            Main.gfxQuality = 0.5f;
                        else if(Main.qaStyle == 3)
                            Main.gfxQuality = 0f;

                        Main.maxDustToDraw = (int)(6000f * (Main.gfxQuality * 0.7f + 0.3f));
                        if(Main.gfxQuality < 0.9)
                            Main.maxDustToDraw = (int)(Main.maxDustToDraw * Main.gfxQuality);

                        if(Main.maxDustToDraw < 1000)
                            Main.maxDustToDraw = 1000;

                        Gore.goreTime = (int)(600f * Main.gfxQuality);
                        if(!WorldGen.gen) {
                            Liquid.cycles = (int)(17f - 10f * Main.gfxQuality);
                            Liquid.curMaxLiquid = (int)(Liquid.maxLiquid * 0.25 + Liquid.maxLiquid * 0.75 * Main.gfxQuality);
                            if(Main.Setting_UseReducedMaxLiquids)
                                Liquid.curMaxLiquid = (int)(2500f + 2500f * Main.gfxQuality);
                        }

                        if(Main.superFast) {
                            Main.graphics.SynchronizeWithVerticalRetrace = false;
                            Main.drawSkip = false;
                        }

                        if(Main.gfxQuality < 0.2)
                            LegacyLighting.RenderPhases = 8;
                        else if(Main.gfxQuality < 0.4)
                            LegacyLighting.RenderPhases = 7;
                        else if(Main.gfxQuality < 0.6)
                            LegacyLighting.RenderPhases = 6;
                        else if(Main.gfxQuality < 0.8)
                            LegacyLighting.RenderPhases = 5;
                        else
                            LegacyLighting.RenderPhases = 4;

                        if(!WorldGen.gen && Liquid.quickSettle) {
                            Liquid.curMaxLiquid = Liquid.maxLiquid;
                            if(Main.Setting_UseReducedMaxLiquids)
                                Liquid.curMaxLiquid = 5000;

                            Liquid.cycles = 1;
                        }

                        if(WorldGen.tenthAnniversaryWorldGen && !Main.gameMenu) {
                            WorldGen.tenthAnniversaryWorldGen = false;
                        }

                        if(WorldGen.drunkWorldGen || WorldGen.remixWorldGen) {
                            if(!Main.gameMenu) {
                                WorldGen.drunkWorldGen = false;
                                WorldGen.remixWorldGen = false;
                                /*main.*/logoRotation = 0f;
                                /*main.*/logoRotationSpeed = 0f;
                                /*main.*/logoScale = 1f;
                            }
                        }
                        else if(Main.gameMenu && Math.Abs(logoRotationSpeed) > 1000f) {
                            logoRotation = 0f;
                            logoRotationSpeed = 0f;
                            logoScale = 1f;
                        }

                        Do(/*main.UpdateOldNPCShop()*/() => {

                        });
                        Main.hasFocus = _base.IsActive;
#if false   // if !NETCORE
                        if (Platform.IsWindows) {
                            Form form = Control.FromHandle(base.Window.Handle) as Form;
                            bool num3 = form.WindowState == FormWindowState.Minimized;
                            bool flag = Form.ActiveForm == form;
                            hasFocus |= flag;
                            if (num3)
                                hasFocus = false;
                        }
#endif

                        if(!Main.hasFocus && Main.netMode == NetmodeID.SinglePlayer) {
                            if(!Platform.IsOSX)
                                _base.IsMouseVisible = true;

                            if(Main.netMode != NetmodeID.Server && Main.myPlayer >= 0)
                                Main.player[Main.myPlayer].delayUseItem = true;

                            Main.mouseLeftRelease = false;
                            Main.mouseRightRelease = false;
                            // Reset TML-introduced extra buttons
                            Main.mouseMiddleRelease = false;
                            Main.mouseXButton1Release = false;
                            Main.mouseXButton2Release = false;

                            if(Main.gameMenu) {
                                Dos(/*main.UpdateMenu()*/);
                            }

                            Main.gamePaused = true;
                            return;
                        }

                        if(!Platform.IsOSX)
                            _base.IsMouseVisible = false;

                        SkyManager.Instance.Update(gameTime);
                        if(!Main.gamePaused)
                            EmoteBubble.UpdateAll();

                        ScreenObstruction.Update();
                        ScreenDarkness.Update();
                        MoonlordDeathDrama.Update();
                        Dos(/*main.DoUpdate_AnimateCursorColors()*/);
                        Dos(/*main.DoUpdate_AnimateTileGlows()*/);
                        Dos(/*main.DoUpdate_AnimateDiscoRGB()*/);
                        Dos(/*main.DoUpdate_AnimateVisualPlayerAura()*/);
                        Dos(/*main.DoUpdate_AnimateWaterfalls()*/);
                        Dos(/*main.DoUpdate_AnimateWalls()*/);
                        Dos(/*main.AnimateTiles()*/);
                        Dos(/*main.DoUpdate_AnimateItemIcons()*/);
                        Dos(/*main.DoUpdate_F10_ToggleFPS()*/);
                        Dos(/*main.DoUpdate_F9_ToggleLighting()*/);
                        Dos(/*main.DoUpdate_F8_ToggleNetDiagnostics()*/);
                        Dos(/*main.DoUpdate_F7_ToggleGraphicsDiagnostics()*/);
                        Dos(/*main.DoUpdate_F11_ToggleUI()*/);
                        Dos(/*main.DoUpdate_AltEnter_ToggleFullscreen()*/);
                        Dos(/*main.DoUpdate_HandleInput()*/);
                        Dos(/*main.DoUpdate_HandleChat()*/);
                        Dos(/*main.DoUpdate_Enter_ToggleChat()*/);
                        if((Main.timeForVisualEffects += 1.0) >= 216000.0)
                            Main.timeForVisualEffects = 0.0;

                        if(Main.gameMenu) {
                            Dos(/*main.UpdateMenu()*/);
                            if(Main.netMode != NetmodeID.Server)
                                return;

                            Main.gamePaused = false;
                        }

                        if(!Main.CanUpdateGameplay && Main.netMode != NetmodeID.Server)
                            return;

                        Main.CheckInvasionProgressDisplay();
                    }

                    Dos(/*main.UpdateWindyDayState()*/);
                    if(Main.netMode == NetmodeID.Server)
                        Main.cloudAlpha = Main.maxRaining;

                    bool isActive = _base.IsActive;
                    if(Main.netMode == NetmodeID.MultiplayerClient) {
                        Dos(/*main.TrySyncingMyPlayer()*/);
                    }

                    if(Main_CanPauseGame()) {  //Main.CanPauseGame()
                        Dos(/*main.DoUpdate_WhilePaused()*/);
                        PlayerLoader.UpdateAutopause(Main.player[Main.myPlayer]);
                        Main.gamePaused = true;
                        return;
                    }

                    Main.gamePaused = false;
                    Main_OnTickForInternalCodeOnly?.Invoke();   //Main.OnTickForInternalCodeOnly()

                    if((Main.dedServ || (Main.netMode != NetmodeID.MultiplayerClient && !Main.gameMenu && !Main.gamePaused)) && Main.AmbienceServer != null)
                        Main.AmbienceServer.Update();

                    WorldGen.BackgroundsCache.UpdateFlashValues();
                    Main.LocalGolfState?.Update();

                    if((isActive || Main.netMode == NetmodeID.MultiplayerClient) && Main.cloudAlpha > 0f)
                        Rain.MakeRain();

                    if(Main.netMode != NetmodeID.MultiplayerClient)
                        Main.instance.updateCloudLayer();

                    if(!(Main.desiredWorldEventsUpdateRate <= 0.0)) {
                        /*main.*/_partialWorldEventUpdates += Main.desiredWorldEventsUpdateRate;
                        Main.worldEventUpdates = (int)_partialWorldEventUpdates;
                        _partialWorldEventUpdates -= Main.worldEventUpdates;
                        for(int i = 0; i < Main.worldEventUpdates; i++) {
                            Main.instance.UpdateWeather(gameTime, i);
                        }
                    }

                    Dos(/*Main.UnpausedUpdateSeed = Utils.RandomNextSeed(Main.UnpausedUpdateSeed)*/);
                    Main.Ambience();
                    if(Main.netMode != NetmodeID.Server) {
                        try {
                            Main.snowing();
                        }
                        catch {
                            if(!Main.ignoreErrors) {
                                throw;
                            }
                        }

                        Sandstorm.EmitDust();
                    }

                    SystemLoader.PreUpdateEntities();
                    if(Main.netMode != NetmodeID.Server) {
                        if(Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0) {
                            Star.UpdateStars();
                            Cloud.UpdateClouds();
                        }
                        else if(Main.shimmerAlpha > 0f) {
                            Star.UpdateStars();
                            int num2 = Main.rand.Next(Main.numStars);
                            if(Main.rand.NextBool(90)) {
                                if(Main.star[num2] != null && !Main.star[num2].hidden && !Main.star[num2].falling) {
                                    Main.star[num2].Fall();
                                }

                                for(int j = 0; j < Main.numStars; j++) {
                                    if(Main.star[j].hidden) {
                                        Star.SpawnStars(j);
                                    }
                                }
                            }
                        }
                    }

                    PortalHelper.UpdatePortalPoints();
                    LucyAxeMessage.UpdateMessageCooldowns();
                    if(Main.instance.ShouldUpdateEntities()) {
                        Do(/*main.DoUpdateInWorld(main._worldUpdateTimeTester)*/() => {
                            Dos(/*main.UpdateParticleSystems()*/);
                            Main.tileSolid[379] = false;
                            Dos(/*NPCShopDatabase.Test()*/);
                            #region UpdatePlayers
                            SystemLoader.PreUpdatePlayers();    //ModSystems.PreUpdatePlayers()
                            int num = 0;
                            int num2 = 0;
                            Main.sittingManager.ClearPlayerAnchors();
                            Main.sleepingManager.ClearPlayerAnchors();
                            for(int i = 0; i < 255; i++) {
                                if(!Main.player[i].active) {
                                    continue;
                                }
                                Dos(() => Main.player[i].Update(i), () => {

                                });
                                if(Main.player[i].active) {
                                    num++;
                                    if(Main.player[i].sleeping.FullyFallenAsleep) {
                                        num2++;
                                    }
                                }
                            }

                            Main.CurrentFrameFlags.ActivePlayersCount = num;
                            Main.CurrentFrameFlags.SleepingPlayersCount = num2;
                            if(Main.netMode != NetmodeID.Server) {
                                int num3 = Main.myPlayer;
                                if(Main.player[num3].creativeGodMode) {
                                    Main.player[num3].statLife = Main.player[num3].statLifeMax2;
                                    Main.player[num3].statMana = Main.player[num3].statManaMax2;
                                    Main.player[num3].breath = Main.player[num3].breathMax;
                                }
                            }
                            SystemLoader.PostUpdatePlayers();   //ModSystems.PostUpdatePlayers()
                            #endregion
                            Dos(/*Main._gameUpdateCount++*/);
                            #region UpdateNPCs
                            SystemLoader.PreUpdateNPCs();
                            NPC.RevengeManager.Update();
                            if(Main.netMode != NetmodeID.MultiplayerClient) {
                                if(Main.remixWorld) {
                                    NPC.SetRemixHax();
                                }

                                try {
                                    NPC.SpawnNPC();
                                }
                                catch {
                                    // ignored
                                }

                                if(Main.remixWorld) {
                                    NPC.ResetRemixHax();
                                }
                            }

                            if(Main.netMode != NetmodeID.MultiplayerClient) {
                                PressurePlateHelper.Update();
                            }

                            for(int j = 0; j < 255; j++) {
                                Main.player[j].nearbyActiveNPCs = 0f;
                                Main.player[j].townNPCs = 0f;
                            }
                            Dos(/*Main.CheckBossIndexes()*/);
                            Main.sittingManager.ClearNPCAnchors();
                            Main.sleepingManager.ClearNPCAnchors();
                            NPC.taxCollector = false;
                            NPC.ClearFoundActiveNPCs();
                            NPC.UpdateFoundActiveNPCs();
                            FixExploitManEaters.Update();
                            if(Main.netMode != NetmodeID.MultiplayerClient) {
                                Main.BestiaryTracker.Sights.ScanWorldForFinds();
                            }

                            bool anyActiveBossNPC = false;
                            if(NPC.offSetDelayTime > 0) {
                                NPC.offSetDelayTime--;
                            }

                            if(Main.remixWorld && NPC.empressRageMode && !NPC.AnyNPCs(636)) {
                                NPC.empressRageMode = false;
                            }

                            if(Main.netMode != NetmodeID.MultiplayerClient && Main.afterPartyOfDoom && !BirthdayParty.PartyIsUp) {
                                for(int k = 0; k < 200; k++) {
                                    NPC nPC = Main.npc[k];
                                    if(nPC.active && nPC.townNPC && nPC.type != NPCID.OldMan && nPC.type != NPCID.SkeletonMerchant && nPC.type != NPCID.TravellingMerchant) {
                                        Dos(/*nPC.StrikeNPCNoInteraction(9999, 10f, -nPC.direction)*/);
                                        if(Main.netMode == NetmodeID.Server) {
                                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, k, 9999f, 10f, -nPC.direction);
                                        }
                                    }
                                }

                                NPC.unlockedPartyGirlSpawn = false;
                                NPC.unlockedPrincessSpawn = false;
                                NPC.unlockedSlimeRainbowSpawn = false;
                                NPC.unlockedSlimeGreenSpawn = false;
                                Main.afterPartyOfDoom = false;
                            }

                            if(NPC.brainOfGravity >= 0 && NPC.brainOfGravity < 200 && (!Main.npc[NPC.brainOfGravity].active || Main.npc[NPC.brainOfGravity].type != NPCID.BrainofCthulhu)) {
                                NPC.brainOfGravity = -1;
                            }

                            for(int l = 0; l < 200; l++) {
                                if(Main.ignoreErrors) {
                                    try {
                                        Main.npc[l].UpdateNPC(l);
                                        if(Main.npc[l].active && (Main.npc[l].boss || NPCID.Sets.DangerThatPreventsOtherDangers[Main.npc[l].type])) {
                                            anyActiveBossNPC = true;
                                        }
                                    }
                                    catch(Exception) {
                                        Main.npc[l] = new NPC();
                                    }
                                }
                                else {
                                    Main.npc[l].UpdateNPC(l);
                                }
                            }

                            Main.CurrentFrameFlags.AnyActiveBossNPC = anyActiveBossNPC;
                            SystemLoader.PostUpdateNPCs();
                            #endregion
                            #region UpateGores
                            SystemLoader.PreUpdateGores();
                            for(int m = 0; m < 600; m++) {
                                if(Main.ignoreErrors) {
                                    try {
                                        Main.gore[m].Update();
                                    }
                                    catch {
                                        Main.gore[m] = new Gore();
                                    }
                                }
                                else {
                                    Main.gore[m].Update();
                                }
                            }
                            SystemLoader.PostUpdateGores();
                            #endregion
                            #region UpdateProjectiles
                            SystemLoader.PreUpdateProjectiles();
                            LockOnHelper.SetUP();
                            Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = false;
                            Dos(/*main.PreUpdateAllProjectiles()*/);
                            for(int n = 0; n < 1000; n++) {
                                Main.ProjectileUpdateLoopIndex = n;
                                if(Main.ignoreErrors) {
                                    try {
                                        Main.projectile[n].Update(n);
                                    }
                                    catch {
                                        Main.projectile[n] = new Projectile();
                                    }
                                }
                                else {
                                    Main.projectile[n].Update(n);
                                }
                            }

                            Main.ProjectileUpdateLoopIndex = -1;
                            Dos(/*main.PostUpdateAllProjectiles()*/);
                            LockOnHelper.SetDOWN();
                            SystemLoader.PostUpdateProjectiles();
                            #endregion
                            #region UpdateItems
                            SystemLoader.PreUpdateItems();
                            Item.numberOfNewItems = 0;
                            for(int num4 = 0; num4 < 400; num4++) {
                                if(Main.ignoreErrors) {
                                    try {
                                        Main.item[num4].UpdateItem(num4);
                                    }
                                    catch {
                                        Main.item[num4] = new Item();
                                    }
                                }
                                else {
                                    Main.item[num4].UpdateItem(num4);
                                }
                            }
                            SystemLoader.PostUpdateItems();
                            #endregion
                            #region UpdateDusts
                            SystemLoader.PreUpdateDusts();
                            if(Main.ignoreErrors) {
                                try {
                                    Dust.UpdateDust();
                                }
                                catch {
                                    for(int num5 = 0; num5 < 6000; num5++) {
                                        Main.dust[num5] = new() {
                                            dustIndex = num5
                                        };
                                    }
                                }
                            }
                            else {
                                Dust.UpdateDust();
                            }
                            SystemLoader.PostUpdateDusts();
                            #endregion
                            if(Main.netMode != NetmodeID.Server) {
                                CombatText.UpdateCombatText();
                                PopupText.UpdateItemText();
                            }
                            #region UpdateTime
                            SystemLoader.PreUpdateTime();
                            Dos(/*Main.UpdateTime()*/);
                            SystemLoader.PostUpdateTime();
                            #endregion
                            Main.tileSolid[379] = true;
                            if(Main.gameMenu && Main.netMode != NetmodeID.Server) {
                                return;
                            }

                            if(Main.netMode != NetmodeID.MultiplayerClient) {
                                WorldGen.UpdateWorld();
                                Dos(/*Main.UpdateInvasion()*/);
                            }

                            if(Main.netMode == NetmodeID.Server) {
                                Dos(/*Main.UpdateServer()*/);
                            }

                            if(Main.netMode == NetmodeID.MultiplayerClient) {
                                Dos(/*Main.UpdateClient()*/);
                            }

                            SystemLoader.PostUpdateEverything();
                            Main.chatMonitor.Update();
                            Main.upTimer = (float)_worldUpdateTimeTester.Elapsed.TotalMilliseconds;
                            if(Main.upTimerMaxDelay > 0f) {
                                Main.upTimerMaxDelay -= 1f;
                            }
                            else {
                                Main.upTimerMax = 0f;
                            }

                            if(Main.upTimer > Main.upTimerMax) {
                                Main.upTimerMax = Main.upTimer;
                                Main.upTimerMaxDelay = 400f;
                            }

                            Chest.UpdateChestFrames();
                            Dos(/*main._ambientWindSys.Update()*/);
                            Main.instance.TilesRenderer.Update();
                            Main.instance.WallsRenderer.Update();
                            if(/*Main.*/cameraLerp > 0f) {
                                /*Main.*/cameraLerpTimer++;
                                if(cameraLerpTimer >= /*Main.*/cameraLerpTimeToggle) {
                                    cameraLerp += ((cameraLerpTimer - cameraLerpTimeToggle) / 3 + 1) * 0.001f;
                                }

                                if(cameraLerp > 1f) {
                                    cameraLerp = 1f;
                                }
                            }
                        });
                    }

                    if(Main.netMode != NetmodeID.Server) {
                        Main.ChromaPainter.Update();
                    }
                });//这里会捕捉所有报错并Log出来(Logging.Terraria.Error(e))
                Dos(() => CinematicManager.Instance.Update(gameTime), () => {

                });
                #region 网络发包
                if(Main.netMode == NetmodeID.Server) {
                    for(int i = 0; i < 256; i++) {
                        Netplay.Clients[i].Socket?.SendQueuedPackets();
                    }
                }
                else if(Main.netMode == NetmodeID.MultiplayerClient) {
                    Netplay.Connection.Socket.SendQueuedPackets();
                }
                #endregion
                _isDrawingOrUpdating = false;
            }

            Dos(/*base.Update(gameTime)*/);
            Do(/*main.ConsumeAllMainThreadActions()*/() => {

            });
            if(GameAskedToQuit) {   //Main.GameAskedToQuit
                Dos(/*Main.QuitGame()*/);
            }
        }
        public static void ShowMainUpdateAudio() {

        }
        public static void ShowMainDoUpdateInWorldSimple() {
            #region params
            Stopwatch _worldUpdateTimeTester = default;
            #endregion
            Dos(/*main.UpdateParticleSystems()*/);
            Main.tileSolid[379] = false;
            Dos(/*NPCShopDatabase.Test()*/);
            #region UpdatePlayers
            SystemLoader.PreUpdatePlayers();    //ModSystems.PreUpdatePlayers()
            int num = 0;
            int num2 = 0;
            Main.sittingManager.ClearPlayerAnchors();
            Main.sleepingManager.ClearPlayerAnchors();
            for(int i = 0; i < 255; i++) {
                if(!Main.player[i].active) {
                    continue;
                }
                Dos(() => Main.player[i].Update(i), () => {

                });
            }
            Main.player.Where(p => p.active).WithIndex().ForeachDo(p => Dos(() => p.Item2.Update(p.Item1), () => {
                //player.Update():
                //重置运动相关的参数(runAcceleration, gravity等)
                //设置运动相关的参数(根据shimmerWet, wet, vortexDebuff等值)
                //重置Hitbox
                PlayerLoader.PreUpdate(p.Item2);
                p.Item2.UpdateBiomes();
                p.Item2.UpdateMinionTarget();
            }));
            SystemLoader.PostUpdatePlayers();   //ModSystems.PostUpdatePlayers()
            #endregion
            Dos(/*Main._gameUpdateCount++*/);
            #region UpdateNPCs
            SystemLoader.PreUpdateNPCs();
            NPC.RevengeManager.Update();
            if(Main.netMode != NetmodeID.MultiplayerClient) {
                if(Main.remixWorld) {
                    NPC.SetRemixHax();
                }

                try {
                    NPC.SpawnNPC();
                }
                catch {

                }

                if(Main.remixWorld) {
                    NPC.ResetRemixHax();
                }
            }

            if(Main.netMode != NetmodeID.MultiplayerClient) {
                PressurePlateHelper.Update();
            }

            for(int j = 0; j < 255; j++) {
                Main.player[j].nearbyActiveNPCs = 0f;
                Main.player[j].townNPCs = 0f;
            }
            Dos(/*Main.CheckBossIndexes()*/);
            Main.sittingManager.ClearNPCAnchors();
            Main.sleepingManager.ClearNPCAnchors();
            NPC.taxCollector = false;
            NPC.ClearFoundActiveNPCs();
            NPC.UpdateFoundActiveNPCs();
            FixExploitManEaters.Update();
            if(Main.netMode != NetmodeID.MultiplayerClient) {
                Main.BestiaryTracker.Sights.ScanWorldForFinds();
            }

            bool anyActiveBossNPC = false;
            if(NPC.offSetDelayTime > 0) {
                NPC.offSetDelayTime--;
            }

            if(Main.remixWorld && NPC.empressRageMode && !NPC.AnyNPCs(636)) {
                NPC.empressRageMode = false;
            }

            if(Main.netMode != NetmodeID.MultiplayerClient && Main.afterPartyOfDoom && !BirthdayParty.PartyIsUp) {
                for(int k = 0; k < 200; k++) {
                    NPC nPC = Main.npc[k];
                    if(nPC.active && nPC.townNPC && nPC.type != NPCID.OldMan && nPC.type != NPCID.SkeletonMerchant && nPC.type != NPCID.TravellingMerchant) {
                        Dos(/*nPC.StrikeNPCNoInteraction(9999, 10f, -nPC.direction)*/);
                        if(Main.netMode == NetmodeID.Server) {
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, k, 9999f, 10f, -nPC.direction);
                        }
                    }
                }

                NPC.unlockedPartyGirlSpawn = false;
                NPC.unlockedPrincessSpawn = false;
                NPC.unlockedSlimeRainbowSpawn = false;
                NPC.unlockedSlimeGreenSpawn = false;
                Main.afterPartyOfDoom = false;
            }

            if(NPC.brainOfGravity >= 0 && NPC.brainOfGravity < 200 && (!Main.npc[NPC.brainOfGravity].active || Main.npc[NPC.brainOfGravity].type != NPCID.BrainofCthulhu)) {
                NPC.brainOfGravity = -1;
            }

            for(int l = 0; l < 200; l++) {
                if(Main.ignoreErrors) {
                    try {
                        Main.npc[l].UpdateNPC(l);
                        if(Main.npc[l].active && (Main.npc[l].boss || NPCID.Sets.DangerThatPreventsOtherDangers[Main.npc[l].type])) {
                            anyActiveBossNPC = true;
                        }
                    }
                    catch(Exception) {
                        Main.npc[l] = new NPC();
                    }
                }
                else {
                    Main.npc[l].UpdateNPC(l);
                }
            }

            Main.CurrentFrameFlags.AnyActiveBossNPC = anyActiveBossNPC;
            SystemLoader.PostUpdateNPCs();
            #endregion
            #region UpateGores
            SystemLoader.PreUpdateGores();
            for(int m = 0; m < 600; m++) {
                if(Main.ignoreErrors) {
                    try {
                        Main.gore[m].Update();
                    }
                    catch {
                        Main.gore[m] = new Gore();
                    }
                }
                else {
                    Main.gore[m].Update();
                }
            }
            SystemLoader.PostUpdateGores();
            #endregion
            #region UpdateProjectiles
            SystemLoader.PreUpdateProjectiles();
            LockOnHelper.SetUP();
            Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = false;
            Dos(/*main.PreUpdateAllProjectiles()*/);
            for(int n = 0; n < 1000; n++) {
                Main.ProjectileUpdateLoopIndex = n;
                if(Main.ignoreErrors) {
                    try {
                        Main.projectile[n].Update(n);
                    }
                    catch {
                        Main.projectile[n] = new Projectile();
                    }
                }
                else {
                    Main.projectile[n].Update(n);
                }
            }

            Main.ProjectileUpdateLoopIndex = -1;
            Dos(/*main.PostUpdateAllProjectiles()*/);
            LockOnHelper.SetDOWN();
            SystemLoader.PostUpdateProjectiles();
            #endregion
            #region UpdateItems
            SystemLoader.PreUpdateItems();  //ModSystems.PreUpdateItems()
            Item.numberOfNewItems = 0;
            Main.item.WithIndex().ForeachDo(p => Dos(() => p.Item2.UpdateItem(p.Item1), () => {

            }));
            SystemLoader.PostUpdateItems(); //ModSystems.PostUpdateItems()
            #endregion
            #region UpdateDusts
            SystemLoader.PreUpdateDusts();
            Dust.UpdateDust();
            SystemLoader.PostUpdateDusts();
            #endregion
            if(Main.netMode != NetmodeID.Server) {
                CombatText.UpdateCombatText();
                PopupText.UpdateItemText();
            }
            #region UpdateTime
            SystemLoader.PreUpdateTime();
            Dos(/*Main.UpdateTime()*/);
            SystemLoader.PostUpdateTime();
            #endregion
            Main.tileSolid[379] = true;
            if(Main.gameMenu && Main.netMode != NetmodeID.Server) {
                return;
            }
            #region Update world and invasion
            if(Main.netMode != NetmodeID.MultiplayerClient) {
                WorldGen.UpdateWorld();
                Dos(/*Main.UpdateInvasion()*/);
            }
            #endregion
            #region Update server and client
            if(Main.netMode == NetmodeID.Server) {
                Dos(/*Main.UpdateServer()*/);
            }
            if(Main.netMode == NetmodeID.MultiplayerClient) {
                Dos(/*Main.UpdateClient()*/);
            }
            #endregion
            SystemLoader.PostUpdateEverything();    //ModSystems.PostUpdateEverything()
            Main.chatMonitor.Update();
            #region upate Main.upTimer
            Main.upTimer = (float)_worldUpdateTimeTester.Elapsed.TotalMilliseconds;
            if(Main.upTimerMaxDelay > 0f) {
                Main.upTimerMaxDelay -= 1f;
            }
            else {
                Main.upTimerMax = 0f;
            }
            if(Main.upTimer > Main.upTimerMax) {
                Main.upTimerMax = Main.upTimer;
                Main.upTimerMaxDelay = 400f;
            }
            #endregion

            Chest.UpdateChestFrames();
            Dos(/*main._ambientWindSys.Update()*/);
            Main.instance.TilesRenderer.Update();
            Main.instance.WallsRenderer.Update();
        }
    }
    public class WorldGen_cls {
        public static WorldGen worldGen;
        public static void ShowWorldGen() {
            Show(WorldGen.crimson); //是否是猩红(否则为腐化)
        }
    }
    public class Recipe_cls {
        public static Recipe recipe;
        public static void ShowRecipe() {
            #region params
            ModItem modItem = null;
            string recipeGroupName = null;
            RecipeGroup recipeGroup = null;
            int stack = 1, recipeGroupID = RecipeGroupID.Wood, itemID = 0, amount = 1;
            #endregion
            recipe = Recipe.Create(itemID, amount); //以某一物品为产物的配方, 可以额外传入amount代表产出几个
            recipe = modItem.CreateRecipe(amount); //创建一个以自己为产物的配方, 可以传入amount代表产出几个
            recipe.AddIngredient(ItemID.DirtBlock, 10); //添加一种原料
            recipe.AddTile(TileID.WorkBenches); //添加一种物块作为环境(当然可以添加不止一种)
            recipe.AddCondition(Condition.NearWater);   //添加一个条件(此处为在水附近)
            recipe.AddRecipeGroup(recipeGroupName, stack);  //向合成表添加入一个合成组
            recipe.AddRecipeGroup(recipeGroup, stack);
            recipe.AddRecipeGroup(recipeGroupID, stack);
            Show(recipe.createItem);    //可以直接对此进行修改以修改产物, 实际的产物会从它Clone出来
            Show(recipe.requiredItem);  //在游戏完成添加Recipe前可以自由修改, 之后就不要随便改了
            //此处可以对所消耗物品的数量进行修改(最好是调小)
            recipe.AddConsumeItemCallback((Recipe recipe, int type, ref int amount) => { });
            recipe.AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy); //让每一种材料的每一个物品都有 1/3 的机会不被消耗
            /*
            在制作时做一些事
            consumedItems: 似乎只是克隆出来的, 修改它没法做到修改消耗物
            item: 产物, 是createItem克隆出来的物品, 可以随意修改, 会反映到结果上, 但是还没制作时看不出来
            destinationStack: 往什么上叠, 一般是Main.mouseItem
            */
            recipe.AddOnCraftCallback((recipe, item, consumedItems, destinationStack) => { });
            recipe.Register();  //使用以在游戏中添加此配方

            Show(recipe.RecipeIndex);     //在Main.recipe中的序号, 在被注册(Register)之前为 -1
            Show(Main.recipe);
            //Show(RecipeLoader.FirstRecipeForItem)  //这么好用的东西竟然是internal的, 可惜
        }
    }
    public class RecipeGroup_cls {
        public static RecipeGroup recipeGroup;
        public static void ShowRecipeGroup() {
            string name = null;
            Show(RecipeGroup.recipeGroupIDs);                      //从RecipeGroup的名字转化为id
            Show(RecipeGroup.recipeGroups); //冲id转化为RecipeGroup
            RecipeGroup.RegisterGroup(name, recipeGroup);                    //加载这个合成组, 返回此合成组id(?)

        }
    }
    public class Lighting_cls {
        public static void ShowLighting() {
            #region params
            int x = 0, y = 0, torchID = 0, i = 0, j = 0;
            float lightAmount = 1, r = 0, g = 0, b = 0;
            Vector2 position = default;
            Vector3 rgb = default;
            #endregion
            Lighting.Brightness(x, y);  //获取某处的亮度, 暂不清楚是世界坐标还是物块坐标
            #region AddLight(position, rgb / torchID)     //添加一个光亮, 似乎是瞬时的
            Lighting.AddLight(position, TorchID.Torch);
            Lighting.AddLight(position, rgb);           //暂不清楚rgb的单位
            Lighting.AddLight(i, j, torchID, lightAmount);
            Lighting.AddLight(position, r, g, b);
            Lighting.AddLight(i, j, r, g, b);
            #endregion
            Lighting.GetColor(x, y);    //获取某处(物块坐标?)的颜色
            Show(Lighting.GlobalBrightness);  //整体亮度
        }
    }
    public class Condition_cls {
        public static Condition condition;
        public static void ShowCondition() {
            #region params
            string LocalizationKey = null;
            int itemID = 0, score = 0, npcID = 0;
            #endregion
            Condition conditionFullMoon = new(LocalizationKey, () => Main.moonPhase == (int)MoonPhase.Full);
            condition.IsMet();  //是否满足条件, 同Predict()
            Show(Condition.NearWater);
            Show(Condition.TimeDay);
            Show(Condition.InDungeon);
            Show(Condition.InClassicMode);
            Show(Condition.DrunkWorld);
            Show(Condition.DownedKingSlime);
            Show(Condition.MoonPhaseFull);
            Show(Condition.PlayerCarriesItem(itemID));
            Show(Condition.GolfScoreOver(score));
            Show(Condition.NpcIsPresent(npcID));
        }
    }
}
