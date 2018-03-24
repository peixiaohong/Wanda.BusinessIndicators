using Lib.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Lib.Data.AppBase;
using System.Runtime.Serialization;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.Model
{
    /// <summary>
    /// This object represents the properties and methods of a InterfaceDefinition.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.S_Interface")]
    public partial class InterfaceDefinition : BaseModel
    {

        #region Public Properties


        [ORFieldMapping("InterfaceName")]
        public string InterfaceName { get; set; }

        [ORFieldMapping("Description")]
        public string Description { get; set; }



        [ORFieldMapping("IsDefault")]
        public bool IsDefault { get; set; }


        #endregion

    }

    /// <summary>
    /// This object represents the properties and methods of a InstanceInstance.
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.S_InterfaceInstance")]
    public partial class InterfaceInstance : BaseModel
    {

        #region Public Properties

        [ORFieldMapping("InterfaceID")]
        public string InterfaceID { get; set; }

        [ORFieldMapping("InterfaceName")]
        public string InterfaceName { get; set; }

        [ORFieldMapping("InterfaceInstanceName")]
        public string InterfaceInstanceName { get; set; }

        [ORFieldMapping("Reference")]
        public string Reference { get; set; }

        [ORFieldMapping("Description")]
        public string Description { get; set; }


        #endregion

    }

}
