using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace TigerLearning.Documents;
public partial class Document {
    public class ChatManager_static_cls {
        public const string intro = """
            可以用来绘制带有小图标(snippet)的字符串
            绘制Tooltip用的就是用的这个
            """;
        public static void ShowChatManager() {
            #region params
#pragma warning disable IDE0018 // 内联变量声明
            SpriteBatch spriteBatch = default;
            DynamicSpriteFont font = default;
            Vector2 position = default, origin = default, scaleV2 = Vector2.One;
            string text = "text";
            Color color = default, shadowColor = default;
            float rotation = default, maxWidth = -1, spread = 2;
            bool ignoreColors = false;
            int hoveredSnippet = default;
#pragma warning restore IDE0018 // 内联变量声明
            #endregion
            string ChatManagerParseMessage的转换规则 = """
                "[i:<ItemID>]"为对应ItemID的物品图标(适用原版)
                "[i:<ModName>/<ItemName>]"为对应模组物品的图标
                "[c/<Color>:<Text>]"为特定颜色的文本, Color为16进制6位数, 如00FF00代表绿色, <Text>为待显示的文本
                ...
                """;
            var snippets = ChatManager.ParseMessage(text, color).ToArray();
            string DrawColorCodedStringWithShadowIntro = $"""
                这五个都相当于调用带有snippets参数的{nameof(ChatManager.DrawColorCodedStringShadow)}和{nameof(ChatManager.DrawColorCodedString)}
                maxWidth默认-1, spread默认2, 这5个都有这俩默认参数
                maxWidth: 最大宽度, -1代表不限
                spread: 阴影的扩散程度
                带有text参数的相当于是使用{nameof(ChatManager.ParseMessage)}后再调用带有snippets参数的版本
                color表示字符串的颜色, 若没有则是{Color.White}, shadowColor表示阴影的颜色, 若没有则是{Color.Black}
                """;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text    , position, color, rotation,              origin, scaleV2, maxWidth, spread);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text    , position, color, shadowColor, rotation, origin, scaleV2);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, snippets, position, rotation,                     origin, scaleV2, out hoveredSnippet);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, snippets, position, rotation, color,              origin, scaleV2, out hoveredSnippet);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, snippets, position, rotation, color, shadowColor, origin, scaleV2, out hoveredSnippet);

            string DrawColorCodedStringIntro = $"""
                maxWidth默认-1, ignoreColors默认false(带有snippets参数的那个maxWidth没有默认值, 需显式指定)
                带有text参数的不会调用{nameof(ChatManager.ParseMessage)}, 而是直接将字符串显示出来
                ignoreColors表示是否忽略由字符串指定的颜色(似乎是使用":sss1", ":sss2", ":sssr"指定红蓝白, TBT)
                """;
            ChatManager.DrawColorCodedString(spriteBatch, font, text, position, color, rotation, origin, scaleV2, maxWidth, ignoreColors);
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, color, rotation, origin, scaleV2, out hoveredSnippet, maxWidth, ignoreColors);

            string DrawColorCodedStringShadowIntro = $"""
                maxWidth默认-1, spread默认2
                带有text参数的不会调用{nameof(ChatManager.ParseMessage)}, 而是直接将字符串显示出来
                """;
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, color, rotation, origin, scaleV2, maxWidth, spread);
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, color, rotation, origin, scaleV2, maxWidth, spread);

            //获取画Snippets所需的大小(若传入text则是Parse后再算), maxWidth默认-1
            ChatManager.GetStringSize(font, text, scaleV2, maxWidth);
            ChatManager.GetStringSize(font, snippets, scaleV2, maxWidth);
        }
    }
}
