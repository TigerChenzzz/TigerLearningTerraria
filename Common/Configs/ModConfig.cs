using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TigerLearning.Common.Configs;

//自动适配Localization中的Mods.TigerLearning.Configs.ClientConfig.DisplayName
[BackgroundColor(153, 61, 97, 200)]
public class ClientConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;//客户端

    //自动适配Localization中的Mods.TigerLearning.Configs.ClientConfig.Debug.{Label / Tooltip}
    [BackgroundColor(178, 53, 103)]
    [DefaultValue(false)]
    public bool Debug;

    [BackgroundColor(178, 53, 103)]
    public List<string> DebugTags = new();

    public override void OnChanged() {

    }

    public override void OnLoaded() {
        Config = this;
    }
}