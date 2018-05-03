using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web;
using LJTH.BusinessIndicators.Web.AjaxHandler;
using Newtonsoft.Json;
using Lib.Core;
using Lib.Xml;
using Lib.Expression;
using Wanda.Platform.WorkFlow.ClientComponent;
using System.Web;
using BPF.Workflow.Object;
using BPF.Workflow.Client;
using System.Data;
using System.Web.Configuration;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class TargetController : BaseController
    {
        [LibAction]
        public C_System GetSystem(string SysID)
        {
            C_System result = C_SystemOperator.Instance.GetSystem(Guid.Parse(SysID));
            return result;
        }

        [LibAction]
        public C_System GetNowSystem(string SysID)
        {
            C_System result = StaticResource.Instance[Guid.Parse(SysID), DateTime.Now];
            return result;
        }
        [LibAction]
        public List<TargetDetail> GetSumMonthTargetDetail(string FinYear, string SystemID)
        {
            List<TargetDetail> view = A_TargetplandetailOperator.Instance.GetSumMonthTargetDetail(int.Parse(FinYear), Guid.Parse(SystemID));
            return view;
        }
        [LibAction]
        public List<TargetDetail> GetSumMonthTargetDetailByTID(string TargetPlanID)
        {
            List<TargetDetail> view = A_TargetplandetailOperator.Instance.GetSumMonthTargetDetailByTID(Guid.Parse(TargetPlanID));
            return view;
        }

        [LibAction]
        public List<TargetPlanDetailVList> GetSumTargetDetail(string FinYear, string SystemID)
        {
            List<TargetPlanDetailVList> result = A_TargetplandetailOperator.Instance.GetSumTargetDetail(int.Parse(FinYear), Guid.Parse(SystemID));
            return result;
        }

        #region
        /// <summary>
        /// 将审批流程进行了分组
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        [LibAction]
        public List<V_ReportApprove> GetAllReportApproveGroup(int FinYear, int FinMonth)
        {
            List<V_ReportApprove> AllReportApprove = new List<V_ReportApprove>();
            List<B_MonthlyReport> result = B_MonthlyreportOperator.Instance.GetMonthlyReportByApproveList(FinYear, FinMonth);

            for (int i = 0; i < result.Count; i++)
            {
                // this.BusinessID = result[i].ID.ToString();
                //List<NavigatActivity> list = new List<NavigatActivity>();

                if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                {
                    List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                    if (list.Count > 0)
                    {
                        V_ReportApprove model = new V_ReportApprove();
                        int num = 0;//默认为0,当AllReportApprove不为空时,循环AllReportApprove,对比是否与现在取出的流程的SystemID相同
                        if (AllReportApprove.Count > 0)
                        {
                            for (int s = 0; s < AllReportApprove.Count; s++)
                            {
                                if (AllReportApprove[s].SystemID == result[i].SystemID)
                                {
                                    num++;//若SystemID匹配到,则++,并Add该AllReportApprove
                                    AllReportApprove[s].list.Add(list);
                                }
                            }
                        }
                        else
                        {
                            num++;

                            model.SystemName = C_SystemOperator.Instance.GetSystem(result[i].SystemID).SystemName;

                            model.SystemID = result[i].SystemID;
                            List<List<NavigatActivity>> allList = new List<List<NavigatActivity>>();
                            model.list = allList;
                            model.list.Add(list);
                            AllReportApprove.Add(model);
                        }
                        if (num == 0)//若num为0,则代表没有匹配到,并且不是首项,则creat新model
                        {

                            model.SystemName = C_SystemOperator.Instance.GetSystem(result[i].SystemID).SystemName;


                            model.SystemID = result[i].SystemID;
                            model.CreatTime = result[i].CreateTime.ToString("yyyy-MM-dd");

                            model.list = new List<List<NavigatActivity>>(); ;
                            model.list.Add(list);
                            AllReportApprove.Add(model);
                        }
                    }

                }

            }
            return AllReportApprove;
        }
        /// <summary>
        /// 没有将审批流程进行分组
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        [LibAction]
        public List<V_ReportApprove> GetAllReportApprove(string SystemID, int FinYear, int FinMonth)
        {
            List<V_ReportApprove> AllReportApprove = new List<V_ReportApprove>();
            if (SystemID == "all")
            {
                List<B_MonthlyReport> result = B_MonthlyreportOperator.Instance.GetMonthlyReportByAllList(FinYear, FinMonth);
                DataTable result1 = B_MonthlyreportOperator.Instance.GetMonthlyReportByGroupList(FinYear, FinMonth);
                List<B_SystemBatch> SysBatch = B_SystemBatchOperator.Instance.GetSystemBatchList("ProSystem", FinYear, FinMonth);
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                    {
                        List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                        V_ReportApprove model = new V_ReportApprove();

                        C_System sys = C_SystemOperator.Instance.GetSystem(result[i].SystemID);
                        model.SystemName = sys.SystemName;
                        model.seq = sys.Sequence;
                        model.Category = sys.Category;

                        if (result[i].WFStatus == "Approved")
                        {
                            model.IfComplate = true;
                        }
                        else
                        {
                            model.IfComplate = false;
                        }
                        if (result1 != null)
                        {
                            for (int j = 0; j < result1.Rows.Count; j++)
                            {
                                var sysID = result1.Rows[j]["SystemID"].ToString();
                                var datetime = DateTime.Parse(result1.Rows[j]["CreateTime"].ToString());
                                var sysCount = int.Parse(result1.Rows[j]["SysCount"].ToString());

                                if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") == datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = sysCount; break;
                                }
                                else if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") != datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = 0; break;
                                }
                            }
                        }
                        else
                        {
                            model.Group = -1;
                        }
                        model.BusinessID = result[i].ID;
                        model.SystemID = result[i].SystemID;
                        model.WFStatus = result[i].WFStatus;
                        model.ReportApprove = result[i].ReportApprove;
                        // model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                        model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                        model.list = new List<List<NavigatActivity>>();
                        model.list.Add(list);
                        AllReportApprove.Add(model);

                    }
                }
                if (SysBatch.Count > 0)
                {
                    int index = 0;
                    int num = 0;
                    for (int i = 0; i < SysBatch.Count; i++)
                    {
                        if (SysBatch[i].ReportApprove != null && SysBatch[i].ReportApprove != "")
                        {
                            index++;
                        }
                    }
                    for (int j = 0; j < SysBatch.Count; j++)
                    {

                        if (SysBatch[j].ReportApprove != null && SysBatch[j].ReportApprove != "")
                        {
                            num++;
                            V_ReportApprove model = new V_ReportApprove();
                            model.SystemName = "项目系统合并后流程";
                            model.seq = 121 + j;
                            model.IfComplate = false;
                            if (SysBatch[j].WFBatchStatus == "Approved")
                            {
                                model.IfComplate = true;
                            }
                            model.ReportApprove = SysBatch[j].ReportApprove;
                            model.WFStatus = SysBatch[j].WFBatchStatus;
                            if (num == 1)
                            {
                                model.Group = index;
                            }
                            else
                            {
                                model.Group = 0;
                            }

                            List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(SysBatch[j].ReportApprove);
                            model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                            model.list = new List<List<NavigatActivity>>();
                            model.list.Add(list);
                            AllReportApprove.Add(model);
                        }
                    }
                }
                AllReportApprove = AllReportApprove.OrderByDescending(a => a.CreatTime).OrderBy(a => a.seq).ToList();
            }
            else
            {
                List<B_MonthlyReport> result = B_MonthlyreportOperator.Instance.GetMonthlyReportBySysIDList(Guid.Parse(SystemID), FinYear, FinMonth);
                DataTable result1 = B_MonthlyreportOperator.Instance.GetMonthlyReportByGroupList(FinYear, FinMonth);
                List<B_SystemBatch> SysBatch = B_SystemBatchOperator.Instance.GetSystemBatchList("ProSystem", FinYear, FinMonth);
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                    {
                        List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                        V_ReportApprove model = new V_ReportApprove();

                        C_System sys = C_SystemOperator.Instance.GetSystem(result[i].SystemID);
                        model.SystemName = sys.SystemName;
                        model.seq = sys.Sequence;
                        model.Category = sys.Category;

                        if (result[i].WFStatus == "Approved")
                        {
                            model.IfComplate = true;
                        }
                        else
                        {
                            model.IfComplate = false;
                        }
                        if (result1 != null)
                        {
                            for (int j = 0; j < result1.Rows.Count; j++)
                            {
                                var sysID = result1.Rows[j]["SystemID"].ToString();
                                var datetime = DateTime.Parse(result1.Rows[j]["CreateTime"].ToString());
                                var sysCount = int.Parse(result1.Rows[j]["SysCount"].ToString());

                                if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") == datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = sysCount; break;
                                }
                                else if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") != datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = 0; break;
                                }
                            }
                        }
                        else
                        {
                            model.Group = -1;
                        }
                        model.BusinessID = result[i].ID;
                        model.SystemID = result[i].SystemID;
                        model.WFStatus = result[i].WFStatus;
                        model.ReportApprove = result[i].ReportApprove;
                        // model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                        model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                        model.list = new List<List<NavigatActivity>>();
                        model.list.Add(list);
                        AllReportApprove.Add(model);

                    }
                }
                if (SysBatch.Count > 0)
                {
                    int index = 0;
                    int num = 0;
                    for (int i = 0; i < SysBatch.Count; i++)
                    {
                        if (SysBatch[i].ReportApprove != null && SysBatch[i].ReportApprove != "")
                        {
                            index++;
                        }
                    }
                    for (int j = 0; j < SysBatch.Count; j++)
                    {


                        if (SysBatch[j].ReportApprove != null && SysBatch[j].ReportApprove != "")
                        {
                            num++;
                            V_ReportApprove model = new V_ReportApprove();
                            model.SystemName = "项目系统合并后流程";
                            model.seq = 121 + j;
                            model.IfComplate = false;
                            if (SysBatch[j].WFBatchStatus == "Approved")
                            {
                                model.IfComplate = true;
                            }
                            model.ReportApprove = SysBatch[j].ReportApprove;
                            model.WFStatus = SysBatch[j].WFBatchStatus;
                            if (num == 1)
                            {
                                model.Group = index;
                            }
                            else
                            {
                                model.Group = 0;
                            }
                            List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(SysBatch[j].ReportApprove);
                            model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                            model.list = new List<List<NavigatActivity>>();
                            model.list.Add(list);
                            AllReportApprove.Add(model);
                        }
                    }
                }
                AllReportApprove = AllReportApprove.OrderByDescending(a => a.CreatTime).OrderBy(a => a.seq).ToList();
            }

            return AllReportApprove;

        }

        /// <summary>
        /// 根据systemID查询审批流程 /月报
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        [LibAction]
        public List<V_ReportApprove> GetSysReportApprove(string SysID, int FinYear, int FinMonth)
        {
            List<V_ReportApprove> AllReportApprove = new List<V_ReportApprove>();
            List<B_MonthlyReport> result = B_MonthlyreportOperator.Instance.GetMonthlyReportBySysIDList(Guid.Parse(SysID), FinYear, FinMonth);
            DataTable result1 = B_MonthlyreportOperator.Instance.GetMonthlyReportByGroupList(FinYear, FinMonth);
            List<B_SystemBatch> SysBatch = B_SystemBatchOperator.Instance.GetSystemBatchList("ProSystem", FinYear, FinMonth);
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                {
                    List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                    V_ReportApprove model = new V_ReportApprove();

                    C_System sys = C_SystemOperator.Instance.GetSystem(result[i].SystemID);
                    model.SystemName = sys.SystemName;
                    model.seq = sys.Sequence;
                    model.Category = sys.Category;

                    if (result[i].WFStatus == "Approved")
                    {
                        model.IfComplate = true;
                    }
                    else
                    {
                        model.IfComplate = false;
                    }
                    if (result1 != null)
                    {
                        for (int j = 0; j < result1.Rows.Count; j++)
                        {
                            var sysID = result1.Rows[j]["SystemID"].ToString();
                            var datetime = DateTime.Parse(result1.Rows[j]["CreateTime"].ToString());
                            var sysCount = int.Parse(result1.Rows[j]["SysCount"].ToString());

                            if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") == datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                            {
                                model.Group = sysCount; break;
                            }
                            else if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") != datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                            {
                                model.Group = 0; break;
                            }
                        }
                    }
                    else
                    {
                        model.Group = -1;
                    }
                    model.BusinessID = result[i].ID;
                    model.SystemID = result[i].SystemID;
                    // model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                    model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                    model.list = new List<List<NavigatActivity>>();
                    model.list.Add(list);
                    AllReportApprove.Add(model);

                }
            }
            if (SysBatch.Count > 0)
            {
                int index = 0;
                int num = 0;
                for (int i = 0; i < SysBatch.Count; i++)
                {
                    if (SysBatch[i].ReportApprove != null && SysBatch[i].ReportApprove != "")
                    {
                        index++;
                    }
                }
                for (int j = 0; j < SysBatch.Count; j++)
                {
                    if (SysBatch[j].ReportApprove != null && SysBatch[j].ReportApprove != "")
                    {
                        num++;
                        V_ReportApprove model = new V_ReportApprove();
                        model.SystemName = "项目系统合并后流程";
                        model.seq = 121 + j;
                        model.IfComplate = false;
                        if (SysBatch[j].WFBatchStatus == "Approved")
                        {
                            model.IfComplate = true;
                        }
                        model.ReportApprove = SysBatch[j].ReportApprove;
                        model.WFStatus = SysBatch[j].WFBatchStatus;
                        if (num == 1)
                        {
                            model.Group = index;
                        }
                        else
                        {
                            model.Group = 0;
                        }
                        List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(SysBatch[j].ReportApprove);
                        model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                        model.list = new List<List<NavigatActivity>>(); ;
                        model.list.Add(list);
                        AllReportApprove.Add(model);
                    }
                }
            }
            AllReportApprove = AllReportApprove.OrderByDescending(a => a.CreatTime).OrderBy(a => a.seq).ToList(); ;
            return AllReportApprove;

        }

        #endregion

        /// <summary>
        /// 指标流程查询
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        [LibAction]
        public List<V_ReportApprove> GetAllTargetApprove(string SysID, int FinYear)
        {
            List<V_ReportApprove> AllReportApprove = new List<V_ReportApprove>();
            if (SysID == "all")
            {
                List<B_TargetPlan> result = B_TargetplanOperator.Instance.GetTargetPlanByAllList(FinYear).ToList();
                DataTable result1 = B_TargetplanOperator.Instance.GetTargetPlanByGroupList(FinYear);
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                    {
                        List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                        V_ReportApprove model = new V_ReportApprove();

                        C_System sys = C_SystemOperator.Instance.GetSystem(result[i].SystemID);
                        model.SystemName = sys.SystemName;
                        model.seq = sys.Sequence;
                        model.Category = sys.Category;

                        if (result[i].WFStatus == "Approved")
                        {
                            model.IfComplate = true;
                        }
                        else
                        {
                            model.IfComplate = false;
                        }
                        if (result1 != null)
                        {
                            for (int j = 0; j < result1.Rows.Count; j++)
                            {
                                var sysID = result1.Rows[j]["SystemID"].ToString();
                                var datetime = DateTime.Parse(result1.Rows[j]["CreateTime"].ToString());
                                var sysCount = int.Parse(result1.Rows[j]["SysCount"].ToString());

                                if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") == datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = sysCount; break;
                                }
                                else if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") != datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = 0; break;
                                }
                            }
                        }
                        else
                        {
                            model.Group = -1;
                        }
                        model.BusinessID = result[i].ID;
                        model.SystemID = result[i].SystemID;
                        model.WFStatus = result[i].WFStatus;
                        model.ReportApprove = result[i].ReportApprove;
                        // model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                        model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                        model.list = new List<List<NavigatActivity>>();
                        model.list.Add(list);
                        AllReportApprove.Add(model);

                    }
                }
                AllReportApprove = AllReportApprove.OrderByDescending(a => a.CreatTime).OrderBy(a => a.seq).ToList(); ;
                return AllReportApprove;
            }
            else
            {
                List<B_TargetPlan> result = B_TargetplanOperator.Instance.GetTargetPlanByApprovedList(Guid.Parse(SysID), FinYear).ToList();
                DataTable result1 = B_TargetplanOperator.Instance.GetTargetPlanByGroupList(FinYear);

                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].ReportApprove != "" && result[i].ReportApprove != null)
                    {
                        List<NavigatActivity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(result[i].ReportApprove);
                        V_ReportApprove model = new V_ReportApprove();

                        C_System sys = C_SystemOperator.Instance.GetSystem(result[i].SystemID);
                        model.SystemName = sys.SystemName;
                        model.seq = sys.Sequence;
                        model.Category = sys.Category;

                        if (result[i].WFStatus == "Approved")
                        {
                            model.IfComplate = true;
                        }
                        else
                        {
                            model.IfComplate = false;
                        }
                        if (result1 != null)
                        {
                            for (int j = 0; j < result1.Rows.Count; j++)
                            {
                                var sysID = result1.Rows[j]["SystemID"].ToString();
                                var datetime = DateTime.Parse(result1.Rows[j]["CreateTime"].ToString());
                                var sysCount = int.Parse(result1.Rows[j]["SysCount"].ToString());

                                if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") == datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = sysCount; break;
                                }
                                else if (result[i].SystemID == Guid.Parse(sysID) && result[i].CreateTime.ToString("yyyy-MM-dd hh-mm-ss") != datetime.ToString("yyyy-MM-dd hh-mm-ss"))
                                {
                                    model.Group = 0; break;
                                }
                            }
                        }
                        else
                        {
                            model.Group = -1;
                        }
                        model.BusinessID = result[i].ID;
                        model.SystemID = result[i].SystemID;
                        model.WFStatus = result[i].WFStatus;
                        model.ReportApprove = result[i].ReportApprove;
                        // model.CreatTime = result[i].CreateTime.ToString("MM-dd HH:mm");
                        model.CreatTime = list[0].Opinions[0].CreateDate.ToString("MM-dd HH:mm");
                        model.list = new List<List<NavigatActivity>>();
                        model.list.Add(list);
                        AllReportApprove.Add(model);

                    }
                }
                AllReportApprove = AllReportApprove.OrderByDescending(a => a.CreatTime).OrderBy(a => a.seq).ToList(); ;
                return AllReportApprove;
            }

        }

        [LibAction]
        public List<C_Target> GetExctargetListByComList(string CompanyID, string SystemID)
        {
            List<C_Target> List = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SystemID), DateTime.Now).ToList();
            if (CompanyID != null)
            {
                List<ExceptionTargetVModel> result = C_ExceptiontargetOperator.Instance.GetExctargetListByComList(Guid.Parse(CompanyID)).ToList();
                for (int i = 0; i < List.Count; i++)
                {
                    List[i].IsCheck = true;
                    for (int j = 0; j < result.Count; j++)
                    {
                        if (List[i].ID == result[j].TargetID)
                        {
                            List[i].IsCheck = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < List.Count; i++)
                {
                    List[i].IsCheck = true;
                }
            }
            return List;
        }
        /// <summary>
        /// 根据指标ID获取补回明细
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        [LibAction]
        public List<MonthlyReportDetail> GetVMissDetailByTargetID(string SystemID, string TargetID, int Year, int Month)
        {
            List<MonthlyReportDetail> result = A_MonthlyreportdetailOperator.Instance.GetVMissDetailByTargetID(Guid.Parse(SystemID), Guid.Parse(TargetID), Year, Month);
            //.OrderBy(p => p.Sequence).ToList()

            return result;
        }
        /// <summary>
        /// 获取各个公司全年的总指标数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        [LibAction]
        public List<V_PlanTargetModel> GetTargetPlan(string TargetPlanID, int FinYear)
        {
            // 获取各个公司全年的总指标数据
            List<V_PlanTargetModel> VPlanTargeList = A_TargetplandetailOperator.Instance.GetAnnualPlanTarget(Guid.Parse(TargetPlanID), FinYear);
            return VPlanTargeList;
        }

       
        [LibAction]
        public List<B_TargetPlan> GetTargetHistory(string SystemID, string Year)
        {
            List<B_TargetPlan> result = new List<B_TargetPlan>();
            result = B_TargetplanOperator.Instance.GetTargetPlanByApprovedAndApproved(Guid.Parse(SystemID), int.Parse(Year)).ToList();
            List<A_TargetPlanDetail> targetList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID.ToGuid(), int.Parse(Year)).ToList();
            //Guid id = Guid.Empty;
            //if (targetList.Count > 0)
            //{
            //    id = targetList[0].TargetPlanID;

            //}
            for (int i = 0; i < result.Count; i++)
            {

                var a = BPF.Workflow.Client.WFClientSDK.GetProcess(null, result[i].ID.ToString());
                Process Pro = WFClientSDK.GetProcess(null, result[i].ID.ToString()).ProcessInstance;//从工作流获取上报时间
                if (Pro != null)
                {
                    result[i].CreateTime = Pro.CreateDateTime;
                }

                result[i].IfCurrentTarget = false;
                if (targetList.Where(x=>x.ID==result[i].ID).Count()>0)
                {
                    result[i].IfCurrentTarget = true;
                }
            }
            return result;
        }
        [LibAction]
        public List<ContrastDetailList> GetContrastDetailList(string FinYear, string FinMonth)
        {
            List<ContrastDetailList> ContrastDetailList = C_ContrastDetailOperator.Instance.GetDetailList(FinYear, FinMonth);


            return ContrastDetailList;
        }
        [LibAction]
        public List<ContarstTargetDetailList> GetContrastList(string MonthlyReportID, int FinYear, int FinMonth, string SystemID, bool IfPro)
        {
            Guid MID;
            if (MonthlyReportID == null)
            {
                MID = Guid.Empty;
            }
            else
            {
                MID = Guid.Parse(MonthlyReportID);
            }

            C_System sys = StaticResource.Instance[Guid.Parse(SystemID), DateTime.Now];
            List<ContarstTargetDetailList> result = new List<ContarstTargetDetailList>();
            if (sys.GroupType != "ProSystem")
            {
                result = A_MonthlyreportdetailOperator.Instance.GetContarstTargetDetailList(MID, FinYear, FinMonth, Guid.Parse(SystemID), IfPro);

            }
            else
            {
                result = A_MonthlyreportdetailOperator.Instance.GetProContarstTargetDetailList(MID, FinYear, FinMonth, Guid.Parse(SystemID), IfPro);

            }

            return result;
        }


        [LibAction]
        public Guid ChangeTargetStatus(string TargetPlanID, string Description)
        {
            Guid result = Guid.Empty;
            B_TargetPlan rpt = B_TargetplanOperator.Instance.GetTargetPlanByID(TargetPlanID.ToGuid());
            List<B_TargetPlanDetail> rptDetailList = B_TargetplandetailOperator.Instance.GetTargetplandetailList(TargetPlanID.ToGuid()).ToList();

            #region 先将B表数据覆盖添加到A表(不改变该数据在B表的状态)


            A_TargetPlan rptA = null;

            List<A_TargetPlanDetail> rptTempDetailList = new List<A_TargetPlanDetail>();

            List<A_TargetPlanDetail> rptADetailList = null;

            //A主表的的数据
            rptA = A_TargetplanOperator.Instance.GetTargetplanList(rpt.SystemID, rpt.FinYear).FirstOrDefault();

            //A表明细数据
            rptADetailList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(rpt.SystemID, rpt.FinYear).ToList();


            //判断当月主表是否是null
            if (rptA == null)
            {
                A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //判断A 明细
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }
                else
                {

                    //删除A表明细的所有数据
                    A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }

                //添加明细数据
                A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            }
            else
            {
                //上来删除主表的ID
                A_TargetplanOperator.Instance.DeleteModel(rptA);

                //新增B表的主表数据
                A_TargetplanOperator.Instance.AddTargetplan(new A_TargetPlan() { ID = rpt.ID, FinYear = rpt.FinYear, Description = rpt.Description, SystemID = rpt.SystemID, Status = 5, CreateTime = DateTime.Now });

                //B表转换到A表
                if (rptADetailList.Count == 0)
                {
                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }
                else
                {
                    //删除A表明细的所有数据
                    A_TargetplandetailOperator.Instance.DeleteTargetPlanDetailList(rptADetailList);


                    #region 明细数据

                    //将B 表数据添加到 A表中
                    rptDetailList.ForEach(p => rptTempDetailList.Add(p.ToAModel()));

                    #endregion
                }

                //添加明细数据
                A_TargetplandetailOperator.Instance.AddTargetPlanDetailList(rptTempDetailList);
            }
            #endregion


            #region 在日志表中添加该次操作的日志

            if (rptDetailList.Count > 0)
            {
                B_TargetPlanAction TargetAction = new B_TargetPlanAction()
                {
                    TargetPlanID = TargetPlanID.ToGuid(),
                    SystemID = rpt.SystemID,
                    FinYear = rpt.FinYear,
                    Action = "激活",
                    Operator = HttpContext.Current.GetUserName(),
                    OperatorTime = DateTime.Now,
                    Description = Description
                };
                result = B_TargetPlanActionOperator.Instance.AddTargetPlanAction(TargetAction);
            }

            #endregion

            return result;
        }
        [LibAction]
        public List<B_TargetPlanAction> GetActionByTargetplanID(string TargetplanID)
        {
            List<B_TargetPlanAction> result = B_TargetPlanActionOperator.Instance.GetActionByTargetplanID(TargetplanID.ToGuid()).ToList();
            return result;
        }

        [LibAction]
        public bool GetShowPrcessNodeName(string strSystemID, string strProcessCode)
        {
            bool ShowProecessNodeName = false;
            if (!string.IsNullOrEmpty(strSystemID))
            {
                C_System cs = StaticResource.Instance.SystemList.Where(p => p.ID == Guid.Parse(strSystemID)).FirstOrDefault();
                if (cs != null)
                {
                    XElement xt = cs.Configuration.Elements("ProcessCode").ToList().Where(p => p.Value == strProcessCode).FirstOrDefault();
                    if (xt.GetAttributeValue("ShowProcessNodeName", "") == "true")
                    {
                        ShowProecessNodeName = true;
                    }
                }
            }

            return ShowProecessNodeName;
        }



        /// <summary>
        /// 获取月报的审批状况
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<SystemTreeModel> GetTreeDataJosn(string SysID, int FinYear, int FinMonth)
        {

            SystemTreeModel model = new SystemTreeModel();
            List<C_SystemTree> T_lsit = new List<C_SystemTree>();

            if (!string.IsNullOrEmpty(SysID))
            {
                var SysModel = C_SystemTreeOperator.Instance.GetSystemTree(SysID.ToGuid());

                if (SysModel != null && SysModel.Category == 0) // 表示全是结构层级数据
                {
                    //表示是顶级节点
                    List<C_SystemTree> P_List = C_SystemTreeOperator.Instance.GetSystemTreeData("'" + SysModel.ID.ToString() + "'");
                    List<C_SystemTree> C_List = C_SystemTreeOperator.Instance.GetSystemTreeDataByParent("'" + SysModel.ID.ToString() + "'");
                    P_List.AddRange(C_List);
                    T_lsit = P_List.Distinct().ToList();
                }
                else
                {
                    //表示是子节点
                    T_lsit = C_SystemTreeOperator.Instance.GetSystemTreeData("'" + SysModel.ID.ToString() + "'");
                }
            }
            else
            {
                //获取所有的数据
                T_lsit = C_SystemTreeOperator.Instance.GetSystemTreeList().ToList();
            }

            // 获取根节点
            var Root_Node = T_lsit.Where(p => p.ParentID == Guid.Empty).FirstOrDefault();

            if (Root_Node != null)
            {
                model.SysID = Root_Node.ID;
                model.Name = Root_Node.TreeNodeName;
                model.Category = Root_Node.Category;
                //获取子级的集合
                model.children = GetSubList(Root_Node.ID, FinYear, FinMonth, T_lsit);
            }

            return model.children;
        }

        /// <summary>
        /// 月报审批的递归
        /// </summary>
        /// <param name="Pid">父级ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <returns></returns>
        private List<SystemTreeModel> GetSubList(Guid Pid, int FinYear, int FinMonth, List<C_SystemTree> List)
        {
            List<SystemTreeModel> list = new List<SystemTreeModel>();
            List<B_SystemBatch> SysBatch = B_SystemBatchOperator.Instance.GetSystemBatchList("ProSystem", FinYear, FinMonth);
            int num = 0;
            int index = 0;
            //获取子List
            var Sub_list = List.Where(P => P.ParentID == Pid).ToList();  //C_SystemTreeOperator.Instance.GetSystemTreeListByID(Pid);

            if (Sub_list != null && Sub_list.Count > 0)
            {
                Sub_list.ForEach(F =>
                {

                    if (F.Category == 0) // 标识不是子集合
                    {
                        SystemTreeModel model = new SystemTreeModel();
                        model.SysID = F.ID;
                        model.Name = F.TreeNodeName;
                        model.Category = F.Category;
                        model.RptTime = string.Empty;
                        model.children = GetSubList(F.ID, FinYear, FinMonth, List);
                        list.Add(model);
                    }
                    else
                    {

                        //月报
                        var MList = B_MonthlyreportOperator.Instance.GetMonthlyReportBySysIDList(F.ID, FinYear, FinMonth).Where(p => !string.IsNullOrEmpty(p.ReportApprove)).ToList();
                        if (MList != null && MList.Count > 0)
                        {
                            MList.ForEach(M =>
                            {
                                SystemTreeModel model = new SystemTreeModel();
                                model.SysID = M.SystemID;
                                model.Name = F.TreeNodeName;
                                model.Category = F.Category;

                                //获取提交时间
                                List<NavigatActivity> NVlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(M.ReportApprove);

                                if (NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00")
                                    model.RptTime = M.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                else
                                    model.RptTime = NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                              
                                model.WFStause = M.WFStatus;
                                model.ReportApprove = M.ReportApprove;
                                list.Add(model);
                            });
                        }
                        else
                        {
                            SystemTreeModel model = new SystemTreeModel();
                            model.SysID = F.ID;
                            model.Name = F.TreeNodeName;
                            model.Category = F.Category;
                            list.Add(model);
                        }
                        if (Pid == WebConfigurationManager.AppSettings["ProJN"].ToGuid())
                        {
                            index++;
                            if (num == 0 && (index == 3 || index == 4))
                            {
                                if (SysBatch.Count() > 0)
                                {
                                    SysBatch = SysBatch.OrderByDescending(o => o.CreateTime).ToList();
                                    num++;
                                    for (int i = 0; i < SysBatch.Count(); i++)
                                    {
                                        if (!string.IsNullOrEmpty(SysBatch[i].ReportApprove))
                                        {
                                            SystemTreeModel model = new SystemTreeModel();
                                            model.SysID = Guid.NewGuid();
                                            model.Name = "合并后项目";
                                            model.Category = 2;
                                            //为了获取提交时间
                                            List<NavigatActivity> NVlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(SysBatch[i].ReportApprove);

                                            if (NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00")
                                                model.RptTime = SysBatch[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                            else
                                                model.RptTime = NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");

                                            model.WFStause = SysBatch[i].WFBatchStatus;
                                            model.ReportApprove = SysBatch[i].ReportApprove;
                                            list.Add(model);
                                        }
                                        else
                                        {
                                            SystemTreeModel model = new SystemTreeModel();
                                            model.SysID = Guid.NewGuid();
                                            model.Name = "合并后项目";
                                            model.Category = 2;
                                            model.RptTime = "";
                                            model.WFStause = "";
                                            model.ReportApprove = "";
                                            list.Add(model);
                                        }
                                    }
                                }
                                else
                                {
                                    SystemTreeModel model = new SystemTreeModel();
                                    model.SysID = Guid.NewGuid();
                                    model.Name = "合并后项目";
                                    model.Category = 2;
                                    model.RptTime = "";
                                    model.WFStause = "";
                                    model.ReportApprove = "";
                                    list.Add(model);
                                }
                            }

                        }
                    }
                });
            }

            return list;
        }


        /// <summary>
        /// 获取年度分解指标的审批状况
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<SystemTreeModel> GetTargetPlanTreeDataJosn(string SysID, int FinYear)
        {
            SystemTreeModel model = new SystemTreeModel();
            List<C_SystemTree> T_lsit = new List<C_SystemTree>();

            if (!string.IsNullOrEmpty(SysID))
            {
                var SysModel = C_SystemTreeOperator.Instance.GetSystemTree(SysID.ToGuid());

                if (SysModel != null && SysModel.Category == 0) // 表示全是结构层级数据
                {
                    //表示是顶级节点
                    List<C_SystemTree> P_List = C_SystemTreeOperator.Instance.GetSystemTreeData("'" + SysModel.ID.ToString() + "'");
                    List<C_SystemTree> C_List = C_SystemTreeOperator.Instance.GetSystemTreeDataByParent("'" + SysModel.ID.ToString() + "'");
                    P_List.AddRange(C_List);
                    T_lsit = P_List.Distinct().ToList();
                }
                else
                {
                    //表示是子节点
                    T_lsit = C_SystemTreeOperator.Instance.GetSystemTreeData("'" + SysModel.ID.ToString() + "'");
                }
            }
            else
            {
                //获取所有的数据
                T_lsit = C_SystemTreeOperator.Instance.GetSystemTreeList().ToList();
            }

            // 获取根节点
            var Root_Node = T_lsit.Where(p => p.ParentID == Guid.Empty).FirstOrDefault();

            if (Root_Node != null)
            {
                model.SysID = Root_Node.ID;
                model.Name = Root_Node.TreeNodeName;
                model.Category = Root_Node.Category;
                //获取子级的集合
                model.children = GetSubList(Root_Node.ID, FinYear, T_lsit);
            }

            return model.children;
        }

        /// <summary>
        /// 年度分解指标的递归
        /// </summary>
        /// <param name="Pid">父级ID</param>
        /// <param name="FinYear">年</param>
        /// <returns></returns>
        private List<SystemTreeModel> GetSubList(Guid Pid, int FinYear, List<C_SystemTree> List)
        {
            List<SystemTreeModel> list = new List<SystemTreeModel>();

            //获取子List
            var Sub_list = List.Where(P => P.ParentID == Pid).ToList();  //C_SystemTreeOperator.Instance.GetSystemTreeListByID(Pid);

            //var Sub_list = C_SystemTreeOperator.Instance.GetSystemTreeListByID(Pid);
            if (Sub_list != null && Sub_list.Count > 0)
            {
                Sub_list.ForEach(F =>
                {

                    if (F.Category == 0) // 标识不是子集合
                    {
                        SystemTreeModel model = new SystemTreeModel();
                        model.SysID = F.ID;
                        model.Name = F.TreeNodeName;
                        model.Category = F.Category;
                        model.RptTime = string.Empty;
                        model.children = GetSubList(F.ID, FinYear, List);
                        list.Add(model);
                    }
                    else
                    {
                        //月报

                        List<B_TargetPlan> MList = B_TargetplanOperator.Instance.GetTargetPlanByApprovedList(F.ID, FinYear).Where(P => !string.IsNullOrEmpty(P.ReportApprove)).ToList();

                        if (MList != null && MList.Count > 0)
                        {
                            MList.ForEach(M =>
                            {
                                SystemTreeModel model = new SystemTreeModel();
                                model.SysID = M.SystemID;
                                model.Name = F.TreeNodeName;
                                model.Category = F.Category;

                                //获取提交时间
                                List<NavigatActivity> NVlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NavigatActivity>>(M.ReportApprove);

                                if (NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00")
                                    model.RptTime = M.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                else
                                    model.RptTime = NVlist[0].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");

                                //指标结束时间赋值。
                                for (int i = 0; i < NVlist.Count(); i++)
                                {
                                    if (NVlist[NVlist.Count() - 1].ActivityName == "通知" && NVlist[NVlist.Count() - 2].ActivityName == "审批")
                                    {
                                        if (NVlist[NVlist.Count() - 2].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss") != "0001-01-01 00:00:00")
                                        {
                                            model.EndTime = NVlist[NVlist.Count() - 2].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                    }
                                    else if (NVlist[i].ActivityName == "审批" && i+1 == NVlist.Count())
                                    {
                                        if (NVlist[i].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss") != "0001-01-01 00:00:00")
                                        {
                                            model.EndTime = NVlist[i].Opinions[0].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                    }
                                }

                                //model.RptTime = M.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                model.WFStause = M.WFStatus;
                                model.ReportApprove = M.ReportApprove;
                                list.Add(model);
                            });
                        }
                        else
                        {
                            SystemTreeModel model = new SystemTreeModel();
                            model.SysID = F.ID;
                            model.Name = F.TreeNodeName;
                            model.Category = F.Category;
                            list.Add(model);
                        }
                    }
                });
            }

            return list;
        }

    }


    public class SystemTreeModel
    {
        public string Name { get; set; }

        public Guid SysID { get; set; }

        public string RptTime { get; set; }

        public string EndTime { get; set; }

        public string ReportApprove { get; set; }

        public string WFStause { get; set; }

        public int Lev { get; set; }

        public int Category { get; set; }


        public List<SystemTreeModel> children { get; set; }

    }
}
