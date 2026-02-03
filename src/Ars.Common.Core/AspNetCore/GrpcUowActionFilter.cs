using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    public class GrpcUowActionFilter : IEndpointFilter
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IArsAspNetCoreConfiguration _arsAspNetCoreConfiguration;

        public GrpcUowActionFilter(IUnitOfWorkManager unitOfWorkManager,
            IArsAspNetCoreConfiguration arsAspNetCoreConfiguration)
        {
            _unitOfWorkManager = unitOfWorkManager;
            
            _arsAspNetCoreConfiguration = arsAspNetCoreConfiguration;
        }

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context, 
            EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;

            // 获取端点元数据
            var workAttribute = httpContext.GetEndpoint()?.Metadata.GetMetadata<UnitOfWorkAttribute>() 
                ?? 
                _arsAspNetCoreConfiguration.unitOfWorkAttribute;

            if (workAttribute.IsDisabled)
            {
                return await next(context);
            }

            using var scope = _unitOfWorkManager.Begin(workAttribute.CreateOption());
            
            var res = await next(context);

            await scope.CompleteAsync();

            return res;
        }
    }
}
