using Terraria.ModLoader.IO;

namespace TigerLearning.Learning;

public class 存取数据 {
    public class ModItem保存数据 : ModItem {
        //假设这是你需要保存的数据
        public int dataToSave;
        public override void SaveData(TagCompound tag) {
            if(dataToSave != 0) {  //推荐在非默认值的情况下才存下来
                tag["dataToSave"] = dataToSave; //名字只需要保证在本物品下不重名即可
            }
        }
        public override void LoadData(TagCompound tag) {
            tag.TryGet("dataToSave", out dataToSave);   //这样即使不存在那也会让dataToSave为默认值

            //如果想不把此类本身的默认值作为默认值(比如要把1作为默认值), 那么可以判TryGet的返回值
            if(!tag.TryGet("dataToSave", out dataToSave)) {
                dataToSave = 1;     //需要和SaveData处对应
            }
        }
    }
}