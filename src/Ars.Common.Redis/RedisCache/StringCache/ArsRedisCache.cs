﻿using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.StringCache
{
    public class ArsRedisCache : ArsCacheBaseAccessor, ICache, ITransientDependency
    {
        public ArsRedisCache(ILogger<ArsRedisCache> logger)
            : base(logger)
        {

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

        public override Task<long> RemoveAsync(string key)
        {
            return RedisHelper.DelAsync(GetLocalizedCacheKey(key));
        }

        public override async Task SetAsync<TValue>(string key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            string rediskey = GetLocalizedCacheKey(key);
            var redisvalue = value;
            if (absoluteExpireTime.HasValue)
            {
                await RedisHelper.SetAsync(rediskey, redisvalue);
                await RedisHelper.ExpireAtAsync(rediskey, absoluteExpireTime.Value.UtcDateTime);
            }
            else if (slidingExpireTime.HasValue)
            {
                await RedisHelper.SetAsync(rediskey, redisvalue, slidingExpireTime.Value);
            }
            else if (DefaultAbsoluteExpireTime.HasValue)
            {
                await RedisHelper.SetAsync(rediskey, redisvalue);
                await RedisHelper.ExpireAtAsync(rediskey, DefaultAbsoluteExpireTime.Value.UtcDateTime);
            }
            else
            {
                await RedisHelper.SetAsync(rediskey, redisvalue, DefaultSlidingExpireTime);
            }
        }

        public override async Task<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>(string key)
        {
            var value = await RedisHelper.GetAsync<TValue>(GetLocalizedCacheKey(key));
            return new ConditionalValue<TValue>(null != value, value);
        }
    }
}
