using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BPF.Workflow.Client;
using BPF.Workflow.Object;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Web.MobileCommon;
using BPF.Workflow.Client;
using BPF.Workflow.Object;

namespace LJTH.BusinessIndicators.Web.MobileCommon
{
    /// <summary>
    /// BaseWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://m.LJTH.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class BaseWebService : System.Web.Services.WebService
    {

        private string configUserID = System.Configuration.ConfigurationManager.AppSettings["MobileUser"];
        private string configPwd = System.Configuration.ConfigurationManager.AppSettings["MobilePWD"];

        protected string authVerifyMessage = "{ \"status\": \"error’\", \"message\": \"头信息验证失败\"}";
        
        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="ProcessCode"></param>
        /// <returns></returns>
        protected WorkflowContext GetProcessCode(string BusinessID, UserInfo user)
        {
            string strResult = string.Empty;
            var process = WFClientSDK.GetProcess(null, BusinessID, user);
            B_MonthlyReportAction BRA = new B_MonthlyReportAction();

            if (process == null)
            {
                //BRA.Action = "WebService";
                //BRA.Description = string.Format("获取流程FlowID:{0} 的Code时process为null", BusinessID);
                //BRA.Operator = "MobileCommonWebService.GetProcessCode";
                //BRA.OperatorTime = DateTime.Now;
                //B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

            }
            if (process.ProcessInstance == null)
            {
                //BRA.Action = "WebService";
                //BRA.Description = string.Format("获取流程FlowID:{0} 的Code时process.ProcessInstance为null", BusinessID);
                //BRA.Operator = "MobileCommonWebService.GetProcessCode";
                //BRA.OperatorTime = DateTime.Now;
                //B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

            }
            
            return process;
        }

        /// <summary>
        /// 获取流程代码
       
        /// <returns></returns>
        protected string GetProcessCode(string BusinessID , string ProType = null)
        {
            string strResult = string.Empty;

            try
            {
                //这里单独判断下项目公司的流程，
                if (!string.IsNullOrEmpty(ProType)) //
                {
                    var approval = B_SystemBatchOperator.Instance.GetSystemBatch(BusinessID.ToGuid());
                    strResult = System.Configuration.ConfigurationManager.AppSettings["MobileWFKey"];  // 去Web.config 获取，这个虚拟的流程，没有配置
                }
                else
                {
                    try
                    {
                        //月报
                        var approval = B_MonthlyreportOperator.Instance.GetMonthlyreport(BusinessID.ToGuid());
                        strResult = StaticResource.Instance[approval.SystemID, DateTime.Now].Configuration.Element("ProcessCode").Value;
                    }
                    catch (Exception)
                    {
                        //分解指标的
                        var approval = B_TargetplanOperator.Instance.GetTargetplan(BusinessID.ToGuid());
                        strResult = StaticResource.Instance[approval.SystemID, DateTime.Now].Configuration.Element("ProcessCode").Value + "-ZB";
                    }
                }

            } catch (Exception ex )
            {
                B_MonthlyReportAction BRA = new B_MonthlyReportAction();

                BRA.Action = "WebService";
                BRA.Description = ex.ToString();
                BRA.Operator = "GetProcessCode(string BusinessID) ";
                BRA.OperatorTime = DateTime.Now;
                BRA.MonthlyReportID = BusinessID.ToGuid();
                BRA.IsDeleted = true;
                B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
            }

            return strResult;
        }
        /// <summary>
        /// 获取手机服务类
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <returns></returns>
        public MobileServiceBase GetMobileService(string BusinessID ,string ProType = null)
        {
            string processCode = GetProcessCode(BusinessID, ProType);
            MobileServiceBase mobileService ;

            if (!string.IsNullOrEmpty(processCode))
            {
                if (processCode.Substring(processCode.Length - 3, 3).ToUpper() == "-ZB".ToUpper())
                {
                    mobileService = new TargetApprovalService();//这是指标的
                }
                else
                {
                    mobileService = new MonthlyApprovalService(); // 这是月报的
                }
            }
            else
            {
                mobileService = null;
            }
            return mobileService;
            
        }

        public MobileServiceBase GetMobileServiceForsubPage(string SubPage)
        {
            //MobileServiceBase mobileService;
            //switch (SubPage.ToLower())
            //{
            //    case "inspectscoringquery.aspx":
            //    default:
            //        mobileService = new InspectSummaryAuditService();
            //        break;
            //}
            //return mobileService;
            return null;
        }

        /// <summary>
        /// 获取url参数 ， 这里获取2个参数
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public string GetBusinessIDFromUrl(string Url)
        {
            string strResult = string.Empty;
            string strResult2 = string.Empty;

            int post = Url.IndexOf("BusinessID=");
            if (post > -1)
            {
                strResult = Url.Substring(post + "BusinessID = ".Length - 2);
            }

            int post2 = Url.IndexOf("ProType=");
            int post3 = Url.IndexOf("&");
            if (post2 > -1 && post3 > -1)
            {
                strResult2 = Url.Substring(post2, post3 - post2).Replace("ProType=", "");
            }

            return strResult+","+ strResult2;
        }

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <returns></returns>
        public bool AuthVerify()
        {
            bool blResult = false;
          
            B_MonthlyReportAction BRA = new B_MonthlyReportAction();
            
            HttpContext context = HttpContext.Current;
            IEnumerator headers = context.Request.Headers.GetEnumerator();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            while (headers.MoveNext())
            {
                string header = headers.Current.ToString();
                
                BRA.Action = "WebService";
                BRA.Description = "获取授权信息,SOAP头：" + header;
                BRA.Operator = "AuthVerify()";
                BRA.OperatorTime = DateTime.Now;
                BRA.IsDeleted = true;
                B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

                if ("authorization".Equals(header.ToLower()))
                {
                    string auth = context.Request.Headers.Get(header);
                    auth = auth.Substring(6);
                    string pwd = string.Format("{0}:{1}", configUserID, configPwd);
                    string encodePWD = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(pwd));
                    if (encodePWD.Equals(auth))
                    {
                        //BRA.Action = "WebService";
                        //BRA.Description = "获取授权信息,SOAP权限验证成功";
                        //BRA.Operator = "AuthVerify()";
                        //BRA.OperatorTime = DateTime.Now;
                        //B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);

                        blResult = true;
                        break;
                    }
                }

            }
            if (!blResult)
            {
                BRA.Action = "WebService";
                BRA.Description = "获取授权信息,SOAP权限验证失败：" + authVerifyMessage;
                BRA.Operator = "AuthVerify()";
                BRA.OperatorTime = DateTime.Now;
                BRA.IsDeleted = true;
                B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(BRA);
            }
            return blResult;
        }
    }
}
