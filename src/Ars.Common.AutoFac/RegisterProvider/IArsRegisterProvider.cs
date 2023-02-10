using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.IDependency
{
    public interface IArsRegisterProvider
    {
        void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies);
    }
}
