using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Diagnostic
{
    public class ChangerTable
    {
        public string TableName { get; set; }

        public DiagnosticEntityState EntityState { get; set; }

        public JObject? OriginalValues { get; set; }

        public JObject? CurrentValues { get; set; }
    }
}
