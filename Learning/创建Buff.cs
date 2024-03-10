namespace TigerLearning.Learning;

public class 创建Buff {
    public class ExampleBuff : ModBuff {
        public const string 参考 = "自定义套装和Buff https://fs49.org/2020/03/12/%e8%87%aa%e5%ae%9a%e4%b9%89%e5%a5%97%e8%a3%85%e5%92%8cbuff/";
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
                if(Main.LocalPlayer.buffType[i] != Type) continue;
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