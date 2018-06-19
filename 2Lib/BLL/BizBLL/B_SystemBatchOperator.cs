using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.DAL;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.BLL
{
    public class B_SystemBatchOperator : BizOperatorBase<B_SystemBatch>
    {
        #region Generate Code

        public static readonly B_SystemBatchOperator Instance = PolicyInjection.Create<B_SystemBatchOperator>();

        private static B_SystemBatchAdapter _bSystemBatchAdapter = AdapterFactory.GetAdapter<B_SystemBatchAdapter>();

        protected override BaseAdapterT<B_SystemBatch> GetAdapter()
        {
            return _bSystemBatchAdapter;
        }

        public IList<B_SystemBatch> GetSystemBatchList(int minMonth)
        {
            IList<B_SystemBatch> result = _bSystemBatchAdapter.GetSystemBatchList(minMonth);
            return result;
        }

        public Guid AddSystemBatch(B_SystemBatch data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_SystemBatch GetSystemBatch(Guid bSystemBatchID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bSystemBatchID == null, "Argument bSystemBatchID is Empty");
            return base.GetModelObject(bSystemBatchID);
        }


        /// <summary>
        /// 通过年月获取批次表的实体（不包含作废的数据）
        /// </summary>
        /// <param name="BatchType"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public B_SystemBatch GetSystemBatch(string BatchType, int FinYear , int FinMonth)
        {
            
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinYear <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinMonth <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(BatchType == null ? true : false, "Argument MonthlyReportID is Empty");

            return _bSystemBatchAdapter.GetSystemBatch(BatchType, FinYear, FinMonth);
           
        }
        public List<B_SystemBatch> GetSystemBatchList(string BatchType, int FinYear, int FinMonth)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(FinYear <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinMonth <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(BatchType == null ? true : false, "Argument MonthlyReportID is Empty");

            return _bSystemBatchAdapter.GetSystemBatchList(BatchType, FinYear, FinMonth);

        }


        /// <summary>
        /// 通过年月获取批次表的实体(只有草稿状态的数据)
        /// </summary>
        /// <param name="BatchType"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public B_SystemBatch GetSystemBatchByDraft(string BatchType, int FinYear, int FinMonth)
        {
            
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinYear <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinMonth <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(BatchType == null ? true : false, "Argument MonthlyReportID is Empty");

            return _bSystemBatchAdapter.GetSystemBatchByDraft(BatchType, FinYear, FinMonth);
        }
           


        public Guid UpdateSystemBatch(B_SystemBatch data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveSystemBatch(Guid bSystemBatchID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bSystemBatchID == null ? true : false, "Argument bSystemBatchID is Empty");
            Guid result = base.RemoveObject(bSystemBatchID);
            return result;
        }

        #endregion
    }
}
