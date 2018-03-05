using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
     [ORTableMapping("dbo.B_Subscription")]
    public class B_Subscription : BaseModel
    {
         [ORFieldMapping("SystemID")]
         public Guid SystemID { get; set; }

         [ORFieldMapping("FinYear")]
         public int FinYear { get; set; }

         [ORFieldMapping("FinMonth")]
         public int FinMonth { get; set; }

         /// <summary>
         /// 操作
         /// 为true时是订阅
         /// </summary>
         [ORFieldMapping("Operating")]
         public bool Operating { get; set; }

    }
}
