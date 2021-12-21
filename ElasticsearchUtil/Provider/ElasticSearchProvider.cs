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
        private ElasticClient _client;
        internal int i;
        public ElasticSearchProvider(IOptions<ElasticSearchConfig> options) : base(options)
        {

        }

        public override Task<ElasticClient> GetEsClient()
        {
            if (null != _client)
                Task.FromResult(_client);

            Interlocked.Increment(ref i);

            if (null != _client)
                Task.FromResult(_client);

            var url = _options.Value.EsUrl;
            _client = new ElasticClient(new Uri(url));


            return Task.FromResult(_client);
        }

        protected override Task Check()
        {
            return Task.CompletedTask;
        }
    }
}
