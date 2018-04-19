using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 流程按钮定义
    /// </summary>
    public class ActionButton
    {
        /// <summary>
        /// 按钮类型
        /// </summary>
        public int ButtonType
        {
            get
            {
                return _ButtonType;
            }
            set
            {
                _ButtonType = value;
            }
        }private int _ButtonType;
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string ButtonName
        {
            get
            {
                return _ButtonName;
            }
            set
            {
                _ButtonName = value;
            }
        }private string _ButtonName = string.Empty;
        /// <summary>
        /// 按钮自定义显示名称
        /// 在客户端审批页面显示的内容
        /// </summary>
        public string ButtonDisplayName
        {
            get
            {
                return _ButtonDisplayName;
            }
            set
            {
                _ButtonDisplayName = value;
            }
        }private string _ButtonDisplayName = string.Empty;

        /// <summary>
        /// 按钮提交时的方法名
        /// </summary>
        public string ButtonMethodName
        {
            get { return _ButtonMethodName; }
            set { _ButtonMethodName = value; }
        }private string _ButtonMethodName = string.Empty;
    }
}
