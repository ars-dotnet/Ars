using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsOperationTest.CppTool
{
    public static class CppFunctions
    {
        public static int Add(int a, int b)
            => NativeMethods.add(a, b);

        public static int ProcessString(string text)
            => NativeMethods.process_string(text);

        public static MyPoint MovePoint(MyPoint pt, int dx, int dy)
        {
            NativeMethods.move_point(ref pt, dx, dy);
            return pt;
        }
    }
}
