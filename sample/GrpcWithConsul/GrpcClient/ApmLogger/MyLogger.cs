using MSLogger = Microsoft.Extensions.Logging.ILogger;

namespace GrpcClient.ApmLogger
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
