using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.LightWorkflow.Entities;
namespace Wanda.Lib.LightWorkflow.Dal
{
    interface ITSM_MessageAdapter
    {
        int InsertMessage(string receiver, string title, string content, int messageType, DateTime? scheduleTime);
        int InsertRTXMessage(string receiver, string content);
        void InsertSMSMessage(string receiver, string content);
        //System.Collections.Generic.List<TMS_Messages> LoadMessageQueue();
        PartlyCollection<TMS_Messages> LoadMessageQueue(int count);
        void UploadMessageStatus(TMS_Messages message);
    }
}
