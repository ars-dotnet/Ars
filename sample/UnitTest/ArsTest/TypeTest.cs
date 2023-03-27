using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class TypeTest
    {
        [Fact]
        public void Test1()
        {
            Animals animals = new Animals();
            animals.arr = new object[] { 1, 2 };
            animals.names = new List<string>()
            {
                "123",
                "223"
            };

            var m =
                animals.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(r => typeof(System.Collections.IEnumerable).IsAssignableFrom(r.PropertyType) || r.PropertyType.IsArray);
            Assert.NotNull(m);
            foreach (var d in m)
            {
                if (d.PropertyType.IsArray)
                {
                    var ar = d.GetValue(animals) as Array;
                    continue;
                }

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(d.PropertyType))
                {
                    System.Collections.IEnumerable x = d.GetValue(animals) as System.Collections.IEnumerable;
                    continue;
                }
            }
        }

        [Fact]
        public void Test2() 
        {

        }
    }

    public class Animals
    {
        public object[] arr { get; set; }

        public IEnumerable<string> names { get; set; }

        public string a { get; set; }
    }
}
