using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore
{
    public class ArsMultipleDbContextConfiguration : IArsMultipleDbContextConfiguration
    {
        public IList<ArsDbContextConfiguration> ArsDbContextConfigurations { get; set; }
        

        public IEnumerable<IArsDbContextConfiguration> ArsDbContextConfiguration
        {
            get
            {
                foreach (var config in ArsDbContextConfigurations)
                {
                   yield return config;
                }
            }
        }
    }
}
