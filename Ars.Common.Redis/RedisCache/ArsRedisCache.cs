using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache
{
    public class ArsRedisCache : ArsCacheBase<string,object>, ICache, ITransientDependency
    {
        private readonly IArsSerializer _arsSerializer;

        public ArsRedisCache(ILogger<ArsRedisCache> logger, IArsSerializer arsSerializer) 
            : base(logger)
        {
            _arsSerializer = arsSerializer;
        }

        public override Task ClearAsync()
        {
            try
            {
                //var script = $"return redis.call('del', unpack(redis.call('keys','*{pattern}*')))";
                var script = $@" 
local dataInfos = redis.call('keys','{GetLocalizedRedisKey("*")}') 
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

        public override Task<long> RemoveAsync(string key)
        {
            return RedisHelper.DelAsync(GetLocalizedRedisKey(key));
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            string rediskey = GetLocalizedRedisKey(key);
            string redisvalue = _arsSerializer.SerializeToJson(value);
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

        public override async Task<ConditionalValue<object>> GetValueOrDefaultAsync(string key)
        {
            var value = await RedisHelper.GetAsync<object>(GetLocalizedRedisKey(key));
            return new ConditionalValue<object>(null != value,value);
        }

        protected virtual string GetLocalizedRedisKey(string key) 
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
