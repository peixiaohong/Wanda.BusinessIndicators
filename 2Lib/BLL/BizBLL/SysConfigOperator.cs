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
    /// SysConfig对象的业务逻辑操作
    /// </summary>
    public class SysConfigOperator : BizOperatorBase<Sys_Config>
	{
    
        #region Generate Code
    
        public static readonly SysConfigOperator Instance = PolicyInjection.Create<SysConfigOperator>();

        private static SysConfigAdapter _sysConfigAdapter = AdapterFactory.GetAdapter<SysConfigAdapter>();

        protected override BaseAdapterT<Sys_Config> GetAdapter()
        {
            return  _sysConfigAdapter;
        }

        public IList<Sys_Config> GetSysConfigListByType(string type)
        {
            return _sysConfigAdapter.GetSysConfigListByType(type);
        }
        public IList<Sys_Config> GetSysConfigList()
        {
            IList<Sys_Config> result = _sysConfigAdapter.GetSysConfigList();
            return result;
        }

        public Guid AddSysConfig(Sys_Config data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public Sys_Config GetSysConfig(Guid sysConfigID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sysConfigID==null ,"Argument sysConfigID is Empty");
            return base.GetModelObject(sysConfigID);
        }

        public Guid UpdateSysConfig(Sys_Config data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveSysConfig(Guid sysConfigID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sysConfigID==null ,"Argument sysConfigID is Empty");
            Guid result = base.RemoveObject(sysConfigID);
            return result;
        }
        

        #endregion
    } 
}

