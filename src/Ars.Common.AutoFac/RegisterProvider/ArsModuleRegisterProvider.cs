using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.IDependency;
using Autofac;
using Autofac.Core;
using Autofac.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Dependency
{
    public class ArsModuleRegisterProvider : IArsRegisterProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public ArsModuleRegisterProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies)
        {
            foreach (var module in GetAllModules(assemblies))
            {
                builder.RegisterModule(module);
            }
        }

        private IEnumerable<ArsAutofacModule> GetAllModules(IReadOnlyCollection<Assembly> assemblies)
        {
            foreach (var type in assemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(r => r.IsArsModuleType()).Select(r => r.GetTypeInfo()))
            {
                var constructorInfo = type.DeclaredConstructors.FirstOrDefault();
                if (null == constructorInfo)
                    continue;

                var @params = constructorInfo.GetParameters()?.Select(r => _serviceProvider.GetService(r.ParameterType)).ToArray();
                yield return (constructorInfo.Invoke(@params) as ArsAutofacModule)!;
            }
        }
    }
}
