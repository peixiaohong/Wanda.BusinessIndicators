using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Web;
using System.Web.Caching;

namespace BPF.Workflow.Client.Tools
{
    /// <summary>
    /// 缓存工具（HttpRuntime.Cache）
    /// </summary>
    public class CacheHelper
    {
        public static Cache CacheInstance
        {
            get { return CacheHelper._Cache; }
            set { CacheHelper._Cache = value; }
        }private static Cache _Cache = HttpRuntime.Cache;

        /// <summary>
        /// 增加缓存
        /// 时间参数为当前时间后，多久过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        /// <returns>添加成功返回true，如果存在key相同的，则添加失败，返回false</returns>
        public static void Add(string app, string typeName, string key, object value, int seconds, int minutes = 0, int hours = 0, int days = 0)
        {
            DateTime offset = DateTime.Now.AddSeconds(seconds).AddMinutes(minutes).AddHours(hours).AddDays(days);
            CacheInstance.Add(BuildCacheKey(app, typeName, key), value, null, offset, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }
        /// <summary>
        /// 更新或新增缓存
        /// 如果有key相同的缓存，则更新该缓存，否则添加新缓存
        /// 时间参数为当前时间后，多久过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        public static void Upsert(string app, string typeName, string key, object value, int seconds, int minutes = 0, int hours = 0, int days = 0)
        {
            DateTimeOffset offset = DateTime.Now.AddSeconds(seconds).AddMinutes(minutes).AddHours(hours).AddDays(days);
            if (CacheInstance.Get(BuildCacheKey(app, typeName, key)) == null)
            {
                Add(app, typeName, key, value, seconds, minutes, hours, days);
            }
            else
            {
                CacheInstance[BuildCacheKey(app, typeName, key)] = value;
            }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>无缓存返回null</returns>
        public static System.Object GetValue(string app, string typeName, string key)
        {
            return CacheInstance.Get(BuildCacheKey(app, typeName, key));
        }
        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>如果存在缓存，返回该对象值，否则返回null</returns>
        public static System.Object Delete(string app, string typeName, string key)
        {
            return CacheInstance.Remove(BuildCacheKey(app, typeName, key));
        }

        private static string BuildCacheKey(string app, string typeName, string key)
        {
            return string.Format("{0}-{1}-{2}", app, typeName, key);
        }
    }
}
