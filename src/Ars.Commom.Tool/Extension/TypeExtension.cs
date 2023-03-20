using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Export;
using Ars.Common.Tool.UploadExcel;
using Ars.Common.Tool.UploadExcel.Validation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class TypeExtension
    {
        public static bool IsAssignableGenericFrom(this Type baseType, Type type)
        {
            if (type.GetInterfaces().Any(r => r.IsGenericType && r.GetGenericTypeDefinition() == baseType))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return true;

            Type? bType = type.BaseType;
            if (bType == null) return false;
            return baseType.IsAssignableGenericFrom(bType);
        }

        public static bool IsExportController(this Type type)
        {
            return type.IsClass &&
                !type.IsAbstract &&
                !type.IsInterface &&
                typeof(ControllerBase).IsAssignableFrom(type) &&
                type.IsDefined(typeof(ExportControllerAttribute), true);
        }

        public static Type GetTaskActuallyType(this Type type)
        {
            return typeof(Task<>).IsAssignableGenericFrom(type) ? type.GetGenericArguments()[0] : type;
        }

        public static IEnumerable<ExcelMappingAttribute> GetExcelMappingAttributes(this Type type)
        {
            ExcelMappingAttribute attribute;
            foreach (var property in type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(r => r.IsDefined(typeof(ExcelMappingAttribute), false)))
            {
                attribute = property.GetCustomAttribute<ExcelMappingAttribute>()!;
                attribute.Property = property.Name;
                attribute.PropertyType = property.PropertyType;

                yield return attribute;
            }
        }

        public static IDictionary<string, IEnumerable<IExcelValidation>> GetExcelValidationAttributes(this Type type) 
        {
            IDictionary<string, IEnumerable<IExcelValidation>> datas = 
                new Dictionary<string, IEnumerable<IExcelValidation>>();
            IEnumerable<IExcelValidation> values;
            foreach (var property in type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(r => r.IsDefined(typeof(ExcelMappingAttribute), false)))
            {
                values = property.GetCustomAttributes(typeof(IExcelValidation),false).Select(r => (IExcelValidation)r);
                if (values.HasValue())
                    datas.Add(property.Name,values);
            }

            return datas;
        }
    }
}
