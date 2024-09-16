using Terraria.ModLoader.Config;
using TigerLearning.Documents;

namespace TigerLearning.Learning;

public class 添加模组配置 {
    public static ModConfig modConfig;  //当然大部分情况下你需要继承这个而不是直接用这个
    public static void ShowModConfig() {
        Show(typeof(ConfigManager));    //这里基本上管理着ModConfig的各种功能(如存储)
    }
    public class ExampleModConfig : ModConfig {
        /// <summary>
        /// 是客户端配置还是服务端配置
        /// </summary>
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public int SomethingYouWantToConfig;
    }
    //详见此处
    public static Document.ModConfig_cls DetailedInfo;
}