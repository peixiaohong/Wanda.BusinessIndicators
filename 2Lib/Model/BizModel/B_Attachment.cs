using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
 
    /// <summary>
    /// This object represents the properties and methods of a Attachments.
    /// </summary>
    [ORTableMapping("dbo.B_Attachments")]
    [Serializable]
    public class B_Attachment : BaseModel
    {

        #region Public Properties



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


        #endregion


    }

}
