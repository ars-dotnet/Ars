using Ars.Commom.Tool.Extension;
using Ars.Common.Consul;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Ars.Common.RpcClientCore.Attributes
{
    /// <summary>
    /// 获取token
    /// </summary>
    public class TokenAttribute : ApiActionAttribute
    {
        public TokenAttribute()
        {
            OrderIndex = 106;
        }

        public override async Task OnRequestAsync(ApiRequestContext context)
        {
            string token = await GetTokenAsync(context);
            if (token.IsNullOrEmpty())
                return;

            context.HttpContext.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",token);
        }

        private async Task<string> GetTokenAsync(ApiRequestContext context) 
        {
            IServiceProvider serviceProvider = context.HttpContext.ServiceProvider;

            IConsulDiscoverConfiguration consulDiscover =
                serviceProvider.GetRequiredService<IConsulDiscoverConfiguration>();

            if (!context.Properties.TryGetValue("serviceName", out string? serviceName)) 
            {
                return string.Empty;
            }

            var option = consulDiscover.ConsulDiscovers.FirstOrDefault(
                r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            if (null == option) 
            {
                return string.Empty;
            }

            IToken tokenservice = serviceProvider.GetRequiredService<IToken>();

            return await tokenservice.GetToken(option);
        }
    }
}
