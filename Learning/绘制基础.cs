using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections;
using System.Linq;
using Terraria.GameContent;
using TigerLearning.Documents;

namespace TigerLearning.Learning;

public class 绘制基础 {
    public const string 参考 = "简单绘制 https://fs49.org/2021/12/22/%e7%ae%80%e5%8d%95%e7%bb%98%e5%88%b6/";
    public const string 世界坐标与屏幕坐标 = """
        世界坐标是从整个世界的左上角算起的, 玩家和npc的位置都是基于世界坐标的
        但是屏幕坐标是基于屏幕左上角的
        那么绘制的时候, 我们需要把世界坐标转换成为屏幕坐标
        我们需要把世界坐标转换成屏幕坐标, 那么我们会用到Main.screenPosition(屏幕在世界的坐标)
        那么世界坐标转屏幕坐标就只用减去 Main.screenPosition就好了
        """;
    public const string 绘制流程 = """
        一定不要在没有SpriteBatch为参数的重写函数里写绘制函数!!!(初学者)
        (其他地方可以通过Main.spriteBatch获取)
        """;
    public static void 获得贴图() {
        #region params
        string texturePath = "ExampleMod/Images/SomeItem";
        Texture texture = default;
        Mod mod = null;
        int itemID = 0;
        #endregion
        //texturePath以mod主类名为开始, 以 '/' 作为分割, 以图片名结尾, 不包含图片文件后缀(.png)
        //但这种方法在图片还没被加载时可能获得不到
        Show(ModContent.Request<Texture2D>(texturePath).Value);
        Main.GetItemDrawFrame(itemID, out var itemTexture, out Rectangle itemFrame);    //稳定的方法, 但获取的图片有范围限制, 获取texture的方式与下面缩进的语句相同
        Main.instance.LoadItem(itemID);
        if(TextureAssets.Item[itemID].State == AssetState.NotLoaded) {
            Main.Assets.Request<Texture2D>(TextureAssets.Item[itemID].Name);
        }
        itemTexture = TextureAssets.Item[itemID].Value;
        ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad);    //待测试
        //获取帧图, 参数分别为: 总共的{横/纵}向帧数, 当前处于{横/纵}向第几帧(从0开始), {横/纵}向偏移
        ModContent.Request<Texture2D>(texturePath).Frame(horizontalFrames: 1, verticalFrames: 1, frameX: 0, frameY: 0, sizeOffsetX: 0, sizeOffsetY: 0);
    }
    public static void DrawInSpriteBatch() {
        string 参考 = "对数螺线: 帧图绘制与用绘制整出的花活 https://fs49.org/2021/12/26/%e5%af%b9%e6%95%b0%e8%9e%ba%e7%ba%bf-%e5%b8%a7%e5%9b%be%e7%bb%98%e5%88%b6%e4%b8%8e%e7%94%a8%e7%bb%98%e5%88%b6%e6%95%b4%e5%87%ba%e7%9a%84%e8%8a%b1%e6%b4%bb/";
        #region params
        SpriteBatch spriteBatch = default;
        Texture2D texture = default;
        string text = "text";
        Rectangle sourceRectangle = default, destinationRectangle = default;
        Vector2 position = default, origin = default;
        Color color = default, textColor = default, borderColor = default;
        float rotation = default, scale = 1f;
        DynamicSpriteFont font = default;
        #endregion
        #region 绘制图片
        //以下position, rectangle等坐标除非特殊说明皆基于屏幕坐标
        //
        /*
        texuture 是Draw出来的图片
        position 屏幕坐标。图像要被绘制的位置
        sourceRectangle 代表需要绘制被绘制贴图的哪一部分, 如果全部绘制就是 null, 一般情况下是 null，绘制帧图的时候才需要设置其他值
        color 要把绘制图片染成什么颜色，一般来说 Color.White（不染色）就行了
        rotation 图像的旋转弧度
        origin, 大概就是图像从哪个位置开始扩散, 从中间开始扩散就填 texuture.Size() / 2f, 左上角就填 Vector2.Zero
            图片的绘制中心, destinationRectangle 参数的x,y 或 position就是这个点在屏幕中绘制出来的实际位置
            旋转为基于此点, 翻转貌似也是(待测试)
        scale 图像缩放倍数
        SpriteEffects effects 图像的翻转，可以水平翻转也可以垂直翻转, 若都翻转可以用SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
        最后一个填0，不用管那么多
        重载:
        scale 图像缩放倍数, 可以为float或Vector2
        position 可替换为destinationRectangle, 然后就不用scale了
            destinationRectangle: 目标矩形, 会自动拉伸图片, 会在设置目标矩形后再进行旋转和翻转(待测试)
        color以后的可以省略
        在color以后的参数省略的情况下sourceRectangle也可以省略
        */
        spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, 0);
        #endregion
        #region 绘制文字(绘制字符串)
        //文字绘制的函数建议使用 Terraria.Utils.DrawBorderStringFourWay(...) 而不是 spriteBatch.DrawString(...)
        /*
        font常用Main.fontMouseText(原版鼠标字体)
        */
        Utils.DrawBorderStringFourWay(spriteBatch, font, text, position.X, position.Y, textColor, borderColor, origin, scale);
        /*
        font可以为DynamicSpriteFont或SpriteFont
            DynamicSpriteFont可用FontAssets.ItemStack.Value等
        text可以为string或StringBuilder
        scale可以为float或Vector2
        以上三条加上{可选的rotation等参数}凑出了12个重载
        */
        spriteBatch.DrawString(font, text, position, color);
        spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.None, 0);

        //绘制带有图标的字符串
        Show(typeof(Document.ChatManager_static_cls));
        #endregion

    }
    public class 拖尾弹幕 : ModProjectile {
        public const string 参考 = "TeddyTerri：使用绘制来实现影子拖尾 https://fs49.org/2022/03/28/teddyterri%ef%bc%9a%e4%bd%bf%e7%94%a8%e7%bb%98%e5%88%b6%e6%9d%a5%e5%ae%9e%e7%8e%b0%e5%bd%b1%e5%ad%90%e6%8b%96%e5%b0%be/";
        //此处为自定义储存方式, 同时还应储存旋转, spriteDirection等信息, 但此处没写, 因为只是演示, 也有官方的处理方法
        public Vector2[] oldPositions = new Vector2[15];    //顺便写下仿队列
        public int endIndex;
        public bool full;

        private Texture2D texture;
        public override void SetDefaults() {
            //...
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            /*
            0: 仅记录坐标于projectile.oldPos数组(1帧一次), 其中0号为最新的一次记录, 也就是说每次记录都会将其他的项后移
            1: 在弹幕存在帧图时仅在framecounter == 0下记录一次
            2: 除了记录了坐标，还记录了rotation与spriteDirection(于oldRot和oldSpriteDirection)
            3: 包括2, 但会强制改rotation为上一帧到此帧的方向
            4: 包括2, 但会每帧将坐标额外加上玩家的位移(适用于天顶剑等轨迹跟随玩家(一般为在屏幕内不变)的弹幕)
            */
            ProjectileID.Sets.TrailingMode[Type] = 0;
            texture = ModContent.Request<Texture2D>("ExampleMod/Content/Projectiles/SomeProjectile").Value;
            //...
        }

        public override void AI() {
            //...

            //自定义储存方式, 每隔一段时间记录旧位置
            if((int)Main.time % 2 == 0) {
                oldPositions[endIndex] = Projectile.position;
                if(++endIndex >= oldPositions.Length) {
                    endIndex -= oldPositions.Length;
                    full = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)  //把残影画在弹幕后面就要用PreDraw
        {
            IEnumerable indexes = full ? Range(endIndex, oldPositions.Length).Concat(Range(endIndex)) : Range(endIndex);
            int transStep = oldPositions.Length - (full ? oldPositions.Length : endIndex);
            foreach(int index in indexes) {
                float transparency = ++transStep / (oldPositions.Length + 1f);  //让第一个和最后一个都不是1f或0f
                Main.spriteBatch.Draw(texture, oldPositions[index], lightColor * transparency);    //当然理应还要加上旋转和{帧图(如果有)}
            }
            return true;
        }
    }
    public class ExampleDrawPostDrawTileSystem : ModSystem {
        private Texture2D _dummyTexture;
        public Texture2D DummyTexture {
            get {
                if (_dummyTexture == null) {
                    _dummyTexture = new(Main.spriteBatch.GraphicsDevice, 1, 1);
                    _dummyTexture.SetData(new Color[] { Color.White });
                }
                return _dummyTexture;
            }
            set => _dummyTexture = value;
        }
        private BlendState[] _blendStates;
        public BlendState[] BlendStates {
            get {
                if(_blendStates == null) {
                    _blendStates = [
                        new() { ColorBlendFunction = BlendFunction.Add },
                        new() { ColorBlendFunction = BlendFunction.Subtract },
                        new() { ColorBlendFunction = BlendFunction.ReverseSubtract },
                        new() { ColorBlendFunction = BlendFunction.Max },
                        new() { ColorBlendFunction = BlendFunction.Min },
                    ];
                    for (int i = 0; i < 5; ++i) {
                        var b = _blendStates[i];
                        b.ColorDestinationBlend = Blend.SourceAlpha;
                        b.ColorSourceBlend = Blend.SourceAlpha;
                        b.AlphaBlendFunction = b.ColorBlendFunction;
                        b.AlphaDestinationBlend = b.ColorDestinationBlend;
                        b.AlphaSourceBlend = b.ColorSourceBlend;
                    }
                }
                return _blendStates;
            }
        }
        public override void PostDrawTiles() {
            var spriteBatch = Main.spriteBatch;
            Rectangle rect = new(Main.mouseX - 9, Main.mouseY - 9, 19, 19);
            Color color = new(1, .2f, .2f);
            for (int i = 0; i < BlendStates.Length; ++i) {
                for (int j = 0; j <= 5; ++j) {
                    color.A = (byte)(255 * j * 0.2f);
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendStates[i]);
                    spriteBatch.Draw(DummyTexture, rect, color);
                    spriteBatch.End();
                    rect.Y += 20;
                }
                rect.Y -= 20 * 6;
                rect.X += 20;
            }
        }
    }
}