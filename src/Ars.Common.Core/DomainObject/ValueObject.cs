using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.DomainObject
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetAtomicValues();

        public bool ValueEquals(ValueObject? other)
        {
            if(null == other || this.GetType() != other.GetType())
                return false;

            IEnumerator<object> currentvalue = this.GetAtomicValues().GetEnumerator();
            IEnumerator<object> othervalue = other.GetAtomicValues().GetEnumerator();
            while (currentvalue.MoveNext() && othervalue.MoveNext()) 
            {
                if (currentvalue.Current == null ^ othervalue.Current == null) //只有一个为null
                    return false;
                if (!Equals(currentvalue.Current, othervalue.Current))
                    return false;
            }

            return !currentvalue.MoveNext() && !othervalue.MoveNext();
        }
    }
}
