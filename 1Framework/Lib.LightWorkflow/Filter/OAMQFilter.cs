using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Filter
{
    public class OAMQFilter : PagenationDataFilter
    {
        [FilterFieldAttribute("Title", "like")]
        public string Title { get; set; }

        [FilterFieldAttribute("Status", "in")]
        public List<int> Status { get; set; }

        [FilterFieldAttribute("CreateTime", ">=")]
        public string CreateTimeStart { get; set; }

        [FilterFieldAttribute("CreateTime", "<=")]
        public string CreateTimeEnd { get; set; }

        [FilterFieldAttribute("Userid")]
        public string Userid { get; set; }

        public string createTimeString { get; set; }
        public string StatusString { get; set; }
    }
}
