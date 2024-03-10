using Terraria.Localization;

namespace TigerLearning.Documents;
public partial class Document {
    public class Language_cls {
        public const string getTextValue = """
            Language.GetTextValue(key)以获得特定文本, 在不同语言时会自动使用不同翻译
            传入的key为.hjson文件中对应的项(用'.'连接), .hjson文件应位于Localization文件夹下, 英文为en-US.hjson, 中文为zh-Hans.hjson
            在[Label(str)], [Tooltip(str)], [Header(str)]等地方可以直接传入第一个字符为$的字符串以获得对应文本(是""$...""不是$""..."")
            """;
        public static void ShowLanguage() {
            Show(Language.ActiveCulture);       //游戏的语言
            Show(Language.GetTextValue("Mods.TigerTestMod.[...]"));
        }
    }
}
