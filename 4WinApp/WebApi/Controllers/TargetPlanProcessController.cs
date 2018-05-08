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

            tp.BusinessID = BusinessID;
            try
            {
                Guid ID = Guid.Empty;
                Guid.TryParse(BusinessID, out ID);
                if (string.IsNullOrEmpty(BusinessID))
                {
                    throw new Exception("BusinessID is null!");
                }
                else if (ID == Guid.Empty)
                    throw new Exception("系统编码错误");
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
                if (PrcessStatus != "Approved")
                {
                    tp.OnProcessExecute(PrcessStatus, OperatorType);
                }
                else
                {
                    //审批结束，调用这个
                    tp.OnProecssCompleted();
                }
                //处理数据
                tp.DisposeBusinessData(OperatorType, WebHelper.GetCurrentLoginUser());
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