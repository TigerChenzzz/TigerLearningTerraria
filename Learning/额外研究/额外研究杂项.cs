namespace TigerLearning.Learning.额外研究;

public class 额外研究杂项 {
    public static void ShowMiscs() {
        global::System.Console.WriteLine(); //保证调用的是全局下的System.Console.WriteLine
        #region goto case
        int[] intArray = {3, 2, 1};
        switch(intArray.Length) {
        case > 3:
            goto case 3;//无法用goto case > 3 这样模式匹配的case
        case 3:
            Console.Write(intArray[2]);
            goto case 2;
        case 2:
            Console.Write(intArray[1]);
            goto case 1;
        case 1:
            Console.Write(intArray[0]);
            goto default;
        default:
            Console.WriteLine();
            break;
        }
        #endregion
    }
}
