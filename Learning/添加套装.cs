namespace TigerLearning.Learning;

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