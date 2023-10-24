using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class PropertyExtension
    {
        /// <summary>
        /// 校验propertyty是否可空
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static bool IsNullableType(this PropertyInfo propertyInfo) 
        {
            var type = propertyInfo.PropertyType;
            if(type.IsValueType)
                return Nullable.GetUnderlyingType(type) != null;

            NullabilityInfoContext context = new();
            NullabilityInfo arrayInfo = context.Create(propertyInfo);

            return arrayInfo.ReadState == NullabilityState.Nullable;
        }
    }
}
