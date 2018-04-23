using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.DAL.BizDal;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.BLL
{
    public class StaticResource
    {

        public Int64 RemoveDot(decimal d)
        {
            decimal t = Math.Abs(d);
            while (t%1> 0)
            {
                t = t * 10;
            }
            try
            {
                return Convert.ToInt64(t);
            }
            catch (Exception exp) 
            { 
                Console.WriteLine( exp); 
            }
            return Convert.ToInt64(d);
        }

        DateTime timer = DateTime.Now;
        public static StaticResource Instance
        {
            get
            {

                if (_instance == null)
                {
                    _instance = new StaticResource();
                }
                else if (DateTime.Now > _instance.timer.AddMinutes(10))
                {
                    _instance.Reload();
                    _instance.timer = DateTime.Now;
                }
                return _instance;
            }
        }private static StaticResource _instance;

        void Reload()
        {
            _SystemList = null;
            _CompanyList = new Dictionary<Guid, List<C_Company>>();
            _TargetList = new Dictionary<Guid, List<C_Target>>();
            KpiList = new List<C_TargetKpi>();
            TargetPlanList = new List<A_TargetPlanDetail>();
        }

        public C_System this[Guid Key,DateTime CurrentDate ]
        {
            get
            {
                if (SystemList != null && SystemList.Count > 0)
                {
                    //return SystemList.ToList().Find(S => S.ID == Key);
                    return C_SystemOperator.Instance.GetSystemList(CurrentDate).ToList().Find(S => S.ID == Key);

                }
                return null;
            }
        }

        public IList<C_System> SystemList
        {
            get
            {
                if (_SystemList == null || _SystemList.Count <= 0)
                {

                    DateTime CurrentDate = DateTime.Now;
                    _SystemList = C_SystemOperator.Instance.GetSystemList(CurrentDate).OrderBy(S => S.Sequence).ToList();
                }
                return _SystemList;
            }
        }private IList<C_System> _SystemList = null;


        private Dictionary<Guid, List<C_Company>> _CompanyList = new Dictionary<Guid, List<C_Company>>();
        public Dictionary<Guid, List<C_Company>> CompanyList
        {
            get
            {
                if (_CompanyList == null || _CompanyList.Count <= 0)
                {
                    IList<C_Company> list = C_CompanyOperator.Instance.GetCompanyList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var c in list)
                        {
                            if (!_CompanyList.ContainsKey(c.SystemID))
                            {
                                _CompanyList.Add(c.SystemID, new List<C_Company>());
                            }
                            _CompanyList[c.SystemID].Add(c);
                        }
                    }
                }
                return _CompanyList;
            }
        }
        private Dictionary<Guid, List<S_Organizational>> _OrgList = new Dictionary<Guid, List<S_Organizational>>();
        public Dictionary<Guid, List<S_Organizational>> OrgList
        {
            get
            {
                if (_OrgList == null || _OrgList.Count <= 0)
                {
                    IList<S_Organizational> list = S_OrganizationalActionOperator.Instance.GetAllData();
                    var g = list.GroupBy(x => x.SystemID);
                    g.ForEach(x => _OrgList.Add(x.Key, x.ToList()));
                }
                return _OrgList;
            }
        }
        /// <summary>
        /// 获取公司
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public C_Company GetCompanyModel(Guid CompanyID)
        {
            
             C_Company _Model = C_CompanyOperator.Instance.GetCompany(CompanyID);

             if (_Model != null)
             {
                 return _Model;
             }

             return null;
        }
        /// <summary>
        /// 获取区域下所有的公司id
        /// </summary>
        /// <param name="systemid"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public IList<Guid> GetCompanyIds(Guid systemid,Guid areaId)
        {
            var list = new List<Guid>();
            GetChildren(systemid, areaId, list);
            return list;
        }
        private void GetChildren(Guid systemid,Guid parentId,List<Guid> list)
        {
            var c= OrgList[systemid].Where(x => x.ParentID == parentId);
            c.ForEach(x => {
                if (x.IsCompany)
                {
                    list.Add(x.ID);
                    return;
                }
                else
                {
                    GetChildren(systemid, x.ID, list);
                }
            });
        }


        private Dictionary<Guid, List<C_Target>> _TargetList = new Dictionary<Guid, List<C_Target>>();
        public Dictionary<Guid, List<C_Target>> TargetList
        {
            get
            {
                if (_TargetList == null || _TargetList.Count <= 0)
                {
                    IList<C_Target> list = C_TargetOperator.Instance.GetTargetList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var t in list)
                        {
                            if (!_TargetList.ContainsKey(t.SystemID))
                            {
                                _TargetList.Add(t.SystemID, new List<C_Target>());
                            }
                            _TargetList[t.SystemID].Add(t);
                        }
                    }
                }
                return _TargetList;
            }
        }

        public IList<C_Target> GetTargetList(Guid SystemID,DateTime CurrentDate )
        {
            IList<C_Target> list = C_TargetOperator.Instance.GetTargetList(SystemID, CurrentDate);

            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list;
        }


        public IList<C_Target> GetTargetListBySysID(Guid SystemID)
        {
            IList<C_Target> list = C_TargetOperator.Instance.GetTargetList(SystemID ,DateTime.Now);

            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list;
        }


        private List<C_TargetKpi> KpiList = new List<C_TargetKpi>();

        public List<C_TargetKpi> GetKpiList(Guid SystemID, int FinYear)
        {
            if (KpiList == null || KpiList.Count <= 0)
            {
                KpiList = C_TargetkpiOperator.Instance.GetTargetkpiList().ToList();
            }
            return KpiList.FindAll(C => C.SystemID == SystemID && C.FinYear == FinYear);
        
        }

        private List<B_TargetPlan> TargetPlan   = new List<B_TargetPlan>();
        public List<B_TargetPlan> GetTargetPlan(Guid SystemID, int FinYear)
        {
            if (TargetPlan == null || TargetPlanList.Count <= 0)
            {
                TargetPlan = B_TargetplanOperator.Instance.GetTargetplanList().ToList();
            }
            return TargetPlan.FindAll(P => P.SystemID == SystemID && P.FinYear == FinYear);
        }

        private List<A_TargetPlanDetail> TargetPlanList = new List<A_TargetPlanDetail>();
        /// <summary>
        /// 获取计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <returns>计划指标</returns>
        public List<A_TargetPlanDetail> GetTargetPlanList(Guid SystemID, int FinYear, int FinMonth)
        {
            if (TargetPlanList == null || TargetPlanList.Count <= 0)
            {
                TargetPlanList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList();
            }
            if (!TargetPlanList.Exists(P => P.FinYear == FinYear && P.SystemID == SystemID))
            {
                TargetPlanList.AddRange(A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList());
            }
            return TargetPlanList.FindAll(P => P.SystemID == SystemID && P.FinYear == FinYear && P.FinMonth == FinMonth);
        }

        /// <summary>
        /// 获取计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <returns>计划指标</returns>
        public List<A_TargetPlanDetail> GetTargetPlanList(Guid SystemID, int FinYear)
        {
            if (TargetPlanList == null || TargetPlanList.Count <= 0)
            {
                TargetPlanList = A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList();
            }
            if (!TargetPlanList.Exists(P => P.FinYear == FinYear && P.SystemID == SystemID))
            {
                TargetPlanList.AddRange(A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList());
            }
            return TargetPlanList.FindAll(P => P.SystemID == SystemID && P.FinYear == FinYear );
        }

        private List<C_ExceptionTarget> ExceptionTargetList = new List<C_ExceptionTarget>();
        /// <summary>
        /// 获取异常指标
        /// </summary>
        /// <param name="SystemID">公司ID</param>
        /// <returns>计划指标</returns>
        public List<C_ExceptionTarget> GetExceptionTargetList(Guid _CompanyID, Guid _TargetID)
        {
            if (ExceptionTargetList == null || ExceptionTargetList.Count <= 0)
            {
                ExceptionTargetList = C_ExceptiontargetOperator.Instance.GetExceptionTList().ToList();
            }
            if (!ExceptionTargetList.Exists(P => P.CompanyID == _CompanyID && P.TargetID == _TargetID))
            {
                ExceptionTargetList.AddRange(C_ExceptiontargetOperator.Instance.GetExceptiontargetList(_CompanyID, _TargetID).ToList());
            }
            return ExceptionTargetList.FindAll(P => P.CompanyID == _CompanyID && P.TargetID == _TargetID);
        }


        public List<C_ExceptionTarget> GetExceptionTargetList() 
        {
            ExceptionTargetList = C_ExceptiontargetOperator.Instance.GetExceptionTList().ToList();
            return ExceptionTargetList;
        }

        private DateTime? ReportDateTime = new DateTime();
        /// <summary>
        /// 获取上报时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetReportDateTime()
        {
            ReportDateTime = C_ReportTimeOperator.Instance.GetReportTime().ReportTime;
            if (ReportDateTime==null)
            {
                ReportDateTime = DateTime.Now.AddMonths(-1);
            }
            return ReportDateTime.Value;
          
        }
        /// <summary>
        /// 根据板块ID获取有效的组织架构信息
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetAllDataBySystemID(Guid systemID)
        {
            return S_OrganizationalActionOperator.Instance.GetAllDataBySystemID(systemID);
        }
    }
}
