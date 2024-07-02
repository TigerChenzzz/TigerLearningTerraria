using Microsoft.Xna.Framework.Graphics;
using TigerLearning.Common.Configs;

namespace TigerLearning.Common.MyUtils;

public static class RefUtils {
    public static ClientConfig Config;
    public static void UnloadRefUtils() {
        Config = null;
    }
    public const bool ObsoleteError = true;
    public static class Textures {
        private static Texture2D hitbox;
        public static Texture2D Hitbox => hitbox ??= ModContent.Request<Texture2D>("TigerLearning/Content/NPCs/Hitbox").Value;
    }
}
