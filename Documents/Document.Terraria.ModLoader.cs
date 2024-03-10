using Terraria.ModLoader.IO;

namespace TigerLearning.Documents;

public partial class Document {
    public class Mod_cls {
        public static Mod mod;
        public const string intro = "一个mod需要有一个且仅一个继承自此类的类";
        public const string postSetupContent_func = "重写PostSetupContent()以在PostSetup阶段做一些事情(此时各种数组的大小已经设置好了)";
        public const string unload_func = "重写Unload()以在卸载此模组时做一些事情";
    }
    public class ModType_cls {
        public static ModType modType;
        public abstract class DocumentModType : ModType {
            public void ShowModType() {
                Show(Mod);               //获得本MOD
                Show(Name);              //内部名字
                Show(FullName);          //包含mod名的全名, mod名和物品名似乎是以 '/' 划分
                Show(PrettyPrintName()); //展示一个好看的名字(暂不清楚效果)
            }
            /// <summary>
            /// 若返回false则这个东西不会被加载(默认true)
            /// 常用于联动内容, 或配置某样东西会不会产生(此时若改变此配置应强行要求重载)
            /// </summary>
            public override bool IsLoadingEnabled(Mod mod) => true;
            public override void Load() => base.Load(); //在加载时做一些事情
            public override void SetStaticDefaults() => Dos();   //用以设置一些不会改变的数据
            public override void Unload() => Dos();  //在卸载时做一些事情

