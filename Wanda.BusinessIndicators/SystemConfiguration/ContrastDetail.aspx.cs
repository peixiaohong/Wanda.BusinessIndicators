using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.BLL;
using Lib.Core;
namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class ContrastDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<C_System> sysList = new List<C_System>();
            if (!IsPostBack)
            {
                List<Guid> IDList = new List<Guid>();
                List<C_System> List = C_SystemOperator.Instance.GetSystemListBySeq().ToList();//先取出所有
                foreach (C_System item in List)//将他们的ID存入List
                {
                    IDList.Add(item.ID);
                }
                IDList = IDList.Distinct().ToList();//去重    
                List<C_System> Csys = new List<C_System>();
                foreach (Guid item in IDList)
                {
                    Csys.Add(StaticResource.Instance[item, DateTime.Now]);
                }

                ddlSystem.DataSource = Csys;
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();

                ddlYear.SelectedValue = DateTime.Now.Year.ToString();


                List<int> Month = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(i);
                }
                ddlMonth.DataSource = Month;
                ddlMonth.DataBind();

                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            }

            if (Request.QueryString["SystemID"] != null)
            {
                FinMonth.Value = Request.QueryString["FinMonth"];
                FinYear.Value = Request.QueryString["FinYear"];
                SysID.Value = Request.QueryString["SystemID"];
            }
           

        }
       
    }
}