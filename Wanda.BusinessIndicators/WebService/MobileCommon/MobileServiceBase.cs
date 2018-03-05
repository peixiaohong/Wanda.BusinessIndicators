using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;
using Wanda.Workflow.Client;
using Wanda.Workflow.Object;
using MJScript = Microsoft.JScript;
using SDKCommon = Wanda.SDK.Common;
using UserSourceSDK = Wanda.UserSource.Client;

namespace Wanda.BusinessIndicators.Web.MobileCommon
{
    public abstract class MobileServiceBase
    {
        /*protected JavaScriptSerializer jss = new JavaScriptSerializer();*/
        public abstract FlowPageShowData getMobileCommonData(string BusinessID, string processCode, WorkflowContext wfc);
        /// <summary>
        ///将List<AAttachment>转换为 List<MFiles>
        /// </summary>
        /// <param name="ListAttachment"></param>
        /// <returns></returns>
        public List<MFiles> ConvertListAAttachmentToMFiles(List<B_Attachment> ListAttachment)
        {
            List<MFiles> ltResult = new List<MFiles>();
            if (ListAttachment != null)
            {
                foreach (B_Attachment attachment in ListAttachment)
                {
                    MFiles mfAttachmenFile = new MFiles()
                    {
                        fileName = attachment.FileName
                        ,
                        fileSize = attachment.Size
                        ,
                        downloadUrl = GetURL(attachment.Url.ToString(), attachment.FileName)// 这里需要在改改
                        ,
                        fileId = attachment.ID.ToString()
                    };
                    ltResult.Add(mfAttachmenFile);

                }
            }
            return ltResult;
        }
        /// <summary>
        /// 获取评审人员
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="ThreeLevelCategoryID"></param>
        /// <returns></returns>

        //public List<VAduitUserModel> GetAduitUser(Guid BusinessID, string ThreeLevelCategoryID)
        //{
        //    List<AGroupFrameworkStaffAssignment> AGroups = AGroupFrameworkStaffAssignmentOperator.Instance.GetAStaffAssignmentLisByBusinessID(BusinessID);

        //    List<VAduitUserModel> viewModel = new List<VAduitUserModel>();

        //    var result = from n in AGroups where n.CategoryLevelThreeID.ToString().ToLower() == ThreeLevelCategoryID.ToString().ToLower() && (n.RightsPersonType == 0 || n.RightsPersonType == 1 || n.RightsPersonType == 2) select n;
        //    result.ForEach(p =>
        //    {
        //        var newList = from l in viewModel where l.MembersCTX == p.MembersCTX select l;
        //        if (newList.Count() == 0 || newList == null)
        //        {
        //            VAduitUserModel model = new VAduitUserModel();
        //            model.StaffAssignmentID = p.ID.ToString();
        //            model.MembersCTX = p.MembersCTX;
        //            model.Members = p.Members;
        //            viewModel.Add(model);
        //        }
        //    });
        //    return viewModel;
        //}

        #region 流程操作 暂不支持加签操作
        /// <summary>
        /// 执行流程操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <returns></returns>
        public string ExecProcess(UserInfo currentUser, string[] uids, SumbitProcessParam SumbitParam)
        {
            string strResult = string.Empty;
            switch (SumbitParam.ButtonMethodName)
            {

                case "ForwardUser"://2 转发
                    strResult = ForwardUser(SumbitParam.requestId, currentUser, SumbitParam.remark, uids);
                    break;
                case "UndoSubmit"://3 撤回
                    strResult = UndoSubmit(SumbitParam.requestId, currentUser, SumbitParam.remark);
                    break;
                case "AddNode"://5 加签
                    strResult = AddNode(currentUser, SumbitParam);// "手机端暂不支持加签操作";
                    break;
                case "RejectProcess"://6 退回 只支持退回到发起节点
                    strResult = RejectProcessToStartNode(SumbitParam.requestId, currentUser, SumbitParam.remark, uids);
                    break;
                case "SubmitProcess"://1 提交
                default:
                    strResult = SubmitProcess(SumbitParam.requestId, currentUser, SumbitParam.remark );
                    break;
            }
            return strResult;
        }

        /// <summary>
        /// 提交审批（当最后的节点是同一个的时候，会有错误）
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <returns></returns>
        public virtual string SubmitProcess(string businessID, UserInfo currentUser, string approvalContent)
        {
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                throw workflow.LastException;
            }
            if (workflow.ProcessInstance.Status == 2)
            {
                strResult = "请到pc端发起流程！";
            }
            if (string.IsNullOrEmpty(strResult))
            {
                BizContext bizContext = new BizContext();
                bizContext.NodeInstanceList = workflow.NodeInstanceList;
                bizContext.CcNodeInstanceList = workflow.CcNodeInstanceList;
                bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                bizContext.BusinessID = workflow.BusinessID;
                bizContext.ApprovalContent = approvalContent;
                bizContext.CurrentUser = currentUser;
                WorkflowContext result = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);
                if (result.StatusCode != 0)
                {
                    strResult = result.LastException.Message;
                }
            }

