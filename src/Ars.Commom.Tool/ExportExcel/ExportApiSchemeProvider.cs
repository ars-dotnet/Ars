using Ars.Common.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Export
{
    internal class ExportApiSchemeProvider : IExportApiSchemeProvider
    {
        private readonly IDictionary<string, ExportApiScheme> apis;
        public ExportApiSchemeProvider()
        {
            apis = new Dictionary<string, ExportApiScheme>();
        }

        public ExportApiScheme? GetExportApiScheme(string key)
        {
            return apis.TryGetValue(key, out var result) ? result : null;
        }

        public void SetExportApiSchemed(Assembly assembly)
        {
            Array.ForEach(assembly.DefinedTypes.Select(t => t.AsType()).Where(r => r.IsExportController()).ToArray(),
                t =>
                {
                    ExportApiScheme exportApiScheme = null;
                    List<ExportMethodScheme> methodSchemes = new List<ExportMethodScheme>(0);
                    ExportMethodScheme exportMethodScheme = null;
                    IDictionary<string, Type> param = null;
                    foreach (var @method in t.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(r => r.IsDefined(typeof(ExportActionAttribute), false))) 
                    {
                        if (methodSchemes.Any(r => r.ActionName.Equals(@method.Name))) 
                        {
                            Valid.ThrowException($"注入导出服务失败,控制器:{t.Name}存在相同名称的可导出方法:{@method.Name}");
                        }

                        param = new Dictionary<string, Type>();
                        foreach (var p in @method.GetParameters()) 
                        {
                            param.Add(p.Name!, p.ParameterType);
                        }

                        exportMethodScheme = new ExportMethodScheme(
                            @method.Name,
                            @method,
                            typeof(Task<>).IsAssignableGenericFrom(@method.ReturnType),
                            param,
                            @method.ReturnType.GetTaskActuallyType());
                        methodSchemes.Add(exportMethodScheme);
                    }

                    if (methodSchemes.Any())
                    {
                        exportApiScheme = new ExportApiScheme(t, methodSchemes);
                        apis.Add(t.Name,exportApiScheme);
                    }
                });
        }
    }
}
