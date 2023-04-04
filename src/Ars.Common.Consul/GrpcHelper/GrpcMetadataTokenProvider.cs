using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    internal class GrpcMetadataTokenProvider : IGrpcMetadataTokenProvider, ISingletonDependency
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SemaphoreSlim SemaphoreSlim;
        public GrpcMetadataTokenProvider(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            SemaphoreSlim = new SemaphoreSlim(1,1);
        }

        public virtual async Task<Metadata?> GetMetadataToken(ConsulConfiguration option)
        {
            Metadata? entries = null;
            if (option.UseIdentityServer4Valid)
            {
                entries = new Metadata();
                string value = await _memoryCache.GetOrCreateAsync(option.ServiceName, async entry =>
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
                            { "client_id",option.ClientId},
                            { "client_secret",option.ClientSecret},
                            { "scope",option.Scope},
                            { "grant_type",option.GrantType},
                        };
                        using var httpclient = _httpClientFactory.CreateClient("http");
                        var reponse = await httpclient.PostAsync(
                            $"{option.IdentityServer4Address.TrimEnd('/')}/connect/token", 
                            new FormUrlEncodedContent(dto)).ConfigureAwait(false);
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

                entries.Add("Authorization", $"Bearer {value}");
            }

            return entries;
        }
    }
}
