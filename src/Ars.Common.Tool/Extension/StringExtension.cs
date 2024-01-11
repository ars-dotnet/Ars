using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ars.Commom.Tool.Extension
{
    public static class StringExtension
    {
        public static string Unescape(this string str)
        {
            return Regex.Unescape(str);
        }

        public static bool IsNullOrEmpty(this string? str) 
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string LowerCamel(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (str.Length <= 1)
            {
                return str.ToLower();
            }

            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