            return strResult;
        }




        /// <summary>
        /// 提交流程
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        public bool SubmitProcess(string businessID, UserInfo currentUser, string approvalContent, out string Msg)
        {
            bool blResult = false;
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);


            B_MonthlyReportAction BRA1 = new B_MonthlyReportAction();
            BRA1.Action = " 进入 MobileServiceBase.SubmitProcess ：businessID:" + businessID;
            BRA1.Description = "workflow.StatusCode : " + workflow.StatusCode + "; workflow.ProcessInstance.Status :" + workflow.ProcessInstance.Status.ToString();
            BRA1.Operator = "MonthlyApprovalService.SubmitProcess";
            BRA1.OperatorTime = DateTime.Now;
            BRA1.CreatorName = currentUser.UserLoginID;
            BRA1.MonthlyReportID = businessID.ToGuid();
            BRA1.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA1);

            if (workflow.StatusCode != 0)
            {
                throw workflow.LastException;
            }
            if (workflow.ProcessInstance.Status == 2)
            {
                strResult = "请到pc端发起流程！";
            }
            if (string.IsNullOrEmpty(strResult))
            {

                BizContext bizContext = new BizContext();
                bizContext.NodeInstanceList = workflow.NodeInstanceList;
                bizContext.CcNodeInstanceList = workflow.CcNodeInstanceList;
                bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                bizContext.BusinessID = workflow.BusinessID;
                bizContext.ApprovalContent = approvalContent;
                bizContext.CurrentUser = currentUser;
                WorkflowContext result = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);

                blResult = CanApproveBusinessData(result);


               
                BRA1.Action = "MobileServiceBase.SubmitProcess 结束 ：businessID:" + businessID;
                BRA1.Description = "strResult : " + strResult + "; blResult :" + blResult.ToString();
                BRA1.Operator = "MonthlyApprovalService.SubmitProcess";
                BRA1.OperatorTime = DateTime.Now;
                BRA1.MonthlyReportID = businessID.ToGuid();
                BRA1.CreatorName = currentUser.UserLoginID;
                BRA1.IsDeleted = true;
                B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA1);
            }
            Msg = strResult;
            return blResult;
        }


        /// <summary>
        /// 退回到上一审批节点
        /// </summary>
        /// <param name="businessID">业务系统数据主键ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="approvalContent">审批意见</param>
        public string RejectProcessToPreNode(string businessID, UserInfo currentUser, string approvalContent)
        {
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                strResult = workflow.LastException.Message;
                return strResult;
            }

            var runningNode = workflow.NodeInstanceList[workflow.ProcessInstance.RunningNodeID];
            if (runningNode == null)
            {
                strResult = "不存在正在运行中的流程节点,workflow.ProcessInstance.RunningNodeID=" + workflow.ProcessInstance.RunningNodeID;
                return strResult;
            }
            var preNodeID = runningNode.PrevNodeID;

            BizContext bizContext = new BizContext();
            bizContext.NodeInstanceList = workflow.NodeInstanceList;
            bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
            bizContext.BusinessID = workflow.BusinessID;
            bizContext.ApprovalContent = approvalContent;
            bizContext.CurrentUser = currentUser;

            bizContext.ExtensionCommond = new Dictionary<string, string>();
            bizContext.ExtensionCommond.Add("RejectNode", preNodeID);//退回到指定节点
            WorkflowContext result = WFClientSDK.ExecuteMethod("RejectProcess", bizContext);
            if (result.StatusCode != 0)
            {
                strResult = result.LastException.Message;
            }
            return strResult;
        }

        /// <summary>
        /// 退回到发起节点
        /// </summary>
        /// <param name="businessID">业务系统数据主键ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="approvalContent">审批意见</param>
        public virtual string RejectProcessToStartNode(string businessID, UserInfo currentUser, string approvalContent, string[] uids)
        {
            string strResult = string.Empty;
            Guid startNode = Guid.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                strResult = workflow.LastException.Message;
                return strResult;
            }
            if (uids != null && uids.Length > 0)
            {
                if (Guid.TryParse(uids[0], out startNode) == false)
                {
                    strResult = "退回节点ID的不正确";
                }
                else if (!startNode.ToString().ToLower().Equals(workflow.ProcessInstance.StartNodeID.ToLower()))
                {
                    strResult = "只能退回到发起节点";
                }
                else
                {
                    startNode = Guid.Empty;
                }
            }
            if (string.IsNullOrEmpty(strResult))
            {
                BizContext bizContext = new BizContext();
                bizContext.NodeInstanceList = workflow.NodeInstanceList;
                bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                bizContext.BusinessID = workflow.BusinessID;
                bizContext.ApprovalContent = approvalContent;
                bizContext.CurrentUser = currentUser;

                bizContext.ExtensionCommond = new Dictionary<string, string>();
                bizContext.ExtensionCommond.Add("RejectNode", startNode.ToString());//如果值为Guid.Empty.ToString()则表示退回到发起节点
                WorkflowContext result = WFClientSDK.ExecuteMethod("RejectProcess", bizContext);
                if (result.StatusCode != 0)
                {
                    strResult = result.LastException.Message;
                }
            }

            return strResult;
        }

        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <returns></returns>
        public virtual string AddNode(UserInfo currentUser, SumbitProcessParam SumbitParam)
        {
            string strResult = string.Empty;
            return strResult;
        }

        /// <summary>
        /// 退回到指定节点
        /// </summary>
        /// <param name="businessID"></param>
        /// <param name="currentUser"></param>
        /// <param name="approvalContent"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public string RejectProcessToNode(string businessID, UserInfo currentUser, string approvalContent, string[] uids)
        {
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                strResult = workflow.LastException.Message;
                return strResult;
            }
            BizContext bizContext = new BizContext();
            string preNode = "";
            if (uids == null || uids.Length == 0)
            {
                preNode = Guid.Empty.ToString();
            }
            else
            {
                preNode = uids[0];
                bizContext.NodeInstanceList = workflow.NodeInstanceList;
                bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                bizContext.BusinessID = workflow.BusinessID;
                bizContext.ApprovalContent = approvalContent;
                bizContext.CurrentUser = currentUser;

                bizContext.ExtensionCommond = new Dictionary<string, string>();
                bizContext.ExtensionCommond.Add("RejectNode", uids[0]);//如果值为Guid.Empty.ToString()则表示退回到发起节点
                WorkflowContext result = WFClientSDK.ExecuteMethod("RejectProcess", bizContext);
                if (result.StatusCode != 0)
                {
                    strResult = result.LastException.Message;
                }
            }
            return strResult;
        }
        /// <summary>
        /// 撤回流程
        /// </summary>
        /// <param name="businessID">业务系统数据主键ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="approvalContent">审批意见</param>
        public string UndoSubmit(string businessID, UserInfo currentUser, string approvalContent)
        {
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                strResult = workflow.LastException.Message;
                return strResult;
            }

            BizContext bizContext = new BizContext();
            bizContext.NodeInstanceList = workflow.NodeInstanceList;
            bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
            bizContext.BusinessID = workflow.BusinessID;
            bizContext.ApprovalContent = approvalContent;
            bizContext.CurrentUser = currentUser;
            WorkflowContext result = WFClientSDK.ExecuteMethod("UndoSubmit", bizContext);
            if (result.StatusCode != 0)
            {
                strResult = result.LastException.Message;

            }
            return strResult;
        }
        /// <summary>
        /// 转发流程
        /// </summary>
        /// <param name="businessID">业务系统数据主键ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="approvalContent">审批意见</param>
        /// <param name="forwardUser">转发人员【完整的用户信息】</param>
        private string ForwardUser(string businessID, UserInfo currentUser, string approvalContent, List<UserInfo> forwardUser)
        {
            string strResult = string.Empty;
            WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);
            if (workflow.StatusCode != 0)
            {
                strResult = workflow.LastException.Message;
                return strResult;
            }

            BizContext bizContext = new BizContext();
            bizContext.NodeInstanceList = workflow.NodeInstanceList;
            bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
            bizContext.BusinessID = workflow.BusinessID;
            bizContext.ApprovalContent = approvalContent;
            bizContext.CurrentUser = currentUser;

            string strForwardUser = JsonConvert.SerializeObject(forwardUser);
            bizContext.ExtensionCommond = new Dictionary<string, string>();
            bizContext.ExtensionCommond["ForwardUser"] = strForwardUser;

            WorkflowContext result = WFClientSDK.ExecuteMethod("ForwardUser", bizContext);
            if (result.StatusCode != 0)
            {
                strResult = result.LastException.Message;
            }
            return strResult;
        }

        public string ForwardUser(string businessID, UserInfo currentUser, string approvalContent, string[] uids)
        {
            string errMsg = string.Empty;
            List<UserInfo> forwardUser = null;
            try
            {
                forwardUser = GetFlowUserInfo(uids);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            if (string.IsNullOrEmpty(errMsg) && forwardUser.Count > 0)
            {
                errMsg = ForwardUser(businessID, currentUser, approvalContent, forwardUser);
            }
            else if (forwardUser.Count == 0)
            {
                errMsg = "要转发的人员为空";
            }
            return errMsg;
        }

        /// <summary>
        /// 是否需要保存业务数据
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="businessID"></param>
        protected bool CanApproveBusinessData(WorkflowContext workflow)
        {
            bool blResult = false;
            //WorkflowContext workflow = WFClientSDK.GetProcess(null, businessID, currentUser);

            if (workflow != null)
            {
                if (workflow.StatusCode != 0)
                {
                    throw workflow.LastException;
                }
                else
                {

                    if (workflow.ProcessInstance.Status == 3)
                    {
                        //判断本次提交是否为审批节点/会签节点/虚拟节点的提交才执行。
                        if (workflow.CurrentUserNodeID != null && workflow.CurrentUserNodeID != "")
                        {
                            Node nodeInfo = null;
                            workflow.NodeInstanceList.TryGetValue(workflow.CurrentUserNodeID, out nodeInfo);
                            if (nodeInfo != null && (nodeInfo.NodeType == 1 || nodeInfo.NodeType == 2 || nodeInfo.NodeType == 7))
                            {
                                blResult = true;
                            }
                        }
                    }
                }
            }
            return blResult;
        }
        #endregion

        #region 获取流程用户
        /// <summary>
        /// 获取流程用户
        /// </summary>
        /// <param name="Uids"></param>
        /// <returns></returns>
        public List<UserInfo> GetFlowUserInfo(string[] Uids)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<UserInfo> ltResult = new List<UserInfo>();

            var userList = SDKCommon.ClientSDKHelper.GetResultContextData<List<UserSourceSDK.UserInfo>>(UserSourceSDK.UserSourceClientSDK.GetUserListByUserCodeList(Uids.ToList()));

            foreach (var item in userList)
            {
                UserInfo user = new UserInfo()
                {
                    UserCode = item.UserCode,
                    UserID = item.UserID,
                    UserJobName = item.UserJobName,
                    UserLoginID = item.UserLoginID,
                    UserName = item.UserName,
                    UserOrgID = item.UserOrgID,
                    UserOrgPathID = item.UserOrgPathID,
                    UserOrgPathName = item.UserOrgPathName
                };
                ltResult.Add(user);
            }
            return ltResult;

            //foreach (string id in Uids)
            //{
            //    if (string.IsNullOrEmpty(id.Trim()))
            //    {
            //        continue;
            //    }
            //    else
            //    {


            //        VUserInfo vuser = VSymUserInfoOperator.Instance.GetUserByLoginNane(id);
            //        UserInfo user = new UserInfo();
            //        if (vuser == null)
            //        {
            //            sb.AppendFormat("不存在用户：{0},", id);
            //        }
            //        else
            //        {
            //            user.UserID = vuser.employeeID;
            //            user.UserLoginID = vuser.LoginName;
            //            user.UserName = vuser.CNName;
            //            user.UserJobName = vuser.JobTitle;
            //            ltResult.Add(user);
            //        }
            //    }

            //}
            //if (sb.Length > 0)
            //{
            //    sb.Remove(sb.Length - 1, 1);
            //    throw new Exception(sb.ToString());
            //}
            //return ltResult;
        }


        public abstract FlowPageShowData getSubPageData(string PlanID, string CategoryID, string Approve);
        #endregion
        /// <summary>
        /// 获取下载地址
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        private string GetURL(string Path, string FileName)
        {
            string result = "";
            string http = System.Configuration.ConfigurationManager.AppSettings["MobileURL"];
            if (http != null)
            {
                result = string.Format("{0}?path={1}&FileName={2}", http, Path, FileName);
            }
            //LogCommon.NLogHelper.Log.Info(result,
            //             LogCommon.NLogHelper.MakeLogObj(LogCommon.NLogHelper.BusinessID_Const, result),
            //        LogCommon.NLogHelper.MakeLogObj(LogCommon.NLogHelper.MethodName_Const, "BaseWebService.GetURL"));
            return result;
        }



        protected string GetLocalFileUrl(string RelativeUrl)
        {
            string strFileWebsite = System.Configuration.ConfigurationManager.AppSettings["FileWebsite"];
            if (string.IsNullOrEmpty(strFileWebsite))
            {
                strFileWebsite = "http://zbgk.wanda.cn";
            }
            return strFileWebsite + RelativeUrl;
        }
        /// <summary>
        /// 加工 工作流程信息
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="processCode"></param>
        /// <param name="wfc"></param>
        /// <param name="flowdata"></param>
        /// <returns></returns>
        protected FlowPageShowData MakeWorkFlowInfo(string BusinessID, string processCode, WorkflowContext wfc, FlowPageShowData flowdata)
        {
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
            return flowdata;
        }
        /// <summary>
        /// 解密js方法escape加密后的文件 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        protected string JScriptUnEscape(string strValue)
        {
            string strRes = string.Empty;
            if (string.IsNullOrEmpty(strValue) == false)
            {
                strRes = MJScript.GlobalObject.unescape(strValue);
            }
            return strRes;
        }
    }
}