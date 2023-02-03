using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public class UowActionFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IArsAspNetCoreConfiguration _arsAspNetCoreConfiguration;
        public UowActionFilter(IUnitOfWorkManager unitOfWorkManager,
            IArsAspNetCoreConfiguration arsAspNetCoreConfiguration)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _arsAspNetCoreConfiguration = arsAspNetCoreConfiguration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionDescriptor.IsControllerAction())
                return;

            UnitOfWorkAttribute workAttribute = GetUnitOfWorkAttribute(context)
                ?? _arsAspNetCoreConfiguration.unitOfWorkAttribute;
            if (workAttribute.IsDisabled) 
            {
                await next();
                return;
            }

            using var scope = _unitOfWorkManager.Begin(workAttribute.CreateOption());
            var res = await next();
            if (res.Exception == null || res.ExceptionHandled)
            {
                await scope.CompleteAsync();
            }
        }

        private UnitOfWorkAttribute? GetUnitOfWorkAttribute(ActionExecutingContext context) 
        {
            MethodInfo method = context.ActionDescriptor.GetMethodInfo();

            var attrs = method.GetCustomAttributes<UnitOfWorkAttribute>(true);
            if (attrs?.Any() ?? false)
            {
                return attrs.First();
            }

            var classAttrs = method.DeclaringType!.GetTypeInfo().GetCustomAttributes<UnitOfWorkAttribute>(true);
            if (classAttrs?.Any() ?? false) 
            {
                return classAttrs.First();
            }

            return null;
        }
    }
}
