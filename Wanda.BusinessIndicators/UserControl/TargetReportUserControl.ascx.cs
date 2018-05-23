using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LJTH.BusinessIndicators.Web.UserControl
{
    public partial class TargetReportUserControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// 模板下载
        /// </summary>
        private bool downLoadTemplateState;

        public bool DownLoadTemplateState
        {
            get { return downLoadTemplateState; }
            set { downLoadTemplateState = value; }
        }
        /// <summary>
        /// 数据上传
        /// </summary>
        private bool dataUploadState;

        public bool DataUploadState
        {
            get { return dataUploadState; }
            set { dataUploadState = value; }
        }
        /// <summary>
        /// 未完成说明
        /// </summary>
        private bool missTargetReportState;

        public bool MissTargetReportState
        {
            get { return missTargetReportState; }
            set { missTargetReportState = value; }
        }

        /// <summary>
        /// 月报说明
        /// </summary>
        private bool monthReportState = false;

        public bool MonthReportState
        {
            get { return monthReportState; }
            set { monthReportState = value; }
        }
        /// <summary>
        /// 月报上报
        /// </summary>
        private bool monthReportSubmitState = false;

        public bool MonthReportSubmitState
        {
            get { return monthReportSubmitState; }
            set { monthReportSubmitState = value; }
        }


        /// <summary>
        /// 保存
        /// </summary>
        private bool monthReportReadyState = false;

        public bool MonthReportReadyState
        {
            get { return monthReportReadyState; }
            set { monthReportReadyState = value; }
        }


        public string CurrentControlID
        {
            get { return ItemStateHidden.Value ; }
            //set { currentControlID = value; }
        }


        public delegate void CalculateDelegate(string ddlValue, string keyWords); //定义全局委托
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void InitData(int itemID)
        {
            string showSpanID = "";
            if (downLoadTemplateState == true||itemID>=1)
            {
                showSpanID += "downLoadTemplateSpan,";
                if (dataUploadState == true || itemID >= 2)
                {
                    showSpanID += "dataUploadSpan,";
                    if (missTargetReportState == true || itemID >= 3)
                    {
                        showSpanID += "missTargetReportSpan,missCurrentTargetReportSpan,"; 
                        if (monthReportState == true || itemID >= 4)
                        {
                            showSpanID += "monthReportSpan,";
                            if (monthReportReadyState  == true || itemID ==5)
                            {
                                showSpanID += "monthReportReadySpan,";
                                showSpanID += "monthReportSubmitSpan,";

                                //if (monthReportSubmitState == true || itemID == 5)
                                //{
                                //    showSpanID += "monthReportSubmitSpan,";
                                //}
                            }
                        }
                    }
                }
            }
            
            if(showSpanID!="")
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "_setStlye", "<script>$('#ReportedDone').addClass('hide');setStlye('" + showSpanID + "');</script>");
            else
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "_setdisable", "<script>$('#ReportedDone').removeClass('hide');</script>");

        }
        /// <summary>
        /// 设置要选中的button
        /// </summary>
        /// <param name="itemID">用户控件中细项的ID</param>
        public void SetButtonStyle(string itemID)
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(),"MyFunction","<script>ClickItems('" + itemID + "');</script>");
        }

        /// <summary>
        /// 设置要激活的button
        /// </summary>
        /// <param name="itemID">用户控件中细项的ID</param>
        public void SetButtonSpanStyle(int ItemID)
        {
            InitData(ItemID);
           
        }

      

    }
}