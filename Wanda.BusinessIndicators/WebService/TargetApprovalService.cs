using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPF.Workflow.Object;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Web.AjaxHander;

namespace LJTH.BusinessIndicators.Web.MobileCommon
{
    public class TargetApprovalService : MobileServiceBase
    {
        public override FlowPageShowData getMobileCommonData(string BusinessID, string processCode, WorkflowContext wfc)
        {
            string _BusinessID = BusinessID.Split(',')[0]; // 参数1 
            string ProType = BusinessID.Split(',')[1]; // 参数2

            FlowPageShowData flowdata = new FlowPageShowData();
            flowdata.groups = new List<Groups>();
            if (wfc != null)
            {
                //
                var Targetplan = B_TargetplanOperator.Instance.GetTargetplan(_BusinessID.ToGuid());
                var Sys = StaticResource.Instance[Targetplan.SystemID, Targetplan.ModifyTime];


                if (Sys.Category == 3)
                {
                    //基本信息
                    var baseInfo = InitBaseInfo(Sys, Targetplan);
                    flowdata.groups.Add(baseInfo);

                    //月度报表
                    var monthRptInfo = InitMonthRpt_Group(_BusinessID);
                    flowdata.groups.Add(monthRptInfo);

                }
                else
                {
                    //基本信息
                    var baseInfo = InitBaseInfo(Sys, Targetplan);
                    flowdata.groups.Add(baseInfo);

                    //月度报表
                    var monthRptInfo = InitMonthRpt(_BusinessID);
                    flowdata.groups.Add(monthRptInfo);
                }

                //附件信息（直接添加的）
                var attachmenInfo = InitAttachment(_BusinessID);
                flowdata.groups.Add(attachmenInfo);


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
                    log.LogContent = log.LogContent.Replace("\"", "&#34;");
                }
                flowdata.groups.Add(workflowApprove);
                #endregion

            }

            return flowdata;

        }


        /// <summary>
        /// 基础信息
        /// </summary>
        /// <param name="Sys"></param>
        /// <param name="Tp"></param>
        /// <returns></returns>
        protected Groups InitBaseInfo(C_System Sys, B_TargetPlan Tp)
        {
            Groups baseInfo = new Groups()
            {
                type = ConstantWS.flowData_group_noHeader,
                subTitle = "基本信息",
                rows = new List<Rows>() {
                                new Rows() { key="上报系统", content=Sys.SystemName }
                                ,new Rows() { key="上报年份",content=Tp.FinYear +"年" }
                            }
            };
            return baseInfo;
        }

        /// <summary>
        /// 月度的分解指标
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitMonthRpt(string BusinessID)
        {
            // 这里需要存储过程 来做
            var PlanList = B_TargetplandetailOperator.Instance.GetTargetPlanDetailByMobile(BusinessID.ToGuid());

            //去拿指标
            var TargetList = StaticResource.Instance.GetTargetList(PlanList[0].SystemID, DateTime.Now);

            //表头
            Groups groupMonthResult = new Groups();
            groupMonthResult.type = ConstantWS.flowData_group_haveHeader;
            groupMonthResult.subTitle = "指标分解（单位：万元）";
            if (TargetList != null && TargetList.Count > 0)
            {
                List<Titles> titles = new List<Titles>();
                Titles t1 = new Titles() { content = "月份" };
                titles.Add(t1);

                TargetList.ForEach(T =>
                {
                    titles.Add(new Titles() { content = T.TargetName });
                });

                groupMonthResult.titles = titles;
            }

            if (PlanList != null && PlanList.Count > 0)
            {
                List<Rows> ltResultRows = new List<Rows>();

                for (int i = 1; i < 14; i++)
                {
                    if (i == 13) //如果是13,代表的是汇总全年的数据
                    {
                        Rows rsResult = new Rows();
                        rsResult.cells = new List<Cells>();
                        //月份
                        rsResult.cells.Add(new Cells() { content = "全年", td_style = "text-align:center" });

                        //列值
                        TargetList.ForEach(T =>
                        {
                            var SumTarget = PlanList.Where(p => p.TargetID == T.ID).Sum(S => S.Target);
                            rsResult.cells.Add(new Cells() { content = String.Format("{0:N0}", SumTarget), td_style = "text-align:center" });
                        });

                        ltResultRows.Add(rsResult);

                    }
                    else
                    { //展示每个月的数据

                        var MList = PlanList.Where(p => p.FinMonth == i).ToList();
                        Rows rsResult = new Rows();
                        rsResult.cells = new List<Cells>();

                        //月份
                        rsResult.cells.Add(new Cells() { content = i.ToString() + "月", td_style = "text-align:center" });
                        //列值
                        TargetList.ForEach(T =>
                        {
                            var MP = MList.Where(M => M.TargetID == T.ID).FirstOrDefault();
                            rsResult.cells.Add(new Cells() { content = String.Format("{0:N0}", MP.Target), td_style = "text-align:center" });
                        });

                        ltResultRows.Add(rsResult);
                    }
                }

                groupMonthResult.rows = ltResultRows;
            }
            return groupMonthResult;
        }


