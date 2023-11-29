using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    public interface IEFCoreExistTransactionConnectionStorage : IScopedDependency
    {
        void AddConnection(string connectionString, DbConnection dbConnection);

        bool TryGetConnection(string connectionString, out DbConnection? dBConnection);
    }
}
