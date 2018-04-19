using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 节点扩展属性
    /// 多用于定义场景，供业务系统使用
    /// </summary>
    public class ActivityProperty
    {
        /// <summary>
        /// 属性编码
        /// </summary>
        public string PropertyCode
        {
            get
            {
                return _PropertyCode;
            }
            set
            {
                _PropertyCode = value;
            }
        }private string _PropertyCode;
        /// <summary>
        /// 属性值
        /// </summary>
        public string PropertyValue
        {
            get
            {
                return _PropertyValue;
            }
            set
            {
                _PropertyValue = value;
            }
        }private string _PropertyValue;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
            }
        }private bool _IsActive;
    }
}
