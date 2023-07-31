using Ars.Common.Core.IDependency;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap
{
    internal class ArsConsumerServiceSelector : ConsumerServiceSelector,ISingletonDependency
    {
        public ArsConsumerServiceSelector(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {

        }

        protected override IEnumerable<ConsumerExecutorDescriptor> FindConsumersFromInterfaceTypes(IServiceProvider provider)
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();

            using (var scoped = provider.CreateScope())
            {
                var scopedProvider = scoped.ServiceProvider;
                var consumerServices = scopedProvider.GetServices<ICapSubscribe>();
                foreach (var service in consumerServices)
                {
                    executorDescriptorList.AddRange(GetTopicAttributesDescription(service.GetType().GetTypeInfo()));
                }
            }

            return executorDescriptorList;
        }
    }
}
