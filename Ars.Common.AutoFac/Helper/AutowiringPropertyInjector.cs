using Ars.Common.AutoFac.IDependency;
using Autofac;
using Autofac.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Helper
{
    /// <summary>
    /// 属性注入器
    /// </summary>
    internal static class AutowiringPropertyInjector
    {
        /// <summary>
        /// Name of the parameter containing the instance type provided when resolving an injected service.
        /// </summary>
        private const string InstanceTypeNamedParameter = "Autofac.AutowiringPropertyInjector.InstanceType";

        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> PropertySetters =
            new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        private static readonly ConcurrentDictionary<Type, (PropertyInfo, string)[]> InjectableProperties =
            new ConcurrentDictionary<Type, (PropertyInfo, string)[]>();

        private static readonly MethodInfo CallPropertySetterOpenGenericMethod =
            typeof(AutowiringPropertyInjector).GetDeclaredMethod(nameof(CallPropertySetter));

        /// <summary>
        /// Inject properties onto an instance, filtered by a property selector.
        /// </summary>
        /// <param name="context">The component context to resolve dependencies from.</param>
        /// <param name="instance">The instance to inject onto.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="parameters">The set of parameters for the resolve that can be used to satisfy injectable properties.</param>
        public static void InjectProperties(IComponentContext context, object instance,
            IInjectPropertySelector propertySelector, IEnumerable<Parameter> parameters)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var resolveParameters = parameters as Parameter[] ?? parameters.ToArray();

            var instanceType = instance.GetType();
            var injectableProperties =
                InjectableProperties.GetOrAdd(instanceType,
                    type => GetInjectableProperties(type, instance, propertySelector).ToArray());

            foreach (var (property, serviceName) in injectableProperties)
            {
                // SetMethod will be non-null if GetInjectableProperties included it.
                var setParameter = property.SetMethod!.GetParameters()[0];

                var valueProvider = (Func<object>)null;
                var parameter =
                    resolveParameters.FirstOrDefault(p => p.CanSupplyValue(setParameter, context, out valueProvider));
                if (parameter != null)
                {
                    var setter = PropertySetters.GetOrAdd(property, MakeFastPropertySetter);
                    setter(instance, valueProvider!());
                    continue;
                }

                var propertyService = serviceName == null
                    ? (Service)new TypedService(property.PropertyType)
                    : new KeyedService(serviceName, property.PropertyType);

                var instanceTypeParameter = new NamedParameter(InstanceTypeNamedParameter, instanceType);

                var propertyValue = context.ResolveService(propertyService, instanceTypeParameter);
                {
                    var setter = PropertySetters.GetOrAdd(property, MakeFastPropertySetter);
                    setter(instance, propertyValue);
                }
            }
        }

        private static IEnumerable<(PropertyInfo, string)> GetInjectableProperties(Type instanceType, object instance,
            IInjectPropertySelector propertySelector)
        {
            foreach (var property in instanceType.GetRuntimeProperties())
            {
                if (!property.CanWrite)
                {
                    goto CheckAndContinue;
                }

                // SetMethod will be non-null if CanWrite is true.
                // Don't want to inject onto static properties.
                if (property.SetMethod!.IsStatic)
                {
                    goto CheckAndContinue;
                }

                var propertyType = property.PropertyType;

                if (propertyType.IsValueType && !propertyType.IsEnum)
                {
                    goto CheckAndContinue;
                }

                // GetElementType will be non-null if IsArray is true.
                if (propertyType.IsArray && propertyType.GetElementType()!.IsValueType)
                {
                    goto CheckAndContinue;
                }

                // if (propertyType.IsGenericEnumerableInterfaceType() && propertyType.GenericTypeArguments[0].IsValueType)
                // {
                //     continue;
                // }

                if (property.GetIndexParameters().Length != 0)
                {
                    goto CheckAndContinue;
                }

                if (!propertySelector.TryGetInjectProperty(property, instance, out var serviceName))
                {
                    continue;
                }

                yield return (property, serviceName);
                continue;

            CheckAndContinue:

                if (property.CustomAttributes.Any(x => x.AttributeType == typeof(AutowiredAttribute)))
                {
                    throw new CustomAttributeFormatException(
                        $"{instanceType.FullName},属性：{property.Name} 不是有效的注入属性");
                }
            }
        }

        private static Action<object, object> MakeFastPropertySetter(PropertyInfo propertyInfo)
        {
            // SetMethod will be non-null if we're trying to make a setter for it.
            var setMethod = propertyInfo.SetMethod!;

            // DeclaringType will always be set for properties.
            var typeInput = setMethod!.DeclaringType!;
            var parameters = setMethod.GetParameters();
            var parameterType = parameters[0].ParameterType;

            // Create a delegate TDeclaringType -> { TDeclaringType.Property = TValue; }
            var propertySetterAsAction =
                setMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(typeInput!, parameterType));
            var callPropertySetterClosedGenericMethod =
                CallPropertySetterOpenGenericMethod.MakeGenericMethod(typeInput, parameterType);
            var callPropertySetterDelegate =
                callPropertySetterClosedGenericMethod.CreateDelegate<Action<object, object>>(propertySetterAsAction);

            return callPropertySetterDelegate;
        }

        private static void CallPropertySetter<TDeclaringType, TValue>(
            Action<TDeclaringType, TValue> setter, object target, object value) =>
            setter((TDeclaringType)target, (TValue)value);
    }
}
