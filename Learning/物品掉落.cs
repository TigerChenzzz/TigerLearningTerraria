using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace TigerLearning.Learning;

public class 物品掉落 {
    public static IItemDropRule rule;
    public static IItemDropRuleCondition condition;

    #region 使用物品掉落的场景
    /// <summary>
    /// 自己模组的NPC掉落物
    /// </summary>
    public class ExampleLootNPC : ModNPC {
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(rule);
        }
    }
    /// <summary>
    /// 自己模组的摸彩袋的掉落物
    /// </summary>
    public class ExampleItemBag : ModItem {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            itemLoot.Add(rule);
        }
    }
    /// <summary>
    /// 给特定的 NPC 添加或修改掉落物
    /// 或者为所有 NPC 添加特定掉落
    /// </summary>
    public class ExampleLootGlobalNPC : GlobalNPC {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
            if(npc.type == NPCID.Zombie) {
                npcLoot.Add(rule);
            }
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
            globalLoot.Add(rule);
        }
    }
    /// <summary>
    /// 给特定的摸彩袋添加或修改掉落物
    /// </summary>
    public class ExampleGlobalItemBag : GlobalItem {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
            if(item.type == ItemID.HerbBag) {
                itemLoot.Add(rule);
            }
        }
    }
    #endregion
    public static void 一些简单的规则() {
        #region params
        IItemDropRule ruleForNormalMode = default, ruleForExpertMode = default;
        IItemDropRule ruleForDefault = default, ruleForMasterMode = default;
        #endregion
        //min不能大于max, 否则会报错, 也就是说如果填的 min > 1 就得填max, 不得省略
        //但是分子可以大于分母, 不过这样也只是必掉, 而不会有额外掉落 (源码参考Terraria.GameContent.ItemDropRules.CommonDrop.TryDroppingItem)
        ItemDropRule.Common(ItemID.Torch, 5, 10, 12);   //有 1/5 的概率掉 10 ~ 12 个(包含10和12)
        rule = new CommonDrop(ItemID.Torch, 7, 30, 50, 2);  //有 2/7 的概率掉 30 ~ 50 个
        ItemDropRule.OneFromOptions(2, ItemID.Torch, ItemID.BlueTorch); //有 1/2 的概率掉一个火把或一个蓝火把
        ItemDropRule.OneFromOptionsWithNumerator(3, 2, ItemID.Torch, ItemID.BlueTorch); //有 2/3 的概率掉这两种东西的一种
        ItemDropRule.FewFromOptions(2, 5, ItemID.Torch, ItemID.BlueTorch, ItemID.RedTorch); //有 1/5 的概率掉落这三种东西的两种(不会重复)
        ItemDropRule.FewFromOptionsWithNumerator(2, 5, 4, ItemID.Torch, ItemID.BlueTorch, ItemID.RedTorch); //有 4/5 的概率掉落这三种东西的两种
        rule = new FewFromOptionsDropRule(2, 5, 4, ItemID.Torch, ItemID.BlueTorch, ItemID.RedTorch);    //同上
        #region 不同模式下的掉落
        ItemDropRule.NormalvsExpert(ItemID.Torch, 4, 2);    //在普通模式中以 1/4 的概率掉落, 专家模式中为 1/2
        rule = new DropBasedOnExpertMode(ruleForNormalMode, ruleForExpertMode); //在普通模式和专家模式应用不同规则
        rule = new DropBasedOnMasterAndExpertMode(ruleForDefault, ruleForExpertMode, ruleForMasterMode);    //在三种模式下应用三种规则
        rule = new DropBasedOnMasterMode(ruleForDefault, ruleForMasterMode);    //在非大师模式和大师模式应用不同规则
        #endregion
        ItemDropRule.BossBag(ItemID.Torch); //在专家和大师模式下为所有玩家分别掉落
        rule = new DropPerPlayerOnThePlayer(ItemID.Torch, 5, 10, 12, condition);    //为所有玩家分别掉落, 似乎是分别计算概率和条件(待测试)
        rule = new DropOneByOne(ItemID.Torch, new DropOneByOne.Parameters() {
            MinimumItemDropsCount = 30,
            MaximumItemDropsCount = 50,
            ChanceDenominator = 2,
            ChanceNumerator = 1,
        }); //原版用于四柱
    }
    public static void 条件掉落() {
        ItemDropRule.ByCondition(condition, ItemID.Torch, 6, 2, 7, 5);  //在满足条件时有 5/6 的概率掉 2 ~ 7 个
        rule = new ItemDropWithConditionRule(ItemID.Torch, 6, 2, 7, condition, 5);  //同上

        #region 预设条件
        //从Conditions可以获得许多条件的预设, 具体参见Terraria.GameContent.ItemDropRules.Conditions的源码
        condition = new Conditions.IsPumpkinMoon();             //双月事件(霜月是不是也用这个待测试)
        condition = new Conditions.FromCertainWaveAndAbove(10); //配合上条使用, 代表特定波数及以上
        condition = new Conditions.DownedAllMechBosses();       //三王后
        condition = new Conditions.DownedPlantera();            //花后
        condition = new Conditions.FirstTimeKillingPlantera();  //第一次打败世纪之花时(最好别用)
        condition = new Conditions.BeatAnyMechBoss();           //一王后
        condition = new Conditions.IsExpert();                  //专家模式
        condition = new Conditions.IsMasterMode();              //大师模式
        condition = new Conditions.NotExpert();                 //非专家模式
        condition = new Conditions.NotMasterMode();             //非大师模式
        condition = new Conditions.IsCrimson();                 //需要在猩红世界
        condition = new Conditions.IsCorruption();              //需要在腐化世界
        condition = new Conditions.TenthAnniversaryIsUp();      //十周年种子世界
        condition = new Conditions.TenthAnniversaryIsNotUp();   //非十周年种子世界
        condition = new Conditions.DontStarveIsUp();            //饥荒种子世界
        condition = new Conditions.DontStarveIsNotUp();         //非饥荒种子世界
        //...
        #endregion
    }
    public class 自定义条件 : IItemDropRuleCondition {
        //这里写具体条件判定
        public bool CanDrop(DropAttemptInfo info) => false;
        //是否会在图鉴上显示
        public bool CanShowItemDropInUI() => true;
        //条件的说明
        public string GetConditionDescription() => null;
    }
    public static void 组合规则() {
        rule = new LeadingConditionRule(condition);     //没有实际掉落, 用于组合规则
        IItemDropRule rule1 = default, rule2 = default;
        rule1.OnSuccess(rule2);  //当第一条成功生成时会尝试执行第二条, 需要先将rule1添加到xxxLoot中去, 或者直接将返回值添加进去
        rule1.OnFailedConditions(rule2);    //在条件判断失败时执行第二条
        rule1.OnFailedRoll(rule2);      //在随机取数时失败时执行第二条, LeadingConditionRule不要用这条
        //可以在同一规则上多次使用OnSuccess或其它

        rule = new OneFromRulesRule(3, 2, rule1, rule2);    //有 2/3 的概率执行随机一条规则
    }
    public static void 查询原版掉落代码() {
        //NPC掉落代码位于Terraria.GameContent.ItemDropRules.ItemDropDatabase中
        Show<ItemDropDatabase>();
        //摸彩袋掉落的源码位于Terraria/GameContent/ItemDropRules/ItemDropDatabase.TML.cs
    }
    public static void 其他() {
        #region params
        NPC npc = default;
        NPCLoot npcLoot = default;
        Player player = default;
        int npcType = 0, npcNetID = 0, itemID = 0;
        #endregion
        #region 动态地修改规则
        Main.ItemDropsDB.RegisterToNPC(npcType, rule);
        Main.ItemDropsDB.RegisterToNPCNetId(npcNetID, rule);
        Main.ItemDropsDB.RegisterToGlobal(rule);    //全局NPC的掉落规则
        Main.ItemDropsDB.RegisterToItemId(itemID, rule);    //即使翻源码也只是感觉和RegisterToItem没有区别
        Main.ItemDropsDB.RemoveFromNPC(npcType, rule);    //待测试, 毕竟rule都是引用类型的, 我很怀疑如果new一个rule出来会不会永远都找不到而不会删除
        Main.ItemDropsDB.RemoveFromNPCNetId(npcNetID, rule);
        new GlobalLoot(Main.ItemDropsDB).Remove(rule);    //很奇怪ItemDropDatabase没有自己删全局规则的方法
                                                            //不过全局的规则修改最好还是在GlobalNPC.ModifyGlobalLoot里写, 而且最好是用RemoveWhere而且在条件中不要直接写 rule = ... , 除非你很清楚这意味这什么
        #endregion
        //可以由如下语句来直接生成掉落物
        DropAttemptInfo info = new() {
            IsExpertMode = Main.expertMode,
            IsMasterMode = Main.masterMode,
            npc = npc,  //也可以写item, 表示生成源
            player = player,  //表示杀死此npc, 或者打开此item的玩家, 当为npc时其实一般是最近的玩家Player.FindClosest(position, width, height)  (参见Terraria.NPC.NPCLoot_DropItems)
            rng = Main.rand
        };
        Main.ItemDropSolver.TryDropping(info);

        //不建议直接使用下面这句, 因为它不会处理嵌套情况
        //若要处理单条规则, 建议反射到ItemDropSolver.ResolveRule, 或者把此处的源码抄下来
        rule.TryDroppingItem(info);

        //获得特定npc的所有规则(可以指定包不包含全局规则)
        Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false);
    }

    public class 修改混沌法杖掉率 : GlobalNPC {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
            if(npc.type != NPCID.ChaosElemental) {
                return;
            }
            var rules = npcLoot.Get(false)
                .FindAll(r => r is LeadingConditionRule { condition: Conditions.TenthAnniversaryIsNotUp })
                .SelectMany(r => r.ChainedRules).Where(chain => chain is Chains.TryIfSucceeded { RuleToChain: DropBasedOnExpertMode })
                .Select(chain => chain.RuleToChain as DropBasedOnExpertMode);
            foreach(var rule in rules) {
                if(rule.ruleForNormalMode is CommonDrop { itemId: ItemID.RodofDiscord } normalModeRule) {
                    normalModeRule.chanceDenominator = 3;
                }
                if(rule.ruleForExpertMode is CommonDrop { itemId: ItemID.RodofDiscord } expertModeRule) {
                    expertModeRule.chanceNumerator = 2;
                    expertModeRule.chanceDenominator = 3;
                }
            }
        }
    }
}