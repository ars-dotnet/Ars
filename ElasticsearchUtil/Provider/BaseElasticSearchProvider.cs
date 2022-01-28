using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchUtil.Provider
{
    internal abstract class BaseElasticSearchProvider : IElasticSearchProvider
    {
        internal IOptions<ElasticSearchConfig> _options;
        
        public BaseElasticSearchProvider(IOptions<ElasticSearchConfig> options)
        {
            _options = options;
        }

        public virtual Task<ElasticClient> GetEsClient() 
        {
            Check();
            return Task.FromResult(new ElasticClient());
        }

        protected abstract Task Check();
    }
}
