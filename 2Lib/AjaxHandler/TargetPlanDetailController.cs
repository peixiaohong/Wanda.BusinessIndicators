using Lib.Core;
using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Engine.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using System.Transactions;
using Lib.Data;
using System.Web;
using Wanda.Platform.WorkFlow.ClientComponent;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
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
            Guid TargetPlanID = Guid.Empty;
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

        [LibAction]
        public string UpdateVersionDefault(string ID, string SystemID, string Year)
        {
            int res = 0;
            Guid result = Guid.Empty;
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                res = B_TargetplanOperator.Instance.UpdateVersionDefault(Guid.Parse(ID));

                #region 在日志表中添加该次操作的日志
                string userName = WebHelper.GetCurrentLoginUser();

                B_TargetPlanAction TargetAction = new B_TargetPlanAction()
                {
                    TargetPlanID = ID.ToGuid(),
                    SystemID = SystemID.ToGuid(),
                    FinYear = int.Parse(Year),
                    Action = "变更默认版本",
                    Operator = userName,
                    OperatorTime = DateTime.Now
                };
                result = B_TargetPlanActionOperator.Instance.AddTargetPlanAction(TargetAction);

                #endregion
                scope.Complete();
            }
            if (res > 0)
                return "true";
            else
                return "false";
        }

        [LibAction]
        public string DeleteTargetPlan(string ID,string SystemID,string Year)
        {
            int res = 0;
            Guid result = Guid.Empty;
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                //删除a表数据，物理删除
                A_TargetplanOperator.Instance.DeletePlan(Guid.Parse(ID));
                A_TargetplandetailOperator.Instance.DeletePlanDetail(Guid.Parse(ID));
                //改b表版本结束时间
                B_TargetPlan BtargetPlan = B_TargetplanOperator.Instance.GetTargetplan(Guid.Parse(ID));
                BtargetPlan.Versionend = DateTime.Now;
                B_TargetplanOperator.Instance.UpdateTargetplan(BtargetPlan);

                #region 在日志表中添加该次操作的日志


                B_TargetPlanAction TargetAction = new B_TargetPlanAction()
                    {
                        TargetPlanID = ID.ToGuid(),
                        SystemID = SystemID.ToGuid(),
                        FinYear = int.Parse(Year),
                        Action = "禁用",
                        Operator = HttpContext.Current.GetUserName(),
                        OperatorTime = DateTime.Now
                    };
                    result = B_TargetPlanActionOperator.Instance.AddTargetPlanAction(TargetAction);

                #endregion
                scope.Complete();
                res = 1;
            }
            if (res > 0)
                return "true";
            else
                return "false";
        }
    }
}
