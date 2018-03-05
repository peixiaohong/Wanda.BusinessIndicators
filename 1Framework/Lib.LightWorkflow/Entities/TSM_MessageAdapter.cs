using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Wanda.LightWorkflow.Entities;
using System.ComponentModel;


namespace Wanda.LightWorkflow.Dal 
{
    sealed class TSM_MessageAdapter : BaseAdapter<TMS_Messages>, ITSM_MessageAdapter
    {
        private static TSM_MessageAdapter _instance = new TSM_MessageAdapter();
        public static TSM_MessageAdapter Instance { get { return _instance; } }



        public void InsertMessage(string receiver, string title, string content, int messageType, DateTime? scheduleTime)
        {
            throw new NotImplementedException();
        }

        public void InsertRTXMessage(string receiver, string content)
        {
            throw new NotImplementedException();
        }

        public void InsertSMSMessage(string receiver, string content)
        {
            throw new NotImplementedException();
        }

        public List<TMS_Messages> LoadMessageQueue()
        {
            throw new NotImplementedException();
        }

        public void UploadMessageStatus(TMS_Messages message)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 消息队列状态
    /// </summary>
    public enum MessageStatus
    {
        [Description("未处理")]
        Pending = 0,

        [Description("成功")]
        Successful = 1,

        [Description("失败")]
        Failed = 2,

        [Description("待重试")]
        Retry = 3
    }
}

