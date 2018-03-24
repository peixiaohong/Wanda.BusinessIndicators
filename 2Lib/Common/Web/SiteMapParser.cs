using Lib.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LJTH.BusinessIndicators.Common
{
    public class SiteMapParser
    {

        private string _mapFile = null;

        public SiteMapParser(string mapFile)
        {
            _mapFile = mapFile;
        }

        private NavSiteMap GetMap()
        {
            if (string.IsNullOrEmpty(_mapFile) || File.Exists(_mapFile) == false)
            {
                throw new ApplicationException("导航文件无效。 MapFile=" + _mapFile);
            }


            XDocument xdoc = XDocument.Load(_mapFile);

            NavSiteMap result = new NavSiteMap();

            result.Nodes = ParseNodes(xdoc.Root.Elements().First().Elements()); //必须有个SiteMapNode根节点， 但这个节点不需要属性

            return result;
        }

        /// <summary>
        /// 从缓存中加载数据， 或加载数据后并缓存
        /// </summary>
        /// <returns></returns>
        public NavSiteMap GetCachedMap()
        {
            string cacheKey = _mapFile;

            NavSiteMap result = null;
            if (SiteMapCache.Instance.TryGetValue(cacheKey, out result) == false)
            {
                result = GetMap();
                FileCacheDependency fd = new FileCacheDependency(false, _mapFile);
                SiteMapCache.Instance.Add(cacheKey, result, fd);
            }
            return result;
        }

        private List<NavSiteMapNode> ParseNodes(IEnumerable<XElement> elements)
        {

            List<NavSiteMapNode> result = new List<NavSiteMapNode>();

            foreach (XElement item in elements)
            {
                if (item.Name.LocalName == "siteMapNode")
                {
                    NavSiteMapNode node = ParseNode(item);
                    result.Add(node);
                    if (item.Elements().Count() > 0)
                    {
                        node.Nodes = ParseNodes(item.Elements());
                    }
                }
            }

            return result;
        }

        private NavSiteMapNode ParseNode(XElement element)
        {
            NavSiteMapNode result = new NavSiteMapNode();
            result.ID = GetXAttributeValue(element, "id", "");
            result.Url = GetXAttributeValue(element, "url", "");
            result.Title = GetXAttributeValue(element, "title", "");
            result.Description = GetXAttributeValue(element, "description", "");
            result.ResourceKey = GetXAttributeValue(element, "resourceKey", "");
            result.Enable = bool.Parse(GetXAttributeValue(element, "enable", "true")); //默认为true
            result.Popup = bool.Parse(GetXAttributeValue(element, "popup", "false")); //默认为true
            return result;
        }


        private static string GetXAttributeValue(XElement element, string attributeName, string defaultValue)
        {
            var xAttr = element.Attribute(attributeName);

            if (xAttr == null)
            {
                return defaultValue;
            }
            return xAttr.Value;
        }


    }

    public sealed class NavSiteMap
    {
        public NavSiteMap()
        {
            Nodes = new List<NavSiteMapNode>();
        }
        public List<NavSiteMapNode> Nodes { get; set; }

    }

    public sealed class NavSiteMapNode
    {

        public NavSiteMapNode()
        {
            Nodes = new List<NavSiteMapNode>();

        }

        public NavSiteMapNode(NavSiteMapNode node)
            : this()
        {
            // TODO: Complete member initialization
            this.ID = node.ID;
            this.Url = node.Url;
            this.Title = node.Title;
            this.Description = node.Description;
            this.ResourceKey = node.ResourceKey;
            this.Enable = node.Enable;
            this.Selected = node.Selected;
            this.Popup = node.Popup;
        }

        public List<NavSiteMapNode> Nodes { get; set; }

        public string ID { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceKey { get; set; }

        public bool Enable { get; set; }
        public bool Selected { get; set; }
        public bool Popup { get; set; }
    }

    public sealed class SiteMapCache : CacheQueue<string, NavSiteMap>
    {
        private SiteMapCache()
        {

        }
        public static SiteMapCache Instance = new SiteMapCache();
    }



}
