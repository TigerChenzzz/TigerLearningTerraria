#nullable enable        //WIP

using System.Reflection;
using System.Text;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader.IO;

namespace TigerLearning.Common.MyUtils;

static public partial class TigerUtils
{
    public static bool InDebug => true;
    private static Mod? modInstance;
    public static Mod ModInstance => modInstance ??= ModLoader.GetMod(nameof(TigerLearning));
    public enum MainPrintType {
        Normal,
        Debug,
        AssertFail
    }
    public static void MainPrint(string str, MainPrintType type = MainPrintType.Normal, Color? color = null) {
        switch (type) {
            case MainPrintType.Debug: {
                    if (!InDebug) {
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
        if (Main.netMode != NetmodeID.Server) {
            Main.NewText(str, color);
        }
    }
    public static void DebugPrint(string str, string? tag = null) {
        if (tag == null || Config.DebugTags.Contains(tag)) {
            MainPrint($"[{tag ?? "debug"}]{str}", MainPrintType.Normal);
            Console.WriteLine(str);
        }
    }
    public static Item NewItem<T>() where T : ModItem => new(ModContent.ItemType<T>());
    public static T NewModItem<T>() where T : ModItem => (T)new Item(ModContent.ItemType<T>()).ModItem;
    public static Item SampleItem(int itemID) => ContentSamples.ItemsByType[itemID];
    public static Item SampleItem<T>() where T : ModItem => ContentSamples.ItemsByType[ModContent.ItemType<T>()];

#if MOUSE_MANAGER
    public class MouseManager : ModSystem
    {
        public static bool OldMouseLeft;
        public static bool MouseLeft;
        public static bool MouseLeftDown => MouseLeft && !OldMouseLeft;
        public event Action? OnMouseLeftDown;
        public override void PostUpdateInput()
        {
            OldMouseLeft = MouseLeft;
            MouseLeft = Main.mouseLeft;
            if (MouseLeftDown)
            {
                OnMouseLeftDown?.Invoke();
            }
        }
    }
#endif
    public static class TMLReflection
    {
        private static Assembly? assembly;
        public static Assembly Assembly => assembly ??= typeof(Main).Assembly;
        public static class Types
        {
            private static Dictionary<string, Type>? allTypes;
            public static Dictionary<string, Type> AllTypes => allTypes ?? GetRight(InitTypes, allTypes)!;
            private static void InitTypes()
            {
                allTypes = new();
                Assembly.GetTypes().ForeachDo(t =>
                {
                    if (t.FullName != null)
                    {
                        allTypes.Add(t.FullName, t);
                    }
                });
            }
            public static Type UIModConfig => AllTypes["UIModConfig"];
        }
        public static class UIModConfig
        {
            public static Type Type => Types.AllTypes["UIModConfig"];
            private static PropertyInfo? tooltip_prop;
            private static PropertyInfo Tooltip_prop => tooltip_prop ??= Type.GetProperty("Tooltip", BindingFlags.Public | BindingFlags.Static)!;
            private static Action<string>? setTooltip;
            public static Action<string> SetTooltip => setTooltip ??= Tooltip_prop.SetMethod!.CreateDelegate<Action<string>>(null);
            private static Func<string>? getTooltip;
            public static Func<string> GetTooltip => getTooltip ??= Tooltip_prop.GetMethod!.CreateDelegate<Func<string>>(null);
            public static string Tooltip {
                get => GetTooltip();
                set => SetTooltip(value);
            }
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