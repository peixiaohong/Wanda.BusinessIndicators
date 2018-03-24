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
    public class A_SystemBatchOperator : BizOperatorBase<A_SystemBatch>
    {
        #region Generate Code

        public static readonly A_SystemBatchOperator Instance = PolicyInjection.Create<A_SystemBatchOperator>();

        private static A_SystemBatchAdapter _aSystemBatchAdapter = AdapterFactory.GetAdapter<A_SystemBatchAdapter>();

        protected override BaseAdapterT<A_SystemBatch> GetAdapter()
        {
            return _aSystemBatchAdapter;
        }

        public IList<A_SystemBatch> GetSystemBatchList()
        {
            IList<A_SystemBatch> result = _aSystemBatchAdapter.GetSystemBatchList();
            return result;
        }

        public Guid AddSystemBatch(A_SystemBatch data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_SystemBatch GetSystemBatch(Guid bSystemBatchID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bSystemBatchID == null, "Argument bSystemBatchID is Empty");
            return base.GetModelObject(bSystemBatchID);
        }


        /// <summary>
        /// 通过年月获取批次表的实体
        /// </summary>
        /// <param name="BatchType"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public A_SystemBatch GetSystemBatch(string BatchType, int FinYear , int FinMonth)
        {
            
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinYear <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(FinMonth <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(BatchType == null ? true : false, "Argument MonthlyReportID is Empty");

            return _aSystemBatchAdapter.GetSystemBatch(BatchType, FinYear, FinMonth);
           
        }


        public Guid UpdateSystemBatch(A_SystemBatch data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }


        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="bSystemBatchID"></param>
        /// <returns></returns>
        public Guid RemoveSystemBatch(Guid bSystemBatchID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bSystemBatchID == null ? true : false, "Argument bSystemBatchID is Empty");
            Guid result = base.RemoveObject(bSystemBatchID);
            return result;
        }


        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int DeleteSystemBatch(A_SystemBatch data)
        {
            return _aSystemBatchAdapter.DeleteModel(data);
        }

        #endregion
    }
}
