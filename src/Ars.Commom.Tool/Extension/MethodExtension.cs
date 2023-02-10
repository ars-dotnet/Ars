using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Tool.Extension
{
    public static class MethodExtension
    {
        public static object Invoke(MethodInfo methodInfo,object obj,params object[] @params) 
        {
            return methodInfo.Invoke(obj, @params);
        }

        public static async Task<object> InvokeAsync(MethodInfo methodInfo, object obj, params object[] @params)
        {
            var task = (Task)methodInfo.Invoke(obj, @params);
            await task;

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
        }
    }
}
