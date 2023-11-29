using HslCommunication.Profinet.Melsec.Helper;
using MathNet.Numerics.Providers.SparseSolver;
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


        [Fact]
        public void TestB()
        {
            var a = System.Text.RegularExpressions.Regex.Replace("{1060}xx", @"[^0-9]+", "");
            Assert.True(a.Equals("1060"));
        }
    }

}
