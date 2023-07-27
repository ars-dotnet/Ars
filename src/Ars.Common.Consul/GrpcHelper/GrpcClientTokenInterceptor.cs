using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    public class GrpcClientTokenInterceptor : Interceptor
    {
        private readonly ConsulConfiguration _consulConfiguration;
        private readonly IGrpcMetadataTokenProvider _grpcMetadataTokenProvider;
        public GrpcClientTokenInterceptor(
            ConsulConfiguration consulConfiguration, 
            IGrpcMetadataTokenProvider grpcMetadataTokenProvider)
        {
            _consulConfiguration = consulConfiguration;
            _grpcMetadataTokenProvider = grpcMetadataTokenProvider;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context, 
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Metadata? metadata = null;
            if (_consulConfiguration.Communication.UseIdentityServer4Valid) 
            {
                metadata = _grpcMetadataTokenProvider.GetMetadataToken(_consulConfiguration)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            
            if (null != metadata) 
            {
                if (context.Options.Headers.HasValue()) 
                {
                    foreach (var entry in metadata)
                    {
                        context.Options.Headers.Add(entry.Key, entry.Value);
                    }

                    metadata = context.Options.Headers;
                }

                context =
                    new ClientInterceptorContext<TRequest, TResponse>(
                        context.Method,
                        context.Host,
                        new CallOptions(
                            metadata,
                            context.Options.Deadline,
                            context.Options.CancellationToken,
                            context.Options.WriteOptions,
                            context.Options.PropagationToken,
                            context.Options.Credentials));
            }

            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}
