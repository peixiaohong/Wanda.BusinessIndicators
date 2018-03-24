using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wanda.Workflow.Object;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Engine;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.Web.AjaxHander;
using Lib.Config;

namespace LJTH.BusinessIndicators.Web.MobileCommon
{
    public class MonthlyApprovalService : MobileServiceBase
    {
        /// <summary>
        /// 手机页面上的数据展示
        /// </summary>
        /// <param name="BusinessID"> 这个可能是多个参数，用“，”连接 </param>
        /// <param name="processCode"></param>
        /// <param name="wfc"></param>
        /// <returns></returns>
        public override FlowPageShowData getMobileCommonData(string BusinessID, string processCode, WorkflowContext wfc)
        {

            string _BusinessID = BusinessID.Split(',')[0]; // 参数1 
            string ProType = BusinessID.Split(',')[1]; // 参数2


            FlowPageShowData flowdata = new FlowPageShowData();
            flowdata.groups = new List<Groups>();
            if (wfc != null)
            {

                #region 页面数据展示
                
             
                if (!string.IsNullOrEmpty(ProType)) // 合并审批
                {
                    var Batch = B_SystemBatchOperator.Instance.GetSystemBatch(_BusinessID.ToGuid()); // 去拿批次数据    
                    List<V_SubReport> BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(Batch.SubReport);
                    
                    // for 循环多个 项目公司加载出来
                    if (BatchRptList.Count > 0)
                    {
                        BatchRptList.ForEach(BR =>
                        {
                            var approval = B_BusinessBaseOperator.Instance.GetBusinessBase(BR.ReportID);
                          
                            //基本信息
                            var baseInfo = InitBaseInfo(approval);
                            flowdata.groups.Add(baseInfo);
                            //附件信息（直接添加的）
                            var attachmenInfo = InitAttachment(approval);
                            flowdata.groups.Add(attachmenInfo);
                        });
                    }
                }
                else // 分支审批
                {
                    var approval = B_BusinessBaseOperator.Instance.GetBusinessBase(_BusinessID.ToGuid());
                    var monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(_BusinessID.ToGuid());
                    //这里判断系统的类型
                    int SysType = StaticResource.Instance[monthRpt.SystemID, approval.CreateTime].Category;
                    
                    if (SysType == 1 || SysType == 4)  // 直管 经营
                    {
                        //基本信息
                        var baseInfo = InitBaseInfo(approval);
                        flowdata.groups.Add(baseInfo);

                        //月度报表
                        var monthRptInfo = InitMonthRpt(approval);
                        flowdata.groups.Add(monthRptInfo);

                        //年度报表
                        var yearRptInfo = InitYearRpt(approval);
                        flowdata.groups.Add(yearRptInfo);

                        //附件信息（直接添加的）
                        var attachmenInfo = InitAttachment(approval);
                        flowdata.groups.Add(attachmenInfo);

                    }
                    else if (SysType == 2) //项目公司
                    {
                        //基本信息
                        var baseInfo = InitBaseInfo(approval);
                        flowdata.groups.Add(baseInfo);

                        //附件信息（直接添加的）
                        var attachmenInfo = InitAttachment(approval);
                        flowdata.groups.Add(attachmenInfo);
                    }
                    else if (SysType == 3) // 总部系统
                    {
                        //基本信息
                        var baseInfo = InitBaseInfo(approval);
                        flowdata.groups.Add(baseInfo);

                        //年度报表
                        var yearRptInfo = InitYearRpt(approval);
                        flowdata.groups.Add(yearRptInfo);

                        //附件信息（直接添加的）
                        var attachmenInfo = InitAttachment(approval);
                        flowdata.groups.Add(attachmenInfo);
                    }

                }
                
                #endregion

                #region 工作流信息
                flowdata.mainTitle = wfc.ProcessInstance.ProcessTitle;
                flowdata.requestId = BusinessID;
                flowdata.processCode = processCode;
                flowdata.status = ConstantWS.success;
                flowdata.message = ConstantWS.successText;
                flowdata.remarkisshow = "0";
                Groups workflowApprove = new Groups()
                {
                    type = ConstantWS.flowData_group_workFlow
                     ,
                    approvesubTitle = "审批流程"
                     ,
                    logsubTitle = "审批历史"
                    ,
                    AppCode = wfc.AppCode
                    ,
                    AppID = wfc.AppID
                    ,
                    BusinessID = wfc.BusinessID
                    ,
                    CurrentUser = wfc.CurrentUser
                    ,
                    WFToken = wfc.WFToken
                      ,
                    ProcessInstance = wfc.ProcessInstance
                    ,
                    NodeInstanceList = wfc.NodeInstanceList
                    ,
                    CcNodeInstanceList = wfc.CcNodeInstanceList
                    ,
                    CurrentUserNodeID = wfc.CurrentUserNodeID
                    ,
                    ProcessLogList = wfc.ProcessLogList
                     ,
                    CurrentUserSceneSetting = wfc.CurrentUserSceneSetting
                   ,
                    CurrentUserHasTodoTask = wfc.CurrentUserHasTodoTask
                    ,
                    CurrentUserTodoTaskIsRead = wfc.CurrentUserTodoTaskIsRead
                   ,

                    CurrentUserActivityPropertiesList = wfc.CurrentUserActivityPropertiesList
                    ,
                    ExtensionInfos = wfc.ExtensionInfos
                    ,
                    StatusCode = wfc.StatusCode.ToString()
                    ,
                    StatusMessage = wfc.StatusMessage
                    ,
                    LastException = wfc.LastException == null ? "" : wfc.LastException.Message
                };
                foreach (var log in workflowApprove.ProcessLogList)
                {
                    log.LogContent = log.LogContent.Replace("\"", "&#34;"); // 替换双引号，导致的Json数据截断问题。
                }
                flowdata.groups.Add(workflowApprove);
                #endregion

            }

            return flowdata;
        }


