using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchUtil
{
    public class A
    {
        public virtual int GetNum() 
        {
            return 0;
        }
    }

    public class B : A 
    {
        public override int GetNum() 
        {
            return 1;
        }
    }
}
