using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest.C_.Version.C_9._0
{
    public class RecordTest
    {
        [Fact]
        public void Test1() 
        {
            Person person = new Person("Tom", 29,new int[] { 123,223 });
            person.arr[1] = 666;

            Address address = new Address(12.3,678.9);
            Address address1 = new Address(12.3, 678.9);

            Assert.True(address == address1);
            Assert.True(address.Equals(address1));
            Assert.True(Equals(address,address1));
            Assert.False(ReferenceEquals(address,address1)); //装箱

            Address address2 = address1 with { xPoint = 12.000 };
            Assert.True(address2.xPoint == 12.000 );
            Assert.True(address1.xPoint == 12.3);

            Address address3 = address1 with { };
            Assert.True(address3.xPoint == 12.3);

            Person person1 = person with { arr = new int[1] { 666 } };
            Assert.True(person.arr[0] == 123);
        }

        [Fact]
        public void Test2() 
        {
            Test t = new Test();

            //访问结构体对象的副本 
            int a = t.m.Mutate();
            int b = t.m.Mutate();
            int c = t.m.Mutate();

            Assert.True(1 == a && a == b && b == c);

            Mutable mutable = new Mutable();
            mutable.x += 2;

            Mutable mutable1 = mutable;
            Assert.True(mutable1.x == 2);
        }
    }

    public record Address(double xPoint, double yPoint);

    public record struct Person(string name,int age, int[] arr);

    struct Mutable
    {
        public int x;

        public int Mutate()
        {
            this.x = this.x + 1;
            return this.x;
        }
    }

    class Test
    {
        /// <summary>
        /// 访问只读结构体属性，得到的是它的副本，对副本修改，不会影响到原来的值
        /// </summary>
        public readonly Mutable m;

        public readonly int a;

        public Test()
        {
            m = new Mutable();
        }
    }
}
