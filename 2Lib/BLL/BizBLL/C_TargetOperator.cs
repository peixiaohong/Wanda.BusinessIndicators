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


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Target对象的业务逻辑操作
    /// </summary>
    public class C_TargetOperator : BizOperatorBase<C_Target>
	{
    
        #region Generate Code

        public static readonly C_TargetOperator Instance = PolicyInjection.Create<C_TargetOperator>();

        private static C_TargetAdapter _cTargetAdapter = AdapterFactory.GetAdapter<C_TargetAdapter>();

        protected override BaseAdapterT<C_Target> GetAdapter()
        {
            return  _cTargetAdapter;
        }

        public IList<C_Target> GetTargetList()
        {
            IList<C_Target> result = _cTargetAdapter.GetTargetList();
            return result;
        }

        public IList<C_Target> GetTargetList(Guid SystemID, DateTime CurrentDate)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");

            return _cTargetAdapter.GetTargetList(SystemID, CurrentDate);
        }


        public IList<string> GetTargetList(string SystemIDs, DateTime CurrentDate)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemIDs == null ? true : false, "Argument SystemID is Empty");

            return _cTargetAdapter.GetTargetList(SystemIDs, CurrentDate);
        }



        public IList<C_Target> GetDetailTargetList(Guid SystemID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");

            return _cTargetAdapter.GetDetailTargetList(SystemID);
        }

        public Guid AddTarget(C_Target data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_Target GetTarget(Guid cTargetID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetID==null ,"Argument cTargetID is Empty");
            return base.GetModelObject(cTargetID);
        }

        /// <summary>
        /// 获取版本实体
        /// </summary>
        /// <param name="cTargetID"></param>
        /// <param name="VersionTime"></param>
        /// <returns></returns>
        public C_Target GetTarget(Guid cTargetID, DateTime? VersionTime) 
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetID == null, "Argument cTargetID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(VersionTime == null, "Argument cTargetID is Empty");

            List<C_Target> TargerList =  _cTargetAdapter.GetTargetListByVersionTime(cTargetID, VersionTime).ToList();

            return TargerList.FirstOrDefault();
        }



        public Guid UpdateTarget(C_Target data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }
        public int UpdateTargetVerSion(C_Target data,DateTime VerStart)
        {
            return _cTargetAdapter.UpdateTargetVerSion(data, VerStart);
            
        }

        public int DeleteTargetListByID(Guid TargetID)
        {
            return _cTargetAdapter.DeleteTargetListByID(TargetID);
        }


        public Guid RemoveTarget(Guid cTargetID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetID==null ,"Argument cTargetID is Empty");
            Guid result = base.RemoveObject(cTargetID);
            return result;
        }
        public List<C_Target> GetTargetForType(Guid SystemID)
        {
            List<C_Target> TargerList = _cTargetAdapter.GetTargetForType(SystemID).ToList();

            return TargerList;
        }
        #endregion
    } 
}

