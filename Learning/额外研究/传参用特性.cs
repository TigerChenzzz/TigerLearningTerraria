using System.Runtime.CompilerServices;

namespace TigerLearning.Learning.额外研究;

public class 传参用特性 {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression">
    /// 使用CallerArgumentExpression以获得对应参数被调用时传入的表达式字符串
    /// </param>
    /// <param name="callerLine">使用CallerLineNumber以获得调用时的行数</param>
    /// <param name="callerMember">使用CallerMemberName以获得调用者的成员名字</param>
    /// <param name="callerFile">
    /// 使用CallerFilePath以获得调用者所在的文件的全路径(编译时)
    /// </param>
    public static void PrintParExpression(int value,
        [CallerArgumentExpression(nameof(value))] string expression = null,
        [CallerLineNumber] int callerLine = 0,
        [CallerMemberName] string callerMember = null,
        [CallerFilePath] string callerFile = null) {
        Console.WriteLine($"{expression} = {value}");
    }
}