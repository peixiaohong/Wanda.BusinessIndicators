using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Wanda.Financing.Entities;
using Wanda.Financing.Common;

namespace Wanda.Financing.Web.UserControls
{
    /// <summary>
    /// 需要两个参数
    /// </summary>
    public partial class AttachmentControlNoborder : BaseUserControl
    {
        #region 属性
        public bool IsShowTemplateFile { get; set; }
        /// <summary>
        /// 外部表的名称
        /// </summary>
        public string TableName
        {
            get
            {
                return (string)ViewState["FKTableName"];
            }
            set
            {
                ViewState["FKTableName"] = value;
            }
        }

        /// <summary>
        /// 外部表的记录的编号(FkTableKeyValue)
        /// </summary>
        public int TableRecordId
        {
            get
            {
                return Convert.ToInt32(ViewState["TableRecordId"] ?? "0");
            }
            set
            {
                ViewState["TableRecordId"] = value;
            }
        }

        /// <summary>
        /// 外部表的记录的编号(FkTableKeyName)
        /// </summary>
        public string TableKeyName
        {
            get
            {
                return (ViewState["TableKeyName"] ?? string.Empty).ToString();
            }
        }

        public string UploadUrl
        {
            get
            {
                return this.Page.ResolveUrl(string.Format("~/Projects/FileUpload.ashx?tb={0}&id={1}", TableName, TableRecordId));
            }
        }

        public string DeleteImgUrl
        {
            get
            {
                return this.Page.ResolveUrl("~/Images/cross.png");
            }
        }

        public string PreviewImgUrl
        {
            get
            {
                return this.Page.ResolveUrl("~/Image/ceico.gif");
            }
        }

        /// <summary>
        /// 是否使用编辑模式,当使用编辑模式时,可以上传删除
        /// </summary>
        public bool UseEditMode
        {
            get
            {
                return Convert.ToBoolean(ViewState["UseEditMode"] ?? "false");
            }
            set
            {
                ViewState["UseEditMode"] = value;
                ltlCtrl.Visible = value;
            }
        }

        /// <summary>
        /// 是否使用宽屏模式
        /// </summary>
        public bool UseWideMode
        {
            get
            {
                return Convert.ToBoolean(ViewState["UseWideMode"] ?? "true");
            }
            set
            {
                ViewState["UseWideMode"] = value;
            }
        }

        private List<Attachment> DataSource
        {
            get
            {
                return (List<Attachment>)ViewState["DataSource"];
            }
            set
            {
                ViewState["DataSource"] = value;
            }
        }

        private BizProcess CurrentBizProcess
        {
            get;
            set;
        }
        #endregion

        #region 页面事件
        protected void Page_Load(object sender, EventArgs e)
        {
            //目前有三个类型，流程，资产库子表（审批表），资产库表
            if (this.TableName.Equals("BizProcess", StringComparison.OrdinalIgnoreCase))
            {
                CurrentBizProcess = Bll.BizProcessBll.Instance.LoadBizProcess(this.TableRecordId);
            }
            if (this.TableName.Equals("BP19AssetList", StringComparison.OrdinalIgnoreCase))
            {
                CurrentBizProcess = Bll.BizProcessBll.Instance.LoadBizProcess(Bll.BP19AssetListBll.Instance.Load(this.TableRecordId).BizProcessID);
            }
            InitPage();
        }

        public void btnAddAttachment_Click(object sender, EventArgs e)
        {
            this.DataSource.Add(new Attachment() { IsEdit = true });
            BindData();
        }

        public void rptAttachment_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Attachment atta = e.Item.DataItem as Attachment;
                Image ibnDelete = e.Item.FindImage("ibnDelete");
                Image ibnPreview = e.Item.FindImage("ibnPreview");

                ibnDelete.Attributes.Add("onclick", "deleteAttachFile(this)");
                ibnDelete.Attributes.Add("attaid", atta.AttachmentID.ToString());