        /// <summary>
        /// 添加基础信息
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitBaseInfo(B_BusinessBase approval)
        {
            Groups baseInfo = new Groups()
            {
                type = ConstantWS.flowData_group_noHeader,
                subTitle = "基本信息",
                rows = new List<Rows>() {
                                new Rows() { key="上报系统", content=approval.SystemName }
                                ,new Rows() { key="上报年份",content=approval.FinYear +"年" }
                                ,new Rows() { key="上报月份", content=approval.FinMonth + "月"}
                                ,new Rows() { key="上报说明", content=approval.Description.Replace("\n", "<br/>").Replace(" ","&nbsp;")}
                            }
            };
            return baseInfo;
        }

        /// <summary>
        /// 添加月报信息
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitMonthRpt(B_BusinessBase approval)
        {
            var VModel = JsonConvert.DeserializeObject<List<DictionaryVmodel>>(approval.FormData);

            Groups groupMonthResult = new Groups()
            {
                type = ConstantWS.flowData_group_haveHeader,
                subTitle = "月度经营报告（本月）",
                titles = new List<Titles>()
                             {
                                 new Titles() { content="项目" }
                                  , new Titles() { content="本月计划<br>（万元）" }
                                   , new Titles() { content="本月实际<br>（万元）" }
                                    , new Titles() { content="完成率" }
                             }
            };

            List<MonthReportSummaryViewModel> lstResult = JsonConvert.DeserializeObject<List<MonthReportSummaryViewModel>>(VModel[1].ObjValue.ToString());

            List<Rows> ltResultRows = new List<Rows>();
            Rows rsResult;
            foreach (var model in lstResult)
            {
                rsResult = new Rows();
                rsResult.cells = new List<Cells>();

                if (model.TargetID.ToString().ToUpper() == AppSettingConfig.GetSetting("MonthSGRent", "").ToUpper())
                {
                    //这里 对商管的租金收缴率，做下特殊处理
                    rsResult.cells.Add(new Cells() { content = model.TargetName, td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = model.NPlanStr, td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = model.NActualStr, td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = model.NActualRate, td_style = "text-align:center" });
                }
                else
                {
                    rsResult.cells.Add(new Cells() { content = model.TargetName, td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = String.Format("{0:N0}", model.NPlanAmmount), td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = string.Format("{0:N0}", model.NActualAmmount), td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = model.NActualRate, td_style = "text-align:center" });
                }
                ltResultRows.Add(rsResult);
            }
            groupMonthResult.rows = ltResultRows;

            return groupMonthResult;
        }

