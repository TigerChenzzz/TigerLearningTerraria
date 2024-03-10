using Terraria.Localization;

namespace TigerLearning.Learning;

public class 添加饰品 {
    public class ExampleAccessories : ModItem {
        public const string 参考 = "自定义饰品和翅膀 https://fs49.org/2020/03/11/%e8%87%aa%e5%ae%9a%e4%b9%89%e9%a5%b0%e5%93%81%e5%92%8c%e7%bf%85%e8%86%80/";
        public const string 说明 = "需额外准备与类名同名且在对应命名空间下的图片(.png)文件";

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
        public const string 参考 = "自定义饰品和翅膀 https://fs49.org/2020/03/11/%e8%87%aa%e5%ae%9a%e4%b9%89%e9%a5%b0%e5%93%81%e5%92%8c%e7%bf%85%e8%86%80/";
        public const string 说明 = "需额外准备名字为 [类名]_Wings 且在对应命名空间下的图片(.png)文件(四帧)";
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
    public class 信息饰品 {
        public const string intro = """
            如手机等可以在右侧地图下面展示一些信息的饰品, 也是可以自己造的
            """;
        public static InfoDisplay infoDisplay;
        public static void ShowInfoDisplay() {
            #region params
            Player player = default;
            int infoDisplayType = default;
            #endregion
            Show(infoDisplay.Type);     //信息展示内置ID
            Show(player.hideInfo[infoDisplayType]); //此信息展示是否被隐藏(一般在背包中地图下有一排小图标调的就是这个)
        }
        #region 自制一个信息饰品的简单示例
        public class ExampleInfoDisplayer : ModItem {
            public override void SetDefaults() {
                Item.width = Item.height = 32;
                Item.rare = ItemRarityID.Green;
                Item.value = Item.sellPrice(0, 1, 0, 0);
                Item.accessory = true;  //似乎不是必要的(TBT)
            }
            public override void UpdateInfoAccessory(Player player) {
                player.GetModPlayer<ExampleInfoPlayer>().display = true;
            }
        }
        public class ExampleInfoPlayer : ModPlayer {
            public bool display;
            public override void ResetInfoAccessories() {
                display = false;
            }
            public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
                var otherInfoPlayer = otherPlayer.GetModPlayer<ExampleInfoPlayer>();
                if(otherInfoPlayer.display) {
                    display = true;
                }
            }
        }
        public class ExampleInfoDisplay : InfoDisplay {
            public const string tip = """
                需要在同目录下准备"<类名>.png"(比如此处就是"ExampleInfoDisplay.png")表示其小图标(14x14?)
                """;
            public const string localization = $"""
                需要在本地化文件中设置Mods.[Mod类名].InfoDisplays.[此类名].DisplayName的值
                或者重写{nameof(DisplayName)}以设置获取显示名的获取
                """;
            public override bool Active() => Main.LocalPlayer.GetModPlayer<ExampleInfoPlayer>().display;
            /// <summary>
            /// 在地图下方显示的值<br/>
            /// 如果为空或者空字符串会有一些问题(会连带图标一起不显示), 尽量保证它长度不为0
            /// </summary>
            public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) => "Value";
            /// <summary>
            /// <br/>当鼠标放在小图标上时显示的字符串
            /// <br/>默认是Mods.[Mod类名].InfoDisplays.[此类名].DisplayName
            /// <br/>一般不需要重写, 除非想要在鼠标放上去时显示更多的信息,
            /// <br/>此时可以用<see cref="LocalizedText.WithFormatArgs"/>来携带额外信息,
            /// <br/>同时可以判断一下<see cref="Main.playerInventory"/>(当玩家打开背包时一般是管理信息饰品是否显示的)
            /// <br/>实时更新
            /// </summary>
            public override LocalizedText DisplayName => base.DisplayName;
            /// <summary>
            /// <br/>小图标路径, 默认就在InfoDisplay对应路径
            /// <br/>基本不需要重写
            /// <br/>从Mod名开始的路径, 不加后缀(".png"), 例如: "ExampleMod/Content/Items/ExampleInfoDisplay"
            /// </summary>
            public override string Texture => base.Texture;
            /// <summary>
            /// <br/>当在背包中鼠标在小图标上时小图标上叠加显示的图片的路径
            /// <br/>基本不需要重写
            /// <br/>默认为"Terraria/Images/UI/InfoIcon_13"
            /// </summary>
            public override string HoverTexture => base.HoverTexture;
        }
        #endregion
    }
}