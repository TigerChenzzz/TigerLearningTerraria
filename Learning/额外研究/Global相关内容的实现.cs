using Terraria.ModLoader.Core;

namespace TigerLearning.Learning.额外研究;

public class Global相关内容的实现;
public class Global相关内容的实现<TGlobal, TEntity> where TGlobal : GlobalType<TEntity, TGlobal> where TEntity : IEntityWithGlobals<TGlobal> {
    public static readonly string Intro = """
        对于每个 {nameof(TEntity)}, 其实现会含有一个 {nameof(TGlobal)} 的数组
        在 SetDefaults 时通过 {nameof(GlobalLoaderUtils<TGlobal, TEntity>.SetDefaults)} 设置其值
        数组的长度由{GlobalList<TGlobal>.SlotsPerEntity}决定
        """;
    public static readonly string InstancePerEntity由来 = $"""
        {GlobalList<TGlobal>.SlotsPerEntity} 由注册时的 {nameof(GlobalType<TGlobal>.SlotPerEntity)} 决定,
            此值一般为 {nameof(GlobalType<TGlobal>.InstancePerEntity)},
            当此值为 {true} 时会在注册时使 {GlobalList<TGlobal>.SlotsPerEntity} + 1
        """;

    public static void ShowGlobalLoaderUtilsSetDefaults(TEntity entity, ref TGlobal[] entityGlobals, Action<TEntity> setModEntityDefaults) {
        int initialType = entity.Type; // 获取实体类型 (ID)

        Show(InstancePerEntity由来);
        entityGlobals = new TGlobal[GlobalList<TGlobal>.SlotsPerEntity]; // 设置数组长度

        Show($"""
            {nameof(GlobalLoaderUtils<TGlobal, TEntity>)} 内部会有一个
                {typeof(TGlobal[][])} 类型的 SlotPerEntityGlobals,
                保存有每个实体类型对应的 {typeof(TGlobal[])} 数组
            此处会从中选取对应实体类型的数组, 根据 {nameof(GlobalType<TGlobal>.InstancePerEntity)}
                决定是调用 {nameof(GlobalType<TEntity, TGlobal>.NewInstance)} 还是直接使用此实例放入 {entityGlobals} 数组
            """);

        setModEntityDefaults(entity); // 调用类型特定的 SetDefaults

        Show($"""
            {nameof(GlobalLoaderUtils<TGlobal, TEntity>)} 中也有 {typeof(TGlobal[][])} 类型
                的 HookSetDefaultsEarly 和 HookSetDefaultsLate,
                保存每个实体类型对应的 {typeof(TGlobal[])} 数组, 代表应该早些还是晚些 SetDefaults
            此处会先调用 HookSetDefaultsEarly 中的 SetDefaults, 然后 HookSetDefaultsLate
            """);
    }
}
