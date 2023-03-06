using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class EqualsTest
    {
        [Fact]
        public void TestEquals() 
        {
            //值类型 比较值
            int a = 1;
            int b = 1;
            int codea = a.GetHashCode();
            int codeb = b.GetHashCode();
            Assert.True(a == b);
            Assert.True(a.Equals(b));
            Assert.True(Equals(a,b));
            Assert.False(ReferenceEquals(a,b));

            //引用类型 
            //string 
            string s1 = "abc";
            string s2 = "abc";
            string s3 = new string("abc");
            string s4 = "abcc";

            var codes1 = s1.GetHashCode();
            var codes2 = s2.GetHashCode();
            var codes3 = s3.GetHashCode();
            var code4 = s4.GetHashCode();

            //比较值
            Assert.True(codes1 == codes2 && codes1 == codes3 && codes1 != code4); 
            Assert.True(s1 == s2);
            Assert.True(s1 == s3);
            Assert.False(s1 == s4);

            //比较值
            Assert.True(s1.Equals(s2));
            Assert.True(s1.Equals(s3));
            Assert.False(s1.Equals(s4));

            //比较值
            Assert.True(Equals(s1,s2));
            Assert.True(Equals(s1,s3));
            Assert.False(Equals(s1,s4));

            //比较实例  
            Assert.True(ReferenceEquals(s1, s2));
            Assert.False(ReferenceEquals(s1, s3));
            Assert.False(ReferenceEquals(s1, s4));

            //非string 比较实例
            Animalss ani1 = new Animalss("Dog");
            Animalss ani2 = ani1;
            Animalss ani3 = new Animalss("Dog");
            Animalss ani4 = new Animalss("Tom");

            Assert.True(ani1 == ani2);
            Assert.False(ani1 == ani3);
            Assert.False(ani1 == ani4);

            Assert.True(ani1.Equals(ani2));
            Assert.False(ani1.Equals(ani3));
            Assert.False(ani1.Equals(ani4));

            Assert.True(Equals(ani1, ani2));
            Assert.False(Equals(ani1, ani3));
            Assert.False(Equals(ani1, ani4));

            Assert.True(ReferenceEquals(ani1, ani2));
            Assert.False(ReferenceEquals(ani1, ani3));
            Assert.False(ReferenceEquals(ani1, ani4));

            Assert.False(true ^ true);
            Assert.True(false ^ true);
            Assert.False(false ^ false);
        }
    }

    public class Animalss
    {
        public Animalss(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
