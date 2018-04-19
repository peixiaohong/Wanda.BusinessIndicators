using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 流程对象基类
    /// </summary>
    public class BaseProcessObject
    {
        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtensionInfo
        {
            get
            {
                return _ExtensionInfo;
            }
            set
            {
                _ExtensionInfo = value;
            }
        }private string _ExtensionInfo = string.Empty;
    }
}
