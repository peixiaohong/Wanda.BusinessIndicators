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
    /// <summary>
    /// System对象的业务逻辑操作
    /// </summary>
    public class C_SystemOperator:BizOperatorBase<C_System>
	{
    
        #region Generate Code

        public static readonly C_SystemOperator Instance = PolicyInjection.Create<C_SystemOperator>();
        private static C_SystemAdapter _cSystemAdapter = AdapterFactory.GetAdapter<C_SystemAdapter>();

        protected override BaseAdapterT<C_System> GetAdapter()
        {
            return  _cSystemAdapter;
        }

        public IList<C_System> GetSystemList(DateTime CurrentDate)
        {
            IList<C_System> result = _cSystemAdapter.GetSystemList(CurrentDate);
            return result;
        }
        public IList<C_System> GetSystemListBySeq()
        {
            List<C_System> aresult = _cSystemAdapter.GetSystemListBySeq().ToList();
            List<Guid> IDList = new List<Guid>();
           // List<C_System> List = C_SystemOperator.Instance.GetSystemListBySeq().ToList();//先取出所有
            foreach (C_System item in aresult)//将他们的ID存入List
            {
                IDList.Add(item.ID);
            }
            IDList = IDList.Distinct().ToList();//去重    
            List<C_System> result = new List<C_System>();
            foreach (Guid item in IDList)
            {
                result.Add(StaticResource.Instance[item, DateTime.Now]);
            }

            return result;
        }
        public IList<C_System> GetSystemListByConfigID(Guid cid)
        {
            return _cSystemAdapter.GetSystemListByConfigID(cid);
        }
        public Guid AddSystem(C_System data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_System GetSystem(Guid cSystemID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cSystemID==null ,"Argument cSystemID is Empty");
            return base.GetModelObject(cSystemID);
        }

        public List<C_System> GetSystemListByGrouptype(string GroupType)
        {
            List<C_System> result = _cSystemAdapter.GetSystemListByGrouptype(GroupType).ToList();
            return result;
        }
        /// <summary>
        /// 获取版本实体
        /// </summary>
        /// <param name="_cSystemID"></param>
        /// <param name="_VersionTime"></param>
        /// <returns></returns>
        public C_System GetSystem(Guid _cSystemID, DateTime? _VersionTime)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(_cSystemID == null, "Argument _cSystemID is Empty");

            ExceptionHelper.TrueThrow<ArgumentNullException>(_VersionTime == null, "Argument _VersionTime is Empty");

            List<C_System> SysList = _cSystemAdapter.GetSystemListByVersionTime(_cSystemID, _VersionTime).ToList();

            return SysList.FirstOrDefault();
        }



        public Guid UpdateSystem(C_System data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveSystem(Guid cSystemID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cSystemID==null ,"Argument cSystemID is Empty");
            Guid result = base.RemoveObject(cSystemID);
            return result;
        }
        /// <summary>
        /// 根据系统名称获取信息
        /// </summary>
        /// <param name="SystemName"></param>
        /// <returns></returns>
        public C_System GetSystemByName(string SystemName)
        {
            return _cSystemAdapter.GetSystemByName(SystemName);
        }
        #endregion
    } 
}

