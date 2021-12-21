using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchUtil.Provider
{
    public interface IElasticSearchProvider
    {
        Task<ElasticClient> GetEsClient();
    }
}
