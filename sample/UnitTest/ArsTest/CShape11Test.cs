using Ars.Common.Tool.Extension;
using ArsTest.C_.Version.C_9._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    /// <summary>
    /// C#11
    /// </summary>
    public class CShape11Test
    {
        [Fact]
        public void Test1() 
        {
            TestXml testXml = new TestXml
            {
                macCode = "Tom",
                wipEntity = "China",
                Item = new List<Item>
                    {
                        new Item
                        {
                            TagCode = "top",
                            TagValue = "175",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "sex",
                            TagValue = "男",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "like",
                            TagValue = "music",
                            TimeStamp = "1212212"
                        },
                    }
            };
            string a = testXml.XMLSerialize();

            //原始字符串文本
            string ax = """
                <?xml version="1.0" encoding="utf-8"?>
                <CallAgv xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" macCode="Tom" wipEntity="China">
                  <item tagCode="top" tagValue="175" timeStamp="1212212" />
                  <item tagCode="sex" tagValue="男" timeStamp="1212212" />
                  <item tagCode="like" tagValue="music" timeStamp="1212212" />
                </CallAgv>
                """;

            var data = ax.DeXmlSerialize<TestXml>();

            Assert.Equal("Tom", data!.macCode);
        }
    }
}
