using Ars.Common.Core.IDependency;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.AdoNet
{
    public interface IDbWriter<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        Task<IDbContextTransaction> BeginAsync(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);

        Task<int> ExecuteNonQuery(string commandText, SqlParameter[]? parameters, IDbContextTransaction? dbTransaction = null);
    }
}
