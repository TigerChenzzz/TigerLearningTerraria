#nullable enable        //WIP

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
    public static bool Debug => true;
    public static void MainPrint(string str, MainPrintType type = MainPrintType.Normal, Color? color = null) {
        switch(type) {
        case MainPrintType.Debug: {
            if(!Debug) {
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
    public static void DebugPrint(string str, string? tag = null) {
        if(tag == null || Config.DebugTags.Contains(tag)) {
            MainPrint($"[{tag ?? "debug"}]{str}", MainPrintType.Normal);
            Console.WriteLine(str);
        }
    }
}

static public partial class TigerExtensions {
    #region TagCompound 拓展
    public static void SetWithDefault<T>(this TagCompound tag, string key, T value, T? defaultValue = default) where T : IEquatable<T> {
        if(value == null && defaultValue == null || value?.Equals(defaultValue) == true) {
            return;
        }
        tag.Set(key, value);
    }
    public static T? GetWithDefault<T>(this TagCompound tag, string key, T? defaultValue = default, bool removeIfDefault = true) where T : IEquatable<T> {
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
    public static bool GetWithDefault<T>(this TagCompound tag, string key, out T? value, T? defaultValue = default, bool removeIfDefault = false) where T : IEquatable<T> {
        if(!tag.TryGet(key, out value)) {
            value = defaultValue;
            return false;
        }
        if(removeIfDefault) {
            if(value == null && (defaultValue == null || defaultValue.Equals(value)) || value != null && value.Equals(defaultValue)) {
                tag.Remove(key);
            }
        }
        return true;
    }
    #endregion
    #region AppendItem
    public static StringBuilder AppendItem(this StringBuilder stringBuilder, Item item) =>
        stringBuilder.Append(ItemTagHandler.GenerateTag(item));
    public static StringBuilder AppendItem(this StringBuilder stringBuilder, int itemID) =>
        stringBuilder.Append(ItemTagHandler.GenerateTag(ContentSamples.ItemsByType[itemID]));
    #endregion
    #region Add or Insert Tooltips
    public static Mod? ModInstance => null;
    public static bool AddIf(this List<TooltipLine> tooltips, bool condition, string name, string text, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, name, text);
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Add(line);
            return true;
        }
        return false;
    }
    public static bool AddIf(this List<TooltipLine> tooltips, bool condition, Func<string> nameDelegate, Func<string> textDelegate, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, nameDelegate?.Invoke(), textDelegate?.Invoke());
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Add(line);
            return true;
        }
        return false;
    }
    public static bool AddIf(this List<TooltipLine> tooltips, bool condition, string name, Func<string> textDelegate, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, name, textDelegate?.Invoke());
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Add(line);
            return true;
        }
        return false;
    }
    public static bool InsertIf(this List<TooltipLine> tooltips, bool condition, int index, string name, string text, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, name, text);
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Insert(index, line);
            return true;
        }
        return false;
    }
    public static bool InsertIf(this List<TooltipLine> tooltips, bool condition, int index, Func<string> nameDelegate, Func<string> textDelegate, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, nameDelegate?.Invoke(), textDelegate?.Invoke());
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Insert(index, line);
            return true;
        }
        return false;
    }
    public static bool InsertIf(this List<TooltipLine> tooltips, bool condition, int index, string name, Func<string> textDelegate, Color? overrideColor = null) {
        if(condition) {
            TooltipLine line = new(ModInstance, name, textDelegate?.Invoke());
            if(overrideColor != null) {
                line.OverrideColor = overrideColor;
            }
            tooltips.Insert(index, line);
            return true;
        }
        return false;
    }
    #endregion

}