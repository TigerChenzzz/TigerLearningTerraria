// #define Terra143
//以下部分源码源自新版裙中世界教程的模板Mod https://github.com/CXUtk/TemplateMod2
//以及tml的Example Mod (简称Ex Mod) https://github.com/tModLoader/tModLoader
//一些东西也参考了群中世界的教程 https://fs49.org/

#pragma warning disable CA2211 // 非常量字段应当不可见
#pragma warning disable CS0219 // 变量已被赋值, 但从未使用过它的值
#pragma warning disable CS0649 // 从未对字段赋值, 字段将一直保持其默认值 null
#pragma warning disable IDE0059 // 不需要赋值
#pragma warning disable IDE0060 // 删除未使用的参数

#if Terraria143
using IL.Terraria.GameContent.UI;
#endif
using ImproveGame.Common.Configs;
using ImproveGame.Content.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria.Audio;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.Skies;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.Initializers;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.Utilities;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using WingSlotExtra;

namespace TigerLearning;

public class Learning {
    public static void 杂项() {
        Show("任何武器有4%的基础暴击率(待测试)");
    }
    public static void 一些有用的属性() {
        #region params
        Rectangle location = default;
        #endregion
        #region 世界进度
        Show(NPC.downedGoblins);            //哥布林
        Show(NPC.downedSlimeKing);          //史王
        Show(Main.hardMode);                //肉后
        Show(NPC.downedPlantBoss);          //世花
        Show(NPC.downedMoonlord);           //月后
        #endregion
        #region 显示文字
        Main.NewText("string", Color.White);
        CombatText.NewText(location, Color.White, "string");
        //PopupText.NewText()       //重铸时的弹出语使用此方法
        #endregion
        #region 未分类(暂无)
        #endregion
    }

    public class 物品基础 {
        /// <summary>
        /// 一些物品相关的属性与方法
        /// </summary>
        public static void ShowItem() {
            #region params
            Item item = default;
            #endregion
            #region 物品ID
            Show(item.type);
            Show(ItemID.None);//仅限原版
            Show(ModContent.ItemType<UnloadedItem>());
            #endregion
        }
        /// <summary>
        /// 自制药水
        /// </summary>
        public class ExamplePotion : ModItem {
            public static string 参考 = "自定义套装和Buff https://fs49.org/2020/03/12/%e8%87%aa%e5%ae%9a%e4%b9%89%e5%a5%97%e8%a3%85%e5%92%8cbuff/";
            public override void SetStaticDefaults() {
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 999;    //设置旅行模式研究所需物品
            }
            public override void SetDefaults() {
                Item.width = 14;
                Item.height = 24;
                Item.useAnimation = 17;
                Item.useTime = 17;
                Item.maxStack = 30;
                Item.rare = ItemRarityID.Pink;
                Item.value = Item.sellPrice(silver: 50);

                Item.useStyle = ItemUseStyleID.DrinkLiquid;
                Item.UseSound = SoundID.Item3;      // 喝药的声音
                Item.consumable = true;     //消耗品
                Item.useTurn = true;
                // 告诉TR内部系统, 这个物品是一个生命药水物品, 用于TR系统的特殊目的(比如一键喝药水), 默认为false
                Item.potion = false;
                // 这个药水能给玩家加多少血, 跟potion一起使用喝完药就会有抗药性debuff
                Item.healLife = 50;
                #region 加buff的方法1：设置物品的buffType为buff的ID
                // 这里我设置了着火debuff(2333
                Item.buffType = BuffID.OnFire;
                // 用于在物品描述上显示buff持续时间
                Item.buffTime = 60000;
                #endregion
            }
            /// <summary>
            /// 当物品使用的时候触发
            /// </summary>
            /// <returns>决定是否会有使用间隔, 返回null以按原来的处理</returns>
            public override bool? UseItem(Player player) {
                // 给玩家加上中毒buff, 持续 60000 / 60 = 1000秒
                // 第一个填buff的ID, 第二个填持续时间
                player.AddBuff(BuffID.Poisoned, 60000);
                player.AddBuff(BuffID.Venom, 60000);    //猛毒1000s
                return null;
            }

