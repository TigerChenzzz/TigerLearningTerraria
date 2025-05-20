using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TigerLearning.Learning;

public class 信号量 {
    public const string 介绍 = """
        在多线程开发中用以取代传统的轮询及标志位
        """;
    public const string 参考 = """
        B站 十月的寒流
            "在 C# 多线程开发中如何用信号量来取代传统的轮询及标志位" https://www.bilibili.com/video/BV1e7o2YTEpi/
            "在 C# 多线程开发中用信号量的补充内容及异步编程的延申" https://www.bilibili.com/video/BV1kRdZY2Ez7/
            相关博客 https://blog.coldwind.top/posts/use-signal-over-polling-flags/
        微软文档
            https://learn.microsoft.com/zh-cn/dotnet/standard/threading/eventwaithandle
            https://learn.microsoft.com/zh-cn/dotnet/standard/threading/semaphore-and-semaphoreslim
        """;

    public static void ShowWaitHandle() {
        #region ManualResetEvent
        string ManualResetEvent介绍 = """
            ManualResetEvent 手动重置的信号量
            false 代表关闭状态, 当调用 WaitOne() 时就会进入堵塞状态, 直至打开 (放行)
            true 则为打开状态, 调用 WaitOne() 不会堵塞
            初始化后通过 Set() 和 Reset() 来打开和关闭
            """;
        using ManualResetEvent manualResetEvent = new(false);
        if (DoNothing()) {
            manualResetEvent.Set(); // 打开
            manualResetEvent.WaitOne(); // -> 通过
            manualResetEvent.Reset(); // 关闭
            manualResetEvent.WaitOne(); // -> 堵塞
        }
        #region ManualResetEvent 使用示例
        if (DoNothing()) {
            DateTime start = DateTime.Now;
            string CurrentTime() {
                return (DateTime.Now - start).TotalSeconds.ToString("0.00");
            }
            using Task workerTask = Task.Run(() =>{
                Thread.Sleep(0);
                Console.WriteLine($"[w][{CurrentTime()}]Start worker task");
                manualResetEvent.WaitOne();
                Console.WriteLine($"[w][{CurrentTime()}]after wait one");
                Thread.Sleep(1000);
                Console.WriteLine($"[w][{CurrentTime()}]after sleep 1");
                manualResetEvent.WaitOne();
                Console.WriteLine($"[w][{CurrentTime()}]after sleep 1 and wait one");
            });
            using Task controllerTask = Task.Run(() =>{
                // 1 秒后打开一下, 2 秒后再打开
                Thread.Sleep(1000);
                manualResetEvent.Set();
                Console.WriteLine($"[c][{CurrentTime()}]set event");
                manualResetEvent.Reset();
                Console.WriteLine($"[c][{CurrentTime()}]reset event");
                Thread.Sleep(1000);
                manualResetEvent.Set();
                Console.WriteLine($"[c][{CurrentTime()}]set event again");
            });
            Thread.Sleep(5000);
            Console.WriteLine($"[m][{CurrentTime()}]stop");
            string 比较理想的输出 = """
            [w][0.00]Start worker task
            [w][1.02]after wait one
            [c][1.02]set event
            [c][1.02]reset event
            [w][2.02]after sleep 1
            [w][3.03]after sleep 1 and wait one
            [c][3.03]set event again
            [m][5.01]stop
            """;
        }
        #endregion
        #endregion
        #region AutoResetEvent
        string AutoResetEvent介绍 = """
            AutoResetEvent 自动重置的信号量
            与 ManualResetEvent 基本相同, 只是当 WaitOne() 时它会自动关闭
            """;
        using AutoResetEvent autoResetEvent = new(false);
        if (DoNothing()) {
            manualResetEvent.Set(); // 打开
            manualResetEvent.WaitOne(); // -> 通过
            manualResetEvent.WaitOne(); // -> 堵塞
            manualResetEvent.Reset(); // 关闭
        }
        #endregion
        #region Semaphore
        string Semaphore介绍 = """
            持有一个整数值, 当此值为 0 时阻塞, 当通过时使此值减 1
            通过 Release(...) 增加此值
            初始化时需指定此值的上限 (需要是正数)
            """;
        using Semaphore semaphore = new(0, int.MaxValue);
        if (DoNothing()) {
            semaphore.Release(); // 放行 1 次
            semaphore.WaitOne(); // -> 通过
            semaphore.Release(2); // 放行 2 次
            semaphore.WaitOne(); // -> 通过
            semaphore.WaitOne(); // -> 通过
            semaphore.WaitOne(); // -> 堵塞
        }
        #endregion
        #region SemaphoreSlim
        string SemaphoreSlim介绍 = """
            SemaphoreSlim 轻量化的 Semaphore, 不继承自 WaitHandle
            """;
        using SemaphoreSlim semaphoreSlim = new(0);
        if (DoNothing()) {
            semaphoreSlim.Release();
            semaphoreSlim.Wait();
            semaphoreSlim.Release(2);
            Show(semaphoreSlim.CurrentCount);
        }
        #endregion
    }

    public static void ShowBlockingCollection() {
        string BlockingCollection介绍 = """
            它是一个线程安全的集合类型，而且它还提供了阻塞和通知的功能
            """;
        BlockingCollection<int> blockingCollection = []; // 可指定最大容量和实际使用的容器 (IProducerConsumerCollection<>)
        blockingCollection.Add(2); // 添加一个元素
        blockingCollection.CompleteAdding(); // 标记为不再添加元素
        Show(blockingCollection.IsAddingCompleted); // 检查是否标记为不再添加元素
        blockingCollection.TryAdd(2); // 尝试添加元素, 可选参数包括等待时间和 CancellationToken
        blockingCollection.TryTake(out int item); // 取出元素, 可选参数包括等待时间和 CancellationToken
    }
}
