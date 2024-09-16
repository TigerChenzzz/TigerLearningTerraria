using System.IO;
using Terraria.Localization;

namespace TigerLearning.Learning.额外研究;

public class 本地化的一些东西 {
    public static Type[] 一些关键的类型 = [
        typeof(LocalizationLoader),
        typeof(LanguageManager),
    ];
    public const string FinishSetup = $"""
        在 {nameof(ModContent.Load)} 中, 会在将近最后时执行 {nameof(LocalizationLoader.FinishSetup)}
        此方法会分别执行 {nameof(LocalizationLoader.UpdateLocalizationFiles)} 和 {nameof(LocalizationLoader.SetupFileWatchers)}
        {nameof(LocalizationLoader.UpdateLocalizationFiles)}:
            会将本地化内容更新到 .hjson 文件中去
            如果一个模组有对别的模组的翻译, 那么也会将别的模组的本地化内容更新到此模组的 .hjson 文件中
        {nameof(LocalizationLoader.SetupFileWatchers)}:
            使用 {nameof(FileSystemWatcher)} 以监视 ModSource 文件夹下各个启用的模组中所有 .hjson 文件的变化
            当有 是本地化文件的 .hjson 文件 变化时会启用一个 60 帧计时器, 并暂存变化了的文件, 当此计时器正好归零时它会
            调用 {nameof(LanguageManager.ReloadLanguage)} 以进行更新 (此逻辑见 {nameof(LocalizationLoader.Update)})
        """;
}
