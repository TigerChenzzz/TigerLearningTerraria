using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TigerLearning.Learning;

public class UI制作 {
    public class ExampleUI : UIState {
        public static string 参考 = "UI的制作 https://fs49.org/2020/03/27/ui%e7%9a%84%e5%88%b6%e4%bd%9c/";
        public bool visible;
        public override void OnInitialize() {
            #region 实例化一个面板，并且将其注册到UIState
            //实例化一个面板
            UIPanel panel = new();
            //设置面板的宽度
            panel.Width.Set(488f, 0f);
            //设置面板的高度
            panel.Height.Set(568f, 0f);
            //设置面板距离屏幕最左边的距离
            panel.Left.Set(-244f, 0.5f);
            //设置面板距离屏幕最上端的距离
            panel.Top.Set(-284f, 0.5f);
            //将这个面板注册到UIState
            //若需要将其他UIElement添加到panel上则在添加完毕后调用
            Append(panel);
            #endregion
            #region 往面板上面添加一个按钮
            //用tr原版图片实例化一个图片按钮
            UIImageButton button = new(ModContent.Request<Texture2D>("Terraria/UI/ButtonDelete"));
            //设置按钮距宽度
            button.Width.Set(22f, 0f);
            //设置按钮高度
            button.Height.Set(22f, 0f);
            //设置按钮距离所属ui部件的最左端的距离
            button.Left.Set(-11f, 0.5f);
            //设置按钮距离所属ui部件的最顶端的距离
            button.Top.Set(-11f, 0.5f);//注册一个事件，这个事件将会在按钮按下时被激活
            button.OnLeftClick += (evt, element) => visible = false;
            //将按钮注册入面板中，这个按钮的坐标将以面板的坐标为基础计算
            panel.Append(button);
            #endregion
            #region 添加自定义UI
            进度条 progressBar = new(() => Main.LocalPlayer.statLifeMax2, () => Main.LocalPlayer.statLife);
            progressBar.Width.Set(100f, 0f);
            progressBar.Height.Set(20f, 0f);
            progressBar.Left.Set(20f, 0f);
            progressBar.Top.Set(20f, 0f);
            progressBar.Border = 2f;
            panel.Append(progressBar);
            #endregion

            //更多类型:
            Show(typeof(UIImage));      //用于显示图片
            Show(typeof(UIText));       //用于展示文字
            Show(typeof(UITextBox));    //供玩家输入字符串
            Show(typeof(ItemSlot));     //能放入物品
            Show(typeof(UIList));       //用于显示一个列表
            Show(typeof(UIScrollbar));  //用于配合UIList以滚动列表条
        }
    }
    public class ExampleUISystem : ModSystem {
        public static string 参考 = ExampleUI.参考;
        public static ExampleUI exampleUI;
        public static UserInterface exampleUserInterface;     //UserInterface是用来托管UI事件的一个类
        public override void Load() {
            exampleUI = new();
            exampleUI.Activate();
            exampleUserInterface = new();
            exampleUserInterface.SetState(exampleUI);
        }
        public override void UpdateUI(GameTime gameTime) {
            if(exampleUI.visible) {
                exampleUserInterface?.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if(mouseTextIndex != -1) {
                LegacyGameInterfaceLayer legacyLayer = new("Example: ExampleUI", () =>
                {
                    if (exampleUI.visible)
                    {
                        exampleUI.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI);
                layers.Insert(mouseTextIndex, legacyLayer);
            }
        }
    }
    public class ExampleUIPlayer : ModPlayer {
        public static string 参考 = ExampleUI.参考;
        public override void OnEnterWorld() {
            ExampleUISystem.exampleUI.visible = true;
        }
    }

    public class 进度条 : UIElement {
        public static string 参考 = "UI部件示例：进度条 https://fs49.org/2020/03/31/ui%e9%83%a8%e4%bb%b6%e7%a4%ba%e4%be%8b%ef%bc%9a%e8%bf%9b%e5%ba%a6%e6%9d%a1/";
        private float _maxValue;
        private float _value;
        public float MaxValue {
            get => _maxValue;
            set {
                _maxValue = value;
                if(_maxValue < 0) {
                    _maxValue = 0;
                }
                if(_value > _maxValue) {
                    _value = _maxValue;
                }
            }
        }
        public float Value {
            get => _value;
            set {
                _value = value;
                if(_value < 0) {
                    _value = 0;
                }
                else if(_value > _maxValue) {
                    _value = _maxValue;
                }
            }
        }
        public float BorderX {
            get => PaddingLeft;
            set {
                value = value < 0 ? 0 : Math.Min(value, GetDimensions().Width / 2);
                PaddingLeft = PaddingRight = value;
            }
        }
        public float BorderY {
            get => PaddingTop;
            set {
                value = value < 0 ? 0 : Math.Min(value, GetDimensions().Height / 2);
                PaddingTop = PaddingBottom = value;
            }
        }
        public float Border {
            get => PaddingLeft;
            set {
                value = value < 0 ? 0 : Min(value, GetDimensions().Width / 2, GetDimensions().Height / 2);
                SetPadding(value);
            }
        }
        public Func<float> _setMaxValue;
        public Func<float> _setValue;
        public 进度条(float maxValue, float value) {
            MaxValue = maxValue;
            Value = value;
        }
        public 进度条(Func<float> setMaxValue, Func<float> setValue) {
            _setMaxValue = setMaxValue;
            _setValue = setValue;
            MaxValue = setMaxValue?.Invoke() ?? 0;
            Value = setValue?.Invoke() ?? 0;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch) {
            if(_setMaxValue != null) {
                MaxValue = _setMaxValue.Invoke();
            }
            if(_setValue != null) {
                Value = _setValue.Invoke();
            }
            Rectangle boxRect = GetDimensions().ToRectangle();
            Rectangle barRect = GetInnerDimensions().ToRectangle();
            barRect.Width = (int)(barRect.Width * (MaxValue == 0 ? 1 : Value / MaxValue));
            spriteBatch.Draw(ModContent.Request<Texture2D>("Example/Image/UI/box").Value, boxRect, Color.White);    //外边框
            spriteBatch.Draw(ModContent.Request<Texture2D>("Example/Image/UI/bar").Value, barRect, Color.White);    //条
        }
    }
    public 泰拉瑞亚On.动态icon 动态icon;
}