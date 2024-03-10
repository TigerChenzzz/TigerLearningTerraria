using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using TigerLearning.Learning;

namespace TigerLearning.Documents;
public partial class Document {
    #region namespace MonoMod.Cil
    public class ILContext_cls {
        public static ILContext ilContext;
        public static string intro = """
            表示一个方法的 IL 上下文
            """;
        public static void ShowILContext() {
            #region params
            MethodBase method = default;
            Instruction instruction = default;
            #endregion
            _ = new ILCursor(ilContext);    //创建一个从此方法头部开始的ILCursor
            Show(ilContext.Instrs);         //此方法中的所有语句
            Show(ilContext.Labels);         //此方法中的所有标签
            ilContext.GetIncomingLabels(instruction);   //获得此语句前的标签(实际上是搜索ilContext.Labels以获得所有Target是此语句的标签)
            ilContext.ToString();   //获得此方法的方法名以及主体IL代码
        }
    }
    public class ILCursor_cls {
        public static ILCursor ilCursor;
        public static string intro = """
            指示il的位置, 可以理解为处于两个语句之间
            """;
        public static void ShowILCursor() {
            #region params
            ILContext ilContext = default;
            Instruction instruction = default;
            ParameterReference parameterReference = default;
            VariableReference variableReference = default;
            MethodInfo methodInfo = default;
            MethodReference methodReference = default;
            FieldInfo fieldInfo = default;
            FieldReference fieldReference = default;
            ILLabel label = default;
            ILLabel[] labels = default;
            int index = default;
            Main any = default;
            #endregion
            #region 创建一个ILCursor
            ilCursor = new(ilContext);  //从ilContext的头部开始
            ilCursor = new(ilCursor);   //从另一个ilCursor处开始
            #endregion
            Show(ilCursor.Context);     //对应的ILContext
            Show(ilCursor.Instrs);      //此方法中的所有语句, 即Context.Instrs
            Show(ilCursor.IncomingLabels);  //获得下一条语句前的所有标签, 不论在指针前后, 即Context.GetIncomingLabels(Next)
            #region 定位
            Show(ilCursor.Previous);    //上一句, 如果在头部则为空, 可设置以进行跳转
            Show(ilCursor.Prev);        //同Previous
            Show(ilCursor.Next);        //下一句, 如果在尾部则为空, 可设置以进行跳转
            Show(ilCursor.Index);       //Next在Context中的位置
            //实际上ilCursor内部存有的是_next; Prev, Index都是由它计算而来

            ilCursor.Goto(instruction); //跳转到此句之前, 可以为空表示跳到尾部
            ilCursor.Goto(instruction, MoveType.Before);    //同上
            ilCursor.Goto(instruction, MoveType.After);   //跳转到此句之后或者是此句前的标签之后
            ilCursor.Goto(instruction, MoveType.AfterLabel);   //跳转到此句前的标签之后(相当于带有ilCursor.MoveAfterLabels()的MoveType.Before)
            //setTarget (默认 false )代表在下次搜索时是否跳过搜索此条语句, 为默认值 false 时会将 SearchTarget 设置为 None
            //当 moveType 为 Before (包括 AfterLabel )时会忽略下一条语句, 即把 SearchTarget 设置为 Next , 为 After 时则相反
            //当使用 (Try)GotoNext/(Try)GotoPrev 时就会跳过此句(具体为当 SearchTarget 为 Next 时 GotoNext 会跳过一句, 反之亦然)
            //而使用 Goto, (Try)GotoNext, (Try)GotoPrev 本身也会改变 SearchTarget, 后两者会调用 setTarget 参数为 true 的 Goto
            ilCursor.Goto(instruction, MoveType.Before, setTarget: true);
            ilCursor.Goto(index);   //可以使用数字表示语句的序号代替语句, 可以为负数表示倒数第几句
            //当搜索时搜索带的语句是在之前或者之后, 当下次搜索时会跳过此句, 可以看作是一个选中状态
            //当使用setTarget为false的Goto或者使用Prev, Next, Index等方式挪动指针时都会重置它
            //另外这也代表这只能只是一条语句, 即使搜索的是多条语句, 下次往相应方向搜索时也只会跳过壹句
            //还有一个特性, 当指针在末尾时它不会是Next, 在开头时不会是Prev
            Show(ilCursor.SearchTarget);
            Show(SearchTarget.None);
            Show(SearchTarget.Next);
            Show(SearchTarget.Prev);
            ilCursor.MoveAfterLabels(); //跳转到下一句前的所有标签之后
            ilCursor.MoveBeforeLabels(); //跳转到下一句前的所有标签之前
            ilCursor.GotoLabel(label);      //移动指针到此标签后, 相当于用AfterLabel跳到此标签指示的语句
            #endregion
            #region 查找
            ilCursor.TryGotoNext(); //为空时且SearchTarget为Next时跳过一条语句(此时若在尾部则不动并返回false), 建议不要使用
                                    //如果想跳过一条语句, 建议使用 ilCursor.Next = ilCursor.Next?.Next; 或 ilCursor.Index++;(注意超界)
            ilCursor.TryGotoNext(i => i.MatchAdd()); //跳到下一条操作符是加的语句的前面, 若没找到则返回false且不动
            ilCursor.TryGotoNext(MoveType.Before, ILPatternMatchingExt.MatchAdd); //同上
            ilCursor.TryGotoNext(MoveType.After, ILPatternMatchingExt.MatchAdd); //跳到下一条操作符是加的语句的后面, 若没找到则返回false且不动
            ilCursor.TryGotoNext(MoveType.After, ILPatternMatchingExt.MatchAdd); //跳到下一条操作符是加的语句前的标签后面, 若没找到则返回false且不动
            //向下寻找两条语句, 其中第一条是 add, 第二条是 sub, 然后跳到第一条之前, 若使用MoveType.After则是第二条之后
            ilCursor.TryGotoNext(ILPatternMatchingExt.MatchAdd, ILPatternMatchingExt.MatchSub); //跳到下一条操作符是加的语句的前面, 若没找到则返回false且不懂

            ilCursor.GotoNext(i => i.MatchLdcI4(10));    //基本同TryGotoNext, 只是TryGotoNext返回false时会报错; 返回自身(this)
            ilCursor.TryGotoPrev(); //除了为向上搜索外都与TryGotoNext一致
            ilCursor.GotoPrev();    //当TryGotoPrev返回false时报错, 否则返回自身

            //基本同TryGotoNext, 但不会动自身, 而是将predicates对应的ILCursor封装成cursors
            //cursors的长度与后续参数(predicates)的个数一致
            ilCursor.TryFindNext(out ILCursor[] cursors, i => i.OpCode == OpCodes.Add, i => i.OpCode == OpCodes.Add);
            ilCursor.TryFindPrev(out cursors, i => i.OpCode == OpCodes.Add);
            ilCursor.FindNext(out cursors, i => i.OpCode == OpCodes.Add);
            ilCursor.FindPrev(out cursors, i => i.OpCode == OpCodes.Add);
            #endregion
            #region 打标签
            label = ilCursor.DefineLabel(); //定义一个标签
            ilCursor.MarkLabel(label);      //标记上此标签, 并跳到此标签后
            label = ilCursor.MarkLabel();   //用此句可以干上面两行的事
            label.Target = ilCursor.Next;   //标记上此标签, 并留在此标签前
            label = ilCursor.MarkLabel(instruction);    //在此句上打上标签, 若指针也在此句前, 则将指针挪到标签后
            #endregion
            #region 插入语句
            //插入一条语句后默认都会挪到插入的语句之后
            ilCursor.Emit(OpCodes.Ldarg_1);     //插入一个操作符, 如果有参数则在继续后面传入参数
            #region 基本指令
            OpCodes_static_cls.ShowOpCodes();   //解释详见此处
            #region 简单运算
            ilCursor.EmitAdd();
            ilCursor.EmitSub();
            ilCursor.EmitMul();
            ilCursor.EmitDiv();
            ilCursor.EmitRem();
            ilCursor.EmitNeg();
            ilCursor.EmitCeq();
            ilCursor.EmitCgt();
            ilCursor.EmitClt();
            ilCursor.EmitAnd();
            ilCursor.EmitOr();
            ilCursor.EmitNot();
            ilCursor.EmitXor();
            #endregion
            #region 压入数据
            #region 将常数压栈
            ilCursor.EmitLdcI8(1L);
            ilCursor.EmitLdcI4(1);
            ilCursor.EmitLdcR8(1.0);
            ilCursor.EmitLdcR4(1.0F);
            ilCursor.Emit(OpCodes.Ldc_I4_0);
            ilCursor.Emit(OpCodes.Ldc_I4_8);
            ilCursor.EmitLdstr("Hello World!");
            #endregion
            ilCursor.EmitDup();
            #endregion
            #region 调用参数
            #region 压入参数
            ilCursor.EmitLdarg0();
            ilCursor.EmitLdarg1();
            ilCursor.EmitLdarg(10);
            ilCursor.EmitLdarg(parameterReference);
            #endregion
            #region 设置参数
            ilCursor.EmitStarg(3);
            #endregion
            #endregion
            #region 局部变量
            #region 压入局部变量
            ilCursor.EmitLdloc0();
            ilCursor.EmitLdloc1();
            ilCursor.EmitLdloc(10);
            ilCursor.EmitLdloc(variableReference);
            #endregion
            #region 设置局部变量
            ilCursor.EmitStloc0();
            ilCursor.EmitStloc(22);
            ilCursor.EmitStloc(variableReference);
            #endregion
            #endregion
            #region 调用方法或属性
            ilCursor.EmitCall(methodInfo);
            ilCursor.EmitCallvirt(methodInfo);
            ilCursor.EmitCall(methodReference);
            ilCursor.EmitCallvirt(methodReference);
            #endregion
            #region 调用字段
            ilCursor.EmitLdsfld(fieldInfo);
            ilCursor.EmitStsfld(fieldInfo);
            ilCursor.EmitLdfld(fieldInfo);
            ilCursor.EmitStfld(fieldInfo);
            ilCursor.EmitLdsfld(fieldReference);   //TBT
            ilCursor.EmitStsfld(fieldReference);   //TBT
            ilCursor.EmitLdfld(fieldReference);    //TBT
            ilCursor.EmitStfld(fieldReference);    //TBT
            #endregion
            #region 跳转指令
            ilCursor.EmitBr     (instruction); //用instruction时会自动新建一个Label
            ilCursor.EmitBr     (label);    //后面的都有instruction或label, 就只写短的一种了
            ilCursor.EmitBeq    (label);
            ilCursor.EmitBge    (label);
            ilCursor.EmitBgt    (label);
            ilCursor.EmitBle    (label);
            ilCursor.EmitBlt    (label);
            ilCursor.EmitBgeUn  (label);
            ilCursor.EmitBgtUn  (label);
            ilCursor.EmitBleUn  (label);
            ilCursor.EmitBltUn  (label);
            ilCursor.EmitBneUn  (label);
            ilCursor.EmitBrtrue (label);
            ilCursor.EmitBrfalse(label);
            ilCursor.EmitBle(label);
            ilCursor.EmitSwitch(labels);
            #endregion
            #region .s系指令
            //好像并没有直接给出ilCursor.EmitXxxS的指令
            ilCursor.Emit(OpCodes.Ldc_I4_S, 1);
            #endregion
            #region 其他指令
            ilCursor.EmitBreak();
            #endregion
            #endregion
            #region 插入引用
            int id = ilCursor.AddReference(any);    //同Context.AddReference(), 将任意对象引入并获得其id
            ilCursor.EmitGetReference<Main>(id);    //在指针处插入语句: 取得id和type对应的对象入栈
            ilCursor.EmitReference(any);    //干上面两句的事, 并返回id以供下次使用

            ilCursor.EmitDelegate((int i) => ++i);    //在指针处插入一个方法的调用, 相当于插入一个 call 语句(不是 callvirt)
            #endregion
            #endregion
            #region 去除语句
            ilCursor.Remove();  //去除下一条语句, 需要自己判超界
            ilCursor.RemoveRange(5);    //去除下面 5 条语句
            #endregion
            #region 杂项
            Show(ilCursor.IsBefore(instruction));   //是否在此语句之前(不必紧靠)
            Show(ilCursor.IsAfter(instruction));    //是否在此语句之后(不必紧靠)
            #endregion
        }
    }
    public class ILLabel_cls {
        public static ILLabel label;
        public static string intro = """
            表示一个标签, 如goto语句就是显示使用的标签
            if-else, while, for, switch等条件控制语句也都会使用标签控制
            """;
        public static void ShowLabel() {
            #region params
            ILContext ilContext = default;
            ILCursor ilCursor = default;
            Instruction instruction = default;
            #endregion
            #region 创建一个标签
            label = ilContext.DefineLabel();    //定义一个标签, 需要设置其Target
            label = ilContext.DefineLabel(instruction); //把标签打在这里
            #endregion
            Show(label.Branches);   //所有操作数是此标签的语句(即所有指向这里的语句)(通过搜索Context.Instrs获得)
            Show(label.Target);     //label指向的语句

            #region 打上标签
            ilCursor.MarkLabel(label);  //将标签打在指针前
            #endregion
        }
    }
    #endregion
    #region namespace Mono.Cecil.Cil
    public class OpCodes_static_cls {
        public static string tips = """
            在IL中压栈通常以ld开头, 出栈则以st开头
            """;
        public static string reference = """
            C# IL 指令集 - 云城 - 博客园: https://www.cnblogs.com/yuncheng/p/3437419.html
            """;
        public static void ShowOpCodes() {
            #region params
            ILCursor il = default;
            MethodInfo methodInfo = default, constructor = default;
            MethodReference methodReference = default;
            FieldInfo fieldInfo = default;
            Instruction insctruction = default;
            ILLabel label = default;
            ILLabel[] labels = default;
            ParameterReference parameterReference = default;
            VariableReference variableReference = default;
            #endregion
            #region 简单运算
            il.Emit(OpCodes.Add);           //将栈上的两个值相加并将结果压栈
            il.Emit(OpCodes.Sub);           //从栈弹出两个值, 将后弹出的值(先压入的值, 也称第一个值)减去先弹出的值后将结果压栈
            il.Emit(OpCodes.Mul);           //相乘
            il.Emit(OpCodes.Div);           //相除, 结果为浮点或整型(TBT)
            il.Emit(OpCodes.Rem);           //取余
            il.Emit(OpCodes.Neg);           //取反
            il.Emit(OpCodes.Ceq);           //从栈弹出两个值, 若相等则将1(true)压栈, 否则将0(false)压栈
            il.Emit(OpCodes.Cgt);           //大于
            il.Emit(OpCodes.Clt);           //小于
            il.Emit(OpCodes.And);           //与
            il.Emit(OpCodes.Or );           //或
            il.Emit(OpCodes.Not);           //非
            il.Emit(OpCodes.Xor);           //异或
            //cge 和 cle 是没有的
            #endregion
            #region 压入数据
            #region 将常数压栈
            il.Emit(OpCodes.Ldc_I8, 5_000_000_000L);//压入一个8字节整数(long)
            il.Emit(OpCodes.Ldc_I4, 100);           //压入int (根据传入参数也可以是压入short或byte)
            il.Emit(OpCodes.Ldc_R8, 2.2);           //压入double
            il.Emit(OpCodes.Ldc_R4, 1.1f);          //压入float
            il.Emit(OpCodes.Ldc_I4_0);              //压入int值0
            il.Emit(OpCodes.Ldc_I4_8);              //压入int值8
            il.Emit(OpCodes.Ldstr, "Hello World!"); //压入字符串
            #endregion
            il.Emit(OpCodes.Dup);   //弹出栈顶的元素, 然后将其压入两遍
            #endregion
            #region 调用参数
            #region 压入参数
            il.Emit(OpCodes.Ldarg_0);       //压入第一个参数(如果不是静态方法第一个参数实际上是this, 从第二个开始才是方法上标明的参数)
            il.Emit(OpCodes.Ldarg_1);       //压入第二个参数
            il.Emit(OpCodes.Ldarg_S, 10);   //压入第十一个参数(非静态方法且不算this的话就是第十个)
            il.Emit(OpCodes.Ldarg, parameterReference);  //TBT
            #endregion
            #region 设置参数
            //这里就没有starg.0, starg.1之类的, 有点奇怪
            il.Emit(OpCodes.Starg, 3);      //弹出一个值存入第四个参数中
            #endregion
            #endregion
            #region 局部变量
            #region 压入局部变量
            il.Emit(OpCodes.Ldloc_0);       //压入调用堆栈索引为0处的值(编号为0的局部变量)
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldloc_S, 10);
            il.Emit(OpCodes.Ldloc, variableReference);  //TBT
            #endregion
            #region 设置局部变量
            il.Emit(OpCodes.Stloc_0);   //弹出一个值并存入编号为0的局部变量
            il.Emit(OpCodes.Stloc, 22); //弹出一个值并存入编号为22的局部变量
            il.Emit(OpCodes.Stloc, variableReference);  //TBT
            #endregion
            #endregion
            #region 调用方法
            //call或callvirt会根据方法的参数列表(包含 this, 如果其是成员方法时)逆顺序弹出对应数量参数并以此调用对应方法
            //对于有返回值的方法, call / callvirt 调用完后会将返回值压栈
            il.Emit(OpCodes.Call, methodInfo);      //call 指令一旦指定了对应方法, 那么在运行时调用的方法是不会变的, 所以它通常生成于静态方法的调用中
            il.Emit(OpCodes.Callvirt, methodInfo);  //callvirt 在运行时会检测目标类型, 并向下查找可能的被重写后的方法, 所以按字面意思它经常生成于虚方法的调用中
                                                    //额外的, callvirt 需要检查目标类型, 在调用对象为 null 时就抛出 NullReferenceException, 而 call 指令可能直到方法调用一半时才察觉 this 为 null, 这是一个很危险的行为
                                                    //所以对于普通成员方法的调用, C# 编译器一般也会生成 callvirt 指令
            il.Emit(OpCodes.Call, methodReference);         //TBT
            il.Emit(OpCodes.Callvirt, methodReference);     //TBT
            il.Emit(OpCodes.Newobj, constructor);   //constructor是构造器的MethodInfo, 通过这个构造器实例化一个对象并压入栈中
            #endregion
            #region 调用属性
            //相当于调用方法, 通常, 一个名为 MyProp 的属性的 getter 方法叫做 get_MyProp, setter 方法叫做 set_MyProp
            //顺便, 这一对方法它们各自都有一个 special name 的特殊标记以便编译器知晓它们归属于一个属性
            #endregion
            #region 调用字段
            il.Emit(OpCodes.Ldsfld, fieldInfo); //获取静态字段的值压栈
            il.Emit(OpCodes.Stsfld, fieldInfo); //从栈中取出一个值并存入静态字段中
            il.Emit(OpCodes.Ldfld, fieldInfo);  //从栈中取出一个值作为 this, 然后获取其字段的值压栈
            il.Emit(OpCodes.Stfld, fieldInfo);  //从栈中取出两个值, 将先取出的(第二个压入的)存入第一个作为this的字段中
            #endregion
            #region 跳转指令
            il.Emit(OpCodes.Br, insctruction);  //无条件跳转, instruction可以写 ilCursor.Next/Prev/Previous 等
            il.Emit(OpCodes.Br, label);         //无条件跳转, label可以由 ilCursur.MarkLabel 等方式得到
            il.Emit(OpCodes.Beq, label);        //(从栈中弹出两个数)相等时跳转
            il.Emit(OpCodes.Bge, label);        //大于等于时跳转(后弹出的(先压入的)与另一个比较)
            il.Emit(OpCodes.Bgt, label);        //大于时跳转
            il.Emit(OpCodes.Ble, label);        //小于等于时跳转
            il.Emit(OpCodes.Blt, label);        //小于时跳转
            il.Emit(OpCodes.Bge_Un, label);     //当比较无符号整数值或不可排序的浮点型值时, 大于等于时跳转
            il.Emit(OpCodes.Bgt_Un, label);     //当比较无符号整数值或不可排序的浮点型值时, 大于时跳转
            il.Emit(OpCodes.Ble_Un, label);     //当比较无符号整数值或不可排序的浮点型值时, 小于等于时跳转
            il.Emit(OpCodes.Blt_Un, label);     //当比较无符号整数值或不可排序的浮点型值时, 小于时跳转
            il.Emit(OpCodes.Bne_Un, label);     //当比较无符号整数值或不可排序的浮点型值时, 不等于时跳转
            il.Emit(OpCodes.Brfalse, label);    //为假时跳转(包括空或0)
            il.Emit(OpCodes.Brtrue, label);     //为真时跳转(包括非空非0值)
            il.Emit(OpCodes.Switch, labels);    //从栈顶弹出一个无符号整数值, 跳转到labels对应下标的标签处
            #endregion
            #region .s系指令
            Show(泰拉瑞亚IL.一个入门IL教程.短格式版本指令);
            //这里不全部收录
            il.Emit(OpCodes.Ldc_I4_S, 20);
            il.Emit(OpCodes.Ldarg_S, 10);
            il.Emit(OpCodes.Ldloc_S, 10);
            il.Emit(OpCodes.Stloc_S, 10);
            il.Emit(OpCodes.Br_S);
            #endregion
            #region 其他指令
            il.Emit(OpCodes.Break);    //引发断点 (C# 的 break 语句实际上对应的是无条件跳转)
            #endregion
            Show(OpCodes.Ret);              //返回, 无参, 会检查栈, 若无返回值则栈需为空, 若有栈中应只有需要返回的值
            Show(OpCodes.Nop);              //空语句
        }
    }
    public class Instruction_cls {
        public static Instruction instruction;
        public static string intro = """
            代表一条语句
            """;
        public static void ShowInstruction() {
            Show(instruction.Next);     //下一条语句
            Show(instruction.Previous); //上一条语句
            Show(instruction.OpCode);   //操作代码
            Show(instruction.Operand);  //操作数(参数), 若没有则是null
            Show(instruction.Offset);   //地址偏移, 一般不需要使用
            #region 创建一条语句
            Instruction.Create(OpCodes.Add);    //如操作符有参数则需额外传入此参数
            #endregion
        }
    }
    #endregion
}
