using Ars.Commom.Tool.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class EnumExtension
    {
        private static ConcurrentDictionary<string, string> enumTypeList =
            new ConcurrentDictionary<string, string>();
        public static string Description(this Type enumType)
        {
            string res = "";
            if (enumTypeList.TryGetValue(enumType.FullName, out res))
            {
                return res;
            }

            List<string> enumDescriptions = new List<string>();
            var enums = Enum.GetValues(enumType);

            foreach (object enumOption in enums)
            {
                var type = enumOption.GetType();

                var desc = type
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.Name == enumOption.ToString())
                    .FirstOrDefault()!
                    .GetCustomAttributes()
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault();

                string Description = string.Empty;
                if (desc != null)
                {
                    Description = string.Format("{0} = {1} <br/> ", enumOption.ToString()!.LowerCamel(), desc.Description);
                }
                else
                {
                    Description = string.Format("{0} = {1} <br/> ", (int)enumOption, Enum.GetName(enumOption.GetType(), enumOption));
                }

                enumDescriptions.Add(Description);
            }

            res = "<br/> 枚举【" + enumType.Name + "】定义：<br/> " + string.Join("", enumDescriptions.ToArray());

            if (!enumTypeList.ContainsKey(enumType.FullName))
                enumTypeList.TryAdd(enumType.FullName, res);

            return res;
        }

        public static string GetDescriotion(this Enum @enum) 
        {
            var type = @enum.GetType();
            return type.GetField(@enum.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? @enum.ToString();
        }
    }
}
