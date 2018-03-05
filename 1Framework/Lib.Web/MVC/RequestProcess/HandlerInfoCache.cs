using System;
using System.Collections.Generic;
using Lib.Cache;

namespace Lib.Web.MVC
{
    internal class ControllerInfoCache : CacheQueue<System.Type, ControllerInfo>
    {
        public static readonly ControllerInfoCache Instance = CacheManager.GetInstance<ControllerInfoCache>();

        private ControllerInfoCache()
        {
        }
    }
}
