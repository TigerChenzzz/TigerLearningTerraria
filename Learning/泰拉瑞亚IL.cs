using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Linq;
using System.Reflection;

namespace TigerLearning.Learning;

public class 泰拉瑞亚IL {
    public const string sharpLab = """
        https://sharplab.io/
        这里可以简单的查看一段代码的IL代码或者汇编代码,
        或者从IL代码反推源代码, 也可以直接运行一段代码
        """;
    public class 一个入门IL教程 {
        public const string 参考 = "https://celestemod.saplonily.link/trans/il/";//是的, 这是蔚蓝的modder写的(
        public const string 评估栈 = $$"""
        在 IL 代码中, 大量操作符本质上都是在操作一个叫做"评估栈"的东西
        又名 运算栈 / 运算堆栈 等
        比如如下 C# 方法: 
            static int Add(int a, int b, int c) 
            {
                return a + b + c;
            }
        它在编译器的 Release 优化下会编译成如下 IL:
            .method private hidebysig static 
                int32 Add (
                    int32 a,
                    int32 b,
                    int32 c
                ) cil managed 
            {
                .maxstack 8

                IL_0000: ldarg.0 ({{nameof(OpCodes.Ldarg_0)}})
                IL_0001: ldarg.1 ({{nameof(OpCodes.Ldarg_1)}})
                IL_0002: add     ({{nameof(OpCodes.Add)}})
                IL_0003: ldarg.2 ({{nameof(OpCodes.Ldarg_2)}})
                IL_0004: add     ({{nameof(OpCodes.Add)}})
                IL_0005: ret     ({{nameof(OpCodes.Ret)}})
            } // end of method Program::Add
        这里我们把一部分声明部分放出来了, 不过我们只需要关心这一行: .maxstack 8
            它表示请求在这个方法执行过程中评估栈有确保 8 个大小的空间可以使用
        首先我们会介绍一个系列的操作符 ldarg.*, 它表示将方法参数列表中的第 (* + 1) 个参数压入评估栈中, 不带参数
            注意这是在静态方法中而言的, 如果该方法是成员方法, 那么 ldarg.0 实际上压入的是 this 的值, 而 ldarg.1 才是第一个参数
        在上述段 IL 中, 该方法是个静态方法, 所以前两行 ldarg.0 与 ldarg.1 会将第一个参数和第二个参数压入评估栈中
        然后要介绍的是 add 操作符, add 操作符会弹出评估栈上的两个元素, 然后将它们相加, 然后将结果压入评估栈
        最后是 ret 操作符, 当方法没有返回值时它会将控制权交回给调用者, 同时 jit 会为我们检查评估栈是否清空,
            如果评估栈上还有东西那么同样 jit 会抛出异常({{nameof(InvalidProgramException)}}).
            当方法拥有返回值时它会确保评估栈上有且只剩一个元素, 然后将这个值弹出并压入调用者的评估栈上(评估栈是方法独立的)
        乘法的操作符为 mul, 其使用方法与 add 一致
        减法的操作符为 sub, 它会弹出两个值, 将后弹出的值减去先弹出的值后将结果压入评估栈
            (相减的顺序刚好就是压入的顺序, 也即弹出的逆顺序, 这点会让我们在某些情况下手写 IL 的更符合直觉一点)
        """;
        public const string 将不同类型的数据压栈 = $"""
        ldc系列(load const, 压入常数)
            ldc.r8({nameof(OpCodes.Ldc_R8)}) 将参数中的double(Float64)字面量压入评估栈中
            ldc.r4({nameof(OpCodes.Ldc_R4)}) 则是float, ldc.i4({nameof(OpCodes.Ldc_I4)})指int(Int32)
            il 中没有对应的加载 int16 和 int8 的指令, 我们得使用 ldc.i4
                虽然它本来是用来加载 int32 的, 但 jit 会知道我们想要干什么
            对于小一点的整数字面量 IL 还提供了一个 ldc.i4.s({nameof(OpCodes.Ldc_I4_S)}) 指令, 其参数为 Int8 即 byte 或 sbyte 类型
        """;
        public const string 方法的调用 = $"""
        在 IL 中有三种调用方法指令:
            操作符                                 参数          描述
            call({nameof(OpCodes.Call)})           方法token     根据方法的参数列表(包含 this, 如果其是成员方法时)逆顺序弹出对应数量参数并以此调用对应方法
            callvirt({nameof(OpCodes.Callvirt)})   方法token     同 call, 但是该指令在对应方法为虚方法时会向下寻找重写后的方法
            calli({nameof(OpCodes.Calli)})         callsite描述  根据 callsite 描述 弹出对应参数并再次弹出所需的函数指针并调用
        其中用的最多的是 call 和 callvirt, 最后一个 calli 在做与本机交互时才常用, 因为它要求我们有对应的函数指针
            (函数指针可能很多教程不会提及, 你可以在MSDN 上的不安全代码、数据指针和函数指针这篇文章中了解)
        call 通常生成于静态方法的调用中, 而 callvirt 在运行时会检测目标类型, 并向下查找可能的被重写后的方法
            一般对于普通成员方法的调用, C# 编译器也会生成 callvirt 指令, 这是因为 callvirt 需要检查目标类型,
            在调用对象为 null 时就抛出 NullReferenceException, 而 call 指令可能直到方法调用一半时才察觉 this 为 null,
            这是一个很危险的行为
        call 和 callvirt 需要的参数为方法token, 一般为对应方法的MethodInfo
        对于有返回值的方法, call / callvirt 调用完后会将返回值压入堆栈
        当我们不需要返回值时, 我们必须显式使用 pop 指令舍弃它, 防止它"污染"我们的评估栈
        方法的参数列表顺序和压栈顺序一致, 调用多参数方法也会显得很自然
        """;
        public const string 对象实例化 = $"""
        使用 newobj({nameof(OpCodes.Newobj)}) 操作符以实例化一个对象
            其参数为对应对象的一个构造器(MethodInfo)
            该指令执行后会将我们要的对象压入评估栈
        """;
        public const string 成员方法的调用 = """
        成员方法的调用与静态方法调用基本一致, 但是每次调用之前我们都必须记得将 this 的值作为第0个参数压入堆栈
        顺便, 记住这里的 this 是作为参数传递的, 每次调用都会被弹出评估栈, 所以在连续调用它的成员方法时记得将 this 再次压入
        通常我们会使用 callvirt 来调用成员方法, 一方面为了确保调用到了重写后的虚函数, 一方面为了尽可能早的检测出 this 为 null
        """;
        public const string 局部变量 = $"""
        使用 stloc.0({nameof(OpCodes.Stloc_0)}) 以将栈顶弹出并存到0号位的局部变量上
        使用 ldloc.0({nameof(OpCodes.Ldloc_0)}) 以读取0号位的局部变量的值压入栈中(可重复压栈)
        使用 dup({nameof(OpCodes.Dup)}) 以弹出栈顶的元素, 然后压入两遍这个元素
        """;
        public const string 属性的访问 = """
        通常, 一个名为 MyProp 的属性的 getter 方法叫做 get_MyProp, setter 方法叫做 set_MyProp
        顺便, 这一对方法它们各自都有一个 special name 的特殊标记以便编译器知晓它们归属于一个属性
        """;
        public const string 字段的访问 = $"""
        对于静态字段的访问, 我们需要使用 ldsfld({nameof(OpCodes.Ldsfld)}) 操作符
            它接受此静态字段的 token (FieldInfo) 作为参数, 将对应的静态字段的值压入评估栈
        对于成员字段的访问则是用 ldfld({nameof(OpCodes.Ldfld)}), 它会额外弹出一个 this 元素
        """;
        public const string 跳转指令 = $"""
        在 IL 中, 为了方便 大于, 小于, 大于等于, 小于等于, 是否 null, 是否非0 等众多条件表达式的判断, IL 引入了大量有关的指令:
            指令                              描述
            beq({nameof(OpCodes.Beq)})        如果两个值相等，则将控制转移到目标指令
            beq.s({nameof(OpCodes.Beq_S)})    如果两个值相等，则将控制转移到目标指令(短格式, 后不累述)
            bge({nameof(OpCodes.Bge)})        如果第一个值大于或等于第二个值，则将控制转移到目标指令
            bge.un({nameof(OpCodes.Bge_Un)})  当比较无符号整数值或不可排序的浮点型值时，如果第一个值大于或等于第二个值，则将控制转移到目标指令(后不累述)
            bgt({nameof(OpCodes.Bgt)})        如果第一个值大于第二个值，则将控制转移到目标指令
            ble({nameof(OpCodes.Ble)})        如果第一个值小于或等于第二个值，则将控制转移到目标指令
            blt({nameof(OpCodes.Blt)})        如果第一个值小于第二个值，则将控制转移到目标指令
            bne.un({nameof(OpCodes.Bne_Un)})  当两个无符号整数值或不可排序的浮点型值不相等时，将控制转移到目标指令
            br({nameof(OpCodes.Br)})          无条件地将控制转移到目标指令
            brfalse({nameof(OpCodes.Brfalse)})如果 value 为 false、空引用零，则将控制转移到目标指令
            brtrue({nameof(OpCodes.Brtrue)})  如果 value 不满足上条的条件，则将控制转移到目标指令
        可通过{nameof(ILContext.DefineLabel)}或{nameof(ILCursor.DefineLabel)}定义标记, {nameof(ILCursor.MarkLabel)}打上标记, 将此Label作为跳转的参数
        """;
        public const string 短格式版本指令 = """
        (.s 系指令)
        一些 IL 指令同时还有一个带 .s 后缀的版本, 这个我们一般叫它的 "短格式" 版本, 反之叫 "长格式" 版本
        通常类似这一对 IL 指令的区别就是 .s 版本的参数会短一点, 比如长格式版本的参数长度是 4 字节, 而短格式版本的参数长度可能就是 2 字节
        注意这里不是说的是压入评估栈的值的类型, 长短格式版本所做的事情是完全一样的, 只是传参允许你用短一点的参数
        这其实就是一个对 IL 指令的大小优化, 编译器通常就会能用 .s 版本就用 .s 版本, 当参数需求超过短格式参数表达范围时才会使用长格式
        对于我们的话如果你想微微的优化一下你的 IL 的大小的话, 你可以选择在参数范围够用的情况下使用 .s 版本
        """;
    }
    public Documents.Document.OpCodes_static_cls 更多的IL指令;
    public static void 挂IL钩子() {
        string 参考 = "https://celestemod.saplonily.link/trans/adv_hooks/";
        //首先找到要钩取的方法, 在Mod.Load重写或ModSystem.Load重写中写下此句
        IL_Main.DrawInfoAccs += IL_Main_DrawInfoAccs;
        //如果IL_Xxx中没有给出想要钩取的方法可参考下条
        Show<泰拉瑞亚On.给任意方法上钩子>();
        //通过以上两种方法加载的钩子TML保证会卸载
        Show<泰拉瑞亚On.给任意方法上钩子而且不自动卸载的那种_可脱离TML使用>();
        static void IL_Main_DrawInfoAccs(ILContext il) {
            // 这里写IL
        }
    }
    public const string 关于指针语句和标签的位置关系 = $"""
        指针: {nameof(ILCursor)}, 语句: {nameof(Instruction)}, 标签: {nameof(ILLabel)}
        指针的位置处于语句的缝隙中, 包括第一条语句前和最后一条语句后
            指针的上一句和下一句是挨在一起的
        标签指向一个语句, 可以看作在语句之前的位置, 一个语句可能会有多个标签指向它
        语句本身不保存关于指向自己的标签的数据, 若需要则要从ILContext.Labels中搜索获得
        指针内部存有 ILLabel[]类型的 _afterLabels 数据标明自己在下一条语句的哪些标签后
            即指针默认是在下一条语句的所有标签之前的
        当指针插入一条语句时, 会将 _afterLabels 中的标签导向到新插入的语句上,
            以表现出指针和标签之间的位置关系
        另外使用{nameof(ILCursor.Emit)}插入一条语句时, 指针会跳到这条语句之后, 方便
            继续插入语句
        """;
    public static void 如何写钩子(ILContext il) {
        // 一般我们需要至少一个IL指针:
        ILCursor cursor = new(il);  // 指针初始会位于所有语句之前, 方法的开头处
        #region 定位
        Show("""
        假如需要定位到一句 value += 1; 之前查看到对应的IL语句为:
            ldloc.0
            ldc.i4.1
            add
            stloc.0
        则下面这条语句就会定位到这句之前
        """);
        cursor.TryGotoNext(MoveType.AfterLabel,
            ILPatternMatchingExt.MatchLdloc0,
            i => i.MatchLdcI4(1),
            ILPatternMatchingExt.MatchAdd,
            ILPatternMatchingExt.MatchStloc0);
        // 定位功能还有其他几个变体
        cursor.TryGotoPrev();   //向前查找, 同样也是遇到第一组符合条件的语句就停下
        cursor.GotoNext();      //若没找到对应语句则直接报错
        cursor.GotoPrev();
        Show($"""
        需要注意方法中可能不止一组语句符合上面的条件
        上面的语句会定位到cursor之前或之后的满足条件的第一组语句前
        如果要跳到第二组或者之后的相同语句, 可以多次调用以上方法
            当多次调用GotoNext系列的方法时, ILCursor会尝试避免定位到与上次相同的位置
                具体原理为ILCursor有一个{nameof(ILCursor.SearchTarget)}, 默认为{SearchTarget.None},当
                    调用GotoNext系列的方法时, 若moveType是{MoveType.After}则将其设置为{SearchTarget.Prev},
                    否则设置为{SearchTarget.Next}.
                    而在开始时如果是向后查找而其是{SearchTarget.Next}, 或者反之, 则会跳过指针之后或者之
                    前的第一句, 从第二句开始查找.
        关于moveType参数:
            默认为{MoveType.Before}, 代表定位到这组语句之前
            如果要在这组语句之前插入语句, 则最好使用{MoveType.AfterLabel}, 
                这样会将指向这组语句的第一条的所有标签重定向到插入的语句上,
                保证这组语句前必然是所插入的语句
            使用{MoveType.After}则会定位到这组语句之后

        如果只是简单的想跳过下一条语句则并不建议使用GotoNext系列, 可以直接按下面的方法写:
        """);
        // 跳过下一条语句
        cursor.Next = cursor.Next.Next; // 需要注意如果cursor在末尾的话cursor.Next会是空, 此时会报错
        // 回到上一条语句之前
        cursor.Next = cursor.Prev;  // 如果cursor在开头cursor.Prev会是空, 此时会让指针挪到末尾
        // 也可以这么写, 虽然它的时间效率是O(n) (需要在il.Instrs中查找它的位置)
        cursor.Index += 1;

        // 也可以直接简单粗暴的在ILContext中搜索
        Instruction instr = il.Instrs.First(i => i.MatchLdloc0());
        cursor.Goto(instr);     // 跳到搜索到的语句之前
                                // 十分不推荐使用il.Instrs[i]的形式, 因为其他模组也有可能对此IL进行修改
        #endregion
        #region 插入IL语句
        Show("""
        为了安全性起见(防御性编程), 最好不要移除语句, 而只是插入语句
        如果不想要一段语句执行, 可以在前面插入跳转, 跳转到这段语句之后
        """);
        // 假设0号局部变量是 value, 在 cursor 处插入一句 value += 1;
        cursor.EmitLdloc0();
        cursor.EmitLdcI4(1);
        cursor.EmitAdd();
        cursor.EmitStloc0();

        // 如果想做更复杂的事情也可以直接插入函数调用
        cursor.EmitLdloc0();
        cursor.EmitDelegate((int i) => Math.Clamp(i, 0, 100) / 5);
        cursor.EmitStloc0();

        #region 直接压栈外部变量
        // 假如我们现在有一个变量
        int i = Main.DiscoColor.R;
        // 当然这种方式是可以的:
        cursor.EmitDelegate(() => i);
        // 不过其实有更简单的方式:
        int idOfI = cursor.EmitReference(i);
        cursor.EmitGetReference<int>(idOfI); //重复调用时可以把上面的结果保留再给此方法使用
        #endregion
        #endregion
        #region 跳过一段语句
        // 找到对应的语句, 然后在其之前插入一个跳转到这段语句之后的无条件跳转
        cursor.GotoNext(MoveType.AfterLabel,
            ILPatternMatchingExt.MatchLdloc0,
            i => i.MatchLdcI4(1),
            ILPatternMatchingExt.MatchAdd,
            ILPatternMatchingExt.MatchStloc0);
        cursor.EmitLdloc0();
        cursor.EmitLdcI4(3);
        cursor.EmitAdd();
        cursor.EmitStloc0();
        cursor.EmitBr(il.Instrs[cursor.Index + 4]);
        // 当然以上5句也可以简写为以下3句, 只是被修改的方法的性能会下降一些罢了
        // cursor.EmitLdloc0();
        // cursor.EmitDelegate((int i) => i + 3);
        // cursor.EmitStloc0();
        #endregion
        #region 不使用ILCursor
        // 不是很推荐这种方式, 毕竟 ILCursor 已经包装好了
        int index = 0;
        for(; index + 3 < il.Instrs.Count; ++index) {
            if(il.Instrs[index].MatchLdloc0()
                && il.Instrs[index + 1].MatchLdcI4(1)
                && il.Instrs[index + 2].MatchAdd()
                && il.Instrs[index + 3].MatchStloc0()) {
                break;
            }
        }
        if (index + 3 >= il.Instrs.Count) {
            throw new Exception("找不到对应的语句");
        }
        var labels = il.GetIncomingLabels(il.Instrs[index]);
        // Instrs集合在添加元素时会自动设置前后元素的Next和Previous
        il.Instrs.Insert(index + 1, il.IL.Create(OpCodes.Dup));
        il.Instrs.Insert(index + 2, il.IL.Create(OpCodes.Add));
        // 这样就把 value += 1; 改为了 value = value * 2 + 1;
        // 当然其实并不推荐破坏掉一个完整的C#语句, 这样会让其他mod的IL找不到这条语句
        #endregion
    }
    public static void 添加局部变量(ILContext il) {
        // 假如需要添加一个int类型的局部变量
        Type type = typeof(int);

        // 通过下面两句就可以将一个局部变量的声明添加到il对应的方法中
        VariableDefinition variable = new(il.Method.DeclaringType.Module.ImportReference(type));
        il.Body.Variables.Add(variable);

        // 然后就可以像对待局部变量一样对待它了
        ILCursor cursor = new(il);
        cursor.EmitLdloc(variable);
        cursor.EmitStloc(variable);
    }
}
