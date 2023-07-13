global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System;
global using System.Collections.Generic;
global using Terraria;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using TigerLearning.Common.MyUtils;
global using TigerLearning.Common.MyUtils.ID;
global using static TigerLearning.Common.MyUtils.RefUtils;
global using static TigerLearning.Common.MyUtils.TigerUtils;
using System.IO;

namespace TigerLearning;

public class TigerLearning : Mod {
    public static TigerLearning Instance { get; private set; }
    public override void Load() {
        Instance = this;
    }
    public override void Unload() {
        UnloadRefUtils();
    }
    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        int messageType = reader.ReadInt32();
        if(NetHandler.Handlers.TryGetValue(messageType, out var handler)) {
            handler.Invoke(reader, whoAmI);
        }
    }
}