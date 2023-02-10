using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    public interface IGrpcClientProvider
    {
        Task<T> GetGrpcClient<T>(string serviceName) where T : ClientBase<T>;
    }
}
