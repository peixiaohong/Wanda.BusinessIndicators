using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Cache;

namespace Lib.Xml
{
    internal sealed class XmlMappingsCache : CacheQueue<System.Type, XmlObjectMappingItemCollection>
    {
        public static readonly XmlMappingsCache Instance = CacheManager.GetInstance<XmlMappingsCache>();

        private XmlMappingsCache()
        {
        }
    }
}
