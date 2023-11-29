using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    internal class EFCoreExistTransactionConnectionStorage : IEFCoreExistTransactionConnectionStorage
    {
        private readonly IDictionary<string, DbConnection> _storage;
        public EFCoreExistTransactionConnectionStorage()
        {
            _storage = new Dictionary<string, DbConnection>();
        }

        public void AddConnection(string connectionString, DbConnection dbConnection)
        {
            _storage.TryAdd(connectionString, dbConnection);
        }

        public bool TryGetConnection(string connectionString,out DbConnection? dBConnection)
        {
            return _storage.TryGetValue(connectionString,out dBConnection);
        }
    }
}
