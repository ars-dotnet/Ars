using Ocelot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    public class ArsRequestTimedOutError : Error
    {
        public ArsRequestTimedOutError(Exception exception) 
            : base($"Timeout making http request, exception: {exception}", 
                  OcelotErrorCode.RequestTimedOutError, 
                  504)
        {
        }
    }
}
