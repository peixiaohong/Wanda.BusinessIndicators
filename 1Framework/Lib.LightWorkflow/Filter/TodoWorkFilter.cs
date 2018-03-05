using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Filter
{
    [Serializable]
    public class TodoWorkFilter : PagenationDataFilter
    {

        [FilterFieldAttribute("UserID")]
        public int UserID { get; set; }
        [FilterFieldAttribute("InstanceName", "like")]
        public string Title { get; set; }
        [FilterFieldAttribute("ProcessCode")]
        public string TodoType { get; set; }
        public string TodoTypeString { get; set; }
        [FilterFieldAttribute("UserName")]
        public string UserName { get; set; }
        [FilterFieldAttribute("BizProcessID")]
        public int BizProcessID { get; set; }
        [FilterFieldAttribute("CreateUserLoginName", "like")]
        public string CreatorName { get; set; }
        [FilterFieldAttribute("LoginName", "like")]
        public string LoginName { get; set; }
        [FilterFieldAttribute("CreatedTime", ">=")]
        public string CreateTimeStart { get; set; }
        [FilterFieldAttribute("CreatedTime", "<")]
        public string CreateTimeEnd { get; set; }
        public string CreateTimeString { get; set; }
        [FilterFieldAttribute("CreateProcessTime", ">=")]
        public string CreateProcessTimeStart { get; set; }
        [FilterFieldAttribute("CreateProcessTime", "<")]
        public string CreateProcessTimeEnd { get; set; }
        public string CreateProcessTimeString { get; set; }
        [FilterFieldAttribute("IsDeleted", "=", DefaultV=-1)]
        public int IsDeleted { get; set; }

        private Dictionary<string, FieldAndOperator> _filterDict = new Dictionary<string, FieldAndOperator>();
        public Dictionary<string, FieldAndOperator> FilterDict
        {
            get { return _filterDict; }
        }
    }
}
