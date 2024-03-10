using Terraria.ModLoader.Config;

namespace TigerLearning.Learning;

public class 杂项 {
    public static void ShowMiscs() {
        Show("任何武器有4%的基础暴击率(待测试)");
        string 泰拉瑞亚的自动加载原理 = $"""
            像ModPlayer, ModProjectile等等这些东西只要你写一个类继承自它们
            那么这个类就会被自动加载
            实际是在Terraria/ModLoader/Mod.Internals.cs的Autoload()中实现
            其中会通过反射获取此mod程序集中所有实现了{nameof(ILoadable)}接口的非抽象非泛型类,
            且此类需含有一个无参构造函数(无论公共私有),
            且不带有{nameof(AutoloadAttribute)}属性或此属性允许自动加载,
            然后将它们按照{nameof(Type.FullName)}排序,
            对它们依次调用{nameof(Mod.AddContent)}.
            在{nameof(Mod.AddContent)}中当{nameof(ILoadable.IsLoadingEnabled)}返回{true}时,
            就会调用{nameof(ILoadable.Load)}以及进行一系列其他操作
                包括将它加入Mod.content中
                    这样就可以通过{nameof(Mod.GetContent)}来获取
                    包括它在内的所有{nameof(ILoadable)} 或 特定类型的mod内容了
                和对它调用{nameof(ContentInstance.Register)}
                    这样就可以通过 ContentInstance<类名>.Instance 来获取它了
                    实际上不管是不是{nameof(ILoadable)}都可以通过调用这个方法来达到这个目的
                    如果不调用{nameof(ContentInstance.Register)}而直接使用 ContentInstance<类名>.Instance 只会获得{null}
            这样就完成了这个类的自动加载
            """;
    }
    public static void 一些有用的东西() {
        #region params
        Rectangle location = default;
        #endregion
        #region 世界进度
        Show(NPC.downedGoblins);            //哥布林
        Show(NPC.downedSlimeKing);          //史王
        Show(Main.hardMode);                //肉后
        Show(NPC.downedPlantBoss);          //世花
        Show(NPC.downedMoonlord);           //月后
        #endregion
        #region 显示文字
        Main.NewText("string", Color.White);
        CombatText.NewText(location, Color.White, "string");
        //PopupText.NewText()       //重铸时的弹出语使用此方法
        #endregion
        #region 未分类(暂无)
        #endregion
    }
    /// <summary>
    /// 像得到某样东西的ID, 通过ID得到名字等
    /// </summary>
    public static class 查找 {
        public static void ShowFindings() {
            #region params
            string modTypeFullName = default;
            string modName = default;
            string modTypeName = default;
            ModItem instanceToRegister = default;
            int itemType = default, projectileType = default;
            #endregion
            //查找与ModType相关的东西
            //TML中继承自ModType的应该都能用
            //对于自定义类型, (一般在Register()中)调用ModTypeLookup<ModTypeName>.Register(this)即可使用
            ModContent.Find<ModItem>(modTypeFullName);
            ModContent.TryFind(modName, modTypeName, out ModItem modItem);
            ModTypeLookup<ModItem>.Register(instanceToRegister);
            #region ModContent.xxxType
            ModContent.ItemType<ModItem>();
            ModContent.NPCType<ModNPC>();
            ModContent.ProjectileType<ModProjectile>();
            ModContent.BuffType<ModBuff>();
            ModContent.DustType<ModDust>();
            ModContent.GoreType<ModGore>();
            ModContent.PrefixType<ModPrefix>();
            ModContent.TileEntityType<ModTileEntity>();
            //...
            #endregion
            //对于继承自ILoadable的类型, 通过以下属性可以获取它的实例
            //对于没有自动加载的或不继承自ILoadable的类型, 也可以通过ContentInstance.Register(this)使之可以使用
            //如果同一个类注册了多个实例, 那么应使用Instances, 此时Instance为null
            Show(ContentInstance<ModItem>.Instance);
            ContentInstance.Register(instanceToRegister);

            //获得一个样本
            Show(ContentSamples.ItemsByType[itemType]);
            Show(ContentSamples.ProjectilesByType[projectileType]);
        }
        public static void ShowEntityDefinitions() {
            #region params
            string modName = default, itemName = default;
            int itemType = default;
            ItemDefinition itemDefinition = default;
            string projectileName = default;
            int projectileType = default;
            ProjectileDefinition projectileDefinition = default;
            #endregion
            //使用ItemDefinition可以从物品名和物品id间切换
            //其内部实际为使用ItemID.Search
            itemDefinition = new(modName, itemName);
            itemDefinition = new(itemType);
            Show(itemDefinition.Mod);
            Show(itemDefinition.Name);
            Show(itemDefinition.DisplayName);
            Show(itemDefinition.Type);
            Show(itemDefinition.IsUnloaded);

            projectileDefinition = new(modName, projectileName);
            projectileDefinition = new(projectileType);
            Show(projectileDefinition.Mod);
            Show(projectileDefinition.Name);
            Show(projectileDefinition.DisplayName);
            Show(projectileDefinition.Type);
            Show(projectileDefinition.IsUnloaded);
        }
        public static void ShowXxxIDSearch() {
            #region params
            int itemType = default;
            string itemName = default;
            int projectileType = default;
            string projectileName = default;
            #endregion
            //物品id转名字
            //若为Mod添加的物品则会是<Mod名>/<物品名>的形式
            //原版则直接是物品名
            Show(ItemID.Search.GetName(itemType));
            //物品名转id
            //物品名也需遵循上述格式
            Show(ItemID.Search.GetId(itemName));

            Show(ProjectileID.Search.GetName(projectileType));
            Show(ProjectileID.Search.GetId(projectileName));
        }
    }
}
