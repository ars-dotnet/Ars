using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IEnumerable = System.Collections.IEnumerable;

namespace Ars.Common.Tool.Export
{
    public class ExportManager : IExportManager
    {
        private readonly IExportApiSchemeProvider _schemeProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IXmlFileManager _xmlFileManager;

        public ExportManager(
            IExportApiSchemeProvider schemeProvider, 
            IServiceProvider serviceProvider,
            IXmlFileManager xmlFileManager)
        {
            _schemeProvider = schemeProvider;
            _serviceProvider = serviceProvider;
            _xmlFileManager = xmlFileManager;
        }

        public virtual async Task<FileStreamResult> GetExcel(ExportExcelInput input)
        {
            var apischeme = _schemeProvider.GetExportApiScheme(input.ControllerName);
            Valid.ThrowException(null == apischeme, "控制器没有注册，请添加[ExportControllerAttribute]特性至控制器");
            var methodscheme = apischeme!.MethodSchemes?.FirstOrDefault(r => r.ActionName.Equals(input.ActionName));
            Valid.ThrowException(null == methodscheme, "方法没有注册，请添加[ExportActionAttribute]特性至方法");

            using var scope = _serviceProvider.CreateScope();
            var apinstance = scope.ServiceProvider.GetRequiredService(apischeme.ControllerType);
            Valid.ThrowException(null == apinstance, "容器找不到控制器实例");

            List<object?> @params = new List<object?>();
            //参数组装
            foreach (var p in methodscheme!.Params)
            {
                if (input.Params.TryGetValue(p.Key, out var value))
                {
                    bool isConvert = ConvertTool.TryChangeType(value, p.Value, out var newvalue);
                    if (isConvert)
                    {
                        @params.Add(newvalue);
                    }
                    else 
                    {
                        Valid.ThrowException($"参数[{p.Key}]类型转化失败");
                    }
                }
                else 
                {
                    Valid.ThrowException($"Params中未获取到参数[{p.Key}]");
                }
            }
            Valid.ThrowException(@params.Count != methodscheme.Params.Count,"参数个数不匹配"); 

            var result = methodscheme!.IsAsync
                ? await methodscheme.MethodInfo.InvokeAsync(apinstance!, @params.ToArray())
                : methodscheme.MethodInfo.Invoke(apinstance!, @params.ToArray());
            Valid.ThrowException(null == result,"导出查询为空");

            //返回值转IEnumerable
            IEnumerable? list = ToEnumerable(result!, methodscheme.ReturnType,input.ReturnEnumerablePropertyName,out Type? itemtype);
            Valid.ThrowException(null == list, "结果转集合失败");
            Valid.ThrowException(null == itemtype, "获取集合泛型具体类型失败");

            //导出列组装
            if (!input.Column.HasValue()) 
            {
                input.Column = SetColumn(itemtype!);
            }

            //生成excel
            return ExportExcel(new Tools.ExcelExportScheme 
            {
                ExportFileName = input.ExportFileName, 
                Title = input.Title,
                Header = input.Header,
                Column = input.Column, 
                List = list!, 
                ItemType = itemtype!
            });
        }

        /// <summary>
        /// 返回值转IEnumerable
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnType"></param>
        /// <param name="returnEnumerablePropertyName"></param>
        /// <param name="itemType">集合泛型具体类型</param>
        /// <returns></returns>
        public virtual IEnumerable? ToEnumerable(object value, Type returnType,string returnEnumerablePropertyName,out Type? itemType) 
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
                if (!returnEnumerablePropertyName.IsNullOrEmpty())
                {
                    propertyInfo = returnType.GetProperty(returnEnumerablePropertyName);
                    Valid.ThrowException(null == propertyInfo, $"获取查询结果属性[{returnEnumerablePropertyName}]失败");

                    if (typeof(IEnumerable<>).IsAssignableGenericFrom(propertyInfo!.PropertyType))
                    {
                        list = propertyInfo.GetValue(value)!.As<IEnumerable>()!;
                        itemType = propertyInfo!.PropertyType.GetGenericArguments()[0];
                    }
                    else 
                    {
                        list = new List<object> { propertyInfo.GetValue(value)! };
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
            
            return list;
        }

        /// <summary>
        /// 导出列组装
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual IDictionary<string,string> SetColumn(Type itemtype) 
        {
            return itemtype
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(r => r.Name)
                .ToDictionary(t => t,t => _xmlFileManager.GetPropertyXmlSummary(itemtype,t));
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
