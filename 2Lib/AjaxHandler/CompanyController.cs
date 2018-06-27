using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Web;
using Newtonsoft.Json;
using Lib.Core;
using Lib.Xml;
using Lib.Expression;
namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    public class CompanyController : BaseController
    {
        [LibAction]
        public List<C_Company> GetCompanyList(string SysID)
        {
            List<C_Company> ComList = C_CompanyOperator.Instance.GetCompanyList(Guid.Parse(SysID)).OrderBy(p => p.Sequence).ToList();
            return ComList;
        }
        [LibAction]
        public string CompanyXMLList(string SysID)
        {
            string xml;
            XElement xelement = StaticResource.Instance[Guid.Parse(SysID), DateTime.Now].Configuration;

            if (xelement != null)
            {
                List<XElement> XMLList1 = xelement.Elements("ListCompanyProperty").Elements("CompanyProperty").ToList();
                xml = JsonConvert.SerializeObject(XMLList1);
            }
            else
            {
                xml = "";
            }
            return xml;
        }


        /// <summary>
        /// 修改公司,同时修改异常指标
        /// </summary>
        /// <param name="info"></param>
        /// <param name="TargetIDList">没有选中的指标List</param>
        /// <param name="SelTargetIDList">选中的指标List</param>
        /// <returns></returns>
        [LibAction]
        public int UpdateCompany(string info, string TargetIDList, string SelTargetIDList)
        {
            int result = 0;

            C_Company detail = JsonHelper.Deserialize<C_Company>(info);
            //取出该公司异常type=1的指标名称
            List<ExceptionTargetVModel> list = C_ExceptiontargetOperator.Instance.GetExctargetListByComList(detail.ID).ToList();

            C_CompanyOperator.Instance.UpdateCompany(detail);



            if (TargetIDList != null)
            {
                //取出页面传来的所有没有勾中的指标
                string[] arr = TargetIDList.Split('|');

                //获取页面选中的指标
                string[] selArr = SelTargetIDList.Split('|');


                //位选中的值中循环
                for (int i = 0; i < arr.Length; i++)
                {
                    //循环所有传过来的ID
                    int arrl = 0;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (arr[i] == list[j].TargetID.ToString())
                        {
                            arrl++;
                        }
                    }
                    if (arrl == 0)
                    {
                        //若arrl==0,说明该指标在原本的异常表里不存在,添加操作
                        C_ExceptionTarget model = new C_ExceptionTarget();
                        model.CompanyID = detail.ID;
                        model.TargetID = Guid.Parse(arr[i]);
                        model.ExceptionType = 2;
                        C_ExceptiontargetOperator.Instance.AddExceptiontarget(model);
                    }
                }

                //选中值里循环异常表的数据
                for (int i = 0; i < list.Count; i++)
                {
                    //int arrl = 0;
                    for (int j = 0; j < selArr.Length; j++)
                    {
                        if (list[i].TargetID.ToString() != selArr[j])
                        {
                            #region 对于存在异常指标里的数据

                            int flag = CompanyExceptionTargetEngine.CompanyExceptionTargetEngineService.GetCompanyExceptionTarget(detail);

                            if (flag == 1) //flag: 0 的时候表示正常的数据，不用像异常表里添加数据， 而 flag： 1的时候需要像异常表中添加数据
                            {
                                C_ExceptionTarget newModel = new C_ExceptionTarget();
                                newModel.CompanyID = detail.ID;
                                newModel.TargetID = Guid.Parse(selArr[j]);
                                newModel.ExceptionType = (int)ExceptionTargetType.HaveDeTailNONeedEvaluation; ;
                                C_ExceptiontargetOperator.Instance.AddExceptiontarget(newModel);
                            }
                            else if (flag == 0)
                            {
                                List<C_ExceptionTarget> ExcList = C_ExceptiontargetOperator.Instance.GetExceptiontargetList(detail.ID, selArr[j].ToGuid()).ToList();

                                if (ExcList != null && ExcList.Count() > 0)
                                {
                                    ExcList.ForEach(p =>
                                    {
                                        p.IsDeleted = true;
                                        C_ExceptiontargetOperator.Instance.UpdateExceptiontarget(p);
                                    });
                                }

                            }

                            #endregion
                        }
                    }
                }
            }
            else
            {
                //若TargetIDList为空 说明页面全部勾中

                //获取页面选中的指标
                string[] selArr = SelTargetIDList.Split('|');

                for (int j = 0; j < selArr.Length; j++)
                {

                    #region 对于存在异常指标里的数据

                    int flag = CompanyExceptionTargetEngine.CompanyExceptionTargetEngineService.GetCompanyExceptionTarget(detail);

                    if (flag == 1) //flag: 0 的时候表示正常的数据，不用像异常表里添加数据， 而 flag： 1的时候需要像异常表中添加数据
                    {
                        C_ExceptionTarget newModel = new C_ExceptionTarget();
                        newModel.CompanyID = detail.ID;
                        newModel.TargetID = Guid.Parse(selArr[j]);
                        newModel.ExceptionType = (int)ExceptionTargetType.HaveDeTailNONeedEvaluation; ;
                        C_ExceptiontargetOperator.Instance.AddExceptiontarget(newModel);
                    }
                    else if (flag == 0)
                    {
                        List<C_ExceptionTarget> ExcList = C_ExceptiontargetOperator.Instance.GetExceptiontargetList(detail.ID, selArr[j].ToGuid()).ToList();

                        if (ExcList != null && ExcList.Count() > 0)
                        {
                            ExcList.ForEach(p =>
                            {
                                p.IsDeleted = true;
                                C_ExceptiontargetOperator.Instance.UpdateExceptiontarget(p);
                            });
                        }

                    }

                    #endregion
                }
            }

            return result;
        }
        [LibAction]
        public int UpdateCompanyByList(string info)
        {
            int result = 0;
            List<C_Company> detailist = JsonHelper.Deserialize<List<C_Company>>(info);
            foreach (var detail in detailist)
            {
                C_CompanyOperator.Instance.UpdateCompany(detail);
            }
            return result;
        }



        /// <summary>
        /// 添加公司，同时修改异常指标
        /// </summary>
        /// <param name="info">公司表的信息，同时含有没有选中的指标</param>
        /// <param name="SysID"></param>
        /// <param name="SelTargetIDList"> 选中的指标</param>
        /// <returns></returns>
        [LibAction]
        public int AddCompany(string info, string SysID, string SelTargetIDList)
        {
            int a = 0;
            string[] brr;
            C_System model = new C_System();
            model = StaticResource.Instance[Guid.Parse(SysID), DateTime.Now];
            List<string> TargetList = new List<string>();
            C_Company detail = new C_Company();
            string[] arr = info.Split('|');
            for (int i = 0; i < arr.Length; i++)
            {
                brr = arr[i].Split(':');
                #region//解析info
                if (brr[0] == "CompanyProperty1")
                {
                    detail.CompanyProperty1 = brr[1];
                }
                else if (brr[0] == "CompanyProperty2")
                {
                    detail.CompanyProperty2 = brr[1];
                }
                else if (brr[0] == "CompanyProperty3")
                {
                    detail.CompanyProperty3 = brr[1];
                }
                else if (brr[0] == "CompanyProperty4")
                {
                    detail.CompanyProperty4 = brr[1];
                }
                else if (brr[0] == "CompanyProperty5")
                {
                    detail.CompanyProperty5 = brr[1];
                }
                else if (brr[0] == "CompanyProperty6")
                {
                    detail.CompanyProperty6 = brr[1];
                }
                else if (brr[0] == "CompanyProperty7")
                {
                    detail.CompanyProperty7 = brr[1];

                }
                else if (brr[0] == "CompanyProperty8")
                {
                    detail.CompanyProperty8 = brr[1];

                }
                else if (brr[0] == "CompanyProperty9")
                {
                    detail.CompanyProperty9 = brr[1];

                }
                else if (brr[0] == "Sequence")
                {
                    detail.Sequence = int.Parse(brr[1]);

                }
                else if (brr[0] == "CompanyName")
                {
                    detail.CompanyName = brr[1];

                }
                else if (brr[0] == "SystemID")
                {
                    detail.SystemID = Guid.Parse(brr[1]);

                }
                else if (brr[0] == "OpeningTime")
                {
                    detail.OpeningTime = DateTime.Parse(brr[1]);
                }
                else if (brr[0] == "ExTargetList")
                {
                    if (brr[1] != "")
                    {
                        TargetList = new List<string>(brr[1].Split(','));
                    }
                }

                #endregion
            }

            Guid Id = C_CompanyOperator.Instance.AddCompany(detail);

            #region  对筹备门店和尾盘做处理操作

            if (SelTargetIDList != null)
            {
                string[] selArr = SelTargetIDList.Split('|'); //选中的指标

                for (int j = 0; j < selArr.Length; j++)
                {
                    int flag = CompanyExceptionTargetEngine.CompanyExceptionTargetEngineService.GetCompanyExceptionTarget(detail);

                    if (flag == 1) //flag: 0 的时候表示正常的数据，不用像异常表里添加数据， 而 flag： 1的时候需要像异常表中添加数据
                    {
                        C_ExceptionTarget newModel = new C_ExceptionTarget();
                        newModel.CompanyID = Id;
                        newModel.TargetID = Guid.Parse(selArr[j]);
                        newModel.ExceptionType = (int)ExceptionTargetType.HaveDeTailNONeedEvaluation; ;
                        C_ExceptiontargetOperator.Instance.AddExceptiontarget(newModel);
                    }
                }
            }

            #endregion

            if (TargetList.Count > 0)
            {
                C_ExceptiontargetOperator.Instance.AddExceptiontargetList(TargetList, Id);
            }
            return a;
        }
        [LibAction]
        public C_Target GetTarget(string TargetID)
        {
            return C_TargetOperator.Instance.GetTarget(Guid.Parse(TargetID), DateTime.Now);
        }

        [LibAction]
        public List<List<C_Company>> GetCompanyLists(string SysID)
        {

            List<List<C_Company>> NewCompanyList = new List<List<C_Company>>();
            IList<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SysID), DateTime.Now).ToList();

            foreach (C_Target item in TargetList)
            {
                if (item.HaveDetail == true)
                {
                    NewCompanyList.Add(C_CompanyOperator.Instance.GetCompanyTargetList(item.ID, item.SystemID));
                }
                else
                {
                    List<XElement> listHaveNotDetailCompany = item.Configuration.Elements("HavenotDetail").Elements("Company").ToList();
                    List<C_Company> listCom = new List<C_Company>();
                    foreach (XElement xml in listHaveNotDetailCompany)
                    {
                        C_Company TempCompany = StaticResource.Instance.GetCompanyModel(xml.GetAttributeValue("CompanyID", "").ToGuid());
                        if (TempCompany != null)
                        {
                            listCom.Add(TempCompany);
                        }
                    }
                    NewCompanyList.Add(listCom);
                }

            }
            return NewCompanyList;
        }
        [LibAction]
        public List<C_Target> GetTargetList(string SysID)
        {
            List<C_Target> result = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SysID), DateTime.Now).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), result[i].TargetType);
            }
            return result;
        }


        [LibAction]
        public List<C_Target> GetVerTargetList(string SysID, string FinYear)
        {
            List<A_TargetPlanDetail> Detail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(Guid.Parse(SysID), int.Parse(FinYear)).ToList();
            DateTime time = DateTime.Now;
            if (Detail.Count > 0)
            {
                time = B_TargetplanOperator.Instance.GetTargetplan(Detail[0].TargetPlanID).CreateTime;
            }
            List<C_Target> result = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SysID), time).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), result[i].TargetType);
            }
            return result;
        }

        [LibAction]
        public List<C_Target> GetVerTargetListByTargetPlanID(string TargetPlanID)
        {
            B_TargetPlan tp = B_TargetplanOperator.Instance.GetTargetplan(TargetPlanID.ToGuid());
            DateTime time = tp.CreateTime;
            Guid SysID = tp.SystemID;
            List<C_Target> result = C_TargetOperator.Instance.GetTargetList(SysID, time).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), result[i].TargetType);
            }
            return result;
        }

        [LibAction]
        public List<C_ExceptionTarget> GetExetargetList(string CompanyID, string TargetID)
        {
            return C_ExceptiontargetOperator.Instance.GetExceptiontargetList(Guid.Parse(CompanyID), Guid.Parse(TargetID)).ToList();
        }

        /// <summary>
        /// 获取审批过后的当前数据  
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public List<C_Target> GetTargetListNew(string SystemID, int Year, int Month)
        {
            DateTime t = DateTime.Now;

            List<A_MonthlyReportDetail> AM = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(Guid.Parse(SystemID), Year, Month).ToList();
            if (AM.Count > 0)
            {
                B_MonthlyReport BM = B_MonthlyreportOperator.Instance.GetMonthlyreport(AM[0].MonthlyReportID);
                t = BM.CreateTime;
            }

            List<C_Target> Result = StaticResource.Instance.GetTargetList(Guid.Parse(SystemID), t).ToList();
            Result = Result.Where(p => p.NeedEvaluation == true).OrderBy(s => s.Sequence).ToList();
            return Result;
        }


        [LibAction]
        public List<TargetTypeEnum> GetTargetTypeEnum(string adj)
        {
            List<TargetTypeEnum> result = new List<TargetTypeEnum>();
            TargetTypeEnum model;

            if (adj == null)
            {
                model = new TargetTypeEnum();
                model.IfSelect = true;
                model.TargetType = 0;
                model.TargetTypeValue = "--请选择--";
                result.Add(model);
                for (int a = 1; a <= EnumUtil.GetItems(typeof(EnumTargetType)).Count; a++)
                {
                    model = new TargetTypeEnum();
                    model.IfSelect = false;
                    model.TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), a);
                    model.TargetType = a;
                    result.Add(model);
                }
            }
            else
            {
                for (int a = 1; a <= EnumUtil.GetItems(typeof(EnumTargetType)).Count; a++)
                {
                    model = new TargetTypeEnum();
                    model.IfSelect = false;
                    if (a == int.Parse(adj))
                    {
                        model.IfSelect = true;
                    }
                    model.TargetTypeValue = EnumHelper.GetEnumDescription(typeof(EnumTargetType), a);
                    model.TargetType = a;
                    result.Add(model);
                }
            }
            return result;
        }
        [LibAction]
        public Guid AddTarget(string info, string SystemID)
        {
            string[] brr;
            string[] arr = info.Split(',');
            C_Target model = new C_Target();
            for (int i = 0; i < arr.Length; i++)
            {
                brr = arr[i].Split(':');
                if (brr[0] == "TargetName")
                {
                    model.TargetName = brr[1];
                }
                else if (brr[0] == "HaveDetail")
                {
                    model.HaveDetail = bool.Parse(brr[1]);
                }
                else if (brr[0] == "NeedEvaluation")
                {
                    model.NeedEvaluation = bool.Parse(brr[1]);
                }
                else if (brr[0] == "NeedReport")
                {
                    model.NeedReport = bool.Parse(brr[1]);
                }
                else if (brr[0] == "TargetType")
                {
                    model.TargetType = int.Parse(brr[1]);
                }
                else if (brr[0] == "Unit")
                {
                    if (brr[1] == "null")
                    {
                        model.Unit = null;
                    }
                    else
                    {
                        model.Unit = brr[1];
                    }

                }
                else if (brr[0] == "Sequence")
                {
                    model.Sequence = int.Parse(brr[1]);
                }
                else if (brr[0] == "BaseLine")
                {
                    if (brr[1] == "null")
                    {
                        model.BaseLine = DateTime.MinValue;
                    }
                    else
                    {
                        model.BaseLine = DateTime.Parse(brr[1]);
                    }
                }
            }
            model.IsDeleted = false;
            model.SystemID = Guid.Parse(SystemID);
            model.CreateTime = DateTime.Now;
            model.VersionStart = DateTime.Now;
            model.MeasureRate = null;
            model.VersionEnd = DateTime.Parse("9999-12-31 00:00:00.000");
            model.Configuration = XElement.Parse("<Root><SummaryTargetDisplay ShowKpi=\"False\" /></Root>");
            return C_TargetOperator.Instance.AddTarget(model);
        }

        [LibAction]
        public Guid UpdateTarget(string info, string ID)
        {
            C_Target result = C_TargetOperator.Instance.GetTarget(Guid.Parse(ID), DateTime.Now);
            DateTime time = result.VersionStart;
            result.VersionEnd = DateTime.Now;
            C_TargetOperator.Instance.UpdateTargetVerSion(result, time);


            C_Target Newresult = JsonHelper.Deserialize<C_Target>(info);
            C_Target AddModel = result;
            if (Newresult.BaseLine.ToString() == "0001/1/1 0:00:00")
            {
                Newresult.BaseLine = DateTime.MinValue;
            }
            AddModel.TargetName = Newresult.TargetName;
            AddModel.HaveDetail = Newresult.HaveDetail;
            AddModel.NeedEvaluation = Newresult.NeedEvaluation;
            AddModel.NeedReport = Newresult.NeedReport;
            AddModel.TargetType = Newresult.TargetType;
            AddModel.BaseLine = Newresult.BaseLine;
            AddModel.Unit = Newresult.Unit;
            AddModel.Sequence = Newresult.Sequence;

            AddModel.VersionStart = DateTime.Now;
            AddModel.VersionEnd = DateTime.Parse("9999-12-31 00:00:00.000");

            return C_TargetOperator.Instance.AddTarget(AddModel);

        }
        [LibAction]
        public Guid DeleteTarget(string ID)
        {
            C_Target result = C_TargetOperator.Instance.GetTarget(Guid.Parse(ID), DateTime.Now);
            result.IsDeleted = true;

            C_TargetOperator.Instance.UpdateTargetVerSion(result, result.VersionStart);

            return Guid.NewGuid();
        }

        [LibAction]
        public List<ExceptionTargetUpdateList> GetExceptionTarget(string SystemID)
        {
            List<ExceptionTargetUpdateList> result = new List<ExceptionTargetUpdateList>();
            List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SystemID), DateTime.Now).ToList();
            //获取异常指标,没有实际意义仅在更改完异常指标后,更新静态数据源
            List<C_ExceptionTarget> Except = StaticResource.Instance.GetExceptionTargetList();
            for (int i = 0; i < TargetList.Count; i++)
            {
                ExceptionTargetUpdateList model = new ExceptionTargetUpdateList();
                model.TargetID = TargetList[i].ID;
                model.ExcepListA = C_ExceptiontargetOperator.Instance.GetExctargetListByTarList(TargetList[i].ID, (int)ExceptionTargetType.HaveDeTailNONeedEvaluation).ToList();
                model.ExcepListB = C_ExceptiontargetOperator.Instance.GetExctargetListByTarList(TargetList[i].ID, (int)ExceptionTargetType.HaveDetailNONeedReport).ToList();
                model.ExcepListC = C_ExceptiontargetOperator.Instance.GetLastExctargetListByTarList(TargetList[i].ID, Guid.Parse(SystemID)).ToList();
                result.Add(model);
            }
            return result;
        }
        [LibAction]
        public Guid ChangeExceptionTarget(string OldType, string NewType, string ID, string TargetID)
        {
            Guid result;
            if (OldType == "3")//当OldType=3时,证明该数据在异常表里不存在,是从普通公司表插入异常表,其ID为公司ID
            {
                C_ExceptionTarget Model = new C_ExceptionTarget();
                Model.CompanyID = Guid.Parse(ID);
                Model.TargetID = Guid.Parse(TargetID);
                Model.ExceptionType = int.Parse(NewType);
                result = C_ExceptiontargetOperator.Instance.AddExceptiontarget(Model);


            }
            else
            {
                if (NewType == "3")//当NewType=3时,则该数据在需要在异常表里删除
                {
                    //result = C_ExceptiontargetOperator.Instance.RemoveExceptiontarget(Guid.Parse(ID));
                    C_ExceptionTarget model = C_ExceptiontargetOperator.Instance.GetExceptiontarget(Guid.Parse(ID));
                    result = C_ExceptiontargetOperator.Instance.RemoveExceptiontarget(Guid.Parse(ID));

                    //C_Company Commodel = C_CompanyOperator.Instance.GetCompany(model.CompanyID);
                    //C_CompanyOperator.Instance.RemoveCompany(model.CompanyID);
                    //C_Company NewModel = Commodel;
                    //NewModel.VersionStart = DateTime.Now;
                    //NewModel.VersionEnd = Commodel.VersionStart;
                    //C_CompanyOperator.Instance.AddCompany(NewModel);
                }
                else
                {
                    C_ExceptionTarget model = C_ExceptiontargetOperator.Instance.GetExceptiontarget(Guid.Parse(ID));
                    model.ExceptionType = int.Parse(NewType);
                    result = C_ExceptiontargetOperator.Instance.UpdateExceptiontarget(model);
                }
            }
            return result;
        }
        [LibAction]
        public List<Guid> SaveExceptionReplace(string IDlist, string Type, string SelectType, string TargetID)
        {
            List<Guid> result = new List<Guid>();
            string[] arr = IDlist.Split('|');
            if (Type == "C")//数据本身是不存在于异常表中
            {
                if (SelectType == "A")//在异常表中添加数据,类型给1
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        C_Company Company = C_CompanyOperator.Instance.GetCompany(Guid.Parse(arr[i]));
                        C_ExceptionTarget model = new C_ExceptionTarget();
                        model.CompanyID = Company.ID;
                        model.TargetID = Guid.Parse(TargetID);
                        model.ExceptionType = (int)ExceptionTargetType.HaveDeTailNONeedEvaluation;
                        Guid ID = C_ExceptiontargetOperator.Instance.AddExceptiontarget(model);
                        result.Add(ID);
                    }
                }
                else
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        C_Company Company = C_CompanyOperator.Instance.GetCompany(Guid.Parse(arr[i]));
                        C_ExceptionTarget model = new C_ExceptionTarget();
                        model.CompanyID = Company.ID;
                        model.TargetID = Guid.Parse(TargetID);
                        model.ExceptionType = (int)ExceptionTargetType.HaveDetailNONeedReport;
                        Guid ID = C_ExceptiontargetOperator.Instance.AddExceptiontarget(model);
                        result.Add(ID);
                    }
                }
            }
            else//数据本身存在于异常表中
            {
                if (SelectType == "C")//用户选择了上报审批,即从异常表中删除
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        //取到异常数据
                        //C_ExceptionTarget model = C_ExceptiontargetOperator.Instance.GetExceptiontarget(Guid.Parse(arr[i]));
                        //取到该公司数据  加版本暂时不用
                        //C_Company Commodel = C_CompanyOperator.Instance.GetCompany(model.CompanyID);
                        //C_CompanyOperator.Instance.RemoveCompany(model.CompanyID);
                        //C_Company NewModel = Commodel;
                        //NewModel.VersionStart = DateTime.Now;
                        //NewModel.VersionEnd = Commodel.VersionStart;
                        //C_CompanyOperator.Instance.AddCompany(NewModel);
                        Guid ID = C_ExceptiontargetOperator.Instance.RemoveExceptiontarget(Guid.Parse(arr[i]));
                        result.Add(ID);
                    }
                }
                else
                {
                    if (SelectType == "B")
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            C_ExceptionTarget model = C_ExceptiontargetOperator.Instance.GetExceptiontarget(Guid.Parse(arr[i]));
                            model.ExceptionType = EnumHelper.GetEnumValue(typeof(ExceptionTargetType), "不上报不考核");
                            Guid ID = C_ExceptiontargetOperator.Instance.UpdateExceptiontarget(model);
                            result.Add(ID);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            C_ExceptionTarget model = C_ExceptiontargetOperator.Instance.GetExceptiontarget(Guid.Parse(arr[i]));
                            model.ExceptionType = EnumHelper.GetEnumValue(typeof(ExceptionTargetType), "上报不考核");
                            Guid ID = C_ExceptiontargetOperator.Instance.UpdateExceptiontarget(model);
                            result.Add(ID);
                        }
                    }
                }
            }
            return result;
        }


        [LibAction]
        public List<ExceptionReplaceList> GetExceptionReplace(string SystemID)
        {
            List<ExceptionReplaceList> result = new List<ExceptionReplaceList>();
            List<C_Target> TargetList = C_TargetOperator.Instance.GetTargetList(Guid.Parse(SystemID), DateTime.Now).ToList();
            for (int i = 0; i < TargetList.Count; i++)
            {
                ExceptionReplaceList model = new ExceptionReplaceList();
                model.TargetID = TargetList[i].ID;
                model.ExcepListA = C_ExceptiontargetOperator.Instance.GetExctargetListByTarList(TargetList[i].ID, EnumHelper.GetEnumValue(typeof(ExceptionTargetType), "全年不可比门店")).ToList();

                model.ExcepListB = C_ExceptiontargetOperator.Instance.GetNoExceptionReplaceList(TargetList[i].ID, Guid.Parse(SystemID)).ToList();
                result.Add(model);
            }
            return result;
        }
        [LibAction]
        public Guid SaveCompareException(string SelectType, string ID, string TargetID)
        {
            Guid result;
            if (SelectType == "A")//本身为不可比 变为可比,从异常表中删除即可
            {
                result = C_ExceptiontargetOperator.Instance.RemoveExceptiontarget(Guid.Parse(ID));
            }
            else//本身为可比,变为不可比,在异常表中添加
            {
                C_ExceptionTarget Model = new C_ExceptionTarget();
                Model.CompanyID = Guid.Parse(ID);
                Model.TargetID = Guid.Parse(TargetID);
                Model.ExceptionType = EnumHelper.GetEnumValue(typeof(ExceptionTargetType), "全年不可比门店");
                result = C_ExceptiontargetOperator.Instance.AddExceptiontarget(Model);
            }
            return result;
        }
        [LibAction]
        public List<Guid> SaveCompareExceptionList(string IDlist, string SelectType, string TargetID)
        {
            List<Guid> result = new List<Guid>();
            string[] arr = IDlist.Split('|');
            if (SelectType == "B")//从可比变为不可比   在异常表中添加
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    C_ExceptionTarget model = new C_ExceptionTarget();
                    model.CompanyID = Guid.Parse(arr[i]);
                    model.TargetID = Guid.Parse(TargetID);
                    model.ExceptionType = EnumHelper.GetEnumValue(typeof(ExceptionTargetType), "全年不可比门店");
                    Guid ID = C_ExceptiontargetOperator.Instance.AddExceptiontarget(model);
                    result.Add(ID);
                }
            }
            else//从不可比变为可比  在异常表中删除
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    Guid ID = C_ExceptiontargetOperator.Instance.RemoveExceptiontarget(Guid.Parse(arr[i]));
                    result.Add(ID);
                }
            }
            return result;

        }
        [LibAction]
        public List<ContrastMisstargetList> GetContrastMisstarget(string FinYear, string FinMonth, string SystemID, bool IsPro)
        {
            List<ContrastMisstargetList> returnresult = A_MonthlyreportdetailOperator.Instance.GetContrastMisstarget(int.Parse(FinYear), int.Parse(FinMonth), SystemID, IsPro);

            return returnresult;
        }



        [LibAction]
        public List<C_System> GetSystemSeq()
        {

            List<C_System> result = C_SystemOperator.Instance.GetSystemListBySeq().ToList();
            return result;
        }

        [LibAction]
        public List<ContrastDetailList> GetContrastDetail(int FinYear, int FinMonth, bool IsPro)
        {

            List<ContrastDetailList> result = A_MonthlyreportdetailOperator.Instance.GetContrastDetail(FinYear, FinMonth, IsPro);
            return result;
        }

        [LibAction]
        public MissTargetEvaluationModel GetContrastRemark(string SystemID, string TargetID, string FinMonth, string FinYear, bool IsPro)
        {
            MissTargetEvaluationModel result = new MissTargetEvaluationModel();

            C_System sys = C_SystemOperator.Instance.GetSystem(Guid.Parse(SystemID));
            if (sys.GroupType != "ProSystem")
            {
                result.SystemName = sys.SystemName;
            }
            else
            {
                result.SystemName = "境内项目";
            }
            R_MissTargetEvaluationScope model = new R_MissTargetEvaluationScope();
            Guid TarID = Guid.Empty;
            if (Guid.Parse(TargetID) == Guid.Empty)
            {
                TarID = Guid.Parse("99999999-9999-9999-9999-999999999999");
                result.TargetName = "门店数量";
            }
            else
            {
                TarID = Guid.Parse(TargetID);
                result.TargetName = C_TargetOperator.Instance.GetTarget(Guid.Parse(TargetID)).TargetName;
            }
            Guid SysID = Guid.Parse(SystemID);

            string m = "ContrastRemark";
            if (IsPro == false)
            {
                m = "AContrastRemark";
            }

            model = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(SysID, TarID, int.Parse(FinYear), int.Parse(FinMonth), m);
            if (model == null)
            {
                result.EvaluationRemark = null;
            }
            else
            {
                result.EvaluationRemark = model.ContrastRemark;
            }
            return result;
        }
        [LibAction]
        public Guid UpdateContrastRemark(string SystemID, string TargetID, string FinMonth, string FinYear, string Remark, bool IsPro)
        {

            string m = "ContrastRemark";
            if (IsPro == false)
            {
                m = "AContrastRemark";
            }
            Guid Result = Guid.Empty;
            R_MissTargetEvaluationScope model = new R_MissTargetEvaluationScope();
            Guid TarID = Guid.Empty;
            Guid SysID = Guid.Parse(SystemID);
            if (Guid.Parse(TargetID) == Guid.Empty)
            {
                TarID = Guid.Parse("99999999-9999-9999-9999-999999999999");
            }
            else
            {
                TarID = Guid.Parse(TargetID);
            }
            model = R_MissTargetEvaluationScopeOperator.Instance.GetEvaluationDetailByType(SysID, TarID, int.Parse(FinYear), int.Parse(FinMonth), m);
            if (model != null)
            {
                model.ContrastRemark = Remark;
                Result = R_MissTargetEvaluationScopeOperator.Instance.UpdateMissTargetEvaluationScope(model);
            }
            else
            {
                model = new R_MissTargetEvaluationScope();
                model.SystemID = SysID;
                model.TargetID = TarID;
                model.FinMonth = int.Parse(FinMonth);
                model.FinYear = int.Parse(FinYear);
                model.EvaluationType = m;
                model.ContrastRemark = Remark;
                Result = R_MissTargetEvaluationScopeOperator.Instance.AddMissTargetEvaluationScope(model);
            }
            return Result;
        }

    }
}