        /// <summary>
        /// 添加年累计
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitYearRpt(B_BusinessBase approval)
        {
            var VModel = JsonConvert.DeserializeObject<List<DictionaryVmodel>>(approval.FormData);

            List<MonthReportSummaryViewModel> lstResult = JsonConvert.DeserializeObject<List<MonthReportSummaryViewModel>>(VModel[1].ObjValue.ToString());

            Groups groupYearResult = new Groups()
            {
                type = ConstantWS.flowData_group_haveHeader,
                subTitle = "月度经营报告（累计）",
                titles = new List<Titles>()
                             {
                                 new Titles() { content="项目" }
                                  , new Titles() { content="累计计划<br>（万元）" }
                                   , new Titles() { content="累计实际<br>（万元）" }
                                    , new Titles() { content="完成率" }
                             }
            };

            List<Rows> ltYearResultRows = new List<Rows>();
            Rows rsYearResult;
            foreach (var model in lstResult)
            {
                rsYearResult = new Rows();
                rsYearResult.cells = new List<Cells>();

                if (model.TargetID.ToString().ToUpper() == AppSettingConfig.GetSetting("MonthSGRent", "").ToUpper())
                {
                    //这里 对商管的租金收缴率，做下特殊处理
                    rsYearResult.cells.Add(new Cells() { content = model.TargetName, td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = model.NAccumulativePlanStr, td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = model.NAccumulativeActualStr, td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = model.NAccumulativeActualRate, td_style = "text-align:center" });
                }
                else
                {
                    rsYearResult.cells.Add(new Cells() { content = model.TargetName, td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = String.Format("{0:N0}", model.NAccumulativePlanAmmount), td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = String.Format("{0:N0}", model.NAccumulativeActualAmmount), td_style = "text-align:center" });
                    rsYearResult.cells.Add(new Cells() { content = model.NAccumulativeActualRate, td_style = "text-align:center" });
                }

             
                ltYearResultRows.Add(rsYearResult);
            }
            groupYearResult.rows = ltYearResultRows;

            return groupYearResult;
        }

        /// <summary>
        /// 添加附件(这个附件是上传，缺少生成的附件)
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitAttachment(B_BusinessBase approval)
        {

            List<Rows> ltrAttachment = new List<Rows>();
            List<B_Attachment> ltAAttachment = B_AttachmentOperator.Instance.GetAttachmentList(approval.MonthlyReportID).OrderBy(P=>P.BusinessType).OrderBy(P1=>P1.CreateTime).ToList();
            List<MFiles> ltmAttachmenFile = ConvertListAAttachmentToMFiles(ltAAttachment);
            Rows rsAttachment = new Rows()
            {
                key = "[多个]附件"
                 ,
                content = ""
                 ,
                type = "file"
                 ,
                files = ltmAttachmenFile
            };
            ltrAttachment.Add(rsAttachment);
            Groups Attachment = new Groups()
            {
                type = ConstantWS.flowData_group_noHeader,
                subTitle = "附件",
                rows = ltrAttachment
            };

            return Attachment;
        }


        public override FlowPageShowData getSubPageData(string PlanID, string CategoryID, string Approve)
        {

            return new FlowPageShowData();
        }


        /// <summary>
        /// 审批事件，审批到最后一个人走业务数据
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <returns></returns>
        public override string SubmitProcess(string businessID, UserInfo currentUser, string approvalContent)
        {
            string _businessID = businessID.Split(',')[0];
            string ProType = businessID.Split(',')[1]; // 参数2 
            
            string strResult = string.Empty;
            bool blResult = base.SubmitProcess(_businessID, currentUser, approvalContent, out strResult);

            #region 日志
            B_MonthlyReportAction BRA1 = new B_MonthlyReportAction();
            BRA1.Action = "业务数据提交审批判断：businessID:" + _businessID;
            BRA1.Description = "strResult : " + strResult + "; blResult :" + blResult.ToString() ;
            BRA1.Operator = "MonthlyApprovalService.SubmitProcess";
            BRA1.OperatorTime = DateTime.Now;
            BRA1.CreatorName = currentUser.UserLoginID;
            BRA1.IsDeleted = true;
            BRA1.MonthlyReportID = _businessID.ToGuid();
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA1);
            #endregion
            
