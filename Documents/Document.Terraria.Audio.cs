using Terraria.Audio;

namespace TigerLearning.Documents;

public partial class Document {
    public class SoundEngine_cls {
        public static void ShowSoundEngine() {
            SoundStyle soundStyle = SoundID.MenuOpen;
            SoundEngine.PlaySound(SoundID.MenuOpen);    //播放一个声音(可以指定位置)
            var slotID = SoundEngine.PlaySound(SoundID.MenuOpen.WithVolumeScale(2f));    //播放一个2倍音量的声音(待测试)
            SoundEngine.TryGetActiveSound(slotID, out var activeSound);
            activeSound.Sound.Pitch = 2f;   //播放速度
        }
    }
}
