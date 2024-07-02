using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace TigerLearning.Learning;

public class 键位绑定
{
    // 用此类以定义一个键位, 这样就可以在游戏中设置了
    public static ModKeybind ExampleKeybind;
    public static void 注册键位()
    {
        #region params
        Mod mod = default;
        string name = default;
        Keys key = default;
        #endregion
        // 在加载阶段使用, 如Mod.Load 或 ModSystem.OnModLoad 中
        ExampleKeybind = KeybindLoader.RegisterKeybind(mod, name, key);
        // 本地化键值为 Mods.[ModName].Keybinds.[keybind name].DisplayName
    }

    public static void 使用键位()
    {
        // 以下实际上都是使用 PlayerInput.Triggers 作检查
        // 一般需要在 ModPlayer.ProcessTriggers 中使用
        Show(ExampleKeybind.Current);       // 检测是否按住
        Show(ExampleKeybind.JustPressed);   // 是否刚刚按下
        Show(ExampleKeybind.JustReleased);  // 是否刚刚抬起
        Show(ExampleKeybind.Old);           // 上一帧是按住
    }
}
