namespace TigerLearning.Learning;

public class 杂项 {
    public static void ShowMiscs() {
        Show("任何武器有4%的基础暴击率(待测试)");
    }
    public static void 一些有用的东西() {
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
}
