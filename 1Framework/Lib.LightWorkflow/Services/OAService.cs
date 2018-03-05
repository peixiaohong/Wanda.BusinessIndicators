using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Configs;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Tools;
using Wanda.Lib.LightWorkflow.Dal;
using Wanda.Lib.LightWorkflow.Filter;
using System.Transactions;
using Lib.Data;
using Wanda.HR.Common.Web;
using System.Collections;

namespace Wanda.Lib.LightWorkflow.Services
{
    /// <summary>
    /// 向中间表中发送OA待办和删除OA待办的服务接口
    /// </summary>
    public class OAService
    {
        public OAService()
        {
        }

        /// <summary>
        /// OA服务实例
        /// </summary>	
        public static OAService OAServiceInstance
        {
            get
            {
                return _OAServiceInstance;
            }
        }private static OAService _OAServiceInstance = new OAService();

        /// <summary>
        /// 是否需要发送OA待办
        /// 默认值为false不发送
        /// </summary>
        public bool? NeedSendOAMessage
        {
            get
            {
                if (_NeedSendOAMessage == null)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["NeedSendOAMessage"];
                    _NeedSendOAMessage = setting == null ? false : bool.Parse(setting.Value);
                }
                return _NeedSendOAMessage;
            }
        }private bool? _NeedSendOAMessage = null;


