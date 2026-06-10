using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Tools
{
    public readonly ref struct SplitResult
    {
        private readonly ReadOnlySpan<char> _original;
        private readonly int[] _segments;

        public SplitResult(ReadOnlySpan<char> original, int[] segments)
        {
            _original = original;
            _segments = segments;
        }

        public int Count => _segments.Length / 2;

        public ReadOnlySpan<char> this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int start = _segments[index * 2];
                int length = _segments[index * 2 + 1];
                return _original.Slice(start, length);
            }
        }

        public string[] ToArray()
        {
            var result = new string[Count];
            for (int i = 0; i < Count; i++)
            {
                result[i] = this[i].ToString();
            }
            return result;
        }

        // 使用方法
        public static SplitResult SplitOptimized(ReadOnlySpan<char> input, ReadOnlySpan<char> separators)
        {
            if (input.IsEmpty)
                return new SplitResult(input, Array.Empty<int>());

            // 估算分段数量
            int estimatedSegments = 1;
            for (int i = 0; i < input.Length; i++)
            {
                if (separators.IndexOf(input[i]) >= 0)
                    estimatedSegments++;
            }

            // 存储 [start, length] 对
            var segments = new int[estimatedSegments * 2];
            int segmentCount = 0;
            int start = 0;

            for (int i = 0; i <= input.Length; i++)
            {
                if (i == input.Length || separators.IndexOf(input[i]) >= 0)
                {
                    if (i > start)
                    {
                        if (segmentCount >= segments.Length)
                        {
                            Array.Resize(ref segments, segments.Length * 2);
                        }

                        segments[segmentCount++] = start;
                        segments[segmentCount++] = i - start;
                    }
                    start = i + 1;
                }
            }

            Array.Resize(ref segments, segmentCount);
            return new SplitResult(input, segments);
        }
    }
}
