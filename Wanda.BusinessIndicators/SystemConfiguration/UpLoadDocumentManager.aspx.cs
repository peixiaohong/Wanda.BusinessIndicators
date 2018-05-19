using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class UpLoadDocumentManager : System.Web.UI.Page
    {
        List<string> PermissionList = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                int finYear = datetime.Year;
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                FinsYear.DataSource = Year;
                FinsYear.DataBind();

                FinsYear.SelectedValue = finYear.ToString();
                DocMutipleUpload.FinValueYear = finYear.ToString();
                List<C_System> sysList = new List<C_System>();
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    foreach (var item in PermissionList)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                    }
                }
                if (sysList.Count > 0)
                {

                    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                }


                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();//绑定前台下拉框
                DocMutipleUpload.LoadByBusinessID("");



                //List<C_DocumentTree> Select1 = new List<C_DocumentTree>();
                //Select1 = C_DocumentTreeOperator.Instance.GetTypeList(Guid.Parse("99999999-9999-9999-9999-FFFFFFFFFFFF"));
                //ValueA.DataSource = Select1;
                //ValueA.DataTextField = "TreeNodeName";
                //ValueA.DataValueField = "ID";
                //ValueA.DataBind();//绑定前台下拉框
                //ValueA.Items.Insert(0, new ListItem("请选择", "0"));

            }
        }
        //系统(已隐藏)
        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SysID = ddlSystem.SelectedValue;
            hideTerrNodeId.Value = "";
            DocMutipleUpload.SystemId = SysID;
        }
        protected void FinYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            DocMutipleUpload.FinValueYear = FinsYear.SelectedValue;
        }
        //protected void ValueA_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DocMutipleUpload.ValueA = ValueA.SelectedValue;
        //    hidefile.Value = ValueA.SelectedValue;
        //    ValueB.Items.Clear();
        //    ValueB.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueB = "0";

        //    ValueC.Items.Clear();
        //    ValueC.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueC = "0";

        //    ValueD.Items.Clear();
        //    ValueD.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueD = "0";
        //    if (ValueA.SelectedValue != "0")
        //    {
        //        ValueB.Items.Clear();
        //        DocumentTree Select1 = new DocumentTree();
        //        Select1 = C_DocumentTreeOperator.Instance.GetList(Guid.Parse(ValueA.SelectedValue));
        //        if (!Select1.IsChildLastTree)
        //        {
        //            ValueB.DataSource = Select1.TreeList;
        //            ValueB.DataTextField = "TreeNodeName";
        //            ValueB.DataValueField = "ID";
        //            ValueB.DataBind();//绑定前台下拉框
        //        }

        //       ValueB.Items.Insert(0, new ListItem("请选择", "0"));
        //    }
        //}

        //protected void ValueB_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DocMutipleUpload.ValueB = ValueB.SelectedValue;
        //    ValueC.Items.Clear();
        //    ValueC.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueC = "0";

        //    ValueD.Items.Clear();
        //    ValueD.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueD = "0";
        //    if (ValueB.SelectedValue != "0")
        //    {
        //        ValueC.Items.Clear();
        //        DocumentTree Select1 = new DocumentTree();
        //        Select1 = C_DocumentTreeOperator.Instance.GetList(Guid.Parse(ValueB.SelectedValue));
        //        if (!Select1.IsChildLastTree)
        //        {
        //            ValueC.DataSource = Select1.TreeList;
        //            ValueC.DataTextField = "TreeNodeName";
        //            ValueC.DataValueField = "ID";
        //            ValueC.DataBind();//绑定前台下拉框
        //        }
        //        ValueC.Items.Insert(0, new ListItem("请选择", "0"));
        //    }
        //}
        //protected void ValueC_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DocMutipleUpload.ValueC = ValueC.SelectedValue;
        //    ValueD.Items.Clear();
        //    ValueD.Items.Insert(0, new ListItem("请选择", "0"));
        //    DocMutipleUpload.ValueD = "0";
        //    if (ValueC.SelectedValue != "0")
        //    {
        //        ValueD.Items.Clear();
        //        DocumentTree Select1 = new DocumentTree();
        //        Select1 = C_DocumentTreeOperator.Instance.GetList(Guid.Parse(ValueC.SelectedValue));
        //        if (!Select1.IsChildLastTree)
        //        {
        //            ValueD.DataSource = Select1.TreeList;
        //            ValueD.DataTextField = "TreeNodeName";
        //            ValueD.DataValueField = "ID";
        //            ValueD.DataBind();//绑定前台下拉框
        //        }
        //        ValueD.Items.Insert(0, new ListItem("请选择", "0"));
        //    }
        //}


        //protected void ValueD_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DocMutipleUpload.ValueD = ValueD.SelectedValue;
        //}


    }
}