using Lib.Data;
using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Filter
{
    public class WorkFlowFilter : PagenationDataFilter
    {
        [FilterFieldAttribute("UserID")]
        public int UserID { get; set; }
        [FilterFieldAttribute("InstanceName", "like")]
        public string Title { get; set; }
        [FilterFieldAttribute("ProcessCode")]
        public string ProcessCode { get; set; }
        public string ProcessCodeString { get; set; }
        [FilterFieldAttribute("UserName")]
        public string CreatorName { get; set; }
        [FilterField("UserLoginName", "like")]
        public string UserLoginName { get; set; }
        [FilterFieldAttribute("Createtime", ">=")]
        public string CreateTimeStart { get; set; }
        [FilterFieldAttribute("Createtime", "<")]
        public string CreateTimeEnd { get; set; }
        public string CreateTimeString { get; set; }
        [FilterFieldAttribute("Status","in")]
        public List<string> Status { get; set; }
        public string StatusString { get; set; }
        [FilterFieldAttribute("LastUpdatedTime", ">=")]
        public string LastUpdateTimeStart { get; set; }
        [FilterFieldAttribute("LastUpdatedTime", "<")]
        public string LastUpdateTimeEnd { get; set; }
        public string LastUpdateString { get; set; }


        [FilterFieldAttribute("ProcessCode", "not in")]
        public List<string> ExcludeProcessCode { get; set; }

        private Dictionary<string, FieldAndOperator> _filterDict = new Dictionary<string, FieldAndOperator>();
        public Dictionary<string, FieldAndOperator> FilterDict
        {
            get { return _filterDict; }
        }

    }
}
