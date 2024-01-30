using Terraria.DataStructures;

namespace TigerLearning.Documents;

public partial class Document {
    public class IEntitySource_Interface {
        public static IEntitySource source;
        public static void ShowEntitySource() {
            EntitySource_ItemUse_WithAmmo source_ammo = null;
            Show(source_ammo.AmmoItemIdUsed);   //所使用的弹药的itemID(若没有则为0(待测试))
        }
    }
}
