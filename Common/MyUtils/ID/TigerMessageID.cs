namespace TigerLearning.Common.MyUtils.ID;

public static class TigerMessageID {
    public const int None = 0;
    public const int HealNPC = 1;   //npcWhoAmI and int amount
    public const int PreHealNPCWhenZeroDamage = 2;  //only npcWhoAmI: heal by one ignore lifeMax
    public const int HealPlayer = 3;    //playerWai and int amount
    public const int HealPlayerMana = 4;    //playerWai and int amount
    public const int ProjectileHitPlayer = 5;   //projectileWai and playerWai,  handle penetrate-out-to-kill logic
    public const int SetNPCUnactive = 6;    //npcWai
}
