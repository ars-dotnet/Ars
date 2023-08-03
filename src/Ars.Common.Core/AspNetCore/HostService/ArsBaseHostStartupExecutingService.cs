using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.HostService
{
    public abstract class ArsBaseHostStartupExecutingService : ArsBaseExecutingService, IArsHostStartupExecutingService
    {
        protected ArsBaseHostStartupExecutingService(ILoggerFactory loggerFactory) 
            : base(loggerFactory)
        {
        }
    }
}
