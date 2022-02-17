using Ars.Common.AutoFac.Helper;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Dependency
{
    public class ArsPropertyRegisterProvider : IArsRegisterProvider
    {
        private readonly IOptions<PropertyAutowiredOption> _options;
        public ArsPropertyRegisterProvider(IOptions<PropertyAutowiredOption> options)
        {
            _options = options;
        }

        public void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies)
        {
            if (!_options.Value.Autowired)
                return;

            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired(new AutowiredPropertySelector());
        }
    }
}
