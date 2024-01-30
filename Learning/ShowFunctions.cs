global using static TigerLearning.Learning.ShowFunctions;

namespace TigerLearning.Learning; 
public static class ShowFunctions {
    public static bool Shows(params object[] objs) {
        return Do(objs);
    }
    public static bool Show(object obj) {
        return Do(obj);
    }
    public static bool Show<T>(params object[] objs) {
        return Do(objs);
    }
}
