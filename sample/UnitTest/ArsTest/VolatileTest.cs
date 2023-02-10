using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class Worker
    {
        private volatile bool _shouldStop;

        public void DoWork()
        {
            bool work = false;
            // 注意：這裡會被編譯器優化為 while(true)
            while (!_shouldStop)
            {
                work = !work; // do sth.
            }
            Console.WriteLine("工作執行緒：正在終止...");
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }
    }

    public class Program
    {
        [Fact]
        public void Main()
        {
            var worker = new Worker();

            Console.WriteLine("主執行緒：啟動工作執行緒...");
            var workerTask = Task.Run(worker.DoWork);

            // 等待 500 毫秒以確保工作執行緒已在執行
            Thread.Sleep(500);

            Console.WriteLine("主執行緒：請求終止工作執行緒...");
            worker.RequestStop();

            // 待待工作執行緒執行結束
            workerTask.Wait();
            //workerThread.Join();

            Console.WriteLine("主執行緒：工作執行緒已終止");
            Console.Read();
        }

        [Fact]
        public void TestTimeSpan() 
        {
            var a = TimeSpan.FromMinutes(1);
            var b = a.Seconds;
            var c = a.TotalSeconds;
        }
    }

    public interface IWoker
    {
        void Talk();
    }

    public abstract class WorkerBase 
    {
        public virtual void Talk() => Console.WriteLine("aa");
    }

    public sealed class WorkerA : WorkerBase, IWoker 
    {

    }
}
