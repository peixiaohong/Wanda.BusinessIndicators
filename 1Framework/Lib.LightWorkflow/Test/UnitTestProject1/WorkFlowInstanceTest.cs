using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wanda.LightWorkflow;
using Wanda.LightWorkflow.Entities;
using Wanda.LightWorkflow.Dal;
using System.Collections.Generic;
using System.Collections;

namespace WorkflowTest
{
    [TestClass]
    public class WorkFlowInstanceTest
    {
        [TestMethod]
        public void WorkFlowStartTest()
        {
            ProcessInstanceAdapter adapter = new ProcessInstanceAdapter();
            //WorkflowInstance instance = new WorkflowInstance();
            //WorkflowInstance instance1 = new WorkflowInstance("2F2145C7-483C-4887-9B34-18AFC873F2FD");
            //ProcessInstance pInstance = adapter.Load("43EA20EC-6569-45F9-8A83-A49122883AE7");
            //WorkflowInstance instance2 = new WorkflowInstance(pInstance);
            //instance.sta
            string pid = Guid.NewGuid().ToString();
            string pcode = "BP01";
            string bizPid = "B1CA586C-7775-4096-B4E3-E5F9F9802017";
            string projId = string.Empty;
            string instanceName = "UINT_TEST_INSTANCE";
            string approvalNote = "Unit_Test";

            List<ProcessNode> nodeList = ProcessNodeAdapter.Instance.LoadList("3CA36C1B-1975-4E92-9A55-628FE422C160", "0");
            List<ProcessNodeInstance> list = new List<ProcessNodeInstance>();
            string guid = Guid.NewGuid().ToString();
            nodeList.ForEach(p =>
            {
                guid = Guid.NewGuid().ToString();
                list.Add(new ProcessNodeInstance()
                {
                    ID = guid,
                    BizProcessID = bizPid,
                    CreatedTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreatorName = "1",
                    cUserID = "1",
                    Description = "DESC",
                    Expression = "1==1",
                    IsDeleted = false,
                    IsHandSign = false,
                    LastUpdatedTime = DateTime.Now,
                    NodeID = p.ID,
                    NodeInstanceID = guid,
                    NodeName = p.NodeName,
                    NodeSeq = p.NodeSeq,
                    NodeType = p.NodeType,
                    OperationType = p.NodeSeq == 1 ? 1 : p.NodeSeq == 6 ? 5 : 2,
                    PreviousNodeInstanceID = "",
                    ProcessID = pid,
                    ProcessInstanceID = "",
                    ProcessType = "0",
                    RoleID = "1",
                    Status = p.NodeSeq == 1 ? 2 : 4,
                    UserCode = "111",
                    UserID = 1,
                    UserName = "111"
                });
            });
            //创建NodeList
            //List<ProcessNodeInstance> list=new List<ProcessNodeInstance>();
            //list = ProcessNodeInstanceAdapter.Instance.LoadList("43EA20EC-6569-45F9-8A83-A49122883AE7");
            //list[0].NodeInstanceID = Guid.NewGuid().ToString();
            //list[0].ID = list[0].NodeInstanceID;
            //list[0].cUserID = "1";
            //list[0].UserCode = "1";
            //list[0].NodeSeq = 1;
            //list[0].NodeName = "发起节点";
            //list[1].NodeInstanceID = Guid.NewGuid().ToString();
            //list[1].ID = list[1].NodeInstanceID;
            //list[1].cUserID = "11";
            //list[1].UserID = 11;
            //list[1].UserCode = "11";
            //list[1].NodeSeq = 2;
            //list[1].NodeName = "第一节点";


            Hashtable ht = new Hashtable();
            WorkflowInstance.Start(pid, pcode, bizPid, projId, instanceName, approvalNote, list, ht);

        }

        [TestMethod]
        public void WorkFlowSubmitTest()
        {
            ProcessNodeInstance pni = ProcessNodeInstanceAdapter.Instance.GetModelByID("67C69C34-AE8F-43FD-8D2D-9F90D8475133");
            WorkflowInstance instance = new WorkflowInstance("2367b136-88eb-4add-93c5-1949b2c9f27d");
            instance.DoSubmit("提交测试1", pni);
        }
        [TestMethod]
        public void WorkFlowRejectTest()
        {
            //ProcessNodeInstance pni = ProcessNodeInstanceAdapter.Instance.GetModelByID("CB0A8866-6A48-407F-B20A-198BBB136210");
            WorkflowInstance instance = new WorkflowInstance("2367b136-88eb-4add-93c5-1949b2c9f27d");
            instance.Reject("提交测试1", 111);
        }

    }
}
