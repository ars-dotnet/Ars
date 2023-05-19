using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Diagnostic
{
    public enum DiagnosticEntityState
    {
        Default,

        [Description("新增")]
        Added,

        [Description("修改")]
        Modified,

        [Description("删除")]
        Deleted
    }
}
