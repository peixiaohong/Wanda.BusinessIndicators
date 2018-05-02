﻿using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;
using LJTH.BusinessIndicators.Common;
using Lib.Core;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.ViewModel;

using BPF.Workflow.Client;

namespace WebApi.Controllers
{
    public class ApprovalController : ApiController
    {
        private string VirtualUser = System.Configuration.ConfigurationManager.AppSettings["WF.VirtualUser"];
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="strProType"></param>
        /// <param name="ExecuteType"></param>
        /// <param name="OperatorType"></param>
        /// <param name="PrcessStatus"></param>
        public void ProcessRequest(string BusinessID, string strProType, string ExecuteType, int OperatorType, string PrcessStatus)
        {
            //业务处理
            DisposeBusinessData(BusinessID, strProType);

            //执行按钮事件的处理
            ExecutionBusinessData(BusinessID, strProType, ExecuteType, OperatorType);

        }



        #region 业务处理
        /// <summary>
        ///  业务处理，更新Monthlyreport表ProcessOwn字段，添加actio表记录
        /// </summary>
        private void DisposeBusinessData(string BusinessID, string strProType)
        {
            #region  业务处理
            if (string.IsNullOrEmpty(BusinessID))
            {
                throw new Exception("BusinessID is null!");
            }
            else
            {
                //添加谁点击了提交审批按钮
                B_MonthlyReport ReportModel = null;

                B_SystemBatch _BatchModel = null;
                Guid ID = Guid.Empty;
                Guid.TryParse(BusinessID, out ID);
                if (ID == Guid.Empty)
                    throw new Exception("系统编码错误");
                //正常的月报ID
                if (string.IsNullOrEmpty(strProType))
                {

                    ReportModel = B_MonthlyreportOperator.Instance.GetMonthlyreport(ID);
                    //月报业务ID ，没有问题
                    if (string.IsNullOrEmpty(ReportModel.ProcessOwn))
                    {
                        ReportModel.ProcessOwn = WebHelper.GetCurrentLoginUser();
                        B_MonthlyreportOperator.Instance.UpdateMonthlyreport(ReportModel);
                    }
                    B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
                    _bMonthlyReportAction.SystemID = ReportModel.SystemID;
                    _bMonthlyReportAction.MonthlyReportID = ReportModel.ID;
                    _bMonthlyReportAction.FinYear = ReportModel.FinYear;
                    _bMonthlyReportAction.FinMonth = ReportModel.FinMonth;
                    _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Submit);
                    _bMonthlyReportAction.Operator = WebHelper.GetCurrentLoginUser();
                    _bMonthlyReportAction.OperatorTime = DateTime.Now;
                    _bMonthlyReportAction.ModifierName = WebHelper.GetCurrentLoginUser();
                    _bMonthlyReportAction.CreatorName = WebHelper.GetCurrentLoginUser();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);

                }
                else
                {
                    //批次ID获取实体

                    _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(ID);

                    _BatchModel.ModifierName = WebHelper.GetCurrentLoginUser();
                    B_SystemBatchOperator.Instance.UpdateSystemBatch(_BatchModel);

                    B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
                    _bMonthlyReportAction.SystemID = Guid.Empty;
                    _bMonthlyReportAction.MonthlyReportID = _BatchModel.ID;
                    _bMonthlyReportAction.FinYear = _BatchModel.FinYear;
                    _bMonthlyReportAction.FinMonth = _BatchModel.FinMonth;
                    _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Submit);
                    _bMonthlyReportAction.Operator = WebHelper.GetCurrentLoginUser();
                    _bMonthlyReportAction.OperatorTime = DateTime.Now;
                    _bMonthlyReportAction.ModifierName = WebHelper.GetCurrentLoginUser();
                    _bMonthlyReportAction.CreatorName = WebHelper.GetCurrentLoginUser();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);
                }

            }
            #endregion

        }

        /// <summary>
        /// 处理流程按钮事件
        /// </summary>
        /// <param name="UserLonginID"></param>
        private void ExecutionBusinessData(string BusinessID, string strProType, string ExecuteType, int OperatorType, string UserLonginID = null)
        {

            switch (ExecuteType)
            {
                case "saveApplicationData":
                    //SaveApplactionBusinessData();
                    break;
                case "afterAction":
                    ProcessExecuteBusinessData(BusinessID, strProType, OperatorType);
                    AfterExcuteBusinessData(BusinessID, strProType, UserLonginID);
                    Guid ID = Guid.Empty;
                    Guid.TryParse(BusinessID, out ID);
                    if (ID == Guid.Empty)
                        throw new Exception("系统编码错误");
                    if (string.IsNullOrEmpty(strProType))
                    {

                        B_MonthlyReport ReportModelA = B_MonthlyreportOperator.Instance.GetMonthlyreport(ID);

                        List<NavigatActivity1> lstna = GetProcessIntance(ReportModelA.ID.ToString(), UserLonginID);
                        string Json = Newtonsoft.Json.JsonConvert.SerializeObject(lstna);
                        if (ReportModelA.WFStatus == "Draft" && OperatorType == 7)
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
                        var BacthModel = B_SystemBatchOperator.Instance.GetSystemBatch(ID);
                        List<NavigatActivity1> lstna = GetProcessIntance(BacthModel.ID.ToString(), UserLonginID);
                        string Json = Newtonsoft.Json.JsonConvert.SerializeObject(lstna);
                        BacthModel.ReportApprove = Json;
                        B_SystemBatchOperator.Instance.UpdateSystemBatch(BacthModel);
                    }
                    break;
                case "beforeAction":
                    //BeforeExcuteBusinessData();
                    break;
            }
        }

        /// <summary>
        /// 更新执行工作流后更新对应字段的值。如果是合并审批中退回，退回所有分支流程。
        /// </summary>
        private void ProcessExecuteBusinessData(string BusinessID, string strProType, int OperatorType)
        {

            B_MonthlyReport tempRPT = null;

            B_MonthlyReport rpt = null;
            B_SystemBatch BacthModel = null;
            Guid ID = Guid.Empty;
            Guid.TryParse(BusinessID, out ID);
            if (ID == Guid.Empty)
                throw new Exception("系统编码错误");

            if (string.IsNullOrEmpty(strProType)) //非批次
            {
                rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(ID);
                ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));
            }
            else //批次
            {
                BacthModel = B_SystemBatchOperator.Instance.GetSystemBatch(ID);
                ExceptionHelper.TrueThrow(BacthModel == null, string.Format("cannot find the report data which id={0}", BusinessID));

            }
            //WfOperationInfo wfop = JsonConvert.DeserializeObject<WfOperationInfo>(OperaionInfo);
            if (!string.IsNullOrEmpty(strProType)) //批次
            {
                List<V_SubReport> monthRpt = JsonConvert.DeserializeObject<List<V_SubReport>>(BacthModel.SubReport);

                #region 所有子流程的状态

                foreach (var item in monthRpt)
                {
                    B_MonthlyReport _rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(item.ReportID);

                    tempRPT = _rpt;
                    ExceptionHelper.TrueThrow(_rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));

                    //工作流状态，如果合并审批退回，退回所有分支流程
                    WorkFlowStatus(_rpt, true, OperatorType, BusinessID);

                    //针对退回，单独做了处理
                    if (OperatorType == 6)
                    {
                        item.IsReady = false;
                    }
                }

                #endregion


                //有效流程状态：Progress, Approved
                //6-return, 7 -withdraw

                #region 这个是整个批次的审批流程

                if (OperatorType == 6 || OperatorType == 7)
                {
                    BacthModel.WFBatchStatus = "Draft";
                    if (OperatorType == 6)
                    {
                        BacthModel.SubReport = JsonConvert.SerializeObject(monthRpt);

                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, ID);
                    }
                    else if (OperatorType == 7)
                    {
                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, ID);
                    }
                }
                else if (OperatorType == 9)//9, cancel
                {
                    BacthModel.WFBatchStatus = "Cancel";
                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, ID);
                }
                else
                {

                    if (BacthModel.WFBatchStatus != "Approved") //这里为了避免通知节点的提交覆盖了审批完成状态，而添加的
                    {
                        BacthModel.WFBatchStatus = "Progress";
                    }
                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, tempRPT.SystemID, tempRPT.FinYear, tempRPT.FinMonth, ID);
                }

                //修改批次数据
                B_SystemBatchOperator.Instance.UpdateSystemBatch(BacthModel);

                #endregion
            }
            else //不含批次
            {
                //工作流状态
                WorkFlowStatus(rpt, false, OperatorType, BusinessID);

                AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, ID);
            }
        }

        /// <summary>
        /// 记录工作流状态,针对退回特殊处理
        /// </summary>
        /// <param name="rpt"></param>
        /// <param name="IsProType">是否是批次</param>
        private void WorkFlowStatus(B_MonthlyReport rpt, bool IsProBatch, int OperatorType, string BusinessID)
        {
            Guid ID = Guid.Empty;
            Guid.TryParse(BusinessID, out ID);
            if (ID == Guid.Empty)
                throw new Exception("系统编码错误");
            //有效流程状态：Progress, Approved
            //4-return, 9 -withdraw
            if (OperatorType == 6 || OperatorType == 7)
            {
                rpt.WFStatus = "Draft";
                if (OperatorType == 6) //退回操作
                {

                    if (IsProBatch)
                    {
                        WorkflowContext wfc = WFClientSDK.GetProcess(null, rpt.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + VirtualUser });
                        BizContext bizContext = new BizContext();
                        bizContext.NodeInstanceList = wfc.NodeInstanceList;
                        bizContext.ProcessRunningNodeID = wfc.ProcessInstance.RunningNodeID;
                        bizContext.BusinessID = rpt.ID.ToString();
                        bizContext.ApprovalContent = "项目汇总退回服务发起";
                        bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$" + VirtualUser };
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

                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, rpt.SystemID, rpt.FinYear, rpt.FinMonth, ID);
                }
                else if (OperatorType == 7)
                {
                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, rpt.SystemID, rpt.FinYear, rpt.FinMonth, ID);

                    rpt.WFStatus = "Draft";
                }
            }
            else if (OperatorType == 9)//9, cancel
            {
                rpt.WFStatus = "Cancel";
                AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, rpt.SystemID, rpt.FinYear, rpt.FinMonth, ID);
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
                AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, ID);
            }

            B_MonthlyreportOperator.Instance.UpdateMonthlyreport(rpt);

        }



        /// <summary>
        /// 判断是否为虚拟暂挂节点，如果是 则更新IsReady=true。如果不是，更新合并审批意见。汇总流程审批完成。执行listB TO listA
        /// </summary>
        /// <param name="UserLonginID"></param>
        private void AfterExcuteBusinessData(string BusinessID, string ProType, string UserLonginID = null)
        {
            if (WFClientSDK.Exist(BusinessID))
            {
                var p = BPF.Workflow.Client.WFClientSDK.GetProcess(null, BusinessID, UserLonginID);
                //获取当前节点是否为暂挂节点。
                var nodetype = 0;
                Guid ID = Guid.Empty;
                Guid.TryParse(BusinessID, out ID);
                if (ID == Guid.Empty)
                    throw new Exception("系统编码错误");
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
                        B_MonthlyReport bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(ID);
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
                                //批次是草稿状态,
                                V_SubReportList = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);
                                V_SubReportList.Where(x => x.SystemID == bmr.AreaID).ForEach(x => x.IsReady = true);
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
                        _batchModel = B_SystemBatchOperator.Instance.GetSystemBatch(ID);
                        List<BPF.Workflow.Object.ProcessLog> _list = BPF.Workflow.Client.WFClientSDK.GetProcessLogList(BusinessID);

                        _batchModel.Batch_Opinions = JsonConvert.SerializeObject(_list);

                        //修改批次数据
                        B_SystemBatchOperator.Instance.UpdateSystemBatch(_batchModel);
                    }
                }
                //
                //汇总流程审批完成。执行listB TO listA ,对最后的审批做了更加严谨的判断
                if (p.ProcessInstance.Status == 3)
                {
                    if (p.CurrentUserNodeID != null && p.CurrentUserNodeID != "")
                    {
                        var nodeInfo = p.NodeInstanceList[p.CurrentUserNodeID];
                        if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7))
                        {
                            OnProcessCompletedBusinessData(BusinessID, ProType);
                        }
                    }

                }
            }
        }


        /// <summary>
        /// 审批结束，调用
        /// </summary>
        private void OnProcessCompletedBusinessData(string BusinessID, string ProType)
        {
            if (!string.IsNullOrEmpty(ProType))//批次
            {
                B_SystemBatch sysBatch = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
                ExceptionHelper.TrueThrow(sysBatch == null, string.Format("cannot find the report data which id={0}", BusinessID));

                //获取批次A表中的数据
                A_SystemBatch _sysBatch = A_SystemBatchOperator.Instance.GetSystemBatch(sysBatch.BatchType, sysBatch.FinYear, sysBatch.FinMonth);

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
                    WorkflowContext workflow = WFClientSDK.GetProcess(null, tempModel.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + VirtualUser });
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
                        bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$" + VirtualUser };
                        bizContext.ProcessURL = "/BusinessReport/TargetApprove.aspx";
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
                CreatorName = WebHelper.GetCurrentLoginUser(),
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
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, SystemBatchID = rpt.SystemBatchID, CreatorName = WebHelper.GetCurrentLoginUser(), CreateTime = DateTime.Now });

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
                A_MonthlyreportOperator.Instance.AddMonthlyreport(new A_MonthlyReport() { ID = rpt.ID, FinYear = rpt.FinYear, FinMonth = rpt.FinMonth, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, SystemBatchID = rpt.SystemBatchID, CreatorName = WebHelper.GetCurrentLoginUser(), CreateTime = DateTime.Now });

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



        private List<NavigatActivity1> GetProcessIntance(string BusinessID, string UserLonginID = null)
        {

            var p = BPF.Workflow.Client.WFClientSDK.GetProcess(null, BusinessID, UserLonginID);
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            NavigatActivity1 na1 = null;
            Dictionary<string, BPF.Workflow.Object.Node> list = new Dictionary<string, Node>();
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
                    na1.ActivityType = p1.Value.NodeType;
                    na1.RunningStatus = (p1.Value.Status > 1 ? 3 : p1.Value.Status);
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

        [Serializable]
        private class NavigatActivity1
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
            private int activityType;

            public int ActivityType
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
            private int runningStatus;

            public int RunningStatus
            {
                get { return runningStatus; }
                set { runningStatus = value; }
            }
        }

        [Serializable]
        private class ClientOpinion1
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
        private class NavigatCandidate1
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
        #endregion




        #region 日志
        /// <summary>
        /// 操作记录日志
        /// </summary>
        /// <param name="ActionType"></param>
        /// <param name="SysId"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="MonthReportID"></param>
        public void AddMonthlyReportAction(int ActionType, Guid SysId, int FinYear, int FinMonth, Guid MonthReportID)
        {
            B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
            _bMonthlyReportAction.SystemID = SysId;
            _bMonthlyReportAction.MonthlyReportID = MonthReportID;
            _bMonthlyReportAction.FinYear = FinYear;
            _bMonthlyReportAction.FinMonth = FinMonth;
            _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), ActionType);
            _bMonthlyReportAction.Operator = WebHelper.GetCurrentLoginUser();
            _bMonthlyReportAction.OperatorTime = DateTime.Now;
            _bMonthlyReportAction.ModifierName = WebHelper.GetCurrentLoginUser();
            _bMonthlyReportAction.CreatorName = WebHelper.GetCurrentLoginUser();

            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);
        }
        #endregion
    }
}