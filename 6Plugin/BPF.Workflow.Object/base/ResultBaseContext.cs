using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// WebService返回Json信息基类
    /// </summary>
    public class ResultBaseContext : BaseServeContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResultBaseContext()
        {
            this.StatusCode = ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_NOERROR;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Data">数据</param>
        public ResultBaseContext(object Data)
        {
            this.StatusCode = ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_NOERROR;
            this.Data = Data;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return StatusCode == ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_NOERROR;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        } private string _Key = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
