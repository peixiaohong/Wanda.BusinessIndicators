using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LJTH.BusinessIndicators.Model;
using Lib.Xml;
using Lib.Core;
using LJTH.BusinessIndicators;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class CompleteTargetDetailViewModel
    {

    }
    /// <summary>
    /// 完成明细情况
    /// </summary>
    public class VTarget
    {
        public VTarget(XElement vtarget)
        {
            TargetIDs = new List<Guid>();
            CounterList = new List<VCounter>();
            TargetName = vtarget.GetAttributeValue("TargetName", "");
            string TargetValue = vtarget.GetAttributeValue("TargetValue", "");
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(TargetValue), "TargetValue is NULL in the configuration setting");
            foreach (string id in TargetValue.Split(',').ToList())
            {
                TargetIDs.Add(id.ToGuid());
            }
            string senquence = vtarget.GetAttributeValue("Senquence", "");
            int t = 10000;
            int.TryParse(senquence, out t);
            Senquence = t;
            
            ExceptionHelper.TrueThrow(vtarget.Elements("Counter") == null, string.Format("Group:{0} don't have Counter define", TargetName));
            List<XElement> Counters = vtarget.Elements("Counter").ToList();
            foreach (XElement Counter in Counters)
            {
                CounterList.Add(new VCounter(Counter));
            }
        }

        public string TargetName { get; set; }
        public List<Guid> TargetIDs { get; set; }
        public int Senquence { get; set; }
        public List<VCounter> CounterList { get; set; }
    }

    public class VCompany 
    {
        public string CompanyName { get; set; }

        public string CompanyID { get; set; }

        public VCompany(XElement xElementCP)
        {
            CompanyName = xElementCP.GetAttributeValue("CompanyName", "");
            CompanyID = xElementCP.GetAttributeValue("CompanyID", "");
        }
    
    }

    /// <summary>
    /// 门店属性
    /// </summary>
    public class VItemCompanyProperty
    {
        public VItemCompanyProperty(XElement xElementCP)
        {
            ItemCompanyPropertyName = xElementCP.GetAttributeValue("ItemPropertyName", "");
            ItemCompanyPropertyValue = xElementCP.GetAttributeValue("ItemPropertyValue", "");
            IsHideCounter=false;
            if(!string.IsNullOrEmpty(xElementCP.GetAttributeValue("ItemPropertyValue", "")))
            {
                if (xElementCP.GetAttributeValue("IsHideCounter", "").ToLower() == "true")
                {
                    IsHideCounter = true;
                }
            }
        }

        public string ItemCompanyPropertyName{get;set;}

        public string ItemCompanyPropertyValue { get; set; }

        public bool IsHideCounter { get; set; }
    }
    /// <summary>
    /// 公司属性
    /// </summary>
    public class VCompanyProperty
    {
        public VCompanyProperty(XElement xElementCP)
        {
            if (xElementCP != null)
            {
                CompanyPropertyName = xElementCP.GetAttributeValue("PropertyName", "");
                CompanyPropertyValue = xElementCP.GetAttributeValue("PropertyValue", "");
                CompanyPropertySenquence = xElementCP.GetAttributeValue("Senquence", "");
                CompanyPropertyDisplay = xElementCP.GetAttributeValue("ListDisplay", "");
                CompanyPropertySearchGroup = xElementCP.GetAttributeValue("SearchGroup", "");
                ColumnName = xElementCP.GetAttributeValue("ColumnName", "");
                if (xElementCP.Elements("ItemProperty") != null)
                {
                    List<XElement> listItemCP = xElementCP.Elements("ItemProperty").ToList();
                    VItemCompanyProperty vicp =null;
                    listCP = new List<VItemCompanyProperty>();
                    foreach (XElement xvicp in listItemCP)
                    {
                        vicp = new VItemCompanyProperty(xvicp);
                        listCP.Add(vicp);
                    }
                }
            }
        }

        public VCompanyProperty(){}

        public string CompanyPropertyName{get;set;}

        public string CompanyPropertyValue { get; set; }

        public string CompanyPropertySenquence { get; set; }

        public string CompanyPropertyDisplay { get; set; }

        public string CompanyPropertySearchGroup { get; set; }

        public string ColumnName { get; set; }

        public List<VItemCompanyProperty> listCP{get;set;}
    }


   

    /// <summary>
    /// 月度报表统计
    /// </summary>
    [Serializable]
    public class MonthReportDetailViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 指标明细
        /// </summary>
        public Guid TargetDetailID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 指标计划数
        /// </summary>
        public double NPlanAmmount { get; set; }

        /// <summary>
        /// 实际数
        /// </summary>
        public double NActualAmmount { get; set; }

        /// <summary>
        /// 实际完成率
        /// </summary>
        public string NActualRate { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        public double NDifference { get; set; }

        /// <summary>
        /// 年累计指标计划数
        /// </summary>
        public double NAccumulativePlanAmmount { get; set; }

        /// <summary>
        /// 年累计实际数
        /// </summary>
        public double NAccumulativeActualAmmount { get; set; }

        /// <summary>
        /// 年累计完成率
        /// </summary>
        public string NAccumulativeActualRate { get; set; }

        /// <summary>
        /// 年累计差额
        /// </summary>
        public double NAccumulativeDifference { get; set; }

        /// <summary>
        /// 警示灯
        /// </summary>
        public string WarningLamp { get; set; }
    }
}
