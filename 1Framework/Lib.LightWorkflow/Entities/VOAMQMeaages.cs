using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Entities
{
    [ORViewMapping(@"SELECT A.ID,A.Title,A.Status,A.CreateTime,A.Operatetime,A.Userid FROM Oamqmessages A", "VOAMQMeaages")]
    public class VOAMQMeaages : IBaseComposedModel
    {
        [ORFieldMapping("ID")]
        public int ID { get; set; }

        [ORFieldMapping("Title")]
        public string Title { get; set; }

        [ORFieldMapping("Status")]
        public int Status { get; set; }

        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("Operatetime")]
        public DateTime SendTime { get; set; }

        [ORFieldMapping("Userid")]
        public string Userid { get; set; }

        public string StatusString { get; set; }
    }
}
