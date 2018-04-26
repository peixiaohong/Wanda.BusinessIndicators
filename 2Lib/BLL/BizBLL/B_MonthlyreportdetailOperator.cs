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
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Monthlyreportdetail对象的业务逻辑操作
    /// </summary>
    public class B_MonthlyreportdetailOperator : BizOperatorBase<B_MonthlyReportDetail>
    {

        #region Generate Code

        public static readonly B_MonthlyreportdetailOperator Instance = PolicyInjection.Create<B_MonthlyreportdetailOperator>();

        private static B_MonthlyreportdetailAdapter _bMonthlyreportdetailAdapter = AdapterFactory.GetAdapter<B_MonthlyreportdetailAdapter>();

        protected override BaseAdapterT<B_MonthlyReportDetail> GetAdapter()
        {
            return _bMonthlyreportdetailAdapter;
        }

        public IList<B_MonthlyReportDetail> GetMonthlyreportdetailList()
        {
            IList<B_MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList();
            return result;
        }

        /// <summary>
        /// 获取B表的（审批中的）详细数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthReportID">可以为NULL</param>
        /// <returns></returns>
        public IList<B_MonthlyReportDetail> GetMonthlyreportdetailList(Guid SystemID, int Year, int Month, Guid MonthReportID)
        {
            //if (MonthReportID == Guid.Empty)
            //{
            //    B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyReport(SystemID, Year, Month);
            //    MonthReportID = report.ID;
            //}
            //if (MonthReportID != Guid.Empty)
            //{
            //    IList<B_MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList(MonthReportID);
            //    return result;
            //}
            //return new List<B_MonthlyReportDetail>();
            if (MonthReportID == Guid.Empty)
                return _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList(SystemID, Year, Month);
            else
                return _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList(MonthReportID);
        }

        public IList<B_MonthlyReportDetail> GetMonthlyreportdetailList(Guid MonthReportID)
        {
            if (MonthReportID != Guid.Empty && MonthReportID != null)
            {
                IList<B_MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList(MonthReportID);
                return result;
            }
            return new List<B_MonthlyReportDetail>();
        }
        public IList<MonthlyReportVM> GetBVMonthlyReport(Guid MonthlyReportID)
        {
            if (MonthlyReportID != Guid.Empty && MonthlyReportID != null)
            {
                IList<MonthlyReportVM> result = _bMonthlyreportdetailAdapter.GetBVMonthlyReport(MonthlyReportID);
                return result;
            }
            return new List<MonthlyReportVM>();
        }

        /// <summary>
        /// 获取B表的（包含草稿）详细数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public IList<B_MonthlyReportDetail> GetMonthlyReportDetailListBytDraft(Guid SystemID, int Year, int Month, Guid MonthReportID)
        {
            B_MonthlyReport report = null;
            if (MonthReportID == Guid.Empty)
            {
                report = B_MonthlyreportOperator.Instance.GetLastMonthlyReportList(SystemID, Year, Month);
            }
            else
            {
                report = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            }
            if (report != null)
            {
                IList<B_MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyreportdetailList(report.ID);
                return result;
            }
            return new List<B_MonthlyReportDetail>();
        }


        public Guid AddMonthlyreportdetail(B_MonthlyReportDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_MonthlyReportDetail GetMonthlyreportdetail(Guid bMonthlyreportdetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportdetailID == null, "Argument bMonthlyreportdetailID is Empty");
            return base.GetModelObject(bMonthlyreportdetailID);
        }

        public Guid UpdateMonthlyreportdetail(B_MonthlyReportDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlyreportdetail(Guid bMonthlyreportdetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportdetailID == null, "Argument bMonthlyreportdetailID is Empty");
            Guid result = base.RemoveObject(bMonthlyreportdetailID);
            return result;
        }

        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int UpdateMonthlyreportDetailList(List<B_MonthlyReportDetail> List)
        {
            if (List.Count > 0)
            {
                return _bMonthlyreportdetailAdapter.UpdateMonthlyreportdetailLisr(List);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int AddMonthlyreportDetailList(List<B_MonthlyReportDetail> List)
        {
            if (List.Count > 0)
            {
                return _bMonthlyreportdetailAdapter.AddMonthlyreportdetailLisr(List);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加指标数据
        /// </summary>
        /// <param name="list">指标数据</param>
        /// <returns>执行状态</returns>
        public int AddOrUpdateTargetDetail(List<B_MonthlyReportDetail> list, string strOperateType)
        {
            if (strOperateType == "Update")
            {
                UpdateMonthlyreportDetailList(list);
                return list.Count;
            }
            else
            {

                AddMonthlyreportDetailList(list);

                return list.Count;
            }
        }


        /// <summary>
        /// 包含有草稿的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetMonthlyReportDetailList_Draft(Guid SystemID, int Year, int Month, Guid MonthReportID)
        {
            //B_MonthlyReport report = null;
            //if (MonthReportID == Guid.Empty)
            //{
            //    report = B_MonthlyreportOperator.Instance.GetLastMonthlyReportList(SystemID, Year, Month);
            //}
            //else
            //{
            //    report = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            //}
            //if (report != null)
            //{
            //    List<MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyReportDetailList(report.ID);
            //    return result;
            //}
            //return new List<MonthlyReportDetail>();
            return _bMonthlyreportdetailAdapter.GetMonthlyReportDetailList(MonthReportID,SystemID);
        }

        /// <summary>
        /// 只有审批中的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetMonthlyReportDetailList_Approve(Guid SystemID, int Year, int Month, Guid MonthReportID)
        {
            //B_MonthlyReport report = null;
            //if (MonthReportID == Guid.Empty)
            //{
            //    report = B_MonthlyreportOperator.Instance.GetMonthlyReport(SystemID, Year, Month);
            //}
            //else
            //{
            //    report = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthReportID);
            //}
            //if (report != null)
            //{
            //    List<MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthlyReportDetailList(SystemID);
            //    return result;
            //}
            //return new List<MonthlyReportDetail>();
            return _bMonthlyreportdetailAdapter.GetMonthlyReportDetailList(MonthReportID, SystemID);

        }

        /// <summary>
        /// 万达电影固定报表，国内影城-票房收入指标查询
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<B_MonthlyReportDetail> GetMonthTargetDetailCNPF(Guid SystemID, int FinYear, int FinMonth, Guid TargetID)
        {
            List<B_MonthlyReportDetail> result = _bMonthlyreportdetailAdapter.GetMonthTargetDetailCNPF(SystemID, FinYear, FinMonth, TargetID);
            return result;
        }

        #endregion

        /// <summary>
        /// 判断明细表中的数据，是否存在
        /// </summary>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public bool GetMonthlyReportDetailCount(Guid MonthReportID)
        {
            return _bMonthlyreportdetailAdapter.GetMonthlyReportDetailCount(MonthReportID);
        }

        /// <summary>
        /// 从A表中复制一份数据
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SysId"></param>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByAToB(int FinYear, int FinMonth, Guid SysId, Guid MonthReportID)
        {
            return _bMonthlyreportdetailAdapter.GetMonthlyReportDetail_ByAToB(FinYear, FinMonth, SysId, MonthReportID);
        }

        /// <summary>
        /// 从B表中复制一份数据
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SysId"></param>
        /// <param name="MonthReportID"></param>
        /// <returns></returns>
        public List<B_MonthlyReportDetail> GetMonthlyReportDetail_ByBToB(int FinYear, int FinMonth, Guid SysId, Guid OldMonthReportID, Guid NewMonthReportID)
        {
            return _bMonthlyreportdetailAdapter.GetMonthlyReportDetail_ByBToB(FinYear, FinMonth, SysId, OldMonthReportID, NewMonthReportID);
        }



        /// <summary>
        /// 添加指标数据
        /// </summary>
        /// <param name="list">指标数据</param>
        /// <returns>执行状态</returns>
        public int BulkAddTargetDetail(List<B_MonthlyReportDetail> list)
        {
            if (list.Count > 0)
            {
                return _bMonthlyreportdetailAdapter.BulkAddMonthlyReportDetailLisr(list);
            }
            else
            {
                return 0;
            }
        }
    }
}

