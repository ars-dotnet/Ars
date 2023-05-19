using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Diagnostic
{
    public class ArsCommandEventData
    {
        public DbCommand DbCommand { get; set; }

        public IEnumerable<ChangerTable> ChangerTables { get; set; }
    }
}
