using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class RegularTest
    {
        [Fact]
        public void TestA() 
        {
            Regex regex = new Regex("youtu");
            Assert.Matches(regex, "https://youtu.be/rFq19GdZRC8");
            Assert.Matches(regex, "https://www.youtube.com/watch?v=AWa5DDAl6g8");
            //Assert.Matches(regex,"123");
        }
    }
}
