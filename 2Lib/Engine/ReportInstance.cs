using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Model;
using System.Web.Script.Serialization;

namespace LJTH.BusinessIndicators.Engine
{
    public partial class ReportInstance
    {
        public ReportInstance()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="IsLatestVersion">是否是最新的版本，true：从B表中获取，false：从A表中获取 </param>
        /// <param name="_DataSource">当从B表中获取时，这里有Draft：草稿状态， 或者是审批中状态 </param>
        public ReportInstance(Guid SystemID, int Year, int Month, bool IsLatestVersion = false, string _DataSource = "Draft")
        {
            _SystemID = SystemID;
            FinYear = Year;
            FinMonth = Month;
            DataSource = _DataSource;
            InitialData(IsLatestVersion);
        }



        /// <summary>
        /// 仅上报和审批页面调用
        /// </summary>
        /// <param name="MonthReportID"></param>
        /// <param name="IsLatestVersion">是否是最新的版本，true：从B表中获取，false：从A表中获取</param>
        /// <param name="_DataSource">当从B表中获取时，这里有Draft：草稿状态， 或者是审批中状态 </param>
        public ReportInstance(Guid MonthReportID, bool IsLatestVersion = false, string _DataSource = "Draft")
        {
            _MonthReportID = MonthReportID;
            B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyreport(_MonthReportID);
            _SystemID = report.SystemID;
            FinYear = report.FinYear;
            FinMonth = report.FinMonth;
            DataSource = _DataSource;
            InitialData(IsLatestVersion);

        }

        private string DataSource = string.Empty;

        public Guid _SystemID { get; set; }
        public int FinYear { get; set; }
        public int FinMonth { get; set; }
        public string SummaryTemplate { get; set; }
        public Sys_Config SysConfig { get; set; }
        public MonthlyReport Report { get; set; }
        public List<MonthlyReportDetail> ReportDetails { get; set; }

        [ScriptIgnore]
        public C_System _System { get { return StaticResource.Instance[_SystemID,DateTime.Now]; } }
        [ScriptIgnore]
        public List<C_Target> _Target { get { return StaticResource.Instance.GetTargetList(_SystemID,DateTime.Now).ToList(); } }

        public Guid _MonthReportID { get; set; }



        public List<B_MonthlyReportDetail> LastestMonthlyReportDetails
        {
            get
            {
                if (_LastestMonthlyReportDetails == null || _LastestMonthlyReportDetails.Count <= 0)
                {
                    if (DataSource == "Draft")
                    {
                        _LastestMonthlyReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailListBytDraft(_System.ID, FinYear, FinMonth, _MonthReportID).ToList();
                    }
                    else
                    {
                        _LastestMonthlyReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(_System.ID, FinYear, FinMonth, _MonthReportID).ToList();
                    }
                }
                return _LastestMonthlyReportDetails;
            }
        }private List<B_MonthlyReportDetail> _LastestMonthlyReportDetails = null;

        public B_MonthlyReport LastestMonthlyReport
        {
            get
            {
                if (_LastestMonthlyReport == null)
                {
                    if (_MonthReportID == Guid.Empty)
                    {
                        if (DataSource == "Draft")  //获取草稿状态的
                            _LastestMonthlyReport = B_MonthlyreportOperator.Instance.GetLastMonthlyReportList(_System.ID, FinYear, FinMonth);
                        else // 获取审批中状态下的（修改为审批中和已完成两种状态的）
                            //_LastestMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(_System.ID, FinYear, FinMonth);
                            _LastestMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(_System.ID, FinYear, FinMonth);
                        
                        if (_LastestMonthlyReport != null)
                            _MonthReportID = _LastestMonthlyReport.ID;
                    }
                    else
                    {
                        _LastestMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(_MonthReportID);
                    }
                }
                return _LastestMonthlyReport;
            }
        }private B_MonthlyReport _LastestMonthlyReport = null;

        public List<A_MonthlyReportDetail> ValidatedMonthlyReportDetails
        {
            get
            {
                if (_ValidatedMonthlyReportDetails == null || _ValidatedMonthlyReportDetails.Count <= 0)
                {
                    _ValidatedMonthlyReportDetails = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(_System.ID, FinYear, FinMonth).ToList();
                }
                return _ValidatedMonthlyReportDetails;
            }
        }private List<A_MonthlyReportDetail> _ValidatedMonthlyReportDetails = null;

        public A_MonthlyReport ValidatedMonthlyReport
        {
            get
            {
                if (_ValidatedMonthlyReport == null)
                {
                    if (_MonthReportID == Guid.Empty)
                    {
                        _ValidatedMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(_System.ID, FinYear, FinMonth);

                        if (_ValidatedMonthlyReport != null)
                        {
                            IList<B_MonthlyReport> _BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyreportListByMonthlyReportID(_ValidatedMonthlyReport.ID);
                            if (_BMonthlyReport.Count > 0)
                                _MonthReportID = _ValidatedMonthlyReport.ID;
                        }
                    }

                }
                return _ValidatedMonthlyReport;
            }
        }private A_MonthlyReport _ValidatedMonthlyReport = null;

        #region Private Method
        /// <summary>
        /// Initial the Data Source of MonthReport and MonthlyReportDetail. 
        /// if IsLatestVersion is true, the data source will be B_ table  and WFStatus == Progress.
        /// otherwise, the data source is A_ atble 
        /// </summary>
        /// <param name="IsLatestVersion">if the data source should be  B_ Table</param>
        void InitialData(bool IsLatestVersion)
        {
            if (IsLatestVersion)
            {
                if (LastestMonthlyReport != null)
                {
                    Report = LastestMonthlyReport.ToVModel();
                    if (_MonthReportID == Guid.Empty)
                    {
                        _MonthReportID = Report.ID;
                    }

                }
                ReportDetails = new List<MonthlyReportDetail>();

                if (DataSource == "Draft")
                    ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Draft(_System.ID, FinYear, FinMonth, _MonthReportID);
                else
                    ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Approve(_System.ID, FinYear, FinMonth, _MonthReportID);

                //LastestMonthlyReportDetails.ForEach(P => ReportDetails.Add(P.ToVModel()));
            }
            else
            {
                if (ValidatedMonthlyReport != null)
                {
                    Report = ValidatedMonthlyReport.ToVModel();
                    if (_MonthReportID == Guid.Empty)
                    {
                        _MonthReportID = Report.ID;
                    }
                }
                ReportDetails = new List<MonthlyReportDetail>();
                ReportDetails = A_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Result(_System.ID, FinYear, FinMonth);

                //ValidatedMonthlyReportDetails.ForEach(P => ReportDetails.Add(P.ToVModel()));
            }
        }
        #endregion Private Method

    }
}
