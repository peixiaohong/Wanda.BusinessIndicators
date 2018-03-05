using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.LightWorkflow.Dal;
using Wanda.LightWorkflow.Entities;

namespace WorkFlowDemo
{
    public partial class TodoWorkList : System.Web.UI.Page
    {
        private string bizid = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            int uid = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["uid"]))
            {
                int.TryParse(Request.QueryString["uid"], out uid);
            }
            if (!IsPostBack)
            {
                List<TodoWork> list = TodoWorkAdapter.Instance.LoadListByUserIDandBizProecessIds(uid, "");
                List<TodoViewModel> dataList = new List<TodoViewModel>();
                
                foreach (var item in list)
                {
                    ProcessInstance pi= ProcessInstanceAdapter.Instance.Load(item.ProcessInstanceID);
                    dataList.Add(new TodoViewModel()
                     {
                         
                         BizID = pi.BizProcessID,
                         Name=item.NodeName,
                         UserID=uid.ToString()
                     });
                    
                }
                this.Rpt.DataSource = dataList;
                this.Rpt.DataBind();
            }
        }
        internal class TodoViewModel{
            public string BizID { get; set; }
            public string Name { get; set; }
            public string UserID { get; set; }
        }
    }
}