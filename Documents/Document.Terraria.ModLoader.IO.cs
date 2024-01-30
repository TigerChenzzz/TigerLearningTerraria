using Terraria.ModLoader.IO;

namespace TigerLearning.Documents;
public partial class Document {
    public class TagCompound_cls {
        public static TagCompound tagCompound;
        public static string intro = "相当于字典";
        public static string initialize = "可以像初始化字典一样初始化";
        public static void ShowTagCompound() {
            tagCompound = new() {
                {"tag1", 1 },
                {"tag2", "a" }
            };                                 //初始化
            Show(tagCompound["tag2"] as string);                    //使用下标引用
            Show(tagCompound.GetInt("tag1"));                       //获取整型
            Show(tagCompound.GetString("tag2"));                    //获取字符串
            tagCompound.Add("tag3", new List<int>() { 1, 2 });      //添加
            tagCompound.Set("tag3", new List<int>() { 1, 2 }, true);//设置
            Show(tagCompound.Get<List<int>>("tag3"));               //获取指定类型
            Show(tagCompound.Get<List<int>>("tag3")); //获取指定类型
            //Get实际上引用了TryGet, 而索引器则引用了Get和Set, Add则是非替代地调用Set
            //总结: 其中效率最高的是TryGet和Set
        }
    }
}
