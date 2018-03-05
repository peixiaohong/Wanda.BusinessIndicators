using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.Platform.WorkFlow.ClientComponent;
using Wanda.Workflow.Object;
using Wanda.Workflow.Client;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;
namespace Wanda.BusinessIndicators.Web.BusinessReport
{
    public partial class ModifiedErrorData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(("FDA708B6-257D-4753-A93F-C5751CF44282").ToGuid());
            //List<NavigatActivity1> listna = GetProcessIntance(bm.ID.ToString());
            //List<B_MonthlyReport> listbm = B_MonthlyreportOperator.Instance.GetMonthlyReportByApproveList(2015, 5);
            //for (int i = 0; i < listbm.Count(); i++)
            //{
            //    List<NavigatActivity1> listna = GetProcessIntance(listbm[i].ID.ToString());
            //    if (listna.Count > 0)
            //    {
            //        B_MonthlyReport ReportModel = listbm[i];
            //        string Json = Newtonsoft.Json.JsonConvert.SerializeObject(listna);
            //        ReportModel.ReportApprove = Json;
            //        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModel);
            //    }
            //}
        }
        public List<NavigatActivity1> GetProcessIntance(string BusinessID)
        {
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            if (Wanda.Workflow.Client.WFClientSDK.Exist(BusinessID))
            {
                var p = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, BusinessID);
                if (p != null)
                {
                    NavigatActivity1 na1 = null;
                    Dictionary<string, Wanda.Workflow.Object.Node> list = new Dictionary<string, Node>();
                    string strNextNodeID = p.ProcessInstance.StartNodeID;
                    foreach (var p1 in p.NodeInstanceList)
                    {
                        if (!string.IsNullOrEmpty(strNextNodeID))
                        {
                            var CurrentNode = p.NodeInstanceList[strNextNodeID];
                            list.Add(strNextNodeID, CurrentNode);
                            strNextNodeID = CurrentNode.NextNodeID;
                        }
                    }

                    foreach (var p1 in list)
                    {
                        if (string.IsNullOrEmpty(p1.Value.ParentNodeID))
                        {
                            na1 = new NavigatActivity1();
                            na1.ActivityID = p1.Value.NodeID;
                            na1.ActivityName = p1.Value.NodeName;
                            na1.ActivityType = (ActivityType)p1.Value.NodeType;
                            na1.RunningStatus = (WFRunningStatus)(p1.Value.Status > 1 ? 3 : p1.Value.Status);
                            List<ClientOpinion1> listclientOp = new List<ClientOpinion1>();
                            listclientOp.Add(new ClientOpinion1()
                            {
                                CreateDate = list[p.ProcessInstance.StartNodeID].FinishDateTime
                            });
                            na1.Opinions = listclientOp;

                            List<NavigatCandidate1> listnc = new List<NavigatCandidate1>();

                            var lstNode = p.NodeInstanceList.Where(s => s.Value.ParentNodeID == p1.Value.NodeID);
                            if (lstNode.Count() > 0)
                            {
                                foreach (var itme in lstNode)
                                {
                                    listnc.Add(new NavigatCandidate1()
                                    {
                                        Id = itme.Value.User.UserID,
                                        Name = itme.Value.User.UserName,
                                        Title = itme.Value.User.UserJobName,
                                        DeptName = itme.Value.User.UserOrgPathName,
                                        Completed = itme.Value.Status == 2 ? true : false
                                    });
                                }
                            }
                            else
                            {

                                listnc.Add(new NavigatCandidate1()
                                {
                                    Id = p1.Value.User.UserID,
                                    Name = p1.Value.User.UserName,
                                    Title = p1.Value.User.UserJobName,
                                    DeptName = p1.Value.User.UserOrgPathName,
                                    Completed = p1.Value.Status == 2 ? true : false
                                });
                            }
                            na1.Candidates = listnc;
                            listna.Add(na1);
                        }

                    }

                }
            }
            return listna;
        }

    }
    [Serializable]
    public class NavigatActivity1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private ActivityType activityType;

        public ActivityType ActivityType
        {
            get { return activityType; }
            set { activityType = value; }
        }
        private bool canBeReturned;

        public bool CanBeReturned
        {
            get { return canBeReturned; }
            set { canBeReturned = value; }
        }
        private List<NavigatCandidate1> candidates;

        public List<NavigatCandidate1> Candidates
        {
            get { return candidates; }
            set { candidates = value; }
        }
        private bool compelPass;

        public bool CompelPass
        {
            get { return compelPass; }
            set { compelPass = value; }
        }
        private List<ClientOpinion1> opinions;

        public List<ClientOpinion1> Opinions
        {
            get { return opinions; }
            set { opinions = value; }
        }
        private WFRunningStatus runningStatus;

        public WFRunningStatus RunningStatus
        {
            get { return runningStatus; }
            set { runningStatus = value; }
        }
    }

    [Serializable]
    public class ClientOpinion1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private bool canEdit;

        public bool CanEdit
        {
            get { return canEdit; }
            set { canEdit = value; }
        }
        private DateTime createDate;

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }


        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string operationType;

        public string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }
        private string opinContent;

        public string OpinContent
        {
            get { return opinContent; }
            set { opinContent = value; }
        }
        private string taskID;

        public string TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string userID;

        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }

    [Serializable]
    public class NavigatCandidate1
    {
        private bool completed;

        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }
        private string deptName;

        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; }
        }
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
    }
}