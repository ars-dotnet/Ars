using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public interface IWebApiClientCoreExceptionManager : ISingletonDependency
    {
        bool IsWebApiClientCoreApiResponseStatusException(Exception e);

        (int, string) GetWebApiClientCoreApiResponseStatusExceptionErr(Exception e);
    }
}
