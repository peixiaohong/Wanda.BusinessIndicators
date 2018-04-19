using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 客户端常量定义
    /// </summary>
    public static class ClientConstDefine
    {
        /// <summary>
        /// 工作流服务错误-无错误
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_NOERROR = 0;
        /// <summary>
        /// 无错误
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_NOERROR = "";
        /// <summary>
        /// 选人控件错误-错误但不影响正常操作，用于轮询时判断
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ERRORSUCCESS = -1;
        /// <summary>
        /// 错误但不影响正常操作，用于轮询时判断
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ERRORSUCCESS = "错误但不影响正常操作";
        /// <summary>
        /// 选人控件错误-程序执行出错
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ERROR = 500;
        /// <summary>
        /// 程序执行出错
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ERROR = "程序执行出错";
        /// <summary>
        /// 选人控件错误-参数错误
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ARGUMENTERROR = 501;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ARGUMENTERROR = "参数错误";
        /// <summary>
        /// 选人控件错误-获取服务端接口数据异常
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SERVERWEBSERVICEERROR = 502;
        /// <summary>
        /// 获取服务端接口数据异常
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_SERVERWEBSERVICEERROR = "获取服务端接口数据异常";


        /// <summary>
        /// 选人控件错误-SDK参数RequestType或Token为空
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKREQUESTTYPEORTOKENEMPTY = 1001;
        /// <summary>
        /// SDK参数RequestType或Token为空
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_SDKREQUESTTYPEORTOKENEMPTY = "SDK参数RequestType或Token为空";
        /// <summary>
        /// 选人控件错误-SDK参数RequestType输入值不正确
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKREQUESTTYPEERROR = 1002;
        /// <summary>
        /// SDK参数RequestType输入值不正确
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_SDKREQUESTTYPEERROR = "SDK参数RequestType输入值不正确";
        /// <summary>
        /// 选人控件错误-SDK参数Action或Method为空
        /// </summary>
        public const int WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKACTIONORMETHODEMPTY = 1003;
        /// <summary>
        /// SDK参数Action或Method为空
        /// </summary>
        public const string WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_SDKACTIONORMETHODEMPTY = "SDK参数Action或Method为空";



    }

}
