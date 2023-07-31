using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap
{
    public class ArsCapSubscribeAttribute : CapSubscribeAttribute
    {
        public ArsCapSubscribeAttribute(string name, bool isPartial = false) 
            : base(name, isPartial)
        {

        }
    }
}
