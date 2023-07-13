using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader.IO;

namespace TigerLearning.Common.MyUtils;

static public partial class TigerUtils {
    public enum MainPrintType {
        Normal,
        Debug,
        AssertFail
    }
    public static void MainPrint(string str, MainPrintType type = MainPrintType.Normal, Color? color = null) {
        switch(type) {
        case MainPrintType.Debug: {
            if(!Config.Debug) {
                return;
            }
            str = "[Debug]" + str;
            break;
        }
        case MainPrintType.AssertFail: {
            str += "(如果出现此句请尝试和作者联系!)";
            break;
        }
        }
        if(Main.netMode != NetmodeID.Server) {
            Main.NewText(str, color);
        }
    }
    public static void DebugPrint(string str, string tag = null) {
        if(tag == null || Config.DebugTags.Contains(tag)) {
            MainPrint($"[{tag ?? "debug"}]{str}", MainPrintType.Normal);
            Console.WriteLine(str);
        }
    }
    public static StringBuilder AppendItem(this StringBuilder stringBuilder, Item item) =>
        stringBuilder.Append(ItemTagHandler.GenerateTag(item));
    public static StringBuilder AppendItem(this StringBuilder stringBuilder, int itemID) =>
        stringBuilder.Append(ItemTagHandler.GenerateTag(ContentSamples.ItemsByType[itemID]));
    public static bool AddIf(this List<TooltipLine> tooltips, bool condition, string name, string text) {
        if(condition) {
            tooltips.Add(new(TigerLearning.Instance, name, text));
            return true;
        }
        return false;
    }
    #region Terraria-independent
    #region Lerp and Clamp
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
    /// <returns>(序号, 迭代值) 其中序号从0开始</returns>
    public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> enumerable) {
        int index = 0;
        foreach(T t in enumerable) {
            yield return (index++, t);
        }
    }
    #endregion
    #region Random
    public static class MyRandom {
        public static double RandomAverage(double min, double max, Random rand = null) {
            if(min == max) {
                return min;
            }
            rand ??= new();
            return min + (max - min) * rand.NextDouble();
        }
        public static double RandomNormal(double μ, double σ, Random rand = null)//产生正态分布随机数
        {
            rand ??= new();
            double r1 = rand.NextDouble();
            double r2 = rand.NextDouble();
            double standardNormal = Math.Sqrt(-2 * Math.Log(r1)) * Math.Sin(2 * Math.PI * r2);
            return standardNormal * σ + μ;
        }
        public static double RandomNormalRangeApproximate(double min, double max, double μ, double σ, Random rand = null) {
            if(min == max) {
                return min;
            }
            if(σ == 0) {
                return μ;
            }
            rand ??= new();
            σ = Math.Abs(σ);
            double minoversigma = (min - μ) / σ;
            double maxoversigma = (max - μ) / σ;
            double maxun = Math.Max(Math.Abs(maxoversigma), Math.Abs(minoversigma));
            if(maxun == 0) {
                return min;
            }
            minoversigma /= maxun;
            maxoversigma /= maxun;
            double minr1 = Math.Exp( maxun * maxun / -2);
            double r1 = minr1 + rand.NextDouble() * (1 - minr1);
            double r2 = minoversigma + (maxoversigma - minoversigma) * rand.NextDouble();
            double standardNormal = Math.Sqrt(-2 * Math.Log(r1)) * Math.Sin(Math.PI * r2 / 2);
            return standardNormal * σ + μ;
        }
        public static double RandomDistribution(double min, double max, double μ, double sharpness, Random rand = null) {
            if(sharpness == 0) {
                return RandomAverage(min, max, rand);
            }
            return RandomNormalRangeApproximate(min, max, μ, Math.Max(Math.Abs(min - μ), Math.Abs(max - μ)) / sharpness, rand);
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
    #endregion
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
    #region 一些数学运算(统计学取整和约等于)
    /// <summary>
    /// 统计学取整
    /// (四舍六入五成双)
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
    /// <summary>
    /// 约等于
    /// 实际返回两者之差是否在<paramref name="tolerance"/>之内
    /// </summary>
    public static bool RoughEqual(float a, float b, float tolerance = .01f) {
        return MathF.Abs(a - b) <= tolerance;
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
    private static int GetIndex(IList list, Index index) {
        return index.IsFromEnd ? list.Count - index.Value : index.Value;
    }
    public static void RemoveAt(this IList list, Index index) {
        list.RemoveAt(GetIndex(list, index));
    }
    public static void RemoveRange<T>(this List<T> list, Range range) {
        int start = GetIndex(list, range.Start), end = GetIndex(list, range.End);
        if(start > end) {
            start ^= end;
            end ^= start;
            start ^= end;
        }
        list.RemoveRange(start, end - start);
    }
    #endregion
    #region List.AddIf
    public static bool AddIf<T>(this List<T> list, bool condition, T element) {
        if(condition) {
            list.Add(element);
        }
        return condition;
    }
    #endregion
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
    #region TagCompound 拓展
    public static void SetWithDefault<T>(this TagCompound tag, string key, T value, T defaultValue = default) where T : IEquatable<T> {
        if(value.Equals(defaultValue)) {
            return;
        }
        tag.Set(key, value);
    }
    public static T GetWithDefault<T>(this TagCompound tag, string key, T defaultValue = default, bool removeIfDefault = true) where T : IEquatable<T> {
        if(!tag.TryGet(key, out T value)) {
            return defaultValue;
        }
        if(removeIfDefault) {
            if(value.Equals(defaultValue)) {
                tag.Remove(key);
            }
        }
        return value;
    }
    /// <summary>
    /// 返回是否成功得到值, 返回假时得到的是默认值(返回真时也可能得到默认值(若保存的为默认值的话))
    /// </summary>
    public static bool GetWithDefault<T>(this TagCompound tag, string key, out T value, T defaultValue = default, bool removeIfDefault = true) where T : IEquatable<T> {
        if(!tag.TryGet(key, out value)) {
            value = defaultValue;
            return false;
        }
        if(removeIfDefault) {
            if(value.Equals(defaultValue)) {
                tag.Remove(key);
            }
        }
        return true;
    }
    #endregion
    #region Min / Max
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
    public static T GetRight<T>(object left, T right) => right;
#pragma warning restore IDE0060 // 删除未使用的参数
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="action"/>.
    /// 返回<paramref name="condition"/>
    /// </summary>
    public static bool DoIf(bool condition, Action action) {
        if(condition) {
            action?.Invoke();
        }
        return condition;
    }
    /// <summary>
    /// 若<paramref name="condition"/>为真则调用<paramref name="action"/>, 否则调用<paramref name="altAction"/>.
    /// 返回<paramref name="condition"/>
    /// 若为表达式推荐使用<see cref="Do"/>配合三目运算符
    /// </summary>
    public static bool DoIfElse(bool condition, Action action, Action altAction) {
        if(condition) {
            action?.Invoke();
        }
        else {
            altAction?.Invoke();
        }

        return condition;
    }
    /// <summary>
    /// return false when action or condition is null, else return true.
    /// when condition is null but action is not would still do action once
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
    /// if break out, return true, else return false.
    /// when condition is null but action is not would still do action once and try break out
    /// </summary>
    /// <param name="action">when get true, break out</param>
    public static bool DoWhileBreak(Func<bool> action, Func<bool> condition) {
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
    /// return false when action or condition is null, else return true
    /// </summary>
    public static bool WhileDo(Func<bool> condition, Action action) {
        if(condition == null || action == null) {
            return false;
        }
        while(condition()) {
            action();
        }
        return true;
    }
    /// <summary>
    /// when break out, return true, else return false
    /// </summary>
    /// <param name="action">when get true, break out</param>
    public static bool WhileDoBreak(Func<bool> condition, Func<bool> action) {
        if(condition == null || action == null) {
            return false;
        }
        while(condition()) {
            if(action()) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// return false when action or condition is null, else return true
    /// </summary>
    public static bool ForDo(Action init, Func<bool> condition, Action iter, Action action) {
        if(condition == null || action == null) {
            return false;
        }
        init?.Invoke();
        if(iter != null) {
            while(condition()) {
                action();
                iter();
            }
        }
        else {
            while(condition()) {
                action();
            }
        }
        return true;
    }
    /// <summary>
    /// when break out, return true, else return false
    /// </summary>
    /// <param name="action">when get true, break out</param>
    public static bool ForDoBreak(Action init, Func<bool> condition, Action iter, Func<bool> action) {
        if(condition == null || action == null) {
            return false;
        }
        init?.Invoke();
        if(iter != null) {
            while(condition()) {
                if(action()) {
                    return true;
                }
                iter();
            }
        }
        else {
            while(condition()) {
                if(action()) {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// return false when action or condition is null, else return true
    /// </summary>
    public static bool ForeachDo<T>(IEnumerable<T> enumerable, Action<T> action) {
        if(enumerable == null || action == null) {
            return false;
        }
        foreach(T t in enumerable) {
            action(t);
        }
        return true;
    }
    /// <summary>
    /// when break out, return true, else return false
    /// </summary>
    /// <param name="action">when get true, break out</param>
    public static bool ForeachDoBreak<T>(IEnumerable<T> enumerable, Func<T, bool> action) {
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
    #region 杂项

    /// <summary>
    /// 空类, 用以做标识
    /// </summary>
    public class Identifier { }
    #endregion
    #endregion
}