        /// <summary>
        /// 集团分解指标
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <returns></returns>
        protected Groups InitMonthRpt_Group(string BusinessID)
        {
            //表头       
            Groups groupMonthResult = new Groups()
            {
                type = ConstantWS.flowData_group_haveHeader,
                subTitle = "指标分解",
                titles = new List<Titles>()
                             {
                                 new Titles() { content="项目" }
                                  , new Titles() { content="全年预算" }
                             }
            };

            // 分解指标明细
            var PlanList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(BusinessID.ToGuid());
            //去拿指标
            var TargetList = StaticResource.Instance.GetTargetList(PlanList[0].SystemID, DateTime.Now).Where(T => T.HaveDetail == false).OrderBy(S => S.Sequence).ToList();


            decimal YearSum = 0;

            if (TargetList != null && TargetList.Count > 0)
            {

                List<Rows> ltResultRows = new List<Rows>();

                TargetList.ForEach(T =>
                {
                    // 指标预算
                    var Tp = PlanList.Where(P => P.TargetID == T.ID).FirstOrDefault();

                    Rows rsResult = new Rows();
                    rsResult.cells = new List<Cells>();
                    //月份
                    rsResult.cells.Add(new Cells() { content = T.TargetName, td_style = "text-align:center" });
                    rsResult.cells.Add(new Cells() { content = String.Format("{0:N0}", Tp.Target), td_style = "text-align:center" });

                    ltResultRows.Add(rsResult);

                    YearSum = YearSum + Tp.Target;
                });


                //合计
                Rows rsResult_Sum = new Rows();
                rsResult_Sum.cells = new List<Cells>();
                //月份
                rsResult_Sum.cells.Add(new Cells() { content = "合计", td_style = "text-align:center" });
                rsResult_Sum.cells.Add(new Cells() { content = String.Format("{0:N0}", YearSum), td_style = "text-align:center" });

                ltResultRows.Add(rsResult_Sum);

                groupMonthResult.rows = ltResultRows;
            }

            return groupMonthResult;
        }


        /// <summary>
        /// 添加附件(这个附件是上传，缺少生成的附件)
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        protected Groups InitAttachment(string BusinessID)
        {

            List<Rows> ltrAttachment = new List<Rows>();
            List<B_Attachment> ltAAttachment = B_AttachmentOperator.Instance.GetAttachmentList(BusinessID.ToGuid()).OrderBy(P => P.BusinessType).OrderBy(P1 => P1.CreateTime).ToList();
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
            BRA1.Description = "strResult : " + strResult + "; blResult :" + blResult.ToString();
            BRA1.Operator = "TargetApprovalService.SubmitProcess";
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
                    TargetPlanProcessController TPPC = new TargetPlanProcessController();
                    TPPC.BusinessID = _businessID;
                    TPPC.OnProecssCompleted();
                    TPPC.DisposeBusinessData(currentUser.UserLoginID);
                    

                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据提交审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "TargetApprovalService.SubmitProcess";
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
                    BRA.Operator = "TargetApprovalService.SubmitProcess";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.IsDeleted = true;
                    BRA.MonthlyReportID = _businessID.ToGuid();
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                    #endregion

                }
            }
            else
            {
                TargetPlanProcessController TPPC = new TargetPlanProcessController();
                TPPC.BusinessID = _businessID;

                TPPC.DisposeBusinessData(currentUser.UserLoginID);
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
                    TargetPlanProcessController TPPC = new TargetPlanProcessController();
                    TPPC.BusinessID = _businessID;
                    TPPC.DisposeBusinessData(currentUser.UserLoginID);
                    TPPC.OnProcessExecute("",6);

                    #region 日志
                    B_MonthlyReportAction BRA = new B_MonthlyReportAction();
                    BRA.Action = "业务数据退回审批：businessID:" + _businessID;
                    BRA.Description = strResult;
                    BRA.Operator = "TargetApprovalService.RejectProcessToStartNode";
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
                    BRA.Operator = "TargetApprovalService.RejectProcessToStartNode";
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