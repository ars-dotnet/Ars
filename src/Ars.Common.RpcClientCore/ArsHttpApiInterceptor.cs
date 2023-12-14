using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore
{
    public class ArsHttpApiInterceptor : IHttpApiInterceptor
    {
        /// <summary>
        /// 服务上下文
        /// </summary>
        private readonly HttpClientContext context;

        /// <summary>
        /// 接口方法的拦截器
        /// </summary>
        /// <param name="context">httpClient上下文</param> 
        public ArsHttpApiInterceptor(HttpClientContext context)
        {
            this.context = context;
        }

        public object Intercept(ApiActionInvoker actionInvoker, object?[] arguments)
        {
            return actionInvoker.Invoke(this.context, arguments);
        }
    }
}
