using Ars.Common.Tool.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class EnumTest
    {
        [Fact]
        public void Test1() 
        {
            TestEnum testEnum = TestEnum.A;
            Assert.True("AAA".Equals(testEnum.GetDescriotion()));
        }
    }

    public enum TestEnum 
    {
        [Description("AAA")]
        A,

        [Description("BBB")]
        B
    }
}
