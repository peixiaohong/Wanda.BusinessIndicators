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
    public class C_DocumentTreeOperator : BizOperatorBase<C_DocumentTree>
    {
        public static readonly C_DocumentTreeOperator Instance = PolicyInjection.Create<C_DocumentTreeOperator>();

        private static C_DocumentTreeAdapter   _cDocumentTreeAdapter = AdapterFactory.GetAdapter<C_DocumentTreeAdapter>();

        protected override BaseAdapterT<C_DocumentTree> GetAdapter()
        {
            return _cDocumentTreeAdapter;
        }

        internal IList<C_DocumentTree> GetDocumentTreeList()
        {
            IList<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeList();
            return result;
        }

        /// <summary>
        /// 通过系统ID获取文档树的节点
        /// </summary>
        /// <param name="SysId"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeList(Guid SysId)
        {
            IList<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeList().ToList().Where(p => p.SystemID == SysId).ToList();
            return result;
        }

        /// <summary>
        /// 通过父Id，获取其下面的子集节点List
        /// </summary>
        /// <param name="SysId"></param>
        /// <returns></returns>
        public IList<C_DocumentTree> GetDocumentTreeListByParentID(Guid ParentID, Guid SysId)
        {
            IList<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeList(ParentID, SysId).ToList();
            return result;
        }
        public IList<C_DocumentTree> GetDocumentTreeListByName(string ParentID, string TreeName)
        {
            IList<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeListByName(ParentID, TreeName).ToList();
            return result;
        }
        public List<C_DocumentTree> GetDocumentTreeListByValue(Guid TypeID, Guid CompanyID, string MinYear)
        {
            List<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeListByValue(TypeID, CompanyID, MinYear).ToList();
            return result;
        }
        public List<C_DocumentTree> GetDocumentTreeListByID(Guid ParentID)
        {
            List<C_DocumentTree> result = _cDocumentTreeAdapter.GetDocumentTreeListByID(ParentID).ToList();
            return result;
        }
        public DocumentTree GetList(Guid ParentID)
        {
            DocumentTree model = new DocumentTree();
            model.IsLastTree = true;//默认其自身是最后一级
            model.IsChildLastTree = true;//默认其自身为最后一级 则无子集  
            model.TreeList = _cDocumentTreeAdapter.GetDocumentTreeListByID(ParentID).ToList();
            if (model.TreeList.Count > 0)
            {
                model.IsLastTree = false;//
                List<C_DocumentTree> ChildTree = _cDocumentTreeAdapter.GetDocumentTreeListByID(model.TreeList[0].ID).ToList();//取其子集的第一项,看其是否有子集
                if (ChildTree.Count>0)
                {
                    model.IsChildLastTree = false;
                }
            }
            return model;
        }
       
        public Guid AddDocumentTree(C_DocumentTree data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_DocumentTree GetDocumentTree(Guid cDocumentTreeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cDocumentTreeID == null, "Argument cTargetkpiID is Empty");
            return base.GetModelObject(cDocumentTreeID);
        }

        public Guid UpdateDocumentTree(C_DocumentTree data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveDocumentTree(Guid cDocumentTreeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cDocumentTreeID == null, "Argument cTargetkpiID is Empty");
            Guid result = base.RemoveObject(cDocumentTreeID);
            return result;
        }
        /// <summary>
        /// 取类别专用  查询了两次
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public List<C_DocumentTree> GetTypeList(Guid ParentID)
        {
            List<C_DocumentTree> result = new List<C_DocumentTree>();
            List<C_DocumentTree> detail = GetDocumentTreeListByID(ParentID).ToList();
            if (detail.Count > 0)
            {
                result = GetDocumentTreeListByID(detail[0].ID).ToList();
            }
            return result;
        }

        public List<C_DocumentTree> GetCompanyList(Guid ParentID)
        {
            List<C_DocumentTree> detail = GetDocumentTreeListByID(ParentID).ToList();
            return detail;
        }

    }
}