            #region idk
            public override void SetupContent() => Dos();
            protected override void InitTemplateInstance() => Dos();
            protected override void Register() => Dos();
            protected override void ValidateType() => Dos();
            #endregion
        }
    }
    public class ModItem_cls {
        public static ModItem modItem;
        /// <summary>
        /// <see cref="ModType_cls"/>
        /// </summary>
        public static ModType 基类;
        public const string intro = "继承自此类以添加一种物品";
        public class DocumentModItem : ModItem {
            public void ShowModItem() {
                #region params
                int itemId = 0;
                #endregion
                NewInstance(new Item());    //为item新建一个ModItem(但好像并不SetDefaults)
                ItemLoader.GetItem(itemId);     //获取一个ModItem的模板, 不会关联Item, 不会SetDefaults, 不推荐实际使用, 不如用new Item(itemId).ModItem, 这里只是为了获得模板
            }
            public override void SetStaticDefaults() => Dos();   //重写SetStaticDefaults()以在初始化完成后做一些事情
            public override void SetDefaults() => Dos(); //重写SetDefaults()以在进入游戏时以及创建一个物品时为这个物品做一些事情
            public override void AddRecipes() => Dos();  //重写AddRecipes()以在添加配方阶段做一些事情(一般是添加配方)
            public static Recipe_cls recipe_cls;
            /// <summary>
            /// 重写ModifyShootStats(...)以修改伤害与发射位置等等
            /// </summary>
            public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => Dos();
            public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack) => Dos();  //自定义提炼机使用
        }
    }
    public class ModProjectile_cls {
        public static ModProjectile modProjectile;
        public static void SetAIToVanillaStyle() {
            //与原版木箭完全一样的AI
            modProjectile.AIType = ProjectileID.WoodenArrowFriendly;
            modProjectile.Projectile.aiStyle = ProjAIStyleID.Arrow;
        }
    }
    public class ModTile_cls {
        public static ModTile modTile;
        public const string intro = "继承自此类以添加一类物块";
        public const string setStaticDefaults_func = "重写SetStaticDefaults()以在初始化完成后做一些事情";
        public const string addMapEntry_func = "在SetStaticDefaults()中使用AddMapEntry(Color, name = null)以在地图中添加显示, name可使用Language.GetText(path)";
        public const string drop_func = "重写bool Drop(i, j)以改写其掉落, 目前仅对1x1物块生效, 返回true以掉落默认掉落的物品, 可以使用Item.NewItem(...)以手动添加掉落";
        public const string killMultiTile_func = "重写KillMultiTile(i, j, frameX, frameY)以定义在多个联和的块被摧毁时干的事情(只执行一次, 可用以生成掉落物)";
        public const string numDust_func = "重写NumDust(i, j, fail, ref num)以改写敲物块时发出的粒子数";
        public const string createDust_func = "重写bool CreateDust(i, j, ref type)以修改在敲击物块时发出粒子的类型, 返回false使不发出默认粒子";
        public const string rightClick_func = "重写bool RightClick(int i, int j)以设置是否可以右键点击(?)";
        public static void ShowModTile() {
            Show(modTile.MineResist);
            Show(modTile.MinPick);
        }
    }
    public class ModBlockType_cls {
        public static ModBlockType modBlockType;
        public const string intro = "ModTile 和 ModWall的基类";
        public static void ShowModBlockType() {
            Show(modBlockType.DustType);        //物块被击打时发出的粒子
#if Terraria143
            Show(modBlockType.ItemDrop);        //物块被打掉时掉落的物品, 默认0代表什么都不掉
#endif
        }
    }
    public class ModSystem_cls {
        public static ModSystem modSystem;
        public const string intro = "额外的系统";
        public class ExampleModSystem : ModSystem {
            /// <summary>
            /// <br/>重写以在加载阶段做一些事情
            /// <br/>此时不能保证Mod本身已加载完成
            /// <br/>如果要使用Mod, 则需在<see cref="OnModLoad"/>中完成
            /// </summary>
            public override void Load() {
                base.Load();
            }
            /// <summary>
            /// <br/>在<see cref="Mod.Load"/>完成后立即执行
            /// <br/>这样可以保证Mod已存在, 且已加载
            /// </summary>
            public override void OnModLoad() {
                base.OnModLoad();
            }
            /// <summary>
            /// <br/>保存世界数据
            /// <br/>提供的<paramref name="tag"/>总是没有内容的(但不是null)
            /// <br/>在<paramref name="tag"/>中写入数据
            /// <br/>需和<see cref="LoadWorldData"/>一起重写
            /// </summary>
            public override void SaveWorldData(TagCompound tag) {
                tag.SetWithDefault("data", 1);
            }
            /// <summary>
            /// <br/>读取世界数据
            /// <br/>tag一般为在<see cref="SaveWorldData"/>中保存的数据
            /// <br/>也有可能是没有内容的(不是null)(在还没保存时)
            /// <br/>需和<see cref="SaveWorldData"/>一起重写
            /// </summary>
            public override void LoadWorldData(TagCompound tag) {
                tag.GetWithDefault<int>("data", out _);
            }
        }
    }
    public class ModPlayer_cls {
        public static ModPlayer modPlayer;
        public const string intro = "继承自此mod以改写人物";
        public const string kill_func = "重写Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)以在死亡时做一些事情";
        public const string save_func = "重写TagCompound Save()以保存数据";
        public const string load_func = "重写Load(TagCompound tag)以在加载时做一些事并获取保存的数据";

    }
    public class GlobalType_cls {
#if Terraria143
        public static GlobalType globalType;
#else
        public static GlobalType<GlobalItem> globalType;
#endif
        public const string appliesToEntity_func = """
            重写AppliesToEntity(entity, bool lateInstiation)来判断是否给对应物品附加上此类
            lateInstiation表示是否在SetDefaults之后检查, 若需要SetDefaults做出更改, 则需只在lateInstiation为真时返回真
            """;
    }
    public class GlobalItem_cls {
        public static GlobalItem globalItem;
#if Terraria143
        public static GlobalType superType;
#else
        public static GlobalType<GlobalItem> superType;
#endif
        public const string intro = "用以魔改所有物品(包括原版)";
        public const string load_func = "重写Load()以在加载阶段做一些事情";
        public const string setDefaults_func = "重写SetDefaults(item)以在原物品SetDefaults时做一些事情";
        public const string saveData_func = "重写SaveData(item, tagCompound)以保存数据";
        public const string loadData_func = "重写LoadData(item, tagCompound)以加载数据";
        public const string canAutoReuseItem_func = "重写bool? CanAutoReuseItem(item, player)以改写是否可自动连用, 不做改动为返回null";
        public const string consumeItem_func = "重写bool ConsumeItem(item, player)以改写在消耗一个消耗品时是否真的消耗掉它, 若没有消耗, 则不会调用OnConsumeItem, 默认返回true";
        public const string canBeConsumedAsAmmo_func = "重写bool CanBeConsumedAsAmmo(ammo, weapon, player)以改写在消耗弹药时是否真的消耗掉它, 默认返回true";
        public const string modifyTooltip_func = "重写ModifyTooltips(item, List<TooltipLine> tooltips)通过修改tooltips可修改介绍文本, 例tooltips.Add(new(Mod, \"Rare\", \"Rare: \" + item.rare))";
        public const string grabRange_func = "重写GrabRange(item, player, ref int grabRange)以改写其拾取距离";
        public const string itemSpace_func = "重写bool ItemSpace(item, player)以判断是否忽略其他条件使玩家吸引物品";
        public static void ShowGlobalItem() {
            #region params
            Item item = new();
            bool exactType = true;
            List<TooltipLine> tooltips = new();
            Mod mod = new();
            #endregion
#if Terraria143
            item.TryGetGlobalItem(out GlobalItem _, exactType);     //获取物品对应的GlobalItem
#else
            item.TryGetGlobalItem(out GlobalItem _);
#endif
            globalItem.Instance(item);                              //获取物品对应的GlobalItem
            #region MidifyTooltip
            tooltips.Add(new(mod, "ItemName", "text"));             //在物品名字处
            tooltips.Add(new(mod, "Favorite", "text"));             //当收藏时在收藏处
            tooltips.Add(new(mod, "Favorite", "text"));             //当收藏时在告诉此物品已收藏处
            tooltips.Add(new(mod, "FavoriteDesc", "text"));         //当收藏时在对收藏的描述处
            tooltips.Add(new(mod, "Social", "text"));               //当在时装栏时告诉在时装栏处
            tooltips.Add(new(mod, "SocialDesc", "text"));           //当在时装栏时对时装栏的描述处
            tooltips.Add(new(mod, "Damage", "text"));               //伤害处
            tooltips.Add(new(mod, "CritChance", "text"));           //暴击处
            tooltips.Add(new(mod, "Speed", "text"));                //使用速度处
            tooltips.Add(new(mod, "Knockback", "text"));            //击退处
            tooltips.Add(new(mod, "FishingPower", "text"));         //渔力处
            tooltips.Add(new(mod, "NeedsBait", "text"));            //告诉你捕鱼需要鱼饵处
            tooltips.Add(new(mod, "BaitPower", "text"));            //饵力处
            tooltips.Add(new(mod, "Equipable", "text"));            //告诉你这东西可以装备处
            tooltips.Add(new(mod, "WandConsumes", "text"));         //物品法杖的消耗物处
            tooltips.Add(new(mod, "Quest", "text"));                //告诉你这是任务物品处
            tooltips.Add(new(mod, "Vanity", "text"));               //告诉你这是虚荣物品处
            tooltips.Add(new(mod, "Defense", "text"));              //防御处
            tooltips.Add(new(mod, "PickPower", "text"));            //镐力处
            tooltips.Add(new(mod, "AxePower", "text"));             //斧力处
            tooltips.Add(new(mod, "HammerPower", "text"));          //锤力处
            tooltips.Add(new(mod, "TileBoost", "text"));            //放置距离加成处
            tooltips.Add(new(mod, "HealLife", "text"));             //治疗量处
            tooltips.Add(new(mod, "HealMana", "text"));             //治疗魔量处
            tooltips.Add(new(mod, "UseMana", "text"));              //使用魔量处
            tooltips.Add(new(mod, "Placeable", "text"));            //告诉你这东西可以放置处
            tooltips.Add(new(mod, "Ammo", "text"));                 //告诉你这东西是个子弹处
            tooltips.Add(new(mod, "Consumable", "text"));           //告诉你这东西可以消耗处
            tooltips.Add(new(mod, "Material", "text"));             //告诉你这东西是材料处
            tooltips.Add(new(mod, "Tooltip#", "text"));             //在特定行处, #实际为一个数字
            tooltips.Add(new(mod, "EtherianManaWarning", "text"));  //天国魔力警告处
            tooltips.Add(new(mod, "WellFedExpert", "text"));        //在专家模式告诉你这个食物会增加生命回复处
            tooltips.Add(new(mod, "BuffTime", "text"));             //Buff持续时长处
            tooltips.Add(new(mod, "OneDropLogo", "text"));          //悠悠球的logo处
            tooltips.Add(new(mod, "PrefixDamage", "text"));         //伤害修正处
            tooltips.Add(new(mod, "PrefixSpeed", "text"));          //使用速度修正处
            tooltips.Add(new(mod, "PrefixCritChance", "text"));     //暴击修正处
            tooltips.Add(new(mod, "PrefixUseMana", "text"));        //耗魔修正处
            tooltips.Add(new(mod, "PrefixSize", "text"));           //近战武器大小修正处
            tooltips.Add(new(mod, "PrefixShootSpeed", "text"));     //射速修正处
            tooltips.Add(new(mod, "PrefixKnockback", "text"));      //击退修正处
            tooltips.Add(new(mod, "PrefixAccDefense", "text"));     //饰品的防御加成处
            tooltips.Add(new(mod, "PrefixAccMaxMana", "text"));     //饰品的魔量加成处
            tooltips.Add(new(mod, "PrefixAccCritChance", "text"));  //饰品的暴击加成处
            tooltips.Add(new(mod, "PrefixAccMoveSpeed", "text"));   //饰品的移速加成处
            tooltips.Add(new(mod, "PrefixAccMeleeSpeed", "text"));  //饰品的近战速度加成处
            tooltips.Add(new(mod, "SetBonus", "text"));             //套装奖励处
            tooltips.Add(new(mod, "Expert", "text"));               //告诉你这东西是专家物品处
            tooltips.Add(new(mod, "SpecialPrice", "text"));         //特殊价格处
            tooltips.Add(new(mod, "Price", "text"));                //价格处
            #endregion
        }
    }
    public class ItemLoader_cls {
        public static void ShowItemLoader() {
            Show(ItemLoader.ItemCount);
        }
    }
    public class NPCShop_cls {
        public static NPCShop npcShop;
        public static void ShowNPCShop() {
            #region params
            int npcID = 0;
            int targetItemID = 0, itemID = 0;
            string shopName = "Shop";
            NPC npc = null;
            #endregion
            npcShop = new(npcID, shopName);
            Show(npcShop.NpcType);
            Show(npcShop.Name);
            NPCShop.Entry entry = new(ItemID.Torch);
            NPCShop.Entry entry2 = new(ItemID.CopperBar, Condition.BloodMoon);
            NPCShop.Entry entry3 = new(new Item(ItemID.BlueTorch)); //当想要先设置item的一些属性时可以用这个
            NPCShop.Entry entry4 = new(new Item(ItemID.HellCake), Condition.BirthdayParty, Condition.InUnderworld);
            npcShop.Add(entry, entry2, entry3, entry4);
            //以下所有Add都带有一个param Condition[] 参数
            npcShop.Add(ItemID.RedTorch);
            npcShop.Add(new Item(ItemID.RedTorch));
            npcShop.Add<ModItem>();

            //InsertBefore和InsertAfter也都带有一个param Condition[] 参数
            npcShop.InsertBefore(targetItemID, itemID);
            npcShop.InsertBefore(targetItemID, new Item(itemID));
            npcShop.InsertBefore(entry, new Item(itemID));  //强烈建议配合GetEntry使用
            npcShop.InsertAfter(targetItemID, itemID);      //与上面那个一样也有三个重载

            npcShop.GetEntry(targetItemID); //获得对应物品id的Entry
            npcShop.AllowFillingLastSlot(); //让此商店允许填充最后一格
            Show(npcShop.FillLastSlot);     //看看此商店是否允许填充最后一格
            Show(npcShop.Entries);          //查看所有Entry
            Show(npcShop.ActiveEntries);    //查看所有符合条件的Entry

            npcShop.Register();     //将此商店注册到对应NPC上, 这样它才真的有这个商店
        }
    }
    public class AutoloadAttribute_attr {
        public static AutoloadAttribute autoloadAttr;
        [Autoload(Side = ModSide.Both)]
        public class ExampleModSystemThatAutoloadOnBothSide : ModSystem { }
        [Autoload(Side = ModSide.Client)]
        public class ExampleModSystemThatAutoloadOnClientSide : ModSystem { }
        [Autoload(Side = ModSide.Server)]
        public class ExampleModSystemThatAutoloadOnServerSide : ModSystem { }
        [Autoload(false)]
        public class ExampleModSystemThatDontAutoload : ModSystem { }
    }
}
