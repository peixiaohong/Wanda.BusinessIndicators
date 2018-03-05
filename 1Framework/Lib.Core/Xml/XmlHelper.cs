using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using Lib.Core;
using Lib.Core.Properties;
using System.Xml.Linq;
using System.Linq;
using System.Web;

namespace Lib.Xml
{
    /// <summary>
    /// 本基类提供和XML相关数据处理的相关函数，这里包含的所有方法都是与XML文档对象操作相关，目的就是为了方便程序员的程序开发，提供有效、快捷的共
    /// 用函数
    /// </summary>
    /// <remarks>
    /// 本基类提供和XML相关数据处理的相关函数。这里采用静态方法的形式提供出各种数据对象与XML数据之间的数据转换。
    /// <list type="bullet">
    /// <item>数据集到XML文档对象的转换。如DataSet, DataReader等的数据转换成XML文档对象的方法</item>
    /// <item>XML文档对象的节点上添加节点和属性的不同操作</item>
    /// <item>XML文档对象中节点替换、存在、遍历的处理</item>
    /// </list>
    /// </remarks>
    public static class XmlHelper
    {
        /// <summary>
        /// 在枚举Xml NodeList的时候回调的函数接口
        /// </summary>
        /// <param name="nodeRoot">Xml文档的节点</param>
        /// <param name="oParam">对Xml文档的节点所做的操作</param>
        /// <remarks>通过回调函数，实现根据xml节点的需要而进行不同的操作功能。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" lang="cs" title="枚举Xml NodeList时候的回调函数接口" />
        /// </remarks>
        public delegate void DoNodeList(XmlNode nodeRoot, object oParam);

        #region 根据Exception生成Xml文档对象

        /// <summary>
        /// 将异常对象生成为一个XML文档对象，相当于将Exception序列化为Xml
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns>表示异常对象的Xml文档</returns>
        /// <remarks>相当于将Exception对象序列化为Xml文档对象，有利于Exception对象输出到用户
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetExceptionXmlTest" lang="cs" title="将Exception序列化为Xml" />
        /// </remarks>
        public static XmlDocument GetExceptionXmlDoc(System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            XmlDocument xmlDoc = CreateDomDocument("<ResponseError/>");
            XmlElement root = xmlDoc.DocumentElement;

            AppendExceptionNode(root, ex);

            return xmlDoc;
        }

        private static void AppendExceptionNode(XmlNode root, System.Exception ex)
        {
            AppendNode(root, "Value", ex.Message);
            AppendNode(root, "Stack", ex.StackTrace);

            if (ex.InnerException != null)
            {
                XmlNode nodeInnerEx = AppendNode(root, "InnerException");

                AppendExceptionNode(nodeInnerEx, ex.InnerException);
            }
        }

        /// <summary>
        /// 在XmlWriter中添加Exception的信息
        /// </summary>
        /// <param name="writer">XmlWriter对象</param>
        /// <param name="ex">异常对象</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="AppendExceptionInfoTest" lang="cs" title="将Exception序列化为Xml" />
        /// </remarks>
        public static void AppendExceptionInfo(XmlWriter writer, System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            writer.WriteStartElement("ResponseError");
            AppendExceptionNode(writer, ex);
            writer.WriteEndElement();
        }

        private static void AppendExceptionNode(XmlWriter writer, System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            XmlHelper.AppendNode(writer, "Value", ex.Message);
            XmlHelper.AppendNode(writer, "Stack", ex.StackTrace);

            if (ex.InnerException != null)
            {
                writer.WriteStartElement("InnerException");
                AppendExceptionNode(writer, ex.InnerException);
                writer.WriteEndElement();
            }
        }
        #endregion

        #region 加载和建立Xml文档对象
        /// <summary>
        /// 从某个磁盘文件加载XML文档对象
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>XML文档对象</returns>
        /// <remarks>从某个磁盘文件加载XML文档对象，该方法和XmlDocument.Load的不同点在于它支持共享读，即使在别的程序正在写这个文件
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="LoadDocumentTest" lang="cs" title="从磁盘加载Xml文档对象" />
        ///</remarks>
        public static XmlDocument LoadDocument(string path)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(fs);

                return xmlDoc;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// 建立一个XML文档对象，并初始化文档（方便文档对象的创建和初始化）
        /// </summary>
        /// <param name="xml">XML文档对象在初始化的时候要求的数据值</param>
        /// <returns>创建以后并且被初始化的XmlDocument对象</returns>
        /// <remarks>
        /// 建立一个XML文档对象，并且可以初始化文档（方便文档对象的创建和初始化）。该函数包含了XML文档对象
        /// 创建和初始化的两步工作，这样方便了编程人员的创建XML文档对象和初始化的工作。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="CreateDocumentTest" lang="cs" title="创建一个Xml文档对象" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = XMLHelper.CreateDomDocument("&lt;DataSet/&gt;");
        /// </code>
        /// </example>
        public static XmlDocument CreateDomDocument(string xml)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(xml, "xml");

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xml);

