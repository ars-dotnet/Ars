using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo.MyDbFunctionsExtensions
{
    public static class DbFunctionsExtensions
    {
        /// <summary>
        /// 调用数据库的加密方法
        /// </summary>
        /// <param name="_"></param>
        /// <param name="context"></param>
        /// <param name="typeid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ThisLike(this DbFunctions _, string matchExpression, string pattern)
        {
            throw new InvalidOperationException(
                "该方法仅用于实体框架核心，没有内存实现。");
        }

        /// <summary>
        /// 调用数据库的解密方法
        /// </summary>
        /// <param name="_"></param>
        /// <param name="context"></param>
        /// <param name="typeid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ThisLike(this DbFunctions _, string matchExpression, string pattern, string escapeCharacter)
        {
            throw new InvalidOperationException(
                "该方法仅用于实体框架核心，没有内存实现。");
        }
    }
}
