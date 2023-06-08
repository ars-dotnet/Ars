using Ars.Common.Tool.Extension;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Ars.Common.Tool.Loggers
{
    internal class ArsLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly string _log4NetConfigFile;

        public ArsLoggerProvider(string log4NetConfigFile)
        {
            this._log4NetConfigFile = log4NetConfigFile;
        }

        public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name =>
        {
            if (!CheckCustomCategoryNamePrefix(name))
            {
                return new ArsNoneLogger();
            }

            //用assembly的话，会和Microsoft.Extensions.Logging.Log4Net.AspNetCore冲突
            //导致log4net.Config配置记录不了日志
            //var assembly = GetLoggingReferenceAssembly();
            var loggerRepo = LoggerManager.CreateRepository($"{categoryName}.Repository", typeof(Hierarchy));

            IXmlRepositoryConfigurator configurableRepository = (loggerRepo as IXmlRepositoryConfigurator)!;

            XmlDocument newDoc = new XmlDocument { XmlResolver = null };
            var documentElement = _log4NetConfigFile.ParseXmlConfigFile().DocumentElement!;
            XmlElement newElement = (XmlElement)newDoc.AppendChild(newDoc.ImportNode(documentElement, true))!;
            configurableRepository.Configure(newElement);

            foreach (ArsRollingFileAppender appender in loggerRepo.GetAppenders())
            {
                appender.DatePattern = string.Format(appender.DatePattern, name);

                //生成log文件
                appender.ActivateOptionsTrue();
            }

            return new ArsLogger(name, loggerRepo.Name);
        });

        public void Dispose()
        {
            _loggers.Clear();
        }

        /// <summary>
        /// 校验是不是需要分日志文件的类别名称前缀
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        protected virtual bool CheckCustomCategoryNamePrefix(string categoryName)
        {
            if (categoryName.StartsWith(ArsLogNames.CustomLogCategoryPrefix, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
