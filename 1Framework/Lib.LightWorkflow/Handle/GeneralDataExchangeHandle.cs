using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wanda.Lib.LightWorkflow.Interface;
using Wanda.Lib.LightWorkflow.Services;

namespace Wanda.Lib.LightWorkflow.Handle
{
    /// <summary>
    /// 通用的数据交换操作
    /// </summary>
    public class GeneralDataExchangeHandle : IDataExchangeHandle
    {
        public void OnProcessStarted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessReStarted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessApprovalCompleted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessCompleted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnNodeInitiated(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnNodeCompleted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessForwarded(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessEntrusted(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessCanceled(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }

        public void OnProcessRejected(object sender, WorkflowEventArgs e)
        {
            //nothing to do
        }
    }
}
