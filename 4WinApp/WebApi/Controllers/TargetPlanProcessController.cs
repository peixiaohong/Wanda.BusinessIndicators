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
using WebApi.Models;
using Lib.Web.Json;
using System.Web;
using Plugin.SSO;

namespace WebApi.Controllers
{
    public class TargetPlanProcessController : ApiController
    {
        ApprovalController ac = new ApprovalController();
        private string VirtualUser = System.Configuration.ConfigurationManager.AppSettings["WF.VirtualUser"];
        private string CurrentUser = WebHelper.GetCurrentLoginUser();
        #region 指标分解审批
        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="OperatorType"></param>
        /// <param name="PrcessStatus"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultContext TargetPlanProcessRequest(string BusinessID, int OperatorType, string PrcessStatus)
        {
            LJTH.BusinessIndicators.Web.AjaxHander.TargetPlanProcessController tp = new LJTH.BusinessIndicators.Web.AjaxHander.TargetPlanProcessController();
            
            try
            {
                HttpContext context = HttpContext.Current;
                SSOClaimsIdentity claimsIdentity = new SSOClaimsIdentity
                {
                    UserName = WebHelper.GetCurrentLoginUser()

                };
                SSOClaimsPrincipal claimsPrincipal = new SSOClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;

                tp.ProcessRequest(context);
                return new ResultContext();
            }
            catch (Exception ex)
            {
                return new ResultContext((int)StatusCodeEnum.isCatch, ex.ToString());
                throw;
            }
        }

        #endregion


    }
}