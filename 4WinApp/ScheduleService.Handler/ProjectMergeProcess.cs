using System;
using System.Collections.Generic;
using System.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Core;
using LJTH.BusinessIndicators.Common;
using BPF.Workflow.Client;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using System.Xml.Linq;
using Lib.Xml;
using System.Collections;
using LJTH.BusinessIndicators.Engine;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class ProjectMergeProcess : Quartz.IJob
    {
        int finMonth = 0;
        int finYear = 0;

        List<V_SubReport> BatchRptList = null;

        public void Execute(Quartz.IJobExecutionContext context)
        {
            DateTime datetime = StaticResource.Instance.GetReportDateTime();

            finMonth = datetime.Month;
            finYear = datetime.Year;
            //string[] array = System.Configuration.ConfigurationManager.AppSettings["GroupTypes"].Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries);
            var array= StaticResource.Instance.SystemList.Where(x=>!string.IsNullOrEmpty(x.GroupType));
            foreach (var c_System in array)
            {
                bool _WFStarts = true;

                Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程服务，读取批次表里是否有合适的数据Stare!");

                B_SystemBatch batchModel = B_SystemBatchOperator.Instance.GetSystemBatchByDraft(c_System.GroupType, finYear, finMonth);

                if (batchModel != null)
                {
                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"读取数据成功!" + batchModel.FinYear + "年" + batchModel.FinMonth + "月");

                    BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(batchModel.SubReport);
                    int n = BatchRptList.Count(x => !x.IsReady);
                    _WFStarts = n == 0;
                }

                Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程服务，读取批次表里是否有合适的数据End!");

                if (_WFStarts)
                {
                    string ProcessKey = c_System.Configuration.Element("ProcessCode").Value + "-HB";
                    //string ProcessKey = "YY_ZBGK-FDCHZ";
                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程服务，有符合条件的的数据!");

                    this.ProcessKey = ProcessKey;
                    this.BusinessID = batchModel.ID.ToString();

                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程服务，有符合条件的的数据BusinessID=" + this.BusinessID + "!");

                    //添加日志
                    B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
                    _bMonthlyReportAction.SystemID = Guid.Empty;
                    _bMonthlyReportAction.MonthlyReportID = batchModel.ID;
                    _bMonthlyReportAction.FinYear = batchModel.FinYear;
                    _bMonthlyReportAction.FinMonth = batchModel.FinMonth;
                    _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), (int)MonthlyReportLogActionType.Submit);
                    _bMonthlyReportAction.Operator = this.CurrentUser;
                    _bMonthlyReportAction.OperatorTime = DateTime.Now;
                    _bMonthlyReportAction.ModifierName = this.CurrentUser;
                    _bMonthlyReportAction.CreatorName = this.CurrentUser;
                    B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);


                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程服务，准备启动合并流程数据!");

                    try
                    {
                        if (!WFClientSDK.Exist(BusinessID)) //判断业务ID是否存在
                        {//开启流程
                            CallMethed("startprocess", c_System.GroupType);
                            Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程服务，合并流程数据启动完成!");
                        }
                        else
                        {
                            Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程服务，有错误！！！！！！");
                            //当汇总后流程退回，再次发起用该方法
                            if (batchModel.WFBatchStatus == "Draft")
                            {
                                #region 该方法只能在草稿状态的时候启动

                                try
                                {
                                    string FinYear = batchModel.FinYear.ToString() + "年";
                                    string FinMonth = batchModel.FinMonth.ToString("D2");
                                    //string _Title = "项目系统(南、中、北、大项目)" + FinYear + FinMonth + "月度报告";
                                    string _Title = c_System.SystemName + FinYear + FinMonth + "汇总月度报告";
                                    Dictionary<string, string> Dic = new Dictionary<string, string>();
                                    Dic["ReportName"] = _Title;
                                    Dic["ProcessKey"] = ProcessKey;

                                    #region 记录审批日志的Json

                                    Common.ScheduleService.Log.Instance.Info("合并流程再次启动开始！");


                                    string opiniontext = string.Empty;


                                    //重新在批次表中，获取数据
                                    BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(batchModel.SubReport);
                                    List<BPF.Workflow.Object.ProcessLog> _list = new List<ProcessLog>();
                                    if (BatchRptList.Count > 0)
                                    {
                                        BatchRptList.ForEach(BR =>
                                        {
                                        //B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(BR.ReportID);
                                        _list.AddRange(BPF.Workflow.Client.WFClientSDK.GetProcessLogList(BR.ReportID.ToString()));
                                        });
                                    }
                                    batchModel.Opinions = JsonConvert.SerializeObject(_list.OrderByDescending(p => p.FinishDateTime));

                                    #endregion

                                    WorkflowContext workflow = WFClientSDK.GetProcess(null, this.BusinessID, new UserInfo() { UserCode = "$VirtualUserCode$"+this.CurrentUser });
                                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程,获取流程成功！");
                                    //先启动流程，流程能启动了，在写入数据库流程审批日志
                                    Dictionary<string, object> formParams = new Dictionary<string, object>();
                                    formParams.Add("ReportName", _Title);
                                    formParams.Add("ProcessKey", ProcessKey);

                                    var DynamicRoleUserList = JsonUser.GetDynamicRoleUserList(ProcessKey);
                                    BizContext bizContext = new BizContext();
                                    bizContext.NodeInstanceList = workflow.NodeInstanceList;
                                    bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                                    bizContext.BusinessID = BusinessID;
                                    bizContext.FlowCode = ProcessKey;
                                    bizContext.ApprovalContent = "各区域数据已经汇总完成，请领导审批";
                                    bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$"+this.CurrentUser };
                                    bizContext.ProcessURL = "/BusinessReport/TargetApprove.aspx?ProType=Batch";
                                    bizContext.FormParams = formParams;
                                    bizContext.DynamicRoleUserList = DynamicRoleUserList;
                                    bizContext.ExtensionCommond = new Dictionary<string, string>();
                                    bizContext.ExtensionCommond.Add("RejectNode", Guid.Empty.ToString());
                                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程,流程参数配置成功！");
                                    WorkflowContext wfc = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);
                                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程,流程提交成功！");
                                    OperationType = 1;
                                    //ProcessExecute();


                                    //先启动流程，流程能启动了，在写入数据库流程审批日志
                                    //ProcessResponse r = ClientProcess.MoveTo(BusinessID, "南、北、中、文旅项目已经汇总完成，请领导审批", Dic, "", true);

                                    //将批次的审批状态改变
                                    ExceptionHelper.TrueThrow(batchModel == null, string.Format("cannot find the report data which id={0}", BusinessID));
                                    batchModel.WFBatchStatus = "Progress";
                                    batchModel.Description = GetDescription(c_System.ID, batchModel.FinYear, batchModel.FinMonth);
                                    //获取流程导航
                                    List<NavigatActivity1> listna = GetProcessIntance(wfc);

                                    if (listna.Count > 0)
                                    {
                                        batchModel.ReportApprove = JsonConvert.SerializeObject(listna);
                                    }

                                    B_SystemBatchOperator.Instance.UpdateSystemBatch(batchModel);



                                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程服务，再次合并流程数据启动完成!");
                                }
                                catch (Exception ex)
                                {
                                    Common.ScheduleService.Log.Instance.Error(c_System.SystemName + "合并流程再次启动失败！，错误信息：" + ex.ToString());
                                }

                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.ScheduleService.Log.Instance.Error( "合并流程启动失败！，错误信息：" + ex.ToString());
                    }


                }
                else
                {
                    Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程服务，没有查找到符合条件合并流程!");
                }
            }
        }

        protected void CallMethed(string methedName,string groupType)
        {
            if (methedName == null)
            {
                throw new NotImplementedException();
            }
            switch (methedName.ToLower())
            {
                case "startprocess":
                    StartProcess(groupType);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        protected string GetDescription(Guid systemtId, int year, int month)
        {
            string Description = string.Empty;
            C_System sysModel = StaticResource.Instance[systemtId, DateTime.Now];
            XElement element = sysModel.Configuration;
            var RptList = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailList_Approve(systemtId, year, month, Guid.Empty,false);

            if (element.Elements("Report").Elements("Rgroup") != null)
            {
                Description = element.Element("Report").GetElementValue("Rgroup", "");
                if (!string.IsNullOrEmpty(Description))
                {
                    Hashtable p = MonthDescriptionValueEngine.MonthDescriptionValueService.GetMonthDescriptionValue(RptList, systemtId, "");
                    foreach (string key in p.Keys)
                    {
                        Description = Description.Replace("【" + key + "】", p[key].ToString());
                    }
                }
            }
            return Description;
        }
        public void AddMonthlyReport(B_SystemBatch batch,C_System c_System,string description)
        {
            B_MonthlyReport bmr = new B_MonthlyReport();
            bmr.ID = batch.ID;
            bmr.SystemID = c_System.ID;
            bmr.AreaID = Guid.Empty;
            bmr.FinMonth = batch.FinMonth;
            bmr.FinYear = batch.FinYear;
            bmr.Status = 2;
            bmr.WFStatus = "Progress";
            bmr.CreateTime = DateTime.Now;
            bmr.Description = description;
            bmr.ID = B_MonthlyreportOperator.Instance.AddMonthlyreport(bmr);
           
        }
        protected void StartProcess(string groupType)
        {
            
            Dictionary<string, object> formParams = new Dictionary<string, object>();
            var c_System = StaticResource.Instance.SystemList.Where(x => x.GroupType == groupType).FirstOrDefault();
            string ProcessKey = c_System.Configuration.Element("ProcessCode").Value + "-HB";
            //string ProcessKey = "YY_ZBGK-FDCHZ";

            //系统2014年7月月报

            B_SystemBatch batch = B_SystemBatchOperator.Instance.GetSystemBatch(Guid.Parse(BusinessID));

            string FinYear = batch.FinYear.ToString() + "年";
            string FinMonth = batch.FinMonth.ToString("D2");
            string _Title = c_System.SystemName + FinYear + FinMonth + "汇总月度报告";

            formParams.Add("ReportName", _Title);
            formParams.Add("ProcessKey", ProcessKey);

            Common.ScheduleService.Log.Instance.Info(c_System.SystemName+"合并流程服务，合并流程启动, 上报年：" + FinYear + ",上报月:" + FinMonth);

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic["ReportName"] = _Title;
            Dic["ProcessKey"] = ProcessKey;
            WorkflowContext wfc = null;
            try
            {
                Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程启动开始！");
                var dynamicRoleUserList = JsonUser.GetDynamicRoleUserList(ProcessKey);
                var starup = new BPF.Workflow.Client.WFStartupParameter()
                {
                    FlowCode = ProcessKey,
                    BusinessID = this.BusinessID,
                    CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$"+this.CurrentUser },
                    FormParams = formParams,
                    DynamicRoleUserList = dynamicRoleUserList,
                };

                string opiniontext = string.Empty;

                //重新在批次表中，获取数据
                BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(batch.SubReport);
                List<BPF.Workflow.Object.ProcessLog> _list = new List<ProcessLog>();
                if (BatchRptList.Count > 0)
                {
                    BatchRptList.ForEach(BR =>
                    {
                        //B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(BR.ReportID);
                        _list.AddRange(BPF.Workflow.Client.WFClientSDK.GetProcessLogList(BR.ReportID.ToString()));
                    });
                }
                batch.Opinions = JsonConvert.SerializeObject(_list);


                WorkflowContext workflow = WFClientSDK.CreateProcess(null, starup);
                Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "项目系统合并流程启动中！");

                BizContext bizContext = new BizContext();
                bizContext.NodeInstanceList = workflow.NodeInstanceList;
                bizContext.ProcessRunningNodeID = workflow.ProcessInstance.RunningNodeID;
                bizContext.BusinessID = BusinessID;
                bizContext.FlowCode = ProcessKey;
                bizContext.ApprovalContent = c_System.SystemName + "各区域数据已经汇总完成，请领导审批";
                // bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$项目汇总服务" };
                bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$"+this.CurrentUser };
                bizContext.ProcessURL = "/BusinessReport/TargetApprove.aspx?ProType=Batch";
                bizContext.FormParams = formParams;
                bizContext.ExtensionCommond = new Dictionary<string, string>();
                bizContext.ExtensionCommond.Add("RejectNode", Guid.Empty.ToString());
                bizContext.DynamicRoleUserList = dynamicRoleUserList;

                wfc = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);

                if (wfc.StatusCode != 0)
                {
                    throw  new Exception(wfc.StatusMessage + "; BusinessID = " + BusinessID );   
                }
                
                //提交操作状态为1
                OperationType = 1;

                Common.ScheduleService.Log.Instance.Info(c_System.SystemName + "合并流程启动结束！");

            }
            catch (Exception ex)
            {
                Common.ScheduleService.Log.Instance.Error(c_System.SystemName + "合并流程启动失败！，错误信息：" + ex.ToString());
                batch = null;
            }


            //将批次的审批状态改变

            ExceptionHelper.TrueThrow(batch == null, string.Format("cannot find the report data which id={0}", BusinessID));
            batch.WFBatchStatus = "Progress";
            
            //跟新后面两个大老板的状态
            //var AP = ProcessResponse.GetProcess(batch.ID.ToString(), null);
            //List<NavigatActivity> APlist = AP.Navigat;
            //string Json = Newtonsoft.Json.JsonConvert.SerializeObject(APlist);
            //batch.ReportApprove = Json;
            //获取流程导航
            List<NavigatActivity1> listna = GetProcessIntance(wfc);
            if (listna.Count > 0)
            {
                batch.ReportApprove = JsonConvert.SerializeObject(listna);
            }
            batch.Description = GetDescription(c_System.ID, batch.FinYear, batch.FinMonth);
            //修改批次数据
            B_SystemBatchOperator.Instance.UpdateSystemBatch(batch);
        }
        /// <summary>
        /// 获取流程导航
        /// </summary>
        /// <param name="BusinessID"></param>
        /// <returns></returns>
        public List<NavigatActivity1> GetProcessIntance(WorkflowContext p)
        {

            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            if (BPF.Workflow.Client.WFClientSDK.Exist(BusinessID))
            {
                try
                {
                    NavigatActivity1 na1 = null;
                    Dictionary<string, BPF.Workflow.Object.Node> list = new Dictionary<string, Node>();
                    string strNextNodeID = p.ProcessInstance.StartNodeID;
                    foreach (var p1 in p.NodeInstanceList)
                    {
                        if (!string.IsNullOrEmpty(strNextNodeID))
                        {
                            var CurrentNode = p.NodeInstanceList[strNextNodeID];
                            list.Add(strNextNodeID, CurrentNode);
                            strNextNodeID = CurrentNode.NextNodeID;
                        }
                    }

                    foreach (var p1 in list)
                    {
                        if (string.IsNullOrEmpty(p1.Value.ParentNodeID))
                        {
                            na1 = new NavigatActivity1();
                            na1.ActivityID = p1.Value.NodeID;
                            na1.ActivityName = p1.Value.NodeName;
                            //na1.ActivityType = (ActivityType)p1.Value.NodeType;
                            //na1.RunningStatus = (WFRunningStatus)(p1.Value.Status > 1 ? 3 : p1.Value.Status);
                            List<ClientOpinion1> listclientOp = new List<ClientOpinion1>();
                            listclientOp.Add(new ClientOpinion1() { CreateDate = p1.Value.FinishDateTime });
                            na1.Opinions = listclientOp;
                            List<NavigatCandidate1> listnc = new List<NavigatCandidate1>();
                            var lstNode = p.NodeInstanceList.Where(s => s.Value.ParentNodeID == p1.Value.NodeID);
                            if (lstNode.Count() > 0)
                            {
                                foreach (var itme in lstNode)
                                {
                                    listnc.Add(new NavigatCandidate1()
                                    {
                                        Id = itme.Value.User.UserID,
                                        Name = itme.Value.User.UserName,
                                        Title = itme.Value.User.UserJobName,
                                        DeptName = itme.Value.User.UserOrgPathName,
                                        Completed = itme.Value.Status == 2 ? true : false
                                    });
                                }
                            }
                            else
                            {
                                listnc.Add(new NavigatCandidate1()
                                {
                                    Id = p1.Value.User.UserID,
                                    Name = p1.Value.User.UserName,
                                    Title = p1.Value.User.UserJobName,
                                    DeptName = p1.Value.User.UserOrgPathName,
                                    Completed = p1.Value.Status == 2 ? true : false
                                });
                            }
                            na1.Candidates = listnc;
                            listna.Add(na1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.ScheduleService.Log.Instance.Error("合并流程获取流程导航出错！，错误信息：" + ex.ToString());
                }
            }
            return listna;
        }


        protected void ProcessExecute()
        {
            B_MonthlyReport rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(Guid.Parse(BusinessID));
            ExceptionHelper.TrueThrow(rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));

            if (rpt.SystemBatchID != Guid.Empty) //如果含有批次
            {
                B_SystemBatch BacthModel = B_SystemBatchOperator.Instance.GetSystemBatch(rpt.SystemBatchID);
                ExceptionHelper.TrueThrow(BacthModel == null, string.Format("cannot find the report data which id={0}", BusinessID));
                // BacthModel.WFBatchStatus = "Progress";

                List<V_SubReport> monthRpt = JsonConvert.DeserializeObject<List<V_SubReport>>(BacthModel.SubReport);

                foreach (var item in monthRpt)
                {
                    B_MonthlyReport _rpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(item.ReportID);
                    ExceptionHelper.TrueThrow(_rpt == null, string.Format("cannot find the report data which id={0}", BusinessID));


                    //有效流程状态：Progress, Approved
                    //6-return, 7 -withdraw
                    if (OperationType == 6 || OperationType == 7)
                    {
                        _rpt.WFStatus = "Draft";
                        if (OperationType == 6)
                        {
                            AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, _rpt.SystemID, _rpt.FinYear, _rpt.FinMonth, Guid.Parse(BusinessID));
                        }
                        else if (OperationType == 7)
                        {
                            AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, _rpt.SystemID, _rpt.FinYear, _rpt.FinMonth, Guid.Parse(BusinessID));
                        }
                    }
                    else if (OperationType == 9)//9, cancel
                    {
                        _rpt.WFStatus = "Cancel";
                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, _rpt.SystemID, _rpt.FinYear, _rpt.FinMonth, Guid.Parse(BusinessID));
                    }
                    else
                    {
                        _rpt.WFStatus = "Progress";
                        _rpt.Status = 5;
                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, _rpt.SystemID, _rpt.FinYear, _rpt.FinMonth, Guid.Parse(BusinessID));
                    }

                    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(_rpt);
                }


                //有效流程状态：Progress, Approved
                //6-return, 7 -withdraw
                if (OperationType == 6 || OperationType == 7)
                {
                    BacthModel.WFBatchStatus = "Draft";
                    if (OperationType == 6)
                    {
                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Return, rpt.SystemID, rpt.FinYear, rpt.FinMonth, Guid.Parse(BusinessID));
                    }
                    else if (OperationType == 7)
                    {
                        AddMonthlyReportAction((int)MonthlyReportLogActionType.Withdraw, rpt.SystemID, rpt.FinYear, rpt.FinMonth, Guid.Parse(BusinessID));
                    }
                }
                else if (OperationType == 9)//9, cancel
                {
                    BacthModel.WFBatchStatus = "Cancel";
                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Cancel, rpt.SystemID, rpt.FinYear, rpt.FinMonth, Guid.Parse(BusinessID));
                }
                else
                {
                    BacthModel.WFBatchStatus = "Progress";
                    AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, Guid.Parse(BusinessID));
                }

                //修改批次数据
                B_SystemBatchOperator.Instance.UpdateSystemBatch(BacthModel);
            }
            else //不含批次
            {
                rpt.WFStatus = "Progress";
                B_MonthlyreportOperator.Instance.UpdateMonthlyreport(rpt);

                AddMonthlyReportAction((int)MonthlyReportLogActionType.Submit, rpt.SystemID, rpt.FinYear, rpt.FinMonth, Guid.Parse(BusinessID));
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActionType"></param>
        /// <param name="SysId"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="MonthReportID"></param>
        protected void AddMonthlyReportAction(int ActionType, Guid SysId, int FinYear, int FinMonth, Guid MonthReportID)
        {
            B_MonthlyReportAction _bMonthlyReportAction = new B_MonthlyReportAction();
            _bMonthlyReportAction.SystemID = SysId;
            _bMonthlyReportAction.MonthlyReportID = MonthReportID;
            _bMonthlyReportAction.FinYear = FinYear;
            _bMonthlyReportAction.FinMonth = FinMonth;
            _bMonthlyReportAction.Action = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), ActionType);
            _bMonthlyReportAction.Operator = CurrentUser;
            _bMonthlyReportAction.OperatorTime = DateTime.Now;
            _bMonthlyReportAction.ModifierName = CurrentUser;
            _bMonthlyReportAction.CreatorName = CurrentUser;

            B_MonthlyReportActionOperator.Instance.AddMonthlyReportAction(_bMonthlyReportAction);
        }



        protected string ProcessKey { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string BusinessID
        {
            get;
            set;
        }

        public string CurrentUser
        {
            //get { return "虚拟"; }
            get { return System.Configuration.ConfigurationManager.AppSettings["virtualUser"]; }
        }

        public int OperationType
        {
            get;
            protected set;
        }

        protected string ProcessJSON { get; set; }

        public virtual string Recovery()
        {
            return ProcessJSON;
        }

        public virtual void Resident(string processJSON)
        {
            ProcessJSON = processJSON;
        }

    }


    public class WfOperationInfo
    {
        public int OperationType { get; set; }
    }


    public class TempOpinion
    {
        public string a { get; set; }
        public string an { get; set; }
        public bool ce { get; set; }
        public DateTime d { get; set; }
        public string i { get; set; }
        public string r { get; set; }
        public string c { get; set; }
        public string t { get; set; }
        public string u { get; set; }
        public string ui { get; set; }
    }

    public class TempOpinionObj
    {
        public TempOpinion obj { get; set; }
        public TempDom dom { get; set; }
        public string actid { get; set; }
    }

    public class TempDom
    {
        public int length { get; set; }
    }
    [Serializable]
    public class NavigatActivity1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private int activityType;

        public int ActivityType
        {
            get { return activityType; }
            set { activityType = value; }
        }
        private bool canBeReturned;

        public bool CanBeReturned
        {
            get { return canBeReturned; }
            set { canBeReturned = value; }
        }
        private List<NavigatCandidate1> candidates;

        public List<NavigatCandidate1> Candidates
        {
            get { return candidates; }
            set { candidates = value; }
        }
        private bool compelPass;

        public bool CompelPass
        {
            get { return compelPass; }
            set { compelPass = value; }
        }
        private List<ClientOpinion1> opinions;

        public List<ClientOpinion1> Opinions
        {
            get { return opinions; }
            set { opinions = value; }
        }
        private int runningStatus;

        public int RunningStatus
        {
            get { return runningStatus; }
            set { runningStatus = value; }
        }
    }

    [Serializable]
    public class ClientOpinion1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private bool canEdit;

        public bool CanEdit
        {
            get { return canEdit; }
            set { canEdit = value; }
        }
        private DateTime createDate;

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string operationType;

        public string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }
        private string opinContent;

        public string OpinContent
        {
            get { return opinContent; }
            set { opinContent = value; }
        }
        private string taskID;

        public string TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string userID;

        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }

    [Serializable]
    public class NavigatCandidate1
    {
        private bool completed;

        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }
        private string deptName;

        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; }
        }
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
    }
}
