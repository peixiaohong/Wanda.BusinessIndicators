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
    public class B_DocumentAttachmentsOperator : BizOperatorBase<B_DocumentAttachments>
    {
        public static readonly B_DocumentAttachmentsOperator Instance = PolicyInjection.Create<B_DocumentAttachmentsOperator>();

        private static B_DocumentAttachmentsAdapter _bDocumentAttachmentsAdapter = AdapterFactory.GetAdapter<B_DocumentAttachmentsAdapter>();

        protected override BaseAdapterT<B_DocumentAttachments> GetAdapter()
        {
            return _bDocumentAttachmentsAdapter;
        }

        internal IList<B_DocumentAttachments> GetDocumentAttachmentsList()
        {
            IList<B_DocumentAttachments> result = _bDocumentAttachmentsAdapter.GetDocumentAttachmentsList();
            return result;
        }

        public List<B_DocumentAttachments> GetAllFile() {
            List<B_DocumentAttachments> result = _bDocumentAttachmentsAdapter.GetDocumentAttachmentsList().ToList();
            return result;
        
        }
        public Guid AddDocumentAttachments(B_DocumentAttachments data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }


        public List<B_DocumentAttachments> GetListByBID(Guid BusinessID)
        {
            List<B_DocumentAttachments> result = _bDocumentAttachmentsAdapter.GetListByBID(BusinessID).ToList();

            return result;
        }

        public B_DocumentAttachments GetDocumentAttachments(Guid bDocumentAttachmentsID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bDocumentAttachmentsID == null, "Argument bDocumentAttachmentsID is Empty");
            return base.GetModelObject(bDocumentAttachmentsID);
        }
      

        public Guid UpdateDocumentAttachments(B_DocumentAttachments data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveDocumentAttachments(Guid bDocumentAttachmentsID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bDocumentAttachmentsID == null, "Argument bDocumentAttachmentsID is Empty");
            Guid result = base.RemoveObject(bDocumentAttachmentsID);
            return result;
        }

        /// <summary>
        /// 获取List
        /// </summary>
        /// <param name="bDocumentAttachmentsID"></param>
        /// <returns></returns>
        public List<B_DocumentAttachments> GetDocAttachmentsList(Guid BusinessID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(BusinessID == null, "Argument TreeNodeID is Empty");
            List<B_DocumentAttachments> list = new List<B_DocumentAttachments>();
            if (BusinessID != Guid.Empty)
            {
                list = GetListByBID(BusinessID);
            }
        


            return list;

        }
        public List<B_DocumentAttachments> GetListByValueA(Guid ValueA)
        {
            List<B_DocumentAttachments> result = _bDocumentAttachmentsAdapter.GetListByValueA(ValueA).ToList();
            return result;

        }


        public List<B_DocumentAttachments> GetDocAttachmentsList(Guid BusinessID, string FileName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(BusinessID == null, "Argument TreeNodeID is Empty");

            List<B_DocumentAttachments> list = _bDocumentAttachmentsAdapter.GetDocumentAttachmentsListBySearch(BusinessID, FileName).ToList();
            return list;
            
        }
        
        public List<B_DocumentAttachments> GetDocumentAttachmentsListByName(string FileName)
        {


            List<B_DocumentAttachments> list = _bDocumentAttachmentsAdapter.GetDocumentAttachmentsListByName( FileName).ToList();
            return list;

        }

    }
}