            if (string.IsNullOrEmpty(strResult) && blResult)
            {
                try
                {
                    if (!string.IsNullOrEmpty(ProType)) //合并的流程
                    {
                        //项目公司的
                        ProProcessController ProMonthRptController = new ProProcessController();
                        ProMonthRptController.BusinessID = _businessID;
                        ProMonthRptController.ProType = businessID.Split(',')[1];
                        ProMonthRptController.ExecType = "afterAction";

                        ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                        ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                    }
                    else
                    {
                        var monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(_businessID.ToGuid());
                        //这里判断系统的类型
                        int SysType = StaticResource.Instance[monthRpt.SystemID, monthRpt.CreateTime].Category;

                        if (SysType == 2) //项目公司的分支流程
                        {
                            //项目公司的
                            ProProcessController ProMonthRptController = new ProProcessController();
                            ProMonthRptController.BusinessID = _businessID;
                            ProMonthRptController.ProType = businessID.Split(',')[1];
                            ProMonthRptController.ExecType = "afterAction";

                            ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                            ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                        }
                        else
                        {
                            // 这是经营的 直管， 总部的
                            ProcessController MonthRptController = new ProcessController();
                            MonthRptController.BusinessID = _businessID;

                            MonthRptController.OnProcessCompletedBusinessData();
                            MonthRptController.DisposeBusinessData(currentUser.UserLoginID); //  处理业务数据，保存工作流审批Json
                        }

                    }
                    
                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据提交审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "MonthlyApprovalService.SubmitProcess";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.IsDeleted = true;
                    BRA.MonthlyReportID = _businessID.ToGuid();

                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                   
                    #endregion
                }
                
                catch (Exception ex)
                {
                    strResult = "业务数据异常：" + ex.ToString();
                    
                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据提交审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "MonthlyApprovalService.SubmitProcess";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.IsDeleted = true;
                    BRA.MonthlyReportID = _businessID.ToGuid();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                    #endregion

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ProType))
                {
                    //项目公司的
                    ProProcessController ProMonthRptController = new ProProcessController();
                    ProMonthRptController.BusinessID = _businessID;
                    ProMonthRptController.ProType = businessID.Split(',')[1];
                    ProMonthRptController.ExecType = "afterAction";

                    ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                    ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                }
                else
                {
                    //每个审批人都要做的操作
                    var monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(_businessID.ToGuid());
                    //这里判断系统的类型
                    int SysType = StaticResource.Instance[monthRpt.SystemID, monthRpt.CreateTime].Category;
                    
                    if (SysType == 2)
                    {
                        //项目公司的
                        ProProcessController ProMonthRptController = new ProProcessController();
                        ProMonthRptController.BusinessID = _businessID;
                        ProMonthRptController.ProType = businessID.Split(',')[1];
                        ProMonthRptController.ExecType = "afterAction";

                        ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                        ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                    }
                    else
                    {
                        // 这是经营的 直管， 总部的
                        ProcessController MonthRptController = new ProcessController();
                        MonthRptController.BusinessID = _businessID;

                        MonthRptController.DisposeBusinessData(currentUser.UserLoginID); //  处理业务数据，保存工作流审批Json
                    }
                }

             

            }


            return strResult;
        }

        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public override string RejectProcessToStartNode(string businessID, UserInfo currentUser, string approvalContent, string[] uids)
        {
            string _businessID = businessID.Split(',')[0];
            string ProType = businessID.Split(',')[1]; // 参数2 

            string strResult = base.RejectProcessToStartNode(_businessID, currentUser, approvalContent, uids);
            if (string.IsNullOrEmpty(strResult))
            {            
                try
                {
                    if (!string.IsNullOrEmpty(ProType)) //合并的流程
                    {
                        //项目公司的
                        ProProcessController ProMonthRptController = new ProProcessController();
                        ProMonthRptController.BusinessID = _businessID;
                        ProMonthRptController.ProType = businessID.Split(',')[1];
                        ProMonthRptController.ExecType = "afterAction";
                        ProMonthRptController.OperaionType = 6;
                        ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                        ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                    }
                    else
                    {
                        var monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(_businessID.ToGuid());
                        //这里判断系统的类型
                        int SysType = StaticResource.Instance[monthRpt.SystemID, monthRpt.CreateTime].Category;

                        if (SysType == 2)
                        {
                            //项目公司的
                            ProProcessController ProMonthRptController = new ProProcessController();
                            ProMonthRptController.BusinessID = _businessID;
                            ProMonthRptController.ProType = businessID.Split(',')[1];
                            ProMonthRptController.ExecType = "afterAction";
                            ProMonthRptController.OperaionType = 6;
                            ProMonthRptController.DisposeBusinessData();//  处理业务数据，保存工作流审批Json
                            ProMonthRptController.ExecutionBusinessData(currentUser.UserLoginID);
                        }
                        else
                        {
                            ProcessController MonthRptController = new ProcessController();
                            MonthRptController.BusinessID = _businessID;
                            MonthRptController.OnProcessExecuteBusinessData("", 6);
                            MonthRptController.DisposeBusinessData(currentUser.UserLoginID); //  处理业务数据，保存工作流审批Json
                        }
                    }
                    
                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据退回审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "MonthlyApprovalService.RejectProcessToStartNode";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.IsDeleted = true;
                    BRA.MonthlyReportID = _businessID.ToGuid();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                    #endregion
                }
                catch (Exception ex)
                {
                    strResult = "业务数据异常：" + ex.ToString();

                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据退回审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "MonthlyApprovalService.RejectProcessToStartNode";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.IsDeleted = true;
                    BRA.MonthlyReportID = _businessID.ToGuid();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                    #endregion
                }
            }

            return strResult;
        }
        
    }
}