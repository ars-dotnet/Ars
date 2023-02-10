using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Entities
{
    public interface IDeleteEntity
    {
        public int? DeleteUserId { get; set; }

        public DateTime? DeleteTime { get; set; }
    }
}
