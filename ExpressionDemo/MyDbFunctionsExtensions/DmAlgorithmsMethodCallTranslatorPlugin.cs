using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ExpressionDemo.MyDbFunctionsExtensions
{
    /// <summary>
    /// 调用方法转换器提供程序
    /// </summary>
    public sealed class DmAlgorithmsMethodCallTranslatorPlugin : RelationalMethodCallTranslatorProvider
    {
        public DmAlgorithmsMethodCallTranslatorPlugin(RelationalMethodCallTranslatorProviderDependencies dependencies) : base(dependencies)
        {
            ISqlExpressionFactory expressionFactory = dependencies.SqlExpressionFactory;
            AddTranslators(
                new IMethodCallTranslator[]
                {
    //这里,将刚刚的方法转换器添加到扩展
                    new DmDbFunctionsTranslateImpl(expressionFactory)
                });
        }
    }
}
