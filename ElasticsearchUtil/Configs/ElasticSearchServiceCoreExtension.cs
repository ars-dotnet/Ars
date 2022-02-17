using ElasticsearchUtil.Provider;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyArsenal.Commom.Tool;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchUtil.Configs
{
    public class ElasticSearchServiceCoreExtension : IArsServiceCoreExtension
    {
        private Action<ElasticSearchConfig> _action;
        public ElasticSearchServiceCoreExtension([NotNull]Action<ElasticSearchConfig> action)
        {
            _action = action;
        }

        public void AddService(IServiceCollection service)
        {
            service.Configure(_action);
            service.AddSingleton<IElasticSearchProvider, ElasticSearchProvider>();
        }
    }
}
