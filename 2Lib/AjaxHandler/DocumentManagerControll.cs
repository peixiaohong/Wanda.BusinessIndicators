using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{

    public class DocumentManagerControll : BaseController
    {





        //页面初次加载方法
        [LibAction]
        public List<C_DocumentTree> GetDocumentTreeList(string SysID)
        {
            Guid _SystemID = SysID.ToGuid();
            List<C_DocumentTree> documentTreeList = C_DocumentTreeOperator.Instance.GetDocumentTreeList(_SystemID).ToList();
            if (documentTreeList.Count > 0)
            {
                var root = documentTreeList.Where(p => p.ParentID == "99999999-9999-9999-9999-FFFFFFFFFFFF".ToGuid()).FirstOrDefault();
                if (root != null)
                    root.open = true;

                return documentTreeList;
            }
            else
            {
                C_System sys = StaticResource.Instance[_SystemID,DateTime.Now];

                Guid treeRootID = AddDocumentTreeNode("99999999-9999-9999-9999-FFFFFFFFFFFF", SysID, sys.SystemName + "根目录");
                if (treeRootID != Guid.Empty)
                    return C_DocumentTreeOperator.Instance.GetDocumentTreeList(_SystemID).ToList();
                else
                    return new List<C_DocumentTree>();
            }
        }

        //获取上传文档List
        [LibAction]
        public List<B_DocumentAttachments> GetDocAttachmentsList(string TreeNodeID)
        {
            Guid _TreeNodeID = TreeNodeID.ToGuid();

            List<B_DocumentAttachments> docList = B_DocumentAttachmentsOperator.Instance.GetDocAttachmentsList(_TreeNodeID).ToList();
            return docList;

        }

        //通过父ID 获取其子节点List
        [LibAction]
        public List<C_DocumentTree> GetDocTreeListByParentID(string TreeNodeID, string SysID)
        {
            Guid _TreeNodeID = TreeNodeID.ToGuid();
            Guid _SysID = SysID.ToGuid();

            List<C_DocumentTree> docList = C_DocumentTreeOperator.Instance.GetDocumentTreeListByParentID(_TreeNodeID, _SysID).ToList();
            return docList;

        }
        //通过父Id和名称获取
        [LibAction]
        public List<C_DocumentTree> GetDocumentTreeListByName(string TreeNodeID, string TreeName)
        {
            List<C_DocumentTree> result = new List<C_DocumentTree>();
            List<C_DocumentTree> docList = C_DocumentTreeOperator.Instance.GetDocumentTreeListByName(TreeNodeID, TreeName).ToList();

            if (docList.Count > 0)
            {
                result = C_DocumentTreeOperator.Instance.GetDocumentTreeListByID(docList[0].ID).ToList();
            }

            return result;

        }
        /// <summary>
        /// 获取所有文件类型,仅适用于取第一级数据
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [LibAction]
        public List<C_DocumentTree> GetTypeList(string ParentID)
        {
            List<C_DocumentTree> result = new List<C_DocumentTree>();
            List<C_DocumentTree> detail = C_DocumentTreeOperator.Instance.GetDocumentTreeListByID(Guid.Parse(ParentID)).ToList();
            if (detail.Count > 0)
            {
                result = C_DocumentTreeOperator.Instance.GetDocumentTreeListByID(detail[0].ID).ToList();
            }


            return result;
        }
        [LibAction]
        public List<C_DocumentTree> GetDocumentTreeListByID(string ParentID)
        {
            List<C_DocumentTree> result = C_DocumentTreeOperator.Instance.GetDocumentTreeListByID(Guid.Parse(ParentID)).ToList();
            return result;
        }


        [LibAction]
        public DocumentTree GetDTreeListByID(string ParentID)
        {
            DocumentTree model = C_DocumentTreeOperator.Instance.GetList(Guid.Parse(ParentID));
            return model;
        }

        /// <summary>
        /// 获取附件列表
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        [LibAction]
        public List<B_DocumentAttachments> GetAttachments(string businessID)
        {

            List<B_DocumentAttachments> list = new List<B_DocumentAttachments>();

            if (string.IsNullOrEmpty(businessID))
                return list;

            Guid _businessID = Guid.Parse(businessID);
            list = B_DocumentAttachmentsOperator.Instance.GetDocAttachmentsList(_businessID).ToList();
            return list;
        }

        [LibAction]
        public List<YearModel> GetFinYear()
        {
            List<YearModel> result = new List<YearModel>();


            for (int i = -2; i <= 1; i++)
            {
                YearModel model = new YearModel();
                string Year = DateTime.Now.AddYears(i).Year.ToString();
                string Value = Year + "年";
                if (i == -2)
                {
                    Value += "以前";
                }
                model.Year = Year;
                model.YearValue = Value;
                result.Add(model);
            }


            return result;
        }
        /// <summary>
        /// 通过条件获取附件列表
        /// </summary>
        /// <param name="firstID">一级节点ID</param>
        /// <returns></returns>
        [LibAction]
        public List<B_DocumentAttachments> GetAttachmentsByType(string ValueA, string ValueB, string ValueC, string ValueD, string Year)
        {
            List<B_DocumentAttachments> AllList = new List<B_DocumentAttachments>();

            AllList = B_DocumentAttachmentsOperator.Instance.GetListByValueA(Guid.Parse(ValueA));


            if (ValueB != null)
            {
                if (AllList.Count > 0)
                {
                    AllList = AllList.Where(adj => adj.ValueB == Guid.Parse(ValueB)).ToList();
                }
            }
            if (ValueC != null)
            {
                if (AllList.Count > 0)
                {
                    AllList = AllList.Where(adj => adj.ValueC == Guid.Parse(ValueC)).ToList();
                }
            }
            if (ValueD != null)
            {
                if (AllList.Count > 0)
                {
                    AllList = AllList.Where(adj => adj.ValueD == Guid.Parse(ValueD)).ToList();
                }
            }
            if (Year != null)
            {
                int FinYear = int.Parse(Year.Substring(0, 4));
                if (Year.Length > 5)
                {
                    AllList = AllList.Where(adj => adj.FinYear <= FinYear).ToList();
                }
                else
                {
                    AllList = AllList.Where(adj => adj.FinYear == FinYear).ToList();
                }
            }
            return AllList;
        }
        //通过名称模糊查询
        [LibAction]
        public List<B_DocumentAttachments> GetAttachmentsByName(string Value)
        {
            List<B_DocumentAttachments> result = new List<B_DocumentAttachments>();
            result = B_DocumentAttachmentsOperator.Instance.GetAllFile();
            if (Value!=null&&result.Count>0)
            {
                result = result.Where(adj => adj.FileName.Contains(Value.Trim())).ToList();
            }
            
            return result;
        }
        /// <summary>
        /// 搜索获取文档文件
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [LibAction]
        public List<B_DocumentAttachments> GetAttachmentsBySearch(string BusinessID, string FileName)
        {
            List<B_DocumentAttachments> list = new List<B_DocumentAttachments>();

            if (string.IsNullOrEmpty(BusinessID))
                return list;
            else
            {
                list = B_DocumentAttachmentsOperator.Instance.GetDocAttachmentsList(BusinessID.ToGuid(), FileName);

                return list;
            }
        }



        //添加
        [LibAction]
        public Guid AddDocumentTreeNode(string TreeNodeID, string SysID, string NodeNmae)
        {
            C_DocumentTree Model = new C_DocumentTree();
            Model.ParentID = TreeNodeID.ToGuid();
            Model.SystemID = SysID.ToGuid();
            Model.TreeNodeName = NodeNmae;
            return C_DocumentTreeOperator.Instance.AddDocumentTree(Model);

        }

        //修改
        [LibAction]
        public Guid UpdateDocumentTreeNode(string TreeNodeID, string SysID, string NodeNmae)
        {
            C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(TreeNodeID.ToGuid());
            Model.TreeNodeName = NodeNmae;
            return C_DocumentTreeOperator.Instance.UpdateDocumentTree(Model);

        }

        //删除树节点
        [LibAction]
        public Guid DelDocumentTreeNode(string TreeNodeID, string SysID, string NodeNmae)
        {
            C_DocumentTree Model = C_DocumentTreeOperator.Instance.GetDocumentTree(TreeNodeID.ToGuid());
            Model.IsDeleted = true;
            return C_DocumentTreeOperator.Instance.UpdateDocumentTree(Model);

        }

        //删除文档文件
        [LibAction]
        public string DelDocAttachments(string DocAttachmentsID)
        {
            if (!string.IsNullOrEmpty(DocAttachmentsID))
            {
                Guid _DocAttachmentsID = B_DocumentAttachmentsOperator.Instance.RemoveDocumentAttachments(DocAttachmentsID.ToGuid());

                if (_DocAttachmentsID != Guid.Empty)
                    return "Succeed";
                else
                    return "Failure";
            }
            else
                return "Failure";


        }


        //修改文档备注
        [LibAction]
        public string UpdateDocManageByRemark(string Data)
        {

            if (!string.IsNullOrEmpty(Data))
            {
                B_DocumentAttachments detail = JsonHelper.Deserialize<B_DocumentAttachments>(Data);

                Guid _DocAttachmentsID = B_DocumentAttachmentsOperator.Instance.UpdateDocumentAttachments(detail);
                if (_DocAttachmentsID != Guid.Empty)
                    return "Succeed";
                else
                    return "Failure";
            }
            else
                return "Failure";

        }





    }
}
