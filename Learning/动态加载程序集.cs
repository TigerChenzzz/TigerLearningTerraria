using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using Terraria.ModLoader.Core;

namespace TigerLearning.Learning;

public class 动态加载程序集 {
    public const string TML内相关内容 = $"""
        {nameof(ModLoader)}
        {nameof(TmodFile)}
        {nameof(LocalMod)}
        {nameof(AssemblyManager)}
        {nameof(AssemblyManager.ModLoadContext)}
        """;

    public static void TML加载Mod时的相关过程() {
        Dos(nameof(ModLoader.Load), () => {
            Dos(nameof(ModOrganizer.FindAllMods), () => {
                Do(nameof(Directory.GetFiles)); // 使用 Directory.GetFiles 获取目录下的 .tmod 文件
                // 对于每个文件, 将文件读取为 LocalMod
                Dos(nameof(ModOrganizer.TryReadLocalMod), () => {
                    Do(nameof(TmodFile)); // 将 文件路径传给 TmodFile 构造方法生成一个 TmodFile
                    Dos(nameof(TmodFile.Read), (string path) => {
                        using var fileStream = File.OpenRead(path); // 使用文件读入流打开文件
                        BinaryReader reader = new(fileStream); // 以二进制方式读取
                        #region 读取文件内容
                        Debug.Assert(Encoding.ASCII.GetString(reader.ReadBytes(4)) != "TMOD");
                        var tmlVersion = reader.ReadString();
                        var hash = reader.ReadBytes(20);
                        var signature = reader.ReadBytes(256);
                        var dateLength = reader.ReadInt32();
                        // 计算哈希, 后优化到加载阶段而不是读入阶段
                        if (DoNothing()) {
                            var hashStartPos = fileStream.Position;
                            var verifyHash = SHA1.Create().ComputeHash(fileStream);
                            if (!verifyHash.SequenceEqual(hash))
			                    throw new Exception();
                            fileStream.Position = hashStartPos;
                        }
                        var modName = reader.ReadString();
                        var modVersion = reader.ReadString();
                        #region 读取内含文件
                        var fileCount = reader.ReadInt32();
                        var fileTable = new TmodFile.FileEntry[fileCount];
                        int offset = 0;
                        for (int i = 0; i < fileCount; ++i) {
                            fileTable[i] = new(reader.ReadString(), offset, reader.ReadInt32(), reader.ReadInt32());
                            offset += fileTable[i].CompressedLength;
                        }
                        var fileStartPos = (int)fileStream.Position;
                        foreach (var file in fileTable) {
                            file.Offset += fileStartPos;
                        }
                        // 在头部记录每一个文件的名称和长度, 并以此记录它们的偏移
                        // 需要使用时再使用 FileStream.Seek 和 (TML)EntryReadStream / DeflateStream (若压缩) 来读取 (见 TmodFile.GetStream)
                        #endregion
                        #endregion
                    });
                    Do(nameof(BuildProperties.ReadModFile)); // 读取文件中的 build.txt 以获取一些属性
                });
            });
            Do(nameof(ModOrganizer.SelectAndSortMods)); // 根据 enabled.json(初始) 或模组选择界面选择的模组挑选实际需要加载的模组
            // 根据 LocalMod 实例化为 Mod
            Dos(nameof(AssemblyManager.InstantiateMods), () => {
                Dos(nameof(AssemblyManager.ModLoadContext), $"""
                    先将所有 {nameof(LocalMod)} 实例化为 {nameof(AssemblyManager.ModLoadContext)}:
                    继承自 {nameof(AssemblyLoadContext)}, 设置其 isCollectible 为 true 以使之可卸载
                    """);
                Dos(nameof(AssemblyManager.ModLoadContext.AddDependency), $"""
                    再根据 {nameof(BuildProperties)} 添加模组依赖
                    此依赖处理可以让 {nameof(AssemblyManager.ModLoadContext.Load)}
                    在尝试获取程序集时可以直接获取到依赖模组的程序集
                    """);
                Dos(nameof(AssemblyManager.ModLoadContext.LoadAssemblies), () => {
                    Dos(nameof(AssemblyManager.ModLoadContext.LoadAssembly), $"""
                        根据 {nameof(BuildProperties)} 加载 lib/xxx.dll 下的程序集依赖,
                        再使用 mod.dll 和 mod.pdb 加载本体程序集
                        通过 {nameof(AssemblyLoadContext.LoadFromStream)} 完成
                        """);
                    Dos(nameof(AssemblyManager.GetLoadableTypes), $"""
                        使用了 {nameof(MetadataLoadContext)} 和
                        {nameof(AssemblyManager.ModLoadContext.MetadataResolver)}
                        以获取运行时程序集?
                        """);
                });
                Dos(nameof(AssemblyManager.Instantiate), () => {
                    Do(nameof(AssemblyManager.VerifyMod)); // 验证此模组最多只有一个实际的 Mod 类
                    Do(nameof(Activator.CreateInstance)); // 使用反射创建实例
                    // 设置一些 Mod 的基本属性
                });
            });
            // 加载模组内容
            Dos(nameof(ModContent.Load), () => {
                Dos(nameof(ModContent.JITModsAsync), $"""
                    获取每个模组的所有程序集然后对每个程序集中的每个需要加载的类 ({nameof(AssemblyManager.GetLoadableTypes)})
                    获取需要 JIT 的方法 (包括构造方法和属性访问器) ({nameof(Mod.PreJITFilter)})
                    进行强制 JIT ({nameof(AssemblyManager.ForceJITOnMethod)} -> {nameof(RuntimeHelpers.PrepareMethod)})
                    """);

                Do("其余的 Mod 加载工作");
            });
        });
    }

    public static void ShowAssemblyLoadContext() {
        Show(nameof(Assembly.Load)); // 可以通过此方法简单的加载程序集, 但是这样没法卸载, 也不能解决程序集依赖冲突的问题
        Show<AssemblyLoadContext>(); // 加载 / 卸载 程序集的核心类, 并且需依此解决程序集依赖冲突的问题
    }
}
