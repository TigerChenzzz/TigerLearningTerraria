using Terraria.Utilities;

namespace TigerLearning.Documents;

public partial class Document {
    public class WeightedRandom_cls {
        public static void ShowWeightedRandom() {
            WeightedRandom<string> randomStrings = new(Main.rand);
            randomStrings.Add("weight 1");
            randomStrings.Add("weight 2", 2);
            randomStrings.Add("weight 3", 3);
            randomStrings.Add("weight 2/3", 2 / 3.0);   //double类型的权值
            string result = randomStrings;  //可以直接隐式转换
        }
    }
}
