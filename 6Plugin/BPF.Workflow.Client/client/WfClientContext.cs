using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Wanda.Workflow.Object;
using Newtonsoft.Json;

namespace Wanda.Workflow.Client
{
    //public class WfClientContext
    //{
    //    private WfClientContext()
    //    {
    //        //Regsit();
    //    }
    //    /// <summary>
    //    /// IActivityHost接口
    //    /// </summary>
    //    private IActivityHost _Host = null;
    //    /// <summary>
    //    /// 已注册
    //    /// </summary>
    //    private bool _Regsited = false;
    //    private const string ContextKey = "WF_CONTROL_BAR_PROCESS_DATA";
    //    private const string OperaionInfoKey = "MCS_WF_OperationJSON";

    //    public static WfClientContext Current
    //    {
    //        get
    //        {
    //            if (!HttpContext.Current.Items.Contains(ContextKey))
    //                HttpContext.Current.Items[ContextKey] = new WfClientContext();
    //            return HttpContext.Current.Items[ContextKey] as WfClientContext;
    //        }
    //    }
    //    private WorkflowContext _processResponse = null;
    //    /// <summary>
    //    /// 请求的流程信息 
    //    /// </summary>
    //    public WorkflowContext ProcessResponse
    //    {
    //        get
    //        {
    //            if (_processResponse == null)
    //            {
    //                _processResponse = ClientProcess.GetProcess(Current._Host.BusinessID, Current._Host.CurrentUser);
    //            }
    //            return _processResponse;
    //        }
    //        private set { _processResponse = value; }
    //    }

    //    /// <summary>
    //    /// 注册
    //    /// </summary>
    //    public static void Regsit(WfClientListener listener, IActivityHost caller)
    //    {
    //        if (listener == null) listener = new WfClientListener();
    //        if (Current._Regsited) return;
    //        Current._Host = caller;
    //        ;
    //        var str_op = HttpContext.Current.Request[OperaionInfoKey];
    //        listener.OnListen();
    //        if (string.IsNullOrEmpty(str_op))
    //        {
    //            if (Current._Host.IsAutoCreate)
    //            {
    //                if (Current._Host.BusinessID == "" || Current.ProcessResponse.StatusCode == 211 || Current.ProcessResponse.ProcessInstance == null)
    //                {
    //                    //如果没有BusinessID或流程的状态为211（业务数据ID不存在）或流程实例为null，则认为是要创建流程。
    //                    listener.Start(Current._Host, Current);
    //                }
    //                else
    //                {
    //                    Current.Resident(Current.ProcessResponse);
    //                }
    //            }
    //            else
    //            {
    //                Current.Resident(Current.ProcessResponse);
    //            }
    //        }
    //        else
    //        {
    //            var param = InitExcuteParameter(str_op);
    //            //Current.Resident(JsonConvert.SerializeObject(Current.ProcessResponse));
    //            listener.Excute(Current._Host, param, Current);
    //        }
    //    }

    //    /// <summary>
    //    /// 注册ProcessInfo信息
    //    /// </summary>
    //    /// <param name="processInfo"></param>
    //    internal void Resident(WorkflowContext workflowContext)
    //    {
    //        var page = Current._Host as System.Web.UI.Page;
    //        if (page != null)
    //        {
    //            string processInfo = JsonConvert.SerializeObject(workflowContext);
    //            string script = "var " + AppSettingInfo.WorkflowContextJsonVarName + "=" + processInfo + ";";
    //            page.ClientScript.RegisterClientScriptBlock(page.GetType(), string.Empty, script, true);
    //        }
    //        else
    //        {
    //            Current._Host.Resident(workflowContext);
    //        }
    //    }

    //    /// <summary>
    //    /// 反序列化前端POST的BizContext的Json串
    //    /// </summary>
    //    /// <param name="json"></param>
    //    /// <returns></returns>
    //    private static ProcessExecuteParameter InitExcuteParameter(string json)
    //    {
    //        return JsonConvert.DeserializeObject<ProcessExecuteParameter>(json);
    //    }
    //}
}
