using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Common;
using Wanda.Platform.Permission.ClientComponent;
using Wanda.Platform.Permission.Contract;
using Lib.Web.Json;
using Wanda.Platform.WorkFlow.ClientComponent;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// ProcessController 的摘要说明
    /// </summary>
    public class ProcessController : IHttpHandler
    {
        protected string Content { get; set; }
        int OperatorType = 0;
        public void ProcessRequest(HttpContext context)
        {
            this.BusinessID = context.Request["BusinessID"];

            
            if (!string.IsNullOrEmpty(context.Request["OperatorType"]))
            {
                OperatorType = int.Parse(context.Request["OperatorType"]);
            }
            string strPrcessStatus = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["PrcessStatus"]))
            {
                strPrcessStatus = context.Request["PrcessStatus"];
            }
            if (string.IsNullOrEmpty(this.BusinessID))
            {
                throw new Exception("BusinessID is null!");
            }
            else
            {
                //添加谁点击了提交审批按钮,加入了获取流程信息json
                B_MonthlyReport ReportModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());

                if (string.IsNullOrEmpty(ReportModel.ProcessOwn))
                {
                    ReportModel.ProcessOwn = this.CurrentUser;

                    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModel);
                }

            }
            if (strPrcessStatus != "Approved")
            {
                OnProcessExecuteBusinessData(strPrcessStatus, OperatorType);
            }
            else
            {
                //审批结束，调用这个
                OnProcessCompletedBusinessData();
            }


            //处理本系统的数据
            DisposeBusinessData();



        }
        public void OnProcessExecuteBusinessData(string strPrcessStatus, int OperatorType)
        {
            //流程状态，0：未运行，草稿，1：运行中，2：退回，3：审批完成，4：归档，-1：作废，-2：驳回后归档
            B_MonthlyReport rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
            ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            //有效流程状态：Progress, Approved
            //4-return, 9 -withdraw
            if (OperatorType == 6 || OperatorType == 7)
            {

                if (OperatorType == 6)
                {
                    rpt.WFStatus = "Draft";
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
                }
                else if (OperatorType == 7)
                {
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());

                    rpt.WFStatus = "Draft";
                }
            }
            else if (OperatorType == 9)//9, cancel
            {
                rpt.WFStatus = "Cancel";
                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
            }
            else
            {
                if (rpt.WFStatus != "Approved") //这里为了避免通知节点的提交覆盖了审批完成状态，而添加的
                {
                    rpt.WFStatus = "Progress";
                    //CreatTime取流程提交时间,该时间为流程开始时间
                    rpt.CreateTime = DateTime.Now;
                    rpt.Status = 5;
                }
                //保存月度经营上报操作状态
                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
            }

            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(rpt);
            //RegistEvent();
        }


        /// <summary>
        /// 审批结束，调用
        /// </summary>
        public void OnProcessCompletedBusinessData()
        {
            B_MonthlyReport rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
            List<B_MonthlyReportDetail> rptDetailList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(BusinessID.ToGuid()).ToList();

            ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            rpt.WFStatus = "Approved";
            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(rpt);


            A_MonthlyReport rptA = null;

            List<A_MonthlyReportDetail> rptTempDetailList = new List<A_MonthlyReportDetail>();

            List<A_MonthlyReportDetail> rptADetailList = null;

            //A主表的的数据
            rptA = A_MonthlyreportOperator.Instance.GetAMonthlyReport(rpt.SystemID, rpt.FinYear, rpt.FinMonth);

            //A表明细数据
            rptADetailList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(rpt.SystemID, rpt.FinYear, rpt.FinMonth).ToList();


            //判断当月主表是否是null
            if (rptA == null)
            {
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //判断A 明细
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel(true)));

                    #endregion
                }
                else
                {

                    //删除A表明细的所有数据
                    A_MonthlyreportdetailOperator.Instance.DeleteMonthlyreportdetailList(rptADetailList);

                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel(true)));

                    #endregion
                }

                //添加明细数据
                A_MonthlyreportdetailOperator.Instance.AddMonthlyreportDetailList(rptTempDetailList);

            }
            else
            {
                //上来删除主表的ID
                A_MonthlyreportOperator.Instance.DeleteModel(rptA);

                //新增B表的主表数据
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //B表转换到A表
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel(true)));

                    #endregion
                }
                else
                {
                    //删除A表明细的所有数据
                    A_MonthlyreportdetailOperator.Instance.DeleteMonthlyreportdetailList(rptADetailList);

                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel(true)));

                    #endregion
                }

                //添加明细数据
                A_MonthlyreportdetailOperator.Instance.AddMonthlyreportDetailList(rptTempDetailList);
            }
        }

        /// <summary>
        /// 处理业务数据
        /// </summary>
        public void DisposeBusinessData(string UserLonginID = null)
        {
            #region


            B_MonthlyReport ReportModelA = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
            if (ReportModelA.WFStatus == "Draft")
            {
                ReportModelA.CreateTime = DateTime.Now;
            }
            var p = BPF.Workflow.Client.WFClientSDK.GetProcess(null , ReportModelA.ID.ToString(), UserLonginID);
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            NavigatActivity1 na1 = null;
            Dictionary<string, BPF.Workflow.Object.Node> list = new Dictionary<string, BPF.Workflow.Object.Node>();
            string strNextNodeID = p.ProcessInstance.StartNodeID;

            if (p.NodeInstanceList != null)
            {
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
                            CreateDate = p1.Value.FinishDateTime
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
           
            string Json = Newtonsoft.Json.JsonConvert.SerializeObject(listna);
            if (ReportModelA.WFStatus == "Draft" && OperatorType == 7)
            {
                ReportModelA.ReportApprove = null;
            }
            else
            {
                ReportModelA.ReportApprove = Json;
            }
            
            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModelA);

            #endregion
        }




        #region

        protected string ProcessKey { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string BusinessID
        {
            get;
            set;
        }

        public string CurrentUser
        {
            get
            {
                return PermissionHelper.GetCurrentUser;
            }
        }

        public string OperaionInfo
        {
            get;
            protected set;
        }

        protected string ProcessJSON { get; set; }

        public virtual string Recovery()
        {
            return ProcessJSON;
        }

        public virtual void Resident(string processJSON)
        {
            ProcessJSON = processJSON;
        }

        #endregion
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