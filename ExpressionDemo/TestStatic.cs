using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    public class TestStatic
    {
        private static int count;
        public TestStatic()
        {
            count++;
        }

        static TestStatic() 
        {
            count = 20;
        }
    }
}
