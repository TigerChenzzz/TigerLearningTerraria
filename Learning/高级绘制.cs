using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace TigerLearning.Learning;

public class 高级绘制 {
    public static class 制作Shader {
        public const string 参考 = "Shader简介 https://fs49.org/2020/04/09/shader%e7%ae%80%e4%bb%8b/";
        public const string 介绍 = $"""
            Shader 即 着色器
            泰拉瑞亚中的 {nameof(Effect)} 即 Shader, 使用 .fx 文件 (着色器脚本文件) 编写, 使用 HLSL 脚本
            然后需要使用专门的编译器编译为 .xnb 文件, 这样才能被读取到
            内置函数:       https://learn.microsoft.com/zh-cn/windows/win32/direct3dhlsl/dx-graphics-hlsl-intrinsic-functions
            内置函数(英文): https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-intrinsic-functions
            """;
    }

    public static class 使用Shader {
        public static void Show() {
            #region params
            Asset<Effect> effect;
            string path = "[Mod]/[Path]";
            #endregion
            // 通过以下方法加载一个 Shader
            effect = ModContent.Request<Effect>(path);  // path 以 '/' 分隔, 以 Mod 名开头, 不包括拓展名

            #region 在绘制方法中
            var spriteBatch = Main.spriteBatch;
            #region 重新开始画布
            spriteBatch.End();
            string sortModeIntro = $"""
            SpriteSortMode: 绘制顺序
            默认为 {nameof(SpriteSortMode.Deferred)}: 会把绘制信息缓存起来, 在刷新缓存时一次性绘制到屏幕上, 但没法应用 Shader
            需要 {nameof(SpriteSortMode.Immediate)} 以应用 Shader: 在调用 Draw 时立即画到屏幕上
            """;
            string blendStateIntro = $"""
                BlendState: 混合模式, 将图像绘制到背景上时的颜色混合方式
                令要绘制的图像颜色为 Ca, 透明度 alpha, 背景颜色 Cb, 那么混合得到的颜色为:
                默认 {nameof(BlendState.AlphaBlend)} 为: ??
                {nameof(BlendState.NonPremultiplied)} 为: ??
                {nameof(BlendState.Additive)} 为: Ca + Cb
                {nameof(BlendState.Opaque)} 为: Ca
                也可以自己定义一个 {nameof(BlendState)}
                """;
            string samplerStateIntro = $"""
                SamplerState: 采样方式
                主要分为 Point, Linear, Anisotropic, 越往右则抗锯齿能力越强, 但非 Point 的情况下小图放大会变糊
                以及 Clamp 和 Wrap, 指当采样点超出范围时是使用 Clamp 到范围内还是使用取模的方式限制到范围内
                """;
            string rasterizerStateIntro = $"""
                Rasterization: 光栅化
                默认为 {RasterizerState.CullCounterClockwise}, 即逆时针的三角形会被剔除掉
                还有 {RasterizerState.CullClockwise} 和 {RasterizerState.CullNone}, 代表剔除顺时针 或者 不剔除
                """;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            #region 获取 Technique
            EffectTechnique tech = effect.Value.Techniques["TechniqueName"];
            tech = effect.Value.Techniques[0];
            // CurrentTechnique 默认即 Techniques[0]
            tech = effect.Value.CurrentTechnique;
            #endregion
            #region 获取 Pass
            EffectPass pass = tech.Passes["PassName"];
            pass = tech.Passes[0];
            #endregion

            // 在应用之前可以给它传参
            effect.Value.Parameters["ParameterName"].SetValue(Color.Red.ToVector4());
            // 可以用 ?.SetValue 的方式选择性传参

            // 应用
            pass.Apply();

            // 然后就可以在这里 Draw 了

            // 设置回原来的参数
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
        }

        #region 全局 Shader
        // 注: 全局 Shader 需要特定的照明模式 (颜色或者白), 否则不会起效
        public class GlobalShaderExample : ModSystem {
            const string effectFilter = "TigerLearning:ExampleEffect";
            public override void Load() {
                var effect = ModContent.Request<Effect>("TigerLearning/Effects/ExampleEffect");
                Filters.Scene[effectFilter] = new(new ExampleScreenShaderData(effect, "ExamplePass"), EffectPriority.Medium);
                Filters.Scene[effectFilter].Load();
            }

            // 设置在某个条件下开启 Shader
            private static bool Condition => Main.IsItDay();

            // 在一处合适的 Update 中
            public override void PostUpdatePlayers() {
                if (Condition) {
                    // 开启 全局 Shader
                    if (!Filters.Scene[effectFilter].Active) {
                        Filters.Scene.Activate(effectFilter);
                    }
                }
                else {
                    // 关闭 全局 Shader
                    if (Filters.Scene[effectFilter].Active) {
                        Filters.Scene.Deactivate(effectFilter);
                    }
                }
            }
        }

