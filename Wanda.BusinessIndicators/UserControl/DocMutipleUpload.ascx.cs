using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.UserControl
{
    /// <summary>
    /// 该类只使用文档分类管理，不适用其它的地方
    /// </summary>
    public partial class DocMutipleUpload : System.Web.UI.UserControl
    {
        protected string UploudFilePath = ConfigurationManager.AppSettings["UploadFilePath"];
        
        public string AttachmentType
        {
            get
            {
                if (ViewState["AttachmentType"] == null)
                {
                    ViewState["AttachmentType"] = "";
                }
                return ViewState["AttachmentType"].ToString();
            }
            set
            {
                ViewState["AttachmentType"] = value;
            }
        }
        
        public string BusinessID
        {
            get
            {
                if (ViewState["BusinessID"] == null)
                {
                    ViewState["BusinessID"] = "";
                }
                return ViewState["BusinessID"].ToString();
            }
            set
            {
                ViewState["BusinessID"] = value;
            }
        }

        public string ValueA
        {
            get
            {
                if (ViewState["ValueA"] == null)
                {
                    ViewState["ValueA"] = "";
                }
                return ViewState["ValueA"].ToString();
            }
            set
            {
                ViewState["ValueA"] = value;
            }
        }
        public string ValueB
        {
            get
            {
                if (ViewState["ValueB"] == null)
                {
                    ViewState["ValueB"] = "";
                }
                return ViewState["ValueB"].ToString();
            }
            set
            {
                ViewState["ValueB"] = value;
            }
        }
        public string ValueC
        {
            get
            {
                if (ViewState["ValueC"] == null)
                {
                    ViewState["ValueC"] = "";
                }
                return ViewState["ValueC"].ToString();
            }
            set
            {
                ViewState["ValueC"] = value;
            }
        }
        public string ValueD
        {
            get
            {
                if (ViewState["ValueD"] == null)
                {
                    ViewState["ValueD"] = "";
                }
                return ViewState["ValueD"].ToString();
            }
            set
            {
                ViewState["ValueD"] = value;
            }
        }
        public string FinValueYear
        {
            get
            {
                if (ViewState["FinValueYear"] == null)
                {
                    ViewState["FinValueYear"] = "";
                }
                return ViewState["FinValueYear"].ToString();
            }
            set
            {
                ViewState["FinValueYear"] = value;
            }
        }
        public bool ShowOnly
        {
            get;
            set;
        }

        public string Title
        {
            get
            {
                if (ViewState["Title"] == null)
                {
                    ViewState["Title"] = "";
                }
                return ViewState["Title"].ToString();
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public string Note
        {
            get;
            set;
        }

        public string FileFilter
        {
            get
            {
                if (ViewState["FileFilter"] == null)
                {
                    ViewState["FileFilter"] = "*.*";

                    //ViewState["FileFilter"] = "*.doc; *.docx; *.xls;*.xlsx;*.zip;*.rar";
                }
                return ViewState["FileFilter"].ToString();
            }
            set
            {
                ViewState["FileFilter"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lbTitle.InnerText = Title;
            lbNote.InnerText = string.IsNullOrEmpty(Note) ? "" : Note;
            if (string.IsNullOrEmpty(Note))
            {
                lbIcon.Visible = false;
            }

            if (ShowOnly)
            {
                AttaUpload.Disabled = true;
                AttaUpload.Visible = false;
            }
        }

        public void LoadByBusinessID(string businessID)
        {
            string regScript = @"ID: '{0}',
                    FileName: '{1}',
                    FileSize:'{2}',
                    Url: '{3}',
                    CreatorName: '{4}',
                    CreateTime: '{5}',
                    Remark:{6}";

            DocumentAttachments filter = new DocumentAttachments();
            filter.BusinessID = businessID;
            if (string.IsNullOrEmpty(AttachmentType) == false)
            {
                filter.BusinessType = AttachmentType;
            }
            IList<B_DocumentAttachments> attachments = B_DocumentAttachmentsOperator.Instance.GetDocAttachmentsList(businessID.ToGuid() );

            if (attachments.Count == 0 && this.ShowOnly)
            {
                this.Visible = false; return;
            }
           

            if (attachments.Count > 0)
            {
                StringBuilder sScript = new StringBuilder("");
                for (int i = 0; i < attachments.Count; i++)
                {
                    sScript.Append(this.ClientID + "_Attachments[" + i.ToString() + "] = {");
                    sScript.Append(string.Format(regScript, new string[]{attachments[i].ID.ToString(),
                    attachments[i].FileName,
                    attachments[i].Size,
                    HttpUtility.UrlEncode(attachments[i].Url),
                    attachments[i].CreatorName,
                    attachments[i].CreateTime.ToShortDateString(),
                    attachments[i].Remark,
                    attachments[i].SystemID.ToString()
                    }));
                    sScript.Append("};");
                }
                sScript.Append(this.ClientID + "_Show();");
                ScriptManager.RegisterStartupScript(this, this.GetType(), this.ClientID, sScript.ToString(), true);
            }
        }

//        public void LoadByIDS(List<int> ids)
//        {
//            string regScript = @"ID: '{0}',
//                    FileName: '{1}',
//                    FileSize:'{2}',
//                    Url: '{3}',
//                    CreatorName: '{4}',
//                    CreateTime: '{5}'";

//            IList<B_DocumentAttachments> attachments = AttachmentOperator.Instance.GetAttachmentListByIDS(ids);
//            if (attachments.Count == 0 && this.ShowOnly)
//            {
//                this.Visible = false; return;
//            }
//            StringBuilder sScript = new StringBuilder("");
//            for (int i = 0; i < attachments.Count; i++)
//            {
//                sScript.Append(this.ClientID + "_Attachments[" + i.ToString() + "] = {");
//                sScript.Append(string.Format(regScript, new string[]{attachments[i].ID.ToString(),
//                attachments[i].FileName,
//                attachments[i].Size,
//                HttpUtility.UrlEncode(attachments[i].Url),
//                null== UserinfoOperator.Instance.GetUserInfoByName(attachments[i].CreatorName)?"":UserinfoOperator.Instance.GetUserInfoByName(attachments[i].CreatorName).DisplayName,
//                attachments[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
//                }));
//                sScript.Append("};");
//            }
//            sScript.Append(this.ClientID + "_Show();");
//            ScriptManager.RegisterStartupScript(this, this.GetType(), "Initial", sScript.ToString(), true);
//        }


    }
}