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

namespace DataCalculate
{
    class Program
    {
        static void Main(string[] args)
        {
            string systemId = "2EB7465B-95DA-450C-AA2F-9AF04E06D9BC";
            var r=  StaticResource.Instance.GetSystem_Regional(systemId.ToGuid());
            Console.WriteLine(JsonConvert.SerializeObject(StaticResource.Instance.GetReportDateTime()));
            Console.ReadKey();
            return;


            Console.Write("Please select process A or B model (default is B): ");
            string P=Console.ReadLine();
            if (P.ToLower().Trim() == "a")
            {
                Console.WriteLine("Input Month....");
                string FinMonth = Console.ReadLine();
                int m = 0;
                int.TryParse(FinMonth, out m);
                if (m <= 0) m = DateTime.Now.AddMonths(-1).Month;
                Console.WriteLine("Input Year....");
                string FinYear = Console.ReadLine();
                int y = 0;
                int.TryParse(FinYear, out y);
                if (y <= 0) y = DateTime.Now.AddMonths(-1).Year;
                Console.WriteLine("Processing A model list....");
                ProcessA(m,y);
            }
            else
            {
                Console.WriteLine("Processing B model list....");
                ProcessB();
            }
        }

        static void ProcessB()
        {
            List<B_MonthlyReportDetail> list = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList().ToList().OrderBy(P=>(P.FinYear*100+P.FinMonth)).ToList();

            Console.WriteLine(string.Format("There are {0} records", list.Count));
            int Index = list.Count;
            int Success = 0;
            int InitialDataError = 0;
            List<Exception> exps = new List<Exception>();
            foreach (B_MonthlyReportDetail RptDetail in list)
            {
                try
                {
                    B_MonthlyReportDetail res = TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail,false);
                    B_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(res);
                    Success++;
                }
                catch (Exception exp)
                {
                    if (exp is InitialDataException)
                    {
                        InitialDataError++;
                        Console.WriteLine(string.Format("Processing B_MonthlyReportDetail(ID='{0}') error, Message:{1}", RptDetail.ID.ToString(), exp.ToString()));
                        Console.WriteLine();
                    }
                    else
                    {
                        if (!exps.Exists(e => e.Message == exp.Message && e.StackTrace == exp.StackTrace))
                        {
                            exps.Add(exp);
                            Console.WriteLine(string.Format("Processing B_MonthlyReportDetail(ID='{0}') error, Message:{1}", RptDetail.ID.ToString(), exp.ToString()));
                            Console.WriteLine();
                        }
                    }
                }
                Index--;
                if (Index % 200 == 0 && Index > 0)
                {
                    Console.WriteLine(string.Format("There are {0} records", Index));
                }
            }
            Console.WriteLine();
            Console.WriteLine(string.Format("There are {0} records", list.Count));
            Console.WriteLine(string.Format("There are {0} Success", Success));
            Console.WriteLine(string.Format("There are {0} InitialDataException", InitialDataError));

            Console.Read();
        }

        static void ProcessA(int FM,int FY)
        {
            List<A_MonthlyReportDetail> list = A_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList().ToList().FindAll(D=>D.FinMonth==FM && D.FinYear==FY).OrderBy(P => (P.FinYear * 100 + P.FinMonth)).ToList();

            Console.WriteLine(string.Format("There are {0} records", list.Count));
            int Index = list.Count;
            int Success = 0;
            int InitialDataError = 0;
            List<Exception> exps = new List<Exception>();
            foreach (A_MonthlyReportDetail RptDetail in list)
            {
                try
                {
                    A_MonthlyReportDetail res = TargetEvaluationEngine.TargetEvaluationService.Calculation(RptDetail.ToVModel().ToBModel(),false).ToVModel().ToAModel();
                    A_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportdetail(res);
                    Success++;
                }
                catch (Exception exp)
                {
                    if (exp is InitialDataException)
                    {
                        InitialDataError++;
                        Console.WriteLine(string.Format("Processing B_MonthlyReportDetail(ID='{0}') error, Message:{1}", RptDetail.ID.ToString(), exp.ToString()));
                        Console.WriteLine();
                    }
                    else
                    {
                        if (!exps.Exists(e => e.Message == exp.Message && e.StackTrace == exp.StackTrace))
                        {
                            exps.Add(exp);
                            Console.WriteLine(string.Format("Processing B_MonthlyReportDetail(ID='{0}') error, Message:{1}", RptDetail.ID.ToString(), exp.ToString()));
                            Console.WriteLine();
                        }
                    }
                }
                Index--;
                if (Index % 200 == 0 && Index > 0)
                {
                    Console.WriteLine(string.Format("There are {0} records", Index));
                }
            }
            Console.WriteLine();
            Console.WriteLine(string.Format("There are {0} records", list.Count));
            Console.WriteLine(string.Format("There are {0} Success", Success));
            Console.WriteLine(string.Format("There are {0} InitialDataException", InitialDataError));

            Console.Read();
        }
    }
}
