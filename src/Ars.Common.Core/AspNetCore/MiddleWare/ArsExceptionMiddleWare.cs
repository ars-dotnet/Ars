using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.MiddleWare
{
    public class ArsExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _environment;
        public ArsExceptionMiddleWare(RequestDelegate next, IHostingEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task Invoke(HttpContext httpContext) 
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e) 
            {
                await HandleError(httpContext, e);
            }
        }

        private async Task HandleError(HttpContext httpContext, Exception e) 
        {
            object data = null;
            int code;
            string errorMsg;
            if (e is RpcException rpcException)
            {
                code = 500;
                errorMsg = rpcException.Status.Detail;
            }
            else if (e is ArsException uexception)
            {
                code = uexception.Code;
                errorMsg = uexception.Message;
            }
            else
            {
                code = 500;
                errorMsg = e.GetInnerExceptionMessage();
            }

            httpContext.Response.StatusCode = code;
            httpContext.Response.ContentType = "application/json;charset=utf-8;";
            if (_environment.IsDevelopment())
            {
                await httpContext.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new ArsOutput<object>(data,code, $"错误消息:{errorMsg}{Environment.NewLine}错误追踪:{e.GetInnerException().StackTrace}")));
            }
            else
            {
                await httpContext.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new ArsOutput<object>(data,code, errorMsg)));
            }
        }
    }
}
