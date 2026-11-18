using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace TigerLearning.Learning.额外研究;

public class 手动内存管理 {
    public static void ShowHAlloc() {
        #region 申请
        int size = 2000;
        nint buffer = Marshal.AllocHGlobal(size); // 分配 size 字节数的内存, 返回其指针
        // 指针直接指向数据开头, 此片段内存的元数据在指针前
        #endregion
        #region 写入
        {
            int ofs = 20;
            byte byteVal = 12;
            char charVal = 'c';
            short shortVal = 13;
            int intVal = 14;
            long longVal = 15;
            float floatVal = 0.3f;
            nint nintValue = 16;
            Marshal.WriteByte(buffer, ofs, byteVal); // 在指针偏移 20 字节处写入一个 byte, 与下面那行等价
            Marshal.WriteByte(buffer + ofs, byteVal);
            Marshal.WriteInt16(buffer, charVal);
            Marshal.WriteInt16(buffer, shortVal);
            Marshal.WriteInt32(buffer, intVal);
            Marshal.WriteInt64(buffer, longVal);
            Marshal.StructureToPtr(floatVal, buffer, false);
            Marshal.WriteIntPtr(buffer, nintValue); // 写入指针, 32 位为 4 字节, 64 位为 8 字节, 通过 IntPtr.Size 获取
            Show(IntPtr.Size); // 32 位为 4, 64 位为 8

            // 写入结构体
            SomeStruct s = new();
            bool deleteOld = false; // 是否对原位置上的对应结构体调用 Marshal.DestroyStructure(), 用于管理结构体内含有的引用对象
            Marshal.StructureToPtr(s, buffer, deleteOld);
            Show((object)Marshal.DestroyStructure<SomeStruct>);
        }
        #endregion
        #region 读取
        {
            int ofs = 20;
            Marshal.ReadByte(buffer, ofs);
            // 也可以直接解引用读取:
            unsafe {
                byte byteVal = *(byte*)buffer;
            }
            Marshal.ReadByte(buffer + ofs);
            Marshal.ReadInt16(buffer);
            Marshal.ReadInt32(buffer);
            Marshal.ReadInt64(buffer);
            Marshal.ReadIntPtr(buffer);
            Marshal.PtrToStructure<float>(buffer);
            Marshal.PtrToStructure<SomeStruct>(buffer);
        }
        #endregion
        #region 释放
        // 一般需要放在 finally 块中
        Marshal.FreeHGlobal(buffer);
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SomeStruct {
        public int IntValue;
        public float FloatValue;
    }

    // 使用 SafeHandle 管理结构体到非托管内存 (由 AI 完成, 未验证)
    class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid {
        private readonly int _offset;
        public SafeMemoryHandle(int offset, SomeStruct s) : base(true) {
            _offset = offset;
            handle = Marshal.AllocHGlobal(offset + sizeof(SomeStruct));
            Marshal.StructureToPtr(s, handle + offset, false);
        }

        protected override bool ReleaseHandle() {
            Marshal.DestroyStructure<SomeStruct>(handle + _offset);
            Marshal.FreeHGlobal(handle);
            return true;
        }

        public IntPtr Address => handle;

        public static void SampleUsage() {
            using SafeMemoryHandle handle = new(4, new() { IntValue = 2 } );
            Marshal.WriteInt32(handle.Address, 42);
        }
    }
}
