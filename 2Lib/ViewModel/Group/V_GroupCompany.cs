using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LJTH.Lib.Data.AppBase;
using Lib.Xml;
using Lib.Core;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class V_GroupCompany : BaseModel
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid CompanyID { get; set; } 
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 系统ID
        /// </summary>
        public Guid SystemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<MonthlyReportDetail> ListGroupTargetDetail { get; set; }
    }

    public class GroupDictionaryVmodel
    {
        public GroupDictionaryVmodel()
        { 
        
        }
        public GroupDictionaryVmodel(Guid ID, string Names, List<MonthlyReportDetail> Value, object Values, string Marks, string HtmlTemplate)
        {
            this.ID = ID;
            this.Name = Names;
            this.Value = Value;
            this.ObjValue = Values;
            this.Mark = Marks;
            this.HtmlTemplate = HtmlTemplate;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }
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
        /// 节点的值
        /// </summary>
        public List<MonthlyReportDetail> Value { get; set; }

        /// <summary>
        /// 表头,Tmpl模板,Excel模板(以逗号隔开)
        /// </summary>
        public string HtmlTemplate { get; set; }
    }

    /// <summary>
    /// 集团总部指标
    /// </summary>
    public class V_GroupTargetXElement
    {
        public V_GroupTargetXElement(XElement vGroupXElementTarget)
        {
            #region 获取当前指标的数据
            TargetName = vGroupXElementTarget.GetAttributeValue("TargetName", "");
            TargetValue = vGroupXElementTarget.GetAttributeValue("TargetValue", "");
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(TargetValue), "TargetValue is NULL in the configuration setting");
            Expression = vGroupXElementTarget.GetAttributeValue("Expression", "");
            if (vGroupXElementTarget.GetAttributeValue("HaveDetail", "").ToLower() == "true")
            {
                HaveDetail = true;
            }
            else
            { 
                HaveDetail = false; 
            }
            if (vGroupXElementTarget.GetAttributeValue("GroupDetail", "").ToLower() == "true")
            {
                GroupDetail = true;
            }
            else
            {
                GroupDetail = false;
            }
            #endregion
        }
        public string TargetName { get; set; } //指标名称
        public string TargetValue { get; set; }//指标ID
        public string Expression { get; set; }//表达式
        public bool HaveDetail { get; set; }//是否是明细指标
        public bool GroupDetail { get; set; }//指标是否存在分组数据（即明细指标数据）
    }
}
