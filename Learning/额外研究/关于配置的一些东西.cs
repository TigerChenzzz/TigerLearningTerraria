using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace TigerLearning.Learning.额外研究;

public class 关于配置的一些东西 {
	#region 关于 Headers
	public const string HeaderIntro = $"""
		使用 {nameof(HeaderAttribute)} 可以让一个属性前有一个标头

		在 {nameof(UIModConfig.OnActivate)} 中会遍历合适的字段和属性
		在 {nameof(UIModConfig.HandleHeader)} 会获取配置中一个字段或属性的
			{nameof(HeaderAttribute)} 并提取它的 {nameof(HeaderAttribute.Header)}
		在 {nameof(UIModConfig.WrapIt)} 被处理为 {nameof(HeaderElement)}
		""";
	#endregion
}
