namespace TigerLearning.Learning;

public class 吟唱武器 {
    public const string 介绍 = """
        item.channel这个属性会让玩家在使用物品后进入channel状态, 可以用player.channel来检测
        """;
    public class ExampleChannelWeapon : ModItem {
        public const string 参考 = "魔法导弹类武器 https://fs49.org/2022/01/22/%e9%ad%94%e6%b3%95%e5%af%bc%e5%bc%b9%e7%b1%bb%e6%ad%a6%e5%99%a8/";
        public override void SetDefaults() {
            //...
            Item.channel = true;
            //...
        }
    }
    public class ExampleChannelProjectile : ModProjectile {
        public const string 参考 = "魔法导弹类武器 https://fs49.org/2022/01/22/%e9%ad%94%e6%b3%95%e5%af%bc%e5%bc%b9%e7%b1%bb%e6%ad%a6%e5%99%a8/";
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