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
    /// This object represents the properties and methods of a C_DocumentTree.
    /// </summary>
    [ORTableMapping("dbo.C_DocumentTree")]
    [Serializable]
    public class C_DocumentTree : BaseModel
    {
        [ORFieldMapping("ParentID")]
        public Guid ParentID { get; set; }

        [ORFieldMapping("SystemID")]
        public Guid SystemID { get; set; }

        [ORFieldMapping("TreeNodeName")]
        public string TreeNodeName { get; set; }


        [NoMapping]
        public bool open { get; set; }





    }
    public class DocumentTree
    {
        public List<C_DocumentTree> TreeList { get; set; }

        public bool IsLastTree { get; set; }//其自身是否为最后一级
        public bool IsChildLastTree { get; set; }//其子集是否为最后一级

    }
}
