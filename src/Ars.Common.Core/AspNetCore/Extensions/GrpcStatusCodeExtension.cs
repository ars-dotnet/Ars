using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Extensions
{
    public static class GrpcStatusCodeExtension
    {
        public static int GetHttpStatusCode(this StatusCode statusCode) 
        {
            switch (statusCode) 
            {
                case StatusCode.OK:
                    return 200;
                case StatusCode.Unauthenticated:
                    return 401;
                case StatusCode.PermissionDenied:
                    return 403;
                case StatusCode.NotFound:
                    return 404;
                default:
                    return 500;
            }
        } 
    }
}
