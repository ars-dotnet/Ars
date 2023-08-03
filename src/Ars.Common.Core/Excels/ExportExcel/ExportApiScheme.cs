using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.ExportExcel
{
    public class ExportApiScheme
    {
        public ExportApiScheme()
        {

        }

        public ExportApiScheme(Type controllerType, IEnumerable<ExportMethodScheme> methodSchemes)
        {
            ControllerType = controllerType;
            MethodSchemes = methodSchemes;
        }

        public Type ControllerType { get; set; }

        public IEnumerable<ExportMethodScheme> MethodSchemes { get; set; }
    }

    public class ExportMethodScheme
    {
        public ExportMethodScheme()
        {

        }

        public ExportMethodScheme(
            string actionName,
            MethodInfo methodInfo,
            bool isAsync,
            IDictionary<string, Type> @params,
            Type returnType)
        {
            ActionName = actionName;
            MethodInfo = methodInfo;
            IsAsync = isAsync;
            Params = @params;
            ReturnType = returnType;
        }

        public string ActionName { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public bool IsAsync { get; set; }

        public IDictionary<string, Type> Params { get; set; }

        public Type ReturnType { get; set; }
    }
}
