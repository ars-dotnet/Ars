using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.CacheConfiguration
{
    public interface ICacheConfiguration
    {
        string CacheName { get; }

        Action<IArsCacheOption> Action { get; }
    }

    public class CacheConfiguration : ICacheConfiguration
    {
        public CacheConfiguration()
        {

        }

        public CacheConfiguration([NotNull]Action<IArsCacheOption> action)
        {
            Action = action;
        }

        public CacheConfiguration(string cacheName, [NotNull]Action<IArsCacheOption> action)
        {
            CacheName = cacheName;
            Action = action;
        }

        public string CacheName { get; set; }

        public Action<IArsCacheOption> Action { get;set; }
    }
}
