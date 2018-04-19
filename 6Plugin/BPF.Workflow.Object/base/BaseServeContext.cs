using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 上下文基类信息
    /// </summary>
    public abstract class BaseServeContext
    {
        /// <summary>
        /// 交互状态代码
        /// 0：成功
        /// 1~200：交互错误
        /// >200：异常错误
        /// </summary>
        public int StatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }
        }private int _StatusCode = 0;
        /// <summary>
        /// 交互状态消息
        /// </summary>
        public string StatusMessage
        {
            get { return _StatusMessage; }
            set { _StatusMessage = value; }
        }private string _StatusMessage = string.Empty;
        /// <summary>
        /// 最后一次异常信息
        /// </summary>
        public Exception LastException
        {
            get { return _LastException; }
            set { _LastException = value; }
        }private Exception _LastException = null;
    }
}
