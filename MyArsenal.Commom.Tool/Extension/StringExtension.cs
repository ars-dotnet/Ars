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
    }
}
