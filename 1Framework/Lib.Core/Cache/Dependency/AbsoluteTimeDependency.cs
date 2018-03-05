using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Cache
{
    public class AbsoluteTimeDependency : DependencyBase
    {
        private static TimeSpan cacheItemExpirationTime = TimeSpan.Zero;
        public static DateTime NextExpirationTime
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - DateTime.Now;
                if (_nextExpirationTime == null || _nextExpirationTime==DateTime.MinValue)
                {
                    _nextExpirationTime = DateTime.UtcNow.Date.Add(ts + cacheItemExpirationTime);
                }
                return _nextExpirationTime;
            }
            set
            {
                _nextExpirationTime = value;
            }
        }
        private static DateTime _nextExpirationTime;
        /// <summary>
        /// 获取初始化时设定的过期时间间隔
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static TimeSpan CacheItemExpirationTime
        {
            get { return cacheItemExpirationTime; }
        }

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expirationTime">过期时间间隔</param>
        /// <remarks>
        /// </remarks>
        public AbsoluteTimeDependency(TimeSpan expirationTime)
        {
            cacheItemExpirationTime = expirationTime;
            //更新最后修改时间和最后访问时间
            UtcLastModified = DateTime.UtcNow;
            UtcLastAccessTime = DateTime.UtcNow;
        }
        #endregion

        /// <summary>
        /// 属性，获取本Dependency是否过期
        /// </summary>
        /// <remarks>
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                if (DateTime.UtcNow >= NextExpirationTime)
                {
                    NextExpirationTime = NextExpirationTime.AddDays(1);
                    return true;
                }
                return false;
            }
        }
    }
}
