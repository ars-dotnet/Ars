using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class ExceptionExtension
    {
        public static string GetInnerExceptionMessage(this Exception exception)
        {
            return GetInnerException(exception).Message;
        }

        public static Exception GetInnerException(this Exception exception)
        {
            Exception exp = exception;
            while (exp.InnerException != null)
            {
                exp = exp.InnerException;
            }

            return exp;
        }
    }
}
