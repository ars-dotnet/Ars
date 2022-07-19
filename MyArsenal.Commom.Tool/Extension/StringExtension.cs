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

        public static bool IsNullOrEmpty(this string str) 
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}
