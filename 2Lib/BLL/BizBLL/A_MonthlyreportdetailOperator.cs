using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.DAL;
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
using Lib.Data.AppBase;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators;
using System.Data;
using System.Xml.Linq;
using LJTH.BusinessIndicators.Common;

namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Monthlyreportdetail对象的业务逻辑操作
    /// </summary>
    public class A_MonthlyreportdetailOperator : BizOperatorBase<A_MonthlyReportDetail>
    {

        #region Generate Code

        public static readonly A_MonthlyreportdetailOperator Instance = PolicyInjection.Create<A_MonthlyreportdetailOperator>();

        private static A_MonthlyreportdetailAdapter _aMonthlyreportdetailAdapter = AdapterFactory.GetAdapter<A_MonthlyreportdetailAdapter>();

        private static V_MonthlyreportdetailAdapter _vMonthlyreportdetailAdapter = AdapterFactory.GetAdapter<V_MonthlyreportdetailAdapter>();

        private static C_TargetAdapter _cTargetAdapter = AdapterFactory.GetAdapter<C_TargetAdapter>();
        /// <summary>
        /// 获取ViewModel的list
        /// </summary>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetVModelList(int FinYear, int FinMonth, Guid SystemID)
        {
            return _vMonthlyreportdetailAdapter.GetList(FinYear, FinMonth, SystemID);
        }


        /// <summary>
        /// 获取ViewModel的list
        /// </summary>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetVModelList(int FinYear, int FinMonth, Guid SystemID, Guid TargetID)
        {
            return _vMonthlyreportdetailAdapter.GetList(FinYear, FinMonth, SystemID, TargetID);
        }



        protected override BaseAdapterT<A_MonthlyReportDetail> GetAdapter()
        {
            return _aMonthlyreportdetailAdapter;
        }

        public IList<A_MonthlyReportDetail> GetMonthlyreportdetailList()
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetMonthlyreportdetailList();
            return result;
        }

        public Guid AddMonthlyreportdetail(A_MonthlyReportDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_MonthlyReportDetail GetMonthlyreportdetail(Guid aMonthlyreportdetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMonthlyreportdetailID == null, "Argument aMonthlyreportdetailID is Empty");
            return base.GetModelObject(aMonthlyreportdetailID);
        }

        //商管系统专用！！
        public Guid UpdateMonthlyreportdetail(A_MonthlyReportDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }
        //其他项目年内补回功能用
        public Guid UpdateReportdetail(A_MonthlyReportDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlyreportdetail(Guid aMonthlyreportdetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMonthlyreportdetailID == null, "Argument aMonthlyreportdetailID is Empty");
            Guid result = base.RemoveObject(aMonthlyreportdetailID);
            return result;
        }
        public IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year, int Month)
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailList(SystemID, Year, Month);

            return result;
        }

        public List<MonthlyReportDetail> GetMonthlyReportDetailList_Result(Guid SystemID, int Year, int Month,Guid TargetPlanID)
        {
            List<MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetMonthlyReportDetailList_Result(SystemID, Year, Month, TargetPlanID);
            return result;
        }

        





        public IList<MonthlyReportVM> GetAVMonthlyReport(Guid SystemID, int Year, int Month)
        {
            IList<MonthlyReportVM> result = _aMonthlyreportdetailAdapter.GetAVMonthlyReport(SystemID, Year, Month);

            return result;
        }
        /// <summary>
        /// 获取未完成数据
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="TargetID">指标</param>
        /// <param name="IsSpecial">是否特殊处理  如果为true  则为特殊处理</param>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetVMissDetail(Guid SystemID, int Year, int Month)
        {
            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();
            B_MonthlyReport BM = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(SystemID, Year, Month);
            DateTime T = DateTime.Now;
            if (BM != null)
            {
                T = BM.CreateTime;
            }
            List<C_Target> TargetList = StaticResource.Instance.GetTargetList(SystemID, T).ToList();

            foreach (C_Target item in TargetList)
            {
                if (item.Configuration.Elements("IsMissTargetConfigration").ToList().Count > 0)
                {
                    List<MonthlyReportDetail> DiffResult = _aMonthlyreportdetailAdapter.GetVMissDetail(SystemID, Year, Month, item.ID, true).ToList();
                    for (int i = 0; i < DiffResult.Count; i++)
                    {
                        if (DiffResult[i].PromissDate==null&&DiffResult[i].ReturnType==(int)EnumReturnType.Returning)
                        {
                            DiffResult[i].PromissDate = DiffResult[i].CurrentMonthCommitDate;
                        }
                        result.Add(DiffResult[i]);
                    }
                }
                else
                {
                    List<MonthlyReportDetail> DiffResult = _aMonthlyreportdetailAdapter.GetVMissDetail(SystemID, Year, Month, item.ID, false).ToList();
                    for (int i = 0; i < DiffResult.Count; i++)
                    {
                        if (DiffResult[i].PromissDate == null && DiffResult[i].ReturnType == (int)EnumReturnType.Returning)
                        {
                            DiffResult[i].PromissDate=DiffResult[i].CurrentMonthCommitDate  ;
                        }
                        result.Add(DiffResult[i]);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 通过指标ID获取该指标下当年当月的未完成公司
        /// </summary>
        /// <param name="TargetID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<MonthlyReportDetail> GetVMissDetailByTargetID(Guid SystemID,Guid TargetID, int Year, int Month)
        {
            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();
            DateTime t = DateTime.Now;
            C_Target Model = new C_Target();
            List<A_MonthlyReportDetail> AL = GetAMonthlyreportdetailList(SystemID, Year, Month, TargetID).ToList();
            if (AL.Count>0)
            {
                B_MonthlyReport bm = B_MonthlyreportOperator.Instance.GetMonthlyreport(AL[0].MonthlyReportID);
                t = bm.CreateTime;
            }
            Model = StaticResource.Instance.GetTargetList(SystemID, t).Where(s => s.ID == TargetID).FirstOrDefault();
            if (Model.Configuration.Elements("IsMissTargetConfigration").ToList().Count > 0)
            {
                result = _aMonthlyreportdetailAdapter.GetVMissDetail(SystemID, Year, Month, Model.ID, true).ToList();

            }
            else
            {
                result = _aMonthlyreportdetailAdapter.GetVMissDetail(SystemID, Year, Month, Model.ID, false).ToList();

            }
            if (result.Count>0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].TargetType = i + 1;
                    if (result[i].PromissDate==null&&result[i].ReturnType==(int)EnumReturnType.Returning)
                    {
                        result[i].PromissDate = result[i].CurrentMonthCommitDate;
                    }
                }
            }
            return result;

        }
        public List<A_MonthlyReportDetail> GetAMReportDetailIsMissTargetList(Guid SystemID, int Year, int Month)
        {
            List<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMReportDetailIsMissTargetList(SystemID, Year, Month).ToList();
            B_MonthlyReport BM = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(SystemID, Year, Month);
            DateTime T = DateTime.Now;
            if (BM != null)
            {
                T = BM.CreateTime;
            }
            List<C_Target> TargetList = StaticResource.Instance.GetTargetList(SystemID, T).ToList();

            foreach (C_Target item in TargetList)
            {
                if (item.Configuration.Elements("IsMissTargetConfigration").ToList().Count > 0)//确定该指标是否是特定的指标,是否特殊处理
                {
                    result.RemoveAll(R => R.TargetID == item.ID);
                    List<A_MonthlyReportDetail> DiffResult = _aMonthlyreportdetailAdapter.GetAMReportDetailDifferenceList(SystemID, Year, Month, item.ID).ToList();
                    for (int a = 0; a < DiffResult.Count; a++)
                    {
                        result.Add(DiffResult[a]);
                    }

                }
            }
            return result;
        }

        public IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year, int Month, Guid TargetID)
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailList(SystemID, Year, Month, TargetID);
            return result;
        }

        public IList<A_MonthlyReportDetail> GetAMonthlyReportDetailListForTargetPlanID(Guid SystemID, int Year, int Month, Guid TargetPlanID)
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyReportDetailListForTargetPlanID(SystemID, Year, Month, TargetPlanID);
            return result;
        }
        public IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid MonthlyReportID)
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailList(MonthlyReportID);
            return result;
        }

        /// <summary>
        /// 通过系统ID获取全年的的数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int Year)
        {
            IList<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailList(SystemID, Year);
            return result;
        }

        public A_MonthlyReportDetail GetAMonthlyreportdetail(Guid SystemID, Guid CompanyID, Guid TargetID, int Year, int Month)
        {
            return _aMonthlyreportdetailAdapter.GetAMonthlyreportdetail(SystemID, CompanyID, TargetID, Year, Month);
        }

        public A_MonthlyReportDetail GetAMonthlyreportdetail(Guid SystemID, Guid CompanyID, Guid TargetID, int Year, int Month,Guid TargetPlanID)
        {
            return _aMonthlyreportdetailAdapter.GetAMonthlyreportdetail(SystemID, CompanyID, TargetID, Year, Month, TargetPlanID);
        }

        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int UpdateMonthlyreportDetailList(List<A_MonthlyReportDetail> List)
        {
            if (List.Count > 0)
            {
                return _aMonthlyreportdetailAdapter.UpdateMonthlyreportdetailLisr(List);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int AddMonthlyreportDetailList(List<A_MonthlyReportDetail> List)
        {
            if (List.Count > 0)
            {
                return _aMonthlyreportdetailAdapter.AddMonthlyreportdetailLisr(List);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int DeleteMonthlyreportdetailList(List<A_MonthlyReportDetail> List)
        {
            if (List.Count > 0)
            {
                return _aMonthlyreportdetailAdapter.DeleteMonthlyreportdetailLisr(List);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 万达电影固定报表，国内影城-票房收入指标查询
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> GetMonthTargetDetailCNPF(Guid SystemID, int FinYear, int FinMonth, Guid TargetID)
        {
            List<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetMonthTargetDetailCNPF(SystemID, FinYear, FinMonth, TargetID);
            return result;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="TargetPlanID"></param>
        /// <returns></returns>
        public List<A_MonthlyReportDetail> GetAMonthlyreportdetailList(Guid SystemID, int FinYear,  Guid TargetPlanID)
        {
            List<A_MonthlyReportDetail> result = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailList(SystemID, FinYear, TargetPlanID);
            return result;
        }


        /// <summary>
        /// 添加指标数据
        /// </summary>
        /// <param name="list">指标数据</param>
        /// <returns>执行状态</returns>
        public int AddOrUpdateTargetDetail(List<A_MonthlyReportDetail> list, string strOperateType)
        {
            if (strOperateType == "Update")
            {
                UpdateMonthlyreportDetailList(list);
                return list.Count;
            }
            else
            {
                foreach (A_MonthlyReportDetail brd in list)
                {
                    AddMonthlyreportdetail(brd);
                }
                return list.Count;
                //AddMonthlyreportDetailList(list);
            }
        }

        #endregion

        public SRptModel GetAMonthlyreportdetailSimpleRpt(SRptModel m)
        {
            SRptModel model = m;
            List<B_MonthlyReportDetail> t = new List<B_MonthlyReportDetail>();
            List<B_MonthlyReportDetail> c = new List<B_MonthlyReportDetail>();
            DataSet ds = _aMonthlyreportdetailAdapter.GetAMonthlyreportdetailSimpleRpt(Guid.Parse(m.ID), int.Parse(m.Year), int.Parse(m.Month));
            if (ds != null && ds.Tables != null && ds.Tables.Count >= 2)
            {
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    B_MonthlyReportDetail detail = new B_MonthlyReportDetail();
                    detail.ID = Guid.NewGuid();
                    detail.SystemID = Guid.Parse(m.ID);
                    detail.FinYear = int.Parse(m.Year);
                    detail.FinMonth = int.Parse(m.Month);
                    detail.TargetID = ds.Tables[0].Rows[i]["ID"].ToString().ToGuid();
                    detail.TargetName = ds.Tables[0].Rows[i]["TargetName"].ToString();
                    detail.NPlanAmmount = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[0].Rows[i]["NPlanAmmount"].ToString()) : Convert.ToDecimal(ds.Tables[0].Rows[i]["NPlanAmmount"].ToString()) / 10000;
                    detail.NActualAmmount = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[0].Rows[i]["NActualAmmount"].ToString()) : Convert.ToDecimal(ds.Tables[0].Rows[i]["NActualAmmount"].ToString()) / 10000;
                    detail.NDifference = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[0].Rows[i]["NDifference"].ToString()) : Convert.ToDecimal(ds.Tables[0].Rows[i]["NDifference"].ToString()) / 10000;
                    // detail =TargetEvaluationEngine.TargetEvaluationService.Calculation(detail);
                    t.Add(detail);
                }
                for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                {

                    B_MonthlyReportDetail cdetail = new B_MonthlyReportDetail();
                    cdetail.ID = Guid.NewGuid();
                    cdetail.SystemID = Guid.Parse(m.ID);
                    cdetail.FinYear = int.Parse(m.Year);
                    cdetail.FinMonth = int.Parse(m.Month);
                    cdetail.TargetID = ds.Tables[1].Rows[j]["ID"].ToString().ToGuid();
                    cdetail.TargetName = ds.Tables[1].Rows[j]["TargetName"].ToString();
                    cdetail.NPlanAmmount = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[1].Rows[j]["NPlanAmmount"].ToString()) : Convert.ToDecimal(ds.Tables[1].Rows[j]["NPlanAmmount"].ToString()) / 10000;
                    cdetail.NActualAmmount = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[1].Rows[j]["NActualAmmount"].ToString()) : Convert.ToDecimal(ds.Tables[1].Rows[j]["NActualAmmount"].ToString()) / 10000;
                    cdetail.NDifference = m.Unit == "万元" ? Convert.ToDecimal(ds.Tables[1].Rows[j]["NDifference"].ToString()) : Convert.ToDecimal(ds.Tables[1].Rows[j]["NDifference"].ToString()) / 10000;
                    //cdetail =TargetEvaluationEngine.TargetEvaluationService.Calculation(cdetail);
                    c.Add(cdetail);
                }
            }
            model.TotalList = t;
            model.CurrentList = c;
            return model;
        }

        /// <summary>
        /// 计算未完成家数对比的相应计算
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<ContrastMisstargetList> GetContrastMisstarget(int FinYear, int FinMonth, string SystemID, bool IsPro)
        {
            List<ContrastMisstargetList> returnresult = new List<ContrastMisstargetList>();

            List<Guid> IDList = new List<Guid>();
            if (SystemID == "0")
            {
                List<C_System> List = C_SystemOperator.Instance.GetSystemListBySeq().ToList();//先取出所有
                foreach (C_System item in List)//将他们的ID存入List
                {
                    IDList.Add(item.ID);
                }
                IDList = IDList.Distinct().ToList();//去重      
            }
            else
            {
                IDList.Add(Guid.Parse(SystemID));
            }
            for (int n = 0; n < IDList.Count; n++)
            {
                DateTime T = DateTime.Now;
                B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(IDList[n], FinYear, FinMonth);
                if (BMonthlyReport != null)
                {
                    T = BMonthlyReport.CreateTime;
                }
                C_System SystemModel = StaticResource.Instance[IDList[n], T];

                List<C_Target> TargetList = StaticResource.Instance.GetTargetList(IDList[n], T).ToList();
                TargetList = TargetList.Where(p => p.TargetType != 3 && p.NeedEvaluation == true).OrderBy(p => p.Sequence).ToList();
                ContrastMisstargetList ContrastMisstargetmodel = new ContrastMisstargetList();
                #region
                List<ContrastMisstarget> result = new List<ContrastMisstarget>();
                List<MonthlyReportVM> AllDetail = new List<MonthlyReportVM>();


                List<MonthlyReportVM> MonthReportB = new List<MonthlyReportVM>();
                List<MonthlyReportVM> MonthReportC = new List<MonthlyReportVM>();
                if (FinMonth > 1)
                {

                    //取出今年所有数据
                    if (IsPro == true && BMonthlyReport != null)
                    {
                        AllDetail = B_MonthlyreportdetailOperator.Instance.GetBVMonthlyReport(BMonthlyReport.ID).ToList();
                    }
                    else
                    {
                        AllDetail = A_MonthlyreportdetailOperator.Instance.GetAVMonthlyReport(IDList[n], FinYear, FinMonth).ToList();

                    }

                    MonthReportB = A_MonthlyreportdetailOperator.Instance.GetAVMonthlyReport(IDList[n], FinYear, FinMonth - 1).ToList();
                    MonthReportC = A_MonthlyreportdetailOperator.Instance.GetAVMonthlyReport(IDList[n], FinYear - 1, FinMonth).ToList();
                }
                else
                {
                    //取出今年所有数据
                    if (IsPro == true && BMonthlyReport != null)
                    {
                        AllDetail = B_MonthlyreportdetailOperator.Instance.GetBVMonthlyReport(BMonthlyReport.ID).ToList();
                    }
                    else
                    {
                        AllDetail = A_MonthlyreportdetailOperator.Instance.GetAVMonthlyReport(IDList[n], FinYear, FinMonth).ToList();

                    }

                    MonthReportB = new List<MonthlyReportVM>();
                    MonthReportC = A_MonthlyreportdetailOperator.Instance.GetAVMonthlyReport(IDList[n], FinYear - 1, FinMonth).ToList();
                }

                for (int i = 0; i < TargetList.Count; i++)
                {
                    string I = "NeedEvaluation";
                    if (IsPro == true)
                    {
                        I = "BNeedEvaluation";
                    }
                    ContrastMisstarget model = new ContrastMisstarget();
                    model.TargetID = TargetList[i].ID;
                    model.TargetName = TargetList[i].TargetName;
                    if (AllDetail.Count > 0)
                    {
                        #region 本月数据

                        R_MissTargetEvaluationScope TypeView = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(IDList[n], TargetList[i].ID, FinYear, FinMonth, I);
                        //取出本月考核范围内家数
                        if (TypeView != null)
                        {
                            model.ThisEvaluationCompany = TypeView.EvaluationNumber;
                        }
                        else
                        {
                            model.ThisEvaluationCompany = 0;
                        }

                        if (model.ThisEvaluationCompany > 0)
                        {

                            //取出本月累计指标未完成家数
                            model.ThisIsMissTarget = AllDetail.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //取出本月当月指标未完成家数
                            model.ThisIsMissCurrent = AllDetail.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();


                            //本月累计指标占比
                            model.ThisProportion = Division(model.ThisIsMissTarget, model.ThisEvaluationCompany);
                            //本月当月指标占比
                            model.ThisProportionCurrent = Division(model.ThisIsMissCurrent, model.ThisEvaluationCompany);
                        }
                        else
                        {
                            //取出本月累计指标未完成家数
                            model.ThisIsMissTarget = AllDetail.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //取出本月当月指标未完成家数
                            model.ThisIsMissCurrent = AllDetail.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();

                            //本月累计指标占比
                            model.ThisProportion = "--";
                            //本月当月指标占比
                            model.ThisProportionCurrent = "--";
                        }
                        #endregion
                    }

                    if (MonthReportB.Count > 0)
                    {


                        #region 上月数据


                        R_MissTargetEvaluationScope LastMonthTypeView = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(IDList[n], TargetList[i].ID, FinYear, FinMonth - 1, "NeedEvaluation");
                        //取出上个月考核范围内家数
                        if (LastMonthTypeView != null)
                        {
                            model.LastEvaluationCompany = LastMonthTypeView.EvaluationNumber;
                        }
                        else
                        {
                            model.LastEvaluationCompany = 0;
                        }

                        if (model.LastEvaluationCompany > 0)
                        {
                            //取出上月累计指标未完成家数
                            model.LastIsMissTarget = MonthReportB.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //取出上月当月指标未完成家数
                            model.LastIsMissCurrent = MonthReportB.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //上月累计指标占比
                            model.LastProportion = Division(model.LastIsMissTarget, model.LastEvaluationCompany);
                            //上月当月指标占比
                            model.LastProportionCurrent = Division(model.LastIsMissCurrent, model.LastEvaluationCompany);
                        }
                        else
                        {
                            //取出上月累计指标未完成家数
                            model.LastIsMissTarget = MonthReportB.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //取出上月当月指标未完成家数
                            model.LastIsMissCurrent = MonthReportB.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //上月累计指标占比
                            model.LastProportion = "--";
                            //上月当月指标占比
                            model.LastProportionCurrent = "--";
                        }
                        #endregion
                    }
                    #region 环比
                    //环比累计指标未完成家数变化
                    model.HuanMissTargetChange = model.ThisIsMissTarget - model.LastIsMissTarget;
                    //环比当月指标未完成家数变化
                    model.HuanMissTargetChangeCurrent = model.ThisIsMissCurrent - model.LastIsMissCurrent;
                    if (model.ThisEvaluationCompany > 0 && model.LastEvaluationCompany > 0)//仅上个月数据存在时,进行同比
                    {
                        //环比累计指标未完成家数占比变化
                        model.HuanMissTargetProportion = DivisionChange(model.ThisIsMissTarget, model.ThisEvaluationCompany, model.LastIsMissTarget, model.LastEvaluationCompany);

                        //环比当月指标未完成家数占比变化
                        model.HuanMissTargetPCurrent = DivisionChange(model.ThisIsMissCurrent, model.ThisEvaluationCompany, model.LastIsMissCurrent, model.LastEvaluationCompany);
                    }
                    else
                    {
                        model.HuanMissTargetProportion = "--";
                        model.HuanMissTargetPCurrent = "--";
                    }
                    #endregion



                    #region 去年数据
                    if (MonthReportC.Count > 0)
                    {

                        //去年考核范围内家数
                        R_MissTargetEvaluationScope LastYearTypeView = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(IDList[n], TargetList[i].ID, FinYear - 1, FinMonth, "NeedEvaluation");

                        if (LastYearTypeView != null)
                        {
                            model.YearEvaluationCompany = LastYearTypeView.EvaluationNumber;
                        }
                        else
                        {
                            model.YearEvaluationCompany = 0;
                        }
                        if (model.YearEvaluationCompany > 0)
                        {
                            //去年本月累计指标未完成家数
                            model.YearIsMissTarget = MonthReportC.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //去年当月指标未完成家数
                            model.YearIsMissCurrent = MonthReportC.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //去年累计指标占比
                            model.YearProportion = Division(model.YearIsMissTarget, model.YearEvaluationCompany);
                            //去年当月指标占比
                            model.YearProportionCurrent = Division(model.YearIsMissCurrent, model.YearEvaluationCompany);
                        }
                        else
                        {
                            //去年本月累计指标未完成家数
                            model.YearIsMissTarget = MonthReportC.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTarget == true && p.Counter != 0 && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //去年当月指标未完成家数
                            model.YearIsMissCurrent = MonthReportC.Where(p => p.TargetID == TargetList[i].ID && p.IsMissTargetCurrent == true && p.CompanyName.IndexOf("总部") == -1 && p.CompanyProperty1 != "筹备门店").OrderByDescending(p => p.CompanyID).Count();
                            //去年累计指标占比
                            model.YearProportion = "--";
                            //去年当月指标占比
                            model.YearProportionCurrent = "--";
                        }
                    }
                    #endregion



                    #region 同比
                    //同比累计指标未完成家数变化
                    model.TongMissTargetChange = model.ThisIsMissTarget - model.YearIsMissTarget;
                    //同比当月指标未完成家数变化
                    model.TongMissTargetChangeCurrent = model.ThisIsMissCurrent - model.YearIsMissCurrent;
                    if (model.ThisEvaluationCompany > 0 && model.YearEvaluationCompany > 0)//当月数据存在时进行同比
                    {
                        //同比累计指标未完成家数占比变化
                        model.TongMissTargetProportion = DivisionChange(model.ThisIsMissTarget, model.ThisEvaluationCompany, model.YearIsMissTarget, model.YearEvaluationCompany);
                        //同比当月指标未完成家数占比变化
                        model.TongMissTargetPCurrent = DivisionChange(model.ThisIsMissCurrent, model.ThisEvaluationCompany, model.YearIsMissCurrent, model.YearEvaluationCompany);
                    }
                    else
                    {
                        model.TongMissTargetProportion = "--";
                        model.TongMissTargetPCurrent = "--";
                    }

                    #endregion

                    result.Add(model);
                }
                #endregion

                ContrastMisstargetmodel.SystemName = SystemModel.SystemName;
                ContrastMisstargetmodel.ContrastMisstarget = result;
                returnresult.Add(ContrastMisstargetmodel);
            }

            return returnresult;
        }
        /// <summary>
        /// 占比计算
        /// </summary>
        /// <param name="x">未完成家数</param>
        /// <param name="y">考核家数</param>
        /// <returns></returns>
        private string Division(decimal x, decimal y)
        {
            decimal A = x / y;
            //Math.Round参数为2则为不保留小数,若需要,则增加
            int result = Convert.ToInt32(Math.Round(A, 2, MidpointRounding.AwayFromZero) * 100);

            return Convert.ToString(result) + "%";
        }
        /// <summary>
        /// 占比变化
        /// </summary>
        /// <param name="A">当月未完成家数</param>
        /// <param name="B">当月考核数量</param>
        /// <param name="C">上月未完成家数</param>
        /// <param name="D">上月考核数量</param>
        /// <returns></returns>
        private string DivisionChange(int A, int B, int C, int D)
        {
            //本月占比
            decimal X = Convert.ToDecimal(A) / Convert.ToDecimal(B);
            //上月占比
            decimal Y = Convert.ToDecimal(C) / Convert.ToDecimal(D);
            //占比变化
            decimal Change = X - Y;
            //变百分数
            int result = Convert.ToInt32(Math.Round(Change, 2, MidpointRounding.AwayFromZero) * 100);
            return Convert.ToString(result) + "%";
        }




        /// <summary>
        /// 获取完成情况年度对比表系统整体数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        ///  <param name="IsPro">如果为true取审批中数据</param>
        /// <returns></returns>
        public List<ContrastDetailList> GetContrastDetail(int FinYear, int FinMonth, bool IsPro)
        {
            string RemarkValue = "ContrastRemark";
            if (IsPro == false)
            {
                RemarkValue = "AContrastRemark";
            }

            //境内项目数据
            List<ContrastDetailList> Proresult = new List<ContrastDetailList>();
            //其他数据
            List<ContrastDetailList> result = new List<ContrastDetailList>();



            List<Guid> IDList = new List<Guid>();
            List<C_System> List = C_SystemOperator.Instance.GetSystemListBySeq().ToList();//先取出所有
            foreach (C_System item in List)//将他们的ID存入List
            {
                IDList.Add(item.ID);
            }
            IDList = IDList.Distinct().ToList();//去重      


            for (int s = 0; s < IDList.Count; s++)
            {

                ContrastDetailList model = new ContrastDetailList();
                List<C_ContrastDetail> adj = new List<C_ContrastDetail>();
                Guid ReportID = Guid.Empty;
                DateTime T = DateTime.Now;
                if (IsPro == true)
                {
                    B_MonthlyReport Report = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(IDList[s], FinYear, FinMonth);
                    if (Report != null)
                    {
                        ReportID = Report.ID;
                        T = Report.CreateTime;
                    }
                }
                else
                {
                    A_MonthlyReport ReportA = A_MonthlyreportOperator.Instance.GetAMonthlyReport(IDList[s], FinYear, FinMonth);
                    if (ReportA != null)
                    {
                        ReportID = ReportA.ID;
                        T = ReportA.CreateTime;
                    }
                }


                C_System SystemModel = StaticResource.Instance[IDList[s], T];
                //经营系统和境外分开显示    项目的南区北区中区文旅组合为境内项目


                if (ReportID != Guid.Empty)
                {


                    #region 计算开始
                    //如果不是境内项目系统
                    if (SystemModel.GroupType != "ProSystem")
                    {
                        int seq = SystemModel.Sequence * 150;
                        List<ContarstTargetDetailList> ContarstList = GetContarstTargetDetailList(ReportID, FinYear, FinMonth, SystemModel.ID, IsPro);

                        //取该系统下排列到第一的指标
                        List<C_Target> TargetList = StaticResource.Instance.GetTargetList(SystemModel.ID, T).ToList();
                        C_Target target = new C_Target();
                        if (TargetList != null) {
                            if (TargetList.Where(p => p.TargetType != 3 && p.NeedEvaluation == true).Count() != 0) {
                                target = TargetList.Where(p => p.TargetType != 3 && p.NeedEvaluation == true).OrderBy(p => p.Sequence).ToList().FirstOrDefault();
                            }
                       
                       
                            #region 提取数据

                        }
                        List<ContrastAllCompanyVM> NotConSum = new List<ContrastAllCompanyVM>();
                        List<ContrastAllCompanyVM> ConSum = new List<ContrastAllCompanyVM>();

                        if (IsPro == true)//取B表数据
                        {
                            //今年不可比门店
                            NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear, FinMonth, ReportID, target.ID, true).ToList();
                            //今年可比门店
                            ConSum = C_ExceptiontargetOperator.Instance.GetContrastCompanyTotal(FinYear, FinMonth, ReportID, target.ID, true).ToList();

                        }
                        else//取A表数据
                        {
                            //今年不可比门店
                            NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear, FinMonth, ReportID, target.ID, false).ToList();
                            //今年可比门店
                            ConSum = C_ExceptiontargetOperator.Instance.GetContrastCompanyTotal(FinYear, FinMonth, ReportID, target.ID, false).ToList();

                        }
                        //去年不可比门店
                        List<ContrastAllCompanyVM> LastNotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear - 1, FinMonth, SystemModel.ID, target.ID, false).ToList();
                        //去年可比门店
                        List<ContrastAllCompanyVM> LastConSum = C_ExceptiontargetOperator.Instance.GetContrastCompanyTotal(FinYear - 1, FinMonth, SystemModel.ID, target.ID, false).ToList();
                        //前年不可比门店
                        List<ContrastAllCompanyVM> LastMoreNotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear - 2, FinMonth, SystemModel.ID, target.ID, false).ToList();

                        //取出所有异常表中的不可比
                        List<C_Company> ANotContrastCompany = C_CompanyOperator.Instance.NotContrastCompany(SystemModel.ID, target.ID);

                        //获取今年所有不可比公司,每个不可比公司为今年为0并且去年为0
                        int Sum = 0;
                        for (int a = 0; a < ANotContrastCompany.Count; a++)
                        {
                            //查询该公司去年的数据
                            int A = LastNotConSum.Where(p => p.CompanyID == ANotContrastCompany[a].ID && p.NAccumulativeActualAmmount != 0).ToList().Count;
                            //查询该公司今年的数据
                            int B = NotConSum.Where(p => p.CompanyID == ANotContrastCompany[a].ID && p.NAccumulativeActualAmmount != 0).ToList().Count;
                            if (A != 0 || B != 0)
                            {

                                Sum++;
                            }
                        }

                        int LastSum = 0;
                        for (int a = 0; a < ANotContrastCompany.Count; a++)
                        {
                            //查询该公司去年的数据
                            int A = LastNotConSum.Where(p => p.CompanyID == ANotContrastCompany[a].ID && p.NAccumulativeActualAmmount != 0).ToList().Count;
                            //查询该公司今年的数据
                            int B = LastMoreNotConSum.Where(p => p.CompanyID == ANotContrastCompany[a].ID && p.NAccumulativeActualAmmount != 0).ToList().Count;
                            if (A != 0 || B != 0)
                            {
                                LastSum++;
                            }
                        }
                        //今年系统整体数量
                        int AllNowConut = Sum + ConSum.Count;
                        //去年系统整体数量
                        int AllLastCount = LastSum + LastConSum.Count;

                        #endregion

                        #region 门店数量(仅经营系统)
                        if (SystemModel.GroupType == "JYSystem")
                        {
                            //门店数量Model
                            C_ContrastDetail SumModel = new C_ContrastDetail();



                            SumModel.FinMonth = FinMonth;
                            SumModel.FinYear = FinYear;
                            SumModel.SystemID = SystemModel.ID;
                            SumModel.SystemName = SystemModel.SystemName;
                            SumModel.TargetName = "门店数量";
                            //不可比公司
                            SumModel.NotContrastLast = LastNotConSum.Count;
                            SumModel.NotContrastNow = Sum;
                            SumModel.NotDifference = CalculateDifference(Sum, LastNotConSum.Count);
                            SumModel.NotMounting = GrowthRate(NotConSum.Count, LastNotConSum.Count);

                            //可比公司
                            SumModel.PossibleContrastLast = LastConSum.Count;
                            SumModel.PossibleContrastNow = ConSum.Count;
                            SumModel.PossibleDifference = CalculateDifference(ConSum.Count, LastConSum.Count);
                            SumModel.PossibleMounting = GrowthRate(ConSum.Count, LastConSum.Count);


                            //门店整体

                            SumModel.LastAllTotal = AllLastCount;
                            SumModel.NowAllTotal = AllNowConut;
                            SumModel.Difference = CalculateDifference(AllNowConut, AllLastCount);
                            SumModel.Mounting = GrowthRate(AllNowConut, AllLastCount);

                            //门店数量排最上,所以不需要加值
                            SumModel.Sequence = seq;


                            //获取备注
                            R_MissTargetEvaluationScope Remarkmodel = new R_MissTargetEvaluationScope();
                            Guid guidempty = Guid.Parse("99999999-9999-9999-9999-999999999999");

                            Remarkmodel = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(SystemModel.ID, guidempty, FinYear, FinMonth, RemarkValue);


                            if (Remarkmodel == null)
                            {
                                SumModel.Remark = "--";
                            }
                            else
                            {
                                SumModel.Remark = Remarkmodel.ContrastRemark;
                            }
                            adj.Add(SumModel);

                        }

                        #endregion
                        #region 分指标数据


                        for (int i = 0; i < ContarstList.Count; i++)
                        {


                            //完成情况对比Model
                            C_ContrastDetail ConModel = new C_ContrastDetail();
                            #region 加载完成情况数据
                            //系统总体
                            ConModel.FinYear = FinYear;
                            ConModel.FinMonth = FinMonth;
                            ConModel.SystemID = SystemModel.ID;
                            ConModel.SystemName = SystemModel.SystemName;
                            ConModel.TargetID = ContarstList[i].TargetID;
                            ConModel.TargetName = ContarstList[i].TargetName;
                            ConModel.LastAllTotal = ContarstList[i].LastTotal;
                            ConModel.NowAllTotal = ContarstList[i].NowTotal; ;
                            ConModel.Difference = ContarstList[i].Difference;
                            ConModel.Mounting = ContarstList[i].Mounting;

                            //不可比公司
                            ConModel.NotContrastLast = ContarstList[i].NotContractLastTotal;
                            ConModel.NotContrastNow = ContarstList[i].NotContractNowTotal;
                            ConModel.NotDifference = ContarstList[i].NotContractDifference;
                            ConModel.NotMounting = ContarstList[i].NotContractMounting;

                            //可比公司
                            ConModel.PossibleContrastLast = ContarstList[i].ContractLastTotal;
                            ConModel.PossibleContrastNow = ContarstList[i].ContractNowTotal;
                            ConModel.PossibleDifference = ContarstList[i].ContractDifference;
                            ConModel.PossibleMounting = ContarstList[i].ContractMounting;
                            ConModel.Sequence = seq + i + 1;


                            //获取备注
                            R_MissTargetEvaluationScope Remarkmodel = new R_MissTargetEvaluationScope();
                            Remarkmodel = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(SystemModel.ID, ContarstList[i].TargetID, FinYear, FinMonth, RemarkValue);
                            if (Remarkmodel == null)
                            {
                                ConModel.Remark = "--";
                            }
                            else
                            {
                                ConModel.Remark = Remarkmodel.ContrastRemark;
                            }


                            #endregion

                            adj.Add(ConModel);
                        }
                        model.ContrastDetailMl = adj;
                        model.systemID = SystemModel.ID;
                        model.SystemName = SystemModel.SystemName;
                        model.MonthlyReportID = ReportID;
                        #endregion
                        result.Add(model);
                    }
                    else
                    {
                        if (Proresult.Count < 1)
                        {


                            #region 分指标数据
                            List<ContarstTargetDetailList> ContarstList = GetContarstTargetDetailList(ReportID, FinYear, FinMonth, SystemModel.ID, IsPro);
                            int seq = SystemModel.Sequence * 150;


                            for (int i = 0; i < ContarstList.Count; i++)
                            {


                                //完成情况对比Model
                                C_ContrastDetail ConModel = new C_ContrastDetail();
                                #region 加载完成情况数据
                                //系统总体
                                ConModel.FinYear = FinYear;
                                ConModel.FinMonth = FinMonth;
                                ConModel.SystemID = SystemModel.ID;
                                ConModel.SystemName = SystemModel.SystemName;
                                ConModel.TargetID = ContarstList[i].TargetID;
                                ConModel.TargetName = ContarstList[i].TargetName;
                                ConModel.LastAllTotal = ContarstList[i].LastTotal;
                                ConModel.NowAllTotal = ContarstList[i].NowTotal; ;
                                ConModel.Difference = ContarstList[i].Difference;
                                ConModel.Mounting = ContarstList[i].Mounting;

                                //不可比公司
                                ConModel.NotContrastLast = ContarstList[i].NotContractLastTotal;
                                ConModel.NotContrastNow = ContarstList[i].NotContractNowTotal;
                                ConModel.NotDifference = ContarstList[i].NotContractDifference;
                                ConModel.NotMounting = ContarstList[i].NotContractMounting;

                                //可比公司
                                ConModel.PossibleContrastLast = ContarstList[i].ContractLastTotal;
                                ConModel.PossibleContrastNow = ContarstList[i].ContractNowTotal;
                                ConModel.PossibleDifference = ContarstList[i].ContractDifference;
                                ConModel.PossibleMounting = ContarstList[i].ContractMounting;
                                ConModel.Sequence = seq + i + 1;
                                #endregion

                                adj.Add(ConModel);
                            }
                            model.ContrastDetailMl = adj;
                            model.systemID = SystemModel.ID;
                            model.SystemName = SystemModel.SystemName;
                            model.MonthlyReportID = ReportID;
                            #endregion
                            Proresult.Add(model);//Add到境内项目专用list中

                        }

                    }
                    #endregion
                }

            }


            if (Proresult.Count > 0)
            {
                ContrastDetailList Newmodel = new ContrastDetailList();
                Newmodel.SystemName = "境内项目";
                Newmodel.systemID = Proresult[0].systemID;
                Newmodel.MonthlyReportID = Proresult[0].MonthlyReportID;
                //合同数据
                C_ContrastDetail NewmodelDetailA = new C_ContrastDetail();
                NewmodelDetailA.SystemID = Proresult[0].systemID;
                NewmodelDetailA.SystemName = "境内项目";
                NewmodelDetailA.TargetID = Proresult[0].ContrastDetailMl[0].TargetID;//因为根据排序  无论哪个项目系统的合同收入都在回款之前  所以0必然为合同
                NewmodelDetailA.TargetName = Proresult[0].ContrastDetailMl[0].TargetName;
                NewmodelDetailA.FinMonth = FinMonth;
                NewmodelDetailA.FinYear = FinYear;

                //回款数据
                C_ContrastDetail NewmodelDetailB = new C_ContrastDetail();
                NewmodelDetailB.SystemID = Proresult[0].systemID;
                NewmodelDetailB.SystemName = "境内项目";
                NewmodelDetailB.TargetID = Proresult[0].ContrastDetailMl[1].TargetID;
                NewmodelDetailB.TargetName = Proresult[0].ContrastDetailMl[1].TargetName;
                NewmodelDetailB.FinMonth = FinMonth;
                NewmodelDetailB.FinYear = FinYear;
                for (int i = 0; i < Proresult.Count; i++)
                {
                    #region 合同数据合并
                    NewmodelDetailA.LastAllTotal += Proresult[i].ContrastDetailMl[0].LastAllTotal;
                    NewmodelDetailA.NowAllTotal += Proresult[i].ContrastDetailMl[0].NowAllTotal;

                    //不可比公司
                    NewmodelDetailA.NotContrastLast += Proresult[i].ContrastDetailMl[0].NotContrastLast;
                    NewmodelDetailA.NotContrastNow += Proresult[i].ContrastDetailMl[0].NotContrastNow;

                    //可比公司
                    NewmodelDetailA.PossibleContrastLast += Proresult[i].ContrastDetailMl[0].PossibleContrastLast;
                    NewmodelDetailA.PossibleContrastNow += Proresult[i].ContrastDetailMl[0].PossibleContrastNow;
                    #endregion


                    #region 回款数据合并
                    NewmodelDetailB.LastAllTotal += Proresult[i].ContrastDetailMl[1].LastAllTotal;
                    NewmodelDetailB.NowAllTotal += Proresult[i].ContrastDetailMl[1].NowAllTotal;
                    //不可比公司
                    NewmodelDetailB.NotContrastLast += Proresult[i].ContrastDetailMl[1].NotContrastLast;
                    NewmodelDetailB.NotContrastNow += Proresult[i].ContrastDetailMl[1].NotContrastNow;
                    //可比公司
                    NewmodelDetailB.PossibleContrastLast += Proresult[i].ContrastDetailMl[1].PossibleContrastLast;
                    NewmodelDetailB.PossibleContrastNow += Proresult[i].ContrastDetailMl[1].PossibleContrastNow;
                    #endregion
                }

                #region 重新计算差额等数据


                NewmodelDetailA.Difference = CalculateDifference(NewmodelDetailA.NowAllTotal, NewmodelDetailA.LastAllTotal);
                NewmodelDetailA.NotDifference = CalculateDifference(NewmodelDetailA.NotContrastNow, NewmodelDetailA.NotContrastLast);
                NewmodelDetailA.PossibleDifference = CalculateDifference(NewmodelDetailA.PossibleContrastNow, NewmodelDetailA.PossibleContrastLast);


                NewmodelDetailB.Difference = CalculateDifference(NewmodelDetailB.NowAllTotal, NewmodelDetailB.LastAllTotal);
                NewmodelDetailB.NotDifference = CalculateDifference(NewmodelDetailB.NotContrastNow, NewmodelDetailB.NotContrastLast);
                NewmodelDetailB.PossibleDifference = CalculateDifference(NewmodelDetailB.PossibleContrastNow, NewmodelDetailB.PossibleContrastLast);

                if (NewmodelDetailA.LastAllTotal != 0)
                {
                    NewmodelDetailA.Mounting = GrowthRate(NewmodelDetailA.NowAllTotal, NewmodelDetailA.LastAllTotal);

                }
                else
                {
                    NewmodelDetailA.Mounting = "--";
                }
                if (NewmodelDetailA.NotContrastLast != 0)
                {
                    NewmodelDetailA.NotMounting = GrowthRate(NewmodelDetailA.NotContrastNow, NewmodelDetailA.NotContrastLast);

                }
                else
                {
                    NewmodelDetailA.NotMounting = "--";
                }
                if (NewmodelDetailA.PossibleContrastLast != 0)
                {
                    NewmodelDetailA.PossibleMounting = GrowthRate(NewmodelDetailA.PossibleContrastNow, NewmodelDetailA.PossibleContrastLast);
                }
                else
                {
                    NewmodelDetailA.PossibleMounting = "--";
                }
                if (NewmodelDetailB.LastAllTotal != 0)
                {
                    NewmodelDetailB.Mounting = GrowthRate(NewmodelDetailB.NowAllTotal, NewmodelDetailB.LastAllTotal);
                }
                else NewmodelDetailB.Mounting = "--";
                if (NewmodelDetailB.NotContrastLast != 0)
                {
                    NewmodelDetailB.NotMounting = GrowthRate(NewmodelDetailB.NotContrastNow, NewmodelDetailB.NotContrastLast);

                }
                else
                {
                    NewmodelDetailB.NotMounting = "--";
                }
                if (NewmodelDetailB.PossibleContrastLast != 0)
                {
                    NewmodelDetailB.PossibleMounting = GrowthRate(NewmodelDetailB.PossibleContrastNow, NewmodelDetailB.PossibleContrastLast);
                }
                else
                {
                    NewmodelDetailB.PossibleMounting = "--";
                }
                #endregion


                #region  获取备注
                R_MissTargetEvaluationScope RemarkmodelA = new R_MissTargetEvaluationScope();
                RemarkmodelA = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(Proresult[0].systemID, Proresult[0].ContrastDetailMl[0].TargetID, FinYear, FinMonth, RemarkValue);
                if (RemarkmodelA == null)
                {
                    NewmodelDetailA.Remark = "--";
                }
                else
                {
                    NewmodelDetailA.Remark = RemarkmodelA.ContrastRemark;
                }
                R_MissTargetEvaluationScope RemarkmodelB = new R_MissTargetEvaluationScope();
                RemarkmodelB = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(Proresult[0].systemID, Proresult[0].ContrastDetailMl[1].TargetID, FinYear, FinMonth, RemarkValue);
                if (RemarkmodelB == null)
                {
                    NewmodelDetailB.Remark = "--";
                }
                else
                {
                    NewmodelDetailB.Remark = RemarkmodelB.ContrastRemark;
                }
                #endregion

                List<C_ContrastDetail> NewList = new List<C_ContrastDetail>();
                NewList.Add(NewmodelDetailA);
                NewList.Add(NewmodelDetailB);
                Newmodel.ContrastDetailMl = NewList;
                result.Add(Newmodel);
            }
            return result;
        }
        /// <summary>
        /// 获取完成情况年度对比表单个系统明细(在统计表中,会运行次方法读取数据)
        /// 因为项目系统在统计表中,合并为境内项目,因此在此方法中,将项目系统合并
        /// 而项目系统的明细数据,另写方法
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public List<ContarstTargetDetailList> GetContarstTargetDetailList(Guid MonthlyReportID, int FinYear, int FinMonth, Guid SystemID, bool IfPro)
        {
            DateTime T = DateTime.Now;
            if (MonthlyReportID != Guid.Empty)
            {
                B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthlyReportID);
                if (report != null)
                {
                    T = report.CreateTime;
                }
            }
            List<ContarstTargetDetailList> result = new List<ContarstTargetDetailList>();
            C_System System = StaticResource.Instance[SystemID, T];
            //如果是境内项目,则取出所有的境内项目,并且合并
            if (System.GroupType == "ProSystem")
            {
                ContarstTargetDetailList modelA = new ContarstTargetDetailList();
                ContarstTargetDetailList modelB = new ContarstTargetDetailList();
                List<ContrastCompanyList> AContrastList = new List<ContrastCompanyList>();
                List<ContrastCompanyList> ANotContrastList = new List<ContrastCompanyList>();
                List<ContrastCompanyList> BContrastList = new List<ContrastCompanyList>();
                List<ContrastCompanyList> BNotContrastList = new List<ContrastCompanyList>();
                List<C_System> ProSystem = C_SystemOperator.Instance.GetSystemListByGrouptype("ProSystem");
                string ATName = "";
                Guid ATId = Guid.Empty;
                string BTName = "";
                Guid BTId = Guid.Empty;
                for (int a = 0; a < ProSystem.Count; a++)
                {
                    List<MonthlyReportVM> AllDetail = new List<MonthlyReportVM>();
                    B_MonthlyReport Bs = B_MonthlyreportOperator.Instance.GetMonthlyReporNew(ProSystem[a].ID, FinYear, FinMonth);
                    Guid NewMID = Guid.Empty;
                    if (Bs != null)
                    {
                        NewMID = Bs.ID;
                        //取出今年所有数据
                        if (IfPro == true)
                        {
                            List<B_MonthlyReportDetail> Detail = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(NewMID).ToList();
                            if (Detail != null)
                            {
                                for (int i = 0; i < Detail.Count; i++)
                                {
                                    MonthlyReportVM m = new MonthlyReportVM();
                                    m.CompanyID = Detail[i].CompanyID;
                                    m.TargetID = Detail[i].TargetID;
                                    m.SystemID = Detail[i].SystemID;
                                    m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                                    AllDetail.Add(m);
                                }
                            }

                        }
                        else
                        {
                            List<A_MonthlyReportDetail> Detail = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(ProSystem[a].ID, FinYear, FinMonth).ToList();
                            if (Detail != null)
                            {
                                for (int i = 0; i < Detail.Count; i++)
                                {
                                    MonthlyReportVM m = new MonthlyReportVM();
                                    m.CompanyID = Detail[i].CompanyID;
                                    m.TargetID = Detail[i].TargetID;
                                    m.SystemID = Detail[i].SystemID;
                                    m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                                    AllDetail.Add(m);
                                }
                            }
                        }
                    }



                    DateTime Nt = DateTime.Now;
                    //取出去年所有数据
                    List<A_MonthlyReportDetail> AllLastDetail = GetAMonthlyreportdetailList(ProSystem[a].ID, FinYear - 1, FinMonth).ToList();
                    if (AllLastDetail != null && AllLastDetail.Count() !=0)
                    {
                        Nt = AllLastDetail[0].CreateTime;
                    }
                    List<C_Target> TargetList = StaticResource.Instance.GetTargetList(ProSystem[a].ID, Nt).ToList();

                    if (a == 0)
                    {
                        ATName = TargetList[0].TargetName;
                        ATId = TargetList[0].ID;//指标列表中的第一条数据  为合同数据
                        BTName = TargetList[1].TargetName;
                        BTId = TargetList[1].ID;//指标列表中的第二条数据  为回款数据
                    }



                    #region 取该系统的合同可比公司
                    //取该系统的合同可比公司
                    List<C_Company> A = C_CompanyOperator.Instance.ContrastCompany(ProSystem[a].ID, TargetList[0].ID);
                    foreach (C_Company item in A)
                    {
                        if (item.Sequence > 0)
                        {
                            ContrastCompanyList NewCom = new ContrastCompanyList();
                            NewCom.CompanyName = item.CompanyName;
                            NewCom.CompanyID = item.ID;
                            NewCom.OpeningTime = item.OpeningTime;

                            if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).ToList().Count > 0)
                            {
                                NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.NowAllTotal = 0;
                            }
                            if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).ToList().Count > 0)
                            {
                                NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.LastAllTotal = 0;
                            }
                            NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            AContrastList.Add(NewCom);
                        }
                    }

                    #endregion

                    #region 取该系统的合同不可比公司
                    //所有不可比公司

                    List<C_Company> B = C_CompanyOperator.Instance.NotContrastCompany(ProSystem[a].ID, TargetList[0].ID);
                    foreach (C_Company item in B)
                    {
                        if (item.Sequence > 0)
                        {
                            ContrastCompanyList NewCom = new ContrastCompanyList();
                            NewCom.CompanyName = item.CompanyName;
                            NewCom.CompanyID = item.ID;
                            NewCom.OpeningTime = item.OpeningTime;

                            if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).ToList().Count > 0)
                            {
                                NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.NowAllTotal = 0;
                            }
                            if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).ToList().Count > 0)
                            {
                                NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.LastAllTotal = 0;
                            }
                            NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            ANotContrastList.Add(NewCom);
                        }
                    }
                    #endregion


                    #region 取该系统的回款可比公司
                    //取该系统的合同可比公司
                    List<C_Company> C = C_CompanyOperator.Instance.ContrastCompany(ProSystem[a].ID, TargetList[1].ID);
                    foreach (C_Company item in C)
                    {
                        if (item.Sequence > 0)
                        {
                            ContrastCompanyList NewCom = new ContrastCompanyList();
                            NewCom.CompanyName = item.CompanyName;
                            NewCom.CompanyID = item.ID;
                            NewCom.OpeningTime = item.OpeningTime;

                            if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).ToList().Count > 0)
                            {
                                NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.NowAllTotal = 0;
                            }
                            if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).ToList().Count > 0)
                            {
                                NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.LastAllTotal = 0;
                            }
                            NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            BContrastList.Add(NewCom);
                        }
                    }

                    #endregion

                    #region 取该系统的回款不可比公司
                    //所有不可比公司

                    List<C_Company> D = C_CompanyOperator.Instance.NotContrastCompany(ProSystem[a].ID, TargetList[1].ID);
                    foreach (C_Company item in D)
                    {
                        if (item.Sequence > 0)
                        {
                            ContrastCompanyList NewCom = new ContrastCompanyList();
                            NewCom.CompanyName = item.CompanyName;
                            NewCom.CompanyID = item.ID;
                            NewCom.OpeningTime = item.OpeningTime;

                            if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).ToList().Count > 0)
                            {
                                NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.NowAllTotal = 0;
                            }
                            if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[1].ID).ToList().Count > 0)
                            {
                                NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[0].ID).FirstOrDefault().NAccumulativeActualAmmount;
                            }
                            else
                            {
                                NewCom.LastAllTotal = 0;
                            }
                            NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                            BNotContrastList.Add(NewCom);
                        }
                    }
                    #endregion

                }
                modelA.TargetID = ATId;
                modelA.TargetName = ATName;
                modelB.TargetID = BTId;
                modelB.TargetName = BTName;
                modelA.NotContrastList = ANotContrastList;
                modelA.ContrastList = AContrastList;
                modelB.NotContrastList = BNotContrastList;
                modelB.ContrastList = BContrastList;
                #region 系统数据

                #region A


                //可比门店数据
                modelA.ContractLastTotal = modelA.ContrastList.Sum(p => p.LastAllTotal);
                modelA.ContractNowTotal = modelA.ContrastList.Sum(p => p.NowAllTotal);
                modelA.ContractDifference = CalculateDifference(modelA.ContractNowTotal, modelA.ContractLastTotal);
                if (modelA.ContractLastTotal != 0)
                {
                    modelA.ContractMounting = GrowthRate(modelA.ContractNowTotal, modelA.ContractLastTotal);
                }
                else
                {
                    modelA.ContractMounting = "--";
                }
                //不可比门店数据
                modelA.NotContractLastTotal = modelA.NotContrastList.Sum(p => p.LastAllTotal);
                modelA.NotContractNowTotal = modelA.NotContrastList.Sum(p => p.NowAllTotal);
                modelA.NotContractDifference = CalculateDifference(modelA.NotContractNowTotal, modelA.NotContractLastTotal);
                if (modelA.NotContractLastTotal != 0)
                {
                    modelA.NotContractMounting = GrowthRate(modelA.NotContractNowTotal, modelA.NotContractLastTotal);
                }
                else
                {
                    modelA.NotContractMounting = "--";
                }


                //所有门店数据
                modelA.LastTotal = modelA.ContractLastTotal + modelA.NotContractLastTotal;
                modelA.NowTotal = modelA.ContractNowTotal + modelA.NotContractNowTotal;
                modelA.Difference = CalculateDifference(modelA.NowTotal, modelA.LastTotal);
                if (modelA.LastTotal != 0)
                {
                    modelA.Mounting = GrowthRate(modelA.NowTotal, modelA.LastTotal);
                }
                else
                {
                    modelA.Mounting = "--";
                }
                #endregion

                #region B
                //可比门店数据
                modelB.ContractLastTotal = modelB.ContrastList.Sum(p => p.LastAllTotal);
                modelB.ContractNowTotal = modelB.ContrastList.Sum(p => p.NowAllTotal);
                modelB.ContractDifference = CalculateDifference(modelB.ContractNowTotal, modelB.ContractLastTotal);
                if (modelB.ContractLastTotal != 0)
                {
                    modelB.ContractMounting = GrowthRate(modelB.ContractNowTotal, modelB.ContractLastTotal);
                }
                else
                {
                    modelB.ContractMounting = "--";
                }
                //不可比门店数据
                modelB.NotContractLastTotal = modelB.NotContrastList.Sum(p => p.LastAllTotal);
                modelB.NotContractNowTotal = modelB.NotContrastList.Sum(p => p.NowAllTotal);
                modelB.NotContractDifference = CalculateDifference(modelB.NotContractNowTotal, modelB.NotContractLastTotal);
                if (modelB.NotContractLastTotal != 0)
                {
                    modelB.NotContractMounting = GrowthRate(modelB.NotContractNowTotal, modelB.NotContractLastTotal);
                }
                else
                {
                    modelB.NotContractMounting = "--";
                }


                //所有门店数据
                modelB.LastTotal = modelB.ContractLastTotal + modelB.NotContractLastTotal;
                modelB.NowTotal = modelB.ContractNowTotal + modelB.NotContractNowTotal;
                modelB.Difference = CalculateDifference(modelB.NowTotal, modelB.LastTotal);
                if (modelB.LastTotal != 0)
                {
                    modelB.Mounting = GrowthRate(modelB.NowTotal, modelB.LastTotal);
                }
                else
                {
                    modelB.Mounting = "--";
                }
                #endregion

                #endregion
                result.Add(modelA);
                result.Add(modelB);
            }
            else
            {
                List<C_Target> TargetList = StaticResource.Instance.GetTargetList(SystemID, T).ToList();
                TargetList = TargetList.Where(p => p.TargetType != 3 && p.NeedEvaluation == true).OrderBy(p => p.Sequence).ToList();
                List<MonthlyReportVM> AllDetail = new List<MonthlyReportVM>();
                //取出今年所有数据
                if (IfPro == true)
                {
                    List<B_MonthlyReportDetail> Detail = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(MonthlyReportID).ToList();
                    if (Detail != null)
                    {
                        for (int i = 0; i < Detail.Count; i++)
                        {
                            MonthlyReportVM m = new MonthlyReportVM();
                            m.CompanyID = Detail[i].CompanyID;
                            m.TargetID = Detail[i].TargetID;
                            m.SystemID = Detail[i].SystemID;
                            m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                            AllDetail.Add(m);
                        }
                    }

                }
                else
                {
                    List<A_MonthlyReportDetail> Detail = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(SystemID, FinYear, FinMonth).ToList();
                    if (Detail != null)
                    {
                        for (int i = 0; i < Detail.Count; i++)
                        {
                            MonthlyReportVM m = new MonthlyReportVM();
                            m.CompanyID = Detail[i].CompanyID;
                            m.TargetID = Detail[i].TargetID;
                            m.SystemID = Detail[i].SystemID;
                            m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                            AllDetail.Add(m);
                        }
                    }
                }


                //取出去年所有数据
                List<A_MonthlyReportDetail> AllLastDetail = GetAMonthlyreportdetailList(SystemID, FinYear - 1, FinMonth).ToList();
                #region 分指标数据
                for (int i = 0; i < TargetList.Count; i++)
                {
                    ContarstTargetDetailList model = new ContarstTargetDetailList();
                    model.TargetName = TargetList[i].TargetName;
                    model.TargetID = TargetList[i].ID;
                    #region 获取并计算啊所有可比门店
                    //所有可比公司
                    List<ContrastCompanyList> ContrastList = new List<ContrastCompanyList>();
                    List<C_Company> ContrastCompany = C_CompanyOperator.Instance.ContrastCompany(SystemID, TargetList[i].ID);
                    foreach (C_Company item in ContrastCompany)
                    {
                        ContrastCompanyList NewCom = new ContrastCompanyList();
                        NewCom.CompanyName = item.CompanyName;
                        NewCom.CompanyID = item.ID;
                        NewCom.OpeningTime = item.OpeningTime;
                        if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.NowAllTotal = 0;
                        }
                        if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.LastAllTotal = 0;
                        }
                        NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        ContrastList.Add(NewCom);
                    }
                    model.ContrastList = ContrastList;
                    #endregion
                    #region 获取并计算所有不可比门店
                    List<ContrastCompanyList> NotContrastList = new List<ContrastCompanyList>();
                    //List<C_Company> NotContrastCompany = C_CompanyOperator.Instance.NotContrastCompany(SystemID, TargetList[i].ID);
                    //List<ContrastAllCompanyVM> NotConSum = new List<ContrastAllCompanyVM>();
                    ////如果为空  证明取去年数据A表数据     如果不为空 去去年数据
                    //if (MonthlyReportID!=Guid.Empty)
                    //{
                    //    NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear, FinMonth, MonthlyReportID, TargetList[i].ID, true).ToList();
                    //}
                    //else
                    //{
                    //    NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear-1, FinMonth, SystemID, TargetList[i].ID, false).ToList();
                    //}
                    List<C_Company> NotConSum = C_CompanyOperator.Instance.NotContrastCompany(SystemID, TargetList[i].ID);


                    foreach (C_Company item in NotConSum)
                    {
                        ContrastCompanyList NewCom = new ContrastCompanyList();
                        NewCom.CompanyName = item.CompanyName;
                        NewCom.CompanyID = item.ID;
                        NewCom.OpeningTime = item.OpeningTime;
                        if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.NowAllTotal = 0;
                        }
                        if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.LastAllTotal = 0;
                        }
                        NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NotContrastList.Add(NewCom);
                    }
                    model.NotContrastList = NotContrastList;
                    #endregion

                    //可比门店数据
                    model.ContractLastTotal = model.ContrastList.Sum(p => p.LastAllTotal);
                    model.ContractNowTotal = model.ContrastList.Sum(p => p.NowAllTotal);
                    model.ContractDifference = CalculateDifference(model.ContractNowTotal, model.ContractLastTotal);
                    if (model.ContractLastTotal != 0)
                    {
                        model.ContractMounting = GrowthRate(model.ContractNowTotal, model.ContractLastTotal);
                    }
                    else
                    {
                        model.ContractMounting = "--";
                    }
                    //不可比门店数据
                    model.NotContractLastTotal = model.NotContrastList.Sum(p => p.LastAllTotal);
                    model.NotContractNowTotal = model.NotContrastList.Sum(p => p.NowAllTotal);
                    model.NotContractDifference = CalculateDifference(model.NotContractNowTotal, model.NotContractLastTotal);
                    if (model.NotContractLastTotal != 0)
                    {
                        model.NotContractMounting = GrowthRate(model.NotContractNowTotal, model.NotContractLastTotal);
                    }
                    else
                    {
                        model.NotContractMounting = "--";
                    }


                    //所有门店数据
                    model.LastTotal = model.ContractLastTotal + model.NotContractLastTotal;
                    model.NowTotal = model.ContractNowTotal + model.NotContractNowTotal;
                    model.Difference = CalculateDifference(model.NowTotal, model.LastTotal);
                    if (model.LastTotal != 0)
                    {
                        model.Mounting = GrowthRate(model.NowTotal, model.LastTotal);
                    }
                    else
                    {
                        model.Mounting = "--";
                    }

                    result.Add(model);
                }
                #endregion
            }
            return result;
        }
        /// <summary>
        /// 专门给项目系统完成情况对比明细表写
        /// </summary>
        /// <param name="MonthlyReportID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="IfPro"></param>
        /// <returns></returns>
        public List<ContarstTargetDetailList> GetProContarstTargetDetailList(Guid MonthlyReportID, int FinYear, int FinMonth, Guid SystemID, bool IfPro)
        {
            DateTime T = DateTime.Now;
            if (MonthlyReportID != Guid.Empty)
            {
                B_MonthlyReport report = B_MonthlyreportOperator.Instance.GetMonthlyreport(MonthlyReportID);
                if (report != null)
                {
                    T = report.CreateTime;
                }
            }
            List<ContarstTargetDetailList> result = new List<ContarstTargetDetailList>();
            C_System System = StaticResource.Instance[SystemID, T];

            List<C_Target> TargetList = StaticResource.Instance.GetTargetList(SystemID, T).ToList();

            List<MonthlyReportVM> AllDetail = new List<MonthlyReportVM>();
            //取出今年所有数据
            if (IfPro == true)
            {
                List<B_MonthlyReportDetail> Detail = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(MonthlyReportID).ToList();
                if (Detail != null)
                {
                    for (int i = 0; i < Detail.Count; i++)
                    {
                        MonthlyReportVM m = new MonthlyReportVM();
                        m.CompanyID = Detail[i].CompanyID;
                        m.TargetID = Detail[i].TargetID;
                        m.SystemID = Detail[i].SystemID;
                        m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                        AllDetail.Add(m);
                    }
                }

            }
            else
            {
                List<A_MonthlyReportDetail> Detail = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(SystemID, FinYear, FinMonth).ToList();
                if (Detail != null)
                {
                    for (int i = 0; i < Detail.Count; i++)
                    {
                        MonthlyReportVM m = new MonthlyReportVM();
                        m.CompanyID = Detail[i].CompanyID;
                        m.TargetID = Detail[i].TargetID;
                        m.SystemID = Detail[i].SystemID;
                        m.NAccumulativeActualAmmount = Detail[i].NAccumulativeActualAmmount;
                        AllDetail.Add(m);
                    }
                }
            }


            //取出去年所有数据
            List<A_MonthlyReportDetail> AllLastDetail = GetAMonthlyreportdetailList(SystemID, FinYear - 1, FinMonth).ToList();
            #region 分指标数据
            for (int i = 0; i < TargetList.Count - 1; i++)
            {
                ContarstTargetDetailList model = new ContarstTargetDetailList();
                model.TargetName = TargetList[i].TargetName;
                model.TargetID = TargetList[i].ID;
                #region 获取并计算啊所有可比门店
                //所有可比公司
                List<ContrastCompanyList> ContrastList = new List<ContrastCompanyList>();
                List<C_Company> ContrastCompany = C_CompanyOperator.Instance.ContrastCompany(SystemID, TargetList[i].ID);
                foreach (C_Company item in ContrastCompany)
                {
                    if (item.Sequence > 0)
                    {

                        ContrastCompanyList NewCom = new ContrastCompanyList();
                        NewCom.CompanyName = item.CompanyName;
                        NewCom.CompanyID = item.ID;
                        NewCom.OpeningTime = item.OpeningTime;
                        if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.NowAllTotal = 0;
                        }
                        if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.LastAllTotal = 0;
                        }
                        NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        ContrastList.Add(NewCom);
                    }
                }
                model.ContrastList = ContrastList;
                #endregion
                #region 获取并计算所有不可比门店
                List<ContrastCompanyList> NotContrastList = new List<ContrastCompanyList>();
                //List<C_Company> NotContrastCompany = C_CompanyOperator.Instance.NotContrastCompany(SystemID, TargetList[i].ID);
                //List<ContrastAllCompanyVM> NotConSum = new List<ContrastAllCompanyVM>();
                ////如果为空  证明取去年数据A表数据     如果不为空 去去年数据
                //if (MonthlyReportID!=Guid.Empty)
                //{
                //    NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear, FinMonth, MonthlyReportID, TargetList[i].ID, true).ToList();
                //}
                //else
                //{
                //    NotConSum = C_ExceptiontargetOperator.Instance.GetNoContrastCompanyTotal(FinYear-1, FinMonth, SystemID, TargetList[i].ID, false).ToList();
                //}
                List<C_Company> NotConSum = C_CompanyOperator.Instance.NotContrastCompany(SystemID, TargetList[i].ID);


                foreach (C_Company item in NotConSum)
                {
                    if (item.Sequence > 0)
                    {
                        ContrastCompanyList NewCom = new ContrastCompanyList();
                        NewCom.CompanyName = item.CompanyName;
                        NewCom.CompanyID = item.ID;
                        NewCom.OpeningTime = item.OpeningTime;
                        if (AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.NowAllTotal = AllDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.NowAllTotal = 0;
                        }
                        if (AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).ToList().Count > 0)
                        {
                            NewCom.LastAllTotal = AllLastDetail.Where(p => p.CompanyID == item.ID && p.TargetID == TargetList[i].ID).FirstOrDefault().NAccumulativeActualAmmount;
                        }
                        else
                        {
                            NewCom.LastAllTotal = 0;
                        }
                        NewCom.Difference = CalculateDifference(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NewCom.Mounting = GrowthRate(NewCom.NowAllTotal, NewCom.LastAllTotal);
                        NotContrastList.Add(NewCom);
                    }
                }
                model.NotContrastList = NotContrastList;
                #endregion

                //可比门店数据
                model.ContractLastTotal = model.ContrastList.Sum(p => p.LastAllTotal);
                model.ContractNowTotal = model.ContrastList.Sum(p => p.NowAllTotal);
                model.ContractDifference = CalculateDifference(model.ContractNowTotal, model.ContractLastTotal);
                if (model.ContractLastTotal != 0)
                {
                    model.ContractMounting = GrowthRate(model.ContractNowTotal, model.ContractLastTotal);
                }
                else
                {
                    model.ContractMounting = "--";
                }
                //不可比门店数据
                model.NotContractLastTotal = model.NotContrastList.Sum(p => p.LastAllTotal);
                model.NotContractNowTotal = model.NotContrastList.Sum(p => p.NowAllTotal);
                model.NotContractDifference = CalculateDifference(model.NotContractNowTotal, model.NotContractLastTotal);
                if (model.NotContractLastTotal != 0)
                {
                    model.NotContractMounting = GrowthRate(model.NotContractNowTotal, model.NotContractLastTotal);
                }
                else
                {
                    model.NotContractMounting = "--";
                }


                //所有门店数据
                model.LastTotal = model.ContractLastTotal + model.NotContractLastTotal;
                model.NowTotal = model.ContractNowTotal + model.NotContractNowTotal;
                model.Difference = CalculateDifference(model.NowTotal, model.LastTotal);
                if (model.LastTotal != 0)
                {
                    model.Mounting = GrowthRate(model.NowTotal, model.LastTotal);
                }
                else
                {
                    model.Mounting = "--";
                }

                result.Add(model);
            }
            #endregion

            return result;
        }


        /// <summary>
        /// 差额计算
        /// </summary>
        /// <param name="X">今年数据</param>
        /// <param name="Y">去年数据</param>
        /// <returns></returns>
        private decimal CalculateDifference(decimal X, decimal Y)
        {
            return X - Y;
        }
        /// <summary>
        /// 增长率计算
        /// </summary>
        /// <param name="X">今年数据</param>
        /// <param name="Y">去年数据</param>
        /// <returns></returns>
        private string GrowthRate(decimal X, decimal Y)
        {
            string result;
            if (Y == 0)
            {
                result = "--";
            }
            else
            {
                //差额
                decimal diff = X - Y;
                //比率
                decimal RateA = diff / Math.Abs(Y);

                if (Y < 0 && diff < 0)
                {
                    result = "增亏" + Convert.ToInt64(Math.Round(Math.Abs(RateA), 2, MidpointRounding.AwayFromZero) * 100) + "%";
                }
                else if (Y < 0 && diff > 0)
                {
                    result = "减亏" + Convert.ToInt64(Math.Round(Math.Abs(RateA), 2, MidpointRounding.AwayFromZero) * 100) + "%";
                }
                else if (diff == 0)
                {
                    result = "0";
                }
                else
                {
                    result = Convert.ToInt64(Math.Round(RateA, 2, MidpointRounding.AwayFromZero) * 100) + "%";
                }
            }
            return result;
        }



    }
}

