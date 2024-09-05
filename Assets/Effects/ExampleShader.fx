// 示例 shader

// 寄存器 s0 保存的图片数据, 通过这种方式获得
sampler uImage0 : register(s0);

// 从外界传参, 这个参数需要在应用 shader 前使用
// effect.Parameters["ParameterName"].SetValue(...) 设置
float4 uColor;

float4 GrayScale(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage0, coords);
    // 灰度 = r * 0.3 + g * 0.59 + b * 0.11
    float gs = dot(float3(0.3, 0.59, 0.11), color.rgb);
    return float4(gs, gs, gs, color.a);
}

// 参数中标注了 COLOR0 的可以拿到绘制时传给 SpriteBatch.Draw(...) 的颜色
// 标注了 TEXCOORD0 的可以拿到当前的绘制坐标 (0 ~ 1)
float4 Origin(float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0 {
    return tex2D(uImage0, coords) * color;
}

technique Technique1 {
    pass GrayScale {
        PixelShader = compile ps_2_0 GrayScale();
    }
    pass Origin {
        PixelShader = compile ps_2_0 Origin();
    }
}