using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Tools
{
    public class HttpClientNames
    {
        /// <summary>
        /// webapi http1调用
        /// </summary>
        public const string Http = "Http";

        /// <summary>
        /// webapi http1 ssl调用
        /// </summary>
        public const string Https = "Https";

        /// <summary>
        /// webapi http1调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryHttp = "RetryHttp";

        /// <summary>
        /// webapi http1 ssl调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryHttps = "RetryHttps";

        /// <summary>
        /// grpc http1调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryGrpcHttpV1 = "RetryGrpcHttpV1";

        /// <summary>
        /// grpc http1 ssl调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryGrpcHttpsV1 = "RetryGrpcHttpsV1";

        /// <summary>
        /// grpc http2调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryGrpcHttpV2 = "RetryGrpcHttpV2";

        /// <summary>
        /// grpc http2 ssl调用
        /// 拥有降级 熔断 超时 重试策略
        /// </summary>
        public const string RetryGrpcHttpsV2 = "RetryGrpcHttpsV2";
    }
}
