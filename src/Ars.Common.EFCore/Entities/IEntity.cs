using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Entities
{
    public interface IEntity
    {

    }

    public interface IEntity<TPrimaryKey> : IEntity 
    {
        TPrimaryKey Id { get; set; }
    }
}
