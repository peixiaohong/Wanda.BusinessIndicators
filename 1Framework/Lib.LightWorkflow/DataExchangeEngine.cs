using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Collections;
using Wanda.Lib.LightWorkflow.Builder;
using Wanda.Lib.LightWorkflow.Configs;
using Wanda.Lib.LightWorkflow.Interface;
using Wanda.Lib.LightWorkflow.Handle;

namespace Wanda.Lib.LightWorkflow
{
    /// <summary>
    /// 数据交换引擎
    /// </summary>
    [Serializable]
    public class DataExchangeEngine
    {
        public DataExchangeEngine()
        {
            //注册通用数据交换服务
            AddHandle("*", new GeneralDataExchangeHandle());
            //注册扩展的数据交换服务
            foreach (DataExchangeProviderInfo info in LightWorkflowSettings.Instance.DataExchangeProviderInfos)
            {
                string proecessCode = info.Key.Trim().ToUpper();
                //根据info.Key，实现不同类型的流程注册不同的处理方法
                IDataExchangeHandle handle = DataExchangeHandleBuilder.Instance.Build(info.Key);
                if (handle != null)
                    AddHandle(proecessCode, handle);
            }
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static DataExchangeEngine DataExchangeService
        {
            get
            {
                return _DataExchangeService;
            }
        }private static DataExchangeEngine _DataExchangeService = new DataExchangeEngine();

        private Dictionary<string, DataExchangeInstance> dataExchangeInstanceList = new Dictionary<string, DataExchangeInstance>();

        private DataExchangeInstance GeneralDataExchangeInstance
        {
            get
            {
                return dataExchangeInstanceList["*"];
            }
        }

        protected void AddHandle(string processCode, IDataExchangeHandle handleInstance)
        {
            DataExchangeInstance instance = null;
            if (!dataExchangeInstanceList.ContainsKey(processCode))
            {

                instance = new DataExchangeInstance();
                dataExchangeInstanceList.Add(processCode, instance);
            }
            else
            {
                instance = dataExchangeInstanceList[processCode];
            }
            instance.ProcessStarted += handleInstance.OnProcessStarted;
            instance.ProcessReStarted += handleInstance.OnProcessReStarted;
            instance.ProcessApprovalCompleted += handleInstance.OnProcessApprovalCompleted;
            instance.ProcessCompleted += handleInstance.OnProcessCompleted;
            instance.NodeInitiated += handleInstance.OnNodeInitiated;
            instance.NodeCompleted += handleInstance.OnNodeCompleted;
            instance.ProcessRejected += handleInstance.OnProcessRejected;
            instance.ProcessForwarded += handleInstance.OnProcessForwarded;
            instance.ProcessEntrusted += handleInstance.OnProcessEntrusted;
            instance.ProcessCanceled += handleInstance.OnProcessCanceled;
        }

        public void RaiseProcessStarted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessStarted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessStarted(sender, e);
            }
        }

        public void RaiseProcessReStarted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessReStarted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessReStarted(sender, e);
            }
        }

        public void RaiseProcessApprovalCompleted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessApprovalCompleted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessApprovalCompleted(sender, e);
            }
        }
        public void RaiseProcessCompleted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessCompleted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessCompleted(sender, e);
            }
        }
        public void RaiseNodeInitiated(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseNodeInitiated(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseNodeInitiated(sender, e);
            }
        }
        public void RaiseNodeCompleted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseNodeCompleted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseNodeCompleted(sender, e);
            }
        }
        public void RaiseProcessRejected(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessRejected(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessRejected(sender, e);
            }
        }
        public void RaiseProcessForwarded(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessForwarded(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessForwarded(sender, e);
            }
        }
        public void RaiseProcessEntrusted(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessEntrusted(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessEntrusted(sender, e);
            }
        }
        public void RaiseProcessCanceled(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessCanceled(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessCanceled(sender, e);
            }
        }

        internal void RaiseProcessWidthDrawed(object sender, WorkflowEventArgs e)
        {
            GeneralDataExchangeInstance.RaiseProcessWidthDrawed(sender, e);
            string processCode = e.WorkflowInstance.ProcessCode.ToUpper();
            if (dataExchangeInstanceList.ContainsKey(processCode))
            {
                DataExchangeInstance instance = dataExchangeInstanceList[processCode];
                instance.RaiseProcessWidthDrawed(sender, e);
            }
        }
    }

    [Serializable]
    internal class DataExchangeInstance
    {
        public event WorkflowEventHandler ProcessStarted = delegate { };
        public event WorkflowEventHandler ProcessReStarted = delegate { };
        public event WorkflowEventHandler ProcessApprovalCompleted = delegate { };
        public event WorkflowEventHandler ProcessCompleted = delegate { };
        public event WorkflowEventHandler NodeInitiated = delegate { };
        public event WorkflowEventHandler NodeCompleted = delegate { };
        public event WorkflowEventHandler ProcessRejected = delegate { };
        public event WorkflowEventHandler ProcessForwarded = delegate { };
        public event WorkflowEventHandler ProcessEntrusted = delegate { };
        public event WorkflowEventHandler ProcessCanceled = delegate { };
        public event WorkflowEventHandler ProcessWidthDrawed = delegate { };
        public void RaiseProcessStarted(object sender, WorkflowEventArgs e)
        {
            ProcessStarted(sender, e);
        }
        public void RaiseProcessReStarted(object sender, WorkflowEventArgs e)
        {
            ProcessReStarted(sender, e);
        }
        public void RaiseProcessApprovalCompleted(object sender, WorkflowEventArgs e)
        {
            ProcessApprovalCompleted(sender, e);
        }
        public void RaiseProcessCompleted(object sender, WorkflowEventArgs e)
        {
            ProcessCompleted(sender, e);
        }
        public void RaiseNodeInitiated(object sender, WorkflowEventArgs e)
        {
            NodeInitiated(sender, e);
        }
        public void RaiseNodeCompleted(object sender, WorkflowEventArgs e)
        {
            NodeCompleted(sender, e);
        }
        public void RaiseProcessRejected(object sender, WorkflowEventArgs e)
        {
            ProcessRejected(sender, e);
        }
        public void RaiseProcessForwarded(object sender, WorkflowEventArgs e)
        {
            ProcessForwarded(sender, e);
        }
        public void RaiseProcessEntrusted(object sender, WorkflowEventArgs e)
        {
            ProcessEntrusted(sender, e);
        }
        public void RaiseProcessCanceled(object sender, WorkflowEventArgs e)
        {
            ProcessCanceled(sender, e);
        }

        internal void RaiseProcessWidthDrawed(object sender, WorkflowEventArgs e)
        {
            ProcessWidthDrawed(sender, e);
        }

    }
}
