using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticsearchUtil.Provider
{
    internal class ElasticSearchProvider : BaseElasticSearchProvider, IElasticSearchProvider
    {
        public ElasticSearchProvider(IOptions<ElasticSearchConfig> options) : base(options)
        {

        }

        public override Task<ElasticClient> GetEsClient()
        {
            var url = _options.Value.EsUrl;
            ElasticClient _client = new ElasticClient(new Uri(url));

            return Task.FromResult(_client);
        }

        protected override Task Check()
        {
            return Task.CompletedTask;
        }
    }
}
