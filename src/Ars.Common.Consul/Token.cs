using Ars.Common.Consul.GrpcHelper;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    internal class Token : IToken, ISingletonDependency
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SemaphoreSlim SemaphoreSlim;
        public Token(IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            SemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<string> GetToken(ConsulConfiguration option)
        {
            string value = string.Empty;
            if (option.Communication.UseIdentityServer4Valid)
            {
                value = await _memoryCache.GetOrCreateAsync(option.ServiceName, async entry =>
                {
                    return await SemaphoreSlim.LockAsync(async () =>
                    {
                        if (_memoryCache.TryGetValue(option.ServiceName, out value))
                        {
                            return value;
                        }

                        //获取token
                        IDictionary<string, string> dto = new Dictionary<string, string>
                        {
                            { "client_id",option.Communication.ClientId},
                            { "client_secret",option.Communication.ClientSecret},
                            { "scope",option.Communication.Scope},
                            { "grant_type",option.Communication.GrantType},
                        };

                        using var httpclient = _httpClientFactory.CreateClient(HttpClientNames.RetryHttps);
                        var reponse = await httpclient
                            .PostAsync(
                                $"{option.Communication.IdentityServer4Address.TrimEnd('/')}/connect/token",
                                new FormUrlEncodedContent(dto))
                            .ConfigureAwait(false);
                        reponse.EnsureSuccessStatusCode();
                        var token = JsonConvert.DeserializeObject<GrpcIdentityServer4Result>(
                            await reponse.Content.ReadAsStringAsync().ConfigureAwait(false))!;

                        entry.SlidingExpiration =
                            token.expires_in > 60
                            ? TimeSpan.FromSeconds(token.expires_in - 60)
                            : TimeSpan.FromSeconds(token.expires_in - 1);

                        return token.access_token;
                    });
                });
            }

            return value;
        }
    }
}
