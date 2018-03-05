using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.LightWorkflow;
using Wanda.LightWorkflow.Dal;
using Wanda.LightWorkflow.Entities;

namespace WorkFlowDemo
{
    public partial class Approval : System.Web.UI.Page
    {
        private WorkflowInstance _instance = null;
        private TodoWork _todoWork = null;
        private int uid = 0;
        protected override void OnPreInit(EventArgs e)
        {
            string bid = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["bid"]))
            {
                bid = Request.QueryString["bid"];
            }
            
            if (!string.IsNullOrEmpty(Request.QueryString["uid"]))
            {
                int.TryParse(Request.QueryString["uid"], out uid);
            }
            ProcessInstance p = ProcessInstanceAdapter.Instance.LoadByBizProcessID(bid);
            _instance = new WorkflowInstance(p);
            List<TodoWork> td = _instance.TodoWorks;
            foreach (var item in td)
            {
                if (item.UserID == uid)
                {
                    _todoWork = item; break;
                }
            }
            base.OnPreInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                List<ApprovalLog> al = _instance.ApprovalLogs;
                List<ApprovalViewMode> avm = new List<ApprovalViewMode>();
                foreach (var item in al)
                {
                    avm.Add(new ApprovalViewMode()
                    {
                        ApprovalNote = item.ApprovalNote,
                        CompleteTime = item.CompletedTime,
                        UserName = item.UserName
                    });
                }
                this.Rpt.DataSource = avm;
                this.Rpt.DataBind();

            }
            if (_instance == null)
            {
                approval.Disabled = true;
            }
        }
        protected void Apply_Click(object sender, EventArgs e)
        {
            string approvalNote=approvalNotes.InnerText;
           
            if (_instance != null&&_todoWork!=null)
            {
                ProcessNodeInstance nodeInstance=ProcessNodeInstanceAdapter.Instance.Load(_todoWork.NodeInstanceID);
                _instance.Submit(approvalNote, uid, _todoWork.ID);
            }
        }
        protected void Reject_Click(object sender, EventArgs e)
        {
            string approvalNote = approvalNotes.InnerText;

            if (_instance != null && _todoWork != null)
            {
                ProcessNodeInstance nodeInstance = ProcessNodeInstanceAdapter.Instance.Load(_todoWork.NodeInstanceID);
                _instance.Reject(approvalNote, uid);
            }
        }
        internal class ApprovalViewMode
        {
            public string ApprovalNote { get; set; }
            public string UserName { get; set; }
            public DateTime CompleteTime { get; set; }
        }
    }
}