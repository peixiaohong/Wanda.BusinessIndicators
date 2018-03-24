using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    class DefaultDisplayRate : IGetDisplayRate
    {
        /// <summary>
        /// 完成率算式
        /// </summary>
        /// <param name="Vmodel">月报实体</param>
        /// <returns>完成率</returns>
        public string GetDisplayRate(object Vmodel)
        {
            MonthlyReportDetail mrd = (MonthlyReportDetail)Vmodel;
            string strDisplayRate = "";
            C_Target target = null;
            List<C_Target> listTarget = StaticResource.Instance.GetTargetList(mrd.SystemID, mrd.CreateTime).ToList();
            if (listTarget.Where(p => p.ID == mrd.TargetID).ToList().Count > 0)
            {
                target = listTarget.Where(p => p.ID == mrd.TargetID).ToList()[0];
            }
            if (target != null)
            {
                switch (target.TargetType)
                {
                    case 1:
                    case 3:
                        strDisplayRate = CalculateRevenue(mrd.NPlanAmmount, mrd.NActualAmmount);
                        break;
                    case 2:
                        strDisplayRate = CalculateExpend(mrd.NPlanAmmount, mrd.NActualAmmount);
                        break;
                }
            }
            return strDisplayRate;
        }
        /// <summary>
        /// 收入类和其他类算式
        /// </summary>
        /// <param name="NPlanAmmount">计划数</param>
        /// <param name="NActualAmmount">实际数</param>
        /// <returns>完成率</returns>
        public string CalculateRevenue(decimal NPlanAmmount, decimal NActualAmmount)
        {
            string strRevenueDisplayRate = "";

            if (NPlanAmmount > 0 && NActualAmmount >= 0)//A>0,B≥0:B/A
            {
                strRevenueDisplayRate = ((NActualAmmount / NPlanAmmount)*100).ToString();
            }
            else if (NPlanAmmount == 0 && NActualAmmount > 0)//A=0,B>0:超计划B万元
            {
                strRevenueDisplayRate = "超计划" + NActualAmmount + "万元";
            }
            else if (NPlanAmmount > 0 && NActualAmmount < 0)//A>0,B<0:增亏|(B-A)/A|%
            {
                strRevenueDisplayRate="增亏"+(Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount)*100).ToString()+"%";
            }
            else if (NPlanAmmount < 0 && NActualAmmount >= 0)//A<0,B≥0:减亏|(B-A)/A|%
            {
                strRevenueDisplayRate = "减亏" + (Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount) * 100).ToString() + "%";
            }
            else if (NPlanAmmount < 0 && NPlanAmmount < NActualAmmount && NActualAmmount < 0)//A<0,A<B<0:减亏|(B-A)/A|%
            {
                strRevenueDisplayRate = "减亏" + (Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount) * 100).ToString() + "%";
            }
            else if (NPlanAmmount < 0 && NActualAmmount < 0 && NActualAmmount < NPlanAmmount)//A<0,B<0&&B<A:增亏|(B-A)/A|%
            {
                strRevenueDisplayRate = "增亏" + (Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount) * 100).ToString() + "%";
            }
            else if (NPlanAmmount == 0 && NActualAmmount > 0)//A=0,B>0:减亏B万元
            {
                strRevenueDisplayRate = "减亏" + NActualAmmount + "万元";
            }
            else if (NPlanAmmount == 0 && NActualAmmount < 0)//A=0,B<0:增亏B万元
            {
                strRevenueDisplayRate = "增亏" + NActualAmmount + "万元";
            }

            return strRevenueDisplayRate;
        }
        /// <summary>
        /// 支出算式
        /// </summary>
        /// <param name="NPlanAmmount">计划数</param>
        /// <param name="NActualAmmount">实际数</param>
        /// <returns>完成率</returns>
        public string CalculateExpend(decimal NPlanAmmount, decimal NActualAmmount)
        {
            string strExpentDisplayRat = "";
            if (NPlanAmmount > 0 && NActualAmmount > 0 && NActualAmmount > NPlanAmmount)
            {
                strExpentDisplayRat = "超支" + (Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount) * 100).ToString() + "%";
            }
            else if (NPlanAmmount > 0 && NActualAmmount >= 0 && NActualAmmount < NPlanAmmount)
            {
                strExpentDisplayRat = "节约" + (Math.Abs((NActualAmmount - NPlanAmmount) / NPlanAmmount) * 100).ToString() + "%";
            }
            else {
                strExpentDisplayRat = "超计划" + NActualAmmount + "万元";
            }
            return strExpentDisplayRat;
        }

    }


}
