namespace TigerLearning.Learning;

public class 添加坐骑 {
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