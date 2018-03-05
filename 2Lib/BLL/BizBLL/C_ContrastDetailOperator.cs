using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.DAL;
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Lib.Data.AppBase;


namespace Wanda.BusinessIndicators.BLL
{
    public class C_ContrastDetailOperator : BizOperatorBase<C_ContrastDetail>
    {
        #region Generate Code

        public static readonly C_ContrastDetailOperator Instance = PolicyInjection.Create<C_ContrastDetailOperator>();

        private static C_ContrastDetailAdapter _bContrastDetailAdapter = AdapterFactory.GetAdapter<C_ContrastDetailAdapter>();

        protected override BaseAdapterT<C_ContrastDetail> GetAdapter()
        {
            return _bContrastDetailAdapter;
        }

        public IList<C_ContrastDetail> GetContrastDetailList()
        {
            IList<C_ContrastDetail> result = _bContrastDetailAdapter.GetContrastDetailList();
            return result;
        }



        public Guid AddContrastDetail(C_ContrastDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_ContrastDetail GetContrastDetail(Guid bContrastDetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bContrastDetailID == null, "Argument ContrastDetailID is Empty");
            return base.GetModelObject(bContrastDetailID);
        }

        public Guid UpdateContrastDetail(C_ContrastDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveContrastDetail(Guid bContrastDetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bContrastDetailID == null, "Argument ContrastDetailID is Empty");
            Guid result = base.RemoveObject(bContrastDetailID);
            return result;
        }

        public IList<C_ContrastDetail> GetContrastDetailList(int FinYear, int finMonth, Guid SystemID)
        {
            IList<C_ContrastDetail> result = _bContrastDetailAdapter.GetContrastDetailList(FinYear, finMonth, SystemID);
            return result;
        }


        public IList<C_ContrastDetail> GetAllContrastDetailList(int FinYear, int finMonth)
        {
            IList<C_ContrastDetail> result = _bContrastDetailAdapter.GetAllContrastDetailList(FinYear, finMonth);
            return result;
        }
        
        #endregion


        public List<ContrastDetailList> GetDetailList(string FinYear, string FinMonth)
        {
            List<ContrastDetailList> result = new List<ContrastDetailList>();
           // List<C_System> SystemList = C_SystemOperator.Instance.GetSystemListBySeq().ToList();
            List<C_ContrastDetail> AllSystemList = GetAllContrastDetailList(int.Parse(FinYear), int.Parse(FinMonth)).ToList();

            //获取这个月有数据的系统
            List<C_System> NewSystemList = AllSystemList.GroupBy(o => o.SystemID).Select(p => new C_System { ID = p.Key, SystemName = p.FirstOrDefault().SystemName }).ToList();
            foreach (C_System item in NewSystemList)
            {
                ContrastDetailList model = new ContrastDetailList();
                model.SystemName = item.SystemName;
                model.systemID = item.ID;
                model.ContrastDetailMl = AllSystemList.Where(p => p.SystemID == item.ID).ToList();
                result.Add(model);
            }
           
            return result; ;
        }
    }
}
