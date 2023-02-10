using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.Helper;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Autofac;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.RegisterProvider
{
    public interface IRegisterProviderFactory
    {
        void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies);

        void RegisterAutowaired(IContainer container);
    }

    public class RegisterProviderFactory : IRegisterProviderFactory
    {
        private readonly IEnumerable<IArsRegisterProvider> _arsRegisterProviders;
        private readonly IOptions<PropertyAutowiredOption> _options;
        public RegisterProviderFactory(IOptions<PropertyAutowiredOption> options,
            IEnumerable<IArsRegisterProvider> arsRegisterProviders)
        {
            _options = options;
            _arsRegisterProviders = arsRegisterProviders;
        }

        public void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies)
        {
            foreach (var provider in _arsRegisterProviders) 
            {
                provider.Register(builder, assemblies);
            }
        }

        public void RegisterAutowaired(IContainer container)
        {
            if (!_options.Value.Autowired)
                return;

            container.EnableAutowired();
        }
    }
}
