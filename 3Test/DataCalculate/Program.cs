using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using Newtonsoft.Json;
using BPF.Workflow.Client.Tools;

namespace DataCalculate
{
    class Program
    {
        static void Main(string[] args)
        {
            OnProecssCompleted("b564d96a-57e3-4e96-a003-db9be1273406");
              Console.ReadKey();
            return;
            
        }
        public static void OnProecssCompleted(string BusinessID)
        {
            B_TargetPlan rpt = B_TargetplanOperator.Instance.GetTargetPlanByID(BusinessID.ToGuid());
            List<B_TargetPlanDetail> rptDetailList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(BusinessID.ToGuid()).ToList();

            rpt.WFStatus = "Approved";
            if (!B_TargetplanOperator.Instance.HasDefaultVersion(rpt.SystemID, rpt.FinYear))
            {
                rpt.VersionDefault = 1;
            }
            B_TargetplanOperator.Instance.UpdateTargetplan(rpt);


            //A_TargetPlan rptA = null;

            List<A_TargetPlanDetail> rptTempDetailList = new List<A_TargetPlanDetail>();

            //List<A_TargetPlanDetail> rptADetailList = null;

            A_TargetplanOperator.Instance.AddTargetplan(
                new A_TargetPlan()
                {
                    ID = rpt.ID,
                    VersionName = rpt.VersionName,
                    VersionDefault = rpt.VersionDefault,
                    FinYear = rpt.FinYear,
                    Description = rpt.Description,
                    SystemID = rpt.SystemID,
                    Status = 5,
                    CreateTime = DateTime.Now
                });
            rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));
            A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);


            #region 原有逻辑
            //A主表的的数据
            //rptA = A_TargetplanOperator.Instance.GetTargetplanList(rpt.SystemID, rpt.FinYear).FirstOrDefault();

            ////A表明细数据
            //rptADetailList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(rpt.SystemID, rpt.FinYear).ToList();


            ////判断当月主表是否是null
            //if (rptA == null)
            //{
            //    A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

            //    //判断A 明细
            //    if (rptADetailList.Count == 0)
            //    {
            //        #region 明细数据

            //        //将B 表数据添加到 A表中
            //        rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

            //        #endregion
            //    }
            //    else
            //    {

            //        //删除A表明细的所有数据
            //        A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


            //        #region 明细数据

            //        //将B 表数据添加到 A表中
            //        rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

            //        #endregion
            //    }

            //    //添加明细数据
            //    A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            //}
            //else
            //{
            //    //上来删除主表的ID
            //    A_TargetplanOperator.Instance.DeleteModel(rptA);

            //    //新增B表的主表数据
            //    A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

            //    //B表转换到A表
            //    if (rptADetailList.Count == 0)
            //    {
            //        #region 明细数据

            //        //将B 表数据添加到 A表中
            //        rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

            //        #endregion
            //    }
            //    else
            //    {
            //        //删除A表明细的所有数据
            //        A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


            //        #region 明细数据

            //        //将B 表数据添加到 A表中
            //        rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

            //        #endregion
            //    }

            //    //添加明细数据
            //    A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            //}
            #endregion
        }

    }
}
