using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.IDependency;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    internal class GrpcExceptionManager : IGrpcExceptionManager,ISingletonDependency
    {
        public (int, string) GetGrpcExceptionErr(Exception e)
        {
            var grpcexception = GetGrpcException(e);
            if (null == grpcexception) 
            {
                return (500, e.Message);
            }

            return (grpcexception.StatusCode.GetHttpStatusCode(), grpcexception.Status.Detail);
        }

        public bool IsGrpcException(Exception e)
        {
            return e is RpcException;
        }

        public RpcException? GetGrpcException(Exception e)
        {
            if(e is RpcException rpcException)
                return rpcException;

            return null;
        }
    }
}
