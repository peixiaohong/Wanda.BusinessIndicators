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

    /// <summary>
    /// 未完成分组
    /// </summary>
    public class VGroup
    {
        public VGroup(XElement Group)
        {
            TargetIDs = new List<Guid>();
            CounterList = new List<VCounter>();
            TargetName = Group.GetAttributeValue("TargetName", "");
            string TargetValue = Group.GetAttributeValue("TargetValue", "");
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(TargetValue), "TargetValue is NULL in the configuration setting");
            foreach (string id in TargetValue.Split(',').ToList())
            {
                TargetIDs.Add(id.ToGuid());
            }
            string senquence = Group.GetAttributeValue("Senquence", "");
            int t = 10000;
            int.TryParse(senquence, out t);
            Senquence = t;

            ExceptionHelper.TrueThrow(Group.Elements("Counter") == null, string.Format("Group:{0} don't have Counter define", TargetName));
            List<XElement> Counters = Group.Elements("Counter").ToList();
            foreach (XElement Counter in Counters)
            {
                CounterList.Add(new VCounter(Counter));
            }
        }

        public string TargetName { get; set; }
        public List<Guid> TargetIDs { get; set; }
        public int Senquence { get; set; }

        /// <summary>
        /// 未完成
        /// </summary>
        public List<VCounter> CounterList { get; set; }



    }

    /// <summary>
    /// 补回情况具体明细
    /// </summary>
    public class VCounter
    {
        public VCounter()
        { 
        
        }
        public VCounter(XElement Counter)
        {
            SubCounterList = new List<VCounter>();
            MonthlyReportDetailList = new List<A_MonthlyReportDetail>();
            Expression = Counter.GetAttributeValue("Expression", "");
            TextExpression = Counter.GetAttributeValue("TextExpression", "");
            string senquence = Counter.GetAttributeValue("Senquence", "");
            int t = 10000;
            int.TryParse(senquence, out t);
            Senquence = t;
            Title = Counter.GetAttributeValue("Title", "");
            Display = Counter.GetAttributeValue("Display", "");
            HaveDetail = Counter.GetAttributeValue("HaveDetail", "");
            IsSummaryDetail = Counter.GetAttributeValue("IsSummaryDetail", "");
            if (Counter.Elements("Counter") != null)
            {
                List<XElement> SubConuters = Counter.Elements("Counter").ToList();
                foreach (XElement subCounter in SubConuters)
                {
                    SubCounterList.Add(new VCounter(subCounter));
                }
            }

            DetailMonthlyExpression = Counter.GetAttributeValue("DetailMonthlyExpression", "");//本月累计算式
            DetailExpression = Counter.GetAttributeValue("DetailExpression", "");//本年累计算式

        }
        public string Expression { get; set; }

        public string TextExpression { get; set; }

        public int Senquence { get; set; }
        public string Title { get; set; }

        public string DetailMonthlyExpression { get; set; }
        public string DetailExpression { get; set; }

        public string Display { get; set; }

        public string HaveDetail { get; set; }

        public string IsSummaryDetail { get; set; }
        /// <summary>
        /// 未完成分组
        /// </summary>
        public List<VCounter> SubCounterList { get; set; }

        public List<A_MonthlyReportDetail> MonthlyReportDetailList { get; set; }

    }



    /// <summary>
    /// 项目公司专用
    /// </summary>
    public class ProjectCounter
    {
        public ProjectCounter(XElement _ProjectCounter)
        {
            string senquence = _ProjectCounter.GetAttributeValue("Senquence", "");
            int t = 10000;
            int.TryParse(senquence, out t);
            Senquence = t;

            Title = _ProjectCounter.GetAttributeValue("Title", "");
            CompanyID = _ProjectCounter.GetAttributeValue("CompanyID", "");
            string isDetail = _ProjectCounter.GetAttributeValue("IsDetail", "");
            bool d = false;
            bool.TryParse(isDetail, out d);
            IsDetail = d;

            if (_ProjectCounter.Elements("ProjectCounter") != null)
            {
                List<XElement> SubConuters = _ProjectCounter.Elements("ProjectCounter").ToList();
                foreach (XElement subCounter in SubConuters)
                {
                    SubCounterList.Add(new ProjectCounter(subCounter));
                }
            }
        }

        public int Senquence { get; set; }
        public string Title { get; set; }
        public string CompanyID { get; set; }
        public bool IsDetail { get; set; }
        public List<ProjectCounter> SubCounterList { get; set; }

    }



    public class DictionaryVmodel
    {
        public DictionaryVmodel()
        { }

        public DictionaryVmodel(string Names, object Values)
        {
            this.Name = Names;
            this.ObjValue = Values;
        }

        public DictionaryVmodel(string Names, object Values, C_Company Company, string Marks)
        {
            this.Name = Names;
            this.ObjValue = Values;
            this._Company = Company;
            this.Mark = Marks;
        }

        public DictionaryVmodel(string Names, object Values, string Marks)
        {
            this.Name = Names;
            this.ObjValue = Values;

            this.Mark = Marks;
        }

        public DictionaryVmodel(string Names, object Values, string Marks, string HtmlTemplate)
        {
            this.Name = Names;
            this.ObjValue = Values;
            this.Mark = Marks;
            this.HtmlTemplate = HtmlTemplate;
        }

        public DictionaryVmodel(string Names, object Values, string Marks, string value, int rowSpanCount)
        {
            this.Name = Names;
            this.ObjValue = Values;
            this.Mark = Marks;
            this.Value = value;
            this.RowSpanCount = rowSpanCount;
        }
        public DictionaryVmodel(string Names, object Values, string Marks, string htmlTemplate, int rowSpanCount,int senquence)
        {
            this.Name = Names;
            this.ObjValue = Values;
            this.Mark = Marks;
            this.HtmlTemplate = htmlTemplate;
            this.RowSpanCount = rowSpanCount;
            this.Senquence = senquence;
        }


        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public Object ObjValue { get; set; }

        /// <summary>
        /// 值1
        /// </summary>
        public C_Company _Company { get; set; }

        //分组的ID
        public String GuoupID { get; set; }
        /// <summary>
        /// 节点的值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 通行总数
        /// </summary>
        public int RowSpanCount { get; set; }

        /// <summary>
        /// 分组指标个数
        /// </summary>
        public int TargetGroupCount { get; set; }


        public string SystemName { get; set; }

        /// <summary>
        /// 表头,Tmpl模板,Excel模板(以逗号隔开)
        /// </summary>
        public string HtmlTemplate { get; set; }


        //明细项小计字段
        public B_MonthlyReportDetail BMonthReportDetail { get; set; }
        //是否混合指标
        public bool IsBlendTarget { get; set; }
        //排序
        public int Senquence { get; set; }
        //是否包含区域
        public bool IsHaveArea { get; set; }
    }

    public class TargetMonthlyReportDetail
    {
        public Guid SystemID { get; set; }
        public int FinYear { get; set; }
        public int FinMonth { get; set; }
        public Guid TargetID { get; set; }
        public Guid CompanyID { get; set; }
        public Guid TargetPlanID { get; set; }
        public decimal OPlanAmmount { get; set; }
        public decimal OActualAmmount { get; set; }
        public decimal OActualRate { get; set; }
        public string ODisplayRate { get; set; }
        public decimal NPlanAmmount { get; set; }
        public decimal NActualAmmount { get; set; }
        public decimal NActualRate { get; set; }
        public string NDisplayRate { get; set; }
        public decimal OAccumulativePlanAmmount { get; set; }
        public decimal OAccumulativeActualAmmount { get; set; }
        public decimal OAccumulativeActualRate { get; set; }
        public string OAcccumulativeDisplayRate { get; set; }
        public decimal NAccumulativePlanAmmount { get; set; }
        public decimal NAccumulativeActualAmmount { get; set; }
        public decimal NAccumulativeActualRate { get; set; }
        public string NAccumulativeDisplayRate { get; set; }
        public bool IsMissTarget { get; set; }
        public int Counter { get; set; }
        public DateTime FirstMissTargetDate { get; set; }
        public DateTime PromissDate { get; set; }
        public DateTime CommitDate { get; set; }
        public string MIssTargetReason { get; set; }
        public string MIssTargetDescription { get; set; }
        public int ReturnType { get; set; }

        public C_Company Company { get; set; }
        public int CompanySequence { get; set; }
    }



    public class MissTargetDataSource
    {
        /// <summary>
        /// 当月所有数据
        /// </summary>
       public List<MonthlyReportDetail> MissTargetDataSource1 { get; set; }

        /// <summary>
        /// 上月所有数据
        /// </summary>
       public List<MonthlyReportDetail> MissTargetDataSource2 { get; set; }

        /// <summary>
        /// 上月双指标 + 补回
        /// </summary>
       public List<MonthlyReportDetail> MissTargetDataSource3 { get; set; }

        /// <summary>
        /// 上月单指标 + 补回
        /// </summary>
       public List<MonthlyReportDetail> MissTargetDataSource4 { get; set; }

        /// <summary>
        /// 当月的筛选后数据
        /// </summary>
       public List<MonthlyReportDetail> MissTargetDataSource5 { get; set; }
    }


}
