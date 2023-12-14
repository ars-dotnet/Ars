using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore.Options
{
    public class ArsHttpApiOptions : HttpApiOptions
    {
        /// <summary>
        /// 是否使用ars默认的httpclient
        /// </summary>
        public bool UseArsHttpClient { get; set; }

        /// <summary>
        /// 是否使用https
        /// 当<see cref="UseArsHttpClient=true"/>时，选择ars默认的HttpMessageHandler
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// 是否使用httpclient自定义策略
        /// 当<see cref="UseArsHttpClient=true"/>时，选择ars默认的[降级、熔断、超时、重试策略]
        /// </summary>
        public bool UseHttpClientCustomPolicy { get; set; }
    }
}
