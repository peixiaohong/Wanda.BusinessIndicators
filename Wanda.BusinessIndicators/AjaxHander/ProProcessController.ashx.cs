using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Common;
using Newtonsoft.Json;
using Wanda.Workflow.Client;
using Wanda.Workflow.Object;
using Lib.Xml;
using System.Xml.Linq;
using Wanda.Platform.WorkFlow.ClientComponent;
namespace LJTH.BusinessIndicators.Web.AjaxHander
{
    /// <summary>
    /// ProcessController 的摘要说明
    /// </summary>
    public class ProProcessController : IHttpHandler
    {
        protected string Content { get; set; }
        int OperaionTypes = 0;
        public void ProcessRequest(HttpContext context)
        {
            this.BusinessID = context.Request["BusinessID"];
            this.ProType = context.Request["strProType"];
            this.ExecType = context.Request["ExecuteType"];
            if (!string.IsNullOrEmpty(context.Request["OperatorType"]))
                this.OperaionType = int.Parse(context.Request["OperatorType"]);
            OperaionTypes = this.OperaionType;

            string strPrcessStatus = string.Empty;


            //业务处理
            DisposeBusinessData();

            //执行按钮事件的处理
            ExecutionBusinessData();


        }


        /// <summary>
        ///  业务处理
        /// </summary>
        public void DisposeBusinessData()
        {
            #region  业务处理
            if (string.IsNullOrEmpty(this.BusinessID))
            {
                throw new Exception("BusinessID is null!");
            }
            else
            {
                //添加谁点击了提交审批按钮
                B_MonthlyReport ReportModel = null;

                B_SystemBatch _BatchModel = null;

                //正常的月报ID
                if (string.IsNullOrEmpty(ProType))
                {
                    ReportModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                    //月报业务ID ，没有问题
                    if (string.IsNullOrEmpty(ReportModel.ProcessOwn))
                    {
                        ReportModel.ProcessOwn = this.CurrentUser;
                        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModel);
                    }
                    B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
                    _bMonthlyReportAction.SystemID = ReportModel.SystemID;
                    _bMonthlyReportAction.MonthlyReportID = ReportModel.ID;
                    _bMonthlyReportAction.FinYear = ReportModel.FinYear;
                    _bMonthlyReportAction.FinMonth = ReportModel.FinMonth;
                    _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Submit);
                    _bMonthlyReportAction.Operator = this.CurrentUser;
                    _bMonthlyReportAction.OperatorTime = DateTime.Now;
                    _bMonthlyReportAction.ModifierName = this.CurrentUser;
                    _bMonthlyReportAction.CreatorName = this.CurrentUser;
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);

                }
                else
                {
                    //批次ID获取实体

                    _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());

                    _BatchModel.ModifierName = this.CurrentUser;
                    B_SystemBatchOperator.Instance.UpdateSystemBatch(_BatchModel);

