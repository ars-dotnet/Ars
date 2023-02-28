using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class ListTest
    {
        [Fact]
        public void TestIntersect() 
        {
            List<int> a = new List<int> { 1, 2,2, 3,4,5 };
            List<int> b = new List<int> { 1, 1,2,5 ,6,7};

            var u = a.Union(b).ToList();
            var i = a.Intersect(b).ToList();
            var e = a.Except(b).ToList();
            var e1 = b.Except(a).ToList();
        }

        [Fact]
        public void Test1() 
        {
            IEnumerable<string> strs = new List<string>
            {
                "123",
                "233",
                "2323"
            };

            StringBuilder str = new StringBuilder();
            str.Append("('");
            str.Append(string.Join("','", strs));
            str.Append("')");

            var x = str.ToString();
        }
    }
}
