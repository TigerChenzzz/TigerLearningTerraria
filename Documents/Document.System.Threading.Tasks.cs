using System.Threading;
using System.Threading.Tasks;

namespace TigerLearning.Documents;

public partial class Document {
    public class Task_cls {
        public static Task task;
        public const string intro = """
            用于异步执行
            """;
        public const string 参考 = """
            C#中关于Task（任务）的简单介绍 - CSDN: https://blog.csdn.net/qq_43565708/article/details/130244115
            Task 类 - Microsoft: https://learn.microsoft.com/zh-cn/dotnet/api/system.threading.tasks.task?view=net-8.0
            c# task三种创建方式的区别 - CSDN: https://blog.csdn.net/niechaoya/article/details/98038851
            """;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        public static async Task ShowTask() {
            #region params
            CancellationTokenSource cancellationTokenSource = default;
            #endregion
            task = new(() => Dos());
            #region 开始一个任务
            task = Task.Run(() => Dos());   //开始一个任务(同步转异步)
            task = Task.Run(NewTask);   //开始这个任务
            task.Start();
            Task.Factory.StartNew(() => Dos());
            task.RunSynchronously();    //异步转同步
            #endregion
            #region 等待任务完成
            task.Wait();    //等待此任务完成(同步方法中可以使用)
            await task;     //等待此任务完成(异步方法中使用)
            Task.WaitAny(task, task);   //等待任一任务完成, 返回有多少个任务完成了
            Task.WaitAll(task, task);   //等待所有任务完成
            #endregion
            #region 任务的状态
            Show(task.Status);
            Show(TaskStatus.Created);               //刚被创建但还没被安排
            Show(TaskStatus.WaitingForActivation);  //已被安排但还没开始
            Show(TaskStatus.Running);               //正在执行
            Show(TaskStatus.WaitingForChildrenToComplete);//已执行完成但在隐式地等待子任务完成
            Show(TaskStatus.RanToCompletion);       //成功地完成了
            Show(TaskStatus.Canceled);              //被取消了
            Show(TaskStatus.Faulted);               //引发了未经处理的异常

            Show(task.IsCompleted);                 //任务是否已完成 (即任务处于三种最终状态之一： RanToCompletion、 Faulted或 Canceled)
            Show(task.IsCompletedSuccessfully);     //任务是否成功完成, 即处于RanToCompletion状态
            Show(task.IsCanceled);                  //任务是否被取消了
            Show(task.IsFaulted);                   //任务是否失败了
            #endregion
            #region 获得结果
            //下两句都是等到其完成然后再取结果, 如果完成不成功则会报错
            var result = await NewTask();   //同步方法中无法使用
            Show(NewTask().Result);
            #endregion
            #region 中断任务
            cancellationTokenSource = new();
            task = new(() => Dos(), cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();           //立即取消, 测试上看即使任务完成了再取消也不会有事
            cancellationTokenSource.CancelAfter(200);   //在200ms后取消, 不过测试上这句好像不起作用
            #endregion
            #region 任务完成时自动启动新任务
            task.ContinueWith(task => Dos());
            //可多次调用ContinueWith, 在任务完成时会将所有传入的任务放进线程池的队列中
            #endregion
            #region 子任务
            task = new Task<int[]>(() => {
                var result = new int[3];
                new Task(() => result[0] = 0, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => result[1] = 1, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => result[2] = 2, TaskCreationOptions.AttachedToParent).Start();
                return result;
            });//这样当内含的三个子任务还没有完成时此任务也不算完成状态
            #endregion
            #region 任务工厂
            //以避免将相同的参数传给每个Task的构造器
            task = new Task(() => {
                cancellationTokenSource = new();
                var taskFactory = new TaskFactory<bool>(
                    cancellationTokenSource.Token,
                    TaskCreationOptions.AttachedToParent,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
                var childTask = Range(3).ForeachDo(i => taskFactory.StartNew(() => Do(i)));
            });
            #endregion
        }
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

        /// <summary>
        /// 创建一个异步任务
        /// </summary>
        public static async Task<int> NewTask() {
            Console.WriteLine("New Task!");
            await task;
            return 2;
        }
    }
}
