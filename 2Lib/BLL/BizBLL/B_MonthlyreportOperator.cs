using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.DAL;
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
using Lib.Data.AppBase;
using System.Data;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Monthlyreport对象的业务逻辑操作
    /// </summary>
    public class B_MonthlyreportOperator : BizOperatorBase<B_MonthlyReport>
    {

        //#region Generate Code

        public static readonly B_MonthlyreportOperator Instance = PolicyInjection.Create<B_MonthlyreportOperator>();

        private static B_MonthlyreportAdapter _bMonthlyreportAdapter = AdapterFactory.GetAdapter<B_MonthlyreportAdapter>();

        protected override BaseAdapterT<B_MonthlyReport> GetAdapter()
        {
            return _bMonthlyreportAdapter;
        }

        public IList<B_MonthlyReport> GetMonthlyreportList()
        {
            IList<B_MonthlyReport> result = _bMonthlyreportAdapter.GetMonthlyreportList();
            return result;
        }

        public IList<B_MonthlyReport> GetMonthlyreportListByMonthlyReportID(Guid MonthlyReportID)
        {
            IList<B_MonthlyReport> result = _bMonthlyreportAdapter.GetMonthlyreportList(MonthlyReportID);
            return result;
        }

        public Guid AddMonthlyreport(B_MonthlyReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_MonthlyReport GetMonthlyreport(Guid bMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportID == null ? true : false, "Argument bMonthlyreportID is Empty");
            return base.GetModelObject(bMonthlyreportID);
        }

        /// <summary>
        /// 获取B_MonthlyReport表中最新的数据(包含所有状态)
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetLastMonthlyReportList(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetLastMonthlyReportList(SystemID, Year, Month);
        }
        public B_MonthlyReport GetLastMonthlyReportList(Guid SystemID, int Year, int Month,Guid TargetPlanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanID == null ? true : false, "Argument TargetPlanID is Empty");
            return _bMonthlyreportAdapter.GetLastMonthlyReportList(SystemID, Year, Month, TargetPlanID);
        }
        /// <summary>
        /// 获取B_MonthlyReport表中（审批完成）
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportApproved(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportApprovedList(SystemID, Year, Month);
        }

        /// <summary>
        /// 获取B_MonthlyReport表中（草稿）
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportDraft(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportDraftList(SystemID, Year, Month);
        }



        /// <summary>
        /// 获取B_MonthlyReport表中（草稿）
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportDraft(Guid SystemID, Guid AreaID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(AreaID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportDraftList(SystemID, AreaID, Year, Month);
        }

        /// <summary>
        /// 获取B_MonthlyReport表中（审批中）
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReport(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportList(SystemID, Year, Month);
        }
        /// <summary>
        /// 获取B_MonthlyReport表中（审批中）
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReport(Guid SystemID, Guid SystemBatchID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemBatchID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportList(SystemID, SystemBatchID, Year, Month);
        }

        /// <summary>
        /// 获取不包含MonthlyreportID的B_MonthlyReport集合
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReport(Guid SystemID, int Year, int Month, Guid MonthlyReportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(MonthlyReportID == null ? true : false, "Argument MonthlyReportID is Empty");
            return _bMonthlyreportAdapter.GetLatestMonthlyReport(SystemID, Year, Month, MonthlyReportID);
        }


        /// <summary>
        /// 获取不包含MonthlyreportID的B_MonthlyReport集合
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthlyReportID"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReport(Guid SystemID, Guid AreaID, int Year, int Month, Guid MonthlyReportID, Guid TargetPlanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(AreaID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(MonthlyReportID == null ? true : false, "Argument MonthlyReportID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanID == null ? true : false, "Argument TargetPlanID is Empty");
            return _bMonthlyreportAdapter.GetLatestMonthlyReport(SystemID, AreaID, Year, Month, MonthlyReportID, TargetPlanID);
        }
        /// <summary>
        ///获取最新的数据(审批中和审批完成)
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReporNew(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetLatestMonthlyReport(SystemID, Year, Month);
        }
        public B_MonthlyReport GetMonthlyReporNew(Guid SystemID, int Year, int Month,Guid TargetPlanId)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanId == null ? true : false, "Argument TargetPlanId is Empty");
            return _bMonthlyreportAdapter.GetLatestMonthlyReport(SystemID, Year, Month, TargetPlanId,"");
        }

        public List<B_MonthlyReport> GetMonthlyReportByApproveList(int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportByApproveList(Year, Month);
        }
        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportByAllList(int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportByAllList(Year, Month);
        }
        /// <summary>
        /// 获取全部数据/分组获取
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public DataTable GetMonthlyReportByGroupList(int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportByGroupList(Year, Month);
        }
        /// <summary>
        /// 通过SystemID，获取list/月报
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportBySysIDList(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _bMonthlyreportAdapter.GetMonthlyReportBySysIDList(SystemID, Year, Month);
        }
        /// <summary>
        /// 通过SystemID，获取list/指标
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetTargetPlanBySysIDList(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bMonthlyreportAdapter.GetTargetPlanBySysIDList(SystemID, Year);
        }

        /// <summary>
        /// 通过批次ID，获取list
        /// </summary>
        /// <param name="SystemBatchID"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyreportList(Guid SystemBatchID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemBatchID == null ? true : false, "Argument SystemBatchID is Empty");
            return _bMonthlyreportAdapter.GetMonthlyreportList(SystemBatchID);
        }


        public Guid UpdateMonthlyreport(B_MonthlyReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlyreport(Guid bMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportID == null ? true : false, "Argument bMonthlyreportID is Empty");
            Guid result = base.RemoveObject(bMonthlyreportID);
            return result;
        }
        /// <summary>
        /// 获取已上传月报数据年份
        /// </summary>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportYearList()
        {
            return _bMonthlyreportAdapter.GetMonthlyReportYearList();
        }


        /// <summary>
        /// 删除非当前默认指标下面的月报
        /// </summary>
        /// <param name="MonthlyReport"></param>
        /// <returns></returns>
        public bool DeleteNoDefaultVersionMonthlyReport(B_MonthlyReport MonthlyReport)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(MonthlyReport == null ? true : false, "Argument MonthlyReport is Empty");
            return _bMonthlyreportAdapter.DeleteNoDefaultVersionMonthlyReport(MonthlyReport);
        }

        //#endregion
    }
}

