using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsMultipleDbContextConfiguration
    {
        public IEnumerable<IArsDbContextConfiguration> ArsDbContextConfiguration { get; }
    }
}
