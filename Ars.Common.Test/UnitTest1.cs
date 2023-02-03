using Ars.Commom.Tool;
using System.Threading.Tasks;
using Xunit;

namespace Ars.Common.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            DisposeAction dispose = new DisposeAction(() => 
            {
                Task.FromResult(0);
            });

            dispose.Dispose();
            dispose.Dispose();
        }
    }
}