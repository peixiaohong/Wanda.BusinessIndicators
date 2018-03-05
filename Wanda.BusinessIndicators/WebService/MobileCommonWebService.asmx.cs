using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.BLL;
using Wanda.Workflow.Object;
using UserSourceSDK = Wanda.UserSource.Client;

namespace Wanda.BusinessIndicators.Web.MobileCommon
{
    /// <summary>
    /// MobileCommonWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://m.wanda.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class MobileCommonWebService : BaseWebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]

        public string getProcessCode(string param)
        {
            GetProcessCodeMessage gpcm = new GetProcessCodeMessage();
            string errMessage = string.Empty; ;
            if (base.AuthVerify() == false)
            {
                return base.authVerifyMessage;
            }

            B_MonthlyReportAction BRA = new B_MonthlyReportAction();
            BRA.Action = "WebService";
            BRA.Description = param;
            BRA.Operator = "getProcessCode()";
            BRA.OperatorTime = DateTime.Now;
            BRA.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

          

            GetProcessCodeParam proParam = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProcessCodeParam>(param.Replace(@"\", @""));

            string Urlparameter = GetBusinessIDFromUrl(proParam.sysPtpurl); // Url 的参数用“，”隔开连接
            string businessID = GetBusinessIDFromUrl(proParam.sysPtpurl).Split(',')[0]; // 第一个参数是 业务ID
            string ProType = GetBusinessIDFromUrl(proParam.sysPtpurl).Split(',')[1];//第二个参数

            //todo
            if (string.IsNullOrEmpty(businessID) == false)
            {
                try
                {
                    string processCode = GetProcessCode(businessID,ProType);
                    if (string.IsNullOrEmpty(processCode) == false)
                    {
                        gpcm.status = ConstantWS.success;
                        gpcm.message = ConstantWS.successText;
                        gpcm.processCode = processCode;
                        gpcm.requestId = Urlparameter;

                    }
                    else
                    {
                        gpcm.status = ConstantWS.error;

                        gpcm.processCode = processCode;
                        gpcm.requestId = Urlparameter;
                        gpcm.message = string.Format("获取不存在processCode {0},requestId:{1} 不存在processCode", ConstantWS.errorText, Urlparameter);
                    }
                 
                    BRA.Action = "WebService";
                    BRA.Description = string.Format("获取流程FlowID:{0} 的信息", businessID);
                    BRA.Operator = "MobileCommonWebService.GetProcessCode";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.MonthlyReportID = businessID.ToGuid();
                    BRA.IsDeleted = true;
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

                }
                catch (Exception ex)
                {
                    gpcm.status = ConstantWS.error;
                    gpcm.message = "发生不可预知的错误：" + ex.Message;

                    BRA.Action = "WebService";
                    BRA.Description = gpcm.message;
                    BRA.Operator = "getProcessCode_Error";
                    BRA.OperatorTime = DateTime.Now;
                    BRA.MonthlyReportID = businessID.ToGuid();
                    BRA.IsDeleted = true;
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
                }
            }
            else
            {
                gpcm.status = ConstantWS.error;
                gpcm.message = "requestId为空";
            }
          
            BRA.Action = "WebService";
            BRA.Description = string.Format("获取流程FlowID:{0} 的返回信息：{1}", businessID, Newtonsoft.Json.JsonConvert.SerializeObject(gpcm));
            BRA.Operator = "MobileCommonWebService.GetProcessCode";
            BRA.OperatorTime = DateTime.Now;
            BRA.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);


            return Newtonsoft.Json.JsonConvert.SerializeObject(gpcm);

        }



        /// <summary>
        /// 获取流程页面展示数据
        /// </summary>
        /// <param name="param">
        /// 符合JSON格式的字符串
        /// 字符串中包含以下参数：
        ///processCode:流程类型编码
        ///requestId：业务流程待办ID
        ///userLoginId：当前操作用户登录ID（RTX账号）
        ///userId：当前操作用户ID
        /// </param>
        [WebMethod(Description = @"获取流程页面展示数据,
                                    param符合JSON格式的字符串
                                        processCode:流程类型编码
                                        requestId：业务流程待办ID
                                        userLoginId：当前操作用户登录ID（RTX账号）
                                        userId：当前操作用户ID]")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]

        public string getMobileCommonData(string param)
        {
            GetProcessCodeMessage gpcm = new GetProcessCodeMessage();
            string errMessage = string.Empty;
            if (base.AuthVerify() == false)
            {
                return base.authVerifyMessage;
            }
            GetProcessInfoParam proParam = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProcessInfoParam>(param);

            B_MonthlyReportAction BRA = new B_MonthlyReportAction();
            BRA.Action = "WebService";
            BRA.Description = param;
            BRA.Operator = "MobileCommonWebService.getMobileCommonData 进入";
            BRA.OperatorTime = DateTime.Now;
            BRA.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);



            //proParam.requestId 这里的requestId 可能是多个值

            string businessID = proParam.requestId.Split(',')[0];
            string ProType = proParam.requestId.Split(',')[1];
            string subPage = proParam.SubPageName;
            MobileServiceBase mobileservice;
            string logID = string.Empty;
            if (string.IsNullOrEmpty(subPage) == false)
            {
                logID = subPage;
                mobileservice = GetMobileServiceForsubPage(subPage);
                #region 获取二级页面信息
                FlowPageShowData flowdata = mobileservice.getSubPageData(proParam.PlanID, proParam.CategoryID, proParam.Approve);
                return Newtonsoft.Json.JsonConvert.SerializeObject(flowdata);
                #endregion

            }
            else if (string.IsNullOrEmpty(businessID) == false)
            {
                logID = logID = subPage; ;
                mobileservice = GetMobileService(businessID, ProType);
                #region 获取流程信息
              
                UserInfo curUser = new UserInfo();
                var vuser = Wanda.SDK.Common.ClientSDKHelper.GetResultContextData<UserSourceSDK.UserInfo>(UserSourceSDK.UserSourceClientSDK.GetUserInfoByUserLoginID(proParam.userLoginId, null));
                
                if (vuser == null)
                {
                    gpcm.status = ConstantWS.error;
                    gpcm.processCode = "";
                    gpcm.requestId = businessID;
                    gpcm.message = string.Format("当前用户：{0} 不存在", proParam.userId);

                }
                else
                {

                    try
                    {
                        curUser.UserID = vuser.UserID;
                        curUser.UserCode = vuser.UserCode;//TODO
                        curUser.UserJobName = vuser.UserJobName;
                        curUser.UserLoginID = vuser.UserLoginID;
                        curUser.UserName = vuser.UserName;
                        curUser.UserOrgID = vuser.UserOrgID;
                        curUser.UserOrgPathID = vuser.UserOrgPathName;
                        curUser.UserOrgPathName = vuser.UserOrgPathName;


                        string processCode = GetProcessCode(businessID, ProType);
                        WorkflowContext wfc = GetProcessCode(businessID, curUser);

                        if (wfc.ProcessInstance.Status == 2 && wfc.CurrentUser.UserLoginID == wfc.ProcessInstance.StartUser.UserLoginID) // 退回时候，不是发起人
                        {
                            gpcm.status = ConstantWS.error;
                            gpcm.processCode = processCode;
                            gpcm.requestId = businessID;
                            gpcm.message = "该待办事项需要重新填报，请在电脑上操作！";

                        }
                        else
                        {
                            // 这里的需要将proParam.requestId ,传递过去，具体的处理 业务ID的数据
                            FlowPageShowData flowdata = mobileservice.getMobileCommonData(proParam.requestId, processCode, wfc);
                            if (string.IsNullOrEmpty(flowdata.requestId) == false)
                            {
                                //Context.Response.Write(jss.Serialize(flowdata));
                                return Newtonsoft.Json.JsonConvert.SerializeObject(flowdata);
                            }
                            else
                            {
                                gpcm.status = ConstantWS.error;
                                gpcm.processCode = processCode;
                                gpcm.requestId = businessID;
                                gpcm.message = string.Format("获取不存在processCode {0},requestId:{1} 不存在processCode", ConstantWS.errorText, businessID);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        gpcm.status = ConstantWS.error;
                        gpcm.message = "发生不可预知的错误：" + ex.Message;
    
                        BRA.Action = "WebService";
                        BRA.Description = gpcm.message;
                        BRA.Operator = "MobileCommonWebService.getMobileCommonData";
                        BRA.OperatorTime = DateTime.Now;
                        BRA.IsDeleted = true;
                        B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

                    }

                }
                #endregion

            }
            else
            {
                gpcm.status = ConstantWS.error;
                gpcm.message = "requestId为空";
                logID = gpcm.message;
            }
           
            BRA.Action = "WebService";
            BRA.Description = Newtonsoft.Json.JsonConvert.SerializeObject(gpcm);
            BRA.Operator = "MobileCommonWebService.getMobileCommonData 序列化成功返回的Json";
            BRA.OperatorTime = DateTime.Now;
            BRA.MonthlyReportID = businessID.ToGuid();
            BRA.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

            return Newtonsoft.Json.JsonConvert.SerializeObject(gpcm);
        }
        /// <summary>
        /// 发送流程审批数据
        /// </summary>
        /// <param name="param"></param>
        [WebMethod(Description = @"发送流程审批数据,
                                    param符合JSON格式的字符串
                                processCode:流程类型编码
                                    requestId：业务流程待办ID
                                    userLoginId：当前操作用户登录ID（RTX账号）
                                    userId：当前操作用户ID
                                    remark：审批意见输入内容
                                    action：审批操作类型
                                    uids：审批操作需要选择的用户ID列表（当需要多选用户时才会发送此参数，其他审批操作无此参数）【可选】
                                    ButtonType,ButtonName,ButtonMethodName,
                                    ButtonDisplayName：统一工作流平台需要4个参数【可选】
                                    ")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]


        public string submitCommonForm(string param)
        {
            B_MonthlyReportAction BRA = new B_MonthlyReportAction();
            BRA.Action = "开始 submitCommonForm ";
            BRA.Description = param;
            BRA.Operator = "MobileCommonWebService.submitCommonForm";
            BRA.OperatorTime = DateTime.Now;
            BRA.IsDeleted = true;
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
            
            string errMessage = string.Empty; ;
            if (base.AuthVerify() == false)
            {
                return base.authVerifyMessage;
            }
            
            SumbitProcessParam proParam = Newtonsoft.Json.JsonConvert.DeserializeObject<SumbitProcessParam>(param);

            ErrMessage emResult = new ErrMessage();
            
            string businessID = proParam.requestId.Split(',')[0];
            string ProType = proParam.requestId.Split(',')[1];

            string strResult = string.Empty;
            string strAction = string.Empty;
            #region 参数验证
            if (string.IsNullOrEmpty(businessID))
            {
                strResult = "缺少参数：requestId,";
            }
            if (string.IsNullOrEmpty(proParam.userLoginId))
            {
                strResult += "缺少参数：userLoginId,";
            }
            if (string.IsNullOrEmpty(proParam.ButtonMethodName))
            {
                strResult += "缺少参数：action,";
            }
            else
            {
                strAction = proParam.ButtonMethodName;
            }
            if (strResult.Length > 0)
            {
                strResult = strResult.TrimEnd(',');
            }
            #endregion
            else
            {
                string[] uids;
                if (string.IsNullOrEmpty(proParam.uids))
                {
                    uids = new string[] { };
                }
                else
                {
                    uids = proParam.uids.Split(',');
                }



                UserInfo user = new UserInfo();
                var vuser = Wanda.SDK.Common.ClientSDKHelper.GetResultContextData<UserSourceSDK.UserInfo>(UserSourceSDK.UserSourceClientSDK.GetUserInfoByUserLoginID(proParam.userLoginId, null));
                if (vuser == null)
                {
                    strResult = "不存在用户：" + proParam.userLoginId;
                }
                else
                {
                    user.UserID = vuser.UserID;
                    user.UserCode = vuser.UserCode;//TODO
                    user.UserJobName = vuser.UserJobName;
                    user.UserLoginID = vuser.UserLoginID;
                    user.UserName = vuser.UserName;
                    user.UserOrgID = vuser.UserOrgID;
                    user.UserOrgPathID = vuser.UserOrgPathName;
                    user.UserOrgPathName = vuser.UserOrgPathName;
                    
                    MobileServiceBase mobileservice = GetMobileService(businessID, ProType);
                    
                    strResult = mobileservice.ExecProcess(user, uids, proParam);
                }
            }


            if (string.IsNullOrEmpty(strResult))
            {
                emResult.status = ConstantWS.success;
                emResult.message = ConstantWS.successText;
            }
            else
            {
                emResult.status = ConstantWS.error;
                emResult.message = strResult;
            }



            BRA.Action = "结束 submitCommonForm ：businessID:" + businessID;
            BRA.Description = strResult;
            BRA.Operator = "MobileCommonWebService.submitCommonForm";
            BRA.OperatorTime = DateTime.Now;
            BRA.IsDeleted = true;
            BRA.MonthlyReportID = businessID.ToGuid();
            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

            return Newtonsoft.Json.JsonConvert.SerializeObject(emResult);
        }
        [WebMethod(Description = @"搜索用户列表,
                                    param符合JSON格式的字符串
                                    username：搜索的用户姓名
                                    pageNo：当前页码
                                    pageSize：每页数据条数
                                    processCode:流程类型编码
                                    requestId：业务流程待办ID
                                    userLoginId：当前操作用户登录ID（RTX账号）
                                    userId：当前操作用户ID
                                    ")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]

        public string searchPageUser(string param)
        {
            
            //NLogHelper.Log.Info(param,
            //        NLogHelper.MakeLogObj(NLogHelper.BusinessID_Const, "进入MobileCommonWebService"),
            //        NLogHelper.MakeLogObj(NLogHelper.MethodName_Const, "MobileCommonWebService.searchPageUser"));
            GetProcessCodeMessage gpcm = new GetProcessCodeMessage();
            string errMessage = string.Empty; ;
            if (base.AuthVerify() == false)
            {
                return base.authVerifyMessage;
            }
            //获取分布用户信息
            SearchUserParam proParam = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchUserParam>(param);
            string strResult = string.Empty;
            string strAction = string.Empty;
            int pageIndex = 0;
            int pageSize = 10;
            string loginName = string.Empty;
            if (string.IsNullOrEmpty(proParam.pageNo) == false)
            {
                int.TryParse(proParam.pageNo, out pageIndex);
            }
            if (string.IsNullOrEmpty(proParam.pageSize) == false)
            {
                int.TryParse(proParam.pageNo, out pageSize);
            }
            if (string.IsNullOrEmpty(proParam.userLoginId) == false)
            {
                loginName = proParam.userLoginId.Trim();
            }
            try
            {
                var rvUserInfo = Wanda.SDK.Common.ClientSDKHelper.GetResultContextData<List<UserSourceSDK.UserInfo>>(UserSourceSDK.UserSourceClientSDK.GetUserListByKeyword(loginName));

                var userListPage = rvUserInfo.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                SearchUserMessage suMessage = new SearchUserMessage();
                List<User> ltu = new List<User>();
                suMessage.status = ConstantWS.success;
                suMessage.message = ConstantWS.successText;
                suMessage.totalCount = rvUserInfo.Count;
                foreach (var vui in userListPage)
                {
                    ltu.Add(new User()
                    {
                        userId = vui.UserCode
                         ,
                        department = vui.UserOrgPathName
                         ,
                        loginId = vui.UserLoginID
                         ,
                        userName = vui.UserName
                    });
                }
                suMessage.rows = ltu;

                //Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(suMessage));
                return Newtonsoft.Json.JsonConvert.SerializeObject(suMessage);

            }
            catch (Exception ex)
            {
                gpcm.status = ConstantWS.error;
                gpcm.message = "发生不可预知的错误：" + ex.Message;
            }
            //Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(gpcm));
            return Newtonsoft.Json.JsonConvert.SerializeObject(gpcm);
        }

    }
}
