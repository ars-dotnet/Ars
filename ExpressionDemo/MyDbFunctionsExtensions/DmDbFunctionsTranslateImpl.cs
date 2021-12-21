using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressionDemo.MyDbFunctionsExtensions
{
    /// <summary>
    /// 调用方法转换器
    /// </summary>
    public class DmDbFunctionsTranslateImpl : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _expressionFactory;

        private static readonly MethodInfo _dmAlgorithmsEncryptMethod
            = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.ThisLike),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly MethodInfo _dmAlgorithmsDecryptMethod
            = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.ThisLike),
                new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(string) });


        public DmDbFunctionsTranslateImpl(ISqlExpressionFactory expressionFactory)
        {
            _expressionFactory = expressionFactory;
        }


        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            //判断方法是否一致
            if (method == _dmAlgorithmsEncryptMethod)
            {
                var args = new List<SqlExpression> { arguments[1], arguments[2] };
                return _expressionFactory.Function(instance, "like", args, typeof(bool));
            }
            if (method == _dmAlgorithmsDecryptMethod)
            {
                var args = new List<SqlExpression> { arguments[1], arguments[2], arguments[3] };
                return _expressionFactory.Function(instance, "like", args, typeof(bool));
            }

            return null;

        }
    }
}
