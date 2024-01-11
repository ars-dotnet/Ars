using log4net.Core;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using log4net.Filter;
using log4net.Appender;
using static log4net.Appender.RollingFileAppender;
using log4net.Config;

namespace Ars.Common.Tool.Loggers
{
    internal class ArsLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _repositoryName;

        private readonly ConcurrentDictionary<string, log4net.Core.ILogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public ArsLogger(string name, string repositoryName)
            => (_categoryName, _repositoryName) = (name, repositoryName);

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        protected virtual bool IsArsLog()
        {
            if (_categoryName.StartsWith(ArsLogNames.CustomLogCategoryPrefix))
                return true;

            return false;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (!IsArsLog())
            {
                return;
            }

            string message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }
            else
            {
                var level = GetLevel(logLevel);
                var logger = GetILoggerNew(level);

                LoggingEvent loggingEvent = new LoggingEvent(
                    callerStackBoundaryDeclaringType: typeof(LoggerExtensions),
                    repository: logger.Repository,
                    loggerName: logger.Name,
                    level: level,
                    message: message,
                    exception: exception);

                logger.Log(loggingEvent);
            }
        }

        protected virtual Level GetLevel(LogLevel logLevel)
        {
            Level log4NetLevel;
            switch (logLevel)
            {
                case LogLevel.Critical:
                    log4NetLevel = Level.Critical;
                    break;
                case LogLevel.Debug:
                    log4NetLevel = Level.Debug;
                    break;
                case LogLevel.Error:
                    log4NetLevel = Level.Error;
                    break;
                case LogLevel.Information:
                    log4NetLevel = Level.Info;
                    break;
                case LogLevel.Warning:
                    log4NetLevel = Level.Warn;
                    break;
                case LogLevel.Trace:
                    log4NetLevel = Level.Trace;
                    break;
                default:
                    log4NetLevel = Level.Off;
                    break;
            }

            return log4NetLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected virtual log4net.Core.ILogger GetILoggerNew(Level level)
        {
            var repository = LogManager.GetRepository(_repositoryName);

            string loggername = $"{level}.{_categoryName}".ToLower();
            log4net.Core.ILogger logger = repository.GetLogger(loggername);

            return logger;
        }

        /// <summary>
        /// 通过自定义appender创建ILogger
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected virtual log4net.Core.ILogger GetILogger(Level level)
        {
            string name = $"{level}.{_categoryName}".ToLower();

            var data = new Lazy<log4net.Core.ILogger>(() =>
            {
                //配置输出日志格式。%m表示message即日志信息。%n表示newline换行
                log4net.Layout.PatternLayout layout =
                    new log4net.Layout.PatternLayout(
                        @"%date [%thread] %-5level %logger %newline%message%newline---------------------------------------------------------------------------------------%newline");
                layout.ActivateOptions();

                //配置日志级别为所有级别
                LevelRangeFilter filter = new LevelRangeFilter();
                filter.LevelMin = level;
                filter.LevelMax = level;
                filter.ActivateOptions();

                //配置日志【循环附加，累加】
                RollingFileAppender appender = new RollingFileAppender();
                appender.File = string.Format("logs//");
                appender.AppendToFile = true;
                appender.LockingModel = new FileAppender.MinimalLock();

                appender.RollingStyle = RollingMode.Composite;
                appender.DatePattern = $"yyyyMMdd//'{name}.log'";
                appender.MaxSizeRollBackups = 20;
                appender.MaximumFileSize = "100MB";

                appender.StaticLogFileName = false;

                appender.ImmediateFlush = true;

                appender.PreserveLogFileNameExtension = true;
                appender.AddFilter(filter);
                appender.Layout = layout;

                //配置缓存，增加日志效率
                //var bufferappender = new BufferingForwardingAppender();
                //bufferappender.AddAppender(appender);
                //bufferappender.BufferSize = 500;
                //bufferappender.Lossy = false;
                //bufferappender.Fix = FixFlags.None;
                //bufferappender.ActivateOptions();

                var repository = LogManager.CreateRepository(name);
                BasicConfigurator.Configure(repository, appender);

                log4net.Core.ILogger logger = repository.GetLogger(name);
                return logger;
            });

            return data.Value;
        }
    }
}
