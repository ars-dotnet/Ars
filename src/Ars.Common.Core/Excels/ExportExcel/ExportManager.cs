using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Extensions;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Impl;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IEnumerable = System.Collections.IEnumerable;

namespace Ars.Common.Core.Excels.ExportExcel
{
    public class ExportManager : IExportManager
    {
        private readonly IExportApiSchemeProvider _schemeProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IXmlFileManager _xmlFileManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ExportManager(
            IExportApiSchemeProvider schemeProvider,
            IServiceProvider serviceProvider,
            IXmlFileManager xmlFileManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _schemeProvider = schemeProvider;
            _serviceProvider = serviceProvider;
            _xmlFileManager = xmlFileManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task<FileStreamResult> GetExcel(ExportExcelInput input)
        {
            ExportApiScheme? apischeme = null;

            if (input.ControllerName.IsNotNullOrEmpty())
            {
                apischeme = _schemeProvider.GetExportApiScheme(input.ControllerName!);

                Valid.ThrowException(null == apischeme, $"控制器没有注册，请添加[{nameof(ExportControllerAttribute)}]特性至控制器");
            }
            else if (input.ServiceName.IsNotNullOrEmpty())
            {
                apischeme = _schemeProvider.GetExportApiScheme(input.ServiceName!);

                Valid.ThrowException(null == apischeme, $"服务没有注册，请添加[{nameof(ExportServiceAttribute)}]特性至服务接口");
            }
            else 
            {
                Valid.ThrowException($"服务没有注册，请添加[{nameof(ExportServiceAttribute)}]特性至服务接口");
            }

            var methodscheme = apischeme!.MethodSchemes?.FirstOrDefault(r => r.ActionName.Equals(input.ActionName));
            Valid.ThrowException(null == methodscheme, $"方法没有注册，请添加[{nameof(ExportActionAttribute)}]特性至方法");

            using var scope = _serviceProvider.CreateScope();
            var apinstance = scope.ServiceProvider.GetRequiredService(apischeme.ControllerType);
            Valid.ThrowException(null == apinstance, "容器找不到控制器实例");

            List<object?> @params = new List<object?>();
            //参数组装
            foreach (var p in methodscheme!.Params)
            {
                object? param = null;

                if (input.Params.TryGetValue(p.Key, out var value))
                {
                    bool isConvert = ConvertTool.TryChangeType(value, p.Value, out param);

                    Valid.ThrowException(!isConvert,$"参数[{p.Key}]类型转化失败");
                }
                else
                {
                    param = scope.ServiceProvider.GetService(p.Value);

                    Valid.ThrowException(null == param,$"Params中未获取到参数[{p.Key}];如果是从容器获取，需保证参数已添加到服务");
                }

                @params.Add(param);
            }
            Valid.ThrowException(@params.Count != methodscheme.Params.Count, "参数个数不匹配");

            using var transScope = _unitOfWorkManager.Begin();

            var result = methodscheme!.IsAsync
                ? await methodscheme.MethodInfo.InvokeAsync(apinstance!, @params.ToArray())
                : methodscheme.MethodInfo.Invoke(apinstance!, @params.ToArray());
            Valid.ThrowException(null == result, "导出查询为空");

            //返回值转IEnumerable
            IEnumerable? list = ToEnumerable(result!, methodscheme.ReturnType, input.ReturnEnumerablePropertyName, out Type? itemtype);
            Valid.ThrowException(null == itemtype, "获取集合泛型具体类型失败");

            if (null == list)
                list = Array.CreateInstance(itemtype!, 0);

            //导出列组装
            if (!input.Column.HasValue())
            {
                input.Column = SetColumn(itemtype!);
            }

            //生成excel
            var excel = ExportExcel(new ExcelExportScheme
            {
                ExportFileName = input.ExportFileName,
                Title = input.Title,
                Header = input.Header,
                Column = input.Column.Select(r => new ExcelColumn { Field = r.Key, Column = r.Value }),
                List = list!,
                ItemType = itemtype!
            });

            await transScope.CompleteAsync();

            return excel;
        }

        /// <summary>
        /// 返回值转IEnumerable
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnType"></param>
        /// <param name="returnEnumerablePropertyName"></param>
        /// <param name="itemType">集合泛型具体类型</param>
        /// <returns></returns>
        public virtual IEnumerable? ToEnumerable(object value, Type returnType, string returnEnumerablePropertyName, out Type? itemType)
        {
            itemType = null;
            IEnumerable? list = null;
            if (typeof(IEnumerable<>).IsAssignableGenericFrom(returnType))
            {
                list = value.As<IEnumerable>()!;
                itemType = returnType.GetGenericArguments()[0];
            }
            else if (returnType.IsClass && typeof(string) != returnType)
            {
                PropertyInfo? propertyInfo = null;
                var type = returnType;
                if (!returnEnumerablePropertyName.IsNullOrEmpty())
                {
                    foreach (var propertyName in returnEnumerablePropertyName.Split("."))
                    {
                        propertyInfo = type.GetProperty(propertyName);

                        Valid.ThrowException(null == propertyInfo, $"获取查询结果属性[{propertyName}]失败");

                        type = propertyInfo!.PropertyType;

                        value = propertyInfo.GetValue(value)!;
                    }

                    if (typeof(IEnumerable<>).IsAssignableGenericFrom(propertyInfo!.PropertyType))
                    {
                        list = value!.As<IEnumerable>()!;
                        itemType = propertyInfo!.PropertyType.GetGenericArguments()[0];
                    }
                    else
                    {
                        list = new List<object> { value };
                        itemType = propertyInfo!.PropertyType;
                    }
                }
                else
                {
                    propertyInfo = returnType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(r => typeof(IEnumerable<>).IsAssignableGenericFrom(r.PropertyType))
                        .FirstOrDefault();
                    if (null != propertyInfo)
                    {
                        //存在集合对象
                        list = propertyInfo.GetValue(value)!.As<IEnumerable>()!;
                        itemType = propertyInfo!.PropertyType.GetGenericArguments()[0];
                    }
                    else
                    {
                        //不存在集合对象
                        list = new List<object> { value };
                        itemType = returnType;
                    }
                }
            }

            if (null == list)
                list = Array.CreateInstance(itemType!, 0);

            return list;
        }

        /// <summary>
        /// 导出列组装
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual IDictionary<string, string> SetColumn(Type itemtype)
        {
            return itemtype
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(r => r.Name)
                .ToDictionary(t => t, t => _xmlFileManager.GetPropertyXmlSummary(itemtype, t));
        }

        /// <summary>
        /// 生成excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual FileStreamResult ExportExcel(ExcelExportScheme input)
        {
            return ExcelTool.ExportExcel(input);
        }
    }
}
