namespace TigerLearning.Learning;

public class 源码下的基本文件 {
    public const string 必要文件 = """
        build.txt       : 如何构建此模组
        [Mod名].cs      : 相当于主类
        [Mod名].csproj  : 包含了哪些文件
        description.txt : 其实好像不是必须的, 但正式的模组总该有个描述文件
        icon.png        : 好像也不是必须的, 但最好有作为图标
        """;
    public class Build_txt {
        public const string 参考 = "Mod基本信息 https://fs49.org/2020/03/09/mod%e5%9f%ba%e6%9c%ac%e4%bf%a1%e6%81%af/";
        public const string 基本形式 = """
            [key1] = [value1]
            [key2] = [value2]
            ...
            """;
        public const string 支持的一些键 = """
            displayName    : Mod在TML里显示的名字(不是文件夹的名字哦, 可以是中文)
            author         : 作者的名字
            version        : Mod的版本, 会在Mod菜单中显示。注意, 这个属性是要求格式的。格式如下  <数字>.<数字>.<数字>.<数字>, 也就是我们最常见到的版本号的格式, 这里我取了1.0
            dllReferences  : 如果你的Mod要引用外部的dll, 把文件名写在这。文件名不要包含扩展名, 你必须将dll文件放在/lib文件夹中才能构建Mod的引用
            modReferences  : 你的mod依赖的Mod的列表。 同样不包含扩展名。(这里是强依赖
            weakReferences : 弱引用Mod列表, 用于联动Mod, 但是并不要求这个Mod被加载。但是弱引用内容也需要特殊的技巧去处理。
            noCompile      : 这个Mod的源码需不需要被编译, 默认是不用管的。(高级内容, 暂时可以无视)  默认FALSE
            homepage       : 这个Mod主页的链接, 如果有的话, 比如这里是教程官网
            hideCode       : 如果设为true, 你的源码以及编译好的dll就不会被TML抽取, 如果你不想被别人看到源码就设为true    默认FALSE
            hideResources  : 是否隐藏Mod中包含的资源文件, 比如贴图, 音乐文件等等, 不想被别人看见就设为true      默认FALSE
            includeSource  : 是否把.cs源码文件也放入tmod文件中, 这样别人可以看见你写的所有源代码, 不想允许就设为false(make sure to  set hideCode to false).     默认FALSE
            buildIgnore    : 将Mod源码编译成tmod文件的时候, 哪些文件(夹)是不需要放进tmod文件的, 这样能减小tmod文件的大小。includeSource为false的时候自动会忽略.cs文件   默认build.txt, .gitattributes, .gitignore, .git/, .vs/, .idea/, bin/, obj/,  Thumbs.db
            includePDB     : 需不需要包括符号调试文件, 如果包含可以更多提供debug信息, 同时也允许使用VS进行Debug   默认FALSE
            side           : 这个Mod是客户端Mod还是服务器端Mod     默认Both
            """;
    }
}
