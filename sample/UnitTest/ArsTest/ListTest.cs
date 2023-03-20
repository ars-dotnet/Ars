using Ars.Commom.Tool.Extension;
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
            List<int> a = new List<int> { 1, 2, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 1, 2, 5, 6, 7 };

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

        [Fact]
        public void Test2()
        {
            var a = this.Get(1);

            var m = GetObj();
            var mm = m.As<Data>();

            var c = GetList().As<IEnumerable<int>>();
        }

        private IEnumerable<int> Get(int i)
        {
            if (0 == i)
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
        }

        private object GetObj()
        {
            return new Data
            {
                A = new List<string>(){ "aa", "N" },
                Name = "Bill"
            };
        }

        private Object GetList() 
        {
            return Get(0);
        }
    }

    public class Data 
    {
        public IEnumerable<string> A { get; set; }

        public string Name { get; set; }
    }
}
