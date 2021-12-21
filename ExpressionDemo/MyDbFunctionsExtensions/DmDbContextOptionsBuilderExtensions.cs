using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo.MyDbFunctionsExtensions
{
    /// <summary>
    /// 创建DbContext生成时的扩展方法
    /// </summary>
    public static class DmDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseDmAlgorithmsEncryptionFunctions(
    this DbContextOptionsBuilder optionsBuilder)
        {
            //将自定义的配置类添加到配置选项中
            var extension = GetOrCreateExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        //生成创建扩展类
        private static DmDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<DmDbContextOptionsExtension>()
               ?? new DmDbContextOptionsExtension();
    }
}
