using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Common;
using Lib.Web.Json;
using Wanda.Platform.WorkFlow.ClientComponent;

namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// ProcessController 的摘要说明
    /// </summary>
    public class TargetPlanProcessController : IHttpHandler
    {
        protected string Content { get; set; }
        int OperatorType = 1;
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
                //添加谁点击了提交审批按钮
                B_TargetPlan ReportModel = B_TargetplanOperator.Instance.GetTargetPlanByID(BusinessID.ToGuid());
                if (string.IsNullOrEmpty(ReportModel.ProcessOwn))
                {
                    ReportModel.ProcessOwn = this.CurrentUser;
                    B_TargetplanOperator.Instance.UpdateTargetplan(ReportModel);
                }
            }
            if (strPrcessStatus != "Approved")
            {
                OnProcessExecute(strPrcessStatus, OperatorType);
            }
            else
            {
                //审批结束，调用这个
                OnProecssCompleted();
            }
            //处理数据
            DisposeBusinessData();
            
        }
        /// <summary>
        /// 处理业务数据
        /// </summary>
        public void DisposeBusinessData(string UserLonginID = null)
        {
            #region


            B_TargetPlan ReportModelA = B_TargetplanOperator.Instance.GetTargetplan(BusinessID.ToGuid());
            if (ReportModelA.WFStatus == "Draft")
            {
                ReportModelA.CreateTime = DateTime.Now;
            }
            var p = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, ReportModelA.ID.ToString(), UserLonginID);
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            NavigatActivity1 na1 = null;
            Dictionary<string, Wanda.Workflow.Object.Node> list = new Dictionary<string, Wanda.Workflow.Object.Node>();
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

            B_TargetplanOperator.Instance.UpdateTargetplan(ReportModelA);

            #endregion
        }

        public void OnProcessExecute( string strPrcessStatus,int OperatorType)
        {
            B_TargetPlan rpt = B_TargetplanOperator.Instance.GetTargetPlanByID(BusinessID.ToGuid());
            ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            WfOperationInfo op = JsonHelper.Deserialize<WfOperationInfo>(OperaionInfo);
            //有效流程状态：Progress, Approved
            //6-return, 7 -withdraw
            if (OperatorType == 6 || OperatorType == 7)
            {
                if (OperatorType == 6)
                {
                    rpt.WFStatus = "Draft";
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, 0, BusinessID.ToGuid());
                }
                else if (OperatorType == 7)
                {
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, 0, BusinessID.ToGuid());

                    rpt.WFStatus = "Draft";
                }
            }
            else if (OperatorType == 9)//9, cancel
            {
                rpt.WFStatus = "Cancel";
                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, 0, BusinessID.ToGuid());

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
                //保存分解指标上报操作状态
                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, 0, BusinessID.ToGuid());
            }
            B_TargetplanOperator.Instance.UpdateTargetplan(rpt);

            //Wanda.Workflow.Object.WorkflowContext wc= Wanda.Workflow.Client.WFClientSDK.GetProcess(null,rpt.ID.ToString(), UserLonginID);
            //if (wc.ProcessInstance.Status == 3) {
            //    OnProecssCompleted();
            //}
        }
        /// <summary>
        /// 审批结束，调用
        /// </summary>
        public void OnProecssCompleted()
        {
            B_TargetPlan rpt = B_TargetplanOperator.Instance.GetTargetPlanByID(BusinessID.ToGuid());
            List<B_TargetPlanDetail> rptDetailList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(BusinessID.ToGuid()).ToList();

            ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            rpt.WFStatus = "Approved";
            B_TargetplanOperator.Instance.UpdateTargetplan(rpt);


            A_TargetPlan rptA = null;

            List<A_TargetPlanDetail> rptTempDetailList = new List<A_TargetPlanDetail>();

            List<A_TargetPlanDetail> rptADetailList = null;

            //A主表的的数据
            rptA = A_TargetplanOperator.Instance.GetTargetplanList(rpt.SystemID, rpt.FinYear).FirstOrDefault();

            //A表明细数据
            rptADetailList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(rpt.SystemID, rpt.FinYear).ToList();


            //判断当月主表是否是null
            if (rptA == null)
            {
                A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //判断A 明细
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }
                else
                {

                    //删除A表明细的所有数据
                    A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }

                //添加明细数据
                A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            }
            else
            {
                //上来删除主表的ID
                A_TargetplanOperator.Instance.DeleteModel(rptA);

                //新增B表的主表数据
                A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //B表转换到A表
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }
                else
                {
                    //删除A表明细的所有数据
                    A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }

                //添加明细数据
                A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            }

        }


        #region 原有流程实现。
    
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
            get { return PermissionHelper.GetCurrentUser; }
        }

        public string OperaionInfo
        {
            get;
            protected set;
        }

        protected string ProcessJSON { get; set; }

        //public virtual string Recovery()
        //{
        //    return ProcessJSON;
        //}

        //public virtual void Resident(string processJSON)
        //{
        //    ProcessJSON = processJSON;
        //}
        #endregion
    }
}