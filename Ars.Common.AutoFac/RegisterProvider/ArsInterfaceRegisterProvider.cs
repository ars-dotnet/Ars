using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.IDependency;
using Autofac;
using Autofac.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Dependency
{
    public class ArsInterfaceRegisterProvider : IArsRegisterProvider
    {
        public void Register(ContainerBuilder builder, IReadOnlyCollection<Assembly> assemblies)
        {
            Type[] ignore = { typeof(IDisposable), typeof(IArsDependency) };
            Array.ForEach(assemblies.SelectMany(assembly => assembly.GetLoadableTypes()).
                Where(r => r.IsArsRegisterInterfaceType()).ToArray(), r =>
                {
                    if (typeof(ITransientDependency).IsAssignableFrom(r))
                    {
                        if (r.BaseType?.IsAbstract ?? false)
                        {
                            builder.RegisterType(r).As(r.BaseType!).InstancePerDependency();
                        }
                        if (r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(ITransientDependency) != t).Any())
                        {
                            foreach (var i in r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(ITransientDependency) != t))
                            {
                                builder.RegisterType(r).As(i).InstancePerDependency();
                            }
                        }
                        else
                        {
                            builder.RegisterType(r).AsSelf().InstancePerDependency();//.PropertiesAutowired(new AutowiredPropertySelector());
                        }
                    }
                    else if (typeof(ISingletonDependency).IsAssignableFrom(r))
                    {
                        if (r.BaseType?.IsAbstract ?? false)
                        {
                            builder.RegisterType(r).As(r.BaseType!).SingleInstance();
                        }
                        if (r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(ISingletonDependency) != t).Any())
                        {
                            foreach (var i in r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(ISingletonDependency) != t))
                            {
                                builder.RegisterType(r).As(i).SingleInstance();
                            }
                        }
                        else
                        {
                            builder.RegisterType(r).AsSelf().SingleInstance();
                        }
                    }
                    else if(typeof(IScopedDependency).IsAssignableFrom(r))
                    {
                        if (r.BaseType?.IsAbstract ?? false)
                        {
                            builder.RegisterType(r).As(r.BaseType!).InstancePerLifetimeScope();
                        }
                        if (r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(IScopedDependency) != t).Any())
                        {
                            foreach (var i in r.GetInterfaces().Where(t => !ignore.Contains(t) && typeof(IScopedDependency) != t))
                            {
                                builder.RegisterType(r).As(i).InstancePerLifetimeScope();
                            }
                        }
                        else
                        {
                            builder.RegisterType(r).AsSelf().InstancePerLifetimeScope();
                        }
                    }
                });
        }
    }
}
