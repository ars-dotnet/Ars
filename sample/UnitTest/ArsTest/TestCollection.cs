using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class TestArray
    {
        [Fact]
        public void TestArray1() 
        {
            int[] arrays = Array.Empty<int>();
            arrays = new int[2];
            arrays[0] = 1;
            arrays[1] = 2;

            //多维数组
            int[,] vs = new int[2, 3];
            vs[0, 0] = 1;
            vs[0, 1] = 2;
            vs[0, 2] = 3;
            vs[1, 0] = 4;
            vs[1, 1] = 5;
            vs[1, 2] = 6;

            //交错数组
            int[][] vs1 = new int[2][]
            {
                new int[]{1,2,3 },
                new int[]{1,2,3,4 },
            };
        }

        [Fact]
        public void TestList() 
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Insert(0, 1);
            arrayList.Insert(1, 2);
            arrayList.Insert(1, 3);

            List<int> vs = new List<int>();
            vs.Insert(0, 1);
            vs.Insert(1, 2);
            vs.Insert(1, 3);

            LinkedList<int> vs1 = new LinkedList<int>();
            var first = vs1.AddFirst(1);
            vs1.AddAfter(first, new LinkedListNode<int>(2));
            vs1.AddAfter(first, new LinkedListNode<int>(3));

            Hashtable hashtable = new Hashtable();
            hashtable.Add("123", "123");
            hashtable.Add("1234", "123");

            HashSet<int> vs2 = new HashSet<int>();
            vs2.Add(123);
            vs2.Add(234);
            vs2.Add(123);
        }

        class A 
        {
            internal string Atest() 
            {
                return "A";
            }
        }

        class B : A 
        {
            internal string Atest() 
            {
                return "B";
            }
        }

        class C : A 
        {
            internal new string Atest() 
            {
                return "C";
            }
        }
        [Fact]
        public void TestNewKeyWord() 
        {
            A a = new A();
            Assert.Equal("A", a.Atest());
            B b = new B();
            Assert.Equal("B", b.Atest());
            C c = new C();
            Assert.Equal("C", c.Atest());

            a = new B();
            Assert.Equal("A", a.Atest());
            a = new C();
            Assert.Equal("A", a.Atest());
        }
    }
}
