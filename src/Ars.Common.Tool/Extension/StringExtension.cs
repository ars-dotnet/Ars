using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

        /// <summary>
        /// base64 + sha256加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64andSha256Encipher(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);

                var hash = sha.ComputeHash(bytes);

                var hex = new StringBuilder(hash.Length * 2);

                //转16进制字符串
                foreach (byte b in hash)
                {
                    hex.AppendFormat("{0:x2}", b);
                }

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(hex.ToString()));
            }
        }
    }
}