        /// <summary>
        /// 是否需要发送RTX通知消息
        /// 默认值为false不发送
        /// </summary>
        public bool? NeedSendRTXMessage
        {
            get
            {
                if (_NeedSendRTXMessage == null)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["NeedSendRTXMessage"];
                    _NeedSendRTXMessage = setting == null ? false : bool.Parse(setting.Value);
                }
                return _NeedSendRTXMessage;
            }
        }private bool? _NeedSendRTXMessage = null;

        /// <summary>
        /// OA系统识别本系统的名称
        /// </summary>
        public string SystemNameByOA
        {
            get
            {
                if (_SystemNameByOA == string.Empty)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["SystemNameByOA"];
                    if (setting == null)
                        throw new Exception("没有定义OA识别的系统名称。");
                    else
                        _SystemNameByOA = setting.Value;
                }
                return _SystemNameByOA;

            }
        }private string _SystemNameByOA = string.Empty;

        /// <summary>
        /// 表单Url
        /// </summary>
        public string FormUrl
        {
            get
            {
                if (_FormUrl == string.Empty)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["FormUrl"];
                    if (setting == null)
                        throw new Exception("没有定义在OA中打开表单页面Url。");
                    else
                        _FormUrl = setting.Value;
                }
                return _FormUrl;

            }
        }private string _FormUrl = string.Empty;

        /// <summary>
        /// 预警表单Url
        /// </summary>
        public string AlarmUrl
        {
            get
            {
                if (_AlarmUrl == string.Empty)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["AlarmUrl"];
                    if (setting == null)
                        throw new Exception("没有定义在OA中打开预警页面Url。");
                    else
                        _AlarmUrl = setting.Value;
                }
                return _AlarmUrl;

            }
        }private string _AlarmUrl = string.Empty;

        /// <summary>
        /// 创建OA待办
        /// </summary>
        /// <param name="todo"></param>
        public void CreateOATodo(OAMQMessages message)
        {
            if (NeedSendOAMessage == true)
            {
                message.Sendertime = message.Sendertime.AddSeconds(1);
                if (message.TodoType == (int)CommonConsts.TodoType.Notification)
                {
                    message.PtpUrl = string.Format(AlarmUrl, message.TodoID);
                }

                message.ID = OAMQMessagesAdapter.Instance.Insert(message);
            }
        }

        /// <summary>
        /// 删除OA待办，创建已办
        /// </summary>
        /// <param name="todo"></param>
        public void CloseOATodo(OAMQMessages message, bool needDoneMessage = true)
        {
            if (NeedSendOAMessage == true)
            {
                message.Flowmess = 9;//删除待办，oa系统中将看不到
                if (message.TodoType == (int)CommonConsts.TodoType.Notification)
                {
                    message.PtpUrl = string.Format(AlarmUrl, message.TodoID);
                }
                OAMQMessagesAdapter.Instance.Insert(message);
                //如果是流程类型的待办，需要在OA中创建一个已办,并且需要已办历史
                if (message.TodoType == (int)CommonConsts.TodoType.TODO && needDoneMessage)
                {
                    OAMQMessages doneMmessage = message;
                    //doneMmessage.ID = Guid.NewGuid().ToString();
                    doneMmessage.FlowID = message.FlowID + "_" + message.BizID.ToString();
                    doneMmessage.Flowmess = 2;   //已办
                    OAMQMessagesAdapter.Instance.Insert(doneMmessage);
                }
            }
        }

        /// <summary>
        /// 删除OA已办
        /// </summary>
        /// <param name="todo"></param>
        public void RemoveOADone(OAMQMessages message)
        {
            if (NeedSendOAMessage == true)
            {
                message.Flowmess = 9;//删除已办，oa系统中将看不到
                if (message.FlowID.IndexOf("_") <= 0)
                {
                    message.FlowID = message.FlowID + "_" + message.BizID.ToString();
                }
                if (message.TodoType == (int)CommonConsts.TodoType.Notification)
                {
                    message.PtpUrl = string.Format(AlarmUrl, message.TodoID);
                }
                OAMQMessagesAdapter.Instance.Insert(message);
            }
        }


        /// <summary>
        /// 批量删除OA待办，创建已办
        /// </summary>
        /// <param name="todo"></param>
        public void CloseOATodo(List<OAMQMessages> messageList, bool needDoneMessage = true)
        {
            foreach (var message in messageList)
            {
                CloseOATodo(message, needDoneMessage);
            }
        }

        /// <summary>
        /// 标记OA中的待办为已读
        /// </summary>
        /// <param name="todo"></param>
        public void UpdateOATodoReaded(TodoWork todo)
        {
            if (NeedSendOAMessage == true)
            {
                OAMQMessages message = OAMQMessagesAdapter.Instance.LoadByProcessInstanceID(todo.ProcessInstanceID);

                if (message == null)
                {
                    message = BuildMessage(todo);
                    //message.ID = Guid.NewGuid().ToString();
                    message.Sendertime = WebHelper.DateTimeNow;
                    message.Viewtype = -2;   //已读
                    message.Status = 0;
                    message.ErrorCount = 0;
                    message.MessageCreateTime = WebHelper.DateTimeNow;
                    OAMQMessagesAdapter.Instance.Insert(message);
                }
            }
        }

        public OAMQMessages BuildMessage(TodoWork todo)
        {
            OAMQMessages message = new OAMQMessages();
            message.Sender = SystemNameByOA;
            message.Sendertime = WebHelper.DateTimeNow;
            if (todo.ProcessCode == string.Empty)
                message.Flowtype = string.Empty;
            else
                message.Flowtype = WorkflowEngine.WorkflowService.ProcessNameList[todo.ProcessID];
            message.FlowID = todo.ProcessInstanceID.ToString();
            message.Title = todo.InstanceName;
            message.Nodename = todo.NodeName + ObjectHelper.EnumDescription((CommonConsts.NodeType)todo.NodeType);
            message.PtpUrl = string.Format(FormUrl, todo.BizProcessID);
            message.Userid = TodoWorkAdapter.Instance.GetLoginNameByUid(todo.UserID);
            message.CreatorName = TodoWorkAdapter.Instance.GetLoginNameByUid(todo.CreateProcessUserID);
            message.CreateTime = todo.CreateProcessTime;
            message.ModifierName = TodoWorkAdapter.Instance.GetLoginNameByUid(todo.CreateProcessUserID);
            message.ModifyTime = WebHelper.DateTimeNow;
            message.Operatetime = todo.CreatedTime;
            message.Flowmess = 1;   //待办
            message.Viewtype = 0;   //未读
            message.Status = 0;
            message.ErrorCount = 0;
            message.MessageCreateTime = WebHelper.DateTimeNow;
            message.TodoType = todo.TodoType;
            message.TodoID = todo.ID;
            message.BizID = todo.BizProcessID;
            return message;
        }

    }
}
