using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace Lib.Core
{
    /// <summary>
    /// 处理应用环境问题的类
    /// </summary>
    /// <remarks>处理应用环境问题的类
    /// </remarks>
    public static class EnvironmentHelper
    {
        #region Private field

        private static string shortDomainName;
        private static string domainDnsName;
        #endregion

        #region Constructor
        /// <summary>
        /// EnvironmentHelper的构造函数
        /// </summary>
        /// <remarks>EnvironmentHelper的构造函数,该构造函数不带任何参数。
        /// </remarks>
        static EnvironmentHelper()
        {
            EnvironmentHelper.shortDomainName = GetShortDomainName();
            EnvironmentHelper.domainDnsName = GetDomainDnsName();
        }
        #endregion

        #region InterOP
        private enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax,
        }

        private enum EXTENDED_NAME_FORMAT
        {
            NameUnknown = 0,
            NameFullyQualifiedDN = 1,
            NameSamCompatible = 2,
            NameDisplay = 3,
            NameUniqueId = 6,
            NameCanonical = 7,
            NameUserPrincipal = 8,
            NameCanonicalEx = 9,
            NameServicePrincipal = 10,
            NameDnsDomain = 12,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, StringBuilder nameBuffer, ref int bufferSize);

        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat, StringBuilder nameBuffer, ref int bufferSize);

        /*
        ComputerNameDnsDomain:			oa.hgzs.ain.cn
        ComputerNameDnsFullyQualified:	mcs-shenzheng.oa.hgzs.ain.cn
        ComputerNameDnsHostname:		mcs-shenzheng
        ComputerNameNetBIOS:			MCS-SHENZHENG
        ComputerNamePhysicalDnsDomain:	oa.hgzs.ain.cn
        ComputerNamePhysicalDnsFullyQualified:
                                        mcs-shenzheng.oa.hgzs.ain.cn
        ComputerNamePhysicalDnsHostname:
                                        mcs-shenzheng
        ComputerNamePhysicalNetBIOS:	MCS-SHENZHENG

        NameCanonical:			oa.hgzs.ain.cn/Computers/MCS-SHENZHENG
        NameCanonicalEx:		oa.hgzs.ain.cn/Computers
        MCS-SHENZHENG
        NameDisplay:			MCS-SHENZHENG$
        NameDnsDomain:		
        NameFullyQualifiedDN:	CN=MCS-SHENZHENG,CN=Computers,DC=oa,DC=hgzs,DC=ain,DC=cn
        NameSamCompatible:		OA\MCS-SHENZHENG$
        NameServicePrincipal:		
        NameUniqueId:			{848ed57e-afd5-49e9-8d9c-cdb12bd6cf9a}
        NameUnknown:		
        NameUserPrincipal:
        */
        #endregion InterOP

        #region Private helper method
        private static bool CheckIsWebApplication()
        {
            bool isWebApp = false;

            AppDomain domain = AppDomain.CurrentDomain;
            try
            {
                if (domain.ShadowCopyFiles)
                    isWebApp = (HttpContext.Current != null);
            }
            catch (System.Exception)
            {
            }

            return isWebApp;
        }

        private static string GetShortDomainName()
        {
            string machineName = InnerGetComputerObjectName(EXTENDED_NAME_FORMAT.NameSamCompatible);

            string[] nameParts = machineName.Split('\\', '/');

            return nameParts[0];
        }

        private static string GetDomainDnsName()
        {
            return InnerGetComputerName(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsDomain);
        }

        private static string InnerGetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerObjectName(nameFormat, strB, ref nSize);

            return strB.ToString();
        }

        private static string InnerGetComputerName(COMPUTER_NAME_FORMAT nameType)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerNameEx(nameType, strB, ref nSize);

            return strB.ToString();
        }
        #endregion


        /// <summary>
        /// 当前应用是否为web应用的属性 ( Windows / Web)
        /// </summary>
        /// <remarks>该属性是只读的。
        /// <seealso cref="Lib.Configuration.ConfigurationBroker"/>
        /// <seealso cref="Lib.Configuration.MetaConfigurationSourceInstanceElement"/>
        /// <seealso cref="Lib.Configuration.MetaConfigurationSourceMappingElement"/>
        /// </remarks>
        public static InstanceMode Mode
        {
            get
            {
                if (EnvironmentHelper.CheckIsWebApplication())
                    return InstanceMode.Web;
                else
                    return InstanceMode.Windows;
            }
        }

        /// <summary>
        /// 如果机器在域上注册，返回短域名
        /// </summary>
        /// <remarks>改属性只读。如果机器在域上的短名称是oa\hb2004-db，那么ShortDomainName就是oa。如果没有加入域，则返回空串</remarks>
        public static string ShortDomainName
        {
            get
            {
                return EnvironmentHelper.shortDomainName;
            }
        }

        /// <summary>
        /// 如果机器在域上注册，返回长域名
        /// </summary>
        /// <remarks>改属性只读。域的长名称如oa.hgzs.ain.cn。如果没有加入域，则返回空串</remarks>
        public static string DomainDnsName
        {
            get
            {
                return EnvironmentHelper.domainDnsName;
            }
        }
    }
}
