using Lib.Data;
using LJTH.Lib.Data.AppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Model.BizModel
{
    /// <summary>
    /// 员工表
    /// </summary>
    [Serializable]
    [ORTableMapping("dbo.Employee")]
    public class Employee:BaseModel
    {
        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }

        [ORFieldMapping("EmployeeName")]
        public string EmployeeName { get; set; }

        [ORFieldMapping("Gender")]
        public int Gender { get; set; }

        [ORFieldMapping("BrithDay")]
        public DateTime BrithDay { get; set; }

        [ORFieldMapping("Email")]
        public string Email { get; set; }

        [ORFieldMapping("Tel")]
        public string Tel { get; set; }

        [ORFieldMapping("Mobile")]
        public string Mobile { get; set; }

        [ORFieldMapping("Address")]
        public string Address { get; set; }

        [ORFieldMapping("JobTitle")]
        public string JobTitle { get; set; }

        [ORFieldMapping("DeptID")]
        public int DeptID { get; set; }

        [ORFieldMapping("JobLevel")]
        public int JobLevel { get; set; }

        [ORFieldMapping("OrderLevel")]
        public int OrderLevel { get; set; }

        [ORFieldMapping("CreatorLoginName")]
        public string CreatorLoginName { get; set; }

        [ORFieldMapping("ModifierLoginName")]
        public string ModifierLoginName { get; set; }

        [ORFieldMapping("EmployeeStatus")]
        public int EmployeeStatus { get; set; }

        [ORFieldMapping("AvatarPath")]
        public string AvatarPath { get; set; }

        [ORFieldMapping("Thumb")]
        public string Thumb { get; set; }
    }
}
