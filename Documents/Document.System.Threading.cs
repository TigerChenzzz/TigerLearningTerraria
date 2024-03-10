using System.Threading;

namespace TigerLearning.Documents;

public partial class Document {
    public class Thread_cls {
        public static Thread thread;
        public const string intro = "线程";
        public static void ShowThread() {
            Show(Thread.CurrentThread); //获得当前线程
            Show(thread.Name);          //线程的名字
            Show(thread.IsAlive);       //线程的当前状态(TBT)
            Show(thread.IsBackground);  //是否是后台线程(?)
            Show(thread.Priority);      //线程的调度优先级
            thread = new Thread(ThreadMethod);    //定义一个线程
            thread.Start();             //开始此线程
            Thread.Sleep(100);          //让此线程暂停100ms
            Thread.SpinWait(100);       //让致线程等待由 iterations 参数定义的时间量(TBT)
            Thread.Yield();             //转到另外的线程
        }
        public static void ThreadMethod() {
            for(int i = 0; i < 10; ++i) {
                Console.WriteLine(i);
                Thread.Yield();
            }
        }
    }
}
