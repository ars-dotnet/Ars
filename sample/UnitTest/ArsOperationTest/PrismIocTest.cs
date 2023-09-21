using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Microsoft.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class PrismIocTest
    {
        [Fact]
        public void Test1() 
        {
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            PrismContainerExtension.Init(services);

            var a = (IContainerProvider)PrismContainerExtension.Init(services);

            Assert.True(a.IsRegistered<IHttpClientFactory>());
        }

        [Fact]
        public void Test2() 
        {
            PrismContainerExtension.Current.RegisterServices(s =>
            {
                s.AddHttpClient();
            });
            Assert.True(((IContainerProvider)PrismContainerExtension.Current).IsRegistered<IHttpClientFactory>());
        }
    }
}
