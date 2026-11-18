using System.Diagnostics;

namespace TigerLearning.Learning.额外研究;

public abstract class 关于Conditional {
    public const string ConditionalOnAttribute = """
        Conditional 可用于特性上，若没有指定的编译符号，此特性视为并没有被标记过
        其子类也都会一并视为没有被标记过
        但是此类依然会存在，并可以通过反射使用
        """;
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class)]
    public class DebugOnlyAttribute : Attribute;

    // 子类在非 DEBUG 时也不存在
    public class DebugOnlySubAttribute : DebugOnlyAttribute;

    public const string ConditionalOnMethod = """
        Conditional 也可用于方法上, 若没有指定的编译符号，此方法视为没有被调用过 (其调用会在 IL 中被抹除)
        但是反射依旧可以获取到，并且可以正常被反射调用
        方法的返回类型必须为 void
        可标记虚方法或抽象方法
        不可在重写时重新标记
        """;
    [Conditional("DEBUG")]
    public void DebugOnlyMethod() => Console.WriteLine($"In DEBUG! ({this})");
    
    [Conditional("DEBUG")]
    public virtual void DebugOnlyVirtualMethod() { }
    [Conditional("DEBUG")]
    public abstract void DebugOnlyAbstractMethod();
    public virtual void NormalVirtualMethod() { }

    public class OverrideClass : 关于Conditional {
        // 重写的虚方法或抽象方法默认带有原声明时的 Conditional
        public override void DebugOnlyVirtualMethod() { }
        public override void DebugOnlyAbstractMethod() { }
        // 不可在重写时重新标记
        /** [Conditional("DEBUG")] /**/
        public override void NormalVirtualMethod() { }
    }
}