                BizProcess bp = Bll.BizProcessBll.Instance.LoadBizProcess(this.TableRecordId);
                if (bp != null && bp.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport)
                {
                    ibnPreview.Attributes.Add("onclick", "showAttachFile(this)");
                    ibnPreview.Attributes.Add("attaid", atta.AttachmentID.ToString());
                }
                else
                {
                    ibnPreview.Visible = false;
                }
                //if (!UseEditMode)
                if (CurrentBizProcess != null)
                {
                    switch ((Enumerator.ProcessStatus)CurrentBizProcess.Status)
                    {
                        case Enumerator.ProcessStatus.Approved: //已审批完成,不能删除
                        case Enumerator.ProcessStatus.Completed: //已办结,不能删除
                            {
                                if (CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport)
                                {
                                    ibnDelete.Visible = false;
                                    ibnPreview.Visible = true;
                                }
                                else
                                {
                                    e.Item.FindControl<HtmlTableCell>("ctlOperation").Controls.Clear();
                                }
                                break;
                            }
                        case Enumerator.ProcessStatus.Approving:  //审批中,只有当前用户能删除
                        case Enumerator.ProcessStatus.Returned:   //已退回,只有当前用户能删除
                            {

                                if (CurrentPage.CurrentUser.UserID != atta.CreateUserID)
                                {
                                    if (CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport)
                                    {
                                        ibnDelete.Visible = false;
                                        ibnPreview.Visible = true;
                                    }
                                    else
                                    {
                                        e.Item.FindControl<HtmlTableCell>("ctlOperation").Controls.Clear();
                                    }
                                }
                                else
                                {
                                    ibnDelete.Visible = true;
                                    ibnPreview.Visible = CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport;
                                }
                                break;
                            }
                        case Enumerator.ProcessStatus.Initialize: //未上报的和撤销的,都可以删除
                        case Enumerator.ProcessStatus.Canceled:
                            {
                                ibnPreview.Visible = CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport;
                                break;
                            }
                    }
                }
                e.Item.FindHyperLink("hyFileUrl").NavigateUrl = this.Page.ResolveUrl("~/Projects/FileResponse.ashx?attid=" + atta.AttachmentID.ToString());
                var ext = atta.ExtName.ToUpper();
                if (!(ext.Contains("XLS") || ext.Contains("XLSX")))
                {
                    ibnPreview.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.TableName) || this.TableRecordId == 0 || this.TableRecordId == DefaultValue.NullInt)
            {
                this.UseEditMode = false;
            }
            else
            {
                #region 新增 工作项(-) 当审批中或退回的流程,当前人不能上报或审批时,移除所有的删除按钮
                if (CurrentBizProcess != null && ((Enumerator.ProcessStatus)CurrentBizProcess.Status == Enumerator.ProcessStatus.Approving || (Enumerator.ProcessStatus)CurrentBizProcess.Status == Enumerator.ProcessStatus.Returned))
                {
                    UseEditMode = HttpContext.Current.Items.Contains("CanApproved");

                    if (CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport && (CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Approved || CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Approving || CurrentBizProcess.Status == (int)Enumerator.ProcessStatus.Completed))
                    {
                        UseEditMode = false;
                    }
                }

                foreach (RepeaterItem ri in this.rptAttachment.Items)
                {
                    if (ri.FindControl<HtmlTableCell>("ctlOperation").Controls.Count == 0)
                    {
                        ri.FindControl<HtmlTableCell>("ctlOperation").InnerText = "-";
                    }
                }
            }
            paneAddFile.Visible = UseEditMode;
            ltlCtrl.Visible = UseEditMode;
                #endregion

            if (!UseEditMode && this.DataSource.Count == 0 && UseWideMode)
            {
                this.Visible = false;
            }

            if (UseEditMode)
            {
                CurrentPage.ClientScript.RegisterStartupScript(this.GetType(), "AttachmentControlInit", "if(window['initUploadCtl']){$(function(){initUploadCtl();})};", true);
            }
            else
            {
                if (CurrentBizProcess != null && CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport)
                {
                    this.UseEditMode = true;
                    foreach (RepeaterItem ri in this.rptAttachment.Items)
                    {
                        if (ri.ItemType == ListItemType.Item)
                        {
                            ri.FindControl("ibnDelete").Visible = false;
                        }
                    }
                }
            }
            InitPage();
        }
        #endregion

        #region 私有方法
        private void InitPage()
        {
            if (IsShowTemplateFile && this.CurrentBizProcess != null)
            {
                if (this.CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.CeoDesktopDataImport)
                {
                    hyCeoExcelTemplateFile.Visible = true;
                }
                else
                {
                    if (this.CurrentBizProcess.ProcessType == (int)Enumerator.ProcessType.FinancingPlan)
                    {
                        linkTemplateFinancingplan.Visible = true;
                    }
                    else
                    {

                        this.hylinkTemplateFile.Visible = true;
                    }
                }
            }
            DataSource = Bll.AttachmentBll.Instance.LoadAttachmentByFK(this.TableName, this.TableRecordId);
            BindData();

        }

        public void BindData()
        {
            if (this.DataSource != null && this.DataSource.Count > 0)
            {
                this.DataSource.OrderBy(item => item.CreateDate).ToList();
                this.rptAttachment.DataSource = DataSource;
                this.rptAttachment.DataBind();
                trEmptyPane.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                this.rptAttachment.DataSource = DataSource;
                this.rptAttachment.DataBind();
                trEmptyPane.Style.Add(HtmlTextWriterStyle.Display, null);
            }
        }

        #endregion
    }
}