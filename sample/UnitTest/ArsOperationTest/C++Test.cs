using ArsOperationTest.CppTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class C__Test
    {
        [Fact]
        public void Test() 
        {
            Assert.True(20 == CppFunctions.Add(10,10));

            string message = "Hello from C#!";

            int length = CppFunctions.ProcessString(message);

            MyPoint p = new MyPoint { X = 1, Y = 2 };

            var point = CppFunctions.MovePoint(p, 5, 8);
        }
    }
}
