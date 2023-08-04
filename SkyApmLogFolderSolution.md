
## 1.新建类MyLoggerFactory实现SkyApm.Logging.ILoggerFactory接口.如果内容报错找不到引用，请看步骤5.

```
using Serilog.Events;
using Serilog;
using SkyApm.Config;
using SkyApm.Utilities.Logging;
using MSLoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace ApmLogger
{
    public class MyLoggerFactory : SkyApm.Logging.ILoggerFactory
    {
        private const string outputTemplate =
            @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{ServiceName}] [{Level}] {SourceContext} : {Message}{NewLine}{Exception}";

        private readonly MSLoggerFactory _loggerFactory;
        private readonly LoggingConfig _loggingConfig;

        public MyLoggerFactory(IConfigAccessor configAccessor)
        {
            _loggingConfig = configAccessor.Get<LoggingConfig>();
            _loggerFactory = new MSLoggerFactory();
            var instrumentationConfig = configAccessor.Get<InstrumentConfig>();

            var level = EventLevel(_loggingConfig.Level);

            _loggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Verbose().Enrich
                .WithProperty("SourceContext", null).Enrich
                .WithProperty(nameof(instrumentationConfig.ServiceName),
                    instrumentationConfig.ServiceName).Enrich
                .FromLogContext().WriteTo.Async(o =>
                    o.File(string.Format(_loggingConfig.FilePath, DateTime.Now.ToString("yyyyMM")), level, outputTemplate, flushToDiskInterval: TimeSpan.FromMilliseconds(500), rollingInterval: RollingInterval.Infinite))
                .CreateLogger());
        }

        public SkyApm.Logging.ILogger CreateLogger(Type type)
        {
            return new MyLogger(_loggerFactory.CreateLogger(type));
        }

        private static LogEventLevel EventLevel(string level)
        {
            return Enum.TryParse<LogEventLevel>(level, out var logEventLevel)
                ? logEventLevel
                : LogEventLevel.Error;
        }
    }
}
```


## 2.添加类MyLogger实现SkyApm.Logging.ILogger接口

```
using MSLogger = Microsoft.Extensions.Logging.ILogger;

namespace ApmLogger
{
    public class MyLogger : SkyApm.Logging.ILogger
    {
        private readonly MSLogger _readLogger;
        public MyLogger(MSLogger readLogger)
        {
            _readLogger = readLogger;
        }

        public void Debug(string message)
        {
            _readLogger.LogDebug(message);
        }

        public void Error(string message, Exception exception)
        {
            _readLogger.LogError(message + Environment.NewLine + exception);
        }

        public void Information(string message)
        {
            _readLogger.LogInformation(message);
        }

        public void Trace(string message)
        {
            _readLogger.LogTrace(message);
        }

        public void Warning(string message)
        {
            _readLogger.LogWarning(message);
        }
    }
}
```

## 3.Program.cs替换服务
```
builder.Services.Replace(ServiceDescriptor.Singleton<SkyApm.Logging.ILoggerFactory, MyLoggerFactory>());
```

## 4.修改skyapm.json配置文件中Logging:FilePath的规则
```
    "Logging": {
      "Level": "Information",
      "FilePath": "logs\\{0}\\skyapm.log"
    },
```

## 5.如果报错找不到引用，则添加以下引用
```
    <PackageReference Include="Serilog.Sinks.Async" Version="1.5.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

## 6.如果每天的日志文件路径没有更新，则类MyLoggerFactory做以下调整
```
using Serilog.Events;
using Serilog;
using SkyApm.Config;
using SkyApm.Utilities.Logging;
using MSLoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace SkyApm.Sample.Logging
{
    public class MyLoggerFactory : SkyApm.Logging.ILoggerFactory
    {
        private const string outputTemplate =
            @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{ServiceName}] [{Level}] {SourceContext} : {Message}{NewLine}{Exception}";

        private readonly MSLoggerFactory _loggerFactory;
        private readonly LoggingConfig _loggingConfig;
        private readonly InstrumentConfig _instrumentConfig;
        private readonly LogEventLevel _logEventLevel;
        public MyLoggerFactory(IConfigAccessor configAccessor)
        {
            _loggingConfig = configAccessor.Get<LoggingConfig>();
            _loggerFactory = new MSLoggerFactory();
            _instrumentConfig = configAccessor.Get<InstrumentConfig>();
            _logEventLevel = EventLevel(_loggingConfig.Level);
        }

        public SkyApm.Logging.ILogger CreateLogger(Type type)
        {
            _loggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Verbose().Enrich
                .WithProperty("SourceContext", null).Enrich
                .WithProperty(nameof(_instrumentConfig.ServiceName),
                    _instrumentConfig.ServiceName).Enrich
                .FromLogContext().WriteTo.Async(o =>
                    o.File(string.Format(_loggingConfig.FilePath, DateTime.Now.ToString("yyyyMMdd")), _logEventLevel, outputTemplate, flushToDiskInterval: TimeSpan.FromMilliseconds(500), rollingInterval: RollingInterval.Infinite))
                .CreateLogger());

            return new MyLogger(_loggerFactory.CreateLogger(type));
        }

        private static LogEventLevel EventLevel(string level)
        {
            return Enum.TryParse<LogEventLevel>(level, out var logEventLevel)
                ? logEventLevel
                : LogEventLevel.Error;
        }
    }
}
```