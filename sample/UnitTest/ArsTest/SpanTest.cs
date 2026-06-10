using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Tools;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class SpanTest
    {
        /// <summary>
        /// span split
        /// </summary>
        [Fact]
        public void TestSplit() 
        {
            string str = "aabb00121200,aabb00131300\0\0\0,aabb";
            var xx = SplitResult.SplitOptimized(str.AsSpan(), new[] { '\0', ',' }.AsSpan()).ToArray();

            string str1 = string.Empty;
            var xxx = SplitResult.SplitOptimized(str1.AsSpan(), new[] { '\0', ',' }.AsSpan()).ToArray();
        }

        [Fact]
        public void TestContainsAny() 
        {
            ReadOnlySpan<char> text = "hello world";

            string texts = "hello world";

            ReadOnlySpan<char> t1 = texts.AsSpan();

            Assert.True(text.ContainsAny('h', 'w')); //因为文本中有h、w，所以返回true

            Assert.True(t1.ContainsAnyInRange('a', 'e')); //因为文本中有d、e在a-e范围内，所以返回true

            Assert.False(t1.ContainsAnyInRange('a', 'c')); //因为文本中没有a、b、c，所以返回false
        }

        [Fact]
        public void TestIndexOfAny() 
        {
            ReadOnlySpan<char> span = "ofind multiple characters".AsSpan();
            ReadOnlySpan<char> searchSet = "aeiou".AsSpan();

            // .NET 8 中这些方法使用了 SIMD 优化
            int index = span.IndexOfAny(searchSet);  // 查找第一个元音 结果应该是 0，因为 'o' 是第一个元音
            int lastIndex = span.LastIndexOfAny(searchSet); // 查找最后一个元音 结果应该是 22，因为 'e' 是最后一个元音
        }

        [Fact]
        public void TestReplace()
        {
            // .NET 8 新增的 Replace 方法
            ReadOnlySpan<char> source = "hello world".AsSpan();
            Span<char> destination = stackalloc char[source.Length];

            // 原地替换字符
            source.Replace(destination, 'o', '0'); // "hell0 w0rld"

            Assert.True("hell0 w0rld".Equals(destination.ToString()));
        }

        [Fact]
        public void TestSubString() 
        {
            string str = "hello world";

            var a = str.AsSpan().Slice(6); //不分配内存

            Assert.True("world".Equals(a.ToString())); //ToString会分配内存

            List<object> list = new List<object>()
            {
                new { name = "bill" },
                new { name = "ars" }
            };

            Span<object> span = CollectionsMarshal.AsSpan(list); //不分配内存
        }

        [Fact]
        public void TestListToArray() 
        {
            List<object> list = new List<object>()
            {
                new { name = "bill" },
                new { name = "ars" }
            };

            // ✅ 从池中租用数组
            object[] pooledArray = ArrayPool<object>.Shared.Rent(2);

            try
            {
                list.CopyTo(pooledArray, 0);

                int a = pooledArray.Length;
            }
            finally
            {
                ArrayPool<object>.Shared.Return(pooledArray);
            }
        }
    }
}
