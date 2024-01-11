using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool
{
    public static class Valid
    {
        public static void ThrowException(string message)
        {
            ThrowException(true, message);
        }

        public static void ThrowException(int code, string message)
        {
            ThrowException(true, code, message);
        }

        public static void ThrowException(bool v, string message)
        {
            if (v)
                throw new ArsException(message);
        }

        public static void ThrowException(bool v, int code, string message)
        {
            if (v)
                throw new ArsException(code, message);
        }
    }
}
