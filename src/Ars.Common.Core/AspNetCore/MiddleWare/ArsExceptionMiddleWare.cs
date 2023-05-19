using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.UploadExcel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IGrpcExceptionManager? _grpcExceptionManager;
        public ArsExceptionMiddleWare(
            RequestDelegate next,
            IWebHostEnvironment environment,
            IGrpcExceptionManager? grpcExceptionManager)
        {
            _next = next;
            _environment = environment;
            _grpcExceptionManager = grpcExceptionManager;
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
            if (_grpcExceptionManager?.IsGrpcException(e) ?? false)
            {
                var err = _grpcExceptionManager.GetGrpcExceptionErr(e);
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
