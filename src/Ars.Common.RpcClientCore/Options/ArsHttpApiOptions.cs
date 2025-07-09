﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore.Options
{
    /// <summary>
    /// 舍弃
    /// <see cref="Ars.Common.Core.Extensions.HttpClientExtension.AddArsHttpClient">此方法多次调用时，策略会重复叠加</see>
    /// </summary>
    public class ArsHttpApiOptions : HttpApiOptions
    {
        /// <summary>
        /// 是否使用ars默认的httpclient
        /// </summary>
        [Obsolete]
        public bool UseArsHttpClient { get; set; }

        /// <summary>
        /// 是否使用https
        /// 当<see cref="UseArsHttpClient=true"/>时，选择ars默认的HttpMessageHandler
        /// </summary>
        [Obsolete]
        public bool UseHttps { get; set; }

        /// <summary>
        /// 是否使用httpclient自定义策略
        /// 当<see cref="UseArsHttpClient=true"/>时，选择ars默认的[降级、熔断、超时、重试策略]
        /// </summary>
        [Obsolete]
        public bool UseHttpClientCustomPolicy { get; set; }
    }
}
