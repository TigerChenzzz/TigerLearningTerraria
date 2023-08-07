#nullable enable        //WIP

using System.IO;
using System.Linq;
using System.Reflection;

namespace TigerLearning.Common.MyUtils;

static public partial class TigerUtils {
    #region Lerp
    public enum LerpType {
        Linear,
        Quadratic,
        Cubic,
        CubicByK,
        Sin,
        Stay,
    }
    public static System.Numerics.Matrix4x4 NewMatrix(Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4) {
        return new(v1.X, v1.Y, v1.Z, v1.W,
                    v2.X, v2.Y, v2.Z, v2.W,
                    v3.X, v3.Y, v3.Z, v3.W,
                    v4.X, v4.Y, v4.Z, v4.W);
    }
    public static float NewLerpValue(float val, bool clamped, LerpType type, params float[] pars) {
        if(clamped) {
            if(val <= 0) {
                return 0;
            }
            if(val >= 1) {
                return 1;
            }
        }
        if(val == 0) {
            return 0;
        }
        if(val == 1) {
            return 1;
        }
        switch(type) {
        case LerpType.Linear: {
            return val;
        }
        case LerpType.Quadratic: {
            //pars[0]:二次函数的极点
            if(pars.Length <= 0) {
                throw new TargetParameterCountException("pars not enough");
            }
            if(pars[0] == 0.5f) {
                return 0;
            }
            return val * (val - 2 * pars[0]) / (1 - 2 * pars[0]);
        }
        case LerpType.Cubic: {
            //pars[0], pars[1]:三次函数的两个极点
            if(pars.Length <= 1) {
                throw new TargetParameterCountException("pars not enough");
            }
            return ((val - 3 * (pars[0] + pars[1]) / 2) * val + 3 * pars[0] * pars[1]) * val /
                (1 - 3 * (pars[0] + pars[1]) / 2 + 3 * pars[0] * pars[1]);
        }
        case LerpType.CubicByK: {
            //pars[0], pars[1]:两处的斜率
            //par[2], par[3](若存在):宽度和高度
            if(pars.Length < 2) {
                throw new TargetParameterCountException("pars not enough");
            }
            float par2 = pars.Length < 3 ? 1 : pars[2], par3 = pars.Length < 4 ? 1 : pars[3];
            if(par2 == 0) {
                return 0;
            }
            Vector4 va = new(0, par2 * par2 * par2, 0, 3 * par2 * par2);
            Vector4 vb = new(0, par2 * par2, 0, 2 * par2);
            Vector4 vc = new(0, par2, 1, 1);
            Vector4 vd = new(1, 1, 0, 0);
            Vector4 v0 = new(0, par3, pars[0], pars[1]);
            var d0 = NewMatrix(va, vb, vc, vd);
            var da = NewMatrix(v0, vb, vc, vd);
            var db = NewMatrix(va, v0, vc, vd);
            var dc = NewMatrix(va, vb, v0, vd);
            var dd = NewMatrix(va, vb, vc, v0);
            if(d0.GetDeterminant() == 0) {
                return 0;
            }
            if(par3 == 0) {
                return (((da.GetDeterminant() * val + db.GetDeterminant()) * val + dc.GetDeterminant()) * val + dd.GetDeterminant()) / d0.GetDeterminant();
            }
            return (((da.GetDeterminant() * val + db.GetDeterminant()) * val + dc.GetDeterminant()) * val + dd.GetDeterminant()) / d0.GetDeterminant() / par3;
        }
        case LerpType.Sin: {
            //pars[0], pars[1] : 两相位的四分之一周期数
            if(pars.Length < 2) {
                throw new TargetParameterCountException("pars not enough");
            }
            float x1 = (float)(Math.PI / 2 * pars[0]), x2 = (float)(Math.PI / 2 * pars[1]), x = Lerp(x1, x2, val);
            float y1 = (float)Math.Sin(x1), y2 = (float)Math.Sin(x2), y = (float)Math.Sin(x);
            if((pars[0] - pars[1]) % 4 == 0 || (pars[0] + pars[1]) % 4 == 2) {
                return y - y1;
            }
            return (y - y1) / (y2 - y1);
        }
        case LerpType.Stay: {
            return 0;
        }
        }
        return val;
    }
    public static float Lerp(float left, float right, float val, bool clamped = false, LerpType type = LerpType.Linear, params float[] pars) {
        val = NewLerpValue(val, clamped, type, pars);
        return left * (1 - val) + right * val;
    }
    public static int Lerp(int left, int right, float val, bool clamped = false, LerpType type = LerpType.Linear, params float[] pars) {
        val = NewLerpValue(val, clamped, type, pars);
        return (int)(left * (1 - val) + right * val);
    }
    #endregion
    #region Clamp
    /*
    /// <summary>
    /// please make sure left is not greater than right, else use ClampS instead
    /// </summary>
    public static double Clamp(double val, double left, double right) => Math.Max(left, Math.Min(right, val));
    /// <summary>
    /// please make sure left is not greater than right, else use ClampS instead
    /// </summary>
    public static float Clamp(float val, float left, float right) => MathF.Max(left, MathF.Min(right, val));
    /// <summary>
    /// please make sure left is not greater than right, else use ClampS instead
    /// </summary>
    public static int Clamp(int val, int left, int right) => Math.Max(left, Math.Min(right, val));
    public static double ClampS(double val, double left, double right) => GetRight((left > right) ? (left, right) = (right, left) : null, Clamp(val, left, right));
    public static float ClampS(float val, float left, float right) => GetRight((left > right) ? (left, right) = (right, left) : null, Clamp(val, left, right));
    public static int ClampS(int val, int left, int right) => GetRight((left > right) ? (left, right) = (right, left) : null, Clamp(val, left, right));
    */
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 最好要保证<paramref name="left"/> 不大于 <paramref name="right"/>, 
    /// 否则最好使用<see cref="ClampToS"/>
    /// </summary>
    public static T Clamp<T>(T self, T left, T right) where T : IComparable
        => self.CompareTo(left) < 0 ? left : self.CompareTo(right) > 0 ? right : self;
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 最好要保证<paramref name="left"/> 不大于 <paramref name="right"/>, 
    /// 否则最好使用<see cref="ClampToS"/>
    /// </summary>
    public static ref T ClampTo<T>(ref T self, T left, T right) where T : IComparable
        => ref Assign(ref self, self.CompareTo(left) < 0 ? left : self.CompareTo(right) > 0 ? right : self);
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 自动判断<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    public static T ClampS<T>(T self, T left, T right) where T : IComparable
        => left.CompareTo(right) > 0 ? self.Clamp(right, left) : self.Clamp(left, right);
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 自动判断<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    public static ref T ClampToS<T>(ref T self, T left, T right) where T : IComparable
        => ref left.CompareTo(right) > 0 ? ref ClampTo(ref self, right, left) : ref ClampTo(ref self, left, right);
    #endregion
    #region IEnumerable拓展(包括Range)
    #region Range
    public enum RangeType {
        Positive,
        Negative,
        Automatic
    }
    public static IEnumerable<int> Range(int end, RangeType type = RangeType.Positive) {
        if(type == RangeType.Positive || type == RangeType.Automatic && end > 0) {
            for(int i = 0; i < end; ++i) {
                yield return i;
            }
        }
        else if(type == RangeType.Negative || type == RangeType.Automatic && end < 0) {
            for(int i = 0; i > end; --i) {
                yield return i;
            }
        }
    }
    public static IEnumerable<int> Range(int start, int end, RangeType type = RangeType.Positive) {
        if(type == RangeType.Positive || type == RangeType.Automatic && start < end) {
            for(int i = start; i < end; ++i) {
                yield return i;
            }
        }
        else if(type == RangeType.Negative || type == RangeType.Automatic && start > end) {
            for(int i = start; i > end; --i) {
                yield return i;
            }
        }
    }
    /// <summary>
    /// <paramref name="step"/>为0会按<see cref="Range(int, int, RangeType)"/>处理(自动模式)
    /// </summary>
    public static IEnumerable<int> Range(int start, int end, int step) {
        if(step == 0) {
            if(start < end) {
                for(int i = start; i < end; ++i) {
                    yield return i;
                }
            }
            else if(start > end) {
                for(int i = start; i > end; --i) {
                    yield return i;
                }
            }
        }
        else if(step > 0) {
            for(int i = start; i < end; i += step) {
                yield return i;
            }
        }
        else {
            for(int i = start; i > end; i += step) {
                yield return i;
            }
        }
    }
    /// <returns>(序号, 迭代值) 其中序号从0开始</returns>
    public static IEnumerable<(int, int)> RangeWithIndex(int end, RangeType type = RangeType.Positive) {
        if(type == RangeType.Positive || type == RangeType.Automatic && end > 0) {
            for(int i = 0; i < end; ++i) {
                yield return (i, i);
            }
        }
        else if(type == RangeType.Negative || type == RangeType.Automatic && end < 0) {
            for(int i = 0; i > end; --i) {
                yield return (-i, i);
            }
        }
    }
    /// <returns>(序号, 迭代值) 其中序号从0开始</returns>
    public static IEnumerable<(int, int)> RangeWithIndex(int start, int end, RangeType type = RangeType.Positive) {
        if(type == RangeType.Positive || type == RangeType.Automatic && start < end) {
            for(int i = start; i < end; ++i) {
                yield return (i - start, i);
            }
        }
        else if(type == RangeType.Negative || type == RangeType.Automatic && start > end) {
            for(int i = start; i > end; --i) {
                yield return (start - i, i);
            }
        }
    }
    /// <summary>
    /// <paramref name="step"/>为0会按<see cref="RangeWithIndex(int, int, RangeType)"/>处理(自动模式)
    /// </summary>
    /// <returns>(序号, 迭代值) 其中序号从0开始</returns>
    public static IEnumerable<(int, int)> RangeWithIndex(int start, int end, int step) {
        if(step == 0) {
            if(start < end) {
                for(int i = start; i < end; ++i) {
                    yield return (i - start, i);
                }
            }
            else if(start > end) {
                for(int i = start; i > end; --i) {
                    yield return (start - i, i);
                }
            }
        }
        else if(step > 0) {
            for(int i = start, index = 0; i < end; i += step, ++index) {
                yield return (index, i);
            }
        }
        else {
            for(int i = start, index = 0; i > end; i += step, ++index) {
                yield return (index, i);
            }
        }
    }
    #endregion
    public static bool ApplyOneToOne<T1, T2>(Func<IEnumerator<T1>>? getEnumerator1, Func<IEnumerator<T2>>? getEnumerator2, Func<T1, T2, bool> condition, Action<T1, T2> action) {
#if false
        //简单暴力待优化
        for(var left = getEnumerator1(); left.MoveNext();) {
            for(var right = getEnumerator2(); right.MoveNext();) {
                if(condition(left.Current, right.Current)) {
                    action(left.Current, right.Current);
                    break;
                }
            }
            return false;
        }
        return true;
#else
        if(getEnumerator1 == null) {
            return true;
        }
        if(getEnumerator2 == null) {
            return false;
        }
        var e1 = getEnumerator1();
        if(!e1.MoveNext()) {
            return true;
        }
        for(int failRounds = 1; failRounds < 2; ++failRounds) {

            for(var e2 = getEnumerator2(); e2.MoveNext();) {
                if(condition(e1.Current, e2.Current)) {
                    failRounds = 0;
                    action(e1.Current, e2.Current);
                    if(!e1.MoveNext()) {
                        return true;
                    }
                    break;
                }
            }
        }
        return false;
#endif
    }
    public static bool ApplyOneToOne<T1, T2>(IEnumerable<T1>? e1, IEnumerable<T2>? e2, Func<T1, T2, bool> condition, Action<T1, T2> action)
        => ApplyOneToOne(e1 == null ? null : e1.GetEnumerator, e2 == null ? null : e2.GetEnumerator, condition, action);
    #endregion
    #region Random
    public static class MyRandom {
        public static double RandomAverage(double min, double max, Random? rand = null) {
            if(min == max) {
                return min;
            }
            rand ??= new();
            return min + (max - min) * rand.NextDouble();
        }
        public static double RandomNormal(double μ, double σ, Random? rand = null)//产生正态分布随机数
        {
            rand ??= new();
            double r1 = rand.NextDouble();
            double r2 = rand.NextDouble();
            double standardNormal = Math.Sqrt(-2 * Math.Log(r1)) * Math.Sin(2 * Math.PI * r2);
            return standardNormal * σ + μ;
        }
        public static double RandomNormalRangeApproximate(double min, double max, double μ, double σ, Random? rand = null, double width = 3) {
            double value = RandomNormal(μ, σ, rand);
            return value.ClampWithTanh(min, max, width);
        }
        /// <summary>
        /// 拟正态分布(但完全不像)
        /// 置若罔闻, 不堪回首
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="μ">峰值</param>
        /// <param name="sharpness">尖锐度, 此值越大随机结果越集中, 为0时为平均分布</param>
        /// <param name="width">没事填3, 有事填5, 千万别填1</param>
        /// <returns></returns>
        public static double RandomDistribution(double min, double max, double μ, double sharpness, Random? rand = null, double width = 3) {
            if(sharpness == 0) {
                return RandomAverage(min, max, rand);
            }
            return RandomNormalRangeApproximate(min, max, μ, Math.Max(Math.Abs(min - μ), Math.Abs(max - μ)) / sharpness, rand);
        }
        public static void RandomDistrubutionTest(double μ, double sharpness, Random? rand = null, double width = 3) {
            rand ??= new();
            int[] bottles = new int[11];
            for(int i = 0; i < 10000; ++i) {
                bottles[(int)RandomDistribution(0, 10, μ, sharpness, rand, width)] += 1;
            }
            for(int i = 0; i < 11; ++i) {
                Console.WriteLine("{0,-2}: {1}", i, bottles[i]);
            }
        }
        public static double Normal(double x, double μ, double σ) //正态分布概率密度函数
        {
            return 1 / (Math.Sqrt(2 * Math.PI) * σ) * Math.Exp((μ - x) * (x - μ) / (2 * σ * σ));
        }
        /// <summary>
        /// 将double转化为int
        /// 其中小数部分按概率转化为0或1
        /// </summary>
        public static int RandomD2I(double x, Random rand) {
            int floor = (int)Math.Floor(x);
            double delta = x - floor;
            if(rand.NextDouble() < delta) {
                return floor + 1;
            }
            return floor;
        }
        /// <summary>
        /// 将double转化为bool
        /// 当大于1时为真, 小于0时为假
        /// 在中间则按概率
        /// </summary>
        /// <param name="x"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static bool RandomD2B(double x, Random rand) {
            return x > 1 - rand.NextDouble();
        }
    }
    public static T RerollIf<T>(Func<T> randomFunc, params Func<T, bool>[] conditions) {
        T t = randomFunc();
        foreach(var condition in conditions) {
            if(condition(t)) {
                t = randomFunc();
            }
        }
        return t;
    }
    public static T RerollIf<T>(Func<T> randomFunc, params bool[] conditions) {
        T t = randomFunc();
        foreach(var condition in conditions) {
            if(condition) {
                t = randomFunc();
            }
        }
        return t;
    }
    #endregion
    #region 一些数学运算(约等于和取模)
    /// <summary>
    /// 约等于
    /// 实际返回两者之差是否在<paramref name="tolerance"/>之内
    /// </summary>
    public static bool RoughEqual(float a, float b, float tolerance = .01f) {
        return MathF.Abs(a - b) <= tolerance;
    }
    #region 取模
    public enum ModularType {
        /// <summary>
        /// 返回非负数
        /// </summary>
        Possitive,
        /// <summary>
        /// 与除数的符号相同
        /// </summary>
        WithB,
        /// <summary>
        /// 与被除数的符号相同(也是%取余的模式)
        /// </summary>
        WithA,
        /// <summary>
        /// 返回非正数
        /// </summary>
        Negative,
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static int Modular(int a, int b, ModularType type = ModularType.Possitive) {
        int result = a % b;
        return type switch {
            ModularType.Possitive => result < 0 ? result + Math.Abs(b) : result,
            ModularType.WithB => (result ^ b) < 0 ? result + b : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? result - Math.Abs(b) : result,
            _ => result,
        };
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static long Modular(long a, long b, ModularType type = ModularType.Possitive) {
        long result = a % b;
        return type switch {
            ModularType.Possitive => result < 0 ? result + Math.Abs(b) : result,
            ModularType.WithB => (result ^ b) < 0 ? result + b : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? result - Math.Abs(b) : result,
            _ => result,
        };
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static short Modular(short a, short b, ModularType type = ModularType.Possitive) {
        short result = (short)(a % b);
        return type switch {
            ModularType.Possitive => result < 0 ? (short)(result + Math.Abs(b)) : result,
            ModularType.WithB => (result ^ b) < 0 ? (short)(result + b) : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? (short)(result - Math.Abs(b)) : result,
            _ => result,
        };
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static sbyte Modular(sbyte a, sbyte b, ModularType type = ModularType.Possitive) {
        sbyte result = (sbyte)(a % b);
        return type switch {
            ModularType.Possitive => result < 0 ? (sbyte)(result + Math.Abs(b)) : result,
            ModularType.WithB => (result ^ b) < 0 ? (sbyte)(result + b) : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? (sbyte)(result - Math.Abs(b)) : result,
            _ => result,
        };
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static float Modular(float a, float b, ModularType type = ModularType.Possitive) {
        float result = a % b;
        return type switch {
            ModularType.Possitive => result < 0 ? result + Math.Abs(b) : result,
            ModularType.WithB => result * b < 0 ? result + b : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? result - Math.Abs(b) : result,
            _ => result,
        };
    }
    /// <summary>
    /// 取余, 默认为返回非负数
    /// </summary>
    public static double Modular(double a, double b, ModularType type = ModularType.Possitive) {
        double result = a % b;
        return type switch {
            ModularType.Possitive => result < 0 ? result + Math.Abs(b) : result,
            ModularType.WithB => result * b < 0 ? result + b : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? result - Math.Abs(b) : result,
            _ => result,
        };
    }
    //byte, ushort, uint, ulong 就直接用 % 就可以了, 也不用担心符号问题
    public static decimal Modular(decimal a, decimal b, ModularType type = ModularType.Possitive) {
        decimal result = a % b;
        return type switch {
            ModularType.Possitive => result < 0 ? result + Math.Abs(b) : result,
            ModularType.WithB => result * b < 0 ? result + b : result,
            ModularType.WithA => result,
            ModularType.Negative => result > 0 ? result - Math.Abs(b) : result,
            _ => result,
        };
    }
    #endregion
    #endregion
    #region Min / Max带有多个值
    public static T Min<T>(T a, T b, params T[] others) where T : IComparable<T> {
        T result = a;
        if(result.CompareTo(b) > 0) {
            result = b;
        }
        foreach(T other in others) {
            if(result.CompareTo(other) > 0) {
                result = other;
            }
        }
        return result;
    }
    public static T Max<T>(T a, T b, params T[] others) where T : IComparable<T> {
        T result = a;
        if(result.CompareTo(b) < 0) {
            result = b;
        }
        foreach(T other in others) {
            if(result.CompareTo(other) < 0) {
                result = other;
            }
        }
        return result;
    }
    #endregion
    #region 流程简化
#pragma warning disable IDE0060 // 删除未使用的参数
    /// <summary>
    /// 什么也不做, 返回false
    /// </summary>
    public static bool Do(object expression) => false;
    /// <summary>
    /// 执行<paramref name="action"/>
    /// </summary>
    /// <returns>false</returns>
    public static bool Do(Action action) => GetRight(action, false);
    /// <summary>
    /// 获得<paramref name="action"/>的返回值
    /// </summary>
    public static T Get<T>(Func<T> supplier) => supplier();
    public static bool Get<T>(Func<T>? supplier, out T? value) {
        if(supplier != null) {
            value = supplier();
            return true;
        }
        value = default;
        return false;
    }
    /// <summary>
    /// 什么也不做
    /// </summary>
    /// <returns>false</returns>
    public static bool Dos(params object[] objs) => false;
    /// <summary>
    /// 若其中有Action, 则自动执行
    /// </summary>
    /// <returns>false</returns>
    public static bool DosS(params object[] objs) => GetRight(objs.ForeachDo(o => Do(o is Action action && Do(action))), false);
    public static T? GetRight<T>(object? left, T? right) => right;
    public static T? GetRight<T>(Action left, T? right) {
        left();
        return right;
    }
#pragma warning restore IDE0060 // 删除未使用的参数
    #region 流程控制 - 条件
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="action"/>.
    /// </summary>
    /// <returns>返回<paramref name="condition"/></returns>
    public static bool DoIf(bool condition, Action action) {
        if(condition) {
            action.Invoke();
        }
        return condition;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为假则调用<paramref name="action"/>.
    /// 相当与DoIf(!<paramref name="condition"/>, <paramref name="action"/>)
    /// </summary>
    /// <returns>返回!<paramref name="condition"/></returns>
    public static bool DoIfNot(bool condition, Action action) {
        if(!condition) {
            action.Invoke();
        }
        return !condition;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="action"/>, 否则调用<paramref name="altAction"/>.
    /// <para/>若为表达式推荐使用<see cref="Do"/>配合三目运算符
    /// </summary>
    /// <returns>返回<paramref name="condition"/></returns>
    public static bool DoIfElse(bool condition, Action action, Action altAction) {
        if(condition) {
            action.Invoke();
        }
        else {
            altAction.Invoke();
        }

        return condition;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="action"/>, 否则调用<paramref name="altAction"/>.
    /// <para/>若为表达式推荐使用<see cref="Do"/>配合三目运算符
    /// <para/>相当与<see cref="DoIfElse"/>
    /// </summary>
    /// <returns>返回<paramref name="condition"/></returns>
    public static bool DoIf(bool condition, Action action, Action altAction) {
        if(condition) {
            action.Invoke();
        }
        else {
            altAction.Invoke();
        }

        return condition;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="actions"/>中的第一项,
    /// 若第一项返回真则调用第二项...直到有任意一项返回假或者全部执行完
    /// </summary>
    /// <returns>若有任意一项返回假则是假(包含最后一项), 只有全部都返回真才是真</returns>
    public static bool DoIfElseIf(bool condition, params Func<bool>[] actions) {
        if(!condition) {
            return false;
        }
        foreach(var action in actions) {
            if(!action()) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="actions"/>中的第一项,
    /// 若第一项返回真则调用第二项...直到有任意一项返回假或者全部执行完
    /// <para/>相当于<see cref="DoIfElseIf"/>
    /// </summary>
    /// <returns>若有任意一项返回假则是假(包含最后一项), 只有全部都返回真才是真</returns>
    public static bool DoIf(bool condition, params Func<bool>[] actions) {
        if(!condition) {
            return false;
        }
        foreach(var action in actions) {
            if(!action()) {
                return false;
            }
        }
        return true;
    }
    public static T? GetIf<T>(bool condition, Func<T?> supplier, T? defaultValue = default)
        => condition ? supplier() : defaultValue;
    public static bool GetIf<T>(bool condition, Func<T?> supplier, out T? value, T? defaultValue = default) {
        if(condition) {
            value = supplier();
            return true;
        }
        value = defaultValue;
        return false;
    }
    public static T? GetIfNot<T>(bool condition, Func<T?> supplier, T? defaultValue = default)
        => condition ? default : supplier();
    public static bool GetIfNot<T>(bool condition, Func<T?> supplier, out T? value, T? defaultValue = default) {
        if(condition) {
            value = defaultValue;
            return false;
        }
        value = supplier();
        return true;
    }
    public static T GetIfElse<T>(bool condition, Func<T> supplier, Func<T> altSupplier)
        => condition ? supplier() : altSupplier();
    public static bool GetIfElse<T>(bool condition, Func<T> supplier, Func<T> altSupplier, out T value) {
        value = condition ? supplier() : altSupplier();
        return condition;
    }
    public static T GetIf<T>(bool condition, Func<T> supplier, Func<T> altSupplier)
        => condition ? supplier() : altSupplier();
    public static bool GetIf<T>(bool condition, Func<T> supplier, Func<T> altSupplier, out T value) {
        value = condition ? supplier() : altSupplier();
        return condition;
    }
    #endregion
    #region 流程控制 - 循环
    /// <summary>
    /// returns false when action or condition is null, else returns true.
    /// would still do action once when condition is null but action is not
    /// </summary>
    public static bool DoWhile(Action action, Func<bool> condition) {
        if(condition == null) {
            action?.Invoke();
            return false;
        }
        if(action == null) {
            return false;
        }
        do {
            action();
        }
        while(condition());
        return true;
    }
    /// <summary>
    /// if break out, returns true, else returns false.
    /// would still do action once and try break out when condition is null but action is not
    /// </summary>
    /// <param name="action">when get true, breaks out</param>
    public static bool DoWhile(Func<bool> action, Func<bool> condition) {
        if(condition == null) {
            if(action?.Invoke() == true) {
                return true;
            }
            return false;
        }
        if(action == null) {
            return false;
        }
        do {
            if(action()) {
                return true;
            }
        }
        while(condition());
        return false;
    }
    /// <summary>
    /// alwayss return false
    /// </summary>
    public static bool WhileDo(Func<bool> condition, Action action) {
        while(condition()) {
            action();
        }
        return false;
    }
    /// <summary>
    /// returns true when break out, else returns false
    /// </summary>
    /// <param name="action">breaks out when get true</param>
    public static bool WhileDo(Func<bool> condition, Func<bool> action) {
        while(condition()) {
            if(action()) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// always return false
    /// </summary>
    public static bool ForDo(Action? init, Func<bool>? condition, Action? iter, Action? action) {
        init?.Invoke();
        while(condition?.Invoke() != false) {
            action?.Invoke();
            iter?.Invoke();
        }
        return false;
    }
    /// <summary>
    /// returns true when break out, else returns false
    /// </summary>
    /// <param name="action">breaks out when get true</param>
    public static bool ForDo(Action? init, Func<bool>? condition, Action? iter, Func<bool>? action) {
        init?.Invoke();
        while(condition?.Invoke() != false) {
            if(action?.Invoke() == true) {
                return true;
            }
            iter?.Invoke();
        }
        return false;
    }
    /// <summary>
    /// always return false
    /// </summary>
    public static bool ForeachDo<T>(IEnumerable<T> enumerable, Action<T> action) {
        foreach(T t in enumerable) {
            action(t);
        }
        return false;
    }
    /// <summary>
    /// returns true when break out, else returns false
    /// </summary>
    /// <param name="action">breaks out when get true</param>
    public static bool ForeachDo<T>(IEnumerable<T> enumerable, Func<T, bool> action) {
        foreach(T t in enumerable) {
            if(action(t)) {
                return true;
            }
        }
        return false;
    }
    public static T? WhileGet<T>(Func<bool> condition, Func<(bool succeeded, T value)> supplier, T? defaultValue = default) {
        while(condition()) {
            var (succeeded, value) = supplier();
            if(succeeded) {
                return value;
            }
        }
        return defaultValue;
    }
    public static bool WhileGet<T>(Func<bool> condition, Func<(bool succeeded, T value)> supplier, out T? value, T? defaultValue = default) {
        while(condition()) {
            var (succeeded, getValue) = supplier();
            if(succeeded) {
                value = getValue;
                return true;
            }
        }
        value = defaultValue;
        return false;
    }
    //foreach部分挪到TigerExtensions中 IEnumerable拓展 的 Foreach 区域了
    #endregion
    #region ref相关
    //ref拓展不知道为什么只能给值类型用, 但若不用拓展就可以
    /// <summary>
    /// 对<paramref name="self"/>执行<paramref name="action"/>
    /// </summary>
    /// <returns><paramref name="self"/>的引用</returns>
    public static ref T Do<T>(ref T self, Action<T> action) {
        action(self);
        return ref self;
    }
    /// <summary>
    /// 将<paramref name="other"/>的值赋给<paramref name="self"/>
    /// </summary>
    /// <returns><paramref name="self"/>的引用</returns>
    public static ref T Assign<T>(ref T self, T other) {
        self = other;
        return ref self;
    }
    #endregion
    #endregion
    #region 杂项
    public static void Swap<T>(ref T left, ref T right) => (left, right) = (right, left);
    public class ValueHolder<T> {
        public T? value;
    }
    /// <summary>
    /// 空类, 用以做标识
    /// </summary>
    public class Identifier { }
    #endregion
}

static public partial class TigerExtensions {

    #region Lua的 And / Or 体系
    /// <summary>
    /// 若<paramref name="i"/>判定为真则返回<paramref name="i"/>, 否则返回<paramref name="o"/>
    /// </summary>
    public static T LuaOr<T>(this T i, T o) {
        if(Convert.ToBoolean(i)) {
            return i;
        }
        return o;
    }
    /// <summary>
    /// 若<paramref name="i"/>判定为假则返回<paramref name="i"/>, 否则返回<paramref name="o"/>
    /// </summary>
    public static T LuaAnd<T>(this T i, T o) {
        if(!Convert.ToBoolean(i)) {
            return i;
        }
        return o;
    }
    /// <summary>
    /// 若i判定为假则将o赋值给i
    /// 对于引用类型, 一般相当于 ??=
    /// </summary>
    public static T LuaOrAssignFrom<T>(this ref T i, T o) where T : struct {
        if(!Convert.ToBoolean(i)) {
            i = o;
        }
        return i;
    }
    /// <summary>
    /// 若i判定为假则将o赋值给i
    /// 对于引用类型, 一般相当于 ??=
    /// </summary>
    public static T LuaOrAssignFrom<T>(this T i, T o) where T : class {
        if(!Convert.ToBoolean(i)) {
            i = o;
        }
        return i;
    }
    /// <summary>
    /// 若i判定为假则将o赋值给i
    /// </summary>
    public static T LuaAndAssignFrom<T>(this ref T i, T o) where T : struct {
        if(Convert.ToBoolean(i)) {
            i = o;
        }
        return i;
    }
    /// <summary>
    /// 若i判定为假则将o赋值给i
    /// </summary>
    public static T LuaAndAssignFrom<T>(this T i, T o) where T : class {
        if(Convert.ToBoolean(i)) {
            i = o;
        }
        return i;
    }
    #endregion
    #region Clamp
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 最好要保证<paramref name="left"/> 不大于 <paramref name="right"/>, 
    /// 否则最好使用<see cref="ClampToS"/>
    /// </summary>
    public static T Clamp<T>(this T self, T left, T right) where T : IComparable
        => self.CompareTo(left) < 0 ? left : self.CompareTo(right) > 0 ? right : self;
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 最好要保证<paramref name="left"/> 不大于 <paramref name="right"/>, 
    /// 否则最好使用<see cref="ClampToS"/>
    /// </summary>
    public static ref T ClampTo<T>(ref this T self, T left, T right) where T : struct, IComparable
        => ref self.Assign(self.CompareTo(left) < 0 ? left : self.CompareTo(right) > 0 ? right : self);
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 自动判断<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    public static T ClampS<T>(this T self, T left, T right) where T : IComparable
        => left.CompareTo(right) > 0 ? self.Clamp(right, left) : self.Clamp(left, right);
    /// <summary>
    /// 得到自身被限制在<paramref name="left"/>和<paramref name="right"/>之间的大小
    /// 自动判断<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    public static ref T ClampToS<T>(ref this T self, T left, T right) where T : struct, IComparable
        => ref left.CompareTo(right) > 0 ? ref self.ClampTo(right, left) : ref self.ClampTo(left, right);
    public static T ClampMin<T>(this T self, T min) where T : IComparable
        => self.CompareTo(min) < 0 ? min : self;
    public static ref T ClampMinTo<T>(ref this T self, T min) where T : struct, IComparable
        => ref self.CompareTo(min) > 0 ? ref self : ref self.Assign(min);
    public static T ClampMax<T>(this T self, T max) where T : IComparable
        => self.CompareTo(max) < 0 ? max : self;
    public static ref T ClampMaxTo<T>(ref this T self, T max) where T : struct, IComparable
        => ref self.CompareTo(max) > 0 ? ref self : ref self.Assign(max);
    /// <summary>
    /// 比较平缓的Clamp方式, 当<paramref name="self"/>在<paramref name="left"/>和<paramref name="right"/>正中间时不变
    /// 在两边时会逐渐趋向两边的值, 但不会达到
    /// 不需要注意<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    /// <param name="width">
    /// 代表变化的缓度, 为1时当<paramref name="self"/>到达<paramref name="left"/>或<paramref name="right"/>时,
    /// 实际得到的值还差25%左右, 当此值越小, 相差的值越小
    /// <para/>与<paramref name="self"/>在<paramref name="left"/>和<paramref name="right"/>正中间的斜率互为倒数
    /// </param>
    public static double ClampWithTanh(this double self, double left, double right, double width = 1) {
        if(left == right) {
            return left;
        }
        double halfDelta = (right -  left) / 2;
        double middle = left + halfDelta;
        return middle + halfDelta * Math.Tanh((self - middle) / halfDelta / width);
    }
    /// <summary>
    /// 比较平缓的Clamp方式, 当<paramref name="self"/>在<paramref name="left"/>和<paramref name="right"/>正中间时不变
    /// 在两边时会逐渐趋向两边的值, 但不会达到
    /// 不需要注意<paramref name="left"/>和<paramref name="right"/>的大小关系
    /// </summary>
    /// <param name="width">
    /// 代表变化的缓度, 为1时当<paramref name="self"/>到达<paramref name="left"/>或<paramref name="right"/>时,
    /// 实际得到的值还差25%左右, 当此值越小, 相差的值越小
    /// <para/>与<paramref name="self"/>在<paramref name="left"/>和<paramref name="right"/>正中间的斜率互为倒数
    /// </param>
    public static ref double ClampWithTanhTo(ref this double self, double left, double right, double width) {
        if(left == right) {
            self = left;
            return ref self;
        }
        double halfDelta = (right -  left) / 2;
        double middle = left + halfDelta;
        self = middle + halfDelta * Math.Tanh((self - middle) / halfDelta / width);
        return ref self;
    }
    #endregion
    #region With Min / Max
    public static T WithMin<T>(this T self, T min) where T : IComparable<T> => self.CompareTo(min) < 0 ? min : self;
    public static T WithMax<T>(this T self, T max) where T : IComparable<T> => self.CompareTo(max) > 0 ? max : self;
    #endregion
    #region 一些数学运算(统计学取整)
    /// <summary>
    /// 统计学取整
    /// (四舍六入五成双)
    /// 其实<see cref="Math.Round"/>默认就是这个, 只是想自己写一个
    /// </summary>
    public static int StatisticalRound(this double x) {
        int ret = (int)x;
        double delta = x - ret;
        if(delta <= -1) {
            return int.MinValue;
        }
        else if(delta >= 1) {
            return int.MaxValue;
        }
        else if(-0.5 < delta && delta < 0.5) {
            return ret;
        }
        else if(delta > 0.5) {
            return ret + 1;
        }
        else if(delta < -0.5) {
            return ret - 1;
        }
        else if(ret % 2 == 0) {
            return ret;
        }
        else if(delta == 0.5) {
            return ret + 1;
        }
        else {
            return ret - 1;
        }
    }
    #endregion

    #region 反射
    /// <summary>
    /// 常用flags:
    /// <see cref="BindingFlags.Public"/>
    /// <see cref="BindingFlags.NonPublic"/>
    /// <see cref="BindingFlags.Instance"/>
    /// <see cref="BindingFlags.Static"/>
    /// </summary>
    public static object GetField(this object self, string fieldName, BindingFlags flags)
        => self.GetType().GetField(fieldName, flags).GetValue(self);
    public static T GetField<T>(this object self, string fieldName, BindingFlags flags)
        => (T)self.GetType().GetField(fieldName, flags).GetValue(self);
    public static void GetField<T>(this object self, out T field, string fieldName, BindingFlags flags)
        => field = (T)self.GetType().GetField(fieldName, flags).GetValue(self);
    public static object GetField(this object self, FieldInfo fieldInfo)
        => fieldInfo.GetValue(self);
    public static T GetField<T>(this object self, FieldInfo fieldInfo)
        => (T)fieldInfo.GetValue(self);
    public static void GetField<T>(this object self, out T field, FieldInfo fieldInfo)
        => field = (T)fieldInfo.GetValue(self);

    public static void SetField(this object self, string fieldName, BindingFlags flags, object value)
        => self.GetType().GetField(fieldName, flags).SetValue(self, value);
    public static void SetField(this object self, FieldInfo fieldInfo, object value)
        => fieldInfo.SetValue(self, value);

    public static object InvokeMethod(this object self, string methodName, BindingFlags flags, params object[] parameters)
        => self.GetType().GetMethod(methodName, flags)?.Invoke(self, parameters);
    public static T InvokeMethod<T>(this object self, string methodName, BindingFlags flags, params object[] parameters)
        => (T)self.GetType().GetMethod(methodName, flags)?.Invoke(self, parameters);
    public static object InvokeMethod(this object self, MethodInfo methodInfo, params object[] parameters)
        => methodInfo.Invoke(self, parameters);
    public static T InvokeMethod<T>(this object self, MethodInfo methodInfo, params object[] parameters)
        => (T)methodInfo.Invoke(self, parameters);

    public static FieldInfo GetFieldInfo(this object self, string fieldName, BindingFlags flags)
        => self.GetType().GetField(fieldName, flags);
    public static FieldInfo GetFieldInfo<T>(string fieldName, BindingFlags flags)
        => typeof(T).GetField(fieldName, flags);

    public static MethodInfo GetMethodInfo(this object self, string methodName, BindingFlags flags)
        => self.GetType().GetMethod(methodName, flags);
    public static MethodInfo GetMethodInfo<T>(string methodName, BindingFlags flags)
        => typeof(T).GetMethod(methodName, flags);
    #endregion

    #region IEnumerable拓展
    #region Foreach
    /// <summary>
    /// returns false when action or condition is null, else returns true
    /// </summary>
    public static bool ForeachDo<T>(this IEnumerable<T> enumerable, Action<T> action) {
        if(enumerable == null || action == null) {
            return false;
        }
        foreach(T t in enumerable) {
            action(t);
        }
        return true;
    }
    /// <summary>
    /// returns true when break out, else returns false
    /// same as <see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
    /// </summary>
    /// <param name="action">break out when get true</param>
    public static bool ForeachDo<T>(this IEnumerable<T> enumerable, Func<T, bool> action) {
        if(enumerable == null || action == null) {
            return false;
        }
        foreach(T t in enumerable) {
            if(action(t)) {
                return true;
            }
        }
        return false;
    }
    public static TResult? ForeachGet<TSource, TResult>(IEnumerable<TSource> enumerable, Func<TSource, (bool succeeded, TResult value)> supplier, TResult? defaultValue = default) {
        foreach(TSource t in enumerable) {
            var (succeeded, value) = supplier(t);
            if(succeeded) {
                return value;
            }
        }
        return defaultValue;
    }
    public static bool ForeachGet<TSource, TResult>(IEnumerable<TSource> enumerable, Func<TSource, (bool succeeded, TResult value)> supplier, out TResult? value, TResult? defaultValue = default) {
        foreach(TSource t in enumerable) {
            var (succeeded, getValue) = supplier(t);
            if(succeeded) {
                value = getValue;
                return true;
            }
        }
        value = defaultValue;
        return false;
    }
    #endregion
    #region out Exception
    public static IEnumerable<(T, Exception?)> WithException<T>(this IEnumerable<T> enumerable) {
        foreach(T t in enumerable) {
            yield return (t, null);
        }
    }
    public delegate TResult ConverterWithException<TSource, TResult>(TSource source, out Exception exception);
    public delegate bool PredicateWithException<T>(T source, out Exception exception);
    public delegate void ActionWithException<T>(T source, out Exception exception);
    public static IEnumerable<(TResult?, Exception?)> Select<TSource, TResult>(this IEnumerable<(TSource, Exception)> source, ConverterWithException<TSource, TResult> selector) {
        foreach((TSource element, Exception e) in source) {
            if(e != null) {
                yield return (default, e);
                yield break;
            }
            TResult result = selector(element, out Exception exception);
            if(exception != null) {
                yield return (default, exception);
                yield break;
            }
            yield return (result, null);
        }
    }
    public static bool Any<TSource>(this IEnumerable<(TSource, Exception)> source, PredicateWithException<TSource> predicate, out Exception? exception) {
        exception = null;
        foreach((TSource element, Exception e) in source) {
            if(e != null) {
                exception = e;
                return false;
            }
            bool result = predicate(element, out exception);
            if(exception != null) {
                return false;
            }
            if(predicate(element, out exception)) {
                return true;
            }
        }

        return false;
    }
    public static List<TSource>? ToList<TSource>(this IEnumerable<(TSource, Exception)> source, out Exception? exception) {
        exception = null;
        List<TSource> list = new();
        foreach((TSource element, Exception e) in source) {
            if(e != null) {
                return null;
            }
            list.Add(element);
        }
        return list;
    }
    public static TSource[]? ToArray<TSource>(this IEnumerable<(TSource, Exception)> source, out Exception? exception) {
        var list = source.ToList(out exception);
        if(exception != null) {
            return null;
        }
        return list?.ToArray();
    }
    public static List<TResult>? ConvertAll<TSource, TResult>(this List<TSource> source, ConverterWithException<TSource, TResult> converter, out Exception? e) {
        e = null;
        List<TResult> list = new(source.Count);
        for(int i = 0; i < source.Count; i++) {
            TResult element = converter(source[i], out e);
            if(e != null)
                return default;
            list.Add(element);
        }
        return list;
    }
    public static T? Find<T>(this IEnumerable<(T, Exception)> source, PredicateWithException<T> match, out Exception? exception) {
        exception = null;
        foreach((T element, Exception e) in source) {
            if(e != null) {
                exception = e;
                return default;
            }
            bool result = match(element, out exception);
            if(exception != null)
                return default;
            if(result)
                return element;
        }
        return default;
    }
    public static void ForEach<T>(this IEnumerable<(T, Exception)> source, ActionWithException<T> action, out Exception? exception) {
        exception = null;
        foreach((T element, Exception e) in source) {
            if(e != null) {
                exception = e;
                return;
            }
            action(element, out exception);
            if(exception != null)
                return;
        }
    }
    #endregion
    /// <returns>(序号, 迭代值) 其中序号从0开始</returns>
    public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> enumerable) {
        int index = 0;
        foreach(T t in enumerable) {
            yield return (index++, t);
        }
    }
    public static T? Random<T>(this IEnumerable<T> enumerable, Random? rand = null) {
        int length = enumerable.Count();
        if(length == 0) {
            return default;
        }
        return enumerable.ElementAt((rand ?? new()).Next(length));
    }
    public static T? Random<T>(this IEnumerable<T> enumerable, out bool succeed, Random? rand = null) {
        int length = enumerable.Count();
        if(length == 0) {
            succeed = false;
            return default;
        }
        succeed = true;
        return enumerable.ElementAt((rand ?? new()).Next(length));
    }
    public static T? Random<T>(this IEnumerable<T> enumerable, Func<T, decimal> weight, Random? rand = null, bool uncheckNegative = false) {
        decimal w = default;
        if(uncheckNegative) {
            decimal totalWeight = enumerable.Sum(t => weight(t));
            decimal randDecimal = (decimal)(rand ?? new()).NextDouble() * totalWeight;
            return enumerable.FirstOrDefault(t => GetRight(w = weight(t), w < randDecimal || TigerUtils.Do(totalWeight -= w)));
        }
        else {
            decimal totalWeight = enumerable.Sum(t => weight(t).WithMin(0M));
            decimal randDecimal = (decimal)(rand ?? new()).NextDouble() * totalWeight;
            return totalWeight <= 0 ? default : enumerable.FirstOrDefault(t => GetRight(w = weight(t).WithMin(0M), w < randDecimal || TigerUtils.Do(totalWeight -= w)));
        }

    }
    public static T? Random<T>(this IEnumerable<T> enumerable, Func<T, float> weight, Random? rand = null, bool uncheckNegative = false) {
        float w = default;
        if(uncheckNegative) {
            float totalWeight = enumerable.Sum(t => weight(t));
            float randf = (rand ?? new()).NextSingle() * totalWeight;
            return enumerable.FirstOrDefault(t => GetRight(w = weight(t), w >= randf || TigerUtils.Do(totalWeight -= w)));
        }
        else {
            float totalWeight = enumerable.Sum(t => weight(t).WithMin(0f));
            float randf = (rand ?? new()).NextSingle() * totalWeight;
            //if (totalWeight <= 0)
            //{
            //    return default;
            //}
            //foreach (T t in enumerable)
            //{
            //    w = weight(t).WithMin(0f);
            //    if (w >= randf)
            //    {
            //        return t;
            //    }
            //    randf -= w;
            //}
            //return default;
            return totalWeight <= 0 ? default : enumerable.FirstOrDefault(t => GetRight(w = weight(t).WithMin(0f), w >= randf || TigerUtils.Do(randf -= w)));
        }

    }
    public static T? Random<T>(this IEnumerable<T> enumerable, Func<T, int> weight, Random? rand = null, bool uncheckNegative = false) {
        int w = default;
        if(uncheckNegative) {
            int totalWeight = enumerable.Sum(t => weight(t));
            int randi = (rand ?? new()).Next(totalWeight);
            return enumerable.FirstOrDefault(t => GetRight(w = weight(t), w < randi || TigerUtils.Do(totalWeight -= w)));
        }
        else {
            int totalWeight = enumerable.Sum(t => weight(t).WithMin(0));
            int randi = (rand ?? new()).Next(totalWeight);
            return totalWeight <= 0 ? default : enumerable.FirstOrDefault(t => GetRight(w = weight(t).WithMin(0), w < randi || TigerUtils.Do(totalWeight -= w)));
        }

    }
    #endregion
    #region 数组和列表相关
    #region 打乱数组/列表
    /// <summary>
    /// 直接在此列表上打乱整个列表
    /// </summary>
    public static List<T> Shuffle<T>(this List<T> list, Random rd = null) {
        T tmp;
        if(list == null || list.Count == 0) {
            return list;
        }
        rd ??= new();
        foreach(int i in Range(list.Count - 1, 0, RangeType.Negative)) {
            int randint = rd.Next(0, i + 1);
            tmp = list[randint];
            list[randint] = list[i];
            list[i] = tmp;
        }
        return list;
    }
    /// <summary>
    /// 直接在此数组上打乱整个数组
    /// </summary>
    public static T[] Shuffle<T>(this T[] array, Random rd = null) {
        T tmp;
        rd ??= new();
        foreach(int i in Range(array.Length - 1, 0, RangeType.Negative)) {
            int randint = rd.Next(0, i + 1);
            tmp = array[randint];
            array[randint] = array[i];
            array[i] = tmp;
        }
        return array;
    }
    /// <summary>
    /// 返回一个打乱了的列表, 原列表不变
    /// </summary>
    public static List<T> Shuffled<T>(this List<T> list) where T : ICloneable {
        List<T> ret = new();
        foreach(T t in list) {
            ret.Add((T)t.Clone());
        }
        return ret.Shuffle();
    }
    /// <summary>
    /// 返回一个打乱了的数组, 原数组不变
    /// </summary>
    public static T[] Shuffled<T>(this T[] array) where T : ICloneable {
        T[] ret = new T[array.Length];
        foreach(int i in Range(array.Length)) {
            ret[i] = (T)array.Clone();
        }
        return ret.Shuffle();
    }
    #endregion
    #region IList的Index和Range拓展
    private static int GetIndex<T>(IList<T> list, Index index) {
        return index.IsFromEnd ? list.Count - index.Value : index.Value;
    }
    private static void GetRange<T>(IList<T> list, Range range, out int start, out int end) {
        start = GetIndex(list, range.Start);
        end = GetIndex(list, range.End);
        if(start > end) {
            start ^= end;
            end ^= start;
            start ^= end;
        }
    }
    public static void RemoveAt<T>(this IList<T> list, Index index) {
        list.RemoveAt(GetIndex(list, index));
    }
    public static void RemoveRange<T>(this List<T> list, Range range) {
        GetRange(list, range, out int start, out int end);
        list.RemoveRange(start, end - start);
    }
    #endregion
    public static bool AddIf<T>(this List<T> list, bool condition, T element) {
        if(condition) {
            list.Add(element);
        }
        return condition;
    }
    public static void ClampLength<T>(this List<T> list, int length) {
        if(list.Count <= length) {
            return;
        }
        list.RemoveRange(length, list.Count - length);
    }
    #region Get<T>
    public static T? Get<T>(this object[] array, int index) => index >= array.Length ? default : (T)array[index];
    public static T? Get<T>(this object[] array, int index, out T? value) => value = index >= array.Length ? default : (T)array[index];
    #endregion
    #endregion
    #region ref相关拓展
    //ref拓展不知道为什么只能给值类型用
    /// <summary>
    /// 对<paramref name="self"/>执行<paramref name="action"/>
    /// </summary>
    /// <returns><paramref name="self"/>的引用</returns>
    public static ref T Do<T>(ref this T self, Action<T> action) where T : struct {
        action(self);
        return ref self;
    }
    /// <summary>
    /// 将<paramref name="other"/>的值赋给<paramref name="self"/>
    /// </summary>
    /// <returns><paramref name="self"/>的引用</returns>
    public static ref T Assign<T>(ref this T self, T other) where T : struct {
        self = other;
        return ref self;
    }
    #endregion
    #region BinaryWriter/Reader 拓展
    //渣, 不要用, 没测试过, 用了概不负责
    /// <summary>
    /// 支持类型: 原生, Color, Vector2, 及其构成的数组或列表或字典
    /// (<see cref="List{T}"/>, <see cref="Dictionary{TKey, TValue}"/>)
    /// </summary>
    public static void WriteObj<T>(this BinaryWriter bw, T obj) {
        if(obj is ulong @ulong) { bw.Write(@ulong); }
        else if(obj is uint @uint) { bw.Write(@uint); }
        else if(obj is ushort @ushort) { bw.Write(@ushort); }
        else if(obj is string @string) { bw.Write(@string); }
        else if(obj is float @float) { bw.Write(@float); }
        else if(obj is sbyte @sbyte) { bw.Write(@sbyte); }
        else if(obj is long @long) { bw.Write(@long); }
        else if(obj is int @int) { bw.Write(@int); }
        else if(obj is Half @Half) { bw.Write(@Half); }
        else if(obj is double @double) { bw.Write(@double); }
        else if(obj is decimal @decimal) { bw.Write(@decimal); }
        else if(obj is char @char) { bw.Write(@char); }
        else if(obj is byte @byte) { bw.Write(@byte); }
        else if(obj is bool @bool) { bw.Write(@bool); }
        else if(obj is short @short) { bw.Write(@short); }
        else if(obj is byte[] buffer) { bw.Write(buffer.Length); bw.Write(buffer); }
        else if(obj is char[] chars) { bw.Write(chars.Length); bw.Write(chars); }
        else if(obj is Color @color) { bw.WriteRGB(@color); }
        else if(obj is Vector2 @vector2) { bw.WritePackedVector2(@vector2); }
        else if(obj is object[] array) { bw.Write(array.Length); foreach(int i in Range(array.Length)) { bw.WriteObj(array[i]); } }
        else if(obj is List<object> list) { bw.Write(list.Count); foreach(int i in Range(list.Count)) { bw.WriteObj(list[i]); } }
        else if(obj is Dictionary<object, object> dict) { bw.Write(dict.Count); foreach(var pair in dict) { bw.WriteObj(pair.Key); bw.WriteObj(pair.Value); } }
        else
            throw new Exception("type not suppoerted for type " + obj.GetType().ToString());
    }
    public static void WriteArray<T>(this BinaryWriter bw, T[] array) {
        bw.Write(array.Length);
        foreach(int i in Range(array.Length)) {
            bw.WriteObj(array[i]);
        }
    }
    public static void WriteList<T>(this BinaryWriter bw, List<T> array) {
        bw.Write(array.Count);
        foreach(int i in Range(array.Count)) {
            bw.WriteObj(array[i]);
        }
    }
    public static void WriteDict<TKey, TValue>(this BinaryWriter bw, Dictionary<TKey, TValue> dict) {
        bw.Write(dict.Count);
        foreach(var pair in dict) {
            bw.WriteObj(pair.Key);
            bw.WriteObj(pair.Value);
        }
    }
    /// <summary>
    /// 支持类型: 原生, Color, Vector2
    /// </summary>
    public static void ReadObj<T>(this BinaryReader br, out T obj) {
        Type type = typeof(T);
        if(false) { }
        else if(type == typeof(ulong)) { obj = (T)(object)br.ReadUInt64(); }
        else if(type == typeof(uint)) { obj = (T)(object)br.ReadUInt32(); }
        else if(type == typeof(ushort)) { obj = (T)(object)br.ReadUInt16(); }
        else if(type == typeof(string)) { obj = (T)(object)br.ReadString(); }
        else if(type == typeof(float)) { obj = (T)(object)br.ReadSingle(); }
        else if(type == typeof(sbyte)) { obj = (T)(object)br.ReadSByte(); }
        else if(type == typeof(long)) { obj = (T)(object)br.ReadInt64(); }
        else if(type == typeof(int)) { obj = (T)(object)br.ReadInt32(); }
        else if(type == typeof(Half)) { obj = (T)(object)br.ReadHalf(); }
        else if(type == typeof(double)) { obj = (T)(object)br.ReadDouble(); }
        else if(type == typeof(decimal)) { obj = (T)(object)br.ReadDecimal(); }
        else if(type == typeof(char)) { obj = (T)(object)br.ReadChar(); }
        else if(type == typeof(byte)) { obj = (T)(object)br.ReadByte(); }
        else if(type == typeof(bool)) { obj = (T)(object)br.ReadBoolean(); }
        else if(type == typeof(short)) { obj = (T)(object)br.ReadInt16(); }
        else if(type == typeof(byte[])) { int length = br.ReadInt32(); obj = (T)(object)br.ReadBytes(length); }
        else if(type == typeof(char[])) { int length = br.ReadInt32(); obj = (T)(object)br.ReadChars(length); }
        else if(type == typeof(Color)) { obj = (T)(object)br.ReadRGB(); }
        else if(type == typeof(Vector2)) { obj = (T)(object)br.ReadPackedVector2(); }
        else
            throw new Exception("type not suppoerted for type " + type.ToString());
    }
    /// <summary>
    /// 支持<see cref="ReadObj"/>所支持类型的数组
    /// </summary>
    public static void ReadArray<T>(this BinaryReader br, out T[] array) {
        int length = br.ReadInt32();
        array = new T[length];
        foreach(int i in Range(length)) {
            br.ReadObj(out array[i]);
        }
    }
    /// <summary>
    /// 支持<see cref="ReadObj"/>所支持类型的列表
    /// </summary>
    public static void ReadList<T>(this BinaryReader br, ref List<T> list) {
        int count = br.ReadInt32();
        if(list == null) {
            list = new(count);
        }
        else {
            list.Clear();
        }
        foreach(int i in Range(count)) {
            br.ReadObj(out T element);
            list[i] = element;
        }
    }
    public static void ReadDict<TKey, TValue>(this BinaryReader br, ref Dictionary<TKey, TValue> dict) {
        int count = br.ReadInt32();
        dict = new();
        foreach(int _ in Range(count)) {
            br.ReadObj(out TKey key);
            br.ReadObj(out TValue value);
            dict.Add(key, value);
        }
    }
    #endregion
}