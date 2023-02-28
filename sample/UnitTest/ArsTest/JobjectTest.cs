using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class JobjectTest
    {
        [Fact]
        public void Test1() 
        {
            JObject obj = new();

            obj.Add("name", "bill");
            obj.Add("age", 12);

            object a = JsonConvert.DeserializeObject<object>(obj.ToString());
        }

        [Fact]
        public void Test2() 
        {
            dynamic a = new System.Dynamic.ExpandoObject();
            a.name = "bill";
            a.age = 123;

            var xx = a;
        }

        [Fact]
        public void Test3() 
        {
            dynamic a = new { name = "bill" };
            Assert.Throws<Exception>(a.age = 123);
        }
    }
}
