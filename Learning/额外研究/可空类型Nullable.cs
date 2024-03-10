namespace TigerLearning.Learning.额外研究;

public class 可空类型Nullable {
#nullable enable
    public const string intro = """
        关于各种 ? 的事情
        """;
    public const string 可为空的值类型 = """
        给值类型后加上?变为可为空的值类型(int? i = null;)(实际上会变为结构体Nullable<int>)
        此类型基本可以正常的当作原类型使用, 只是有空值时计算结果一般为空(2 + (int?)null -> null)
        特别的, 做大小于(包括不大于和不小于)运算时, 若有空值则必为假(此时小于等于不再与小于或者等于等效)
        而等于和不等于则会考虑是否为空的情况
        如果是必须使用原类型的场合(如赋值, 传参)可以直接强制类型转换, 但若为空会报错
        可以直接将原来的值类型的值赋给可为空的值类型
        """;
    public static void ShowNullableValueType() {
        Type nullableHelperType = typeof(Nullable);
        Type nullableType = typeof(Nullable<int>);
        int? a = null, b = 3, c = null;
        int i = 5;
        Console.WriteLine(a + b);  // -> null
        Console.WriteLine(a + i);  // -> null
        Console.WriteLine(i + b);  // -> 8

        Console.WriteLine(a < i);  // -> false
        Console.WriteLine(a >= i);  // -> false
        Console.WriteLine(a >= c);  // -> false
        Console.WriteLine(a == c);  // -> true
        Console.WriteLine(b < i);  // -> true

        i = (int)b; // -> i = 3
        //i = (int)a; // -> 报错
    }
    public const string 空条件运算符 = """
        a?.b 以安全的使用一个实例a的成员b, 当此实例为空时, 不会调用此成员, 而且返回值为空(若原来为值类型, 则会变为可为空的值类型)
            也叫做null传播
        a?[i] a为空时返回空, 否则按正常取索引处理(超界还是会报错)
        a ?? b 为当a为空时选用b, a与b一般得是相同的类型, 或a是可为空的值类型, b为此值类型(此时返回值为值类型而不为空)
            ?? 后也可以直接用throw语句, 代表若前者为空就直接报错, 相当于在其后加!
        a ??= b 当a为空时将b赋值给它
        """;
    public static class ShowNullOperator {
        public static void Show() {
            AClass? a = null;
            var intValue = a?.intValue; //intValue = (int?)null
            var stringValue = a?.stringValue; //stringValue = (string)null
            int i = 1;
            a?.SetIntValue(i = 5); //什么都不会发生, 赋值语句也不会被执行

            int? ni = null, nj = 6, nk = null;
            Console.WriteLine(ni ?? nj); // ->6
            Console.WriteLine(ni ?? nk); // ->null
            i = ni ?? 0; // ni为空则将0赋给i, 这样就不用强制类型转换了
            ni ??= nj; //ni为空时将nj赋给它
        }
        public class AClass {
            public int intValue;
            public string? stringValue;
            public int SetIntValue(int value) {
                return intValue = value;
            }
        }
    }
    public const string 可为空的引用类型 = """
        启用可为空的引用类型:
            单文件内可以使用 #nullable enable 以启用, disable以禁用, restore以还原默认设置
            项目内可以设置项目文件中Project.PropertyGroup.Nullable为enable以启用
            或者:项目设置->生成->常规->可为null的类型
        启用时可以在引用类型后加?代表它是可以为空的, 而没加则代表此引用类型不会为空
        这两种类型本质上没有区别, 只是可能有空引用报错的地方都会标注出来
        如果想要一个变量不被标注(你确保它非空, 但是程序上识别不出来)可以在它后面加!标注
        """;
    public static void ShowNullableReferencesType() {
        string? a = null, a2 = null, a3 = null, a4 = null;
        //string b = null;    //被警告了
        //Show(b.Length);     //虽说这里b本来是string, 不会为空的类型, 但强行赋为了空, 所以还是会给出警告
        void Deleg(ref string? s) => s = "123";
        Deleg(ref a);
        //Show(a.Length); //此时a已经非空, 但程序难以检查, 仍然会给警告
        //Show(a.Length); //(如果有上面那句则这句不会给出警报)此时前面已经给出警告了, 若在那里爆了空引用报错, 就不会执行到这里, 所以程序认为这里a肯定非空(...)
        Deleg(ref a2);
        Show(a2!.Length); //在a后加!会禁用其空引用的检查, 让编译器认为它在这里总是非空的
        void ShowString(string s) => Do(s);
        Deleg(ref a3);
        //ShowString(a3); //同样会给出警报
        Deleg(ref a4);
        ShowString(a4!);
    }

#nullable restore
}