using Lib.Data;
using LJTH.Lib.Data.AppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.OAMessage
{
    [Serializable]
    public class OAMessageEntityFilter : PagenationDataFilter, IDataFilter
    {

        
        [FilterFieldAttribute("FlowID", "IN")]
        public List<string> BusinessIDS { get; set; }
        [FilterFieldAttribute("FlowTitle", "like")]
        public string TaskTitle { get; set; }

        [FilterFieldAttribute("CreateFlowTime", ">=")]
        public DateTime CreatorTimeStart { get; set; }

        [FilterFieldAttribute("CreateFlowUser", "=")]
        public string EmployeeLoginName { get; set; }
    }
}
