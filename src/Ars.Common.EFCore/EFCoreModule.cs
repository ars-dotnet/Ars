using Ars.Common.AutoFac;
using Ars.Common.EFCore.Entities;
using Ars.Common.EFCore.Extension;
using Ars.Common.EFCore.Repository;
using Ars.Common.Tool.Extension;
using Autofac;
using Autofac.Core;
using Autofac.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore
{
    internal class EFCoreModule : ArsAutofacModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            Type repoType = typeof(IRepository<>);
            Type repoTypeTwo = typeof(IRepository<,>);
            Type implrepoTypeTwo = typeof(EfCoreRepositoryBase<,>);
            Type implrepoTypeThree = typeof(EfCoreRepositoryBase<,,>); 
            Type? servicetype = null;
            Type? implementtype = null;
            Type? keytype = null;

            Array.ForEach(
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(r => r.GetLoadableTypes())
                .Where(r => r.IsArsDbContextType())
                .Select(r => r.GetTypeInfo())
                .ToArray(),
                r =>
                {
                    foreach(var info in r.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(t => typeof(DbSet<>).IsAssignableGenericFrom(t.PropertyType) &&
                                 typeof(IEntity<>).IsAssignableGenericFrom(t.PropertyType.GetGenericArguments()[0]))
                     .Select(r => new { DeclaringType = r.DeclaringType!, EntityType = r.PropertyType.GetGenericArguments()[0] })) 
                    {
                        keytype = info.EntityType.GetInterfaces()
                            .FirstOrDefault(r => typeof(IEntity<>).IsAssignableGenericFrom(r))!
                            .GetGenericArguments()[0];
                        if (typeof(int) == keytype) 
                        {
                            servicetype = repoType.MakeGenericType(info.EntityType);
                            if (!builder.ComponentRegistryBuilder.IsRegistered(new TypedService(servicetype)))
                            {
                                implementtype = implrepoTypeTwo.MakeGenericType(info.DeclaringType,info.EntityType);
                                builder.RegisterType(implementtype).As(servicetype).InstancePerDependency();
                            }
                        };

                        servicetype = repoTypeTwo.MakeGenericType(info.EntityType,keytype);
                        if (!builder.ComponentRegistryBuilder.IsRegistered(new TypedService(servicetype)))
                        {
                            implementtype = implrepoTypeThree.MakeGenericType(info.DeclaringType, info.EntityType,keytype);
                            builder.RegisterType(implementtype).As(servicetype).InstancePerDependency();
                        }
                    }
                });
        }
    }
}
