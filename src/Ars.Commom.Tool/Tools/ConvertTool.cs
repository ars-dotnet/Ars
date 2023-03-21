using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Tools
{
    public static class ConvertTool
    {
        public static bool TryChangeType(object? value, Type conversionType, out object? newvalue)
        {
            bool isConvert = false;
            if (null == value) 
            {
                newvalue = null;
                isConvert = true;
                return isConvert;
            }
                
            try
            {
                if ((conversionType.IsClass && typeof(string) != conversionType)
                    || conversionType.IsInterface)
                {
                    if(value.GetType() == typeof(string))
                        newvalue = JsonConvert.DeserializeObject(value.ToString()!, conversionType);
                    else
                        newvalue = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), conversionType);
                }
                else
                {
                    newvalue = Convert.ChangeType(value, conversionType);
                }

                isConvert = true;
            }
            catch (Exception e)
            {
                newvalue = null;
            }

            return isConvert;
        }

        public static string ToString(object? value)
        {
            if (null == value)
                return string.Empty;

            var conversionType = value.GetType();
            if ((conversionType.IsClass && typeof(string) != conversionType)
                || conversionType.IsInterface)
            {
                return JsonConvert.SerializeObject(value);
            }
            else
            {
                return value.ToString()!;
            }
        }
    }
}
