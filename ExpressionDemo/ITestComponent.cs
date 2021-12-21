using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    internal interface ITestComponent
    {
        /// <summary>
        /// 老的行为
        /// </summary>
        void Test();
    }

    internal class TestA : ITestComponent
    {
        public void Test()
        {
            Console.WriteLine("Test");
        }
    }

    internal interface ITestComponentA : ITestComponent 
    {
        /// <summary>
        /// 新的行为
        /// </summary>
        void Test1();
    }

    internal class TestB : ITestComponentA
    {
        private ITestComponent _testComponentA;
        public TestB(ITestComponent testComponentA)
        {
            _testComponentA = testComponentA;
        }

        public void Test()
        {
            _testComponentA.Test();
            this.Test1();
        }

        public void Test1()
        {
            Console.WriteLine("Test1");
        }
    }
}
