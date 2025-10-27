using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    /// <summary>
    /// C#12
    /// </summary>
    public class CShape12Test
    {
        /// <summary>
        /// 集合表达式
        /// </summary>
        [Fact]
        public void Test1()
        {
            int[] a = [1, 2, 3];
            int[] b = [4, 5, 6];

            //..e 添加该表达式中的所有元素
            int[] c = [.. a, .. b];

            Assert.Equal(6, c.Length);
        }

        /// <summary>
        /// 主构造函数
        /// </summary>
        [Fact]
        public void Test2()
        {
            Widget a = new Widget();

            Widget b = new Widget("xxx",1,2,3);

            Assert.Equal(1, a.Volume);
            Assert.Equal(6, b.Volume);
        }

        /// <summary>
        /// 内联数组
        /// </summary>
        [Fact]
        public void Test3() 
        {
            Buffer buffer = new Buffer();

            for (int i = 0; i < 10; i++)
            {
                buffer[i] = i;
            }

            Assert.Equal(9, buffer[9]);
        }

        /// <summary>
        /// 模式
        /// </summary>
        [Fact]
        public void Test4() 
        {
            Widget b = new Widget("xxx", 1, 2, 3);

            string action(Widget x) => x.Volume switch
            {
                < 1 => "Too low",
                > 1 and < 3 => "Good",
                >= 3 => "Not Good",
                _ => "None"
            };

            Assert.Equal("Not Good", action(b));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="depth"></param>
    public class Widget(string name, int width, int height, int depth)
    {
        public Widget() : this("N/A", 1, 1, 1) { } // unnamed unit cube

        public int WidthInCM => width;
        public int HeightInCM => height;
        public int DepthInCM => depth;

        public int Volume => width * height * depth;
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.CompilerServices.InlineArray(10)]
    public struct Buffer 
    {
        private int size;
    }
}
