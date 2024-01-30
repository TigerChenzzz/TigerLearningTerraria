using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.Default;

namespace TigerLearning.Learning;

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
        #region 给予物品前缀
        item.Prefix(0);     //什么都不做, 返回false
        item.Prefix(-1);    //随机前缀(箱子生成, 开始给予, 物品掉落等)
        item.Prefix(-2);    //哥布林重铸(不会重铸到没有前缀)
        item.Prefix(-3);    //只是检查是否可以拥有任意前缀, 不给予
        item.Prefix(PrefixID.Large);    //给予特定前缀
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
