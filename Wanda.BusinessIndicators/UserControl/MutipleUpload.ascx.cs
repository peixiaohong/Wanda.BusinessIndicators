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


namespace LJTH.Fiscal.Budget.Web.Control
{
    public partial class MutipleUpload : System.Web.UI.UserControl
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
          //  lbTitle.Text = Title;

            if (Request.QueryString["isDone"] == "1")
            {
                attaView.Disabled = true;
            }
        }

        public void LoadByBusinessID(string businessID)
        {
            string regScript = @"ID: '{0}',
                    FileName: '{1}',
                    FileSize:'{2}',
                    Url: '{3}',
                    CreatorName: '{4}',
                    CreateTime: '{5}'";

            AttachmentFilter filter = new AttachmentFilter();
            filter.BusinessID = businessID;
            if (string.IsNullOrEmpty(AttachmentType) == false)
            {
                filter.BusinessType = AttachmentType;
            }

            IList<B_Attachment> attachments = B_AttachmentOperator.Instance.GetAttachmentList(Guid.Parse(businessID), "月报上传");

            StringBuilder sScript = new StringBuilder("");
            for (int i = 0; i < attachments.Count; i++)
            {
                sScript.Append(this.ClientID + "_Attachments[" + i.ToString() + "] = {");
                sScript.Append(string.Format(regScript, new string[]{attachments[i].ID.ToString(),
                attachments[i].FileName,
                attachments[i].Size,
                HttpUtility.UrlEncode(attachments[i].Url),
                attachments[i].CreatorName,
                attachments[i].CreateTime.ToShortDateString()
                }));
                sScript.Append("};");
            }
            sScript.Append(this.ClientID + "_Show();");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Initial", sScript.ToString(), true);
        }
    }
}