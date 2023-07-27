using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Loggers
{
    internal class ArsHierarchy : Hierarchy
    {
        public Arslogextension arslogextension { get; set; }

        public class Arslogextension 
        {
            public string customLogCategoryPrefix { get; set; }
        }
    }
}
