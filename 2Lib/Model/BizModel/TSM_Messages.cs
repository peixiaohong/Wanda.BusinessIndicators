using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using LJTH.Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.Model
{

    public class TSM_Messages : BaseModel
    {

        [ORFieldMapping("MessageID", PrimaryKey = true)]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.All, DefaultExpression = "NEWID()")]
        public override Guid ID { get; set; }

        [ORFieldMapping("Target")]
        public string Target { get; set; }

        [ORFieldMapping("Title")]
        public string Title { get; set; }

        [ORFieldMapping("Content")]
        public string Content { get; set; }

        [ORFieldMapping("Priority")]
        public int Priority { get; set; }

        [ORFieldMapping("MessageType")]
        public int MessageType { get; set; }

        [ORFieldMapping("TargetTime")]
        public DateTime TargetTime { get; set; }//调度时间

        [ORFieldMapping("SendTime")]
        public DateTime SendTime { get; set; }

        [ORFieldMapping("Status")]
        public int Status { get; set; }

        [ORFieldMapping("TryTimes")]
        public int TryTimes { get; set; }

        [ORFieldMapping("ErrorInfo")]
        public string ErrorInfo { get; set; }

        [NoMapping]
        public override string CreatorName { get; set; }
        [NoMapping]
        public override string ModifierName { get; set; }
        [NoMapping]
        public override DateTime ModifyTime { get; set; }
        [NoMapping]
        public override bool IsDeleted { get; set; }

    }
}
