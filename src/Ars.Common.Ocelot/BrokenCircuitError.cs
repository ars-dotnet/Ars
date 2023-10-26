using Ocelot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    public class BrokenCircuitError : Error
    {
        public BrokenCircuitError(Exception exception): base($"Breaker making http request, exception: {exception}", OcelotErrorCode.RequestTimedOutError, 504)
        {
            
        }
    }
}
