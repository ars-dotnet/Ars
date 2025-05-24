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

        }
    }
}
