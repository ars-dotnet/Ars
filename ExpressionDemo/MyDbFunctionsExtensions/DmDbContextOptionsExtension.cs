using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo.MyDbFunctionsExtensions
{
    /// <summary>
    /// 创建DbContext扩展类
    /// </summary>
    public class DmDbContextOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public DbContextOptionsExtensionInfo Info
        {
            get
            {
                return this._info ??= new MyDbContextOptionsExtensionInfo(this);
            }
        }

        public void ApplyServices(IServiceCollection services)
        {
            //这里将转换器注入到服务当中.
            services.AddScoped<IMethodCallTranslatorProvider, DmAlgorithmsMethodCallTranslatorPlugin>();
        }

        public void Validate(IDbContextOptions options)
        {

        }

        private sealed class MyDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public MyDbContextOptionsExtensionInfo(IDbContextOptionsExtension instance) : base(instance) { }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "";

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }

            public override long GetServiceProviderHashCode()
            {
                return 0;
            }
        }
    }
}
