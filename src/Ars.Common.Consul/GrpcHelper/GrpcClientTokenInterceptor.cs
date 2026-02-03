using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ocsp;
using Polly;
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

        private readonly ILogger<GrpcClientTokenInterceptor> _logger;
        public GrpcClientTokenInterceptor(
            ConsulConfiguration consulConfiguration, 
            IGrpcMetadataTokenProvider grpcMetadataTokenProvider,
            ILoggerFactory loggerFactory)
        {
            _consulConfiguration = consulConfiguration;
            _grpcMetadataTokenProvider = grpcMetadataTokenProvider;

            _logger = loggerFactory.CreateLogger<GrpcClientTokenInterceptor>();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context, 
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return new AsyncUnaryCall<TResponse>(
                ExecuteWithAuthAsync(request, context, continuation),
                GetMetadata(context),
                () => new Status(StatusCode.OK, "OK"),
                () => new Metadata(),
                () => { });
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = GetMetadata(context).GetAwaiter().GetResult();

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

            return continuation(context);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
            TRequest request, 
            ClientInterceptorContext<TRequest, TResponse> context, 
            AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = GetMetadata(context).GetAwaiter().GetResult();

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

            return continuation(request,context);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = GetMetadata(context).GetAwaiter().GetResult();

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

            return continuation(context);
        }

        private async Task<TResponse> ExecuteWithAuthAsync<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            var headers = await GetMetadata(context);

            var newOptions = context.Options.WithHeaders(headers);

            var newContext = new ClientInterceptorContext<TRequest, TResponse>(
                context.Method, context.Host, newOptions);

            try
            {
                var call = continuation(request, newContext);

                return await call.ResponseAsync.ConfigureAwait(false);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning("Token expired or invalid, refreshing...");

                headers = await GetMetadata(context);

                newOptions = context.Options.WithHeaders(headers);

                newContext = new ClientInterceptorContext<TRequest, TResponse>(
                    context.Method, context.Host, newOptions);

                var retryCall = continuation(request, newContext);

                return await retryCall.ResponseAsync.ConfigureAwait(false);
            }
        }

        protected virtual async Task<Metadata> GetMetadata<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context)
             where TRequest : class
             where TResponse : class
        {
            Metadata? metadata = null;

            if (_consulConfiguration.Communication.UseIdentityServer4Valid)
            {
                metadata = await _grpcMetadataTokenProvider.GetMetadataToken(_consulConfiguration);
            }

            if (null != metadata)
            {
                if (context.Options.Headers.HasValue())
                {
                    foreach (var entry in metadata)
                    {
                        context.Options.Headers?.Add(entry.Key, entry.Value);
                    }

                    metadata = context.Options.Headers;
                }
            }

            return metadata ?? new Metadata();
        }
    }
}
