using Ars.Common.Core.Diagnostic;
using Ars.Common.Tool.Extension;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using SkyApm;
using SkyApm.Common;
using SkyApm.Diagnostics;
using SkyApm.Diagnostics.EntityFrameworkCore;
using SkyApm.Tracing.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArsEvents = Ars.Common.Core.Diagnostic.ArsDiagnosticNames;

namespace Ars.Common.SkyWalking.Diagnostics.EntityFrameworkCore
{
    internal class ArsEntityFrameworkCoreTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        private readonly IEntityFrameworkCoreSegmentContextFactory _contextFactory;
        public ArsEntityFrameworkCoreTracingDiagnosticProcessor(
            IEntityFrameworkCoreSegmentContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public string ListenerName => ArsEvents.ListenerName;

        [DiagnosticName(ArsEvents.CompleteTransactionName)]
        public void Complete([Object] ArsCommandEventData eventData) 
        {
            var context = _contextFactory.Create(ArsEvents.CompleteTransactionName, eventData.DbCommand);
            if (null == context)
                return;

            context.Span.SpanLayer = SpanLayer.DB;
            context.Span.Component = new StringOrIntValue(5001, "Ars");
            context.Span.AddTag(Tags.DB_TYPE, "Sql");
            context.Span.AddLog(LogEvent.Event(ArsEvents.CompleteTransactionName));

            StringBuilder log = new StringBuilder();
            foreach (var state in eventData.ChangerTables.GroupBy(r => r.EntityState)) 
            {
                log.AppendLine($"实体操作 -> {state.Key.GetDescriotion()}");
                foreach (var table in eventData.ChangerTables.Where(r => r.EntityState == state.Key))
                {
                    log.AppendLine($"表名:{table.TableName}");
                    log.AppendLine($"原值:{(null == table.OriginalValues ? string.Empty : JsonConvert.SerializeObject(table.OriginalValues))}");
                    log.AppendLine($"新值:{(null == table.CurrentValues ? string.Empty : JsonConvert.SerializeObject(table.CurrentValues))}");
                }
                log.AppendLine($"----------------------------------------------------------------------------------------------------------------------");
            }

            context.Span.AddLog(LogEvent.Message(log.ToString()));
            _contextFactory.Release(context);
        }
    }
}
