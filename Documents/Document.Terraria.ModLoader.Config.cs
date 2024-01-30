using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace TigerLearning.Documents;

public partial class Document {
    public class ItemDefinition_cls {
        public static string intro = "在Config中方便的引用一个物品, 或者用来做itemId与mod name和item name的转换";
        public static void ShowItemDefinition() {
            #region params
            string modName = null, itemName = null, key = null;
            int itemId = 0;
            #endregion
            //以下物品名皆指内部物品名
            ItemDefinition def = new();     //默认为Terraria/None
            def = new(modName, itemName);   //模组名和物品名, 原版模组名为Terraria
            def = new(key);                 //以 模组名/物品名 的形式, 原版可直接用物品名
            def = new(itemId);              //传入itemId, 相当于传入ItemID.Search.GetName(type)👇
            ItemID.Search.GetName(itemId);  //模组物品时返回 模组名/物品名, 原版未测试
            def.ToString();                 //以 模组名/物品名 的形式返回
            Do(def.Type);                   //获取对应的itemID, 若没得到返回-1, 不要频繁获取此值
            Do(def.Mod);                    //模组名字, 原版时为Terraria
            Do(def.Name);                   //物品名字
            Do(def.IsUnloaded);             //是否加载, 额外的当modName和itemName都为""时返回假
            Do(ItemID.Search.TryGetId(modName != "Terraria" ? $"{modName}/{itemName}" : itemName, out int id) ? id : -1);
            //def.Type的等价版(源码)
        }
        /// <summary>
        /// 通过物品ID获得模组名和物品名
        /// </summary>
        public static void ItemIdToModAndItemName(int itemId) {
            ItemDefinition def = new(itemId);
            Do(def.Mod);            //获得到的模组名(原版物品时为Terraria)
            Do(def.Name);           //获得到的物品名
        }
        /// <summary>
        /// 通过模组名和物品名获得物品ID, 当找不到时返回-1
        /// </summary>
        public static void ModAndItemNameToItemId(string modName, string itemName) {
            ItemDefinition def = new(modName, itemName);
            Do(def.Type);
        }
    }
    public class ModConfig_cls {
        public static string intro = $"""
            继承{nameof(ModConfig)}的类会作为一个配置文件被加载
            里面的公开且实例的字段和属性会被序列化保存, 初始化后会被反序列化设置
            而私有的及内部的字段或属性则不会因反序列化被设置, 也不会因为序列化而被保存
            没有set访问器而有get访问器的属性会被显示但不能被修改(会是灰色字样, 但似乎仍会被保存)
            只有set访问器而没有get访问器的属性会让tmodloader崩溃
            """;
        public class ExampleModConfig : ModConfig {
            /// <summary>
            /// 是客户端配置还是服务端配置
            /// 必须重写的项
            /// </summary>
            public override ConfigScope Mode => ConfigScope.ServerSide;
            #region 可以添加的特性
            //以下所有特性都是可选的
            [Header("$Mods.ModName.Configs.ExampleModConfig.Headers.SomeHeader")]//在此条前显示一个标头, 字符串前加'$'表示本地化键值
            //相当于[Header("SomeHeader")]
            public int IntWithAHeaderAhead;

            #region 设置默认值
            [DefaultValue(2)]//设置默认值, 也可以直接设置默认值(在后面加 = <默认值>), 或者在无参构造函数中初始化
            public int IntWithADefaultValueOf2;

            [DefaultValue(typeof(Color), "12, 12, 12, 255")]//对于某些不方便在特性中直接设置默认值的可以使用这个
            public Color ColorWithCustomDefaultValue;
            #endregion

            [BackgroundColor(178, 53, 103, 200)]//设置此条的背景颜色
            public int IntWithCustomBackgroundColor;

            //以下这四个特性也可以对数组或列表使用, 相当于对其中的元素套用此特性(对Vector2也是可以的)
            [Range(0, 10)]//设置范围, 默认中 float 为 0 - 1, int 为 0 - 100, byte 为 0 - 255, 不过这个只会限制UI, 通过某些方法仍会越过此限制
            [Increment(2)]//设置可设置的值的最小间隔, 同样只会限制UI, 通过某些方法仍会越过此限制
            [Slider]//使用滑动条显示, 目前只影响int类型, 其它某些类型默认就是有滑动条的(如float, enum等)
            [DrawTicks]//对于滑动条, 是否在每个可选值上标上刻度(推荐在枚举值以及可选值不多的滑动条中使用)
            public int IntWithASlider;

            [OptionStrings(new string[]{"One", "Two", "Three"})]//对于字符串类型, 固定它只能是这么几个值
            public string StringWithStaticOptions = "One";

            [SeparatePage]//让此条显示为一个按钮, 点进去为单独的一页来设置此条(好像类默认就是采用的这个)
            public List<int> ListThatShowInSeparatePage = new();

            [ReloadRequired]//如果加上这个, 意味着只要对此条的值作出改变, 就需要重新加载整个模组
            public int IntThatRequireReload;

            [LabelKey($"Mods.{nameof(TigerLearning)}.Configs.{nameof(IntThatWithCustomLabelAndTooltip)}.Label")]//设置此条的标签的本地化键值, 默认即为此处的值
            [TooltipKey($"Mods.{nameof(TigerLearning)}.Configs.{nameof(IntThatWithCustomLabelAndTooltip)}.Tooltip")]//设置此条的提示(鼠标放上去时显示的文本)的本地化键值, 默认即为此处的值
            [LabelArgs("1", "2")]//如果对应的本地化字符串有参数, 从这里传入
            [TooltipArgs("1", "2")]
            public int IntThatWithCustomLabelAndTooltip;

