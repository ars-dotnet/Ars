using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class TaskTest
    {
        [Fact]
        public void Test1() 
        {
            Task.Factory.StartNew(
                () => 
                {

                }, CancellationToken.None, TaskCreationOptions.LongRunning,new ArsTaskScheduler());
        }
    }

    public sealed class ArsTaskScheduler : TaskScheduler
    {
        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            throw new NotImplementedException();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new NotImplementedException();
        }
    }
}
