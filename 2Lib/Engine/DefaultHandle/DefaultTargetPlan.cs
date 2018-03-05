using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.ViewModel;
using System.Xml.Linq;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;
using Lib.Xml;
using Lib.Expression;
using System.Collections;
using System.Configuration;


namespace Wanda.BusinessIndicators.Engine
{
    public class DefaultTargetPlan : ITargetPlanInstance
    {

        public List<DictionaryVmodel> GetTargetPlanSource(Guid SystemID, int FinYear, Guid TargetPlanID, bool IsLatestVersion)
        {
            return GetTargetPlanDetail(SystemID, FinYear, TargetPlanID, IsLatestVersion);
        }

        public List<DictionaryVmodel> GetTargetPlanDetail(Guid SystemID, int FinYear, Guid TargetPlanID, bool IsLatestVersion)
        {
            List<DictionaryVmodel> lstDictionaryVmodel = new List<DictionaryVmodel>();

            List<C_Company> listCompany = StaticResource.Instance.CompanyList[SystemID];
            //DateTime t = DateTime.Now;
            //B_TargetPlan Bt = new B_TargetPlan();
            //if (TargetPlanID!=Guid.Empty)
            //{
            //    Bt = B_TargetplanOperator.Instance.GetTargetplan(TargetPlanID);
            //    if (Bt!=null)
            //    {
            //        t = Bt.CreateTime;
            //    }
            //}



            //List<C_Target> lstTarget = StaticResource.Instance.GetTargetList(SystemID, t).ToList();
            List<C_Target> lstTarget = StaticResource.Instance.GetTargetList(SystemID, DateTime.Now).ToList();
            C_System _cSystem = StaticResource.Instance[SystemID,DateTime.Now];
                
            object listBTargetPlanDetail = FormatData(SystemID, FinYear, TargetPlanID, IsLatestVersion);
            List<DictionaryVmodel> lstCompanyVmodel;
            foreach (C_Target _cTarget in lstTarget.OrderBy(p => p.Sequence))
            {
                lstCompanyVmodel = new List<DictionaryVmodel>();
                if (_cTarget.HaveDetail == true)
                {
                    foreach (C_Company company in listCompany)
                    {
                        if (ExceptionTargetEvaluationEngine.ExceptionTargetEvaluationService.Calculation(SystemID, _cTarget.ID, company.ID, ""))
                        {
                            continue;
                        }
                        else
                        {
                            if (listBTargetPlanDetail != null)
                            {
                                if (listBTargetPlanDetail is List<B_TargetPlanDetail>)
                                {
                                    List<B_TargetPlanDetail> list = ((List<B_TargetPlanDetail>)listBTargetPlanDetail).Where(p => p.TargetID == _cTarget.ID && p.CompanyID == company.ID).OrderBy(p => p.FinMonth).ToList();
                                    lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, list, company, list.Sum(p => p.Target).ToString()));
                                }
                                else
                                {
                                    List<A_TargetPlanDetail> list = ((List<A_TargetPlanDetail>)listBTargetPlanDetail).Where(p => p.TargetID == _cTarget.ID && p.CompanyID == company.ID).OrderBy(p => p.FinMonth).ToList();
                                    lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, list, company, list.Sum(p => p.Target).ToString()));
                                }
                            }
                            else
                            {
                                lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, null, company, "0"));
                            }

                        }
                    }
                }
                else
                {
                    List<XElement> listHaveNotDetailCompany = _cTarget.Configuration.Elements("HavenotDetail").Elements("Company").ToList();
                    List<C_Company> listCom = new List<C_Company>();
                    foreach (XElement xml in listHaveNotDetailCompany)
                    {
                        C_Company TempCompany = StaticResource.Instance.GetCompanyModel(xml.GetAttributeValue("CompanyID", "").ToGuid());
                        if (TempCompany != null)
                        {
                            listCom.Add(TempCompany);
                        }
                    }
                    if (listHaveNotDetailCompany.Count() > 0)
                    {
                        if (ConfigurationManager.AppSettings["HavenotDetailTargetCompanyName"] != null)
                        {

                        }
                    }
                    if (listCom.Count() > 0)
                    {
                        foreach (C_Company company in listCom)
                        {
                            if (listBTargetPlanDetail != null)
                            {
                                if (listBTargetPlanDetail is List<B_TargetPlanDetail>)
                                {
                                    List<B_TargetPlanDetail> list = ((List<B_TargetPlanDetail>)listBTargetPlanDetail).Where(p => p.TargetID == _cTarget.ID && p.CompanyID == company.ID).OrderBy(p => p.FinMonth).ToList();
                                    lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, list, company, list.Sum(p => p.Target).ToString()));
                                }
                                else
                                {
                                    List<A_TargetPlanDetail> list = ((List<A_TargetPlanDetail>)listBTargetPlanDetail).Where(p => p.TargetID == _cTarget.ID && p.CompanyID == company.ID).OrderBy(p => p.FinMonth).ToList();
                                    lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, list, company, list.Sum(p => p.Target).ToString()));
                                }
                            }
                            else
                            {
                                lstCompanyVmodel.Add(new DictionaryVmodel(company.CompanyName, null, company, "0"));
                            }
                        }
                    }

                }
                string strTmpl=string.Empty;
                if(_cTarget.Configuration.Elements("TargetPlanDetailReported").Elements("TableTemplate").ToList().Count > 0)
                {
                    strTmpl=GetTargetPlanDetailReportedTemplate(_cTarget.Configuration);
                }
                else
                {
                     strTmpl=GetTargetPlanDetailReportedTemplate(_cSystem.Configuration);
                }

                lstDictionaryVmodel.Add(new DictionaryVmodel(_cTarget.TargetName, lstCompanyVmodel,"", strTmpl));
            }

            return lstDictionaryVmodel;
        }

        public object FormatData(Guid SystemID, int FinYear, Guid TargetPlanID, bool IsLatestVersion)
        {
            object listBTargetPlanDetail = null;
            if (IsLatestVersion)
            {
                if (TargetPlanID == Guid.Empty)
                {
                    B_TargetPlan _BTargetPlan = B_TargetplanOperator.Instance.GetTargetPlanByProgressOrApproved(SystemID, FinYear);
                    if (_BTargetPlan != null)
                    {
                        listBTargetPlanDetail = B_TargetplandetailOperator.Instance.GetTargetplandetailList(_BTargetPlan.ID).ToList();
                    }
                }
                else
                {
                    listBTargetPlanDetail = B_TargetplandetailOperator.Instance.GetTargetplandetailList(TargetPlanID).ToList();
                }
            }
            else
            {

                if (TargetPlanID == Guid.Empty)
                {
                    A_TargetPlan _ATargetPlan = A_TargetplanOperator.Instance.GetTargetplanList(SystemID, FinYear).FirstOrDefault();
                    if (_ATargetPlan != null)
                    {
                        listBTargetPlanDetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(_ATargetPlan.ID).ToList();
                    }
                }
                else
                {
                    listBTargetPlanDetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(TargetPlanID).ToList();
                }
            }
            return listBTargetPlanDetail;
        }

        /// <summary>
        /// 获取表头名称和tmpl名称
        /// </summary>
        /// <param name="element">XML</param>
        /// <returns>表头、Tmpl、模板、Execl模板</returns>
        public string GetTargetPlanDetailReportedTemplate(XElement element)
        {
            string strValue = "";
            if (element != null)
            {
                if (element.Elements("TargetPlanDetailReported").Elements("TableTemplate").ToList().Count > 0)
                {
                    XElement xt = element.Elements("TargetPlanDetailReported").Elements("TableTemplate").ToList()[0];
                    strValue += xt.GetAttributeValue("TableHeadTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableDataTmplName", "") + ",";
                    strValue += xt.GetAttributeValue("TableExcelTemplateName", "") + ",";
                    strValue += xt.GetAttributeValue("IsYearTargetPlan", "") + ",";
                }
            }
            return strValue;
        }
    }
}
