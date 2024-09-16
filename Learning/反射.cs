using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;

namespace TigerLearning.Learning;

public class 反射 {
    //其实这不是Terraria而是C#的内容
    public static void Type介绍() {
        //这是System.Type, 它表示一个变量的类型
        Type type;
        int i = 0;
        //通过GetType()可以获得一个变量的类型
        type = i.GetType();
        //也可以通过typeof从类名直接获取
        type = typeof(int);
        //GetType()的实际作用在于获得变量真正的类型, 包括继承等作用后的
        //但是当变量为空时会报错
        //GetType()也可以获得被声明为private或internal的类, 只要你有这个实例

        //type的一些属性:
        Show(type.BaseType);        //基类
        Show(type.Name);            //类名
        Show(type.Namespace);       //所在的命名空间名
        Show(type.FullName);        //基本相当于Namespace.Name
        Show(type.IsAbstract);      //是否是抽象类
        Show(type.IsArray);         //是否是数组
        Show(type.IsClass);         //是否是类
        Show(type.IsEnum);          //是否是枚举类型
        Show(type.IsInterface);     //是否是接口
        Show(type.IsNested);        //是否是嵌套类型(定义在一个类中的类型)
        Show(type.IsPublic);        //是否是公共的
        Show(type.IsNotPublic);     //是否不是公共的
        Show(type.IsSealed);        //是否是封闭的
        Show(type.IsValueType);     //是否是值类型
        type.GetInterfaces();       //获得所有此类型实现或继承了的接口的类型
        type.GetArrayRank();        //获取数组的维度
        type.GetEnumNames();        //获得此枚举变量的所有名字
        type.GetEnumValues();       //获得此枚举变量的所有值
        Enum e = default;
        type.GetEnumName(e);        //获取特定枚举变量的名字
    }
    public static void 获得成员() {
        //假如我们已经获得了一个Type
        Type type = typeof(Main);
        #region 成员
        //获取成员信息, 成员包括了字段, 方法, 属性, 索引器, 事件, 运算符重载, 构造函数, 析构函数, 甚至类型和常量等所有类中可以有的东西
        //因此MemberInfo是以下所有Info的基类
        MemberInfo[] memberInfos = type.GetMember("memberName");    //由于一个名字可能对应不止一个成员, 所以返回的是数组
        //BindingFlags代表对获取成员的一些限制条件, 如 实例/静态, 公共/私有 等
        BindingFlags flags = BindingFlags.Instance;
        //MemberTypes代表获取成员的类型, 如 字段, 属性, 方法 等
        MemberTypes types = MemberTypes.Field;
        //通过限制types和flags来限制获得到的MemberInfo
        type.GetMember("memberName", types, flags);
        type.GetMember("memberName", flags);    //types是可省略的, 如果都省略那么只能获得公共成员
        #endregion
        #region 字段
        //获得字段信息
        FieldInfo fieldInfo = type.GetField("fieldName", flags);
        #endregion
        #region 方法
        //获得方法信息, 其中flags可省略, 但省略后只能获得公共方法,下同
        MethodInfo methodInfo = type.GetMethod("MethodName", flags);
        //通过传入Type数组可规定参数类型
        type.GetMethod("DamageVar", flags, new[] { typeof(float), typeof(float) });
        #region 获得参数信息
        //可以通过GetParameters获得参数的信息
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        ParameterInfo parameterInfo = parameterInfos[0];
        Show(parameterInfo.ParameterType);  //获得此参数的类型
        Show(parameterInfo.Name);           //获得此参数的名字
        Show(parameterInfo.HasDefaultValue);//获得此参数是否有默认值
        Show(parameterInfo.DefaultValue);   //获得此参数的默认值(如果有的话)
        Show(parameterInfo.IsIn);           //是否是in参数
        Show(parameterInfo.IsOut);          //是否是out参数
        Show(parameterInfo.IsRetval);       //待测试(似乎是ref参数)
        Show(parameterInfo.Position);       //获得此参数在参数列表中的位置(从0开始)
        #endregion
        #endregion
        #region 属性或索引器
        //获得属性或索引器
        type.GetProperty("PropertyName", flags);
        Type[] typeArray = new Type[] { typeof(int), typeof(int) };
        //通过传入一个type和一个type数组可指定索引器的返回类型和参数列表
        //flags可与其他null一起省略
        //省略flags的情况下返回类型也可省略
        type.GetProperty("PropertyName", flags, null, typeof(int), typeArray, null);
        #endregion
        #region 构造函数和事件
        //获得构造函数.
        type.GetConstructor(flags, typeArray);
        //获取事件
        type.GetEvent("EventName", flags);
        #endregion
        #region 获得全部成员等
        //以上的数组版本
        type.GetMembers(flags);
        type.GetFields(flags);
        type.GetMethods(flags);
        type.GetProperties(flags);
        type.GetConstructors(flags);
        type.GetEvents(flags);
        #endregion
    }
    public static void 处理成员() {
        //假设我们已经有了一个对应的实例(就算这个实例是object也没问题)
        Main main = default;
        #region 获得和设置字段
        //假设我们已经获得了一个字段信息
        FieldInfo fieldInfo = default;
        //获得此字段, 可强制转化为所需类型
        object value = fieldInfo.GetValue(main);    //当为静态时不传实例(传null), 下同
        //设置字段
        fieldInfo.SetValue(main, value);
        //对于引用类型, 修改类型内的值会不会直接反应到原字段还有待测试
        #endregion
        #region 获得与设置属性
        //假设我们已经获得了一个属性信息
        PropertyInfo propertyInfo = default;
        //获得此属性, 可强制转化为所需类型
        value = propertyInfo.GetValue(main);
        //设置属性
        propertyInfo.SetValue(main, value);
        //也可以通过Get和Set方法调用, 但这样更麻烦, 主要还是用来看它有没有 get / set 访问器
        propertyInfo.GetGetMethod();
        propertyInfo.GetSetMethod();
        #endregion
        #region 调用方法
        //假设我们已经获得了一个方法信息
        MethodInfo methodInfo = default;
        //提前准备好参数
        object[] parameters = new object[] { 1, false };
        //调用
        methodInfo.Invoke(main, parameters);
        //若是使用ref或out传参的方法, 可以通过parameters数组获得改变后的值

        //可以创建此方法的委托以在之后执行
        Type delegateType = typeof(Action<int, bool>);
        methodInfo.CreateDelegate(delegateType, main);
        methodInfo.CreateDelegate<Action<int, bool>>(main);
        #endregion
        #region 设置事件
        //假设我们已经获得了一个事件信息
        EventInfo eventInfo = default;
        //再假设我们写好了一个委托
        Delegate handler = default;
        //添加委托
        eventInfo.AddEventHandler(main, handler);
        //移除委托
        eventInfo.RemoveEventHandler(main, handler);
        //也可以获得添加和移除委托的方法信息
        Show(eventInfo.AddMethod);      //此处可以获得到私有的方法
        Show(eventInfo.RemoveMethod);
        #endregion
        #region 构造函数
        //假设我们已经获得了一个构造函数信息
        ConstructorInfo constructorInfo = default;
        //创建一个实例
        main = (Main)constructorInfo.Invoke(parameters);
        //可以与方法一样创建为委托
        constructorInfo.CreateDelegate(delegateType);
        constructorInfo.CreateDelegate<Func<Main>>();
        #endregion
    }
    #region 额外研究
    /// <summary>
    /// <br/>从 <see cref="MethodBase"/> (包括 <see cref="MethodInfo"/>) 中获取 <see cref="MethodDefinition"/> 的方法
    /// <br/>见 <see cref="DynamicMethodDefinition"/> 的私有方法 LoadFromMethod(...)
    /// <br/>(
    /// <br/><see cref="ILHook.Apply"/>
    /// <br/><see cref="DetourManager.ManagedDetourState.AddILHook(DetourManager.SingleILHookState, bool)"/>
    /// <br/><see cref="DetourManager.ManagedDetourState"/> 的私有方法 UpdateEndOfChain()
    /// <br/><see cref="DynamicMethodDefinition"/> 的构造方法
    /// <br/>)
    /// </summary>
    public const string GetMethodDefinitionFromMethodBaseThroughDynamicMethodDefinition = $"""
        这样实际上是将参数, 返回类型, 所有的内部的语句全部
        从 {nameof(MethodInfo)} 复制到了 {nameof(MethodDefinition)} 中,
        需要一一解析, 实际很耗费时间
        """;
    #endregion
}