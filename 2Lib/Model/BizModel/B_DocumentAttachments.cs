using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a B_DocumentAttachments.
    /// </summary>
    [ORTableMapping("dbo.B_DocumentAttachments")]
    [Serializable]
    public class B_DocumentAttachments : BaseModel
    {
        #region Public Properties


        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }


        [ORFieldMapping("BusinessType")]
        public string BusinessType { get; set; }



        [ORFieldMapping("BusinessID")]
        public Guid BusinessID { get; set; }



        [ORFieldMapping("FileName")]
        public string FileName { get; set; }




        [ORFieldMapping("Url")]
        public string Url { get; set; }



        [ORFieldMapping("Size")]
        public string Size { get; set; }


        [ORFieldMapping("Remark")]
        public string Remark { get; set; }

        [ORFieldMapping("ValueA")]
        public Guid ValueA { get; set; }

        [ORFieldMapping("ValueB")]
        public Guid ValueB { get; set; }

        [ORFieldMapping("ValueC")]
        public Guid ValueC { get; set; }

        [ORFieldMapping("ValueD")]
        public Guid ValueD { get; set; }

        [ORFieldMapping("ValueAString")]
        public string ValueAString { get; set; }

        [ORFieldMapping("ValueBString")]
        public string ValueBString { get; set; }

         [ORFieldMapping("ValueCString")]
        public string ValueCString { get; set; }

         [ORFieldMapping("ValueDString")]
        public string ValueDString { get; set; }

        [ORFieldMapping("FinYear")]
        public int FinYear { get; set; }


        #endregion

    }
}
