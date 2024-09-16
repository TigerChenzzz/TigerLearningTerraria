//此项目中部分源码源自新版裙中世界教程的模板Mod https://github.com/CXUtk/TemplateMod2
//以及tml的Example Mod (简称Ex Mod) https://github.com/tModLoader/tModLoader
//一些东西也参考了群中世界的教程 https://fs49.org/

global using Microsoft.Xna.Framework;
global using System;
global using System.Collections.Generic;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
global using TigerLearning.Common.MyUtils;
global using TigerLearning.Common.MyUtils.ID;
global using static TigerLearning.Common.MyUtils.RefUtils;
global using static TigerLearning.Common.MyUtils.TigerUtils;
using System.IO;

namespace TigerLearning;

public class TigerLearning : Mod {
    public static Catalogue 目录;
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