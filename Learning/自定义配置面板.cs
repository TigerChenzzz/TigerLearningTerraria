using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace TigerLearning.Learning;

public class 自定义配置面板 {
    public static string intro = """
        写一个继承自ConfigElement的类
        然后在想自定义的配置字段或属性上添加特性
        [CustomModConfigItem(typeof(类名))]即可使用
        """;
    public class ExampleCustomConfigElement : ConfigElement {
        //也可以继承ConfigElement<T>, T代表具体要修饰哪种对象
        //这样可以由Value直接获得或设置对象
        protected Color backgroundColor;
        public override void OnBind() {
            #region 某些可用的属性和字段介绍
            Show(Item);                 //Item为包含此对象的东西, 通常为ModConfig
            Show(List);                 //若非空, 代表此对象实际被包含在一个集合内
            Show(List?[Index]);         //如此以获取或设置对应值
            SetObject(null);            //不过一般还是通过如下方式获取或设置对象
            Show(GetObject());
            Show(TooltipFunction);      //获得tooltip, 也可以自己写一个以替代
            Show(Label);                //标签
            Show(TextDisplayFunction);  //获得Label, 也可以自己写一个以替代
            Show(NullAllowed);          //是否允许为空(在ConfigElement中不做什么用), 若用[NullAllowed]标注则在base.OnBind()后会是true

            Show(MemberInfo.MemberInfo);//对应的member info(见反射)
            Show(MemberInfo.IsField);   //member info不是字段就是属性
            Show(MemberInfo.IsProperty);
            Show(RangeAttribute);           //各个特性
            Show(BackgroundColorAttribute);
            Show(IncrementAttribute);
            Show(JsonDefaultValueAttribute);
            Show(ConfigManager.GetCustomAttributeFromMemberThenMemberType<SliderColorAttribute>(MemberInfo, Item, List));   //获得自定义特性
            #endregion
            base.OnBind();

            // 这里可以设置此元素的画面大小, 不过只执行一次, 不是实时更新
            Height.Set(30f, 0f);
            DrawLabel = false;  //在原DrawSelf中是否画出文本, 默认true
            //ConfigElement的backgroundColor竟然是private的真是太可恶了
            //此处为它的默认值
            backgroundColor = BackgroundColorAttribute?.Color ?? UICommon.DefaultUIBlue;

            Append(new UIText(Label, large: true) {
                TextOriginX = 0f,
                TextOriginY = .5f,
                Width = StyleDimension.Fill,
                Height = StyleDimension.Fill,
                PaddingLeft = 4f,
                IsWrapped = true,
            });
            //也可以添加一些其他东西, 如UI
        }
        protected override void DrawSelf(SpriteBatch spriteBatch) {

            void BaseDrawSelf(SpriteBatch spriteBatch) {
                CalculatedStyle dimensions = GetDimensions();
                float width = dimensions.Width + 1f;
                Vector2 position = new(dimensions.X, dimensions.Y);

                //画面板
                DrawPanel2(spriteBatch, position, TextureAssets.SettingsPanel.Value, width, dimensions.Height,
                    IsMouseHovering ? backgroundColor : backgroundColor.MultiplyRGBA(new(180, 180, 180)));
                //画标签
                if(DrawLabel) {
                    position.X += 8f;
                    position.Y += 8f;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, TextDisplayFunction(), position,
                        MemberInfo.CanWrite ? Color.White : Color.Gray, 0f, Vector2.Zero, new Vector2(0.8f), width);
                }
                //画tooltip
                if(IsMouseHovering && TooltipFunction != null) {
                    //UIModConfig.Tooltip = TooltipFunction();
                    TMLReflection.UIModConfig.Tooltip = TooltipFunction();
                    //或者可以直接用Main.instance.MouseText(...);
                }
            }
            BaseDrawSelf(spriteBatch);
            //base.DrawSelf(spriteBatch);
        }
    }
}