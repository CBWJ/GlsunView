using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace GlsunView.Infrastructure.Util
{
    public static class MemoryCacheHelper
    {
        /// <summary>
        /// 同步对象
        /// </summary>
        private static object _locker = new object();
        private static object _locker2 = new object();
        /// <summary>
        /// 创建缓存项的逐出和过期策略
        /// </summary>
        /// <param name="slidingExpiration">缓存项在给定时段内未被访问被逐出</param>
        /// <param name="absoluteExpiration">在指定持续时间过后逐出某个缓存项</param>
        /// <returns></returns>
        public static CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            if (slidingExpiration.HasValue)
                policy.SlidingExpiration = slidingExpiration.Value;
            if (absoluteExpiration.HasValue)
                policy.AbsoluteExpiration = absoluteExpiration.Value;
            policy.Priority = CacheItemPriority.Default;
            return policy;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">value返回类型</typeparam>
        /// <param name="key">key名称</param>
        /// <param name="cacheReadFunc">获取value委托</param>
        /// <param name="slidingExpiration">过期时间</param>
        /// <param name="absoluteExpiration">逐出时间</param>
        /// <returns></returns>
        public static T GetCacheItem<T>(string key, Func<T> cacheReadFunc, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");
            if (cacheReadFunc == null) throw new ArgumentNullException("cacheReadFunc");
            if (slidingExpiration == null && absoluteExpiration == null) throw new ArgumentException("Either a sliding expiration or absolute must be provided");
            //在取缓存时更新缓存
            if (MemoryCache.Default[key] == null)
            {
                lock (_locker)
                {
                    //再次判断，以免其他线程已设置
                    if (MemoryCache.Default[key] == null)
                    {
                        var item = new CacheItem(key, cacheReadFunc());
                        var policy = CreatePolicy(slidingExpiration, absoluteExpiration);
                        MemoryCache.Default.Set(item, policy);
                    }
                }
            }
            return (T)MemoryCache.Default[key];
        }

        public static void RemoveCache(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public static void ClearCache()
        {
            List<string> keys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (var key in keys)
            {
                MemoryCache.Default.Remove(key);
            }
        }

        /// <summary>
        /// 设置简单的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCache(string key, object value)
        {
            lock (_locker2)
            {
                var policy = CreatePolicy(null, DateTime.Now.AddYears(10));
                MemoryCache.Default.Set(key, value, policy);
            }
        }

        public static object GetCache(string key)
        {
            lock (_locker2)
            {
                return MemoryCache.Default[key];
            }
        }
    }
}
