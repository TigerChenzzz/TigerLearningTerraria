namespace TigerLearning.Documents;

public partial class Document {
    public class Console_static_cls {
        public const string intro = "用于控制台输入输出";
        public static void ShowConsole() {
            Console.WriteLine("Hello World!");
            Console.WriteLine("count {0}, and count {1}", 1, 0.0M);
        }
    }
}
