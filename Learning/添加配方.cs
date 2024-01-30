using TigerLearning.Documents;

namespace TigerLearning.Learning;

public class 添加配方 {
    
    public static Document.Recipe_cls recipe_cls;
    #region params
    public static Recipe recipe;
    public static ModItem modItem;
    #endregion
    public static void 添加一个配方() {
        #region 在ModItem中重写方法AddRecipes()
        recipe = Recipe.Create(modItem.Type);   //以某个物品为产物, 与下一个选其一
        recipe = modItem.CreateRecipe(); //创建一个以自己为产物的配方, 可以传入amount代表产出几个
        /*
        如果你写俩种相同的材料的话
        它们会分别显示而不是合并成一个
        你最多可以加14个合成材料, 这是泰拉瑞亚本体规定的
        */
        recipe.AddIngredient(ItemID.DirtBlock, 10); //以10个土块作为材料
        recipe.AddTile(TileID.WorkBenches);     //此配方需要在工作台旁边
        recipe.Register();  //把这个合成表装进tr的系统里
        //可以连续书写: modItem.CreateRecipe().AddIngredient(ItemID.DirtBlock, 10).AddTile(TileID.WorkBenches).Register();
        #endregion
    }
    public static void 添加液体环境() {
        recipe.AddCondition(Condition.NearWater);
        recipe.AddCondition(Condition.NearLava);
        recipe.AddCondition(Condition.NearHoney);
        recipe.AddCondition(Condition.NearShimmer);

    }
    public static void 添加合成组() {
        #region 在ModSystem或主类中重写AddRecipeGroup()向mod添加合成组
        RecipeGroup group = new(() => "任意火把", new int[]
        {
            ItemID.Torch,
            ItemID.BlueTorch,
            ItemID.CursedTorch,
            ItemID.DemonTorch,
            ItemID.GreenTorch,
            ItemID.IceTorch,
            ItemID.IchorTorch,
            ItemID.OrangeTorch,
            ItemID.PinkTorch,
            ItemID.PurpleTorch,
            ItemID.RainbowTorch,
            ItemID.RedTorch,
            ItemID.TikiTorch,
            ItemID.UltrabrightTorch,
            ItemID.WhiteTorch,
            ItemID.YellowTorch,
        });
        RecipeGroup.RegisterGroup("AnyTorch", group);
        #endregion
        #region 任意AddRecipe()中
        //...
        recipe.AddRecipeGroup("AnyTorch", 10);  //添加自定义合成组
        recipe.AddRecipeGroup(group);       //直接使用RecipeGroup类为参数
        recipe.AddRecipeGroup(RecipeGroupID.Wood);  //原版合成组
        //...
        #endregion
    }
    public static void 微光转化() {
        recipe.AddCustomShimmerResult(ItemID.DirtBlock, 10);
        recipe.HasShimmerCondition(Condition.Hardmode); //微光转化的条件, 若没有则不加
    }
    public static void 修改产物() {
        recipe.ReplaceResult(ModContent.ItemType<ModItem>());   //替换结果, 虽说可以传入ModItem, 但是和传入itemID的效果是一样的
        recipe.createItem.stack = 2;    //也可以做一些其它修改, 这些修改都会反应到产物上, 而且在制作出来前就会知道
        //在制成时执行, 可以对item进行修改以修改产物, 但修改只会在制作出来后才能生效
        recipe.AddOnCraftCallback((recipe, item, consumedItems, destinationStack) => { });
    }
}
