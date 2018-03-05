using Lib.Core;
using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Engine.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;


namespace Wanda.BusinessIndicators.Web.AjaxHandler
{
    public class TargetPlanDetailController : BaseController
    {
        /// <summary>
        /// 计划指标
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<DictionaryVmodel> GetTargetPlanDetail(string strSystemID, string strFinYear, string strTargetPlanID, bool IsLatestVersion)
        {
            Guid TargetPlanID=Guid.Empty;
            if (!string.IsNullOrEmpty(strTargetPlanID))
            {
                TargetPlanID = strTargetPlanID.ToGuid();
            }
            
            List<DictionaryVmodel> List = (List<DictionaryVmodel>)TargetPlanEngine.TargetPlanEngineService.GetTargetPlanSource(strSystemID.ToGuid(), int.Parse(strFinYear), TargetPlanID, IsLatestVersion);
            return List;
        }

        /// <summary>
        /// 计划指标(新的)
        /// </summary>
        /// <param name="strFinYear"></param>
        /// <param name="strSystemID"></param>
        /// <param name="strTargetPlanID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [LibAction]
        public List<TargetPlanDetailVList> GetSumTargetDetail(string strFinYear, string strSystemID, string strTargetPlanID)
        {
            Guid TargetPlanID = Guid.Empty;
            if (!string.IsNullOrEmpty(strTargetPlanID))
            {
                TargetPlanID = strTargetPlanID.ToGuid();
            }
            var List = A_TargetplandetailOperator.Instance.GetSumTargetDetailApprove(int.Parse(strFinYear), strSystemID.ToGuid(), TargetPlanID);
            
            return List;
        }

        /// <summary>
        /// 获取指标
        /// </summary>
        /// <param name="SysID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        [LibAction]
        public List<C_Target> GetVerTargetList(string strTargetPlanID)
        {
            Guid TargetPlanID = Guid.Empty;
            if (!string.IsNullOrEmpty(strTargetPlanID))
            {
                TargetPlanID = strTargetPlanID.ToGuid();
            }
            var Targetplan = B_TargetplanOperator.Instance.GetTargetplan(TargetPlanID);
            List<C_Target> result = C_TargetOperator.Instance.GetTargetList(Targetplan.SystemID, Targetplan.CreateTime).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), result[i].TargetType);
            }
            return result;
        }


        [LibAction]
        public B_TargetPlan GetTargetPlanByID(string ID)
        {
            B_TargetPlan _BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByID(Guid.Parse(ID));
            return _BTargetPlan;
        }
    }
}
