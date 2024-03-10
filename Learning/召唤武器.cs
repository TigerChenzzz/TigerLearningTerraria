namespace TigerLearning.Learning;

public class 召唤武器 {
    public const string 介绍 = """
        召唤武器其实本质上就是物品带个智能一点的弹幕
        但是为了表示玩家的召唤物的状态, 我们还需要一个Buff以及一个自定义玩家属性来保证召唤物能正常工作以及消失
        """;
    public class ExampleMinionPlayer : ModPlayer {
        public const string 参考 = "召唤武器实战：僚机(1.4再版) https://fs49.org/2022/09/28/%e5%8f%ac%e5%94%a4%e6%ad%a6%e5%99%a8%e5%ae%9e%e6%88%98%ef%bc%9a%e5%83%9a%e6%9c%ba%ef%bc%881-4%e5%86%8d%e7%89%88%ef%bc%89/";
        public bool exampleMinion;
        public override void ResetEffects() {
            exampleMinion = false;
        }
    }
    public class ExampleMinionWeapon : ModItem {
        public const string 参考 = ExampleMinionPlayer.参考;
        public override void SetDefaults() {
            //...
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<ExampleMinionProj>();
            Item.shootSpeed = 10f;
            //...
        }
    }
    public class ExampleMinionProj : ModProjectile {
        public const string 参考 = ExampleMinionPlayer.参考;
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
        public const string 参考 = ExampleMinionPlayer.参考;
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