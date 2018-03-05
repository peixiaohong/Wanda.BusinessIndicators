using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a C_DocumentTree.
    /// </summary>
    [ORTableMapping("dbo.C_SystemTree")]
    [Serializable]
    public class C_SystemTree : BaseModel
    {
        [ORFieldMapping("ParentID")]
        public Guid ParentID { get; set; }

        [ORFieldMapping("TreeNodeName")]
        public string TreeNodeName { get; set; }

        [ORFieldMapping("Category")]
        public int Category { get; set; }

        [ORFieldMapping("Sequence")]
        public int Sequence { get; set; }

        [ORFieldMapping("ExcelGroup")]
        public string ExcelGroup { get; set; }

        [NoMapping]
        public bool open { get; set; }
        
    }
    public class SystemTree
    {
        public List<C_SystemTree> TreeList { get; set; }

        public bool IsLastTree { get; set; }//其自身是否为最后一级
        public bool IsChildLastTree { get; set; }//其子集是否为最后一级

    }
}