                    B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
                    _bMonthlyReportAction.SystemID = Guid.Empty;
                    _bMonthlyReportAction.MonthlyReportID = _BatchModel.ID;
                    _bMonthlyReportAction.FinYear = _BatchModel.FinYear;
                    _bMonthlyReportAction.FinMonth = _BatchModel.FinMonth;
                    _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Submit);
                    _bMonthlyReportAction.Operator = this.CurrentUser;
                    _bMonthlyReportAction.OperatorTime = DateTime.Now;
                    _bMonthlyReportAction.ModifierName = this.CurrentUser;
                    _bMonthlyReportAction.CreatorName = this.CurrentUser;
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);
                }

            }
            #endregion
            
        }

        public void ExecutionBusinessData(string UserLonginID = null)
        {

            switch (this.ExecType)
            {
                case "saveApplicationData":
                    SaveApplactionBusinessData();
                    break;
                case "afterAction":
                    ProcessExecuteBusinessData();
                    AfterExcuteBusinessData(UserLonginID);
                    if (string.IsNullOrEmpty(ProType))
                    {
                        B_MonthlyReport ReportModelA = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                      
                        List<NavigatActivity1> lstna = GetProcessIntance(ReportModelA.ID.ToString(), UserLonginID);
                        string Json = Newtonsoft.Json.JsonConvert.SerializeObject(lstna);
                        if (ReportModelA.WFStatus == "Draft" && OperaionTypes == 7)
                        {
                            ReportModelA.ReportApprove = null;
                        }
                        else
                        {
                            ReportModelA.ReportApprove = Json;
                        }
                        
                        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModelA);
                    }
                    else
                    {
                        B_SystemBatch SystemBatchA = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
                       
                        List<NavigatActivity1> lstna = GetProcessIntance(SystemBatchA.ID.ToString(), UserLonginID);
                        string Json = Newtonsoft.Json.JsonConvert.SerializeObject(lstna);
                        SystemBatchA.ReportApprove = Json;
                        B_SystemBatchOperator.Instance.UpdateSystemBatch(SystemBatchA);
                    }
                    break;
                case "beforeAction":
                    BeforeExcuteBusinessData();
                    break;
            }
        }




        public List<NavigatActivity1> GetProcessIntance(string BusinessID , string UserLonginID = null)
        {

            var p = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, BusinessID, UserLonginID);
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
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
            return listna;
        }

        /// <summary>
        ///新流程结束
        /// </summary>
        protected void OnProcessCompletedBusinessData()
        {
            if (!string.IsNullOrEmpty(this.ProType))//批次
            {
                B_SystemBatch sysBatch = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
                ExceptionHelper.TrueThrow(sysBatch == null, string.Format("cannot find the report data which id={0}", BusinessID));

                //获取批次A表中的数据
                A_SystemBatch _sysBatch = A_SystemBatchOperator.Instance.GetSystemBatch("ProSystem", sysBatch.FinYear, sysBatch.FinMonth);

                if (_sysBatch == null)
                {
                    AddBatchData(sysBatch);
                }
                else
                {
                    //先删除数据，在添加数据
                    int succeed = A_SystemBatchOperator.Instance.DeleteSystemBatch(_sysBatch);
                    ExceptionHelper.TrueThrow(succeed == 0, string.Format("批次表删除失败,批次ID：{0}", _sysBatch.ID));
                    AddBatchData(sysBatch);
                }
            }
            else
            {
                //不包含批次的
                //这里通过BusinessID 获取流程批次的信息
                B_MonthlyReport mrpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                ExceptionHelper.TrueThrow(mrpt == null, string.Format("cannot find the report data which id={0}", BusinessID));

                List<B_MonthlyReportDetail> rptDetailList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(mrpt.ID).ToList();
                mrpt.WFStatus = "Approved";
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(mrpt); //更新
                BtoAData(mrpt, rptDetailList);
            }
        }

        protected void SaveApplactionBusinessData()
        {
            //B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
            //_bMonthlyReportAction.SystemID = Guid.Empty;
            //_bMonthlyReportAction.MonthlyReportID = Guid.Empty;
            //_bMonthlyReportAction.FinYear = DateTime.Now.Year;
            //_bMonthlyReportAction.FinMonth = DateTime.Now.Month;
            //_bMonthlyReportAction.Action = "流程提交";
            //_bMonthlyReportAction.Operator = this.CurrentUser;
            //_bMonthlyReportAction.OperatorTime = DateTime.Now;
            //_bMonthlyReportAction.ModifierName = this.CurrentUser;
            //_bMonthlyReportAction.CreatorName = this.CurrentUser;
            //B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);


        }

        protected void AfterExcuteBusinessData(string UserLonginID = null)
        {
            if (WFClientSDK.Exist(BusinessID))
            {
                var p = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, this.BusinessID, UserLonginID);
                //获取当前节点是否为暂挂节点。
                var nodetype = 0;
                if (p.ProcessInstance.RunningNodeID != null && p.ProcessInstance.RunningNodeID != "")
                {
                    nodetype = p.NodeInstanceList[p.ProcessInstance.RunningNodeID].NodeType;
                }
                if (nodetype == 7)//如果当前节点类型等于7，当前节点为虚拟暂挂节点。
                {
                    //是在分支流程里面,这个判断已经走到了虚拟暂挂节点了。
                    //正对的是项目的分支流程
                    if (string.IsNullOrEmpty(ProType))
                    {
                        //获取月报主表
                        B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(this.BusinessID.ToGuid());
                        if (bmr != null && bmr.SystemBatchID != Guid.Empty) //判断当前的月报是否含有批次ID
                        {
                            #region 添加日志

                            B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();

                            _bMonthlyReportAction.SystemID = bmr.SystemID;
                            _bMonthlyReportAction.MonthlyReportID = bmr.ID;
                            _bMonthlyReportAction.FinYear = bmr.FinYear;
                            _bMonthlyReportAction.FinMonth = bmr.FinMonth;
                            _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Save);
                            _bMonthlyReportAction.Operator = System.Web.HttpContext.Current.User.Identity.Name;
                            _bMonthlyReportAction.OperatorTime = DateTime.Now;
                            _bMonthlyReportAction.ModifierName = System.Web.HttpContext.Current.User.Identity.Name;
                            _bMonthlyReportAction.CreatorName = System.Web.HttpContext.Current.User.Identity.Name;
                            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);

                            #endregion

                            #region 批次数据处理

                            //获取批次实体
                            B_SystemBatch BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(bmr.SystemBatchID);

                            List<V_SubReport> V_SubReportList = null;

                            if (BatchModel != null)
                            {
                                //批次是草稿状态
                                V_SubReportList = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);
                                foreach (var item in V_SubReportList)
                                {
                                    //选择的是那个系统？
                                    if (item.SystemID == bmr.SystemID)
                                    {
                                        //根据选择的系统
                                        item.IsReady = true;
                                    }

                                }
                                BatchModel.SubReport = JsonConvert.SerializeObject(V_SubReportList);
                            }

                            B_SystemBatchOperator.Instance.UpdateSystemBatch(BatchModel);

                            #endregion
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ProType) == false && ProType == "Batch")
                    {
                        //这里针对的是汇总后批次的流程意见
                        B_SystemBatch _batchModel = new B_SystemBatch();
                        _batchModel = B_SystemBatchOperator.Instance.GetSystemBatch(this.BusinessID.ToGuid());
                        List<Wanda.Workflow.Object.ProcessLog> _list = Wanda.Workflow.Client.WFClientSDK.GetProcessLogList(this.BusinessID);

                        _batchModel.Batch_Opinions = JsonConvert.SerializeObject(_list);

                        //修改批次数据
                        B_SystemBatchOperator.Instance.UpdateSystemBatch(_batchModel);
                    }
                }

                //var p = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, this.BusinessID);

                //
                //汇总流程审批完成。执行listB TO listA ,对最后的审批做了更加严谨的判断
                if (p.ProcessInstance.Status == 3)
                {
                    if (p.CurrentUserNodeID != null && p.CurrentUserNodeID != "")
                    {
                        var nodeInfo = p.NodeInstanceList[p.CurrentUserNodeID];
                        if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7))
                        {
                            OnProcessCompletedBusinessData();
                        }
                    }
 
                }
            }
        }

        protected void ProcessExecuteBusinessData()
        {

            B_MonthlyReport tempRPT = null;

            B_MonthlyReport rpt = null;
            B_SystemBatch BacthModel = null;

            if (string.IsNullOrEmpty(ProType)) //非批次
            {
                rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            }
            else //批次
            {
                BacthModel = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
                ExceptionHelper.TrueThrow(BacthModel == null, string.Format("cannot find the report data which id={0}", BusinessID));

            }
            //WfOperationInfo wfop = JsonConvert.DeserializeObject<WfOperationInfo>(OperaionInfo);
            if (!string.IsNullOrEmpty(ProType)) //批次
            {
                List<V_SubReport> monthRpt = JsonConvert.DeserializeObject<List<V_SubReport>>(BacthModel.SubReport);

                #region 所有子流程的状态

                foreach (var item in monthRpt)
                {
                    B_MonthlyReport _rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(item.ReportID);

                    tempRPT = _rpt;
                    ExceptionHelper.TrueThrow(_rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));

                    //工作流状态
                    WorkFlowStatus(_rpt, true);

                    //针对退回，单独做了处理
                    if (OperaionType == 6)
                    {
                        item.IsReady = false;
                    }
                }

                #endregion


                //有效流程状态：Progress, Approved
                //6-return, 7 -withdraw

                #region 这个是整个批次的审批流程

                if (OperaionType == 6 || OperaionType == 7)
                {
                    BacthModel.WFBatchStatus = "Draft";
                    if (OperaionType == 6)
                    {
                        BacthModel.SubReport = JsonConvert.SerializeObject(monthRpt);

                        new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, BusinessID.ToGuid());
                    }
                    else if (OperaionType == 7)
                    {
                        new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, BusinessID.ToGuid());
                    }
                }
                else if (OperaionType == 9)//9, cancel
                {
                    BacthModel.WFBatchStatus = "Cancel";
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, BusinessID.ToGuid());
                }
                else
                {

                    if (BacthModel.WFBatchStatus != "Approved") //这里为了避免通知节点的提交覆盖了审批完成状态，而添加的
                    {
                        BacthModel.WFBatchStatus = "Progress";
                    }
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, BusinessID.ToGuid());
                }

                //修改批次数据
                B_SystemBatchOperator.Instance.UpdateSystemBatch(BacthModel);

                #endregion
            }
            else //不含批次
            {
                //工作流状态
                WorkFlowStatus(rpt, false);

                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
            }
        }


        protected void BeforeExcuteBusinessData()
        {
            ////WfClientContext.Current.ProcessResponse.R
            //if (WfClientContext.Current.Process.CurrentActivity != null && WfClientContext.Current.Process.CurrentActivity.ActivtyID ==
            //    WfClientContext.Current.ProcessResponse.Navigat.Where(p => p.ActivityType != ActivityType.CC && p.ActivityType != ActivityType.AutoInform).Last().ActivityID &&
            //    e.Context.OperationType == OperationType.MoveTo)
            //    if (OnProcessCompleted != null)
            //        OnProcessCompleted(sender, e);
        }


        /// <summary>
        /// 添加更新批次数据
        /// </summary>
        /// <param name="rpt"></param>
        private void AddBatchData(B_SystemBatch _BatchModel)
        {

            //获取批次
            List<V_SubReport> BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(_BatchModel.SubReport);

            //批次更新
            BatchRptList.ForEach(p =>
            {
                B_MonthlyReport tempModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(p.ReportID);
                List<B_MonthlyReportDetail> rptDetailList = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(p.ReportID).ToList();
                tempModel.WFStatus = "Approved";
                #region 子流程虚拟审批人提交,是流程走完。
                try
                {
                    WorkflowContext workflow = WFClientSDK.GetProcess(null, tempModel.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$虚拟汇总人" });
                    if (workflow.ProcessInstance.Status != 3)
                    {
                        Dictionary<string, object> formParams = new Dictionary<string, object>();
                        formParams.Add("ReportName", workflow.ProcessInstance.ProcessTitle);
                        formParams.Add("ProcessKey", workflow.ProcessInstance.FlowCode);
                        BizContext bizContext = new BizContext();
                        bizContext.NodeInstanceList = workflow.NodeInstanceList;
                        bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                        bizContext.BusinessID = tempModel.ID.ToString();
                        bizContext.FlowCode = workflow.ProcessInstance.FlowCode;
                        bizContext.ApprovalContent = "同意";
                        bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$虚拟汇总人" };
                        bizContext.ProcessURL = "/BusinessReport/ProTargetApprove.aspx";
                        bizContext.FormParams = formParams;
                        bizContext.ExtensionCommond = new Dictionary<string, string>();
                        bizContext.ExtensionCommond.Add("RejectNode", Guid.Empty.ToString());
                        WorkflowContext wfc = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHelper.TrueThrow(true, string.Format("{0}", ex));
                }

                #endregion
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(tempModel); //??

                BtoAData(tempModel, rptDetailList);
            });

            _BatchModel.WFBatchStatus = "Approved";
            //修改B批次表的审批状态
            B_SystemBatchOperator.Instance.UpdateSystemBatch(_BatchModel);

            //将B批次表信息，添加到A表
            A_SystemBatchOperator.Instance.AddSystemBatch(new A_SystemBatch()
            {
                ID = _BatchModel.ID,
                FinYear = _BatchModel.FinYear,
                FinMonth = _BatchModel.FinMonth,
                BatchType = _BatchModel.BatchType,
                WFBatchStatus = _BatchModel.WFBatchStatus,
                SubReport = _BatchModel.SubReport,
                CreateTime = DateTime.Now,
                CreatorName = this.CurrentUser,
                Opinions = _BatchModel.Opinions,
                Batch_Opinions = _BatchModel.Batch_Opinions
            });

        }


        /// <summary>
        /// 将B表的数据，同步到A表中
        /// </summary>
        /// <param name="rpt"></param>
        private void BtoAData(B_MonthlyReport rpt, List<B_MonthlyReportDetail> rptDetailList)
        {
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
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, SystemBatchID = rpt.SystemBatchID, CreatorName = this.CurrentUser, CreateTime = DateTime.Now });

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
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, SystemBatchID = rpt.SystemBatchID, CreatorName = this.CurrentUser, CreateTime = DateTime.Now });

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
        /// 记录工作流状态
        /// </summary>
        /// <param name="rpt"></param>
        /// <param name="IsProType">是否是批次</param>
        protected void WorkFlowStatus(B_MonthlyReport rpt, bool IsProBatch)
        {

            //有效流程状态：Progress, Approved
            //4-return, 9 -withdraw
            if (OperaionType == 6 || OperaionType == 7)
            {
                rpt.WFStatus = "Draft";
                if (OperaionType == 6) //退回操作
                {

                    if (IsProBatch)
                    {
                        WorkflowContext wfc = WFClientSDK.GetProcess(null, rpt.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$虚拟汇总人" });
                        BizContext bizContext = new BizContext();
                        bizContext.NodeInstanceList = wfc.NodeInstanceList;
                        bizContext.ProcessRunningNodeID = wfc.ProcessInstance.RunningNodeID;
                        bizContext.BusinessID = rpt.ID.ToString();
                        bizContext.ApprovalContent = "项目汇总退回服务发起";
                        bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$虚拟汇总人" };
                        bizContext.ExtensionCommond = new Dictionary<string, string>();
                        bizContext.ExtensionCommond.Add("RejectNode", Guid.Empty.ToString());
                        WorkflowContext wf = WFClientSDK.ExecuteMethod("RejectProcess", bizContext);
                        if (wf.StatusCode != 0)
                        {
                            throw new Exception("子流程退回到发起人没有成功，请联系管理员。");
                        }


                        //ClientProcess.Return(rpt.ID.ToString(), "项目汇总退回服务发起", -1, null, "", false);

                        //ClientProcess.Return(rpt.ID.ToString(),  "这个是自动的服务意见", -1);
                    }

                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
                }
                else if (OperaionType == 7)
                {
                    new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());

                    rpt.WFStatus = "Draft";
                }
            }
            else if (OperaionType == 9)//9, cancel
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
                    rpt.ModifyTime = DateTime.Now;
                    rpt.Status = 5;
                }
                new MonthlyReportLog().AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, BusinessID.ToGuid());
            }

            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(rpt);

        }



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

        public string ProType
        {
            get;
            set;
        }
        public string ExecType
        {
            get;
            set;
        }

        public string CurrentUser
        {
            get { return PermissionHelper.GetCurrentUser; }
        }

        public int OperaionType
        {
            get;
             set;
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


        #region 工作流的审批意见Josn对象结构

        public class TempOpinion
        {
            public string a { get; set; }
            public string an { get; set; }
            public bool ce { get; set; }
            public DateTime d { get; set; }
            public string i { get; set; }
            public string r { get; set; }
            public string c { get; set; }
            public string t { get; set; }
            public string u { get; set; }
            public string ui { get; set; }
        }

        public class TempOpinionObj
        {
            public TempOpinion obj { get; set; }
            public TempDom dom { get; set; }
            public string actid { get; set; }
        }

        public class TempDom
        {
            public int length { get; set; }
        }

        #endregion
    }
}