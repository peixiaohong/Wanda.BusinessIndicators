using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
    [Serializable]
    public class AttachmentFilter : PagenationDataFilter
    {

        [FilterFieldAttribute("BusinessID")]
        public string BusinessID { get; set; }

        [FilterFieldAttribute("BusinessType")]
        public string BusinessType { get; set; }
    }


    [Serializable]
    public class DocumentAttachments : PagenationDataFilter
    {

        [FilterFieldAttribute("BusinessID")]
        public string BusinessID { get; set; }

        [FilterFieldAttribute("BusinessType")]
        public string BusinessType { get; set; }

        [FilterFieldAttribute("SystemID")]
        public string SystemID { get; set; }

    }


}