        public class ExampleScreenShaderData : ScreenShaderData {
            public ExampleScreenShaderData(string passName) : base(passName) { }
            public ExampleScreenShaderData(Asset<Effect> shader, string passName) : base(shader, passName) { }
            public override void Apply() {
                // 在这里重写 Apply (传参等)
                base.Apply();
            }
        }

        public static void ShowGlobalShader(string filterName) {
            // 检查是否有启用的 Filter (即全局 Shader)
            Filters.Scene.HasActiveFilter();

            // 启用一个 Filter
            // Filter.IsActive() 等价 Filter.Active
            if (Filters.Scene[filterName].Active) {
                Filters.Scene.Activate(filterName);
                // 不要调用 Filters.Scene[filterName].Activate(), 这个不会调用 Filters.Scene.OnActivate(...), 会遗漏一些步骤
            }

            // 禁用一个 Filter
            if (Filters.Scene[filterName].Active) {
                Filters.Scene.Deactivate(filterName);
            }

            // 在全部绘制完成后执行, 需要 Capture 时才会执行
            Filters.Scene.OnPostDraw += () => { };
        }
        
        // 什么时候才会 Capture (Capture 时才会执行全局 Shader):
        public static string whenToCapture = $"""
            完整条件: (摘自 Main.DoUpdate 中 {nameof(Filters.Scene.BeginCapture)} 之前)
                {!Main.drawToScreen && Main.netMode != NetmodeID.Server &&
                    (!Main.gameMenu || Main.dontStarveWorld && WorldGen.generatingWorld || Filters.Scene["Sepia"].IsInUse()) // 这个在原文是 !flag
                     && !Main.mapFullscreen && Lighting.NotRetro && Filters.Scene.CanCapture()}
            大致说来需要 {Main.drawToScreen} 为假, {Lighting.NotRetro} 为真, 且需要 {Filters.Scene.CanCapture()}
            {Main.drawToScreen} 相当于 {Lighting.UpdateEveryFrame}, 即 {Main.LightingEveryFrame && !Main.RenderTargetsRequired && !Lighting.NotRetro}
                {Main.LightingEveryFrame} 一般保持为真, {Main.RenderTargetsRequired} 通常为假
            {Lighting.NotRetro} 则需要照明模式不是复古或迷幻 (而是颜色或者白)
                ({Lighting.Mode}不是 {LightMode.Retro} 或 {LightMode.Trippy}, 而是 {LightMode.Color} 或 {LightMode.White})
            {Filters.Scene.CanCapture()} 则需要有开启的 Filter ({Filters.Scene.HasActiveFilter()}) 或 {nameof(Filters.Scene.OnPostDraw)} 非空
            
            总的来讲需要在颜色或者白的照明模式下, 有开启的 Filter 或 OnPostDraw 时非空时才会进行 Capture
            
            此时在绘制时会将 RenderTarget 设置为 {Main.screenTarget}, 否则 RenderTarget 将是 {null}
            """;
        #endregion
    }

    public static class 使用RenderTarget {
        public static void Show() {
            SpriteBatch spriteBatch = Main.spriteBatch;

            // Main.graphics.GraphicsDevice 和 Main.instance.GraphicsDevice 通常是一个东西
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

            // 定义一个 RenderTarget2D
            RenderTarget2D target = new(graphicsDevice, Main.screenWidth, Main.screenHeight);
            // 注意 RenderTarget2D 实现了 IDisposable, 需要手动控制释放

            // 将绘制目标设置为它 (在此之前需要确保 SpriteBatch 已结束)
            graphicsDevice.SetRenderTarget(target);

            // 然后可以调用 spriteBatch.Draw 以绘制 (需要先 spriteBatch.Begin())

            string setRenderTargetIntro = $"""
                当设置 RenderTarget 为 {null} 时为直接绘制到屏幕上,
                当有全局滤镜时 ({nameof(Filter)}), 泰拉瑞亚会将绘制目标设置为 {Main.screenTarget}
                  进行完绘制后再让滤镜对 {Main.screenTarget} 和 {Main.screenTargetSwap} 进行操作, 最后再绘制到屏幕上
                  (参见 {nameof(Main.DoDraw)} 中 {nameof(Filters.Scene.BeginCapture)} 和 {nameof(Filters.Scene.EndCapture)} (第二个) 之间的片段,
                  全局滤镜则在)
                """;
            graphicsDevice.SetRenderTarget(null);
        }
    }
}
