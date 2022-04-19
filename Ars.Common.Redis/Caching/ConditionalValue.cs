using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public struct ConditionalValue<TValue>
    {
        public ConditionalValue(bool hasvalue,TValue? value)
        {
            HasValue = hasvalue;
            Value = value;
        }

        public bool HasValue { get; }
        
        public TValue? Value { get; }
    }
}
