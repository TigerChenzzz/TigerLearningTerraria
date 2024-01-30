namespace TigerLearning.Learning.额外研究;

public class 非托管类型 {
    public static string intro = """
        如果某个类型是以下类型之一，则它是非托管类型 ：

            sbyte、byte、short、ushort、int、uint、long、ulong、nint、nuint、char、float、double、decimal 或 bool
            任何枚举类型
            任何指针类型
            任何仅包含非托管类型字段的用户定义的结构类型。
        引自https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        """;
    public static string unmanaged约束 = """
        可使用 unmanaged 约束指定：类型参数为“非指针、不可为 null 的非托管类型”
        引自https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        """;
}