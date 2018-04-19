using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// employeeID
        /// </summary>		
        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }private string _UserID = string.Empty;

        /// <summary>
        /// 用户Code
        /// employeeCode
        /// </summary>		
        public string UserCode
        {
            get
            {
                return _UserCode;
            }
            set
            {
                _UserCode = value;
            }
        }private string _UserCode = string.Empty;

        /// <summary>
        /// 用户姓名
        /// employeeName
        /// </summary>		
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }private string _UserName = string.Empty;

        /// <summary>
        /// 用户登录ID
        /// username
        /// </summary>		
        public string UserLoginID
        {
            get
            {
                return _UserLoginID;
            }
            set
            {
                _UserLoginID = value;
            }
        }private string _UserLoginID = string.Empty;

        /// <summary>
        /// 用户职位名称
        /// jobName
        /// </summary>		
        public string UserJobName
        {
            get
            {
                return _UserJobName;
            }
            set
            {
                _UserJobName = value;
            }
        }private string _UserJobName = string.Empty;

        /// <summary>
        /// 用户所在路径ID
        /// unitFullPath
        /// </summary>		
        public string UserOrgPathID
        {
            get
            {
                return _UserOrgPathID;
            }
            set
            {
                _UserOrgPathID = value;
            }
        }private string _UserOrgPathID = string.Empty;

        /// <summary>
        /// 用户所在路径名称
        /// unitName
        /// </summary>		
        public string UserOrgPathName
        {
            get
            {
                return _UserOrgPathName;
            }
            set
            {
                _UserOrgPathName = value;
            }
        }private string _UserOrgPathName = string.Empty;

        /// <summary>
        /// 用户所在组织机构ID
        /// orgID
        /// </summary>		
        public string UserOrgID
        {
            get
            {
                return _UserOrgID;
            }
            set
            {
                _UserOrgID = value;
            }
        }private string _UserOrgID = string.Empty;
    }
}
