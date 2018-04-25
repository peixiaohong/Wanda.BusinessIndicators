using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web;
using LJTH.BusinessIndicators.BLL.BizBLL;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class TargetSimpleReportController : BaseController
    {
        [LibAction]
        public List<C_System> GetSystemListByConfigID(string cid)
        {
            List<C_System> list = new List<C_System>();
            Guid gid = Guid.Empty;
            Guid.TryParse(cid, out gid);
            if (gid != Guid.Empty)
                list = C_SystemOperator.Instance.GetSystemListByConfigID(gid).ToList();
            return list;
        }



        /// <summary>
        /// 根据系统，获取系统下的指标
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [LibAction]
        public List<string> GetTargetList(string SysID)
        {
            if (!string.IsNullOrEmpty(SysID))
            {

                String[] str = SysID.Split(',');
                List<string> list_str = str.ToList();
                string Sys_str = "'" + string.Join("','", list_str.Select(S => S.ToString()).ToList()) + "'";

                var lsit = C_TargetOperator.Instance.GetTargetList(Sys_str, DateTime.Now).Distinct().ToList();

                return lsit;
            }
            else
                return new List<string>();
        }



        [LibAction]
        public C_ReportTime GetReportTime()
        {
               return   C_ReportTimeOperator.Instance.GetReportTime();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="SysIDs">系统ID：用，分割</param>
        /// <param name="FinYears">年：用，分割</param>
        /// <param name="Targets">指标名称：中文，用，分割</param>
        /// <param name="DataType">数据类型：实际数，指标数，完成率 ，差额</param>
        /// <param name="IsCurrent">当月数，累计数</param>
        /// <returns></returns>
        [LibAction]
        public List<ComprehensiveReportVModel> GetComprehensiveReport(string SysIDs, string FinYears, string Targets, string DataType, string IsCurrent)
        {
            String[] str = SysIDs.Split(',');
            List<string> list_str = str.ToList();
            string a =string.Join(",", list_str.Select(S => S.ToString()).ToList()) ;


            List<int> list_year = JsonHelper.Deserialize<List<int>>(FinYears);
            string b = string.Join(",", list_year.Select(S => S.ToString()).ToList()) ;


            List<string> list_target = JsonHelper.Deserialize<List<string>>(Targets);
            string c =  string.Join(",", list_target.Select(S => S.ToString()).ToList()) ;

            var d = DataType;
            var e = IsCurrent;

            //获取下，当前年份，从数据库中获取

            var f = C_ReportTimeOperator.Instance.GetReportTime().ReportTime.Value.Year;

            var list = V_ComprehensiveReportFormsOperator.Instance.GetList(a, b, c, d, e, f);




            return list;



        }


        /// <summary>
        /// 上报系统下拉框数据加载
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public Object GetSystemInfo()
        {
            try
            {
                var data = S_OrganizationalActionOperator.Instance.GetUserSystemData(base.CurrentUserName);
                var success = 1;
                return new
                {
                    Data = data,
                    Success = success,
                    Message = "查询数据没有问题"
                };
            }
            catch (Exception e)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// 获取上边年下拉框
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public object GetYear()
        {
            List<int> Year = new List<int>();
            for (int i = -10; i < 5; i++)
            {
                Year.Add(DateTime.Now.Year + i);
            }
            return new
            {
                Data = Year,
                Success = 1,
                NowYear = DateTime.Now.Year,
                NowMonth = DateTime.Now.Month,
                Message = "查询数据没有问题"
            };
        }

        /// <summary>
        /// 获取页面列表数据
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public object GetListData(string systemID,string year,string month)
        {
            int finYear = DateTime.Now.Year, finMonth = DateTime.Now.Month;
            if (year != "0")
            {
                finYear = Convert.ToInt32(year);
            }
            if (month != "0")
            {
                finMonth = Convert.ToInt32(month);
            }
            try
            {
                var data = V_ComprehensiveReportFormsOperator.Instance.GetComprehensiveReportData(systemID, finYear, finMonth, base.CurrentUserName);
                return new
                {
                    Data=data,
                    Success=1,
                    Message="查询成功"
                };
            }
            catch (Exception ex)
            {

                return new
                {
                    Data = "",
                    Success = 0,
                    Message =ex.Message
                };
            }
        }
    }
}
