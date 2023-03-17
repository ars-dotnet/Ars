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
        public static bool TryChangeType(object? value, Type conversionType,out object? newvalue) 
        {
            bool isConvert = false;
            try
            {
                if (conversionType.IsClass && typeof(string) != conversionType)
                {
                    newvalue = JsonConvert.DeserializeObject(value?.ToString()!, conversionType);
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
    }
}
