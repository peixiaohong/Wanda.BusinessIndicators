using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.DAL
{
    class V_MonthlyreportdetailAdapter: AppBaseCompositionAdapterT<MonthlyReportDetail>
    {
        public static V_MonthlyreportdetailAdapter Instance = new V_MonthlyreportdetailAdapter();

        public List<MonthlyReportDetail> GetList(int FinYear, int FinMonth, Guid SystemID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            if (FinYear > 0)
            {
                where.AppendItem("FinYear", FinYear);
            }
            if (FinMonth > 0)
            {
                where.AppendItem("FinMonth", FinMonth);
            }

            if (SystemID != Guid.Empty)
            {
                where.AppendItem("SystemID", SystemID);
            }


           return base.GetList(where, "FinMonth ASC");
        }


        public List<MonthlyReportDetail> GetList(int FinYear, int FinMonth, Guid SystemID ,Guid TargetID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            if (FinYear > 0)
            {
                where.AppendItem("FinYear", FinYear);
            }
            if (FinMonth > 0)
            {
                where.AppendItem("FinMonth", FinMonth);
            }

            if (SystemID!=Guid.Empty)
            {
                where.AppendItem("SystemID", SystemID);
            }

            if (TargetID !=Guid.Empty)
            {
                where.AppendItem("TargetID", TargetID);
            }


            return base.GetList(where, "FinMonth ASC");
        }

    }
}
