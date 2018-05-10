using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using LJTH.BusinessIndicators.Common;
using Lib.Core;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.ViewModel;

using BPF.Workflow.Client;
using WebApi.Models;
using LJTH.BusinessIndicators.Web.AjaxHander;
using Plugin.SSO;
using System.Web;

namespace WebApi.Controllers
{
    public class ApprovalController : ApiController
    {
        private string VirtualUser = System.Configuration.ConfigurationManager.AppSettings["WF.VirtualUser"];
        private string CurrentUser = WebHelper.GetCurrentLoginUser();
       
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="strProType"></param>
        /// <param name="ExecuteType"></param>
        /// <param name="OperatorType"></param>
        /// <param name="PrcessStatus"></param>
        [HttpGet]
        public ResultContext MonthProcessRequest(string BusinessID, string strProType, string ExecuteType, int OperatorType, string PrcessStatus)
        {
            ProcessController pc = new ProcessController();
            pc.BusinessID = BusinessID;
            pc.ProType = strProType;
            pc.ExecType = ExecuteType;
            pc.OperatorType = OperatorType;
            try
            {
                //写入用户信息
                HttpContext context = HttpContext.Current;
                SSOClaimsIdentity claimsIdentity = new SSOClaimsIdentity
                {
                    UserName = WebHelper.GetCurrentLoginUser()

                };
                SSOClaimsPrincipal claimsPrincipal = new SSOClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
                //业务处理
                pc.DisposeBusinessData();
                
                //执行按钮事件的处理
                pc.ExecutionBusinessData();
                return new ResultContext();
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
           

        }
        
    }
}