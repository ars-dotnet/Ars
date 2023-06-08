using Ars.Common.Tool.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class ILoggingBuilderExternsion
    {
        public static ILoggingBuilder AddArsLog4Net(this ILoggingBuilder loggingBuilder, string log4netConfigFile)
        {
            SetCustomLogCategoryPrefix(log4netConfigFile);

            loggingBuilder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider>(new ArsLoggerProvider(log4netConfigFile)));

            return loggingBuilder;
        }

        /// <summary>
        /// 设置自定义日志类别校验前缀
        /// </summary>
        /// <param name="log4netConfigFile"></param>
        public static void SetCustomLogCategoryPrefix(string log4netConfigFile)
        {
            //设置LogNames.CustomLogCategoryPrefix名称
            var xml = log4netConfigFile.ParseXmlConfigFile();

            var node = xml?.SelectSingleNode("//root")?.SelectSingleNode("//customLogCategoryPrefix");

            var value = node?.Attributes?.GetNamedItem("value")?.Value;

            if (!string.IsNullOrEmpty(value))
                ArsLogNames.CustomLogCategoryPrefix = value;
        }
    }
}
