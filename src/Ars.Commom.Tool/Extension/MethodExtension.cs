using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Tool.Extension
{
    public static class MethodExtension
    {
        public static object Invoke(this MethodInfo methodInfo,object obj,object?[]? @params) 
        {
            return methodInfo.Invoke(obj, @params)!;
        }

        public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, object?[]? @params)
        {
            var task = (Task)methodInfo.Invoke(obj, @params)!;
            await task;

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task)!;
        }

        public static object InvokeGeneric(this MethodInfo methodInfo, object obj, object?[]? @params, params Type[] genericType)
        {
            return methodInfo.MakeGenericMethod(genericType).Invoke(obj, @params)!;
        }

        public static object InvokeGenericAsync(this MethodInfo methodInfo, object obj, object?[]? @params, params Type[] genericType)
        {
            return methodInfo.MakeGenericMethod(genericType).InvokeAsync(obj, @params)!;
        }
    }
}