            [JsonIgnore]//使之既不会在配置中显示, 也不会被序列化保存
            [ShowDespiteJsonIgnore]//即使有JsonIgnore也会在配置中显示, 但仍不会被序列化保存
            public int IntThatWontBeSavedButWouldStillShowUp;

            //使用指定的方法序列化与反序列化
            [JsonConverter(typeof(StringEnumConverter))]//让枚举值用字符串保存下来, 易于json文件的阅读
            public EquipType EnumWithGivenConverter;
            #endregion
            #region 可以直接使用的数据类型
            public bool SomeBool;
            public int SomeInt; //int默认显示的会是一个文本输入框
            public float SomeFloat;
            public string SomeString;
            #region 自定义枚举类型
            public enum ExampleCustomEnum {
                [LabelKey("Mods.{nameof(TigerLearning)}.Configs.{nameof(ExampleCustomEnum)}.{nameof(ExampleCustomEnum.One)}.Label")]    //TBT
                [TooltipKey("Mods.{nameof(TigerLearning)}.Configs.{nameof(ExampleCustomEnum)}.{nameof(ExampleCustomEnum.One)}.Tooltip")]
                One,
                Two,
                Three,
            }
            public static string CustomEnumLocalization = $"""
                对于自定义枚举类型, 需要在本地化文本中给出每个枚举值的label
                具体需要:
                Mods.{nameof(TigerLearning)}.Configs.{nameof(ExampleCustomEnum)}.Label
                Mods.{nameof(TigerLearning)}.Configs.{nameof(ExampleCustomEnum)}.Tooltip   //会被实例上的Tooltip覆盖
                Mods.{nameof(TigerLearning)}.Configs.{nameof(ExampleCustomEnum)}.{nameof(ExampleCustomEnum.One)}.Label  //对于每个枚举值都需要, 可以用LabelKey改写
                                                                                                                        //似乎给枚举值设置Tooltip是不需要的
                """;
            public ExampleCustomEnum SomeCustomEnum;
            #endregion
            public EquipType SomeEnum;
            public byte SomeByte;
            public uint SomeUInt;

            public Color SomeColor;
            public Vector2 SomeVector2;

            public int[] SomeArray = new int[] { 25, 70, 12 };    //数组, 需要给初始值以固定长度
            public List<int> SomeList = new() { 1, 3, 5 }; //列表, 可以不用初始化, 这样默认会是null, 可以在配置界面进行初始化
            public Dictionary<string, int> SomeDictionary = new();
            public HashSet<string> SomeSet = new();

            #region SimpleData具体构造
            [BackgroundColor(255, 7, 7)]
            public class SimpleData {
                [Header("FirstHeader")]
                public int boost;
                public float percent;

                [Header("SecondHeader")]
                public bool enabled;

                [DrawTicks]
                [OptionStrings(new string[] { "Pikachu", "Charmander", "Bulbasaur", "Squirtle" })]
                [DefaultValue("Bulbasaur")]
                public string FavoritePokemon;

                public SimpleData() {
                    FavoritePokemon = "Bulbasaur";
                }

                public override bool Equals(object obj) {
                    if(obj is SimpleData other)
                        return boost == other.boost && percent == other.percent && enabled == other.enabled && FavoritePokemon == other.FavoritePokemon;
                    return base.Equals(obj);
                }

                public override int GetHashCode() {
                    return new { boost, percent, enabled, FavoritePokemon }.GetHashCode();
                }
            }
            #endregion
            //嵌套一个类会自动实现
            public SimpleData SomeClassA;

            //这里不要用ModContent.ItemType<ClassName>()以在初始化中获取物品名字, 因为配置的加载在大多数内容之前
            public ItemDefinition SomeItem = new(nameof(TigerLearning), "ExampleItem");
            public NPCDefinition SomeNPC;
            public ProjectileDefinition SomeProjectile;
            public PrefixDefinition SomePrefix;
            #endregion

            /// <summary>
            /// 在多人模式下用以控制每个客户端能否对此配置作出改动(需要为服务端配置)
            /// </summary>
            public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) {
                //如果不是主机, 则不允许改配置
                if(!NetMessage.DoesPlayerSlotCountAsAHost(whoAmI)) {
                    message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost");
                    //NetworkText也可以由LocalizedText.ToNetworkText()得到
                    return false;
                }

                #region 专用服务器中使用HEROsMod以管理是否可配置
                //参考 https://github.com/JavidPack/ShorterRespawn/blob/1.4/ShorterRespawnConfig.cs#L85
                if(ModLoader.TryGetMod("HEROsMod", out Mod hero) && hero.Version >= new Version(0, 2, 2)) {
                    if(hero.Call("HasPermission", whoAmI, "ModifyTigerLearningExampleModConfig") is bool result && !result) {
                        message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost");
                        return false;
                    }
                }
                #endregion

                return true;
            }

            #region 兼容旧版本
            /// <summary>
            /// 获取到反序列化时未被使用的数据
            /// </summary>
		    [JsonExtensionData]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:添加只读修饰符", Justification = "<挂起>")]
            private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();

            //假如原本有一个OldListOfInts, 但是改名为了NewListOfInts
            public List<int> NewListOfInts;

            [OnDeserialized]
            void OnDeserialized(StreamingContext context) {
                if(_additionalData.TryGetValue("OldListOfInts", out var token)) {
                    var OldListOfInts = token.ToObject<List<int>>();
                    NewListOfInts.AddRange(OldListOfInts);
                }
                _additionalData.Clear(); //确保它清空了否则它会崩溃的(tml的ExMod如是说)
            }
            #endregion
        }
    }
}
