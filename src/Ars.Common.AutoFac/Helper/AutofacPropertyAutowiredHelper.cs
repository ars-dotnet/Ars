using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.IDependency;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Helper
{
    /// <summary>
    /// Autofac属性注入帮助类
    /// </summary>
    internal static class AutofacPropertyAutowiredHelper
    {
        internal static void UseAutowired(IComponentContext scope)
        {
            var services = GetComponentRegistryServices(scope.ComponentRegistry);
            var obj = GetAllRegistration(services);
            UsePropertiesAutowired(services, obj, new InjectPropertySelector());
        }

        private static readonly Lazy<Func<ComponentRegistration, IResolvePipelineBuilder>> GetResolvePipelineBuilderFunc
            = new(() =>
            {
                var tar = Expression.Label(typeof(IResolvePipelineBuilder));

                var lateBuildPipelineField = typeof(ComponentRegistration).GetField("_lateBuildPipeline",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                var parameter = Expression.Parameter(typeof(ComponentRegistration));
                var lateBuildPipelineExpression = Expression.Field(parameter, lateBuildPipelineField!);

                var block = Expression.Block(Expression.Return(tar, lateBuildPipelineExpression),
                    Expression.Label(tar, Expression.Constant(null, typeof(IResolvePipelineBuilder))));

                return Expression.Lambda<Func<ComponentRegistration, IResolvePipelineBuilder>>(block, parameter)
                    .Compile();
            });

        private static void UsePropertiesAutowired(IComponentRegistryServices services,
            IEnumerable<IComponentRegistration> registrations,
            IInjectPropertySelector propertySelector, bool allowCircularDependencies = false)
        {
            foreach (var componentRegistration in GetComponentRegistrations(registrations))
            {
                const string descriptor = nameof(UsePropertiesAutowired);
                var resolvePipelineBuilder = GetResolvePipelineBuilderFunc.Value(componentRegistration);

                if (resolvePipelineBuilder.Middleware.Any(m => m.ToString() == descriptor))
                    continue;

                resolvePipelineBuilder.Use(descriptor,
                    PipelinePhase.Activation,
                    (ctxt, next) =>
                    {
                        // Continue down the pipeline.
                        next(ctxt);

                        if (!ctxt.NewInstanceActivated)
                        {
                            return;
                        }

                        if (allowCircularDependencies)
                        {
                            var capturedInstance = ctxt.Instance;

                            // If we are allowing circular deps, then we need to run when all requests have completed (similar to Activated).
                            ctxt.RequestCompleting += (o, args) =>
                            {
                                var evCtxt = args.RequestContext;
                                InjectProperties(evCtxt, capturedInstance!, propertySelector,
                                    evCtxt.Parameters);
                            };
                        }
                        else
                        {
                            InjectProperties(ctxt, ctxt.Instance!, propertySelector,
                                ctxt.Parameters);
                        }
                    });


                Rebuild(services, componentRegistration);
            }
        }

        private static IEnumerable<ComponentRegistration> GetComponentRegistrations(
            IEnumerable<IComponentRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                var registrationTemp = registration;

            checkAndReturn:
                if (registrationTemp is ComponentRegistration componentRegistration)
                {
                    yield return componentRegistration;
                }
                else
                {
                    registrationTemp = GetComponentRegistration.Value(registrationTemp);

                    goto checkAndReturn;
                }
            }
        }

        private static Lazy<Func<IComponentRegistration, IComponentRegistration>> GetComponentRegistration =>
            new Lazy<Func<IComponentRegistration, IComponentRegistration>>(() =>
            {
                var assembly = typeof(IComponentRegistry).Assembly;
                var type = assembly.GetType("Autofac.Core.Registration.ComponentRegistrationLifetimeDecorator") ??
                           throw new NullReferenceException();
                var field = type.GetField("_inner", BindingFlags.Instance | BindingFlags.NonPublic) ??
                            throw new NullReferenceException();

                var tar = Expression.Label(typeof(IComponentRegistration));

                var parameter = Expression.Parameter(typeof(IComponentRegistration));

                var inner = Expression.Field(Expression.TypeAs(parameter, type), field);
                var ifTrue = Expression.IfThen(Expression.TypeIs(parameter, type), Expression.Return(tar, inner));

                var block = Expression.Block(ifTrue,
                    Expression.Label(tar, parameter));

                var func = Expression.Lambda<Func<IComponentRegistration, IComponentRegistration>>(block, parameter)
                    .Compile();
                return func;
            });

        private static readonly Lazy<Action<ComponentRegistration>> ReBuildResolvePipelineAction =
            new(
                () =>
                {
                    var pipelineField = typeof(ComponentRegistration).GetField("_builtComponentPipeline",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    var parameter = Expression.Parameter(typeof(ComponentRegistration));

                    var pipelineFieldExpression = Expression.Field(parameter, pipelineField!);

                    var lateBuildPipelineField = typeof(ComponentRegistration).GetField("_lateBuildPipeline",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    var lateBuildPipelineExpression = Expression.Field(parameter, lateBuildPipelineField!);

                    var buildMethod =
                        typeof(IResolvePipelineBuilder).GetMethod("Build", BindingFlags.Instance | BindingFlags.Public);

                    var callBuild = Expression.Call(lateBuildPipelineExpression, buildMethod!);

                    var binaryExpression = Expression.Assign(pipelineFieldExpression,
                        callBuild);

                    return Expression.Lambda<Action<ComponentRegistration>>(binaryExpression, parameter).Compile();
                });

        private static void Rebuild(IComponentRegistryServices services, ComponentRegistration componentRegistration)
        {
            ReBuildResolvePipelineAction.Value(componentRegistration);
        }

        private static readonly Lazy<Func<IComponentRegistry, IComponentRegistryServices>>
            GetComponentRegistryServicesFunc =
                new(() =>
                {
                    var assembly = typeof(IComponentRegistry).Assembly;

                    var tar = Expression.Label(typeof(IComponentRegistryServices));

                    var type = assembly.GetType("Autofac.Core.Registration.ComponentRegistry");

                    var registeredServicesTrackerField = type!.GetField("_registeredServicesTracker",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                    var instanceExpression = Expression.Parameter(typeof(IComponentRegistry));
                    var instanceAs = Expression.TypeAs(instanceExpression, type);
                    var registeredServicesTrackerExpression =
                        Expression.Field(instanceAs, registeredServicesTrackerField!);
                    var ifTrue = Expression.IfThen(Expression.TypeIs(instanceExpression, type),
                        Expression.Return(tar, registeredServicesTrackerExpression));

                    var block = Expression.Block(ifTrue,
                        Expression.Label(tar, Expression.Constant(null, typeof(IComponentRegistryServices))));
                    return Expression.Lambda<Func<IComponentRegistry, IComponentRegistryServices>>(block
                        , instanceExpression).Compile();
                });

        private static IComponentRegistryServices GetComponentRegistryServices(IComponentRegistry componentRegistry)
        {
            return GetComponentRegistryServicesFunc.Value(componentRegistry);
        }

        private static readonly Lazy<Func<IComponentRegistryServices, ConcurrentQueue<IComponentRegistration>>>
            GetAllRegistrationFunc =
                new(() =>
                {
                    var assembly = typeof(IComponentRegistry).Assembly;

                    var tar = Expression.Label(typeof(ConcurrentQueue<IComponentRegistration>));

                    var parameter = Expression.Parameter(typeof(IComponentRegistryServices));

                    var type =
                        assembly.GetType("Autofac.Core.Registration.DefaultRegisteredServicesTracker");

                    var instanceAs = Expression.TypeAs(parameter, type!);

                    var typeIsExpression =
                        Expression.TypeIs(parameter, type!);

                    var registrationsField = type!.GetField("_registrations",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    var registrationsExpression =
                        Expression.Field(
                            instanceAs,
                            registrationsField!);

                    var ifThen = Expression.IfThen(typeIsExpression,
                        Expression.Return(tar, registrationsExpression));

                    var blockExpression = Expression.Block(parameter, ifThen,
                        Expression.Label(tar,
                            Expression.Constant(null, typeof(ConcurrentQueue<IComponentRegistration>))));

                    return Expression.Lambda<Func<IComponentRegistryServices, ConcurrentQueue<IComponentRegistration>>>(
                        blockExpression,
                        parameter).Compile();
                });

        private static ConcurrentQueue<IComponentRegistration> GetAllRegistration(IComponentRegistryServices services)
        {
            return GetAllRegistrationFunc.Value(services);
        }

        private static readonly Lazy<Action<IComponentContext, object, IPropertySelector, IEnumerable<Parameter>>>
            InjectPropertiesAction = new(
                () =>
                {
                    var assembly = typeof(IComponentRegistry).Assembly;
                    var type = assembly.GetType("Autofac.Core.Activators.Reflection.AutowiringPropertyInjector");

                    var injectPropertiesMethod = type!.GetMethod("InjectProperties",
                        BindingFlags.Static | BindingFlags.Public);

                    var parameterList = new[]
                    {
                        Expression.Parameter(typeof(IComponentContext)),
                        Expression.Parameter(typeof(object)),
                        Expression.Parameter(typeof(IPropertySelector)),
                        Expression.Parameter(typeof(IEnumerable<Parameter>)),
                    };

                    var injectPropertiesCall =
                        Expression.Call(injectPropertiesMethod!, parameterList.Cast<Expression>());

                    return Expression
                        .Lambda<Action<IComponentContext, object, IPropertySelector, IEnumerable<Parameter>>>(
                            injectPropertiesCall, parameterList).Compile();
                });

        private static void InjectProperties(IComponentContext context, object instance,
            IInjectPropertySelector propertySelector, IEnumerable<Parameter> parameters)
        {
            // InjectPropertiesAction.Value(context, instance, propertySelector, parameters);

            AutowiringPropertyInjector.InjectProperties(context, instance, propertySelector, parameters);
        }
    }
}
