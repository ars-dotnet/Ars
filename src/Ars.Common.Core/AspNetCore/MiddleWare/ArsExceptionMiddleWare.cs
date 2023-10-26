using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ars.Common.Core.Excels.UploadExcel;
using System.Net;
using Polly.Timeout;
using Polly.CircuitBreaker;

namespace Ars.Common.Core.AspNetCore.MiddleWare
{
    public class ArsExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ArsExceptionMiddleWare(
            RequestDelegate next,
            IWebHostEnvironment environment,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _environment = environment;
            _serviceScopeFactory = serviceScopeFactory;
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
            object? data = null;
            int code;
            string errorMsg;
           
            var grpcExcptmanager = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IGrpcExceptionManager>();
            if (grpcExcptmanager?.IsGrpcException(e) ?? false)
            {
                var err = grpcExcptmanager.GetGrpcExceptionErr(e);
                code = err.Item1;
                errorMsg = err.Item2;
            }
            else if (e is ArsExcelException excelException)
            {
                code = excelException.Code;
                errorMsg = excelException.Message;
                data = new { ErrExcelDownUrl = excelException.ErrExcelDownUrl };
            }
            else if (e is ArsException uexception)
            {
                code = uexception.Code;
                errorMsg = uexception.Message;
            }
            else if (e is HttpRequestException requestException)
            {
                code = (int)(requestException.StatusCode ?? HttpStatusCode.InternalServerError);
                errorMsg = requestException.Message;
            }
            else if (e is TimeoutRejectedException timeoutRejectedException) //超时
            {
                code = 504;
                errorMsg = $"请求超时:{timeoutRejectedException.Message}";
            }
            else if (e is BrokenCircuitException brokenException) //熔断
            {
                code = 503;
                errorMsg = $"服务不可用:{brokenException.Message}";
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
