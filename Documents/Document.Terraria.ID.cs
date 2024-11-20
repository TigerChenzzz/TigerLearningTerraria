using Terraria.ModLoader.Default;

namespace TigerLearning.Documents;

public partial class Document {
    public class AmmoID_cls {
        public static void ShowAmmoID() {
            #region params
            Item item = null;
            ModItem modItem = null;
            #endregion
            item.useAmmo = AmmoID.Bullet;       //此物品以子弹作为弹药
            item.ammo = AmmoID.Bullet;    //此物品为一种子弹
        }
    }
    public class ItemID_cls {
        public static void ShowItemID() {
            Show(ItemID.None);                  //获得一个原版物品的id
        }
        public class Sets_cls {
            public static void Showets() {
                Show(ItemID.Sets.BossBag);      //下标对应物品id, 判断一个物品是否是宝藏袋, 也可设置一个物品是否是宝藏袋, 会影响是否会开出开发者套装
                Show(ItemID.Sets.PreHardmodeLikeBossBag);   //是否为肉前宝藏袋
                Show(ItemID.Sets.CatchingTool); //是否可以捕捉小动物
                Show(ItemID.Sets.LavaproofCatchingTool);    //是否可以捕捉岩浆小动物
                Show(ItemID.Sets.Torches);      //火把
                Show(ItemID.Sets.WaterTorches); //可以插在水中的火把
                bool[] _ = ItemID.Sets.Factory.CreateBoolSet(false, ItemID.None, ModContent.ItemType<UnloadedItem>());    //创建一个所有物品个数长度的数组
            }
        }
    }
    public class NPCID_cls {
        public class Sets_cls {
            public static void ShowSets() {
                Show(NPCID.Sets.CannotDropSouls);       //是否不能掉魂, 小动物, 城镇npc, 无钱币掉落的npc不需要加入其中
            }
        }
    }
    public class ProjectileID_cls {
        public static void ShowProjectileID() {
            Show(ProjectileID.Explosives);  //爆炸!
        }

    }
    public class TileID_cls {
        public static void CommonTileID() {
            #region 制作环境
            Show(TileID.WorkBenches);
            Show(TileID.CrystalBall);
            #endregion

        }
    }
}
