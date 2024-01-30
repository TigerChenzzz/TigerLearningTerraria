using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace TigerLearning.Learning;

public class 添加NPC {
    public class ExampleNPC : ModNPC {
        public static string 参考 = "从零开始的NPC编写 https://fs49.org/2022/01/18/%e4%bb%8e%e9%9b%b6%e5%bc%80%e5%a7%8b%e7%9a%84npc%e7%bc%96%e5%86%99/\n" + 城镇NPC.参考;
        public override void SetStaticDefaults() {
            //名字的设置位于Mods.[ModName].NPCs.[NPCName].DisplayName
            Main.npcFrameCount[Type] = 3;   //附带的贴图有几帧(纵向排列)
        }
        public override void SetDefaults() {
            NPC.width = 34;     //npc的长宽, 也是帧图的长和宽(与图不匹配的情况待测试)
            NPC.height = 54;
            NPC.damage = 200;
            NPC.lifeMax = 1100; //NPC.life随后会自动设置为NPC.lifeMax
            NPC.defense = 100;
            NPC.knockBackResist = 0f;   //实际为受到多少击退, 0f为不受击退, 1f才是受到全额击退
            NPC.aiStyle = -1;   //自己写ai时就设为-1
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = Item.buyPrice(0, 0, 15, 0);
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            Banner = Type;  //击杀它算作哪种类型的旗帜, 若没有旗帜填0或不填(默认0), 若有一般填自己的npcID, 若与其它NPC共用击杀数和旗帜, 则设置为它的npcID
            BannerItem = ItemID.Gel;    //若有自己的旗帜, 则设置此值为旗帜的itemID (此处为没写旗帜所以填的凝胶(然后每击杀50个此NPC就会获得1个凝胶()))
            NPC.noGravity = false;  //默认false, 若为true则会无视重力飘起来
            NPC.noTileCollide = false;  //默认false, 若为true则会穿越物块, 不会处理方块碰撞
            AIType = 0;     //若要完全使用原版的NPC的AI则设置此为对应npcID, 且设置NPC.aiStyle为对应的aiStyle, 默认为0即不使用任何原版AI
            AnimationType = 0;      //如何控制此npc的帧图(使用哪个原版npc的控制方法), 默认为0即不使用任何原版逻辑
            NPC.boss = false;   //是否是一个boss, 若是则会掉生命药水, 大量心, 并且会有xxx已被打败的信息等效果, 默认false
        }
        /// <summary>
        /// 设置图鉴
        /// </summary>
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                //生成环境, 对于城镇NPC, 此处写其最喜爱的环境
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                //图鉴内描述
                new FlavorTextBestiaryInfoElement("Use for example"),
            });
        }
        /// <summary>
        /// 决定什么时候生成NPC以及其生成权重
        /// </summary>
        /// <returns>
        /// <para>其返回值的处理参见Terraria.ModLoader.NPCLoader.ChooseSpawn, 其实就是个加权平均</para>
        /// <para>若要自己设置此值, 请按总权值为1的预期处理(比如你想要生成的一半都是这个NPC, 那么0.5f就够了)</para>
        /// </returns>
        public override float SpawnChance(NPCSpawnInfo spawnInfo) {
            float chance = SpawnCondition.Underground.Chance;   //在地下生成
            Player player = spawnInfo.Player;   //这样可以直接拿到player
            if(spawnInfo.PlayerInTown) {    //这条其实主要是用于生成小动物而不是阻止生成怪物的
                return 0;
            }
            if(spawnInfo.PlayerSafe) {    //阻止生成怪物用这个
                return 0;
            }
            if(Main.bloodMoon) {
                chance *= 2;    //在血月时生成的权值更大
                                //但不代表在血月能生成更多, 因为血月能生成一些额外的东西, 而它们都会占用权值
            }
            if(Main.dayTime) {
                chance = SpawnCondition.OverworldDaySlime.Chance;  //在白天与史莱姆的权重相同
                                                                    //也可以判SpawnCondition.OverworldDaySlime.Active看看会不会生成史莱姆
            }
            return chance;
            //调小或者调大返回值可以影响到它生成的权重, 也就是生成NPC时它生成它的概率会变化
            //这样能一定程度上改变它生成的概率
            //但若只有它能生成时, 只要它的权重大于0, 就会生成它
            //若不想这样, 可以使用随机数, 概率返回原值, 概率返回0, 这样能更直接地减少npc的生成概率
        }
        /// <summary>
        /// 设置该NPC死亡后的掉落物
        /// </summary>
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            //照抄某个NPC的掉落物就这么写
            var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie);
            foreach(var zombieDropRule in zombieDropRules) {
                npcLoot.Add(zombieDropRule);
            }

            //自己添加掉落物就这么写
            npcLoot.Add(ItemDropRule.Common(ItemID.Torch, 10, 20, 30)); // 1 / 10 的概率生成 20 - 30 个
                                                                        //必定生成则10处填1(虽说填0好像也可以), 若生成数固定则20和30处填入一样的值(30处不能省略), 若固定只生成1个则20和30处可同时省略

            //更多相关内容参见下面的物品掉落
            Show<物品掉落>();
        }
        /// <summary>
        /// 每帧调用, 用来干各种事情<para/>
        /// 比如移动, 攻击等行为皆在此函数中完成<para/>
        /// 若要重写这个, 则不要设置<see cref="ModNPC.AIType"/>, 也最好把aiStyle设置为 -1 (0的话会自动面向目标)
        /// </summary>
        public override void AI() {
            base.AI();
        }
        /// <summary>
        /// 编写NPC的动画
        /// 若要重写这个, 则不要设置<see cref="ModNPC.AnimationType"/>
        /// </summary>
        /// <param name="frameHeight">
        /// 一帧的高度, 将<see cref="NPC.frame"/>的<see cref="Rectangle.Y"/>设置为
        /// i * <paramref name="frameHeight"/>即可切到第i帧(从0开始)
        /// </param>
        public override void FindFrame(int frameHeight) {
            base.FindFrame(frameHeight);
        }

        public override string Texture => base.Texture; //自定使用什么贴图
        public override string HeadTexture => base.HeadTexture; //自定使用什么头部贴图(用于小地图中的显示)
        public override bool IsLoadingEnabled(Mod mod) => base.IsLoadingEnabled(mod);   //是否真的要加载

    }
    public static void 常用的生成条件(NPCSpawnInfo spawnInfo) {
        #region 环境
        Show(SpawnCondition.Corruption);                        //腐化
        Show(SpawnCondition.Crimson);                           //猩红
        Show(spawnInfo.Granite);                                         //花岗岩
        Show(spawnInfo.Marble);                                          //大理石
        Show(spawnInfo.DesertCave);                                      //沙漠洞穴
        Show(SpawnCondition.JungleTemple);                      //神庙
        Show(spawnInfo.Lihzahrd);                                        //神庙内(蜥蜴的生成条件)
        Show(spawnInfo.SpiderCave);                                      //蜘蛛洞穴
        Show(spawnInfo.Water);                                           //水中
        Show(SpawnCondition.Ocean);                             //海洋
        Show(spawnInfo.Sky);                                             //太空
        Show(SpawnCondition.Sky);                               //太空
        Show(SpawnCondition.Overworld);                         //地上
        Show(SpawnCondition.Underground);                       //地下
        Show(SpawnCondition.Cavern);                            //洞穴
        Show(SpawnCondition.Underworld); //地狱
        #endregion
        #region 事件
        Show(spawnInfo.Invasion);                                            //在入侵中
        Show(Main.invasionType);                                             //入侵的类型, 可以辅以InvasionID
        Show(SpawnCondition.GoblinArmy);                            //哥布林
        Show(SpawnCondition.Pirates);                               //海盗
        Show(SpawnCondition.MartianMadness);                        //火星人
        Show(SpawnCondition.OldOnesArmy);                           //旧日军团(撒旦军团)
        Show(SpawnCondition.PumpkinMoon);                           //南瓜月
        Show(Main.bloodMoon);                                                //血月
        Show(Main.raining);                                                  //在下雨
        Show(Sandstorm.Happening);                                           //沙暴是否正在发生
        Show(SpawnCondition.SandstormEvent); //沙暴是否正在发生(不知道要不要额外判在不在沙漠中)
        #endregion

        Show(Main.dayTime);     //是否是白天
        Show(NPC.AnyNPCs(NPCID.IceGolem));  //是否有冰雪巨人
        Shows(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY);     //在什么物块坐标上生成
        Show(spawnInfo.SpawnTileType);      //在哪种物块上生成
    }
    public static void 其他() {
        #region params
        NPC npc = null;
        #endregion
        Show("""
            请务必区分好npc.type和npc.netID
            有些npc共用相同的npc.type, 但是其netID却不同
            典型的例子是各种史莱姆的type基本都是1(NPC.BlueSlime),但是netID各不相同
            NPCID中的数字对应的是netID, 一般请将这两者作比较
            当然在原版出现这种情况时额外的netID都是负数, 所以也可以判这个来确定是否有这种情况
            """);
        Shows(npc.type, npc.netID);
        Shows(NPCID.BlueSlime, NPCID.GreenSlime, NPCID.RedSlime, NPCID.PurpleSlime);
    }

    [AutoloadHead]
    public class 城镇NPC : ModNPC {
        public static string 参考 = "创建一个基础城镇NPC https://fs49.org/2023/01/20/%e5%88%9b%e5%bb%ba%e4%b8%80%e4%b8%aa%e5%9f%ba%e7%a1%80%e5%9f%8e%e9%95%87npc-1-4-ver/";
        public static string 分类 = """
            按入住方式: 常驻型，旅商型，宠物和其他型
                旅商型: 参见 Example Mod 的Example Traveling Merchant
                其他型: 比如原版的地牢老人和骷髅商人
            按攻击方式: 投掷型，远程型，魔法型，近战型和不攻击型
                投掷型: 原版大多数城镇NPC都属于这个类型。基本特征为四帧攻击动画，发射弹幕且不持有武器
                远程型: 基本特征为四到五帧攻击动画但每次只播放一帧, 发射弹幕且持有武器
                    一般而言，使用弓箭的NPC其攻击动画为五帧，而使用枪械的NPC其攻击动画为四帧
                    绝大多数这类NPC都持有武器但也可以进一步分为两种:
                        像军火商、油漆工这样直接将武器额外绘制在手上的
                        以及像海盗、巫医那样直接将武器画进帧图里的
                魔法型: 基本特征为两帧攻击动画，发射弹幕，不持有武器但发动攻击时脚下会产生一个（发光发热的）魔法光环
                近战型: 基本特征为四帧攻击动画，不发射弹幕且持有武器
                不攻击型: 所有的城镇宠物都属于这个类型，没有攻击手段，遇见敌怪时只会逃跑
            """;
        public static string 贴图 = """
            除了要准备一份[npcName].png的帧图外, 还要准备一份[npcName]_Head.png
            表示NPC在小地图的图标
            """;
        public override void SetStaticDefaults() {
            int npcID = NPCID.Merchant; //直接抄商人的数据
            Main.npcFrameCount[Type] = Main.npcFrameCount[npcID];
            //特殊交互帧（如坐下，攻击）的数量，其作用就是规划这个NPC的最大行走帧数为多少，
            //最大行走帧数即Main.npcFrameCount - NPCID.Sets.ExtraFramesCount
            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[npcID];
            //攻击帧的数量，取决于你的NPC属于哪种攻击类型，如何填写见上文的分类讲解
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[npcID];
            //NPC的攻击方式，同样取决于你的NPC属于哪种攻击类型，投掷型填0，远程型填1，魔法型填2，近战型填3，
            //如果是宠物没有攻击手段那么这条将不产生影响
            NPCID.Sets.AttackType[Type] = NPCID.Sets.AttackType[npcID];
            //NPC的帽子位置中Y坐标的偏移量，这里特指派对帽，
            //当你觉得帽子戴的太高或太低时使用这个做调整（所以为什么不给个X的） 
            NPCID.Sets.HatOffsetY[Type] = NPCID.Sets.HatOffsetY[npcID];
            //这个名字比较抽象，可以理解为 {记录了NPC的某些帧带来的身体起伏量的数组} 的索引值，
            //而这个数组的名字叫 NPCID.Sets.TownNPCsFramingGroups ，详情请在源码的NPCID.cs与Main.cs内进行搜索。
            //举个例子：你应该注意到了派对帽或是机械师背后的扳手在NPC走动时是会不断起伏的，靠的就是用这个进行调整，
            //所以说在画帧图时最好比着原版NPC的帧图进行绘制，方便各种数据调用
            //补充：这个属性似乎是针对城镇NPC的。
            NPCID.Sets.NPCFramingGroup[Type] = NPCID.Sets.NPCFramingGroup[npcID];
            //魔法型NPC在攻击时产生的魔法光环的颜色，如果NPCID.Sets.AttackType不为2那就不会产生光环
            //如果NPCID.Sets.AttackType为2那么默认为白色
            NPCID.Sets.MagicAuraColor[Type] = Color.White;
            //NPC的单次攻击持续时间，如果你的NPC需要持续施法进行攻击可以把这里设置的很长，
            //比如树妖的这个值就高达600
            //补充说明一点：如果你的NPC的AttackType为3即近战型，
            //这里最好选择套用，因为近战型NPC的单次攻击时间是固定的
            NPCID.Sets.AttackTime[Type] = NPCID.Sets.AttackTime[npcID];
            //NPC的危险检测范围，以像素为单位，这个似乎是半径
            NPCID.Sets.DangerDetectRange[Type] = 500;
            //NPC在遭遇敌人时发动攻击的概率，如果为0则该NPC不会进行攻击（待测试）
            //遇到危险时，该NPC在可以进攻的情况下每帧有 1 / (NPCID.Sets.AttackAverageChance * 2) 的概率发动攻击
            //注：每帧都判定
            NPCID.Sets.AttackAverageChance[Type] = 10;
            #region 图鉴部分
            //将该NPC划定为城镇NPC分类
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new() {
                //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
                Velocity = 1f,
            };
            //添加信息至图鉴
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            #endregion
            #region 幸福度设置
            NPC.Happiness
            .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)        //憎恶丛林环境
            .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)//讨厌地下环境
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)          //喜欢雪地环境
            .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)         //最爱海洋环境
            .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)      //讨厌与渔夫做邻居
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Like);         //喜欢与向导做邻居
                                                                        //邻居的喜好级别和环境的AffectionLevel是一样的
            #endregion
        }
        public override void SetDefaults() {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Merchant;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            //...
        }
        /// <summary>
        /// 判断此城镇NPC是否可以生成
        /// </summary>
        /// <param name="numTownNPCs">当前地图存在的城镇NPC的数量</param>
        public override bool CanTownNPCSpawn(int numTownNPCs) {
            return base.CanTownNPCSpawn(numTownNPCs);
        }
        /// <summary>
        /// 特定人物的姓名, 而不是这种城镇NPC的名字
        /// (比如一个向导叫 "Andrew", 这里的姓名指Andrew)
        /// 通过<see cref="NPC.GivenName"/>可获得特定NPC的名字
        /// </summary>
        public override List<string> SetNPCNameList() {
            return new() {
                "Some Name",
                "Some Another Name"
            };
        }
        /// <summary>
        /// 判断可否到国王或女王雕像
        /// 也可以用来判断性别
        /// </summary>
        /// <param name="toKingStatue">若为真则是到国王雕像的判定, 假则是到女王雕像的判定</param>
        /// <returns>
        /// 填<paramref name="toKingStatue"/>则是只能到国王雕像, 填!<paramref name="toKingStatue"/>则是只能到女王雕像<para/>
        /// 填false则是都不能传(骷髅商人用的这个), 填true表示都能传
        /// </returns>
        public override bool CanGoToStatue(bool toKingStatue) {
            return toKingStatue;
        }
        /// <summary>
        /// 可以在传送到国王或女王雕像时做一点事情
        /// </summary>
        public override void OnGoToStatue(bool toKingStatue) {
            base.OnGoToStatue(toKingStatue);
        }
        /// <summary>
        /// 判断是否可以对话
        /// 城镇NPC是否会自动处理, 一般为要让NPC有时不能对话时才用这个重写方法
        /// </summary>
        public override bool CanChat() {
            return base.CanChat();
        }
        /// <summary>
        /// 获取对话内容
        /// </summary>
        public override string GetChat() {
            WeightedRandom<string> chat = new(Main.rand);
            if(NPC.homeless) {
                chat.Add("I'm homeless...");
            }
            else {
                chat.Add($"Hello! I'm {NPC.FullName}");
            }
            #region 若想看看有没有其他(城镇)NPC
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if(guide >= 0) {
                chat.Add($"{Main.npc[guide].GivenOrTypeName} is great!");
            }
            #endregion
            if(BirthdayParty.PartyIsUp) {
                chat.Add("I love party!", 0.5);
            }
            if(Main.bloodMoon) {
                chat.Add("No blood please!!!", 4);
            }
            return chat;
        }
        /// <summary>
        /// 设置对话按钮
        /// </summary>
        /// <param name="button2">虽然是第二个按钮，但实际的排序上“关闭”永远是第二个，button2 是第三个</param>
        public override void SetChatButtons(ref string button, ref string button2) {
            button = "button1 name";    //只要给个名字就能有按钮了
        }
        /// <summary>
        /// 设置按钮被按下后会发生什么
        /// </summary>
        /// <param name="firstButton">是否在第一个按钮按下时进行, 否则为第二个</param>
        /// <param name="shopName"></param>
        public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
            if(firstButton) {
                shopName = "shop name";    //只要给个名字就能有商店了
            }
        }
        public override void AddShops() {
            NPCShop shop = new(Type);
            shop.Add(new NPCShop.Entry(ItemID.Torch), new(ItemID.CopperBar, Condition.BloodMoon));  //固定卖火把, 血月时卖铜锭
            shop.Register();
            //可以连续写成new NPCShop(Type).Add(entries).Register();
        }
    }
}