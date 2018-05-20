using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.DAL;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.BLL
{
    public class B_AttachmentOperator : BizOperatorBase<B_Attachment>
    {
        #region Generate Code

        public static readonly B_AttachmentOperator Instance = PolicyInjection.Create<B_AttachmentOperator>();

        private static B_AttachmentAdapter _bAttachmentAdapter = AdapterFactory.GetAdapter<B_AttachmentAdapter>();

        protected override BaseAdapterT<B_Attachment> GetAdapter()
        {
            return _bAttachmentAdapter;
        }

        public IList<B_Attachment> GetAttachmentList()
        {
            IList<B_Attachment> result = _bAttachmentAdapter.GetAttachmentList();
            return result;
        }

        public IList<B_Attachment> GetAttachmentList(Guid businessID)
        {
            IList<B_Attachment> result = _bAttachmentAdapter.GetAttachmentList(businessID);
            return result;
        }
        public IList<B_Attachment> GetAttachmentList(string businessIDs, string businessType)
        {
            IList<B_Attachment> result = _bAttachmentAdapter.GetAttachmentList(businessIDs, businessType);
            return result;
        }

        public IList<B_Attachment> GetAttachmentList(Guid businessID,string businessType)
        {
            IList<B_Attachment> result = _bAttachmentAdapter.GetAttachmentList(businessID , businessType );
            return result;
        }



        public Guid AddAttachment(B_Attachment data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_Attachment GetAttachment(Guid bAttachmentID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bAttachmentID == null, "Argument aTargetplanID is Empty");
            return base.GetModelObject(bAttachmentID);
        }

        public Guid UpdateAttachment(B_Attachment data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveAttachment(Guid bAttachmentID) 
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bAttachmentID == null, "Argument aTargetplanID is Empty");
            Guid result = base.RemoveObject(bAttachmentID);
            return result;
        }

        #endregion
    }
}
