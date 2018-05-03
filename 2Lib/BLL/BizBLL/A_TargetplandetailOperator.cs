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
using System.Data;
using LJTH.BusinessIndicators.ViewModel;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Targetplandetail对象的业务逻辑操作
    /// </summary>
    public class A_TargetplandetailOperator : BizOperatorBase<A_TargetPlanDetail>
    {

        #region Generate Code

        public static readonly A_TargetplandetailOperator Instance = PolicyInjection.Create<A_TargetplandetailOperator>();

        private static A_TargetplandetailAdapter _aTargetplandetailAdapter = AdapterFactory.GetAdapter<A_TargetplandetailAdapter>();

        protected override BaseAdapterT<A_TargetPlanDetail> GetAdapter()
        {
            return _aTargetplandetailAdapter;
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList()
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetTargetplandetailList();
            return result;
        }

        public IList<A_TargetPlanDetail> GetProTargetplan(Guid SystemID, Guid TargetID, int FinYear)
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetProTargetplan(SystemID, TargetID, FinYear);
            return result;
        }


        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid TargetPlanID)
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetTargetplandetailList(TargetPlanID);
            return result;
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid SystemID, int FinYear)
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetTargetplandetailList(SystemID, FinYear);
            return result;
        }
        public IList<A_TargetPlanDetail> GetTargetplandetailList(int FinYear)
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetTargetplandetailList(FinYear);
            return result;
        }

        public IList<A_TargetPlanDetail> GetTargetplandetailList(Guid SystemID, int FinYear, Guid CompanyID)
        {
            IList<A_TargetPlanDetail> result = _aTargetplandetailAdapter.GetTargetplandetailList(SystemID, FinYear, CompanyID);
            return result;
        }

        public Guid AddTargetplandetail(A_TargetPlanDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_TargetPlanDetail GetTargetplandetail(Guid aTargetplandetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aTargetplandetailID == null, "Argument aTargetplandetailID is Empty");
            return base.GetModelObject(aTargetplandetailID);
        }

        public Guid UpdateTargetplandetail(A_TargetPlanDetail data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveTargetplandetail(Guid aTargetplandetailID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aTargetplandetailID == null, "Argument aTargetplandetailID is Empty");
            Guid result = base.RemoveObject(aTargetplandetailID);
            return result;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public int AddTargetPlanDetailList(List<A_TargetPlanDetail> List)
        {
            if (List.Count > 0)
            {
                return _aTargetplandetailAdapter.AddMonthlyreportdetailLisr(List);
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
        public int DeleteTargetPlanDetailList(List<A_TargetPlanDetail> List)
        {
            if (List.Count > 0)
            {
                return _aTargetplandetailAdapter.DeleteTargetPlanDetailList(List);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        /// <summary>
        /// 获取累计指标汇总数据
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<TargetDetail> GetSumMonthTargetDetail(int FinYear, Guid SystemID)
        {
            List<TargetDetail> result = new List<TargetDetail>();



            List<A_TargetPlanDetail> TD = GetTargetplandetailList(SystemID, FinYear).ToList();
            DateTime t = DateTime.Now;
            if (TD.Count > 0)
            {
                Guid i = TD[0].TargetPlanID;
                B_TargetPlan mo = B_TargetplanOperator.Instance.GetTargetplan(i);
                if (mo != null)
                {
                    t = mo.CreateTime;
                }
            }
            C_System Sys = StaticResource.Instance[SystemID, t];
            for (int Month = 1; Month <= 12; Month++)
            {
                TargetDetail model = new TargetDetail();
                List<TargetDetailList> TargetDetailList = new List<TargetDetailList>();
                model.FinMonth = Month;
                DataTable ds = new DataTable();
                if (Sys.Category != 2)
                {
                    ds = _aTargetplandetailAdapter.GetSumMonthTargetDetailJY(FinYear, Month, SystemID, t);
                }
                else
                {
                    C_Company com = C_CompanyOperator.Instance.ProCompanyAll(SystemID);
                    ds = _aTargetplandetailAdapter.GetSumMonthTargetDetailPro(FinYear, Month, com.ID, t);
                }
                if (ds != null)
                {
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        TargetDetailList view = new TargetDetailList() { Target = 0, SumTarget = 0 };
                        view.TargetName = ds.Rows[i]["TargetName"].ToString();
                        view.TargetID = ds.Rows[i]["TargetID"].ToString().ToGuid();
                        if (!string.IsNullOrEmpty(ds.Rows[i]["target"].ToString()))
                        {
                            view.Target = decimal.Parse(ds.Rows[i]["target"].ToString());
                        }
                        if (!string.IsNullOrEmpty(ds.Rows[i]["SumTarget"].ToString()))
                        {
                            view.SumTarget = decimal.Parse(ds.Rows[i]["SumTarget"].ToString());
                        }

                        TargetDetailList.Add(view);
                    }
                }
                else
                {
                    List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(SystemID, DateTime.Now).ToList();

                    foreach (C_Target item in TargetList)
                    {
                        TargetDetailList view = new TargetDetailList() { Target = null, SumTarget = null };
                        view.TargetName = item.TargetName;
                        view.TargetID = item.ID;
                        TargetDetailList.Add(view);
                    }
                }
                model.TargetDetailList = TargetDetailList;
                result.Add(model);
            }
            return result;

        }


        public List<TargetDetail> GetSumMonthTargetDetailByTID(Guid TargetPlanID)
        {
            List<TargetDetail> result = new List<TargetDetail>();

            B_TargetPlan TargetPlan = B_TargetplanOperator.Instance.GetTargetplan(TargetPlanID);

            C_System Sys = StaticResource.Instance[TargetPlan.SystemID, TargetPlan.CreateTime];

            for (int Month = 1; Month <= 12; Month++)
            {
                TargetDetail model = new TargetDetail();
                List<TargetDetailList> TargetDetailList = new List<TargetDetailList>();
                model.FinMonth = Month;
                DataTable ds = new DataTable();
                if (Sys.Category != 2)
                {
                    ds = _aTargetplandetailAdapter.GetSumMonthTargetDetailByTID(Month, TargetPlanID, TargetPlan.CreateTime);
                }
                else
                {
                    C_Company com = C_CompanyOperator.Instance.ProCompanyAll(Sys.ID);
                    ds = _aTargetplandetailAdapter.GetSumMonthTargetDetailProById(Month, com.ID, TargetPlanID, TargetPlan.CreateTime);
                }
                if (ds != null)
                {
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        TargetDetailList view = new TargetDetailList() { Target = 0, SumTarget = 0 };
                        view.TargetName = ds.Rows[i]["TargetName"].ToString();
                        view.TargetID = ds.Rows[i]["TargetID"].ToString().ToGuid();
                        if (ds.Rows[i]["target"].ToString() != null)
                        {
                            view.Target = decimal.Parse(ds.Rows[i]["target"].ToString());
                        }
                        if (ds.Rows[i]["SumTarget"].ToString() != null)
                        {
                            view.SumTarget = decimal.Parse(ds.Rows[i]["SumTarget"].ToString());
                        }

                        TargetDetailList.Add(view);
                    }
                }
                else
                {
                    List<C_Target> TargetList = StaticResource.Instance.GetTargetList(Sys.ID, TargetPlan.CreateTime).ToList();

                    foreach (C_Target item in TargetList)
                    {
                        TargetDetailList view = new TargetDetailList() { Target = null, SumTarget = null };
                        view.TargetName = item.TargetName;
                        view.TargetID = item.ID;
                        TargetDetailList.Add(view);
                    }
                }
                model.TargetDetailList = TargetDetailList;
                result.Add(model);
            }
            return result;

        }
        public List<TargetPlanDetailVList> GetSumTargetDetail(int FinYear, Guid SystemID)
        {
            List<TargetPlanDetailVList> result = new List<TargetPlanDetailVList>();
            B_TargetPlan BL = null;
            //取出B表中最新一版审批通过数据  如果数据为空 则取当前时间   该操作用来获取版本
            //B_TargetPlan BL = B_TargetplanOperator.Instance.GetTargetPlanByProgressOrApproved(SystemID, FinYear);

            List<A_TargetPlanDetail> ADetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(SystemID, FinYear).ToList();
            if (ADetail != null && ADetail.Count > 0)
            {
                BL = B_TargetplanOperator.Instance.GetTargetPlanByID(ADetail[0].TargetPlanID);
            }
            List<C_Target> TargetList = new List<C_Target>();
            C_System Sys = new C_System();

            if (BL == null)
            {
                TargetList = StaticResource.Instance.GetTargetList(SystemID, DateTime.Now).ToList();
                Sys = StaticResource.Instance[SystemID, DateTime.Now];
            }
            else
            {
                TargetList = StaticResource.Instance.GetTargetList(SystemID, BL.CreateTime).ToList();
                Sys = StaticResource.Instance[SystemID, BL.CreateTime];
            }


            for (int i = 0; i < TargetList.Count; i++)
            {
                TargetPlanDetailVList model = new TargetPlanDetailVList();
                List<TargetPlanDetailVModel> ModelList = new List<TargetPlanDetailVModel>();
                model.TargetID = TargetList[i].ID;
                model.TargetName = TargetList[i].TargetName;
                model.Unit = TargetList[i].Unit;
                DataTable ds = _aTargetplandetailAdapter.GetSumTargetDetail(FinYear, SystemID, TargetList[i].ID);
                decimal SumTar1 = 0;
                decimal SumTar2 = 0;
                decimal SumTar3 = 0;
                decimal SumTar4 = 0;
                decimal SumTar5 = 0;
                decimal SumTar6 = 0;
                decimal SumTar7 = 0;
                decimal SumTar8 = 0;
                decimal SumTar9 = 0;
                decimal SumTar10 = 0;
                decimal SumTar11 = 0;
                decimal SumTar12 = 0;
                decimal SumTarsum1 = 0;
                decimal SumTarsum2 = 0;
                decimal SumTarsum3 = 0;
                decimal SumTarsum4 = 0;
                decimal SumTarsum5 = 0;
                decimal SumTarsum6 = 0;
                decimal SumTarsum7 = 0;
                decimal SumTarsum8 = 0;
                decimal SumTarsum9 = 0;
                decimal SumTarsum10 = 0;
                decimal SumTarsum11 = 0;
                decimal SumTarsum12 = 0;
                int num = 0;
                if (ds != null)
                {
                    for (int j = 0; j < ds.Rows.Count; j++)
                    {
                        num++;
                        TargetPlanDetailVModel View = new TargetPlanDetailVModel()
                        {
                            Target1 = 0,
                            Target2 = 0,
                            Target3 = 0,
                            Target4 = 0,
                            Target5 = 0,
                            Target6 = 0,
                            Target7 = 0,
                            Target8 = 0,
                            Target9 = 0,
                            Target10 = 0,
                            Target11 = 0,
                            Target12 = 0,
                            TargetSum1 = 0,
                            TargetSum2 = 0,
                            TargetSum3 = 0,
                            TargetSum4 = 0,
                            TargetSum5 = 0,
                            TargetSum6 = 0,
                            TargetSum7 = 0,
                            TargetSum8 = 0,
                            TargetSum9 = 0,
                            TargetSum10 = 0,
                            TargetSum11 = 0,
                            TargetSum12 = 0
                        };
                        View.seq = num;
                        View.CompanyName = ds.Rows[j]["CompanyName"].ToString();
                        View.CompanyID = ds.Rows[j]["ID"].ToString().ToGuid();
                        var s = ds.Rows[j]["OpeningTime"].ToString();
                        if (ds.Rows[j]["OpeningTime"].ToString() != null && ds.Rows[j]["OpeningTime"].ToString() != "")
                        {
                            View.OpenTime = DateTime.Parse(ds.Rows[j]["OpeningTime"].ToString()).ToShortDateString();
                        }
                        else
                        {
                            View.OpenTime = "--";
                        }

                        //当该系统不是项目系统时,数据累加  若为项目系统 取项目小计的值即可
                        if (Sys.Category != 2)
                        {
                            #region  指标的读取 当月指标
                            if (ds.Rows[j]["Target1"].ToString() != "")
                            {
                                View.Target1 = decimal.Parse(ds.Rows[j]["Target1"].ToString());
                                SumTar1 += View.Target1;//当月指标累计
                            }

                            if (ds.Rows[j]["Target2"].ToString() != "")
                            {
                                View.Target2 = decimal.Parse(ds.Rows[j]["Target2"].ToString());
                                SumTar2 += View.Target2;
                            }
                            if (ds.Rows[j]["Target3"].ToString() != "")
                            {
                                View.Target3 = decimal.Parse(ds.Rows[j]["Target3"].ToString());
                                SumTar3 += View.Target3;
                            }
                            if (ds.Rows[j]["Target4"].ToString() != "")
                            {
                                View.Target4 = decimal.Parse(ds.Rows[j]["Target4"].ToString());
                                SumTar4 += View.Target4;
                            }
                            if (ds.Rows[j]["Target5"].ToString() != "")
                            {
                                View.Target5 = decimal.Parse(ds.Rows[j]["Target5"].ToString());
                                SumTar5 += View.Target5;
                            }
                            if (ds.Rows[j]["Target6"].ToString() != "")
                            {
                                View.Target6 = decimal.Parse(ds.Rows[j]["Target6"].ToString());
                                SumTar6 += View.Target6;

                            }

                            if (ds.Rows[j]["Target7"].ToString() != "")
                            {
                                View.Target7 = decimal.Parse(ds.Rows[j]["Target7"].ToString());
                                SumTar7 += View.Target7;
                            }
                            if (ds.Rows[j]["Target8"].ToString() != "")
                            {
                                View.Target8 = decimal.Parse(ds.Rows[j]["Target8"].ToString());
                                SumTar8 += View.Target8;
                            }
                            if (ds.Rows[j]["Target9"].ToString() != "")
                            {
                                View.Target9 = decimal.Parse(ds.Rows[j]["Target9"].ToString());
                                SumTar9 += View.Target9;
                            }
                            if (ds.Rows[j]["Target10"].ToString() != "")
                            {
                                View.Target10 = decimal.Parse(ds.Rows[j]["Target10"].ToString());
                                SumTar10 += View.Target10;
                            }
                            if (ds.Rows[j]["Target11"].ToString() != "")
                            {
                                View.Target11 = decimal.Parse(ds.Rows[j]["Target11"].ToString());
                                SumTar11 += View.Target11;
                            }
                            if (ds.Rows[j]["Target12"].ToString() != "")
                            {
                                View.Target12 = decimal.Parse(ds.Rows[j]["Target12"].ToString());
                                SumTar12 += View.Target12;
                            }
                            #endregion
                            #region 累计指标
                            View.TargetSum1 = decimal.Parse(ds.Rows[j]["SumTarget1"].ToString());
                            SumTarsum1 += View.TargetSum1;//累计指标合计
                            View.TargetSum2 = decimal.Parse(ds.Rows[j]["SumTarget2"].ToString());
                            SumTarsum2 += View.TargetSum2;
                            View.TargetSum3 = decimal.Parse(ds.Rows[j]["SumTarget3"].ToString());
                            SumTarsum3 += View.TargetSum3;
                            View.TargetSum4 = decimal.Parse(ds.Rows[j]["SumTarget4"].ToString());
                            SumTarsum4 += View.TargetSum4;
                            View.TargetSum5 = decimal.Parse(ds.Rows[j]["SumTarget5"].ToString());
                            SumTarsum5 += View.TargetSum5;
                            View.TargetSum6 = decimal.Parse(ds.Rows[j]["SumTarget6"].ToString());
                            SumTarsum6 += View.TargetSum6;
                            View.TargetSum7 = decimal.Parse(ds.Rows[j]["SumTarget7"].ToString());
                            SumTarsum7 += View.TargetSum7;
                            View.TargetSum8 = decimal.Parse(ds.Rows[j]["SumTarget8"].ToString());
                            SumTarsum8 += View.TargetSum8;
                            View.TargetSum9 = decimal.Parse(ds.Rows[j]["SumTarget9"].ToString());
                            SumTarsum9 += View.TargetSum9;
                            View.TargetSum10 = decimal.Parse(ds.Rows[j]["SumTarget10"].ToString());
                            SumTarsum10 += View.TargetSum10;
                            View.TargetSum11 = decimal.Parse(ds.Rows[j]["SumTarget11"].ToString());
                            SumTarsum11 += View.TargetSum11;
                            View.TargetSum12 = decimal.Parse(ds.Rows[j]["SumTarget12"].ToString());
                            SumTarsum12 += View.TargetSum12;
                            #endregion
                        }
                        else
                        {
                            if (ds.Rows[j]["Target1"].ToString() != "")
                            {
                                View.Target1 = decimal.Parse(ds.Rows[j]["Target1"].ToString());

                            }

                            if (ds.Rows[j]["Target2"].ToString() != "")
                            {
                                View.Target2 = decimal.Parse(ds.Rows[j]["Target2"].ToString());

                            }
                            if (ds.Rows[j]["Target3"].ToString() != "")
                            {
                                View.Target3 = decimal.Parse(ds.Rows[j]["Target3"].ToString());

                            }
                            if (ds.Rows[j]["Target4"].ToString() != "")
                            {
                                View.Target4 = decimal.Parse(ds.Rows[j]["Target4"].ToString());

                            }
                            if (ds.Rows[j]["Target5"].ToString() != "")
                            {
                                View.Target5 = decimal.Parse(ds.Rows[j]["Target5"].ToString());

                            }
                            if (ds.Rows[j]["Target6"].ToString() != "")
                            {
                                View.Target6 = decimal.Parse(ds.Rows[j]["Target6"].ToString());

                            }
                            if (ds.Rows[j]["Target7"].ToString() != "")
                            {
                                View.Target7 = decimal.Parse(ds.Rows[j]["Target7"].ToString());

                            }
                            if (ds.Rows[j]["Target8"].ToString() != "")
                            {
                                View.Target8 = decimal.Parse(ds.Rows[j]["Target8"].ToString());

                            }
                            if (ds.Rows[j]["Target9"].ToString() != "")
                            {
                                View.Target9 = decimal.Parse(ds.Rows[j]["Target9"].ToString());

                            }
                            if (ds.Rows[j]["Target10"].ToString() != "")
                            {
                                View.Target10 = decimal.Parse(ds.Rows[j]["Target10"].ToString());

                            }
                            if (ds.Rows[j]["Target11"].ToString() != "")
                            {
                                View.Target11 = decimal.Parse(ds.Rows[j]["Target11"].ToString());

                            }
                            if (ds.Rows[j]["Target12"].ToString() != "")
                            {
                                View.Target12 = decimal.Parse(ds.Rows[j]["Target12"].ToString());

                            }
                            View.TargetSum1 = decimal.Parse(ds.Rows[j]["SumTarget1"].ToString());
                            View.TargetSum2 = decimal.Parse(ds.Rows[j]["SumTarget2"].ToString());
                            View.TargetSum3 = decimal.Parse(ds.Rows[j]["SumTarget3"].ToString());
                            View.TargetSum4 = decimal.Parse(ds.Rows[j]["SumTarget4"].ToString());
                            View.TargetSum5 = decimal.Parse(ds.Rows[j]["SumTarget5"].ToString());
                            View.TargetSum6 = decimal.Parse(ds.Rows[j]["SumTarget6"].ToString());
                            View.TargetSum7 = decimal.Parse(ds.Rows[j]["SumTarget7"].ToString());
                            View.TargetSum8 = decimal.Parse(ds.Rows[j]["SumTarget8"].ToString());
                            View.TargetSum9 = decimal.Parse(ds.Rows[j]["SumTarget9"].ToString());
                            View.TargetSum10 = decimal.Parse(ds.Rows[j]["SumTarget10"].ToString());
                            View.TargetSum11 = decimal.Parse(ds.Rows[j]["SumTarget11"].ToString());
                            View.TargetSum12 = decimal.Parse(ds.Rows[j]["SumTarget12"].ToString());
                            List<A_TargetPlanDetail> sumlist = GetProTargetplan(SystemID, TargetList[i].ID, FinYear).ToList();

                            SumTar1 = sumlist.Where(p => p.FinMonth == 1).ToList().FirstOrDefault().Target;
                            SumTar2 = sumlist.Where(p => p.FinMonth == 2).ToList().FirstOrDefault().Target;
                            SumTar3 = sumlist.Where(p => p.FinMonth == 3).ToList().FirstOrDefault().Target;
                            SumTar4 = sumlist.Where(p => p.FinMonth == 4).ToList().FirstOrDefault().Target;
                            SumTar5 = sumlist.Where(p => p.FinMonth == 5).ToList().FirstOrDefault().Target;
                            SumTar6 = sumlist.Where(p => p.FinMonth == 6).ToList().FirstOrDefault().Target;
                            SumTar7 = sumlist.Where(p => p.FinMonth == 7).ToList().FirstOrDefault().Target;
                            SumTar8 = sumlist.Where(p => p.FinMonth == 8).ToList().FirstOrDefault().Target;
                            SumTar9 = sumlist.Where(p => p.FinMonth == 9).ToList().FirstOrDefault().Target;
                            SumTar10 = sumlist.Where(p => p.FinMonth == 10).ToList().FirstOrDefault().Target;
                            SumTar11 = sumlist.Where(p => p.FinMonth == 11).ToList().FirstOrDefault().Target;
                            SumTar12 = sumlist.Where(p => p.FinMonth == 12).ToList().FirstOrDefault().Target;
                            SumTarsum1 = sumlist.Where(p => p.FinMonth <= 1).Sum(o => o.Target);
                            SumTarsum2 = sumlist.Where(p => p.FinMonth <= 2).Sum(o => o.Target);
                            SumTarsum3 = sumlist.Where(p => p.FinMonth <= 3).Sum(o => o.Target);
                            SumTarsum4 = sumlist.Where(p => p.FinMonth <= 4).Sum(o => o.Target);
                            SumTarsum5 = sumlist.Where(p => p.FinMonth <= 5).Sum(o => o.Target);
                            SumTarsum6 = sumlist.Where(p => p.FinMonth <= 6).Sum(o => o.Target);
                            SumTarsum7 = sumlist.Where(p => p.FinMonth <= 7).Sum(o => o.Target);
                            SumTarsum8 = sumlist.Where(p => p.FinMonth <= 8).Sum(o => o.Target);
                            SumTarsum9 = sumlist.Where(p => p.FinMonth <= 9).Sum(o => o.Target);
                            SumTarsum10 = sumlist.Where(p => p.FinMonth <= 10).Sum(o => o.Target);
                            SumTarsum11 = sumlist.Where(p => p.FinMonth <= 11).Sum(o => o.Target);
                            SumTarsum12 = sumlist.Where(p => p.FinMonth <= 12).Sum(o => o.Target);

                        }
                        ModelList.Add(View);
                    }
                }
                model.TargetPlanDetailList = ModelList;
                model.SumTarget1 = SumTar1;
                model.SumTarget2 = SumTar2;
                model.SumTarget3 = SumTar3;
                model.SumTarget4 = SumTar4;
                model.SumTarget5 = SumTar5;
                model.SumTarget6 = SumTar6;
                model.SumTarget7 = SumTar7;
                model.SumTarget8 = SumTar8;
                model.SumTarget9 = SumTar9;
                model.SumTarget10 = SumTar10;
                model.SumTarget11 = SumTar11;
                model.SumTarget12 = SumTar12;
                model.SumTargetSum1 = SumTarsum1;
                model.SumTargetSum2 = SumTarsum2;
                model.SumTargetSum3 = SumTarsum3;
                model.SumTargetSum4 = SumTarsum4;
                model.SumTargetSum5 = SumTarsum5;
                model.SumTargetSum6 = SumTarsum6;
                model.SumTargetSum7 = SumTarsum7;
                model.SumTargetSum8 = SumTarsum8;
                model.SumTargetSum9 = SumTarsum9;
                model.SumTargetSum10 = SumTarsum10;
                model.SumTargetSum11 = SumTarsum11;
                model.SumTargetSum12 = SumTarsum12;
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 根据TargetPlanID，获取全年的指标
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="SystemID"></param>
        /// <param name="TargetPlanID"></param>
        /// <returns></returns>
        public List<TargetPlanDetailVList> GetSumTargetDetailApprove(int FinYear, Guid SystemID, Guid TargetPlanID)
        {
            List<TargetPlanDetailVList> result = new List<TargetPlanDetailVList>();
            B_TargetPlan BL = null;
            //取出B表中最新一版审批通过数据  如果数据为空 则取当前时间   该操作用来获取版本
            //B_TargetPlan BL = B_TargetplanOperator.Instance.GetTargetPlanByProgressOrApproved(SystemID, FinYear);

            BL = B_TargetplanOperator.Instance.GetTargetPlanByID(TargetPlanID);
            List<C_Target> TargetList = new List<C_Target>();
            C_System Sys = new C_System();

            if (BL == null)
            {
                TargetList = StaticResource.Instance.GetTargetList(SystemID, DateTime.Now).ToList();
                Sys = StaticResource.Instance[SystemID, DateTime.Now];
            }
            else
            {
                TargetList = StaticResource.Instance.GetTargetList(SystemID, BL.CreateTime).ToList();
                Sys = StaticResource.Instance[SystemID, BL.CreateTime];
            }


            for (int i = 0; i < TargetList.Count; i++)
            {
                TargetPlanDetailVList model = new TargetPlanDetailVList();
                List<TargetPlanDetailVModel> ModelList = new List<TargetPlanDetailVModel>();
                model.TargetID = TargetList[i].ID;
                model.TargetName = TargetList[i].TargetName;
                model.Unit = TargetList[i].Unit;
                DataTable ds = _aTargetplandetailAdapter.GetSumTargetDetail_B(FinYear, SystemID, TargetList[i].ID, TargetPlanID);
                decimal SumTar1 = 0;
                decimal SumTar2 = 0;
                decimal SumTar3 = 0;
                decimal SumTar4 = 0;
                decimal SumTar5 = 0;
                decimal SumTar6 = 0;
                decimal SumTar7 = 0;
                decimal SumTar8 = 0;
                decimal SumTar9 = 0;
                decimal SumTar10 = 0;
                decimal SumTar11 = 0;
                decimal SumTar12 = 0;
                decimal SumTarsum12 = 0;
                int num = 0;
                if (ds != null)
                {
                    for (int j = 0; j < ds.Rows.Count; j++)
                    {
                        num++;
                        TargetPlanDetailVModel View = new TargetPlanDetailVModel()
                        {
                            Target1 = 0,
                            Target2 = 0,
                            Target3 = 0,
                            Target4 = 0,
                            Target5 = 0,
                            Target6 = 0,
                            Target7 = 0,
                            Target8 = 0,
                            Target9 = 0,
                            Target10 = 0,
                            Target11 = 0,
                            Target12 = 0,
                            TargetSum1 = 0,
                            TargetSum2 = 0,
                            TargetSum3 = 0,
                            TargetSum4 = 0,
                            TargetSum5 = 0,
                            TargetSum6 = 0,
                            TargetSum7 = 0,
                            TargetSum8 = 0,
                            TargetSum9 = 0,
                            TargetSum10 = 0,
                            TargetSum11 = 0,
                            TargetSum12 = 0

                        };
                        View.seq = num;
                        View.CompanyName = ds.Rows[j]["CompanyName"].ToString();
                        View.CompanyID = ds.Rows[j]["ID"].ToString().ToGuid();
                        var s = ds.Rows[j]["OpeningTime"].ToString();
                        if (ds.Rows[j]["OpeningTime"].ToString() != null && ds.Rows[j]["OpeningTime"].ToString() != "")
                        {
                            View.OpenTime = DateTime.Parse(ds.Rows[j]["OpeningTime"].ToString()).ToShortDateString();
                        }
                        else
                        {
                            View.OpenTime = "--";
                        }

                        //当该系统不是项目系统时,数据累加  若为项目系统 取项目小计的值即可
                        if (Sys.Category != 2)
                        {
                            #region  指标的读取 当月指标
                            if (ds.Rows[j]["Target1"].ToString() != "")
                            {
                                View.Target1 = decimal.Parse(ds.Rows[j]["Target1"].ToString());
                                SumTar1 += View.Target1;//当月指标累计
                            }

                            if (ds.Rows[j]["Target2"].ToString() != "")
                            {
                                View.Target2 = decimal.Parse(ds.Rows[j]["Target2"].ToString());
                                SumTar2 += View.Target2;
                            }
                            if (ds.Rows[j]["Target3"].ToString() != "")
                            {
                                View.Target3 = decimal.Parse(ds.Rows[j]["Target3"].ToString());
                                SumTar3 += View.Target3;
                            }
                            if (ds.Rows[j]["Target4"].ToString() != "")
                            {
                                View.Target4 = decimal.Parse(ds.Rows[j]["Target4"].ToString());
                                SumTar4 += View.Target4;
                            }
                            if (ds.Rows[j]["Target5"].ToString() != "")
                            {
                                View.Target5 = decimal.Parse(ds.Rows[j]["Target5"].ToString());
                                SumTar5 += View.Target5;
                            }
                            if (ds.Rows[j]["Target6"].ToString() != "")
                            {
                                View.Target6 = decimal.Parse(ds.Rows[j]["Target6"].ToString());
                                SumTar6 += View.Target6;

                            }

                            if (ds.Rows[j]["Target7"].ToString() != "")
                            {
                                View.Target7 = decimal.Parse(ds.Rows[j]["Target7"].ToString());
                                SumTar7 += View.Target7;
                            }
                            if (ds.Rows[j]["Target8"].ToString() != "")
                            {
                                View.Target8 = decimal.Parse(ds.Rows[j]["Target8"].ToString());
                                SumTar8 += View.Target8;
                            }
                            if (ds.Rows[j]["Target9"].ToString() != "")
                            {
                                View.Target9 = decimal.Parse(ds.Rows[j]["Target9"].ToString());
                                SumTar9 += View.Target9;
                            }
                            if (ds.Rows[j]["Target10"].ToString() != "")
                            {
                                View.Target10 = decimal.Parse(ds.Rows[j]["Target10"].ToString());
                                SumTar10 += View.Target10;
                            }
                            if (ds.Rows[j]["Target11"].ToString() != "")
                            {
                                View.Target11 = decimal.Parse(ds.Rows[j]["Target11"].ToString());
                                SumTar11 += View.Target11;
                            }
                            if (ds.Rows[j]["Target12"].ToString() != "")
                            {
                                View.Target12 = decimal.Parse(ds.Rows[j]["Target12"].ToString());
                                SumTar12 += View.Target12;
                            }
                            #endregion

                            View.TargetSum12 = decimal.Parse(ds.Rows[j]["SumTarget12"].ToString());
                            SumTarsum12 += View.TargetSum12;

                        }
                        else
                        {
                            if (ds.Rows[j]["Target1"].ToString() != "")
                            {
                                View.Target1 = decimal.Parse(ds.Rows[j]["Target1"].ToString());

                            }

                            if (ds.Rows[j]["Target2"].ToString() != "")
                            {
                                View.Target2 = decimal.Parse(ds.Rows[j]["Target2"].ToString());

                            }
                            if (ds.Rows[j]["Target3"].ToString() != "")
                            {
                                View.Target3 = decimal.Parse(ds.Rows[j]["Target3"].ToString());

                            }
                            if (ds.Rows[j]["Target4"].ToString() != "")
                            {
                                View.Target4 = decimal.Parse(ds.Rows[j]["Target4"].ToString());

                            }
                            if (ds.Rows[j]["Target5"].ToString() != "")
                            {
                                View.Target5 = decimal.Parse(ds.Rows[j]["Target5"].ToString());

                            }
                            if (ds.Rows[j]["Target6"].ToString() != "")
                            {
                                View.Target6 = decimal.Parse(ds.Rows[j]["Target6"].ToString());

                            }
                            if (ds.Rows[j]["Target7"].ToString() != "")
                            {
                                View.Target7 = decimal.Parse(ds.Rows[j]["Target7"].ToString());

                            }
                            if (ds.Rows[j]["Target8"].ToString() != "")
                            {
                                View.Target8 = decimal.Parse(ds.Rows[j]["Target8"].ToString());

                            }
                            if (ds.Rows[j]["Target9"].ToString() != "")
                            {
                                View.Target9 = decimal.Parse(ds.Rows[j]["Target9"].ToString());

                            }
                            if (ds.Rows[j]["Target10"].ToString() != "")
                            {
                                View.Target10 = decimal.Parse(ds.Rows[j]["Target10"].ToString());

                            }
                            if (ds.Rows[j]["Target11"].ToString() != "")
                            {
                                View.Target11 = decimal.Parse(ds.Rows[j]["Target11"].ToString());

                            }
                            if (ds.Rows[j]["Target12"].ToString() != "")
                            {
                                View.Target12 = decimal.Parse(ds.Rows[j]["Target12"].ToString());

                            }

                            View.TargetSum12 = decimal.Parse(ds.Rows[j]["SumTarget12"].ToString());


                            var sumlist = B_TargetplandetailOperator.Instance.GetProTargetplan(SystemID, TargetList[i].ID, FinYear, TargetPlanID);

                            SumTar12 = sumlist.Where(p => p.FinMonth == 12).ToList().FirstOrDefault().Target;

                            SumTarsum12 = sumlist.Where(p => p.FinMonth <= 12).Sum(o => o.Target);



                        }
                        ModelList.Add(View);
                    }
                }
                model.TargetPlanDetailList = ModelList;
                model.SumTarget1 = SumTar1;
                model.SumTarget2 = SumTar2;
                model.SumTarget3 = SumTar3;
                model.SumTarget4 = SumTar4;
                model.SumTarget5 = SumTar5;
                model.SumTarget6 = SumTar6;
                model.SumTarget7 = SumTar7;
                model.SumTarget8 = SumTar8;
                model.SumTarget9 = SumTar9;
                model.SumTarget10 = SumTar10;
                model.SumTarget11 = SumTar11;
                model.SumTarget12 = SumTar12;
                model.SumTargetSum12 = SumTarsum12;
                result.Add(model);
            }
            return result;
        }







        /// <summary>
        /// 获取公司整年指标的总数（未完成选择，承诺必保时，提供的数据）
        /// </summary>
        /// <param name="TargetPlanID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public List<V_PlanTargetModel> GetAnnualPlanTarget(Guid TargetPlanID, int FinYear)
        {
            return _aTargetplandetailAdapter.GetAnnualPlanTarget(TargetPlanID, FinYear).ToList();
        }


        public List<A_TargetPlanDetail> GetTargetplandetailListBySYNC(int FinYear, int FinMonth)
        {
            return _aTargetplandetailAdapter.GetTargetplandetailListBySYNC(FinYear, FinMonth);
        }



    }
}

