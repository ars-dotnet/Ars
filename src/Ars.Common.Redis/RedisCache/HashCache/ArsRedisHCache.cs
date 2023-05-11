using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.HashCache
{
    public class ArsRedisHCache : ArsCacheBase,IHCache,ITransientDependency
    {
        public ArsRedisHCache(ILogger<ArsRedisHCache> logger) : base(logger)
        {
            DefaultSlidingExpireTime = TimeSpan.FromMinutes(15).Add(TimeSpan.FromSeconds(new Random().Next(10)));
        }

        public override Task ClearAsync()
        {
            try
            {
                //var script = $"return redis.call('del', unpack(redis.call('keys','*{pattern}*')))";
                var script = $@" 
local dataInfos = redis.call('keys','{GetLocalizedCacheKey("*")}') 
if(dataInfos ~= nil) then 
        for i=1,#dataInfos,1 do
                redis.call('del',dataInfos[i])
        end 
        return #dataInfos
else
        return 0
end
";
                RedisHelper.Eval(script, "");
            }
            catch
            {
                throw;
            }
            return Task.CompletedTask;
        }

        protected override string GetLocalizedCacheKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }

        private async Task ExpireAsync(string key, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null) 
        {
            if (absoluteExpireTime.HasValue)
                await RedisHelper.ExpireAtAsync(key, absoluteExpireTime.Value.UtcDateTime);
            else if (slidingExpireTime.HasValue)
                await RedisHelper.ExpireAsync(key, (int)slidingExpireTime.Value.TotalSeconds);
            else if(DefaultAbsoluteExpireTime.HasValue)
                await RedisHelper.ExpireAtAsync(key, DefaultAbsoluteExpireTime.Value.UtcDateTime);
            else
                await RedisHelper.ExpireAsync(key, (int)DefaultSlidingExpireTime.TotalSeconds);
        }

        public async Task<bool> HMSetAsync(string key, Dictionary<string, object> keyValues, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            string rediskey = GetLocalizedCacheKey(key);
            var kvalues = keyValues.Select(r => new []{ r.Key, r.Value }).SelectMany(r => r).ToArray();
            
            var flag = await RedisHelper.HMSetAsync(rediskey, kvalues);
            await ExpireAsync(rediskey, slidingExpireTime, absoluteExpireTime);
            return flag;
        }

        public async Task<bool> HSetAsync(string key, string field, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            string rediskey = GetLocalizedCacheKey(key);
            var flag = await RedisHelper.HSetAsync(rediskey, field, value);
            await ExpireAsync(rediskey, slidingExpireTime, absoluteExpireTime);
            return flag;
        }

        public Task<long> HDelAsync(string key, params string[] fields)
        {
            return RedisHelper.HDelAsync(GetLocalizedCacheKey(key),fields);
        }

        public Task<TValue> HGetAsync<TValue>(string key, string field)
        {
            return RedisHelper.HGetAsync<TValue>(GetLocalizedCacheKey(key), field);
        }

        public Task<Dictionary<string, TValue>> HGetAllAsync<TValue>(string key)
        {
            return RedisHelper.HGetAllAsync<TValue>(GetLocalizedCacheKey(key));
        }

        public Task<TValue[]> HMGetAsync<TValue>(string key, params string[] fields)
        {
            return RedisHelper.HMGetAsync<TValue>(GetLocalizedCacheKey(key), fields);
        }

        public Task<string[]> HKeysAsync(string key) 
        {
            return RedisHelper.HKeysAsync(GetLocalizedCacheKey(key));
        }
    }
}
