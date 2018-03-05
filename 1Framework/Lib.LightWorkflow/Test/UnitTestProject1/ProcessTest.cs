using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wanda.LightWorkflow.Dal;
using Wanda.LightWorkflow.Entities;
using System.Collections.Generic;
using Wanda.LightWorkflow;

namespace UnitTestProject1
{
    [TestClass]
    public class ProcessTest
    {
        #region ProcessAdapter
        [TestMethod]
        public void ProcessAdapterInsertTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            Process data = new Process()
            {
                ID = Guid.NewGuid().ToString(),
                ProcessCode = "BP01",
                ProcessName = "BP01",
                IsDeleted = false,
                IsActived = true,
                CreateTime = DateTime.Now,
                CreatorName = "huwz Test",
                Description = "Test BP Insert"

            };
            instance.Insert(data);

            data = new Process()
            {
                ID = Guid.NewGuid().ToString(),
                ProcessCode = "BP02",
                ProcessName = "BP02",
                IsDeleted = false,
                IsActived = true,
                CreateTime = DateTime.Now,
                CreatorName = "unit Test",
                Description = "unit Test Insert"

            };
            instance.Insert(data);

        }
        [TestMethod]
        public void ProcessAdapterUpdateTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            Process data = instance.GetAllActivedProcesses()[0];
            data.CreateTime = data.CreateTime.AddYears(1);
            data.CreatorName = data.CreatorName + "_UDP";
            data.Description = data.Description + "_UDP";
            data.IsActived = true;
            data.IsDeleted = true;
            data.ProcessCode = data.ProcessCode + "_UDP";
            data.ProcessName = data.ProcessName + "_UDP";
            instance.Update(data);

        }
        [TestMethod]
        public void ProcessAdapterListTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            Console.WriteLine(instance.GetAllActivedProcesses().Count);
        }
        [TestMethod]
        public void ProcessAdapterLoadTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            Process p = instance.Load("969D9A0B-8696-4A06-B1AE-9666BA35E69B");
            Console.WriteLine(string.Format("{0}<>{1}<>{2}<>{3}<>{4}<>{5}<>{6}<>{7}", p.CreateTime, p.CreatorName, p.Description, p.ID, p.IsActived, p.IsDeleted, p.ProcessCode, p.ProcessName));
        }
        [TestMethod]
        public void ProcessAdapterLoadByCodeTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            Process p = instance.LoadByCode("BP01_UDP");
            Console.WriteLine(string.Format("{0}<>{1}<>{2}<>{3}<>{4}<>{5}<>{6}<>{7}", p.CreateTime, p.CreatorName, p.Description, p.ID, p.IsActived, p.IsDeleted, p.ProcessCode, p.ProcessName));
        }
        [TestMethod]
        public void ProcessAdapterDeleteTest()
        {
            ProcessAdapter instance = ProcessAdapter.Instance;
            instance.Delete("969D9A0B-8696-4A06-B1AE-9666BA35E69B");
        }
        #endregion

        #region ProcessInstanceAdapter
        [TestMethod]
        public void ProcessInstanceAdapterTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            ProcessInstance p = new ProcessInstance()
            {
                BizProcessContext = "unit_test",
                CreateTime = DateTime.Now,
                cUserID = "userID",
                BizProcessID = "B1CA586C-7775-4096-B4E3-E5F9F9802017",
                CreatorName = "unit_test",
                //FinishTime = DateTime.Now.AddHours(1),
                ID = Guid.NewGuid().ToString(),
                InstanceName = "UINT_TEST_INSTANCE+-2",
                IsDeleted = false,
                LastUpdatedTime = DateTime.Now,
                ProcessCode = "BP02",
                ProcessID = "EF5E4365-9DE6-4855-A9CA-A2456D70EAC8",
                ProjectID =string.Empty,
                StartTime = DateTime.Now,
                Status = 2,
                UserCode = "system",
                UserID = 1,
                UserName = "unit_test_add"

            };
            pia.Insert(p);
        }
        [TestMethod]
        public void ProcessInstanceUpdateAdapterTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            ProcessInstance p = pia.Load("59588CFB-CAC0-4E2E-8973-844C505700D5");
            p.BizProcessContext = p.BizProcessContext + "UDP";
            p.InstanceName = p.InstanceName + "UDP";
            p.FinishTime = p.FinishTime.AddYears(10);
            p.Status = 1000;
            p.BizProcessID = p.BizProcessID + "UPD";
            pia.Update(p);
        }
        [TestMethod]
        public void ProcessInstanceDeleteAdapterTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            pia.Delete("01D0527F-BEDB-4C25-A1EB-167A2BF813C9");
        }

        [TestMethod]
        public void ProcessInstanceAdapterLoadByBizProcessIDTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            ProcessInstance p = pia.LoadByBizProcessID("BizProcessIDUPD");
        }
        [TestMethod]
        public void ProcessInstanceAdapterLoadListTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            List<string> s = new List<string>();
            s.Add("BizProcessIDUPD");
            s.Add("BizProcessIDUPD3");
            List<ProcessInstance> p = pia.LoadList(s);
            Console.WriteLine(p.Count);
        }

        [TestMethod]
        public void ProcessInstanceAdapterLoadListByCreateUserIDTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            Dictionary<string, string> dit = new Dictionary<string, string>();
            dit.Add("InstanceName", "UINT_TEST_INSTANCEUDPUDP");
            dit.Add("CreatedTime", "");
            dit.Add("LastUpdatedTime", "");
            dit.Add("ProcessCode", "");
            dit.Add("Status", "");
            List<ProcessInstance> p = pia.LoadListByCreateUserID(1, dit);
            Console.WriteLine(p.Count);
        }

        [TestMethod]
        public void ProcessInstanceAdapterLoadListByRelatedUserIDTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            Dictionary<string, string> dit = new Dictionary<string, string>();
            dit.Add("InstanceName", "UINT_TEST_INSTANCEUDPUDP");
            dit.Add("CreatedTime", "");
            dit.Add("LastUpdatedTime", "");
            dit.Add("ProcessCode", "");
            dit.Add("Status", "");
            dit.Add("UserName", "unit_test_add");
            List<ProcessInstance> p = pia.LoadListByRelatedUserID(1, dit);
            Console.WriteLine(p.Count);
        }
        [TestMethod]
        public void ProcessInstanceAdapterProcessInstanceStatusTest()
        {
            ProcessInstanceAdapter pia = new ProcessInstanceAdapter();
            List<ProcessInstance> list = pia.LoadList(null, 1, "","unit_test");
            List<ProcessInstance> list1 = pia.LoadList(Common.ProcessInstanceStatus.Archiving, 1,"", "unit_test");
            List<ProcessInstance> list2 = pia.LoadList(null, 0, "", "unit_test");
            List<ProcessInstance> list13 = pia.LoadList(Common.ProcessInstanceStatus.Archiving, 0, "", "unit_test");
            //TODO
        }
        #endregion

        #region ProcessNodeAdapter
        [TestMethod]
        public void ProcessNodeAdapterInsertTest()
        {
            ProcessNodeAdapter pna = new ProcessNodeAdapter();
            ProcessNode pn = new ProcessNode()
            {
                CreateTime = DateTime.Now,
                CreatorName = "auto_unit_test",
                Description = "test",
                Expression = "1==1",
                ID = Guid.NewGuid().ToString(),
                IsDeleted = false,
                IsEdit = false,
                IsHandSign = false,
                NodeName = "test_add",
                NodeSeq = 5,
                NodeType = 8,
                ProcessID = "3CA36C1B-1975-4E92-9A55-628FE422C160",
                ProcessType = "0",
                RoleID = Guid.NewGuid().ToString()
            };
            pna.Insert(pn);
        }
        [TestMethod]
        public void ProcessNodeAdapterUpdateTest()
        {
            ProcessNodeAdapter pna = new ProcessNodeAdapter();
            ProcessNode pn = pna.Load("506B7138-CB9B-4E48-BEF4-4AC48CAD9FF6");
            pn.CreateTime = pn.CreateTime.AddYears(10);
            pn.CreatorName = pn.CreatorName + "UDP";
            pn.Description = pn.Description + "UDP";
            pna.Update(pn);

        }
        [TestMethod]
        public void ProcessNodeAdapterLoadListTest()
        {
            ProcessNodeAdapter pna = new ProcessNodeAdapter();
            List<ProcessNode> pn = pna.LoadList("59588CFB-CAC0-4E2E-8973-844C505700D5", "0");

            Console.WriteLine(pn.Count);
        }
        [TestMethod]
        public void ProcessNodeAdapterDeleteTest()
        {
            ProcessNodeAdapter pna = new ProcessNodeAdapter();
            pna.Delete("653E65D5-A32D-4EFE-8A85-842DF8C7D1FB");


        }
        #endregion

        #region ProcessNodeInstanceAdapter
        [TestMethod]
        public void ProcessNodeInstanceAdapterInsertTest()
        {
            ProcessNodeInstanceAdapter pna = new ProcessNodeInstanceAdapter();
            ProcessNodeInstance pn = new ProcessNodeInstance()
            {
                BizProcessID = "unit_test",
                CreatedTime = DateTime.Now,
                CreateTime = DateTime.Now,
                CreatorName = "unit_test_user",
                cUserID = "unit_test_cUser",
                Description = "unit_test_desc",
                Expression = "test_Exp",
                ID = Guid.NewGuid().ToString(),
                IsDeleted = false,
                IsHandSign = false,
                LastUpdatedTime = DateTime.Now,
                NodeID = "BFC6EF76-86D2-40D3-A7EB-37031A91357E",
                NodeInstanceID = Guid.NewGuid().ToString(),
                NodeName = "new_name",
                NodeSeq = 1,
                NodeType = 1,
                OperationType = 1,
                PreviousNodeInstanceID = Guid.NewGuid().ToString(),
                ProcessID = "47d1d866-4c1e-4ce9-81ae-4701e521d540",
                ProcessInstanceID = "F7D243D8-6B03-4DC4-8979-22C0991FB6B9",
                ProcessType = "test",
                RoleID = "123",
                Status = 0,
                UserCode = "test",
                UserID = 2,
                UserName = "test2"
            };

            pna.Insert(pn);

        }
        [TestMethod]
        public void ProcessNodeInstanceAdapterUpdateTest()
        {
            ProcessNodeInstanceAdapter pna = new ProcessNodeInstanceAdapter();
            ProcessNodeInstance pn = pna.Load("C7D4D898-FC23-4F3C-90C4-BBF7AD20CDD7");
            pn.UserName = "UPDATE";
            pn.Description = "UPDATE_DESC";
            pn.CreatedTime = pn.CreateTime.AddYears(100);
            pna.Update(pn);
        }
        [TestMethod]
        public void ProcessNodeInstanceAdapterLoadListTest()
        {
            ProcessNodeInstanceAdapter pna = new ProcessNodeInstanceAdapter();
            List<ProcessNodeInstance> pn = pna.LoadList("F7D243D8-6B03-4DC4-8979-22C0991FB6B7");
            Console.WriteLine(pn.Count);
        }
        [TestMethod]
        public void ProcessNodeInstanceAdapterDeleteTest()
        {
            ProcessNodeInstanceAdapter pna = new ProcessNodeInstanceAdapter();
            pna.Delete("73930c93-d6cb-4bda-a455-f7304ce30292");

        }
        [TestMethod]
        public void ProcessNodeInstanceAdapterDeleteListTest()
        {
            ProcessNodeInstanceAdapter pna = new ProcessNodeInstanceAdapter();
            pna.DeleteList("F7D243D8-6B03-4DC4-8979-22C0991FB6B9");
        }
        #endregion

        #region StakeHolderAdapter
        [TestMethod]
        public void StakeHolderAdapterInsertTest()
        {
            StakeHolderAdapter sha = new StakeHolderAdapter();
            StakeHolder sh = new StakeHolder()
            {
                BizProcessID = "Unit_test",
                //???
                CreatedTime = DateTime.Now,
                CreateTime = DateTime.Now,
                CreatorName = "unit_test",
                cUserID = "unit_test",
                ID = Guid.NewGuid().ToString(),
                IsDeleted = false,
                NodeType = 0,
                ProcessID = "347D876A-1845-4F14-9532-6E454065F8D8",
                ProcessInstanceID = "59588CFB-CAC0-4E2E-8973-844C505700D5",
                UserCode = "unit_test",
                UserID = 0,
                UserName = "unit_test"
            };
            sha.Insert(sh);
        }
        [TestMethod]
        public void StakeHolderAdapterUpdateTest()
        {
            StakeHolderAdapter sha = new StakeHolderAdapter();
            StakeHolder sh = sha.Load("BCCC9DD1-5497-47D0-B137-303EC3CA1562");
            sh.CreatorName = sh.CreatorName + "UDP";
            sh.CreateTime = sh.CreateTime.AddYears(100);
            sh.UserID = 2;
            sha.Update(sh);
        }
        [TestMethod]
        public void StakeHolderAdapterLoadTest()
        {
            StakeHolderAdapter sha = new StakeHolderAdapter();
            StakeHolder sh = sha.Load("DD03D6D7-817E-4AEA-9B28-E0A732E98ADC", 2);
            Console.WriteLine(sh == null);
        }

        [TestMethod]
        public void StakeHolderAdapterDeleteTest()
        {
            StakeHolderAdapter sha = new StakeHolderAdapter();
            sha.Delete("03134BE8-F837-4A16-9DEF-F7FC13A5A97C");
        }
        [TestMethod]
        public void StakeHolderAdapterLoadListTest()
        {
            StakeHolderAdapter sha = new StakeHolderAdapter();
            List<StakeHolder> list = sha.LoadList("DD03D6D7-817E-4AEA-9B28-E0A732E98ADC");
            Console.WriteLine(list.Count);
        }
        #endregion

        #region TodoWorkAdapter
        [TestMethod]
        public void TodoWorkAdapterInsertTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            TodoWork tw = new TodoWork()
            {
                BizProcessID = "Unit_test",
                CreatedTime = DateTime.Now,
                CreateProcesscUserID = "1",
                CreateProcessTime = DateTime.Now,
                CreateProcessUserCode = "unit_test",
                CreateProcessUserID = 1,
                CreateProcessUserName = "unit_test",
                CreateTime = DateTime.Now,
                CreatorName = "unit_test",
                cUserID = "0",
                ID = Guid.NewGuid().ToString(),
                InstanceName = "UINT_TEST_INSTANCE",
                IsDeleted = false,
                ModifycUserID = "2",
                ModifyTime = DateTime.Now,
                ModifyUserCode = "test",
                ModifyUserID = 2,
                ModifyUserName = "test",
                NodeInstanceID = "D225C0B8-ABAC-487C-BB76-553FBCFE83F6",
                NodeName = "test_add",
                NodeType = 1,
                PreviousNodeInstanceID = "8AE10C4B-F27B-4566-89DD-FF3ACC4A9EFF",
                ProcessCode = "unit_test",
                ProcessID = "71173453-63e0-4b56-9d0d-4314d05326d6",
                ProcessInstanceID = "DD03D6D7-817E-4AEA-9B28-E0A732E98AbC",
                ProjectID = "",
                Status = 0,
                TodoType = 2,
                UserCode = "test",
                UserID = 1,
                UserName = "test_add_user"
            };

            ta.Insert(tw);
        }
        [TestMethod]
        public void TodoWorkAdapterUpdateTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            TodoWork tw = ta.Load("201B8623-C22B-4339-9E3B-97C1B214680A");
            tw.UserName = tw.UserName + "UDP";
            tw.Status = 1000;
            tw.ModifyUserName = "new Name";
            tw.CreateTime = tw.CreateTime.AddYears(10);
            ta.Update(tw);
        }

        [TestMethod]
        public void TodoWorkAdapterDeleteTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            TodoWork t = ta.GetModelByID("F2A5A00F-EA09-4974-8D48-EAAB9430807E");

            ta.Delete("10387189-2142-4572-9834-F05F04C0F931");
            ta.Delete(t);
        }

        [TestMethod]
        public void TodoWorkAdapterDeleteALLTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            ta.DeleteAll("DD03D6D7-817E-4AEA-9B28-E0A732E98AbC");
        }
        [TestMethod]
        public void TodoWorkAdapterLoadTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            TodoWork t = ta.Load("DD03D6D7-817E-4AEA-9B28-E0A732E98ADC", 1);
        }
        [TestMethod]
        public void TodoWorkAdapterLoadListTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            Dictionary<string, string> dit = new Dictionary<string, string>();
            dit.Add("InstanceName", "UINT_TEST_INSTANCE");
            dit.Add("CreatedTime", "");
            dit.Add("CreateProcessTime", "");
            dit.Add("ProcessCode", "");
            dit.Add("UserName", "test_add_user");
            List<TodoWork> t = ta.LoadList("DD03D6D7-817E-4AEA-9B28-E0A732E98ADC", 1, false, dit);

            List<TodoWork> t1 = ta.LoadList("Unit_test", 1, true, dit);

        }

        [TestMethod]
        public void TodoWorkAdapterLoadListByUserIDandBizProecessIdsTest()
        {
            TodoWorkAdapter ta = new TodoWorkAdapter();
            Dictionary<string, string> dit = new Dictionary<string, string>();
            List<TodoWork> t = ta.LoadListByUserIDandBizProecessIds(2, "");

            List<TodoWork> t1 = ta.LoadListByUserIDandBizProecessIds(2, "'71173453-63e0-4b56-9d0d-4314d05326d3','71173453-63e0-4b56-9d0d-4314d05326d4'");

        }
        #endregion

        #region ApprovalLog
        [TestMethod]
        public void ApprovalLogAdapterInsertTest()
        {
            ApprovalLogAdapter adp = new ApprovalLogAdapter();
            ApprovalLog al = new ApprovalLog()
            {
                ApprovalNote = "unit_test",
                BizProcessID = "unit_test",
                CompletedTime = DateTime.Now,
                CreatedTime = DateTime.Now.AddHours(-1),
                CreateTime = DateTime.Now.AddHours(-1),
                CreatorName = "unit_test",
                ID = Guid.NewGuid().ToString(),
                InstanceName = "UINT_TEST_INSTANCE",
                IsDeleted = false,
                NodeInstanceID = "201B8623-C22B-4339-9E3B-97C1B214680A",
                NodeName = "test_add",
                NodeType = 1,
                OperationType = 2,
                PreviousNodeInstanceID = "8AE10C4B-F27B-4566-89DD-FF3ACC4A9EFF",
                ProcessID = "71173453-63e0-4b56-9d0d-4314d05326d6",
                ProcessInstanceID = "DD03D6D7-817E-4AEA-9B28-E0A732E98Add",
                Status = 0,
                UserID = 1,
                UserName = "test_add_userUDP"

            };

            adp.Insert(al);
        }
        [TestMethod]
        public void ApprovalLogAdapterUpdateTest()
        {
            ApprovalLogAdapter adp = new ApprovalLogAdapter();
            ApprovalLog al = adp.Load("16BEB142-1067-44FA-9BE6-D52B214AD37E");
            al.UserName = "UPDATE";
            al.NodeName = "UPDATE";
            al.IsDeleted = true;
            adp.Update(al);
        }
        [TestMethod]
        public void ApprovalLogAdapterLoadListTest()
        {
            ApprovalLogAdapter adp = new ApprovalLogAdapter();
            List<ApprovalLog> al = adp.LoadList("DD03D6D7-817E-4AEA-9B28-E0A732E98ADC");
            Console.WriteLine(al.Count);
        }

        #endregion

        #region TODO
        #region TSM_MessageAdapter
        [TestMethod]
        //TODO
        public void TSM_MessageAdapterTest()
        {
            //TSM_MessageAdapter tma = new TSM_MessageAdapter();
            //TMS_Messages tm = new TMS_Messages()
            //{
            //    Content = "unit_test_content",
            //    CreateTime = DateTime.Now,
            //    CreatorName = "unit_test",
            //    ErrorInfo = "unit_test_error",
            //    ID = Guid.NewGuid().ToString(),
            //    IsDeleted = false,
            //    MessageType = 1,
            //    Priority = 1,
            //    SendTime = DateTime.Now.AddHours(1),
            //    Status = 0,
            //    Target = "1",
            //    TargetTime = DateTime.Now.AddHours(2),
            //    Title = "UNIT_TEST_TITILE",
            //    TryTimes = 3
            //};

            //tma.Insert(tm);
        }

        #endregion

        #region OAMQMessagesAdapter
        [TestMethod]
        public void OAMQMessagesAdapterTest()
        {

        }
        #endregion

        #endregion


    }
}
