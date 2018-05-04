using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Model.BizModel;

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
                else if (DateTime.Now > _instance.timer.AddHours(1))
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
            _CompanyList = null;
            _TargetList = new Dictionary<Guid, List<C_Target>>();
            _KpiList = new Dictionary<int, List<C_TargetKpi>>();
            _companyList = null;
            _ExceptionTargetList = null;
            _OrgList = new Dictionary<Guid, List<S_Organizational>>();
            _ReportDateTime = null;
            _TargetPlanDetail = new Dictionary<int, List<A_TargetPlanDetail>>();
        }

        public C_System this[Guid Key,DateTime CurrentDate ]
        {
            get
            {
                if (SystemList != null && SystemList.Count > 0)
                {
                    return SystemList.ToList().Find(S => S.ID == Key);
                    //return C_SystemOperator.Instance.GetSystemList(CurrentDate).ToList().Find(S => S.ID == Key);

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

        private IList<C_Company> _companyList = null;
        private IList<C_Company> ALLCompanyList
        {
            get
            {
                if(_companyList == null)
                {
                    IList<C_Company> list = C_CompanyOperator.Instance.GetCompanyList();
                    _companyList = list;
                }
                return _companyList;
            }
        }

        private Dictionary<Guid, List<C_Company>> _CompanyList = new Dictionary<Guid, List<C_Company>>();
        public Dictionary<Guid, List<C_Company>> CompanyList
        {
            get
            {
                if (_CompanyList == null || _CompanyList.Count <= 0)
                {
                    var g = ALLCompanyList.GroupBy(x => x.SystemID);
                    g.ForEach(x => {
                        _CompanyList.Add(x.Key, x.ToList());
                    });
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
            C_Company _Model = ALLCompanyList.Where(X => X.ID == CompanyID).First();
            // C_Company _Model = C_CompanyOperator.Instance.GetCompany(CompanyID);
            if (_Model != null || !string.IsNullOrEmpty(_Model.CompanyName))
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
            list.AddRange(c.Where(x => x.IsCompany).Select(x=>x.ID));
            c.Where(x => !x.IsCompany).ForEach(x => {
                GetChildren(systemid, x.ID, list);
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
                    var g = list.GroupBy(x => x.SystemID);
                    g.ForEach(x =>
                    {
                        _TargetList.Add(x.Key, x.ToList());
                    });
                }
                return _TargetList;
            }
        }

        public IList<C_Target> GetTargetList(Guid SystemID,DateTime CurrentDate )
        {
            IList<C_Target> list = null;
            if (CurrentDate.Year == DateTime.Now.Year && CurrentDate.Month == DateTime.Now.Month && CurrentDate.Day == DateTime.Now.Day)
            {
                list = TargetList[SystemID];
            }
            else
            {
                list = C_TargetOperator.Instance.GetTargetList(SystemID, CurrentDate);
            }
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list;
        }


        public IList<C_Target> GetTargetListBySysID(Guid SystemID)
        {
            //IList<C_Target> list = C_TargetOperator.Instance.GetTargetList(SystemID ,DateTime.Now);
            IList<C_Target> list = TargetList[SystemID];
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list;
        }
        
        public List<C_TargetKpi> GetKpiList(Guid SystemID, int FinYear)
        {
            if (_KpiList == null || _KpiList.Count <= 0||!_KpiList.Keys.Contains(FinYear))
            {
                var kpis= C_TargetkpiOperator.Instance.GetTargetkpiList(FinYear).ToList();
                _KpiList[FinYear] = kpis;
            }
            return _KpiList[FinYear];
        
        } private Dictionary<int, List<C_TargetKpi>> _KpiList = new Dictionary<int, List<C_TargetKpi>>();

        //public List<B_TargetPlan> GetTargetPlan(Guid SystemID, int FinYear)
        //{
        //    if (_TargetPlan == null || TargetPlanList.Count <= 0)
        //    {
        //        _TargetPlan = B_TargetplanOperator.Instance.GetTargetplanList().ToList();
        //    }
        //    return _TargetPlan.FindAll(P => P.SystemID == SystemID && P.FinYear == FinYear);
        //}
        //private List<B_TargetPlan> _TargetPlan = new List<B_TargetPlan>();

        private Dictionary<int,List<A_TargetPlanDetail>> _TargetPlanDetail = new Dictionary<int, List<A_TargetPlanDetail>>();
        /// <summary>
        /// 获取计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <param name="FinMonth">月</param>
        /// <returns>计划指标</returns>
        public List<A_TargetPlanDetail> GetTargetPlanList(Guid SystemID, int FinYear, int FinMonth)
        {
            if (_TargetPlanDetail == null || _TargetPlanDetail.Count <= 0||!_TargetPlanDetail.Keys.Contains(FinYear))
            {
                var list = A_TargetplandetailOperator.Instance.GetTargetplandetailList(FinYear).ToList();
                _TargetPlanDetail[FinYear] = list;
            }
            //if (!TargetPlanList.Exists(P => P.FinYear == FinYear && P.SystemID == SystemID))
            //{
            //    TargetPlanList.AddRange(A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList());
            //}
            return _TargetPlanDetail[FinYear].FindAll(P => P.SystemID == SystemID && P.FinYear == FinYear && P.FinMonth == FinMonth);
        }

        /// <summary>
        /// 获取计划指标
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="FinYear">年</param>
        /// <returns>计划指标</returns>
        public List<A_TargetPlanDetail> GetTargetPlanList(Guid SystemID, int FinYear)
        {
            if (_TargetPlanDetail == null || _TargetPlanDetail.Count <= 0 || !_TargetPlanDetail.Keys.Contains(FinYear))
            {
                var list = A_TargetplandetailOperator.Instance.GetTargetplandetailList(FinYear).ToList();
                _TargetPlanDetail[FinYear] = list;
            }
            return _TargetPlanDetail[FinYear].FindAll(P => P.SystemID == SystemID);
        }

        private List<C_ExceptionTarget> ExceptionTargetList
        {
            get
            {
                if (_ExceptionTargetList == null || _ExceptionTargetList.Count <= 0)
                {
                    _ExceptionTargetList = C_ExceptiontargetOperator.Instance.GetExceptionTList().ToList();
                }
                return _ExceptionTargetList;
            }
        }
        private List<C_ExceptionTarget> _ExceptionTargetList = null;

        /// <summary>
        /// 获取异常指标
        /// </summary>
        /// <param name="SystemID">公司ID</param>
        /// <returns>计划指标</returns>
        public List<C_ExceptionTarget> GetExceptionTargetList(Guid _CompanyID, Guid _TargetID)
        {
            return ExceptionTargetList.FindAll(P => P.CompanyID == _CompanyID && P.TargetID == _TargetID);
        }

        public List<C_ExceptionTarget> GetExceptionTargetList() 
        {
            return ExceptionTargetList;
        }

        private DateTime? _ReportDateTime = null;
        /// <summary>
        /// 获取上报时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetReportDateTime()
        {
            if (_ReportDateTime == null)
            {
                _ReportDateTime = C_ReportTimeOperator.Instance.GetReportTime().ReportTime;
                if (_ReportDateTime == null)
                    _ReportDateTime = DateTime.Now.AddMonths(-1);
            }
            return _ReportDateTime.Value;
          
        }
        /// <summary>
        /// 根据板块ID获取有效的组织架构信息
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetAllDataBySystemID(Guid systemID)
        {
            return OrgList[systemID];
            //return S_OrganizationalActionOperator.Instance.GetAllDataBySystemID(systemID);
        }
        /// <summary>
        /// 根据板块ID获取是否包含区域
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns>true存在区域false不存在区域</returns>
        public bool GetSystem_Regional(Guid systemID)
        {
            return OrgList[systemID].Count(x => !x.IsCompany && x.ParentID == systemID)>0;
            //var result = S_OrganizationalActionOperator.Instance.GetSystem_Regional(systemID);
            //return result != null && result.Any() ? true : false;
        }

        /// <summary>
        /// 获取板块当前年、月指标版本信息
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>true存在区域false不存在区域</returns>
        public Dictionary<Guid,string> GetTargetVersionType(string systemID, int year, int month,bool IsLatestVersion)
        {
            Dictionary<Guid, string> dc = new Dictionary<Guid, string>();
            //是否查询审批中数据
            if (IsLatestVersion)
            {
                dc = B_TargetplanOperator.Instance.GetTargetVersionType(systemID, year, month);
            }
            else
            {
                dc = A_TargetplanOperator.Instance.GetTargetVersionType(systemID, year, month);
            }
            return dc;
        }
    }
}
