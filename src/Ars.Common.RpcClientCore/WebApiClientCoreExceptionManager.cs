using Ars.Common.Core.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace Ars.Common.RpcClientCore
{
    internal class WebApiClientCoreExceptionManager : IWebApiClientCoreExceptionManager
    {
        public (int, string) GetWebApiClientCoreApiResponseStatusExceptionErr(Exception e)
        {
            ApiResponseStatusException? exception = e as ApiResponseStatusException;

            return null == exception
                ? (500, e.Message)
                : ((int)exception.StatusCode, exception.Message);  
        }

        public bool IsWebApiClientCoreApiResponseStatusException(Exception e)
        {
            return e is ApiResponseStatusException;
        }
    }
}