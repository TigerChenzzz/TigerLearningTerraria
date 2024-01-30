using System.Reflection;

namespace TigerLearning.Learning;

public class 特性 {
    public static string 参考 = "C#之特性 https://blog.csdn.net/weixin_39520967/article/details/122676703";
    #region 已定义的特性
    public static class AttributeUsage_attr {
        public static string intro = """
            AttributeUsage特性只能在Attribute的派生类上生效
            构造参数中validOn代表能应用特性的目标类型
                Inherited代表能否被继承, 默认false
                AllowMultiple代表是否可以给目标上多个此特性, 默认false
            """;
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
        public class ExampleAttribute : Attribute { }
    }
    public static class Obsolete_attr {
        public static string intro = """
            用以标注某些东西已过时, 可传入提示信息(字符串)和是否报错
            """;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:提供 ObsoleteAttribute 消息", Justification = "<挂起>")]
        [Obsolete]
        public static int obsoleteField;
        [Obsolete("message")]
        public static void ObsoleteMethod() { }
        [Obsolete("message", true)]
        public delegate void ObsoleteDelegate();
    }
    #endregion
    #region 自定义特性
    [AttributeUsage(AttributeTargets.Class)]
    public class MyCustomAttribute : Attribute {
        public int number;
        public void AMethod() {
            Console.WriteLine(number);
        }
    }
    public static string 声明一个自定义特性 = """
        定义一个类并且这个类派生自System.Attribute
        类的名字以Attribute结尾
        为了安全性, 可以用一个sealed修饰成一个密封类型(非必须的), 以防止被其他类所继承
        (可以参考预定义特性的源码, 其都被sealed所修饰)
        """;
    public static string 成员 = "特性可以有公有成员，但是公有成员只能是字段, 属性或构造函数";
    public static string 限制特性的使用 = "使用AttributeUsage以限制";
    public static void ShowAttribute() {
        //用type.IsDefined(AttributeType, inherit = false)判断特性是否应用到type类型上, inherit代表是否检查继承链
        typeof(特性).IsDefined(typeof(Attribute), inherit: true);
        //用type.GetCustomAttribute[s]([AttributeType, ]inherit = false)或
        //type.GetCustomAttribute[s]<Attribute>(inherit = false)获得类型上的特性
        typeof(特性).GetCustomAttribute<Attribute>();
        Attribute.GetCustomAttribute(typeof(特性).GetMember("name")[0], typeof(Attribute));
    }
    #endregion
}