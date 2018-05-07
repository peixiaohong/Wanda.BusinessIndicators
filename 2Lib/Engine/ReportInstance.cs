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
        /// <param name="IsAll">是否获取全部月报数据，月报查询true,月报上报false </param>
        public ReportInstance(Guid SystemID, int Year, int Month, bool IsLatestVersion = false, string _DataSource = "Draft", bool IsAll = false)
        {
            _SystemID = SystemID;
            FinYear = Year;
            FinMonth = Month;
            DataSource = _DataSource;
            InitialData(IsLatestVersion, IsAll);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="TargetPlanID">分解指标计划ID</param>
        /// <param name="IsLatestVersion">是否是最新的版本，true：从B表中获取，false：从A表中获取 </param>
        /// <param name="_DataSource">当从B表中获取时，这里有Draft：草稿状态， 或者是审批中状态 </param>
        /// <param name="IsAll">是否获取全部月报数据，月报查询true,月报上报false </param>
        public ReportInstance(Guid SystemID, int Year, int Month, Guid _TargetPlanID, bool IsLatestVersion = false, string _DataSource = "Draft", bool IsAll = false)
        {
            _SystemID = SystemID;
            FinYear = Year;
            FinMonth = Month;
            DataSource = _DataSource;
            TargetPlanID = _TargetPlanID;
            InitialData(IsLatestVersion, IsAll);
        }
        /// <summary>
        /// 仅上报和审批页面调用
        /// </summary>
        /// <param name="MonthReportID"></param>
        /// <param name="IsLatestVersion">是否是最新的版本，true：从B表中获取，false：从A表中获取</param>
        /// <param name="_DataSource">当从B表中获取时，这里有Draft：草稿状态， 或者是审批中状态 </param>
        /// <param name="IsAll">是否获取全部月报数据，月报查询true,月报上报false </param>
        public ReportInstance(Guid MonthReportID, bool IsLatestVersion = false, string _DataSource = "Draft", bool IsAll = false)
        {
            _MonthReportID = MonthReportID;
            B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyreport(_MonthReportID);
            _SystemID = report.SystemID;
            _SystemBatchID = report.SystemBatchID;
            AreaID = report.AreaID;
            FinYear = report.FinYear;
            FinMonth = report.FinMonth;
            DataSource = _DataSource;
            InitialData(IsLatestVersion, IsAll);

        }
        /// <summary>
        /// 审批页面使用，审批页面数据不判断权限
        /// </summary>
        /// <param name="unUseful">没用，只是为了重载</param>
        /// <param name="monthReportID"></param>
        /// <param name="isAll"></param>
        public ReportInstance(Guid? monthReportID, Guid? systemBatchId, bool isAll)
        {
            if (monthReportID != null)
            {
                _MonthReportID = monthReportID.Value;
                B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyreport(_MonthReportID);
                _SystemID = report.SystemID;
                _SystemBatchID = report.SystemBatchID;
                TargetPlanID = report.TargetPlanID;
                AreaID = report.AreaID;
                FinYear = report.FinYear;
                FinMonth = report.FinMonth;
            }
            else if (systemBatchId != null)
            {
                B_SystemBatch b_SystemBatch = B_SystemBatchOperator.Instance.GetSystemBatch(systemBatchId.Value);
                C_System c_System = StaticResource.Instance.SystemList.Where(x => x.GroupType == b_SystemBatch.BatchType).FirstOrDefault();
                _SystemID = c_System.ID;
                _SystemBatchID = b_SystemBatch.ID;
                TargetPlanID = b_SystemBatch.TargetPlanID;
                FinYear = b_SystemBatch.FinYear;
                FinMonth = b_SystemBatch.FinMonth;
            }
            DataSource = "Draft";
            UserPermission = false;
            InitialData(true, isAll);
        }

        private string DataSource = string.Empty;
        private string currentLoginName;
        public string CurrentLoginName
        {
            get
            {
                if (string.IsNullOrEmpty(this.currentLoginName))
                {
                    this.currentLoginName = Common.WebHelper.GetCurrentLoginUser();
                }
                return this.currentLoginName;
            }
            set
            {
                this.currentLoginName = value;
            }
        }
        public Guid _SystemID { get; set; }

        public Guid _SystemBatchID { get; set; }

        public int FinYear { get; set; }
        public int FinMonth { get; set; }
        public string SummaryTemplate { get; set; }
        public Sys_Config SysConfig { get; set; }
        public MonthlyReport Report { get; set; }
        public List<MonthlyReportDetail> ReportDetails { get; set; }
        public bool UserPermission { get; set; } = true;

        [ScriptIgnore]
        public C_System _System { get { return StaticResource.Instance[_SystemID, DateTime.Now]; } }

        public List<C_Target> _Target { get { return StaticResource.Instance.GetTargetList(_SystemID, DateTime.Now).ToList(); } }

        public Guid _MonthReportID { get; set; }
        public Guid AreaID { get; set; }
        /// <summary>
        /// 分解指标计划ID
        /// </summary>
        public Guid TargetPlanID { get; set; }


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
        }
        private List<B_MonthlyReportDetail> _LastestMonthlyReportDetails = null;

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
        }
        private B_MonthlyReport _LastestMonthlyReport = null;

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
        }
        private List<A_MonthlyReportDetail> _ValidatedMonthlyReportDetails = null;

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
        }
        private A_MonthlyReport _ValidatedMonthlyReport = null;

        #region Private Method
        /// <summary>
        /// Initial the Data Source of MonthReport and MonthlyReportDetail. 
        /// if IsLatestVersion is true, the data source will be B_ table  and WFStatus == Progress.
        /// otherwise, the data source is A_ atble 
        /// </summary>
        /// <param name="IsLatestVersion">if the data source should be  B_ Table</param>
        void InitialData(bool IsLatestVersion, bool IsAll)
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
                    ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Draft(_System.ID, FinYear, FinMonth, _MonthReportID, TargetPlanID, IsAll);
                else
                    ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Approve(_System.ID, FinYear, FinMonth, _MonthReportID, TargetPlanID, IsAll);

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
                ReportDetails = A_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Result(_System.ID, FinYear, FinMonth,TargetPlanID);

                //ValidatedMonthlyReportDetails.ForEach(P => ReportDetails.Add(P.ToVModel()));
            }

            if (UserPermission)
            {
                //筛选当前用户有权限的项目公司信息(因为权限数据缺失暂时注释)
                var companyIDArray = BLL.BizBLL.S_OrganizationalActionOperator.Instance.GetUserCompanyDataNoIsDelete(_System.ID, CurrentLoginName).Select(m => m.ID).ToArray();
                if (companyIDArray.Length > 0)
                {
                    ReportDetails = ReportDetails.Where(m => companyIDArray.Contains(m.CompanyID)).ToList();
                }
                else
                {
                    ReportDetails = new List<MonthlyReportDetail>();
                }
            }
        }

        /// <summary>
        /// 获取版块下所有的详细数据(B表数据)，合并审批版块使用，裴晓红新增
        /// </summary>
        public void GetReportDetail()
        {
            ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Draft(_System.ID, FinYear, FinMonth, _MonthReportID, TargetPlanID, true);
        }
        #endregion Private Method
    }
}
