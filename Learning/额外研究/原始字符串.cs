﻿namespace TigerLearning.Learning.额外研究;

public class 原始字符串 {
    public static string intro = """
        (原来其实也可以在字符串前用@来达到相似的效果, 但并不完善)
        原始字符串的特征为以3个双引号开始, 以3个双引号结束
        原始字符串中可以任意换行, 且开始和结束时的换行将被忽略
        原始字符串没有转义字符, 如\t, \n都会照原样输出, 
            它们写成普通字符串的形式应该会是"\\t, \\n"
        同时如上所见单引号和双引号都可以直接打
        而且当结尾的三引号不与开始的处于同一排时
            原始字符串的所有字符都应处于结尾三引号起始位置的右侧
            即与它同缩减的所有空格会被忽略
            (违反此规则是会报错的)
        如果想在字符串中使用连续三个或更多的双引号,
            只需在原始字符串字面上的开始和结束处使用更多双引号包裹即可
        """;
    public static string 启用原始字符串 = """
        如果还没有启用这个特性(于C# 11(.Net 7)时添加)可能要在项目文件(.csproj)中
            将Project.PropertyGroup.LangVersion设置为latest
        """;
    public static string 原始字符串的插值语法 = $$"""
        在原始字符串前使用$来使用插值语法
        $的个数代表所需要的{}数: {{"qqq"}}
        """;
}