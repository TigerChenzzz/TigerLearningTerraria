using ImproveGame.Common.Configs;
using ImproveGame.Content.Items;
using System.Linq;
using Terraria.ModLoader.Default;
using WingSlotExtra;

namespace TigerLearning.Learning;

public class Mod联动 {
    public const string 依赖关系 = """
        在build.txt中modReferences中填的mod名表示强{依赖/引用}, weakReferences表示弱{依赖/引用}
        在强依赖时, 依赖的模组肯定会先于本模组被加载, 否则可能在之后加载或者根本不会加载
        若要指定最低版本, 则在build.txt的references的mod名后加@[版本号], 如 modReferences = ExampleMod@1.0.0.0
        可以将引用的mod的dll添加到项目依赖中获得代码补全
        """;
    public static void 获取其他模组的东西() {
        if(ModLoader.TryGetMod("CalamityMod", out Mod calamity)) {
            calamity.TryFind("PlantyMush", out ModItem plantyMush); //获得特定模组的特定ModItem
        }
        Show(ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC deviantt) ? deviantt.Type : 0);   //获取特定模组的NPCID
    }
    public static void Call接口() {
        #region params
        float bossValue = 1f;
        Func<bool> bossDowned = () => false;
        #endregion
        Show("通过调用其他Mod的Call接口来做到联动, 需要其他mod写好Call接口");
        if(ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist)) {
            bossChecklist.Call("AddBoss", "boss name", bossValue, bossDowned);
        }
    }
    public static class 直接使用其他mod的类 {
        public static class 强引用 {
            public const string intro = """
                在build.txt中的modReferences中填上需要强引用的mod名(后加 @[版本号] 可以指定最低版本)
                将引用的mod的dll添加到项目的程序集依赖, 如果有源码可以直接将其添加到项目的项目依赖
                然后就可以直接使用对应mod的公开的类了
                (然而如果实在要用到私有或内部(internal)的类的话仍需通过反射来调用)
                """;
            public static void ShowModReferences() {
                #region 以强引用更好的体验为例, 由于它开源所以我就直接使用它源码了
                string note = """
                    如果没有源码也可以在项目文件(.csproj)中添加
                    Project.ItemGroup.Reference(Include="ImproveGame").HintPath: lib\ImproveGame.dll
                    (我在那里注释好了的)
                    """;
                var improveGame = ImproveGame.ImproveGame.Instance; //此模组内置获得实例的方法
                    //注意此属性是对方Load时添加的, 如果为弱引用, 则在Load阶段不能保证有值, 不过此处为强引用就无所谓了
                    //若模组不内置获得实例的方法, 也可以通过下面这句获取
                //ImproveGame.ImproveGame improveGame = (ImproveGame.ImproveGame)ModLoader.GetMod(nameof(ImproveGame.ImproveGame));)
                var improveConfigs = (ImproveConfigs)improveGame.GetConfig(nameof(ImproveConfigs)); //获取其配置
                Show(improveConfigs.AlchemyGrassGrowsFaster); //获得其配置中的一项

                //活得玩家背包第一个旗帜盒
                BannerChest bannerChest = Main.LocalPlayer.inventory.Select(i => i?.ModItem as BannerChest).FirstOrDefault(bc => bc != null);
                bannerChest?.StoredBanners.ForEach(i => Show(i)); //遍历它里面的每一个旗帜, 可以对它们做一些事情, 或依据它们做一些事情
                #endregion
            }
        }

        [JITWhenModsEnabled("WingSlotExtra")]//TBT
        public static class 弱引用 {
            public const string intro = $"""
                在build.txt中的weakReferences中填上需要弱引用的mod名(后加 @[版本号] 可以指定最低版本)
                将引用的mod的dll添加到项目的程序集依赖, 如果有源码可以直接将其添加到项目的项目依赖
                然后就可以获得代码补全了
                需要在用到对方的类的类打上[JITWhenModsEnabled(ModNames)]
                如果是继承自 {nameof(ILoadable)} (如 {nameof(ModSystem)} 等) 或
                    继承自对方的类 (TBT), 那么则需要 {nameof(ExtendsFromModAttribute)}
                    即 [ExtendsFromMod(ModNames)]
                注意不要直接在其他类中使用打上了这些特性的类的成员, 因为这样仍然会加载它们,
                    并在对方没有加载时报错, 使用前需要判断对方有没有加载 ({nameof(ModLoader.HasMod)})
                """;
            public static void ShowModReferences() {
                #region 以联动额外翅膀栏为例
                //!! TBT 待测试
                Main.LocalPlayer.GetModPlayer<ModAccessorySlotPlayer>();
                var slot = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<WingSlotExtraSlot>().Type);
                var wing = slot.FunctionalItem;   //这样就能获得额外翅膀栏的饰品了
                #endregion
            }
        }
    }
}