            return xmlDoc;
        }
        #endregion 加载和建立Xml文档对象

        #region AppendNode和Attribute系列
        /// <summary>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点
        /// </summary>
        /// <param name="root">文档对象中的指定要添加子节点的根节点</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <returns>被添加的子节点对象</returns>
        /// <remarks>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点。该子节点的名称由
        /// nodeName指定，它的内容（InnerXml）为空。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="AppendNodeTest" lang="cs" title="在xml文档对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = CreateDomDocument("&lt;FIRST&gt;&lt;SECOND&gt;&lt;THIRD/&gt;&lt;/SECOND&gt;&lt;/FIRST&gt;");
        /// XmlNode root = xmlDoc.DocumentElement.FirstChild;
        /// XmlNode node = XMLHelper.AppendNode(root, "FOURTH");
        /// </code>
        /// 结果为：
        /// <code>
        /// &lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH/&gt;
        ///		&lt;/SECOND&gt;
        /// &lt;/FIRST&gt;
        /// </code>
        /// </example>
        public static XmlNode AppendNode(XmlNode root, string nodeName)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, root.NamespaceURI);

            root.AppendChild(node);

            return node;
        }

        /// <summary>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlNode AppendNodeWithNamespace(XmlNode root, string nodeName, string namespaceURI)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");
            ExceptionHelper.FalseThrow<ArgumentNullException>(namespaceURI != null, "namespaceURI");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, namespaceURI);

            root.AppendChild(node);

            return node;
        }

        /// <summary>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNodeWithNamespace<T>(XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            XmlNode node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

            if (nodeValue != null)
                node.InnerText = nodeValue.ToString();

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendCDataNodeWithNamespace<T>(XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");
            ExceptionHelper.FalseThrow<ArgumentNullException>(namespaceURI != null, "namespaceURI");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, namespaceURI);

            root.AppendChild(node);

            if (nodeValue != null)
            {
                XmlCDataSection cdata = root.OwnerDocument.CreateCDataSection(nodeValue.ToString());

                node.AppendChild(cdata);
            }

            return node;
        }

        /// <summary>
        /// 如果nodeValue不为空(null或string.Empty)，在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotNullNodeWithNamespace<T>(XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = nodeValue.ToString();

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

                if (nodeValue != null)
                    node.InnerText = nodeValue.ToString();
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotDefaultNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            return AppendNotDefaultNodeWithNamespace(root, nodeName, root.NamespaceURI, nodeValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotDefaultNodeWithNamespace<T>(XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            T defaultValue = default(T);

            if (nodeValue.Equals(defaultValue) == false)
                nodeText = nodeValue.ToString();

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

                if (nodeValue != null)
                    node.InnerText = nodeValue.ToString();
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlAttribute AppendAttrWithNamespace(XmlNode node, string attrName, string namespaceURI)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = node.OwnerDocument.CreateAttribute(attrName, namespaceURI);

            node.Attributes.SetNamedItem(attr);

            return attr;
        }

        /// <summary>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <param name="node">要添加属性的节点</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        /// <param name="namespaceURI"></param>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <returns>添加的属性对象</returns>
        /// <remarks>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，默认该属性的值为strValue指定。注意，该函数调用了
        /// AppendAttr(XmlDocument xmlDoc, XmlNode node, string attrName)来创建一个默认值为空的属性，然后再把属性内容修改成
        /// attrValue指定的内容
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="在xml文档对象的node指定节点上创建一个属性" />
        /// </remarks>
        public static XmlAttribute AppendAttrWithNamespace<T>(XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

            if (attrValue != null)
                attr.Value = attrValue.ToString();

            return attr;
        }

        /// <summary>
        /// 当attrValue值非空时(不是null或string.Empty)，为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="node">要添加属性的节点</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        /// <param name="namespaceURI"></param>
        /// <returns>添加的属性对象</returns>
        public static XmlAttribute AppendNotNullAttrWithNamespace<T>(XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = attrValue.ToString();

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// 如果attrValue不等于该类型的缺省值，则添加此属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlAttribute AppendNotDefaultAttrWithNamespace<T>(XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            T defaultValue = default(T);

            if (attrValue.Equals(defaultValue) == false)
                attrText = attrValue.ToString();

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="root">文档对象中的指定要添加子节点的根节点</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <returns>被添加的子节点对象</returns>
        /// <remarks>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点。该子节点的名称由
        /// nodeName指定，它的内容为nodeValue。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendNodeTest" lang="cs" title="在xml文档对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        public static XmlNode AppendNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = AppendNode(root, nodeName);

            if (nodeValue != null)
                node.InnerText = nodeValue.ToString();

            return node;
        }

        /// <summary>
        /// 如果nodeValue不为空(null或string.Empty)，在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="root">文档对象中的指定要添加子节点的根节点</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <returns>被添加的子节点对象</returns>
        public static XmlNode AppendNotNullNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = nodeValue.ToString();

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNode(root, nodeName);

                if (nodeValue != null)
                    node.InnerText = nodeValue.ToString();
            }

            return node;
        }

        /// <summary>
        /// 在XmlWriter对象中添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="writer">XmlWriter对象</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <remarks>在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点。该子节点的名称由
        /// nodeName指定，它的内容为nodeValue。
        /// /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendNodeTest" lang="cs" title="在Xml Writer对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        public static void AppendNode<T>(XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = nodeValue.ToString();

            writer.WriteElementString(nodeName, nodeText);
        }

        /// <summary>
        /// 如果nodeValue不为空(null或string.Empty)，在XmlWriter对象中添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="writer">XmlWriter对象</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        public static void AppendNotNullNode<T>(XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = nodeValue.ToString();

            if (string.IsNullOrEmpty(nodeText) == false)
                writer.WriteElementString(nodeName, nodeText);
        }

        /// <summary>
        /// 在XmlWriter对象中添加一个名称为nodeName指定的子节点，其中该节点包含在CData中，内容为nodeValue
        /// </summary>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <param name="writer">XmlWriter对象</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <remarks>在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点。该子节点的名称由
        /// nodeName指定，其中该节点包含在CData中，内容为nodeValue。
        /// /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendNodeTest" lang="cs" title="在Xml Writer对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        public static void AppendCDataNode<T>(XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = nodeValue.ToString();

            writer.WriteStartElement(nodeName);
            writer.WriteCData(nodeText);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue指定。
        /// </summary>
        /// <param name="root">文档对象中的指定要添加子节点的根节点</param>
        /// <param name="nodeName">要被添加的子节点名称</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <returns>被添加的子节点对象</returns>
        /// <remarks>在xml文档对象中由root指定的节点下面添加一个名称为nodeName指定的子节点，其中该节点的内容为nodeValue指定。但是节点的内容会被CData Section包围起来
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendCDataNodeTest" lang="cs" title="在xml文档对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// XmlDocument xmlDoc = CreateDomDocument("<Document/>");
        /// 
        /// XmlNode root = xmlDoc.DocumentElement;
        /// 
        /// XmlNode node = AppendCDataNode(root, "Data", "  Hello world !");
        ///
        /// 结果为：
        /// <Document>
        ///		<Data>
        ///			<![CDATA[  Hello world !]]/>
        ///		</Data>
        ///	</Document>
        /// ]]>
        /// </code>
        /// </example>
        public static XmlNode AppendCDataNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, root.NamespaceURI);

            root.AppendChild(node);

            if (nodeValue != null)
            {
                XmlCDataSection cdata = root.OwnerDocument.CreateCDataSection(nodeValue.ToString());

                node.AppendChild(cdata);
            }

            return node;
        }

        /// <summary>
        /// 在XML文档对象xmlDoc中的root节点下添加一个名称由nodeName指定内容由nodeValue指定的节点。如果在root指定
        /// 的根节点下已经存在有节点名称为nodeName，那我们就仅仅在它的内容为空的时候把值修改为nodeValue
        /// </summary>
        /// <typeparam name="T">nodeValue的类型</typeparam>
        /// <param name="root">文档对象中的指定要添加子节点的根节点</param>
        /// <param name="nodeName">要被添加的子节点名称（或xPath方式）</param>
        /// <param name="nodeValue">要被添加的子节点的内容</param>
        /// <returns>被添加的子节点对象</returns>
        /// <remarks>
        /// 在XML文档对象xmlDoc中的root节点下添加一个名称由nodeName指定内容由nodeValue指定的节点。如果在root指定
        /// 的根节点下已经存在有节点名称为nodeName，那我们就仅仅在它的内容为空的时候把值修改为nodeValue。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "AppendNotExistsNodeTest" lang="cs" title="在xml文档对象中在指定的节点下面添加指定的子节点" />
        /// </remarks>
        /// <example>
        /// 下面我们可以看一下该函数的处理（原数据与结果数据的对比，这里我们针对节点FOURTH, nodeValue设定为“1234”，
        /// 根节点设定为SECOND指定节点）：
        /// <table align="center">
        ///		<tr>
        ///			<td align="center">XML文档对象原数据</td>
        ///			<td align="center">XML文档对象新数据</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;1234&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH/&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;1234&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;6789&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;6789&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        /// </table>
        /// </example>
        public static XmlNode AppendNotExistsNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.SelectSingleNode(nodeName);

            if (node == null)
                node = AppendNode(root, nodeName);

            if (nodeValue != null && string.IsNullOrEmpty(node.InnerText))
                node.InnerText = nodeValue.ToString();

            return node;
        }

        /// <summary>
        /// 在XML文档对象xmlDoc中的指定根节点root下，如果有名称为nodeName的节点就把该节点的内容修改为nodeValue指定的内容。
        /// 否则就在root下创建一个名称为nodeName内容为nodeValue指定的新节点
        /// </summary>
        /// <param name="root">被替换节点的的根节点</param>
        /// <param name="nodeName">被替换子节点的名称(或xpath)</param>
        /// <param name="nodeValue">被替换子节点的内容</param>
        /// <typeparam name="T">节点值的类型</typeparam>
        /// <returns>被替换节点的节点对象</returns>
        /// <remarks>ReplaceNodeTest
        /// 在XML文档对象xmlDoc中的指定根节点root下，如果有名称为nodeName的节点就把该节点的内容修改为nodeValue指定的内容。
        /// 否则就在root下创建一个名称为nodeName内容为nodeValue指定的新节点
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "ReplaceNodeTest" lang="cs" title="在xml文档对象中添加一个节点，若该节点在xml文档对象中存在，则把该节点的值替换为nodeValue" />
        /// </remarks>
        public static XmlNode ReplaceNode<T>(XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.SelectSingleNode(nodeName);

            if (node == null)
                node = AppendNode<T>(root, nodeName, nodeValue);
            else
                node.InnerText = nodeValue.ToString();

            return node;
        }

        /// <summary>
        /// 在XML文档对象xmlDoc中的指定根节点root下，如果有名称为nodeName的节点就把该节点的内容修改为nodeValue指定的内容。
        /// 否则就在root下创建一个名称为nodeName内容为nodeValue指定的新节点
        /// </summary>
        /// <param name="root">被替换节点的的根节点</param>
        /// <param name="nodeName">被替换子节点的名称(或xpath)</param>
        /// <returns>被替换节点的节点对象</returns>
        /// <remarks> 
        /// 在XML文档对象xmlDoc中的指定根节点root下，如果有名称为nodeName的节点就把该节点的内容修改为nodeValue指定的内容。
        /// 否则就在root下创建一个名称为nodeName内容为nodeValue指定的新节点
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "ReplaceNodeTest" lang="cs" title="在xml文档对象中添加一个节点，若该节点在xml文档对象中存在，则把该节点的值替换为nodeValue" />
        /// </remarks>
        public static XmlNode ReplaceNode(XmlNode root, string nodeName)
        {
            return ReplaceNode(root, nodeName, string.Empty);
        }

        /// <summary>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，默认该属性的值为空
        /// </summary>
        /// <param name="node">要添加属性的节点</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <returns>添加的属性对象（此时属性的内容还是空的）</returns>
        /// <remarks>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，默认该属性的值为空。如果您需要添加具体的属性内容
        /// 可以采用该函数的重载函数AppendAttr(XmlNode node, string attrName, string strValue)
        ///  <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="为xml文档对象的node指定节点上创建一个属性" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = XMLHelper.CreateDomDocument("&lt;FIRST&gt;&lt;SECOND&gt;&lt;THIRD/&gt;&lt;/SECOND&gt;&lt;/FIRST&gt;");
        /// XmlAttribute attr = XMLHelperr.AppendAttr(xmlDoc.DocumentElement.FirstChild, "WEIGHT");
        /// attr.Value = "65kg";
        /// </code>
        /// </example>
        public static XmlAttribute AppendAttr(XmlNode node, string attrName)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = node.OwnerDocument.CreateAttribute(attrName, node.NamespaceURI);

            node.Attributes.SetNamedItem(attr);

            return attr;
        }

        /// <summary>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <param name="node">要添加属性的节点</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <returns>添加的属性对象</returns>
        /// <remarks>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，默认该属性的值为strValue指定。注意，该函数调用了
        /// AppendAttr(XmlDocument xmlDoc, XmlNode node, string attrName)来创建一个默认值为空的属性，然后再把属性内容修改成
        /// attrValue指定的内容
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="在xml文档对象的node指定节点上创建一个属性" />
        /// </remarks>
        public static XmlAttribute AppendAttr<T>(XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = AppendAttr(node, attrName);

            if (attrValue != null)
                attr.Value = attrValue.ToString();

            return attr;
        }

        /// <summary>
        /// 当attrValue值非空时(不是null或string.Empty)，为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="node">要添加属性的节点</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        /// <returns>添加的属性对象</returns>
        public static XmlAttribute AppendNotNullAttr<T>(XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = attrValue.ToString();

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttr(node, attrName);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// 如果attrValue不等于该类型的缺省值，则添加此属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static XmlAttribute AppendNotDefaultAttr<T>(XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            T defaultValue = default(T);

            if (attrValue.Equals(defaultValue) == false)
                attrText = attrValue.ToString();

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttr(node, attrName);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// 为Xml Writer创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="writer">XmlWriter</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        /// <remarks>
        /// 为xml文档对象的node指定节点上创建一个属性，属性名称为attrName，属性的值为attrValue。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendAttrTest" lang="cs" title="为Xml Writer在指定节点上创建一个属性" />
        /// </remarks>
        public static void AppendAttr<T>(XmlWriter writer, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = attrValue.ToString();

            writer.WriteAttributeString(attrName, attrText);
        }

        /// <summary>
        /// 当attrValue值非空时(不是null或string.Empty)，为Xml Writer创建一个属性，属性名称为attrName，属性的值为attrValue
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="writer">XmlWriter</param>
        /// <param name="attrName">要添加的属性名称</param>
        /// <param name="attrValue">要添加的属性内容</param>
        public static void AppendNotNullAttr<T>(XmlWriter writer, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = attrValue.ToString();

            if (string.IsNullOrEmpty(attrText) == false)
                writer.WriteAttributeString(attrName, attrText);
        }

        #endregion AppendNode和Attribute系列

        /// <summary>
        /// 对节点列表nodeList中的每一个节点进行nodeOP指定的操作，其中oParam是操作使用到的参数
        /// </summary>
        /// <param name="nodeList">节点列表</param>
        /// <param name="nodeOP">每次枚举一个节点所做的操作</param>
        /// <param name="oParam">每次操作所传入的参数</param>
        /// <remarks>
        /// 对节点列表nodeList中的每一个节点进行nodeOP指定的操作，其中oParam是操作使用到的参数
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="EnumNodeListTest" lang="cs" title="对节点列表nodeList中的每一个节点进行nodeOP指定的操作" />
        /// </remarks>
        /// <example>
        /// <code>
        /// public static void myMethod(XmlNode nodeRoot, object oParam)
        /// {
        /// }
        ///
        /// public static void Invoke(XmlNodeList nodeList)
        /// {
        /// 	XMLHelper.EnumNodeList(nodeList, new XMLHelper.DoNodeList(myMethod), new object());
        /// }
        /// </code>
        /// </example>
        public static void EnumNodeList(XmlNodeList nodeList, DoNodeList nodeOP, object oParam)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nodeList != null, "nodeList");

            if (nodeOP != null)
                foreach (XmlNode node in nodeList)
                    nodeOP(node, oParam);
        }

        #region Get Node或Attribute的值系列
        /// <summary>
        /// 查询一个节点，得到该节点的正文，如果该节点不存在，则返回空串
        /// </summary>
        /// <param name="nodeRoot">执行查询的根节点</param>
        /// <param name="path">查询的XPath</param>
        /// <returns>查询nodeRoot下，按照path查询出的节点的正文，如果nodeRoot为空，或节点不存在，则返回空串</returns>
        /// <remarks>查询一个节点，得到该节点的正文，如果该节点不存在，则返回空串
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetSingleNodeExceptionTest" lang="cs" title="查询一个节点，得到该节点的正文" />
        /// </remarks>
        public static string GetSingleNodeText(XmlNode nodeRoot, string path)
        {
            return GetSingleNodeValue<string>(nodeRoot, path, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <returns></returns>
        public static string GetSingleNodeText(XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr)
        {
            return GetSingleNodeValue<string>(nodeRoot, path, nsMgr, string.Empty);
        }

        /// <summary>
        /// 查询一个节点，得到该节点的正文，如果该节点不存在，则返回空串
        /// </summary>
        /// <param name="nodeRoot">执行查询的根节点</param>
        /// <param name="path">查询的XPath</param>
        /// <param name="defaultValue">如果节点不存在，返回的缺省值</param>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <returns>查询nodeRoot下，按照path查询出的节点的正文，如果nodeRoot为空，或节点或不存在，则返回缺省值</returns>
        /// <remarks>查询一个节点，得到该节点的正文，如果该节点不存在，则返回缺省值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetSingleNodeValueTest" lang="cs" title="查询一个节点，得到该节点的正文" />
        /// </remarks>
        public static T GetSingleNodeValue<T>(XmlNode nodeRoot, string path, T defaultValue)
        {
            return GetSingleNodeValue(nodeRoot, path, null, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSingleNodeValue<T>(XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr, T defaultValue)
        {
            T result = defaultValue;

            if (nodeRoot != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

                XmlNode node = null;

                if (nsMgr != null)
                    node = nodeRoot.SelectSingleNode(path, nsMgr);
                else
                    node = nodeRoot.SelectSingleNode(path);

                if (node != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(node.InnerText);
                    else
                        result = (T)DataConverter.ChangeType(node.InnerText, defaultValue.GetType());
                }
            }

            return result;
        }

        /// <summary>
        /// 查询一个节点，如果该节点不存在，则抛出异常
        /// </summary>
        /// <param name="nodeRoot">执行查询的根节点</param>
        /// <param name="path">查询的XPath</param>
        /// <returns>查询nodeRoot下，按照path查询出的节点，如果nodeRoot为空，或节点或不存在，则抛出异常</returns>
        /// <remarks>查询一个节点，如果该节点不存在，则抛出异常
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetSingleNodeExceptionTestWithException" lang="cs" title="查询一个节点，如果该节点不存在，则抛出异常" />
        /// </remarks>
        public static XmlNode GetSingleNodeException(XmlNode nodeRoot, string path)
        {
            return GetSingleNodeException(nodeRoot, path, string.Empty);
        }

        /// <summary>
        /// 查询一个节点，如果该节点不存在，则抛出异常
        /// </summary>
        /// <param name="nodeRoot">执行查询的根节点</param>
        /// <param name="path">查询的XPath</param>
        /// <param name="message">异常文本</param>
        /// <returns>查询nodeRoot下，按照path查询出的节点，如果nodeRoot为空，或节点或不存在，则抛出异常</returns>
        /// <remarks>查询一个节点，如果该节点不存在，则抛出异常
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetSingleNodeExceptionTestWithException" lang="cs" title="查询一个节点，如果该节点不存在，则抛出异常" />
        /// </remarks>
        public static XmlNode GetSingleNodeException(XmlNode nodeRoot, string path, string message)
        {
            return GetSingleNodeException(nodeRoot, path, null, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static XmlNode GetSingleNodeException(XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr, string message)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nodeRoot != null, "nodeRoot");
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            XmlNode node = null;

            if (nsMgr != null)
                node = nodeRoot.SelectSingleNode(path, nsMgr);
            else
                node = nodeRoot.SelectSingleNode(path);

            if (string.IsNullOrEmpty(message))
                message = string.Format(Resource.CanNotFindXmlNode, nodeRoot.Name, path);

            ExceptionHelper.FalseThrow<ArgumentException>(node != null, message);

            return node;
        }

        /// <summary>
        /// 从XmlReader中，得到节点的字符串值
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点的字符串值</returns>
        /// <remarks>从XmlReader中，得到节点的字符串值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetNodeValueTest" lang="cs" title="在XmlReader中得到当前节点的字符串值" />
        /// </remarks>
        public static string GetNodeText(XmlReader reader, string nodeName)
        {
            return GetNodeValue(reader, nodeName, string.Empty);
        }

        /// <summary>
        /// 从XmlReader中，得到节点的值
        /// </summary>
        /// <typeparam name="T">节点的值类型</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="nodeName">节点的名称</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>节点的值</returns>
        /// <remarks>从XmlReader中，得到节点的值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetNodeValueTest" lang="cs" title="在XmlReader中得到当前节点的值" />
        /// </remarks>
        public static T GetNodeValue<T>(XmlReader reader, string nodeName, T defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, nodeName);

            T result = defaultValue;

            if (reader.IsEmptyElement == false)
            {
                try
                {
                    reader.ReadStartElement(nodeName);

                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(reader.ReadString());
                    else
                        result = (T)DataConverter.ChangeType<string>(reader.ReadString(), defaultValue.GetType());

                    reader.ReadEndElement();
                }
                catch (System.Xml.XmlException)
                {
                }
            }
            else
            {
                try
                {
                    reader.ReadStartElement(nodeName);
                }
                catch (System.Xml.XmlException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// 在XmlReader中得到当前节点的属性字符串值
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="attrName">属性名称</param>
        /// <returns>当前节点的属性字符串值</returns>
        /// <remarks>在XmlReader中得到当前节点的属性字符串值，如果属性不存在，则返回空串。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetAttrValueTest" lang="cs" title="在XmlReader中得到当前节点的属性字符串值" />
        /// </remarks>
        public static string GetAttributeText(XmlReader reader, string attrName)
        {
            return GetAttributeValue(reader, attrName, string.Empty);
        }

        /// <summary>
        /// 在XmlReader中得到当前节点的属性值
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="attrName">属性名称</param>
        /// <param name="defaultValue">如果属性不存在，缺省值</param>
        /// <returns>当前节点的属性值</returns>
        /// <remarks>在XmlReader中得到当前节点的属性值，如果属性不存在，则返回缺省值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetAttrValueTest" lang="cs" title="在XmlReader中得到当前节点的属性值" />
        /// </remarks>
        public static T GetAttributeValue<T>(XmlReader reader, string attrName, T defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            T result = defaultValue;

            string attrText = reader.GetAttribute(attrName);

            if (attrText != null)
            {
                if (defaultValue == null)
                    result = (T)DataConverter.ChangeType<string, T>(attrText);
                else
                    result = (T)DataConverter.ChangeType(attrText, defaultValue.GetType());
            }

            return result;
        }

        /// <summary>
        /// 获得一个节点的属性的值，如果该属性不存在，则返回空串
        /// </summary>
        /// <param name="nodeRoot">执行查询的节点</param>
        /// <param name="attrName">属性名称</param>
        /// <returns>nodeRoot中某个属性的值，如果该属性不存在或nodeRoot为空，则返回空串</returns>
        /// <remarks>获得一个属性的值，如果该属性不存在，则返回空串
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetAttributeValueTest" lang="cs" title="获得一个节点的属性的值" />
        /// </remarks>
        public static string GetAttributeText(XmlNode nodeRoot, string attrName)
        {
            return GetAttributeValue<string>(nodeRoot, attrName, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static string GetAttributeText(XmlNode nodeRoot, string attrName, string namespaceURI)
        {
            return GetAttributeValue<string>(nodeRoot, attrName, namespaceURI, string.Empty);
        }

        /// <summary>
        /// 获得一个节点的属性的值，如果该属性不存在，则返回缺省值
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="nodeRoot">执行查询的节点</param>
        /// <param name="attrName">属性名称</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>nodeRoot中某个属性的值，如果该属性不存在或nodeRoot为空，则返回空串</returns>
        /// <remarks>获得一个节点的属性的值，如果该属性不存在，则返回缺省值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "GetAttributeValueTest" lang="cs" title="获得一个节点的属性的值" />
        /// </remarks>
        public static T GetAttributeValue<T>(XmlNode nodeRoot, string attrName, T defaultValue)
        {
            return GetAttributeValue(nodeRoot, attrName, nodeRoot.NamespaceURI, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeRoot"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(XmlNode nodeRoot, string attrName, string namespaceURI, T defaultValue)
        {
            T result = defaultValue;

            if (nodeRoot != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

                XmlNode attr = nodeRoot.Attributes.GetNamedItem(attrName, namespaceURI);

                if (attr != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(attr.Value);
                    else
                        result = (T)DataConverter.ChangeType(attr.Value, defaultValue.GetType());
                }
            }

            return result;
        }
        #endregion  Get Node或Attribute的值系列

        #region Xml和对象之间的转换

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        /// <returns></returns>
        public static XmlDocument SerializeObjectToXml<T>(T graph, params string[] ignorProperties)
        {
            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            XmlDocument xmlDoc = CreateDomDocument(string.Format("<{0}/>", mappings.RootName));

            SerializePropertiesToNodes(xmlDoc.DocumentElement, mappings, graph, ignorProperties);

            return xmlDoc;
        }

        public static XmlNode SerializeObjectToXmlNode<T>(T graph, params string[] ignorProperties)
        {
            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            XmlDocument xmlDoc = CreateDomDocument(string.Format("<{0}/>", mappings.RootName));

            SerializePropertiesToNodes(xmlDoc.DocumentElement, mappings, graph, ignorProperties);

            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlDoc"></param>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        public static void DeserializeToObject<T>(XmlDocument xmlDoc, T graph, params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            DeserializeNodesToProperties(xmlDoc.DocumentElement, mappings, graph, ignorProperties);
        }

        public static void DeserializeToObject<T>(XmlNode xmlNode, T graph, params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            DeserializeNodesToProperties(xmlNode, mappings, graph, ignorProperties);
        }

        private static object ConvertData(XmlObjectMappingItem item, object data)
        {
            try
            {
                System.Type realType = XmlObjectMapping.GetRealType(item.MemberInfo);

                return DataConverter.ChangeType(data, realType);
            }
            catch (System.Exception ex)
            {
                throw new SystemSupportException(
                    string.Format(Resource.ConvertXmlNodeToPropertyError,
                        item.NodeName, item.PropertyName, ex.Message),
                        ex
                    );
            }
        }

        private static void DeserializeNodesToObject<T>(XmlNode currentNode, T graph)
        {
            XmlObjectMappingItemCollection subMappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            DeserializeNodesToProperties(currentNode, subMappings, graph);
        }

        private static void DeserializeNodesToProperties<T>(XmlNode parentNode, XmlObjectMappingItemCollection mappings, T graph, params string[] ignorProperties)
        {
            foreach (XmlObjectMappingItem item in mappings)
            {
                if (Array.Exists<string>(ignorProperties,
                    target => (string.Compare(target, item.PropertyName, true) == 0)) == false)
                {
                    System.Type realType = XmlObjectMapping.GetRealType(item.MemberInfo);

                    object data = null;

                    XmlElement subclassNode = (XmlElement)parentNode.SelectSingleNode(string.Format("Subclass[@propertyName=\"{0}\"]", item.PropertyName));

                    if (subclassNode != null)
                    {
                        data = DeserializeSubClassNodeToObject(subclassNode, item, graph);

                        SetValueToObject(item, graph, data);
                    }
                    else
                    {
                        subclassNode = (XmlElement)parentNode.SelectSingleNode(string.Format("Subclass[@propertyName=\"{0}\"]", item.PropertyName));
                    }

                    if (subclassNode != null)
                    {

                    }
                    else
                    {
                        if (item.MappingType == XmlNodeMappingType.Attribute)
                            data = XmlHelper.GetAttributeValue(parentNode, item.NodeName, TypeCreator.GetTypeDefaultValue(realType));
                        else
                            data = XmlHelper.GetSingleNodeValue(parentNode, item.NodeName, TypeCreator.GetTypeDefaultValue(realType));

                        if (Convertible(realType, data))
                            SetValueToObject(item, graph, ConvertData(item, data));
                    }
                }
            }
        }

        private delegate void DeserializeCollectionAction(int index, object itemDate);

        private static object DeserializeSubClassNodeToObject<T>(XmlNode subclassNode, XmlObjectMappingItem item, T graph)
        {
            string subClassTypeName = XmlHelper.GetAttributeText(subclassNode, "subClassTypeName");

            object data = GetValueFromObject(item, graph);

            bool isArray = XmlHelper.GetAttributeValue(subclassNode, "isArray", false);
            XmlNodeList valueNodes = subclassNode.SelectNodes("Items/Item");

            if (isArray)
                data = TypeCreator.CreateInstance(subClassTypeName,
                        valueNodes.Count);
            else
                if (data == null)
                    data = TypeCreator.CreateInstance(subClassTypeName);

            if (isArray)
            {
                DeserializeNodeToCollection(valueNodes, (i, itemData) => ((Array)data).SetValue(itemData, i));
            }
            else
            {
                if (data is IList)
                {
                    IList list = (IList)data;

                    list.Clear();

                    DeserializeNodeToCollection(valueNodes, (i, itemData) => list.Add(itemData));
                }
                else
                    DeserializeNodesToObject(subclassNode, data);
            }

            return data;
        }

        private static void DeserializeNodeToCollection(XmlNodeList valueNodes, DeserializeCollectionAction action)
        {
            for (int i = 0; i < valueNodes.Count; i++)
            {
                System.Type elementType = TypeCreator.GetTypeInfo(XmlHelper.GetAttributeText(valueNodes[i], "type"));

                XmlElement subclassNode = (XmlElement)valueNodes[i].SelectSingleNode("Subclass");

                object itemData = null;

                if (Type.GetTypeCode(elementType) == TypeCode.Object)
                {
                    itemData = TypeCreator.CreateInstance(elementType);
                    DeserializeNodesToObject(valueNodes[i], itemData);
                }
                else
                    itemData = DataConverter.ChangeType(XmlHelper.GetAttributeText(valueNodes[i], "value"), elementType);

                action(i, itemData);
            }
        }

        private static void SetMemberValueToObject(MemberInfo mi, object graph, object data)
        {
            data = DecorateDate(data);

            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)mi;
                    if (pi.CanWrite)
                        pi.SetValue(graph, data, null);
                    break;
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)mi;
                    fi.SetValue(graph, data);
                    break;
                default:
                    XmlObjectMapping.ThrowInvalidMemberInfoTypeException(mi);
                    break;
            }
        }

        /// <summary>
        /// 对数据进行最后的修饰，例如对日期类型的属性加工
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static object DecorateDate(object data)
        {
            object result = data;

            if (data is DateTime)
            {
                DateTime dt = (DateTime)data;

                if (dt.Kind == DateTimeKind.Unspecified)
                    result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }

            return result;
        }

        private static void SetValueToObject(XmlObjectMappingItem item, object graph, object data)
        {
            if (string.IsNullOrEmpty(item.SubClassPropertyName))
                SetMemberValueToObject(item.MemberInfo, graph, data);
            else
            {
                if (graph != null)
                {
                    MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (mi == null)
                        mi = graph.GetType().GetField(item.PropertyName,
                            BindingFlags.Instance | BindingFlags.Public);

                    if (mi != null)
                    {
                        object subGraph = GetMemberValueFromObject(mi, graph);

                        if (subGraph == null)
                        {
                            if (string.IsNullOrEmpty(item.SubClassTypeDescription) == false)
                                subGraph = TypeCreator.CreateInstance(item.SubClassTypeDescription);
                            else
                                subGraph = Activator.CreateInstance(XmlObjectMapping.GetRealType(mi), true);

                            SetMemberValueToObject(mi, graph, subGraph);
                        }

                        SetMemberValueToObject(item.MemberInfo, subGraph, data);
                    }
                }
            }
        }

        private static bool Convertible(System.Type targetType, object data)
        {
            bool result = true;

            if (data == null && targetType.IsValueType)
                result = false;
            else
            {
                if (data == DBNull.Value)
                {
                    if (targetType != typeof(DBNull) && targetType != typeof(string))
                        result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentNode"></param>
        /// <param name="item"></param>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        /// <returns></returns>
        private static XmlNode SerializeObjectToNode<T>(XmlNode parentNode, XmlObjectMappingItem item, T graph, params string[] ignorProperties)
        {
            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            XmlNode node = XmlHelper.AppendNode(parentNode, "Subclass");

            System.Type subClassType = graph.GetType();
            XmlHelper.AppendAttr(node, "subClassTypeName", subClassType.AssemblyQualifiedName);
            XmlHelper.AppendAttr(node, "propertyName", item.PropertyName);

            if (subClassType.IsArray)
                XmlHelper.AppendAttr(node, "isArray", true);

            SerializePropertiesToNodes(node, mappings, graph, ignorProperties);

            if (graph is IEnumerable)
                SerializeIEnumerableObject(node, (IEnumerable)graph);

            return node;
        }

        private static void SerializeIEnumerableObject(XmlNode parentNode, IEnumerable graph)
        {
            XmlNode itemsNode = XmlHelper.AppendNode(parentNode, "Items");

            foreach (object data in graph)
            {
                XmlNode itemNode = XmlHelper.AppendNode(itemsNode, "Item");

                if (data != null)
                {
                    XmlHelper.AppendAttr(itemNode, "type", data.GetType().AssemblyQualifiedName);
                    if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
                    {
                        XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(data);
                        SerializePropertiesToNodes(itemNode, mappings, data);
                    }
                    else
                        XmlHelper.AppendAttr(itemNode, "value", data);
                }
            }
        }

        private static void SerializePropertiesToNodes<T>(XmlNode parentNode, XmlObjectMappingItemCollection mappings, T graph, params string[] ignorProperties)
        {
            foreach (XmlObjectMappingItem item in mappings)
            {
                if (Array.Exists<string>(ignorProperties,
                    target => (string.Compare(target, item.PropertyName, true) == 0)) == false)
                {
                    object data = GetValueFromObject(item, graph);

                    if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) == false)
                    {
                        if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
                        {
                            //子对象
                            SerializeObjectToNode(parentNode, item, data);
                        }
                        else
                        {
                            if (item.MappingType == XmlNodeMappingType.Attribute)
                                XmlHelper.AppendAttr(parentNode, item.NodeName, data);
                            else
                                XmlHelper.AppendNode(parentNode, item.NodeName, data);
                        }
                    }
                }
            }
        }

        private static object GetValueFromObject(XmlObjectMappingItem item, object graph)
        {
            object data = null;

            if (string.IsNullOrEmpty(item.SubClassPropertyName))
                data = GetValueFromObjectDirectly(item, graph);
            else
            {
                if (graph != null)
                {
                    MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (mi == null)
                        mi = graph.GetType().GetField(item.PropertyName,
                            BindingFlags.Instance | BindingFlags.Public);

                    if (mi != null)
                    {
                        object subGraph = GetMemberValueFromObject(mi, graph);

                        if (subGraph != null)
                            data = GetValueFromObjectDirectly(item, subGraph);
                    }
                }
            }

            return data;
        }

        private static object GetValueFromObjectDirectly(XmlObjectMappingItem item, object graph)
        {
            object data = GetMemberValueFromObject(item.MemberInfo, graph);

            if (data != null)
            {
                System.Type dataType = data.GetType();
                if (dataType.IsEnum)
                {
                    data = data.ToString();
                }
                else
                    if (dataType == typeof(TimeSpan))
                        data = ((TimeSpan)data).TotalSeconds;
            }

            return data;
        }

        private static object GetMemberValueFromObject(MemberInfo mi, object graph)
        {
            object data = null;

            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)mi;
                    if (pi.CanRead)
                        data = pi.GetValue(graph, null);
                    break;
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)mi;
                    data = fi.GetValue(graph);
                    break;
                default:
                    XmlObjectMapping.ThrowInvalidMemberInfoTypeException(mi);
                    break;
            }

            return data;
        }
        #endregion Xml和对象之间的转换
    }

    public static class XElementHelper
    {
        /// <summary>
        /// 获得元素下儿子元素的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetElementValue<T>(this XElement element, string elementName, T defaultValue)
        {
            if (element != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(elementName, "elementName");

                XElement item = element.Element(elementName);
                if (item == null)
                {
                    return defaultValue;
                }
                try
                {
                    T result = (T)DataConverter.ChangeType<string, T>(item.Value);
                    return result;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获得元素属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="attrName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(this XElement element, string attrName, T defaultValue)
        {
            if (element != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

                XAttribute attr = element.Attribute(attrName);
                if (attr == null)
                {
                    return defaultValue;
                }
                try
                {
                    T result = (T)DataConverter.ChangeType<string, T>(attr.Value);
                    return result;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 创建唯一的xml子元素， 即使之前子元素有多个， 也会被覆盖
        /// </summary>
        /// <param name="container"></param>
        /// <param name="elementName"></param>
        /// <param name="elementText"></param>
        /// <returns></returns>
        public static XElement CreateUniqueSubElement(this XElement container, string elementName, object elementText)
        {
            if (container == null)
            {
                throw new ApplicationException("Xml节点容器为空！");
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentNullException("elementName");
            }

            container.Elements(elementName).Remove();
            container.SetElementValue(elementName, elementText == null ? null : elementText.ToString());

            return container.Element(elementName);
        }

        /// <summary>
        /// 创建子xml元素， 如果id相同的Element元素已存在， 则更新。 如果id
        /// </summary>
        /// <param name="container"></param>
        /// <param name="elementName"></param>
        /// <param name="elementText"></param>
        /// <param name="id">大于0才有效；负数和0无效，但仍可以创建一个无id的节点</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">
        /// 如果id相同的元素下有子元素， 则抛出异常
        /// </exception>
        public static XElement CreateSubElement(this XElement container, string elementName, object elementText, int id = 0)
        {
            if (container == null)
            {
                throw new ApplicationException("Xml节点容器为空！");
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentNullException("elementName");
            }


            if (id > 0)
            {
                XElement existOne = (from item in container.Elements(elementName)
                                     where item.GetAttributeValue<int>("ID", 0) == id
                                     select item)
                             .FirstOrDefault();

                if (existOne != null)
                {
                    if (existOne.HasElements)
                    {
                        throw new ApplicationException("Element下还有子Element， 无法添加文本");
                    }
                    else
                    {
                        existOne.SetValue(elementText == null ? null : elementText.ToString());
                        return existOne;
                    }
                }


            }

            XElement result = new XElement(elementName);
            if (id > 0)
            {
                result.CreateAttribute("ID", id.ToString());
            }
            result.SetValue(elementText == null ? null : elementText.ToString());
            container.Add(result);
            return result;
        }

        /// <summary>
        /// 获得相同ElementName的最大ID
        /// </summary>
        /// <param name="container"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static int GetMaxElementID(this XElement container, string elementName)
        {
            if (container == null)
            {
                throw new ApplicationException("Xml节点容器为空！");
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentNullException("elementName");
            }

            int max = (from item in container.Elements(elementName)
                       select item.GetAttributeValue<int>("ID", 0)
                                 ).Max();
            return max;

        }

        /// <summary>
        /// 创建/更新元素属性
        /// </summary>
        /// <param name="container"></param>
        /// <param name="elementName"></param>
        /// <param name="elementText"></param>
        /// <returns></returns>
        public static XAttribute CreateAttribute(this XElement container, string attrName, object text)
        {

            if (container == null)
            {
                throw new ApplicationException("Xml节点容器为空！");
            }

            if (string.IsNullOrEmpty(attrName))
            {
                throw new ArgumentNullException("attrName");
            }


            //container.SetAttributeValue(attrName, text.Replace("\r", "").Replace("\n", ""));
            container.SetAttributeValue(attrName, text == null ? null : text.ToString());
            return container.Attribute(attrName);
        }


    }
}