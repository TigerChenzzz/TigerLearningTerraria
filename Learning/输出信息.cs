namespace TigerLearning.Learning;

public class 输出信息 {
    public static void ShowOutputInfo() {
        //在屏幕左下角输出
        Main.NewText("info here");  //可以额外传入Color或r, g, b作为颜色
        Main.NewTextMultiline("multilineInfo\nmultilineInfo line 2");   //一次输出多行
    }
}
