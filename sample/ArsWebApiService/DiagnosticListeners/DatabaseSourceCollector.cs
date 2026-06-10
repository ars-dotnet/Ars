using Ars.Common.Core.Diagnostic;
using Ars.Common.Tool.Extension;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SkyApm.Common;
using SkyApm.Diagnostics;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace ArsWebApiService.DiagnosticListeners
{
    public class DatabaseSourceCollector : IObserver<DiagnosticListener>
    {
        private readonly ILogger<DatabaseSourceCollector> _logger;

        private readonly IHttpContextAccessor _contextAccessor;

        public DatabaseSourceCollector(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseSourceCollector>();

            _contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        }

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {

        }

        [Microsoft.Extensions.DiagnosticAdapter.DiagnosticName(ArsDiagnosticNames.CompleteTransactionName)]
        public void OnCommandExecute(DbCommand dbcommand, IEnumerable<ChangerTable> changertables)
        {
            StringBuilder log = new StringBuilder();

            foreach (var state in changertables.GroupBy(r => r.EntityState))
            {
                log.AppendLine($"实体操作 -> {state.Key.GetDescriotion()}");

                foreach (var table in changertables.Where(r => r.EntityState == state.Key))
                {
                    log.AppendLine($"表名:{table.TableName}");
                    log.AppendLine($"原值:{(null == table.OriginalValues ? string.Empty : JsonConvert.SerializeObject(table.OriginalValues))}");
                    log.AppendLine($"新值:{(null == table.CurrentValues ? string.Empty : JsonConvert.SerializeObject(table.CurrentValues))}");
                }

                log.AppendLine("----------------------------------------------------------------------------------------------------------------------");
            }

            _logger.LogInformation($"Api:{_contextAccessor?.HttpContext?.Request.Path},表数据变更日志:\r\n{log.ToString()}");
        }
    }
}
