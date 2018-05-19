using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;


namespace LJTH.BusinessIndicators.Engine
{
    public class DefaultLastMonthCompanies : ILastMonthCompanies
    {
        public List<Guid> GetLastMonthCompaniesID(object Lastlist)
        {
            List<A_MonthlyReportDetail> ReportDetailList_A = (List<A_MonthlyReportDetail>)Lastlist;

            List<Guid> Companies = new List<Guid>();


            Companies= ReportDetailList_A.Where(x => x.IsMissTarget == true).GroupBy(x => x.CompanyID).Select(x => x.Key).ToList();

            //foreach (A_MonthlyReportDetail item in ReportDetailList_A)
            //{
            //    if (item.IsMissTarget)
            //    {
            //        if (!Companies.Contains(item.CompanyID))
            //        {
            //            Companies.Add(item.CompanyID);
            //        }
            //    }
            //}

            return Companies;
        }
    }


    public class LastMonthCompanies_SG : ILastMonthCompanies
    {
        public List<Guid> GetLastMonthCompaniesID(object Lastlist)
        {
            List<A_MonthlyReportDetail> ReportDetailList_A = (List<A_MonthlyReportDetail>)Lastlist;

            List<Guid> Companies = new List<Guid>();

            foreach (A_MonthlyReportDetail item in ReportDetailList_A)
            {
                if (item.NAccumulativeDifference <0 && item.IsDelayComplete ==false)
                {
                    if (!Companies.Contains(item.CompanyID))
                    {
                        Companies.Add(item.CompanyID);
                    }
                }
            }

            return Companies;
        }
    }


    public class LastMonthCompanies_Directly : ILastMonthCompanies
    {
        public List<Guid> GetLastMonthCompaniesID(object Lastlist)
        {
            List<A_MonthlyReportDetail> ReportDetailList_A = (List<A_MonthlyReportDetail>)Lastlist;

            List<Guid> Companies = new List<Guid>();

            foreach (A_MonthlyReportDetail item in ReportDetailList_A)
            {
                if (item.IsMissTarget)
                {
                    if (!Companies.Contains(item.TargetID))
                    {
                        Companies.Add(item.TargetID);
                    }
                }
            }

            return Companies;
        }
    }



}
