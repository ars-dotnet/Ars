using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Interfaces
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }

    public interface ICreateEntity<T, K> : IEntity<T>
    {
        K? CreationUserId { get; set; }

        DateTime CreationTime { get; set; }
    }

    public interface IModifyEntity<T, K, M> : ICreateEntity<T, K>
    {
        M? ModifyEntityId { get; set; }

        DateTime? ModifiedTime { get; set; }
    }

    public interface IModifyEntity<T, K> : IEntity<T>
    {
        K? ModifyEntityId { get; set; }

        DateTime? ModifiedTime { get; set; }
    }

    public interface IDeleteEntity<T>
    {
        public T? DeleteUserId { get; set; }

        public DateTime? DeleteTime { get; set; }
    }
}