            // 物品合成表的设置部分
            public override void AddRecipes() {
                CreateRecipe()
                    .AddIngredient(ItemID.GoldBar, 5)
                    .AddIngredient(ItemID.IronBar, 5)
                    .AddIngredient(ItemID.Torch, 25)
                    .AddTile(TileID.WorkBenches)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
        /// <summary>
        /// 自制双用途物品
        /// </summary>
        public class ExampleAltUseItem : ModItem {
            public static string 参考 = "双用途武器 https://fs49.org/2022/07/12/%e5%8f%8c%e7%94%a8%e9%80%94%e6%ad%a6%e5%99%a8/";
            /// <summary>
            /// 返回true就能右键使用了
            /// 然后当你按下右键的时候TR就会把 player.altFunctionUse 这个属性设置为2, 也就是右键
            /// 我们之后只需要判断这个属性的值就知道玩家是否用右键使用物品
            /// </summary>
            public override bool AltFunctionUse(Player player) {
                return true;
            }
            public override bool CanUseItem(Player player) {
                if(player.altFunctionUse == 2) {
                    Item.useTime = 20;
                    Item.useAnimation = 20;
                }
                else {
                    Item.useTime = 40;
                    Item.useAnimation = 40;
                }
                return true;
            }
            public static string 右键蓄力 = """
                泰拉瑞亚有一个很灵性的玩意, 那就是 player.channel 不支持右键使用
                那我们是不是可以放弃右键左键都可以做蓄力武器的存在了？当然不可能。
                目前来说, 我们有一个最简单的方法, 那就是用 player.controlUseTile 判右键的持续使用。
                目前我没找到什么原因导致的 player.controlUseTile 在长按右键的时候会一直为true, 所以暂时用着吧, 等有更好的办法再说(
                """;
        }
        public class 添加特技 : ModItem {
            public static string 参考 = "给物品加特技 https://fs49.org/2020/03/13/%e7%bb%99%e7%89%a9%e5%93%81%e5%8a%a0%e7%89%b9%e6%8a%80/";
            /// <summary>
            /// 会在近战武器挥动的时候被触发
            /// </summary>
            public override void MeleeEffects(Player player, Rectangle hitbox) {
                Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Torch, 0, 0, 100, Color.White, 2f);
            }
            /// <summary>
            /// 这个函数允许你设置useStyle为<see cref="ItemUseStyleID.Shoot"/>且不是法杖的物品的使用贴图偏移量
            /// </summary>
            public override Vector2? HoldoutOffset() {
                // X坐标往里移动10像素, Y坐标向上移动5像素
                return new Vector2(-10, -5);
            }
            /// <summary>
            /// 只对useStyle为<see cref="ItemUseStyleID.Shoot"/>且是法杖的物品有用
            /// 能修改物品贴图旋转中心的偏移量
            /// </summary>
            public override Vector2? HoldoutOrigin() {
                return base.HoldoutOrigin();
            }
            /// <summary>
            /// 可以用来修改物品贴图的位置和旋转
            /// </summary>
            public override void HoldStyle(Player player, Rectangle heldItemFrame) {
                base.HoldStyle(player, heldItemFrame);
            }
            /// <summary>
            /// 修改玩家在用这个武器(被选中)的时候玩家的动作
            /// </summary>
            public override void HoldItemFrame(Player player) {
                // 选中武器的时候设置为第3帧
                player.bodyFrame.Y = player.bodyFrame.Height * 2;
                //这样只要切到这个武器玩家就会举起手来(笑)
            }
            public override bool CanConsumeAmmo(Item ammo, Player player) {
                // 假设我们想要60%的几率不消耗弹药
                // 为了有60的几率返回false就是40%几率返回true
                return Main.rand.Next(10) < 4;
            }
            public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
                type = Main.rand.Next(Main.projFrames.Length);//随便一个能获得总弹幕数的数组
                return true;
                //这样就会把所有弹幕都随机射出来, 包括Mod的弹幕
            }
            /// <summary>
            /// 在近战武器砍到敌人后触发
            /// </summary>
            public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
                target.AddBuff(BuffID.Poisoned, 600);
            }
            /// <summary>
            /// 发生在近战武器砍到敌人后, 触发伤害前, 可以修改即将作用到敌人的伤害, 击退, 暴击等
            /// </summary>
            public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) {
                base.ModifyHitNPC(player, target, ref modifiers);
            }
            public static void OtherEffect() {
                #region params
                Player player = null;
                #endregion
                #region 作一个饰品, 使得玩家速度达到一定值以后释放粒子特效
                //写在饰品的Update里面:
                if(player.velocity.Length() > 5) {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Torch, -player.velocity.X, -player.velocity.Y, 100, Color.White, 2.0f);
                    // 粒子不受重力影响
                    dust.noGravity = true;
                }
                #endregion
            }
        }
        public static void 图标的动画效果() {
            #region params
            int itemID = 0;
            ModItem self = null;
            #endregion
            #region 在SetStaticDefaults中
            Main.RegisterItemAnimation(itemID, new DrawAnimationVertical(6, 16));   //物品贴图从上到下总共16帧, 每6游戏帧切一次贴图
            ItemID.Sets.AnimatesAsSoul[self.Type] = true;    //让帧图动画在世界中同样生效
            ItemID.Sets.ItemIconPulse[self.Item.type] = true;    //让物品贴图不断变大变小(若与上条一起可以在世界中也会变大变小)
            #endregion
        }
    }
    public class 添加配方 {
        public static Document.Recipe_cls recipe_cls;
        #region params
        public static Recipe recipe;
        public static ModItem modItem;
        #endregion
        public static void 添加一个配方() {
            #region 在ModItem中重写方法AddRecipes()
            recipe = Recipe.Create(modItem.Type);   //以某个物品为产物, 与下一个选其一
            recipe = modItem.CreateRecipe(); //创建一个以自己为产物的配方, 可以传入amount代表产出几个
            /*
            如果你写俩种相同的材料的话
            它们会分别显示而不是合并成一个
            你最多可以加14个合成材料, 这是泰拉瑞亚本体规定的
            */
            recipe.AddIngredient(ItemID.DirtBlock, 10); //以10个土块作为材料
            recipe.AddTile(TileID.WorkBenches);     //此配方需要在工作台旁边
            recipe.Register();  //把这个合成表装进tr的系统里
            //可以连续书写: modItem.CreateRecipe().AddIngredient(ItemID.DirtBlock, 10).AddTile(TileID.WorkBenches).Register();
            #endregion
        }
        public static void 添加液体环境() {
            recipe.AddCondition(Condition.NearWater);
            recipe.AddCondition(Condition.NearLava);
            recipe.AddCondition(Condition.NearHoney);
            recipe.AddCondition(Condition.NearShimmer);

        }
        public static void 添加合成组() {
            #region 在ModSystem或主类中重写AddRecipeGroup()向mod添加合成组
            RecipeGroup group = new(() => "任意火把", new int[]
            {
                ItemID.Torch,
                ItemID.BlueTorch,
                ItemID.CursedTorch,
                ItemID.DemonTorch,
                ItemID.GreenTorch,
                ItemID.IceTorch,
                ItemID.IchorTorch,
                ItemID.OrangeTorch,
                ItemID.PinkTorch,
                ItemID.PurpleTorch,
                ItemID.RainbowTorch,
                ItemID.RedTorch,
                ItemID.TikiTorch,
                ItemID.UltrabrightTorch,
                ItemID.WhiteTorch,
                ItemID.YellowTorch,
            });
            RecipeGroup.RegisterGroup("AnyTorch", group);
            #endregion
            #region 任意AddRecipe()中
            //...
            recipe.AddRecipeGroup("AnyTorch", 10);  //添加自定义合成组
            recipe.AddRecipeGroup(group);       //直接使用RecipeGroup类为参数
            recipe.AddRecipeGroup(RecipeGroupID.Wood);  //原版合成组
            //...
            #endregion
        }
        public static void 微光转化() {
            recipe.AddCustomShimmerResult(ItemID.DirtBlock, 10);
            recipe.HasShimmerCondition(Condition.Hardmode); //微光转化的条件, 若没有则不加
        }
        public static void 修改产物() {
            recipe.ReplaceResult(ModContent.ItemType<ModItem>());   //替换结果, 虽说可以传入ModItem, 但是和传入itemID的效果是一样的
            recipe.createItem.stack = 2;    //也可以做一些其它修改, 这些修改都会反应到产物上, 而且在制作出来前就会知道
            //在制成时执行, 可以对item进行修改以修改产物, 但修改只会在制作出来后才能生效
            recipe.AddOnCraftCallback((recipe, item, consumedItems, destinationStack) => { });
        }
    }
    public class 添加饰品 {
        public class ExampleAccessories : ModItem {
            public static string 参考 = "自定义饰品和翅膀 https://fs49.org/2020/03/11/%e8%87%aa%e5%ae%9a%e4%b9%89%e9%a5%b0%e5%93%81%e5%92%8c%e7%bf%85%e8%86%80/";
            public static string 说明 = "需额外准备与类名同名且在对应命名空间下的图片(.png)文件";

            public override void SetDefaults() {
                Item.width = 16;
                Item.height = 16;

                // 重点在这里, 这个属性设为true才能带在身上
                Item.accessory = true;

                // 物品的面板防御数值, 装备了以后就会增加
                Item.defense = 16;

                Item.rare = ItemRarityID.Yellow;
                Item.value = Item.sellPrice(gold: 5);

                // 这个属性代表这是专家模式专有物品, 稀有度颜色会是彩虹的！
                Item.expert = true;
            }
            /// <summary>
            /// 每帧执行
            /// </summary>
            public override void UpdateAccessory(Player player, bool hideVisual) {
                player.lifeRegen += 10;     //增加生命回复
                player.statLifeMax2 += 100; //让玩家生命上限增加100
                player.statManaMax2 += 100; //让玩家魔力上限增加100
                #region 增加跳跃高度
                player.jumpSpeedBoost = 5f;
                player.jumpBoost = true;
                #endregion
                #region 连跳
                //具体待测试
                player.ExtraJumps[0].Enable();  //启用
                player.ExtraJumps[0].Disable(); //禁用
                Show(player.ExtraJumps[0].Active);  //是否正在进行这段跳跃
                Show(player.ExtraJumps[0].Available);   //是否能够使用,在跳起后恢复前返回假
                Show(player.ExtraJumps[0].Enabled);     //判断是否启用,会每帧重置
                #endregion
                #region 让玩家直上直下
                if(!player.controlJump && !player.controlDown) {
                    player.gravDir = 0f;
                    player.velocity.Y = 0;
                    player.gravity = 0;
                    player.noFallDmg = true;
                }
                if(player.controlDown) {
                    player.gravity = Player.defaultGravity;
                    player.gravDir = 1;
                    player.noFallDmg = true;
                }
                #endregion
            }
        }
        [AutoloadEquip(EquipType.Wings)]
        public class ExampleWings : ModItem {
            public static string 参考 = "自定义饰品和翅膀 https://fs49.org/2020/03/11/%e8%87%aa%e5%ae%9a%e4%b9%89%e9%a5%b0%e5%93%81%e5%92%8c%e7%bf%85%e8%86%80/";
            public static string 说明 = "需额外准备名字为 [类名]_Wings 且在对应命名空间下的图片(.png)文件(四帧)";
            public override void SetDefaults() {
                Item.width = 22;
                Item.height = 20;
                Item.value = 10000;
                Item.rare = ItemRarityID.Green;
                Item.accessory = true;
            }
            public override void UpdateAccessory(Player player, bool hideVisual) {
                #region 在显示翅膀时飞行时间更久
                player.wingTimeMax = hideVisual ? 50 : 200;
                #endregion
                player.wingTime = player.wingTimeMax;    //让玩家可以一直飞行, 若写了这句那么只要 wingTimeMax >= 1 就是相同的效果
                #region 让玩家可以虚空行走
                if(!player.controlJump && !player.controlDown) {
                    player.gravDir = 0f;
                    player.velocity.Y = 0;
                    player.gravity = 0;
                }
                if(player.controlDown) {
                    player.gravity = Player.defaultGravity;
                    player.gravDir = 1;
                }
                #endregion
                player.noFallDmg = true;    //不会摔伤
            }
            /// <summary>
            /// 翅膀垂直移动数据
            /// </summary>
            /// <param name="ascentWhenFalling">控制玩家在下落的时候开启翅膀的爬升率, 如果设为0那么下落的时候就需要比较长的时间才能上去</param>
            /// <param name="ascentWhenRising">玩家切换到上升状态的时候开启翅膀的爬升率</param>
            /// <param name="maxCanAscendMultiplier">待测试</param>
            /// <param name="maxAscentMultiplier">玩家可以到达的最大爬升率</param>
            /// <param name="constantAscend">是正常飞行时翅膀的爬升率</param>
            public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
                constantAscend = 1f;
                maxAscentMultiplier = 2f;
            }
            /// <summary>
            /// 翅膀水平移动数据
            /// </summary>
            public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration) {
                speed = 20f;
                acceleration = 2.5f;
            }
        }
        public static void 可能用到的玩家属性(Player player) {
            Show(player.statDefense);
            player.endurance += .1f;    //增加10%伤害减免, 有1f的软上限, 不会让承受的伤害低于1
            player.noKnockback = true;  //免疫击退
            #region 伤害
            player.GetDamage(DamageClass.Generic) += 0.2f;  //全部武器增伤20%(加算)
            player.GetDamage(DamageClass.Melee) *= 1.2f;    //近战武器伤害翻1.2倍(乘算)
            player.GetDamage<MagicDamageClass>() -= 0.2f;   //魔法武器减伤20%
            #endregion
            #region 暴击
            player.GetCritChance<RangedDamageClass>() += 0.1f;  //增加10点暴击率(待测试单位)
            Show(player.GetTotalCritChance<RangedDamageClass>());   //获取总暴击
            Show(player.GetWeaponCrit(player.HeldItem));    //获取武器暴击(待测试)
            #endregion
            #region 攻速
            player.GetAttackSpeed<MeleeDamageClass>() += 0.5f;
            Show(player.GetTotalAttackSpeed<MeleeDamageClass>());
            #endregion
            #region 击退
            player.GetKnockback<MeleeDamageClass>() += 0.5f;    //暂时不知道单位
            #endregion
            #region 护甲穿透(暂时不知道是什么东西)
            player.GetArmorPenetration<GenericDamageClass>() += 0.2f;
            #endregion
            #region 生命魔力回复
            player.lifeRegen += 2;          //生命每两秒增加2
            Show(player.lifeRegenCount);    //player生命恢复的逻辑用品, 一般不用   在到达120或-120后, 会让玩家生命+1或-1
            player.lifeRegenTime += 0.2f;   //让玩家(在受击后)更快的开始生命回复

            player.manaRegen += 2;          //魔力每两秒增加2
            player.manaRegenBonus += 2;     //未知
            Show(player.manaRegenBuff);     //是否有魔力恢复药的buff
            Show(player.manaRegenCount);    //参见lifeRegenCount
            Show(player.manaRegenDelay);    //待测试
            #endregion
            #region 召唤物
            player.maxMinions += 1;     //召唤物数量+1
            Show(player.slotsMinions);  //已召唤的召唤物槽位, 会被限制在player.maxMinions下
            Show(player.numMinions);    //已召唤的召唤物个数
            #endregion
            #region 位移相关
            player.gravity = Player.defaultGravity;     //取消太空失重(待测试)
            Show(player.gravDir);   //1为朝下, -1朝上
            player.noFallDmg = true;    //不再摔伤
            #region 移动
            Show(player.maxRunSpeed);
            Show(player.accRunSpeed);
            Show(player.runAcceleration);
            Show(player.runSlowdown);
            Show(player.moveSpeed);
            Show(player.jumpSpeedBoost);
            Show(player.jumpBoost);
            #endregion
            #region 翅膀
            player.wingTimeMax = 180;   //飞行时间
            Show(player.wingTime);      //剩余飞行时间
            #endregion
            #endregion
            Show(player.lifeMagnet);        //是否扩大生命心的捡拾范围
            Show(player.manaMagnet);        //是否扩大魔力心的捡拾范围
            Show(player.treasureMagnet);    //是否扩大钱币的捡拾范围
            player.AddBuff(BuffID.Poisoned, 30);    //添加持续30帧的中毒debuff, 若在UpdateAccessory中直接写会一直存在
        }
    }
    public class 添加套装 {
        public static string 参考 = "自定义套装和Buff https://fs49.org/2020/03/12/%e8%87%aa%e5%ae%9a%e4%b9%89%e5%a5%97%e8%a3%85%e5%92%8cbuff/";
        public static string 简介 = """
            添加头盔, 护甲和护腿需在对应ModItem子类上添加[AutoloadEquip(EquipType.{Head / Body / Legs})]
            在SetDefaults()中用Item.defense设置防御
            重写UpdateEquip(Player player)以在装备时做一些事情
            """;
        public static void 套装效果(Item head, Item body, Item legs, Player player) {
            #region 在头盔中重写IsArmorSet(Item head, Item body, Item legs)
            Do(head);       //head其实不需要
            bool ret = body.type == ItemID.WoodBreastplate && legs.type == ItemID.WoodGreaves;  //头盔中写不用判head
            #endregion
            #region 重写UpdateArmorSet(Player player)
            player.setBonus = "套装效果";   //这里写套装效果的描述, 会显示在装备的Tooltips中
            player.armorEffectDrawShadow = true;    //画出残影
            //可重写ArmorSetShadows(Player player)以自定义效果
            #endregion
        }
        public static void 可能用到的玩家属性(Player player) {
            添加饰品.可能用到的玩家属性(player);
        }
    }
    public class 源码下的基本文件 {
        public static string 必要文件 = """
            build.txt       : 如何构建此模组
            [Mod名].cs      : 相当于主类
            [Mod名].csproj  : 包含了哪些文件
            description.txt : 其实好像不是必须的, 但正式的模组总该有个描述文件
            icon.png        : 好像也不是必须的, 但最好有作为图标
            """;
        public class Build_txt {
            public static string 参考 = "Mod基本信息 https://fs49.org/2020/03/09/mod%e5%9f%ba%e6%9c%ac%e4%bf%a1%e6%81%af/";
            public static string 基本形式 = """
                [key1] = [value1]
                [key2] = [value2]
                ...
                """;
            public static string 支持的一些键 = """
                displayName    : Mod在TML里显示的名字(不是文件夹的名字哦, 可以是中文)
                author         : 作者的名字
                version        : Mod的版本, 会在Mod菜单中显示。注意, 这个属性是要求格式的。格式如下  <数字>.<数字>.<数字>.<数字>, 也就是我们最常见到的版本号的格式, 这里我取了1.0
                dllReferences  : 如果你的Mod要引用外部的dll, 把文件名写在这。文件名不要包含扩展名, 你必须将dll文件放在/lib文件夹中才能构建Mod的引用
                modReferences  : 你的mod依赖的Mod的列表。 同样不包含扩展名。(这里是强依赖
                weakReferences : 弱引用Mod列表, 用于联动Mod, 但是并不要求这个Mod被加载。但是弱引用内容也需要特殊的技巧去处理。
                noCompile      : 这个Mod的源码需不需要被编译, 默认是不用管的。(高级内容, 暂时可以无视)  默认FALSE
                homepage       : 这个Mod主页的链接, 如果有的话, 比如这里是教程官网
                hideCode       : 如果设为true, 你的源码以及编译好的dll就不会被TML抽取, 如果你不想被别人看到源码就设为true    默认FALSE
                hideResources  : 是否隐藏Mod中包含的资源文件, 比如贴图, 音乐文件等等, 不想被别人看见就设为true      默认FALSE
                includeSource  : 是否把.cs源码文件也放入tmod文件中, 这样别人可以看见你写的所有源代码, 我这里设置的是允许。不想允许就设为false(make sure to  set hideCode to false).     默认FALSE
                buildIgnore    : 将Mod源码编译成tmod文件的时候, 哪些文件(夹)是不需要放进tmod文件的, 这样能减小tmod文件的大小。includeSource为false的时候自动会忽略.cs文件   默认build.txt, .gitattributes, .gitignore, .git/, .vs/, .idea/, bin/, obj/,  Thumbs.db
                includePDB     : 需不需要包括符号调试文件, 如果包含可以更多提供debug信息, 同时也允许使用VS进行Debug   默认FALSE
                side           : 这个Mod是客户端Mod还是服务器端Mod     默认Both
                """;
        }
    }
    public class 创建Buff {
        public class ExampleBuff : ModBuff {
            public static string 参考 = "自定义套装和Buff https://fs49.org/2020/03/12/%e8%87%aa%e5%ae%9a%e4%b9%89%e5%a5%97%e8%a3%85%e5%92%8cbuff/";
            public override void SetStaticDefaults() {
                Main.buffNoSave[Type] = true;   //是否不保存, 默认false, 即默认保存
                Main.debuff[Type] = false;      //判定这个buff算不算一个debuff, 如果设置为true会得到TR里对于debuff的限制, 比如无法取消
                Main.lightPet[Type] = false;    //是否为照明宠, 默认false
                Main.vanityPet[Type] = false;   //决定这个buff是不是一个装饰性宠物, 用来判定的, 比如消除buff的时候不会消除它, 默认false
                Main.buffNoTimeDisplay[Type] = false;   //是否不显示buff时间, 默认false
                Main.pvpBuff[Type] = false;     //如果这个属性为true, pvp的时候就可以给对手加上这个debuff/buff
                BuffID.Sets.LongerExpertDebuff[Type] = false;   //这个buff在专家模式会不会持续时间加长, 应用于所有buff而不仅仅是debuff
                BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;  //护士是否不能移除此debuff, 默认false
                BuffID.Sets.TimeLeftDoesNotDecrease[Type] = false;  //是否不自然削减时间, 默认false
            }
            public override void Update(Player player, ref int buffIndex) {
                // 把玩家的所有生命回复清除
                if(player.lifeRegen > 0) {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                // 让玩家的减血速率随着时间而减少
                // player.buffTime[buffIndex]就是这个buff的剩余时间
                player.lifeRegen -= player.buffTime[buffIndex];
                bool 需要手动删除buff = false;
                if(需要手动删除buff) {
                    player.DelBuff(buffIndex--);
                }
            }
            public override void Update(NPC npc, ref int buffIndex) {
                if(npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= npc.buffTime[buffIndex];
            }
            public override bool ReApply(Player player, int time, int buffIndex) {
                player.buffTime[buffIndex] = Math.Max(player.buffTime[buffIndex], time);
                return true;
            }
            public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
                int buffIndex = -1, timeLeft = 0;
                foreach(int i in Range(Main.LocalPlayer.buffType.Length)) {
                    if (Main.LocalPlayer.buffType[i] != Type) continue;
                    buffIndex = i;
                    break;
                }
                if(buffIndex != -1) {
                    timeLeft = Main.LocalPlayer.buffTime[buffIndex];
                }
                if(timeLeft > 0) {
                    tip += $"\n剩余时间{timeLeft / 60f:0.##}s";
                }
            }
        }
    }
    public static void 输出信息() {
        //在屏幕左下角输出
        Main.NewText("info here");  //可以额外传入Color或r, g, b作为颜色
        Main.NewTextMultiline("multilineInfo\nmultilineInfo line 2");   //一次输出多行
    }

    public class 保存数据 {
        public class ModItem保存数据 : ModItem {
            //假设这是你需要保存的数据
            public int dataToSave;
            public override void SaveData(TagCompound tag) {
                if(dataToSave != 0) {  //推荐在非默认值的情况下才存下来
                    tag["dataToSave"] = dataToSave; //名字只需要保证在本物品下不重名即可
                }
            }
            public override void LoadData(TagCompound tag) {
                tag.TryGet("dataToSave", out dataToSave);   //这样即使不存在那也会让dataToSave为默认值

                //如果想不把此类本身的默认值作为默认值(比如要把1作为默认值), 那么可以判TryGet的返回值
                if(!tag.TryGet("dataToSave", out dataToSave)) {
                    dataToSave = 1;     //需要和SaveData处对应
                }
            }
        }
    }
    public class 吟唱武器 {
        public static string 介绍 = """
            item.channel这个属性会让玩家在使用物品后进入channel状态, 可以用player.channel来检测
            """;
        public class ExampleChannelWeapon : ModItem {
            public static string 参考 = "魔法导弹类武器 https://fs49.org/2022/01/22/%e9%ad%94%e6%b3%95%e5%af%bc%e5%bc%b9%e7%b1%bb%e6%ad%a6%e5%99%a8/";
            public override void SetDefaults() {
                //...
                Item.channel = true;
                //...
            }
        }
        public class ExampleChannelProjectile : ModProjectile {
            public static string 参考 = "魔法导弹类武器 https://fs49.org/2022/01/22/%e9%ad%94%e6%b3%95%e5%af%bc%e5%bc%b9%e7%b1%bb%e6%ad%a6%e5%99%a8/";
            public override void AI() {
                #region 得到玩家, 若没有或已死亡等情况则直接让弹幕消失
                Player player = null;
                if(Projectile.owner >= 0 && Projectile.owner < Main.player.Length) {
                    player = Main.player[Projectile.owner];
                }
                if(player == null || !player.active || player.dead || player.ghost) {
                    Projectile.Kill();
                    return;
                }
                #endregion
                if(player.channel) {
                    Vector2 unit = Vector2.Normalize(Main.MouseWorld - player.Center);  //// 从玩家到达鼠标位置的单位向量
                    float rotaion = unit.ToRotation();

                    // 调整玩家转向以及手持物品的转动方向
                    player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
                    player.itemRotation = (float)Math.Atan2(rotaion.ToRotationVector2().Y * player.direction,
                        rotaion.ToRotationVector2().X * player.direction);
                    // 玩家保持物品使用动画
                    player.itemTime = 2;
                    player.itemAnimation = 2;

                    Vector2 vector = Vector2.Normalize(Main.MouseWorld - Projectile.Center);     // 从弹幕到达鼠标位置的单位向量
                    #region 让弹幕缓慢朝鼠标方向移动
                    if(vector.Length() <= 5)   //为防止弹幕在鼠标附近鬼畜
                    {
                        Projectile.velocity = vector;
                    }
                    else {
                        vector.Normalize();
                        Projectile.velocity = vector * 5;
                    }

                    #endregion
                }
                else {
                    if(Projectile.timeLeft > 30) {
                        Projectile.timeLeft = 30;
                    }
                }
            }
        }
    }
    public class 召唤武器 {
        public static string 介绍 = """
            召唤武器其实本质上就是物品带个智能一点的弹幕
            但是为了表示玩家的召唤物的状态, 我们还需要一个Buff以及一个自定义玩家属性来保证召唤物能正常工作以及消失
            """;
        public class ExampleMinionPlayer : ModPlayer {
            public static string 参考 = "召唤武器实战：僚机(1.4再版) https://fs49.org/2022/09/28/%e5%8f%ac%e5%94%a4%e6%ad%a6%e5%99%a8%e5%ae%9e%e6%88%98%ef%bc%9a%e5%83%9a%e6%9c%ba%ef%bc%881-4%e5%86%8d%e7%89%88%ef%bc%89/";
            public bool exampleMinion;
            public override void ResetEffects() {
                exampleMinion = false;
            }
        }
        public class ExampleMinionWeapon : ModItem {
            public static string 参考 = ExampleMinionPlayer.参考;
            public override void SetDefaults() {
                //...
                Item.DamageType = DamageClass.Summon;
                Item.shoot = ModContent.ProjectileType<ExampleMinionProj>();
                Item.shootSpeed = 10f;
                //...
            }
        }
        public class ExampleMinionProj : ModProjectile {
            public static string 参考 = ExampleMinionPlayer.参考;
            public override void SetDefaults() {
                Projectile.width = 16;
                Projectile.height = 16;
                Projectile.friendly = true;
                Projectile.aiStyle = -1;
                Projectile.timeLeft = 3;
                Projectile.penetrate = -1;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.scale = 1.1f;
                // 召唤物必备的属性
                //Main.projPet[Type] = true;    //projPet会让TR系统标记这个弹幕为宠物, 而且会屏蔽召唤物的接触伤害
                //当然这个属性也可以通过 public override bool MinionContactDamage() => false; 来改变
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.netImportant = true;
                Projectile.minionSlots = 1;
                Projectile.minion = true;       //可以防止弹幕因为跟玩家距离太远被刷掉
                ProjectileID.Sets.MinionSacrificable[Type] = true;      //是否会牺牲其它召唤物为此召唤物腾出空间
                ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            }
            public override void AI() {
                #region 得到玩家, 若没有则直接让召唤物消失
                Player player = null;
                if(Projectile.owner >= 0 && Projectile.owner < Main.player.Length) {
                    player = Main.player[Projectile.owner];
                }
                if(player == null || !player.active) {
                    Projectile.Kill();
                    return;
                }
                var modPlayer = player.GetModPlayer<ExampleMinionPlayer>();
                #endregion
                #region 相关处理
                if(player.dead || player.ghost) {
                    modPlayer.exampleMinion = false;
                }
                if(modPlayer.exampleMinion) {
                    Projectile.timeLeft = 2;
                }
                player.AddBuff(ModContent.BuffType<ExampleMinionBuff>(), 2);
                #endregion
            }
        }
        public class ExampleMinionBuff : ModBuff {
            public static string 参考 = ExampleMinionPlayer.参考;
            public override void SetStaticDefaults() {
                Main.buffNoTimeDisplay[Type] = true;
            }
            public override void Update(Player player, ref int buffIndex) {
                var modPlayer = player.GetModPlayer<ExampleMinionPlayer>();
                if(player.ownedProjectileCounts[ModContent.ProjectileType<ExampleMinionProj>()] > 0) {
                    modPlayer.exampleMinion = true;
                }
                if(!modPlayer.exampleMinion)   //玩家取消了这个召唤物
                {
                    player.DelBuff(buffIndex--);
                }
                else {
                    player.buffTime[buffIndex] = 9999;      // 无限buff时间
                }
            }
        }
    }
    public class 坐骑 {
        public static string 介绍 = """
            贴图命名格式: 坐骑名_Front.png 或者 坐骑名_Back.png
            """;
        public class ExampleMountItem : ModItem {
            public static string 参考 = "从零开始的简单坐骑 https://fs49.org/2022/01/17/%e4%bb%8e%e9%9b%b6%e5%bc%80%e5%a7%8b%e7%9a%84%e7%ae%80%e5%8d%95%e5%9d%90%e9%aa%91/";
            public override void SetDefaults() {
                //...
                Item.mountType = ModContent.MountType<ExampleMount>();
                //...
            }
        }
        public class ExampleMount : ModMount {
            public static string 参考 = ExampleMountItem.参考;
            public override void SetStaticDefaults() {
                MountData.spawnDust = DustID.Torch;     //召唤坐骑时的粒子特效
                MountData.buff = ModContent.BuffType<ExampleMountBuff>();
                MountData.heightBoost = 20;     //坐骑给玩家带来的额外高度
                MountData.fallDamage = 0.5f;    //骑着坐骑的时候的掉落伤害, 0.5则为之前的一半
                MountData.runSpeed = 11f;   //坐骑的跑步时的最高速度, 一般很快就会达到这个值, 关于速度的部分还有一个点是, 代码里的速度和游戏里的速度大概是1：5的关系。也就是说10速度, 在游戏里大致会显示50(其实是51)
                MountData.dashSpeed = 8f;   //如果写了这个值, 则坐骑还会在达到跑步最大速度后开始冲刺, 此为冲刺最大速度, 注意要在地上跑才算正在冲刺且加速, 在天上飞是不会加速度的
                MountData.flightTimeMax = 0;    //如果你的坐骑可以飞行, 则设置你的飞行时间, 否则为0
                MountData.fatigueMax = 0;   //可以飞行的坐骑还会有疲劳值, 疲劳值越高, 则坐骑受到重力影响就越大, 此值设置坐骑疲劳值的最大值
                MountData.jumpHeight = 5;   //坐骑的跳跃高度, 越大则跳的越高
                MountData.acceleration = 0.19f; //坐骑的加速度, 越大则越容易达到最大速度, 正常来说都是小数, 大于1的加速度非常快
                MountData.jumpSpeed = 4f;   //跳跃速度
                MountData.blockExtraJumps = false;  //这个值对于飞行或悬浮坐骑来说比较关键, 禁用额外跳跃
                MountData.constantJump = true;  //连跳, 如果为true的话按住空格就可以每次落地时自动跳跃
                /*
                此值为true是则表明该坐骑为一个悬浮坐骑, 悬浮坐骑拥有的属性就很多了
                首先他真的是悬浮的(蛤蛤)
                其次他的疲劳值不会随飞行时间增加, 但是它仍然受到飞行时间的影响
                飞行时间到了之后表现和其他坐骑不一样, 他只会单纯的往你最后速度的方向飘
                比如你在上升, 他就会一直上升, 当然你可以按“下”方向键来下降, 也可以左右移动, 但是不能再上升了
                相当于飞行时间到了后, 此坐骑的上升加速度始终为0
                */
                MountData.usesHover = false;

                MountData.totalFrames = 4;      //坐骑的总帧数
                int[] array = new int[MountData.totalFrames];
                foreach(int i in Range(MountData.totalFrames)) {
                    array[i] = 20;
                }
                MountData.playerYOffsets = array;   //这个值决定了每一帧玩家贴图的位置在哪里
                MountData.xOffset = 13;     // x/y offset 影响玩家的位置和坐骑的位置的偏差
                MountData.yOffset = -12;
                MountData.playerHeadOffset = 22;
                #region 运动时的帧图控制
                MountData.bodyFrame = 3;    //自己的坐骑使用的人物动作

                MountData.standingFrameStart = 0;   //以下三条: 从start开始, 每个运动帧间隔delay帧, 一共有count帧
                MountData.standingFrameCount = 4;
                MountData.standingFrameDelay = 12;

                MountData.runningFrameStart = 0;
                MountData.runningFrameCount = 4;
                MountData.runningFrameDelay = 12;

                MountData.flyingFrameStart = 0;
                MountData.flyingFrameCount = 0;
                MountData.flyingFrameDelay = 0;

                MountData.inAirFrameStart = 0;
                MountData.inAirFrameCount = 1;
                MountData.inAirFrameDelay = 12;

                MountData.idleFrameStart = 0;
                MountData.idleFrameCount = 4;
                MountData.idleFrameDelay = 12;
                MountData.idleFrameLoop = true;

                MountData.swimFrameStart = MountData.inAirFrameStart;
                MountData.swimFrameCount = MountData.inAirFrameCount;
                MountData.swimFrameDelay = MountData.inAirFrameDelay;
                #endregion
            }
        }
        public class ExampleMountBuff : ModBuff {
            public static string 参考 = ExampleMountItem.参考;
            public override void SetStaticDefaults() {
                //...
                Main.buffNoTimeDisplay[Type] = true;
                //...
            }
            public override void Update(Player player, ref int buffIndex) {
                player.mount.SetMount(ModContent.MountType<ExampleMount>(), player);
                player.buffTime[buffIndex] = 10;
            }
        }
        public class 简易自由悬浮坐骑 : ModMount {
            public static string 参考 = ExampleMountItem.参考;
            public override void UpdateEffects(Player player) {
                float verticalSpeedMax = 15f;
                float verticalAcc = 0.5f;
                float stopAcc = 1f;
                int verticalControl = (player.controlDown ? 1 : 0) + (player.controlUp || player.controlJump ? -1 : 0);
                player.gravity = 0;
                player.maxFallSpeed = verticalSpeedMax;
                player.velocity.Y += verticalAcc * verticalControl;
                if(player.velocity.Y > verticalSpeedMax) {
                    player.velocity.Y = verticalSpeedMax;
                }
                else if(player.velocity.Y < -verticalSpeedMax) {
                    player.velocity.Y = -verticalSpeedMax;
                }
                if(verticalControl == 0) {
                    if(player.velocity.Y > stopAcc) {
                        player.velocity.Y -= stopAcc;
                    }
                    else if(player.velocity.Y < -stopAcc) {
                        player.velocity.Y += stopAcc;
                    }
                    else {
                        player.velocity.Y = 0;
                    }
                }
                player.fallStart = (int)player.position.Y / 16;
            }
        }
        public static void 获取贴图(Mount.MountData mountData) {
            if(Main.netMode != NetmodeID.Server) {
                Show(mountData.backTexture);  //当贴图为 坐骑名_Back.png 时用这个
                Show(mountData.frontTexture); //当贴图为 坐骑名_Front.png 时用这个
                                              //当然也可以两个都用
            }
        }
    }
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
                TileObjectData.newTile.CoordinateHeights = new [] { 16, 16, 16 }; //每一块的高度
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
                NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0) {
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
    public class 绘制基础 {
        public static string 参考 = "简单绘制 https://fs49.org/2021/12/22/%e7%ae%80%e5%8d%95%e7%bb%98%e5%88%b6/";
        public static string 世界坐标与屏幕坐标 = """
            世界坐标是从整个世界的左上角算起的, 玩家和npc的位置都是基于世界坐标的
            但是屏幕坐标是基于屏幕左上角的
            那么绘制的时候, 我们需要把世界坐标转换成为屏幕坐标
            我们需要把世界坐标转换成屏幕坐标, 那么我们会用到Main.screenPosition(屏幕在世界的坐标)
            那么世界坐标转屏幕坐标就只用减去 Main.screenPosition就好了
            """;
        public static string 绘制流程 = """
            一定不要在没有SpriteBatch为参数的重写函数里写绘制函数!!!(初学者)
            (其他地方可以通过Main.spriteBatch获取
            """;
        public static void 获得贴图() {
            #region params
            string texturePath = "ExampleMod/Images/SomeItem";
            Texture texture = default;
            Mod mod = null;
            int itemID = 0;
            #endregion
            //texturePath以mod主类名为开始, 以 '/' 作为分割, 以图片名结尾, 不包含图片文件后缀(.png)
            //但这种方法在图片还没被加载时可能获得不到
            Show(ModContent.Request<Texture2D>(texturePath).Value);
            Main.GetItemDrawFrame(itemID, out var itemTexture, out Rectangle itemFrame);    //稳定的方法, 但获取的图片有范围限制, 获取texture的方式与下面缩进的语句相同
            Main.instance.LoadItem(itemID);
            if(TextureAssets.Item[itemID].State == AssetState.NotLoaded) {
                Main.Assets.Request<Texture2D>(TextureAssets.Item[itemID].Name);
            }
            itemTexture = TextureAssets.Item[itemID].Value;
            ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad);    //待测试
            //获取帧图, 参数分别为: 总共的{横/纵}向帧数, 当前处于{横/纵}向第几帧(从0开始), {横/纵}向偏移
            ModContent.Request<Texture2D>(texturePath).Frame(horizontalFrames: 1, verticalFrames: 1, frameX: 0, frameY: 0, sizeOffsetX: 0, sizeOffsetY: 0);
        }
        public static void DrawInSpriteBatch() {
            string 参考 = "对数螺线: 帧图绘制与用绘制整出的花活 https://fs49.org/2021/12/26/%e5%af%b9%e6%95%b0%e8%9e%ba%e7%ba%bf-%e5%b8%a7%e5%9b%be%e7%bb%98%e5%88%b6%e4%b8%8e%e7%94%a8%e7%bb%98%e5%88%b6%e6%95%b4%e5%87%ba%e7%9a%84%e8%8a%b1%e6%b4%bb/";
            #region params
            SpriteBatch spriteBatch = default;
            Texture2D texture = default;
            Rectangle sourceRectangle = default, destinationRectangle = default;
            Vector2 position = default, origin = default;
            Color color = default, textColor = default, borderColor = default;
            float rotation = default, scale = 1f;
            DynamicSpriteFont font = default;
            #endregion
            #region 绘制图片
            //以下position, rectangle等坐标除非特殊说明皆基于屏幕坐标
            //
            /*
            texuture 是Draw出来的图片
            position 屏幕坐标。图像要被绘制的位置
            sourceRectangle 代表需要绘制被绘制贴图的哪一部分, 如果全部绘制就是 null, 一般情况下是 null，绘制帧图的时候才需要设置其他值
            color 要把绘制图片染成什么颜色，一般来说 Color.White（不染色）就行了
            rotation 图像的旋转弧度
            origin, 大概就是图像从哪个位置开始扩散, 从中间开始扩散就填 texuture.Size() / 2f, 左上角就填 Vector2.Zero
                图片的绘制中心, destinationRectangle 参数的x,y 或 position就是这个点在屏幕中绘制出来的实际位置
                旋转为基于此点, 翻转貌似也是(待测试)
            scale 图像缩放倍数
            SpriteEffects effects 图像的翻转，可以水平翻转也可以垂直翻转, 若都翻转可以用SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
            最后一个填0，不用管那么多
            重载:
            scale 图像缩放倍数, 可以为float或Vector2
            position 可替换为destinationRectangle, 然后就不用scale了
                destinationRectangle: 目标矩形, 会自动拉伸图片, 会在设置目标矩形后再进行旋转和翻转(待测试)
            color以后的可以省略
            在color以后的参数省略的情况下sourceRectangle也可以省略
            */
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, 0);
            #endregion
            #region 绘制文字
            //文字绘制的函数建议使用 Terraria.Utils.DrawBorderStringFourWay(...) 而不是 spriteBatch.DrawString(...)
            /*
            font常用Main.fontMouseText(原版鼠标字体)
            */
            Utils.DrawBorderStringFourWay(spriteBatch, font, "text", position.X, position.Y, textColor, borderColor, origin, scale);
            /*
            font可以为DynamicSpriteFont或SpriteFont
                DynamicSpriteFont可用FontAssets.ItemStack.Value等
            text可以为string或StringBuilder
            scale可以为float或Vector2
            以上三条加上{可选的rotation等参数}凑出了12个重载
            */
            spriteBatch.DrawString(font, "text", position, color);
            spriteBatch.DrawString(font, "text", position, color, rotation, origin, scale, SpriteEffects.None, 0);
            #endregion

        }
        public class 拖尾弹幕 : ModProjectile {
            public static string 参考 = "TeddyTerri：使用绘制来实现影子拖尾 https://fs49.org/2022/03/28/teddyterri%ef%bc%9a%e4%bd%bf%e7%94%a8%e7%bb%98%e5%88%b6%e6%9d%a5%e5%ae%9e%e7%8e%b0%e5%bd%b1%e5%ad%90%e6%8b%96%e5%b0%be/";
            //此处为自定义储存方式, 同时还应储存旋转, spriteDirection等信息, 但此处没写, 因为只是演示, 也有官方的处理方法
            public Vector2[] oldPositions = new Vector2[15];    //顺便写下仿队列
            public int endIndex;
            public bool full;

            private Texture2D texture;
            public override void SetDefaults() {
                //...
                ProjectileID.Sets.TrailCacheLength[Type] = 15;
                /*
                0: 仅记录坐标于projectile.oldPos数组(1帧一次), 其中0号为最新的一次记录, 也就是说每次记录都会将其他的项后移
                1: 在弹幕存在帧图时仅在framecounter == 0下记录一次
                2: 除了记录了坐标，还记录了rotation与spriteDirection(于oldRot和oldSpriteDirection)
                3: 包括2, 但会强制改rotation为上一帧到此帧的方向
                4: 包括2, 但会每帧将坐标额外加上玩家的位移(适用于天顶剑等轨迹跟随玩家(一般为在屏幕内不变)的弹幕)
                */
                ProjectileID.Sets.TrailingMode[Type] = 0;
                texture = ModContent.Request<Texture2D>("ExampleMod/Content/Projectiles/SomeProjectile").Value;
                //...
            }

            public override void AI() {
                //...

                //自定义储存方式, 每隔一段时间记录旧位置
                if((int)Main.time % 2 == 0) {
                    oldPositions[endIndex] = Projectile.position;
                    if(++endIndex >= oldPositions.Length) {
                        endIndex -= oldPositions.Length;
                        full = true;
                    }
                }
            }

            public override bool PreDraw(ref Color lightColor)  //把残影画在弹幕后面就要用PreDraw
            {
                IEnumerable indexes = full ? Range(endIndex, oldPositions.Length).Concat(Range(endIndex)) : Range(endIndex);
                int transStep = oldPositions.Length - (full ? oldPositions.Length : endIndex);
                foreach(int index in indexes) {
                    float transparency = ++transStep / (oldPositions.Length + 1f);  //让第一个和最后一个都不是1f或0f
                    Main.spriteBatch.Draw(texture, oldPositions[index], lightColor * transparency);    //当然理应还要加上旋转和{帧图(如果有)}
                }
                return true;
            }
        }
    }
    public class Mod联动 {
        public static string 依赖关系 = """
            在build.txt中modReferences中填的mod名表示强{依赖/引用}, weakReferences表示弱{依赖/引用}
            在强依赖时, 依赖的模组肯定会先于本模组被加载, 否则可能在之后加载或者根本不会加载
            若要指定最低版本, 则在build.txt的references的mod名后加@[版本号], 如 modReferences = ExampleMod@1.0.0.0
            可以将引用的mod的dll添加到项目依赖中获得代码补全
            """;
        public static void 获取其他模组的东西() {
            if(ModLoader.TryGetMod("CalamityMod", out Mod calamity)) {
                calamity.TryFind("PlantyMush", out ModItem plantyMush); //获得特定模组的特定ModItem
            }
            Show(ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC deviantt) ? deviantt.Type : 0);   //获取特定模组的NPCID
        }
        public static void Call接口() {
            #region params
            float bossValue = 1f;
            Func<bool> bossDowned = () => false;
            #endregion
            Show("通过调用其他Mod的Call接口来做到联动, 需要其他mod写好Call接口");
            if(ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist)) {
                bossChecklist.Call("AddBoss", "boss name", bossValue, bossDowned);
            }
        }
        public static class 直接使用其他mod的类 {
            public static class 强引用 {
                public static string intro = """
                    在build.txt中的modReferences中填上需要强引用的mod名(后加 @[版本号] 可以指定最低版本)
                    将引用的mod的dll添加到项目的程序集依赖, 如果有源码可以直接将其添加到项目的项目依赖
                    然后就可以直接使用对应mod的公开的类了
                    (然而如果实在要用到私有或内部(internal)的类的话仍需通过反射来调用)
                    """;
                public static void ShowModReferences() {
                    #region 以强引用更好的体验为例, 由于它开源所以我就直接使用它源码了
                    string note = """
                        如果没有源码也可以在项目文件(.csproj)中添加
                        Project.ItemGroup.Reference(Include="ImproveGame").HintPath: lib\ImproveGame.dll
                        (我在那里注释好了的)
                        """;
                    var improveGame = ImproveGame.ImproveGame.Instance; //此模组内置获得实例的方法
                        //注意此属性是对方Load时添加的, 如果为弱引用, 则在Load阶段不能保证有值, 不过此处为强引用就无所谓了
                        //若模组不内置获得实例的方法, 也可以通过下面这句获取
                    //ImproveGame.ImproveGame improveGame = (ImproveGame.ImproveGame)ModLoader.GetMod(nameof(ImproveGame.ImproveGame));)
                    var improveConfigs = (ImproveConfigs)improveGame.GetConfig(nameof(ImproveConfigs)); //获取其配置
                    Show(improveConfigs.AlchemyGrassGrowsFaster); //获得其配置中的一项

                    //活得玩家背包第一个旗帜盒
                    BannerChest bannerChest = Main.LocalPlayer.inventory.Select(i => i?.ModItem as BannerChest).FirstOrDefault(bc => bc != null);
                    bannerChest?.StoredBanners.ForEach(i => Show(i)); //遍历它里面的每一个旗帜, 可以对它们做一些事情, 或依据它们做一些事情
                    #endregion
                }
            }

            [JITWhenModsEnabled("WingSlotExtra")]
            public static class 弱引用 {
                public static string intro = """
                    在build.txt中的weakReferences中填上需要弱引用的mod名(后加 @[版本号] 可以指定最低版本)
                    将引用的mod的dll添加到项目的程序集依赖, 如果有源码可以直接将其添加到项目的项目依赖
                    然后就可以获得代码补全了
                    需要在用到对方的类的类打上[JITWhenModsEnabled(ModNames)]
                    """;
                public static void ShowModReferences() {
                    #region 以联动额外翅膀栏为例
                    //!! TBT 待测试
                    Main.LocalPlayer.GetModPlayer<ModAccessorySlotPlayer>();
                    var slot = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<WingSlotExtraSlot>().Type);
                    var wing = slot.FunctionalItem;   //这样就能获得额外翅膀栏的饰品了
                    #endregion
                }
            }
        }
    }

    public class UI制作 {
        public class ExampleUI : UIState {
            public static string 参考 = "UI的制作 https://fs49.org/2020/03/27/ui%e7%9a%84%e5%88%b6%e4%bd%9c/";
            public bool visible;
            public override void OnInitialize() {
                #region 实例化一个面板，并且将其注册到UIState
                //实例化一个面板
                UIPanel panel = new();
                //设置面板的宽度
                panel.Width.Set(488f, 0f);
                //设置面板的高度
                panel.Height.Set(568f, 0f);
                //设置面板距离屏幕最左边的距离
                panel.Left.Set(-244f, 0.5f);
                //设置面板距离屏幕最上端的距离
                panel.Top.Set(-284f, 0.5f);
                //将这个面板注册到UIState
                //若需要将其他UIElement添加到panel上则在添加完毕后调用
                Append(panel);
                #endregion
                #region 往面板上面添加一个按钮
                //用tr原版图片实例化一个图片按钮
                UIImageButton button = new(ModContent.Request<Texture2D>("Terraria/UI/ButtonDelete"));
                //设置按钮距宽度
                button.Width.Set(22f, 0f);
                //设置按钮高度
                button.Height.Set(22f, 0f);
                //设置按钮距离所属ui部件的最左端的距离
                button.Left.Set(-11f, 0.5f);
                //设置按钮距离所属ui部件的最顶端的距离
                button.Top.Set(-11f, 0.5f);//注册一个事件，这个事件将会在按钮按下时被激活
                button.OnLeftClick += (evt, element) => visible = false;
                //将按钮注册入面板中，这个按钮的坐标将以面板的坐标为基础计算
                panel.Append(button);
                #endregion
                #region 添加自定义UI
                进度条 progressBar = new(() => Main.LocalPlayer.statLifeMax2, () => Main.LocalPlayer.statLife);
                progressBar.Width.Set(100f, 0f);
                progressBar.Height.Set(20f, 0f);
                progressBar.Left.Set(20f, 0f);
                progressBar.Top.Set(20f, 0f);
                progressBar.Border = 2f;
                panel.Append(progressBar);
                #endregion

                //更多类型:
                Show(typeof(UIImage));      //用于显示图片
                Show(typeof(UIText));       //用于展示文字
                Show(typeof(UITextBox));    //供玩家输入字符串
                Show(typeof(ItemSlot));     //能放入物品
                Show(typeof(UIList));       //用于显示一个列表
                Show(typeof(UIScrollbar));  //用于配合UIList以滚动列表条
            }
        }
        public class ExampleUISystem : ModSystem {
            public static string 参考 = ExampleUI.参考;
            public static ExampleUI exampleUI;
            public static UserInterface exampleUserInterface;     //UserInterface是用来托管UI事件的一个类
            public override void Load() {
                exampleUI = new();
                exampleUI.Activate();
                exampleUserInterface = new();
                exampleUserInterface.SetState(exampleUI);
            }
            public override void UpdateUI(GameTime gameTime) {
                if(exampleUI.visible) {
                    exampleUserInterface?.Update(gameTime);
                }
            }
            public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
                if(mouseTextIndex != -1) {
                    LegacyGameInterfaceLayer legacyLayer = new("Example: ExampleUI", () =>
                    {
                        if (exampleUI.visible)
                        {
                            exampleUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.UI);
                    layers.Insert(mouseTextIndex, legacyLayer);
                }
            }
        }
        public class ExampleUIPlayer : ModPlayer {
            public static string 参考 = ExampleUI.参考;
            public override void OnEnterWorld() {
                ExampleUISystem.exampleUI.visible = true;
            }
        }

        public class 进度条 : UIElement {
            public static string 参考 = "UI部件示例：进度条 https://fs49.org/2020/03/31/ui%e9%83%a8%e4%bb%b6%e7%a4%ba%e4%be%8b%ef%bc%9a%e8%bf%9b%e5%ba%a6%e6%9d%a1/";
            private float _maxValue;
            private float _value;
            public float MaxValue {
                get => _maxValue;
                set {
                    _maxValue = value;
                    if(_maxValue < 0) {
                        _maxValue = 0;
                    }
                    if(_value > _maxValue) {
                        _value = _maxValue;
                    }
                }
            }
            public float Value {
                get => _value;
                set {
                    _value = value;
                    if(_value < 0) {
                        _value = 0;
                    }
                    else if(_value > _maxValue) {
                        _value = _maxValue;
                    }
                }
            }
            public float BorderX {
                get => PaddingLeft;
                set {
                    value = value < 0 ? 0 : Math.Min(value, GetDimensions().Width / 2);
                    PaddingLeft = PaddingRight = value;
                }
            }
            public float BorderY {
                get => PaddingTop;
                set {
                    value = value < 0 ? 0 : Math.Min(value, GetDimensions().Height / 2);
                    PaddingTop = PaddingBottom = value;
                }
            }
            public float Border {
                get => PaddingLeft;
                set {
                    value = value < 0 ? 0 : Min(value, GetDimensions().Width / 2, GetDimensions().Height / 2);
                    SetPadding(value);
                }
            }
            public Func<float> _setMaxValue;
            public Func<float> _setValue;
            public 进度条(float maxValue, float value) {
                MaxValue = maxValue;
                Value = value;
            }
            public 进度条(Func<float> setMaxValue, Func<float> setValue) {
                _setMaxValue = setMaxValue;
                _setValue = setValue;
                MaxValue = setMaxValue?.Invoke() ?? 0;
                Value = setValue?.Invoke() ?? 0;
            }
            protected override void DrawSelf(SpriteBatch spriteBatch) {
                if(_setMaxValue != null) {
                    MaxValue = _setMaxValue.Invoke();
                }
                if(_setValue != null) {
                    Value = _setValue.Invoke();
                }
                Rectangle boxRect = GetDimensions().ToRectangle();
                Rectangle barRect = GetInnerDimensions().ToRectangle();
                barRect.Width = (int)(barRect.Width * (MaxValue == 0 ? 1 : Value / MaxValue));
                spriteBatch.Draw(ModContent.Request<Texture2D>("Example/Image/UI/box").Value, boxRect, Color.White);    //外边框
                spriteBatch.Draw(ModContent.Request<Texture2D>("Example/Image/UI/bar").Value, barRect, Color.White);    //条
            }
        }
        public 泰拉瑞亚On.动态icon 动态icon;
    }
    public class 自定义配置面板 {
        public static string intro = """
            写一个继承自ConfigElement的类
            然后在想自定义的配置字段或属性上添加特性
            [CustomModConfigItem(typeof(类名))]即可使用
            """;
        public class ExampleCustomConfigElement : ConfigElement {
            //也可以继承ConfigElement<T>, T代表具体要修饰哪种对象
            //这样可以由Value直接获得或设置对象
            protected Color backgroundColor;
            public override void OnBind() {
                #region 某些可用的属性和字段介绍
                Show(Item);                 //Item为包含此对象的东西, 通常为ModConfig
                Show(List);                 //若非空, 代表此对象实际被包含在一个集合内
                Show(List?[Index]);         //如此以获取或设置对应值
                SetObject(null);            //不过一般还是通过如下方式获取或设置对象
                Show(GetObject());
                Show(TooltipFunction);      //获得tooltip, 也可以自己写一个以替代
                Show(Label);                //标签
                Show(TextDisplayFunction);  //获得Label, 也可以自己写一个以替代
                Show(NullAllowed);          //是否允许为空(在ConfigElement中不做什么用), 若用[NullAllowed]标注则在base.OnBind()后会是true

                Show(MemberInfo.MemberInfo);//对应的member info(见反射)
                Show(MemberInfo.IsField);   //member info不是字段就是属性
                Show(MemberInfo.IsProperty);
                Show(RangeAttribute);           //各个特性
                Show(BackgroundColorAttribute);
                Show(IncrementAttribute);
                Show(JsonDefaultValueAttribute);
                Show(ConfigManager.GetCustomAttributeFromMemberThenMemberType<SliderColorAttribute>(MemberInfo, Item, List));   //获得自定义特性
                #endregion
                base.OnBind();

                // 这里可以设置此元素的画面大小, 不过只执行一次, 不是实时更新
                Height.Set(30f, 0f);
                DrawLabel = false;  //在原DrawSelf中是否画出文本, 默认true
                //ConfigElement的backgroundColor竟然是private的真是太可恶了
                //此处为它的默认值
                backgroundColor = BackgroundColorAttribute?.Color ?? UICommon.DefaultUIBlue;

                Append(new UIText(Label, large: true) {
                    TextOriginX = 0f,
                    TextOriginY = .5f,
                    Width = StyleDimension.Fill,
                    Height = StyleDimension.Fill,
                    PaddingLeft = 4f,
                    IsWrapped = true,
                });
                //也可以添加一些其他东西, 如UI
            }
            protected override void DrawSelf(SpriteBatch spriteBatch) {

                void BaseDrawSelf(SpriteBatch spriteBatch) {
                    CalculatedStyle dimensions = GetDimensions();
                    float width = dimensions.Width + 1f;
                    Vector2 position = new(dimensions.X, dimensions.Y);

                    //画面板
                    DrawPanel2(spriteBatch, position, TextureAssets.SettingsPanel.Value, width, dimensions.Height,
                        IsMouseHovering ? backgroundColor : backgroundColor.MultiplyRGBA(new(180, 180, 180)));
                    //画标签
                    if(DrawLabel) {
                        position.X += 8f;
                        position.Y += 8f;
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, TextDisplayFunction(), position,
                            MemberInfo.CanWrite ? Color.White : Color.Gray, 0f, Vector2.Zero, new Vector2(0.8f), width);
                    }
                    //画tooltip
                    if(IsMouseHovering && TooltipFunction != null) {
                        //UIModConfig.Tooltip = TooltipFunction();
                        TMLReflection.UIModConfig.Tooltip = TooltipFunction();
                        //或者可以直接用Main.instance.MouseText(...);
                    }
                }
                BaseDrawSelf(spriteBatch);
                //base.DrawSelf(spriteBatch);
            }
        }
    }

    public class 反射 {
        //其实这不是Terraria而是C#的内容
        public static void Type介绍() {
            //这是System.Type, 它表示一个变量的类型
            Type type;
            int i = 0;
            //通过GetType()可以获得一个变量的类型
            type = i.GetType();
            //也可以通过typeof从类名直接获取
            type = typeof(int);
            //GetType()的实际作用在于获得变量真正的类型, 包括继承等作用后的
            //但是当变量为空时会报错
            //GetType()也可以获得被声明为private或internal的类, 只要你有这个实例

            //type的一些属性:
            Show(type.BaseType);        //基类
            Show(type.Name);            //类名
            Show(type.Namespace);       //所在的命名空间名
            Show(type.FullName);        //基本相当于Namespace.Name
            Show(type.IsAbstract);      //是否是抽象类
            Show(type.IsArray);         //是否是数组
            Show(type.IsClass);         //是否是类
            Show(type.IsEnum);          //是否是枚举类型
            Show(type.IsInterface);     //是否是接口
            Show(type.IsNested);        //是否是嵌套类型(定义在一个类中的类型)
            Show(type.IsPublic);        //是否是公共的
            Show(type.IsNotPublic);     //是否不是公共的
            Show(type.IsSealed);        //是否是封闭的
            Show(type.IsValueType);     //是否是值类型
            type.GetInterfaces();       //获得所有此类型实现或继承了的接口的类型
            type.GetArrayRank();        //获取数组的维度
            type.GetEnumNames();        //获得此枚举变量的所有名字
            type.GetEnumValues();       //获得此枚举变量的所有值
            Enum e = default;
            type.GetEnumName(e);        //获取特定枚举变量的名字
        }
        public static void 获得成员() {
            //假如我们已经获得了一个Type
            Type type = typeof(Main);
            #region 成员
            //获取成员信息, 成员包括了字段, 方法, 属性, 索引器, 事件, 运算符重载, 构造函数, 析构函数, 甚至类型和常量等所有类中可以有的东西
            //因此MemberInfo是以下所有Info的基类
            MemberInfo[] memberInfos = type.GetMember("memberName");    //由于一个名字可能对应不止一个成员, 所以返回的是数组
            //BindingFlags代表对获取成员的一些限制条件, 如 实例/静态, 公共/私有 等
            BindingFlags flags = BindingFlags.Instance;
            //MemberTypes代表获取成员的类型, 如 字段, 属性, 方法 等
            MemberTypes types = MemberTypes.Field;
            //通过限制types和flags来限制获得到的MemberInfo
            type.GetMember("memberName", types, flags);
            type.GetMember("memberName", flags);    //types是可省略的, 如果都省略那么只能获得公共成员
            #endregion
            #region 字段
            //获得字段信息
            FieldInfo fieldInfo = type.GetField("fieldName", flags);
            #endregion
            #region 方法
            //获得方法信息, 其中flags可省略, 但省略后只能获得公共方法,下同
            MethodInfo methodInfo = type.GetMethod("MethodName", flags);
            //通过传入Type数组可规定参数类型
            type.GetMethod("DamageVar", flags, new [] { typeof(float), typeof(float) });
            #region 获得参数信息
            //可以通过GetParameters获得参数的信息
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            ParameterInfo parameterInfo = parameterInfos[0];
            Show(parameterInfo.ParameterType);  //获得此参数的类型
            Show(parameterInfo.Name);           //获得此参数的名字
            Show(parameterInfo.HasDefaultValue);//获得此参数是否有默认值
            Show(parameterInfo.DefaultValue);   //获得此参数的默认值(如果有的话)
            Show(parameterInfo.IsIn);           //是否是in参数
            Show(parameterInfo.IsOut);          //是否是out参数
            Show(parameterInfo.IsRetval);       //待测试(似乎是ref参数)
            Show(parameterInfo.Position);       //获得此参数在参数列表中的位置(从0开始)
            #endregion
            #endregion
            #region 属性或索引器
            //获得属性或索引器
            type.GetProperty("PropertyName", flags);
            Type[] typeArray = new Type[] { typeof(int), typeof(int) };
            //通过传入一个type和一个type数组可指定索引器的返回类型和参数列表
            //flags可与其他null一起省略
            //省略flags的情况下返回类型也可省略
            type.GetProperty("PropertyName", flags, null, typeof(int), typeArray, null);
            #endregion
            #region 构造函数和事件
            //获得构造函数.
            type.GetConstructor(flags, typeArray);
            //获取事件
            type.GetEvent("EventName", flags);
            #endregion
            #region 获得全部成员等
            //以上的数组版本
            type.GetMembers(flags);
            type.GetFields(flags);
            type.GetMethods(flags);
            type.GetProperties(flags);
            type.GetConstructors(flags);
            type.GetEvents(flags);
            #endregion
        }
        public static void 处理成员() {
            //假设我们已经有了一个对应的实例(就算这个实例是object也没问题)
            Main main = default;
            #region 获得和设置字段
            //假设我们已经获得了一个字段信息
            FieldInfo fieldInfo = default;
            //获得此字段, 可强制转化为所需类型
            object value = fieldInfo.GetValue(main);    //当为静态时不传实例(传null), 下同
            //设置字段
            fieldInfo.SetValue(main, value);
            //对于引用类型, 修改类型内的值会不会直接反应到原字段还有待测试
            #endregion
            #region 获得与设置属性
            //假设我们已经获得了一个属性信息
            PropertyInfo propertyInfo = default;
            //获得此属性, 可强制转化为所需类型
            value = propertyInfo.GetValue(main);
            //设置属性
            propertyInfo.SetValue(main, value);
            //也可以通过Get和Set方法调用, 但这样更麻烦, 主要还是用来看它有没有 get / set 访问器
            propertyInfo.GetGetMethod();
            propertyInfo.GetSetMethod();
            #endregion
            #region 调用方法
            //假设我们已经获得了一个方法信息
            MethodInfo methodInfo = default;
            //提前准备好参数
            object[] parameters = new object[] { 1, false };
            //调用
            methodInfo.Invoke(main, parameters);
            //若是使用ref或out传参的方法, 可以通过parameters数组获得改变后的值

            //可以创建此方法的委托以在之后执行
            Type delegateType = typeof(Action<int, bool>);
            methodInfo.CreateDelegate(delegateType, main);
            methodInfo.CreateDelegate<Action<int, bool>>(main);
            #endregion
            #region 设置事件
            //假设我们已经获得了一个事件信息
            EventInfo eventInfo = default;
            //再假设我们写好了一个委托
            Delegate handler = default;
            //添加委托
            eventInfo.AddEventHandler(main, handler);
            //移除委托
            eventInfo.RemoveEventHandler(main, handler);
            //也可以获得添加和移除委托的方法信息
            Show(eventInfo.AddMethod);      //此处可以获得到私有的方法
            Show(eventInfo.RemoveMethod);
            #endregion
            #region 构造函数
            //假设我们已经获得了一个构造函数信息
            ConstructorInfo constructorInfo = default;
            //创建一个实例
            main = (Main)constructorInfo.Invoke(parameters);
            //可以与方法一样创建为委托
            constructorInfo.CreateDelegate(delegateType);
            constructorInfo.CreateDelegate<Func<Main>>();
            #endregion
        }
    }
    public class 特性 {
        public static string 参考 = "C#之特性 https://blog.csdn.net/weixin_39520967/article/details/122676703";
        #region 已定义的特性
        public static class AttributeUsage_attr {
            public static string intro = """
                AttributeUsage特性只能在Attribute的派生类上生效
                构造参数中validOn代表能应用特性的目标类型
                    Inherited代表能否被继承, 默认false
                    AllowMultiple代表是否可以给目标上多个此特性, 默认false
                """;
            [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
            public class ExampleAttribute : Attribute { }
        }
        public static class Obsolete_attr {
            public static string intro = """
                用以标注某些东西已过时, 可传入提示信息(字符串)和是否报错
                """;
            [Obsolete]
            public static int obsoleteField;
            [Obsolete("message")]
            public static void ObsoleteMethod() { }
            [Obsolete("message", true)]
            public delegate void ObsoleteDelegate();
        }
        #endregion
        #region 自定义特性
        [AttributeUsage(AttributeTargets.Class)]
        public class MyCustomAttribute : Attribute {
            public int number;
            public void AMethod() {
                Console.WriteLine(number);
            }
        }
        public static string 声明一个自定义特性 = """
            定义一个类并且这个类派生自System.Attribute
            类的名字以Attribute结尾
            为了安全性, 可以用一个sealed修饰成一个密封类型(非必须的), 以防止被其他类所继承
            (可以参考预定义特性的源码, 其都被sealed所修饰)
            """;
        public static string 成员 = "特性可以有公有成员，但是公有成员只能是字段, 属性或构造函数";
        public static string 限制特性的使用 = "使用AttributeUsage以限制";
        public static void ShowAttribute() {
            //用type.IsDefined(AttributeType, inherit = false)判断特性是否应用到type类型上, inherit代表是否检查继承链
            typeof(特性).IsDefined(typeof(Attribute), inherit: true);
            //用type.GetCustomAttribute[s]([AttributeType, ]inherit = false)或
            //type.GetCustomAttribute[s]<Attribute>(inherit = false)获得类型上的特性
            typeof(特性).GetCustomAttribute<Attribute>();
            Attribute.GetCustomAttribute(typeof(特性).GetMember("name")[0], typeof(Attribute));
        }
        #endregion
    }
    public class 泰拉瑞亚On {
        public class 重写伤害计算 : ModSystem {
            public static string 参考 = "On的功能与应用 https://fs49.org/2021/10/28/on%e7%9a%84%e5%8a%9f%e8%83%bd%e4%b8%8e%e5%ba%94%e7%94%a8/";
            //其实来说这种只改一处的东西最好放在IL中(但我暂时不会)
            public static bool ChangePlayerGetDamage = true;    //一般为写在配置里
            public static bool ChangeNPCGetDamage = true;
            public override void Load() {
                On_Player.HurtModifiers.GetDamage += HookPlayerGetDamage;
                On_NPC.HitModifiers.GetDamage += HookNPCGetDamage;
            }
            public override void Unload() {
                On_Player.HurtModifiers.GetDamage -= HookPlayerGetDamage;
                On_NPC.HitModifiers.GetDamage -= HookNPCGetDamage;
            }
            public static float HookPlayerGetDamage(On_Player.HurtModifiers.orig_GetDamage orig, ref Player.HurtModifiers self, float baseDamage, float defense, float defenseEffectiveness) {
                if(ChangePlayerGetDamage) {
                    float damage = self.SourceDamage.ApplyTo(baseDamage) * self.IncomingDamageMultiplier.Value;
                    float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                    defense = Math.Max(defense - armorPenetration, 0);

                    float damageReduction = defense * defenseEffectiveness;
                    damage = Math.Max(damage * 100 / (damageReduction + 100), 1);   //重点在这里

                    return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
                }
                else {
                    //原版的源码, 相当于orig(self, baseDamage, defense, defenseEffectiveness);
                    float damage = self.SourceDamage.ApplyTo(baseDamage) * self.IncomingDamageMultiplier.Value;
                    float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                    defense = Math.Max(defense - armorPenetration, 0);

                    float damageReduction = defense * defenseEffectiveness;
                    damage = Math.Max(damage - damageReduction, 1);

                    return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
                }
            }
            public static int HookNPCGetDamage(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck) {
                if(ChangeNPCGetDamage) {
                    bool? _critOverride = (bool?)typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
                    if(self.SuperArmor) {
                        float dmg = 1;

                        if(_critOverride ?? crit)
                            dmg *= self.CritDamage.Additive * self.CritDamage.Multiplicative;

                        return Math.Clamp((int)dmg, 1, 10);
                    }

                    float damage = self.SourceDamage.ApplyTo(baseDamage);
                    damage += self.FlatBonusDamage.Value + self.ScalingBonusDamage.Value * damage;
                    damage *= self.TargetDamageMultiplier.Value;

                    int variationPercent = Utils.Clamp((int)Math.Round(Main.DefaultDamageVariationPercent * self.DamageVariationScale.Value), 0, 100);
                    if(damageVariation && variationPercent > 0)
                        damage = Main.DamageVar(damage, variationPercent, luck);

                    float defense = self.Defense.ApplyTo(0);
                    float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                    defense = Math.Max(defense - armorPenetration, 0);

                    float damageReduction = defense * self.DefenseEffectiveness.Value;
                    damage = Math.Max(damage * 100 / (damageReduction + 100), 1);     //重点在这里

                    if(_critOverride ?? crit)
                        damage = self.CritDamage.ApplyTo(damage);

                    return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
                }
                else {
                    //原版的源码, 相当于orig(self, baseDamage, crit, damageVariation, luck);
                    bool? _critOverride = (bool?)typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic).GetValue(self);
                    if(self.SuperArmor) {
                        float dmg = 1;

                        if(_critOverride ?? crit)
                            dmg *= self.CritDamage.Additive * self.CritDamage.Multiplicative;

                        return Math.Clamp((int)dmg, 1, 10);
                    }

                    float damage = self.SourceDamage.ApplyTo(baseDamage);
                    damage += self.FlatBonusDamage.Value + self.ScalingBonusDamage.Value * damage;
                    damage *= self.TargetDamageMultiplier.Value;

                    int variationPercent = Utils.Clamp((int)Math.Round(Main.DefaultDamageVariationPercent * self.DamageVariationScale.Value), 0, 100);
                    if(damageVariation && variationPercent > 0)
                        damage = Main.DamageVar(damage, variationPercent, luck);

                    float defense = self.Defense.ApplyTo(0);
                    float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                    defense = Math.Max(defense - armorPenetration, 0);

                    float damageReduction = defense * self.DefenseEffectiveness.Value;
                    damage = Math.Max(damage - damageReduction, 1);

                    if(_critOverride ?? crit)
                        damage = self.CritDamage.ApplyTo(damage);

                    return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
                }
            }
        }
        public class 动态icon : ModSystem {
            public static string 参考 = "使用反射实现动态Icon https://fs49.org/2022/01/18/%e4%bd%bf%e7%94%a8%e5%8f%8d%e5%b0%84%e5%ae%9e%e7%8e%b0%e5%8a%a8%e6%80%81icon/";
            //其实来说也可以直接写在UpdateUI里面(虽说Hook到DrawMenu确实更加精确)
            //所此处更应该说是对C#反射和泰拉瑞亚UI的应用
            int timer, iconFrame;
            Asset<Texture2D>[] icons;
            Asset<Texture2D> Icon => icons[iconFrame];
            public override void Load() {
                On_Main.DrawMenu += HookDrawMenu;
                icons = new Asset<Texture2D>[7];    //7: 总共多少张图
                foreach(int i in Range(icons.Length)) {
                    icons[i] = ModContent.Request<Texture2D>($"ExampleMod/Images/Icons/Icon{i}");
                }
            }
            public override void Unload() {
                On_Main.DrawMenu -= HookDrawMenu;
            }
            void HookDrawMenu(On_Main.orig_DrawMenu orig, Main self, GameTime time) {
                //其实来说如此频繁地反射应该把FieldInfo存起来
                BindingFlags puf = BindingFlags.Public | BindingFlags.Instance, prf = BindingFlags.NonPublic | BindingFlags.Instance;
                Main.MenuUI.GetField(out List<UIState> _history, "_history", puf);
                UIState his = _history.Find(s => s.GetType().FullName == "Terraria.ModLoader.UI.UIMods");
                if(his == null) {
                    goto EndSetIcon;
                }
                //获得UIMods(继承自UIState)的元素elements
                his.GetField(out List<UIElement> elements, "Elements", prf);
                //elements[0]则是uiElement(参见Terraria.ModLoader.UI.UIMods.OnInitialize())
                elements[0].GetField(out List<UIElement> uiElements, "Elements", prf);
                //获得uiElement的元素uiElements, uiElement.Elements[0]则是uiPanel
                uiElements[0].GetField(out List<UIElement> panelElements, "Elements", prf);
                //uiPanel.Elements[0]为modList, 好在modList是一个UIList, 我们可以直接使用它
                var modItem = uiElements.Find(e => e.GetField("_mod", prf).ToString() == Name);
                if(modItem == null) {
                    goto EndSetIcon;
                }
                //找到我们的模组, 那么modItem.Elements[0]若为UIImage则必为图标(参见Terraria.ModLoader.UI.UIModItem.OnInitialize())
                //其实保险起见也应该写for循环的(
                modItem.GetField(out List<UIElement> itemElements, "Elements", prf);
                if(itemElements[0] is not UIImage uiImage) {
                    goto EndSetIcon;
                }
                uiImage.SetImage(Icon);
            //待测试: 是否需要调用SetField
            EndSetIcon:
                #region 计时
                timer += 1;      //timer += time.ElapsedGameTime.TotalSeconds;
                if(timer >= 6) {
                    timer -= 6;
                    iconFrame += 1;
                    if(iconFrame >= icons.Length) {
                        iconFrame %= icons.Length;  //iconFrame -= icons.Length
                    }
                }
                #endregion
                orig(self, time);
            }
        }
        public class 给任意方法上钩子 {
            public static string 参考 = "通过 HookEndpointManager 动态挂钩子 https://fs49.org/2022/11/20/%e9%80%9a%e8%bf%87-hookendpointmanager-%e5%8a%a8%e6%80%81%e6%8c%82%e9%92%a9%e5%ad%90/";
            public static Type 主角 = typeof(MonoModHooks);
            /// <summary>
            /// 代表原方法, 若为静态方法则没有<paramref name="self"/>
            /// </summary>
            /// <param name="self">此类的实例</param>
            /// <param name="parameters">此方法的参数, 实际使用时需用对应参数替代</param>
            delegate void MethodDelegate(object self, params object[] parameters);
            static void MethodHook(MethodDelegate orig, object self, params object[] parameters) {
                //要改什么在这里写(On)
                orig.Invoke(self, parameters);
            }
            static void Manipulate(ILContext il) {
                //IL代码写在这儿
            }
            public static void ShowHooks() {
                MethodBase method = default;    //使用反射获取
                MonoModHooks.Add(method, MethodHook);   //ON, 会在Mod卸载时自动卸载
                MonoModHooks.Modify(method, Manipulate);  //IL, 但由于我不知道所以就放这儿
            }
        }
    }

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

    public class Document {
        #region namespace Terraria
        #region namespace Terraria;
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
                Show(player.HeldItem);          //手上的物品(不是鼠标上的, 而是在第一排中选中的)
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
                        if (Player.BlockInteractionWithProjectiles > 0 && !Main.mouseRight && Main.mouseRightRelease) {
                            Player.BlockInteractionWithProjectiles--;
                        }
                        PlayerInput.SetZoom_UI();
                        for (int num = Main.DelayedProcesses.Count - 1; num >= 0; num--) {
                            IEnumerator enumerator = Main.DelayedProcesses[num];
                            if (!enumerator.MoveNext())
                                Main.DelayedProcesses.Remove(enumerator);
                        }

                        if (!Main.gameMenu || Main.menuMode != MenuID.FancyUI) {
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
                        if (/*Main.*/_hasPendingNetmodeChange) {
                            Main.netMode = /*Main.*/_targetNetMode;
                            _hasPendingNetmodeChange = false;
                        }

                        if(CaptureManager.Instance.IsCapturing) {
                            return;
                        }

                        Main.ActivePlayerFileData?.UpdatePlayTimer();

                        Netplay.UpdateInMainThread();
                        Main.gameInactive = !_base.IsActive;
                        if (Main.changeTheTitle) {
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
                        if (Main.dedServ) {
                            if (Main.dedServFPS) {
                                Main.updatesCountedForFPS++;
                                if (!Main.fpsTimer.IsRunning)
                                    Main.fpsTimer.Restart();

                                if (Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                                    Main.dedServCount1 += Main.updatesCountedForFPS;
                                    Main.dedServCount2++;
                                    float num2 = Main.dedServCount1 / (float)Main.dedServCount2;
                                    Console.WriteLine(Main.updatesCountedForFPS + "  (" + num2 + ")");
                                    Main.updatesCountedForFPS = 0;
                                    Main.fpsTimer.Restart();
                                }
                            }
                            else {
                                if (Main.fpsTimer.IsRunning)
                                    Main.fpsTimer.Stop();

                                Main.updatesCountedForFPS = 0;
                            }
                        }

                        Dos(/*LocalizationLoader.Update()*/);
                        Dos(/*main.DoUpdate_AutoSave()*/);
                        if (!Main.dedServ) {
                            ChromaInitializer.UpdateEvents();
                            Main.Chroma.Update(Main.GlobalTimeWrappedHourly);
                            if (Main.superFast) {
                                _base.IsFixedTimeStep = false;
                                Main.graphics.SynchronizeWithVerticalRetrace = false;
                            }
                            else {
                                if (Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                                    if (_base.IsActive)
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
                            if (Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                                if (Main.fpsCount >= 30f + 30f * Main.gfxQuality) {
                                    Main.gfxQuality += Main.gfxRate;
                                    Main.gfxRate += 0.005f;
                                }
                                else if (Main.fpsCount < 29f + 30f * Main.gfxQuality) {
                                    Main.gfxRate = 0.01f;
                                    Main.gfxQuality -= 0.1f;
                                }

                                if (Main.gfxQuality < 0f)
                                    Main.gfxQuality = 0f;

                                if (Main.gfxQuality > 1f)
                                    Main.gfxQuality = 1f;

                                if (Main.maxQ && _base.IsActive) {
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
                                if (Main.gfxQuality < 0.8f)
                                    Main.mapTimeMax = (int)((1f - Main.gfxQuality) * 60f);
                                else
                                    Main.mapTimeMax = 0;
                            }

                            if (Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                                Main.UpdateTimeAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
                                if (Main.UpdateTimeAccumulator < 0.01666666753590107 && !Main.superFast) {
                                    if (Main.FrameSkipMode == FrameSkipMode.Subtle)
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
                            if (Main.teamCooldown > 0)
                                Main.teamCooldown--;

                            Do(/*main.DoUpdate_AnimateBackgrounds()*/() => {

                            });
                            Animation.UpdateAll();
                            if (Main.qaStyle == 1)
                                Main.gfxQuality = 1f;
                            else if (Main.qaStyle == 2)
                                Main.gfxQuality = 0.5f;
                            else if (Main.qaStyle == 3)
                                Main.gfxQuality = 0f;

                            Main.maxDustToDraw = (int)(6000f * (Main.gfxQuality * 0.7f + 0.3f));
                            if (Main.gfxQuality < 0.9)
                                Main.maxDustToDraw = (int)(Main.maxDustToDraw * Main.gfxQuality);

                            if (Main.maxDustToDraw < 1000)
                                Main.maxDustToDraw = 1000;

                            Gore.goreTime = (int)(600f * Main.gfxQuality);
                            if (!WorldGen.gen) {
                                Liquid.cycles = (int)(17f - 10f * Main.gfxQuality);
                                Liquid.curMaxLiquid = (int)(Liquid.maxLiquid * 0.25 + Liquid.maxLiquid * 0.75 * Main.gfxQuality);
                                if (Main.Setting_UseReducedMaxLiquids)
                                    Liquid.curMaxLiquid = (int)(2500f + 2500f * Main.gfxQuality);
                            }

                            if (Main.superFast) {
                                Main.graphics.SynchronizeWithVerticalRetrace = false;
                                Main.drawSkip = false;
                            }

                            if (Main.gfxQuality < 0.2)
                                LegacyLighting.RenderPhases = 8;
                            else if (Main.gfxQuality < 0.4)
                                LegacyLighting.RenderPhases = 7;
                            else if (Main.gfxQuality < 0.6)
                                LegacyLighting.RenderPhases = 6;
                            else if (Main.gfxQuality < 0.8)
                                LegacyLighting.RenderPhases = 5;
                            else
                                LegacyLighting.RenderPhases = 4;

                            if (!WorldGen.gen && Liquid.quickSettle) {
                                Liquid.curMaxLiquid = Liquid.maxLiquid;
                                if (Main.Setting_UseReducedMaxLiquids)
                                    Liquid.curMaxLiquid = 5000;

                                Liquid.cycles = 1;
                            }

                            if (WorldGen.tenthAnniversaryWorldGen && !Main.gameMenu) {
                                WorldGen.tenthAnniversaryWorldGen = false;
                            }

                            if (WorldGen.drunkWorldGen || WorldGen.remixWorldGen) {
                                if (!Main.gameMenu) {
                                    WorldGen.drunkWorldGen = false;
                                    WorldGen.remixWorldGen = false;
                                    /*main.*/logoRotation = 0f;
                                    /*main.*/logoRotationSpeed = 0f;
                                    /*main.*/logoScale = 1f;
                                }
                            }
                            else if (Main.gameMenu && Math.Abs(logoRotationSpeed) > 1000f) {
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

                            if (!Main.hasFocus && Main.netMode == NetmodeID.SinglePlayer) {
                                if (!Platform.IsOSX)
                                    _base.IsMouseVisible = true;

                                if (Main.netMode != NetmodeID.Server && Main.myPlayer >= 0)
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

                            if (!Platform.IsOSX)
                                _base.IsMouseVisible = false;

                            SkyManager.Instance.Update(gameTime);
                            if (!Main.gamePaused)
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
                            if ((Main.timeForVisualEffects += 1.0) >= 216000.0)
                                Main.timeForVisualEffects = 0.0;

                            if (Main.gameMenu) {
                                Dos(/*main.UpdateMenu()*/);
                                if (Main.netMode != NetmodeID.Server)
                                    return;

                                Main.gamePaused = false;
                            }

                            if (!Main.CanUpdateGameplay && Main.netMode != NetmodeID.Server)
                                return;

                            Main.CheckInvasionProgressDisplay();
                        }

                        Dos(/*main.UpdateWindyDayState()*/);
                        if (Main.netMode == NetmodeID.Server)
                            Main.cloudAlpha = Main.maxRaining;

                        bool isActive = _base.IsActive;
                        if (Main.netMode == NetmodeID.MultiplayerClient) {
                            Dos(/*main.TrySyncingMyPlayer()*/);
                        }

                        if (Main_CanPauseGame()) {  //Main.CanPauseGame()
                            Dos(/*main.DoUpdate_WhilePaused()*/);
                            PlayerLoader.UpdateAutopause(Main.player[Main.myPlayer]);
                            Main.gamePaused = true;
                            return;
                        }

                        Main.gamePaused = false;
                        Main_OnTickForInternalCodeOnly?.Invoke();   //Main.OnTickForInternalCodeOnly()

                        if ((Main.dedServ || (Main.netMode != NetmodeID.MultiplayerClient && !Main.gameMenu && !Main.gamePaused)) && Main.AmbienceServer != null)
                            Main.AmbienceServer.Update();

                        WorldGen.BackgroundsCache.UpdateFlashValues();
                        Main.LocalGolfState?.Update();

                        if ((isActive || Main.netMode == NetmodeID.MultiplayerClient) && Main.cloudAlpha > 0f)
                            Rain.MakeRain();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
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
                                            Main.dust[num5] = new Dust();
                                            Main.dust[num5].dustIndex = num5;
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
                            if(Netplay.Clients[i].Socket != null)
                                Netplay.Clients[i].Socket.SendQueuedPackets();
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
        #endregion
        #region namespace Terraria.DataStructures
        public class IEntitySource_Interface {
            public static IEntitySource source;
            public static void ShowEntitySource() {
                EntitySource_ItemUse_WithAmmo source_ammo = null;
                Show(source_ammo.AmmoItemIdUsed);   //所使用的弹药的itemID(若没有则为0(待测试))
            }
        }
        #endregion
        #region namespace Terraria.GameContent
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
        #endregion
        #region namespace Terraria.ObjectData
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
                TileObjectData.newTile.CoordinateHeights = new [] { 16, 18 };
                TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
                TileObjectData.addTile(tileType);
            }
        }
        #endregion
        #region namespace Terraria.ID
        public class ItemID_cls {
            public static void ShowItemID() {
                Show(ItemID.None);                  //获得一个原版物品的id
            }
            public class Sets_cls {
                public static void Showets() {
                    Show(ItemID.Sets.BossBag);      //下标对应物品id, 判断一个物品是否是宝藏袋, 也可设置一个物品是否是宝藏袋, 会影响是否会开出开发者套装
                    Show(ItemID.Sets.PreHardmodeLikeBossBag);   //是否为肉前宝藏袋
                    Show(ItemID.Sets.CatchingTool); //是否可以捕捉小动物
                    Show(ItemID.Sets.LavaproofCatchingTool);    //是否可以捕捉岩浆小动物
                    Show(ItemID.Sets.Torches);      //火把
                    Show(ItemID.Sets.WaterTorches); //可以插在水中的火把
                    bool[] _ = ItemID.Sets.Factory.CreateBoolSet(false, ItemID.None, ModContent.ItemType<UnloadedItem>());    //创建一个所有物品个数长度的数组
                }
            }
        }
        public class NPCID_cls {
            public class Sets_cls {
                public static void ShowSets() {
                    Show(NPCID.Sets.CannotDropSouls);       //是否不能掉魂, 小动物, 城镇npc, 无钱币掉落的npc不需要加入其中
                }
            }
        }
        public class ProjectileID_cls {
            public static void ShowProjectileID() {
                Show(ProjectileID.Explosives);  //爆炸!
            }

        }
        public class TileID_cls {
            public static void CommonTileID() {
                #region 制作环境
                Show(TileID.WorkBenches);
                Show(TileID.CrystalBall);
                #endregion

            }
        }
        public class AmmoID_cls {
            public static void ShowAmmoID() {
                #region params
                Item item = null;
                ModItem modItem = null;
                #endregion
                item.useAmmo = AmmoID.Bullet;       //此物品以子弹作为弹药
                item.ammo = AmmoID.Bullet;    //此物品为一种子弹
            }
        }
        #endregion
        #region namespace Terraria.Audio
        public class SoundEngine_cls {
            public static void ShowSoundEngine() {
                SoundStyle soundStyle = SoundID.MenuOpen;
                SoundEngine.PlaySound(SoundID.MenuOpen);    //播放一个声音(可以指定位置)
                var slotID = SoundEngine.PlaySound(SoundID.MenuOpen.WithVolumeScale(2f));    //播放一个2倍音量的声音(待测试)
                SoundEngine.TryGetActiveSound(slotID, out var activeSound);
                activeSound.Sound.Pitch = 2f;   //播放速度
            }
        }
        #endregion
        #region namespace Terraria.ModLoader
        public class Mod_cls {
            public static Mod mod;
            public static string intro = "一个mod需要有一个且仅一个继承自此类的类";
            public static string postSetupContent_func = "重写PostSetupContent()以在PostSetup阶段做一些事情(此时各种数组的大小已经设置好了)";
            public static string unload_func = "重写Unload()以在卸载此模组时做一些事情";
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
            public static string intro = "继承自此类以添加一种物品";
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
            public static string intro = "继承自此类以添加一类物块";
            public static string setStaticDefaults_func = "重写SetStaticDefaults()以在初始化完成后做一些事情";
            public static string addMapEntry_func = "在SetStaticDefaults()中使用AddMapEntry(Color, name = null)以在地图中添加显示, name可使用Language.GetText(path)";
            public static string drop_func = "重写bool Drop(i, j)以改写其掉落, 目前仅对1x1物块生效, 返回true以掉落默认掉落的物品, 可以使用Item.NewItem(...)以手动添加掉落";
            public static string killMultiTile_func = "重写KillMultiTile(i, j, frameX, frameY)以定义在多个联和的块被摧毁时干的事情(只执行一次, 可用以生成掉落物)";
            public static string numDust_func = "重写NumDust(i, j, fail, ref num)以改写敲物块时发出的粒子数";
            public static string createDust_func = "重写bool CreateDust(i, j, ref type)以修改在敲击物块时发出粒子的类型, 返回false使不发出默认粒子";
            public static string rightClick_func = "重写bool RightClick(int i, int j)以设置是否可以右键点击(?)";
            public static void ShowModTile() {
                Show(modTile.MineResist);
                Show(modTile.MinPick);
            }
        }
        public class ModBlockType_cls {
            public static ModBlockType modBlockType;
            public static string intro = "ModTile 和 ModWall的基类";
            public static void ShowModBlockType() {
                Show(modBlockType.DustType);        //物块被击打时发出的粒子
#if Terraria143
                Show(modBlockType.ItemDrop);        //物块被打掉时掉落的物品, 默认0代表什么都不掉
#endif
            }
        }
        public class ModSystem_cls {
            public static ModSystem modSystem;
            public static string intro = "额外的系统";
            public static string load_func = "重写Load()以在加载阶段做一些事情";
            public static string saveWorldData = "重写SaveWorldData(tagCompound)以保存数据";
            public static string loadWorldData = "重写LoadWorldData(tagCompound)以载入数据";
        }
        public class ModPlayer_cls {
            public static ModPlayer modPlayer;
            public static string intro = "继承自此mod以改写人物";
            public static string kill_func = "重写Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)以在死亡时做一些事情";
            public static string save_func = "重写TagCompound Save()以保存数据";
            public static string load_func = "重写Load(TagCompound tag)以在加载时做一些事并获取保存的数据";

        }
        public class GlobalType_cls {
#if Terraria143
            public static GlobalType globalType;
#else
            public static GlobalType<GlobalItem> globalType;
#endif
            public static string appliesToEntity_func = """
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
            public static string intro = "用以魔改所有物品(包括原版)";
            public static string load_func = "重写Load()以在加载阶段做一些事情";
            public static string setDefaults_func = "重写SetDefaults(item)以在原物品SetDefaults时做一些事情";
            public static string saveData_func = "重写SaveData(item, tagCompound)以保存数据";
            public static string loadData_func = "重写LoadData(item, tagCompound)以加载数据";
            public static string canAutoReuseItem_func = "重写bool? CanAutoReuseItem(item, player)以改写是否可自动连用, 不做改动为返回null";
            public static string consumeItem_func = "重写bool ConsumeItem(item, player)以改写在消耗一个消耗品时是否真的消耗掉它, 若没有消耗, 则不会调用OnConsumeItem, 默认返回true";
            public static string canBeConsumedAsAmmo_func = "重写bool CanBeConsumedAsAmmo(ammo, weapon, player)以改写在消耗弹药时是否真的消耗掉它, 默认返回true";
            public static string modifyTooltip_func = "重写ModifyTooltips(item, List<TooltipLine> tooltips)通过修改tooltips可修改介绍文本, 例tooltips.Add(new(Mod, \"Rare\", \"Rare: \" + item.rare))";
            public static string grabRange_func = "重写GrabRange(item, player, ref int grabRange)以改写其拾取距离";
            public static string itemSpace_func = "重写bool ItemSpace(item, player)以判断是否忽略其他条件使玩家吸引物品";
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
        #region Terraria.ModLoader.IO
        public class TagCompound_cls {
            public static TagCompound tagCompound;
            public static string intro = "相当于字典";
            public static string initialize = "可以像初始化字典一样初始化";
            public static void ShowTagCompound() {
                tagCompound = new() {
                    {"tag1", 1 },
                    {"tag2", "a" }
                };                                 //初始化
                Show(tagCompound["tag2"] as string);                    //使用下标引用
                Show(tagCompound.GetInt("tag1"));                       //获取整型
                Show(tagCompound.GetString("tag2"));                    //获取字符串
                tagCompound.Add("tag3", new List<int>() { 1, 2 });      //添加
                tagCompound.Set("tag3", new List<int>() { 1, 2 }, true);//设置
                Show(tagCompound.Get<List<int>>("tag3"));               //获取指定类型
                Show(tagCompound.Get<List<int>>("tag3")); //获取指定类型
                //Get实际上引用了TryGet, 而索引器则引用了Get和Set, Add则是非替代地调用Set
                //总结: 其中效率最高的是TryGet和Set
            }
        }
        #endregion
        #region namespace Terraria.ModLoader.Config
        public class ItemDefinition_cls {
            public static string intro = "在Config中方便的引用一个物品, 或者用来做itemId与mod name和item name的转换";
            public static void ShowItemDefinition() {
                #region params
                string modName = null, itemName = null, key = null;
                int itemId = 0;
                #endregion
                //以下物品名皆指内部物品名
                ItemDefinition def = new();     //默认为Terraria/None
                def = new(modName, itemName);   //模组名和物品名, 原版模组名为Terraria
                def = new(key);                 //以 模组名/物品名 的形式, 原版可直接用物品名
                def = new(itemId);              //传入itemId, 相当于传入ItemID.Search.GetName(type)👇
                ItemID.Search.GetName(itemId);  //模组物品时返回 模组名/物品名, 原版未测试
                def.ToString();                 //以 模组名/物品名 的形式返回
                Do(def.Type);                   //获取对应的itemID, 若没得到返回-1, 不要频繁获取此值
                Do(def.Mod);                    //模组名字, 原版时为Terraria
                Do(def.Name);                   //物品名字
                Do(def.IsUnloaded);             //是否加载, 额外的当modName和itemName都为""时返回假
                Do(ItemID.Search.TryGetId(modName != "Terraria" ? $"{modName}/{itemName}" : itemName, out int id) ? id : -1);
                //def.Type的等价版(源码)
            }
            /// <summary>
            /// 通过物品ID获得模组名和物品名
            /// </summary>
            public static void ItemIdToModAndItemName(int itemId) {
                ItemDefinition def = new(itemId);
                Do(def.Mod);            //获得到的模组名(原版物品时为Terraria)
                Do(def.Name);           //获得到的物品名
            }
            /// <summary>
            /// 通过模组名和物品名获得物品ID, 当找不到时返回-1
            /// </summary>
            public static void ModAndItemNameToItemId(string modName, string itemName) {
                ItemDefinition def = new(modName, itemName);
                Do(def.Type);
            }
        }
        #endregion
        #endregion
        #region namespace Terraria.Localization
        public class Language_cls {
            public static string getTextValue = """
                Language.GetTextValue(key)以获得特定文本, 在不同语言时会自动使用不同翻译
                传入的key为.hjson文件中对应的项(用'.'连接), .hjson文件应位于Localization文件夹下, 英文为en-US.hjson, 中文为zh-Hans.hjson
                在[Label(str)], [Tooltip(str)], [Header(str)]等地方可以直接传入第一个字符为$的字符串以获得对应文本(是""$...""不是$""..."")
                """;
            public static void ShowLanguage() {
                Show(Language.ActiveCulture);       //游戏的语言
                Show(Language.GetTextValue("Mods.TigerTestMod.[...]"));
            }
        }
        #endregion
        #region namespace Terraria.Utility
        public class WeightedRandom_cls {
            public static void ShowWeightedRandom() {
                WeightedRandom<string> randomStrings = new(Main.rand);
                randomStrings.Add("weight 1");
                randomStrings.Add("weight 2", 2);
                randomStrings.Add("weight 3", 3);
                randomStrings.Add("weight 2/3", 2 / 3.0);   //double类型的权值
                string result = randomStrings;  //可以直接隐式转换
            }
        }
        #endregion
        #endregion
        #region IL
        #region namespace MonoMod.Cil
        public class ILCursor_cls {
            public static ILCursor ilCursor;
            public static void ShowILCursor() {
                #region params
                ILContext ilContext = null;
                #endregion
                ilCursor = new(ilContext);

            }
        }
        #endregion
        #region namespace Mono.Cecil.Cil
        public class OpCodes_static_cls {
            public static string tips = """
                在IL中压栈通常以ld开头, 出栈则以st开头
                """;
            public static void ShowOpCodes() {
                Show(OpCodes.Add);              //将计算堆栈上的两个值相加并将结果推送到计算堆栈上
                Show(OpCodes.Nop);              //空语句
                Show(OpCodes.Ldstr);            //入栈字符串
                Show(OpCodes.Call);             //调用函数
                Show(OpCodes.Ret);              //返回
                Show(OpCodes.Ldloc_0);          //入栈调用堆栈索引为0处的值
                Show(OpCodes.Ldc_I4_0);         //入栈序号为0的int32数值
                Show(OpCodes.Ldarg_0);          //入栈第一个成员参数
            }
        }
        #endregion
        #endregion
    }
    #region 额外研究
    public class 可空类型Nullable {
#nullable enable
        public static string intro = """
            关于各种 ? 的事情
            """;
        public static string 可为空的值类型 = """
            给值类型后加上?变为可为空的值类型(int? i = null;)(实际上会变为结构体Nullable<int>)
            此类型基本可以正常的当作原类型使用, 只是有空值时计算结果一般为空(2 + (int?)null -> null)
            特别的, 做大小于(包括不大于和不小于)运算时, 若有空值则必为假(此时小于等于不再与小于或者等于等效)
            而等于和不等于则会考虑是否为空的情况
            如果是必须使用原类型的场合(如赋值, 传参)可以直接强制类型转换, 但若为空会报错
            可以直接将原来的值类型的值赋给可为空的值类型
            """;
        public static void ShowNullableValueType() {
            Type nullableHelperType = typeof(Nullable);
            Type nullableType = typeof(Nullable<int>);
            int? a = null, b = 3, c = null;
            int i = 5;
            Console.WriteLine(a + b);  // -> null
            Console.WriteLine(a + i);  // -> null
            Console.WriteLine(i + b);  // -> 8

            Console.WriteLine(a < i);  // -> false
            Console.WriteLine(a >= i);  // -> false
            Console.WriteLine(a >= c);  // -> false
            Console.WriteLine(a == c);  // -> true
            Console.WriteLine(b < i);  // -> true

            i = (int)b; // -> i = 3
            i = (int)a; // -> 报错
        }
        public static string 空条件运算符 = """
            a?.b 以安全的使用一个实例a的成员b, 当此实例为空时, 不会调用此成员, 而且返回值为空(若原来为值类型, 则会变为可为空的值类型)
                也叫做null传播
            a?[i] a为空时返回空, 否则按正常取索引处理(超界还是会报错)
            a ?? b 为当a为空时选用b, a与b一般得是相同的类型, 或a是可为空的值类型, b为此值类型(此时返回值为值类型而不为空)
                ?? 后也可以直接用throw语句, 代表若前者为空就直接报错, 相当于在其后加!
            a ??= b 当a为空时将b赋值给它
            """;
        public static class ShowNullOperator {
            public static void Show() {
                AClass? a = null;
                var intValue = a?.intValue; //intValue = (int?)null
                var stringValue = a?.stringValue; //stringValue = (string)null
                int i = 1;
                a?.SetIntValue(i = 5); //什么都不会发生, 赋值语句也不会被执行

                int? ni = null, nj = 6, nk = null;
                Console.WriteLine(ni ?? nj); // ->6
                Console.WriteLine(ni ?? nk); // ->null
                i = ni ?? 0; // ni为空则将0赋给i, 这样就不用强制类型转换了
                ni ??= nj; //ni为空时将nj赋给它
            }
            public class AClass {
                public int intValue;
                public string? stringValue;
                public int SetIntValue(int value) {
                    return intValue = value;
                }
            }
        }
        public static string 可为空的引用类型 = """
            启用可为空的引用类型:
                单文件内可以使用 #nullable enable 以启用, disable以禁用, restore以还原默认设置
                项目内可以设置项目文件中Project.PropertyGroup.Nullable为enable以启用
                或者:项目设置->生成->常规->可为null的类型
            启用时可以在引用类型后加?代表它是可以为空的, 而没加则代表此引用类型不会为空
            这两种类型本质上没有区别, 只是可能有空引用报错的地方都会标注出来
            如果想要一个变量不被标注(你确保它非空, 但是程序上识别不出来)可以在它后面加!标注
            """;
        public static void ShowNullableReferencesType() {
            string? a = null, a2 = null, a3 = null, a4 = null;
            string b = null;    //被警告了
            Show(b.Length);     //虽说这里b本来是string, 不会为空的类型, 但强行赋为了空, 所以还是会给出警告
            void Deleg(ref string? s) => s = "123";
            Deleg(ref a);
            Show(a.Length); //此时a已经非空, 但程序难以检查, 仍然会给警告
            Show(a.Length); //此时前面已经给出警告了, 若在那里爆了空引用报错, 就不会执行到这里, 所以程序认为这里a肯定非空(...)
            Deleg(ref a2);
            Show(a2!.Length); //在a后加!会禁用其空引用的检查, 让编译器认为它在这里总是非空的
            void ShowString(string s) => Do(s);
            Deleg(ref a3);
            ShowString(a3);
            Deleg(ref a4);
            ShowString(a4!);
        }

#nullable restore
    }
    public class 原始字符串 {
        public static string intro = """
            (原来其实也可以在字符串前用@来达到相似的效果, 但并不完善)
            原始字符串的特征为以3个双引号开始, 以3个双引号结束
            原始字符串中可以任意换行, 且开始和结束时的换行将被忽略
            原始字符串没有转义字符, 如\t, \n都会照原样输出, 
                它们写成普通字符串的形式应该会是"\\t, \\n"
            同时如上所见单引号和双引号都可以直接打
            而且当结尾的三引号不与开始的处于同一排时
                原始字符串的所有字符都应处于结尾三引号起始位置的右侧
                即与它同缩减的所有空格会被忽略
                (违反此规则是会报错的)
            如果想在字符串中使用连续三个或更多的双引号,
                只需在原始字符串字面上的开始和结束处使用更多双引号包裹即可
            """;
        public static string 启用原始字符串 = """
            如果还没有启用这个特性(于C# 11(.Net 7)时添加)可能要在项目文件(.csproj)中
                将Project.PropertyGroup.LangVersion设置为latest
            """;
        public static string 原始字符串的插值语法 = $$"""
            在原始字符串前使用$来使用插值语法
            $的个数代表所需要的{}数: {{"qqq"}}
            """;
    }
    public class 传参用特性 {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression">
        /// 使用CallerArgumentExpression以获得对应参数被调用时传入的表达式字符串
        /// </param>
        /// <param name="callerLine">使用CallerLineNumber以获得调用时的行数</param>
        /// <param name="callerMember">使用CallerMemberName以获得调用者的成员名字</param>
        /// <param name="callerFile">
        /// 使用CallerFilePath以获得调用者所在的文件的全路径(编译时)
        /// </param>
        public static void PrintParExpression(int value,
            [CallerArgumentExpression(nameof(value))]string expression = null,
            [CallerLineNumber]int callerLine = 0,
            [CallerMemberName]string callerMember = null,
            [CallerFilePath]string callerFile = null) {
            Console.WriteLine($"{expression} = {value}");
        }
    }
    #endregion
    private static void Shows(params object[] objs) {
        Do(objs);
    }
    private static void Show( object obj) {
        Do(obj);
    }
    private static void Show<T>(params object[] objs) {
        Do(objs);
    }
}
#pragma warning restore CA2211 // 非常量字段应当不可见
#pragma warning restore CS0219 // 变量已被赋值, 但从未使用过它的值
#pragma warning restore CS0649 // 从未对字段赋值, 字段将一直保持其默认值 null
#pragma warning restore IDE0059 // 不需要赋值
#pragma warning restore IDE0060 // 删除未使用的参数
