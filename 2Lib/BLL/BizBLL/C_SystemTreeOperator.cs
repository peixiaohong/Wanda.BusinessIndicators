using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.DAL;
using Wanda.BusinessIndicators.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.BLL
{
    public class C_SystemTreeOperator : BizOperatorBase<C_SystemTree>
    {
        public static readonly C_SystemTreeOperator Instance = PolicyInjection.Create<C_SystemTreeOperator>();

        private static C_SystemTreeAdapter _cSystemTreeAdapter = AdapterFactory.GetAdapter<C_SystemTreeAdapter>();

        protected override BaseAdapterT<C_SystemTree> GetAdapter()
        {
            return _cSystemTreeAdapter;
        }

        public IList<C_SystemTree> GetSystemTreeList()
        {
            IList<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeList();
            return result;
        }

        /// <summary>
        /// 通过系统ID获取文档树的节点
        /// </summary>
        /// <param name="SysId"></param>
        /// <returns></returns>
        public IList<C_SystemTree> GetSystemTreeList(Guid SysId)
        {
            IList<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeList().ToList().Where(p => p.ID == SysId).ToList();
            return result;
        }

        /// <summary>
        /// 通过父Id，获取其下面的子集节点List
        /// </summary>
        /// <param name="SysId"></param>
        /// <returns></returns>
        public IList<C_SystemTree> GetSystemTreeListByParentID(Guid ParentID, Guid SysId)
        {
            IList<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeList(ParentID, SysId).ToList();
            return result;
        }
        public IList<C_SystemTree> GetSystemTreeListByName(string ParentID, string TreeName)
        {
            IList<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeListByName(ParentID, TreeName).ToList();
            return result;
        }
        public List<C_SystemTree> GetSystemTreeListByValue(Guid TypeID, Guid CompanyID, string MinYear)
        {
            List<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeListByValue(TypeID, CompanyID, MinYear).ToList();
            return result;
        }
        public List<C_SystemTree> GetSystemTreeListByID(Guid ParentID)
        {
            List<C_SystemTree> result = _cSystemTreeAdapter.GetSystemTreeListByID(ParentID).ToList();
            return result;
        }
        public SystemTree GetList(Guid ParentID)
        {
            SystemTree model = new SystemTree();
            model.IsLastTree = true;//默认其自身是最后一级
            model.IsChildLastTree = true;//默认其自身为最后一级 则无子集  
            model.TreeList = _cSystemTreeAdapter.GetSystemTreeListByID(ParentID).ToList();
            if (model.TreeList.Count > 0)
            {
                model.IsLastTree = false;//
                List<C_SystemTree> ChildTree = _cSystemTreeAdapter.GetSystemTreeListByID(model.TreeList[0].ID).ToList();//取其子集的第一项,看其是否有子集
                if (ChildTree.Count>0)
                {
                    model.IsChildLastTree = false;
                }
            }
            return model;
        }

        public Guid AddSystemTree(C_SystemTree data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_SystemTree GetSystemTree(Guid cSystemTreeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cSystemTreeID == null, "Argument cTargetkpiID is Empty");
            return base.GetModelObject(cSystemTreeID);
        }

        public Guid UpdateSystemTree(C_SystemTree data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveSystemTree(Guid cSystemTreeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cSystemTreeID == null, "Argument cTargetkpiID is Empty");
            Guid result = base.RemoveObject(cSystemTreeID);
            return result;
        }
        /// <summary>
        /// 取类别专用  查询了两次
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public List<C_SystemTree> GetTypeList(Guid ParentID)
        {
            List<C_SystemTree> result = new List<C_SystemTree>();
            List<C_SystemTree> detail = GetSystemTreeListByID(ParentID).ToList();
            if (detail.Count > 0)
            {
                result = GetSystemTreeListByID(detail[0].ID).ToList();
            }
            return result;
        }

        public List<C_SystemTree> GetCompanyList(Guid ParentID)
        {
            List<C_SystemTree> detail = GetSystemTreeListByID(ParentID).ToList();
            return detail;
        }




        /// <summary>
        /// 根据子节点的ID串，获取整棵树的数据(子找父)
        /// </summary>
        /// <param name="SysIds"></param>
        /// <returns></returns>
        public List<C_SystemTree> GetSystemTreeData(string SysIds)
        {
            List<C_SystemTree> list = _cSystemTreeAdapter.GetSystemTreeData(SysIds);
            return list;
        }


        /// <summary>
        /// 根据子节点的ID串，获取整棵树的数据(父找子)
        /// </summary>
        /// <param name="SysIds"></param>
        /// <returns></returns>
        public List<C_SystemTree> GetSystemTreeDataByParent(string SysIds)
        {
            List<C_SystemTree> list = _cSystemTreeAdapter.GetSystemTreeDataByParent(SysIds);
            return list;
        }

        /// <summary>
        /// 获取汇总分组数据
        /// </summary>
        /// <returns></returns>
        public List<C_SystemTree> GetSysTreeExcelGroup() 
        {
            List<C_SystemTree> list = _cSystemTreeAdapter.GetSysTreeExcelGroup();
            return list;
        }
        
    }
}
