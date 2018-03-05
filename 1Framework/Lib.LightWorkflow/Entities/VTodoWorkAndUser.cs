using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Entities
{
    [ORViewMapping(@"SELECT a.*,c.Name+'('+c.LoginName+')' AS CreateUserLoginName FROM LWF_TodoWork a 
                    INNER JOIN PB_UserInfo c on a.CreateProcessUserID=c.ID", "VTodoWorkAndUser")]
    public class VTodoWorkAndUser : IBaseComposedModel
    {
        #region Public Properties

        [ORFieldMapping("ID", PrimaryKey = true)]
        public int ID { get; set; }


        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("CreateUserLoginName")]
        public string CreateUserLoginName { get; set; }

        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// 标示是否已删除（标记删除）
        /// </summary>
        [ORFieldMapping("IsDeleted")]
        public virtual bool IsDeleted { get; set; }

        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessInstanceID")]
        public int ProcessInstanceID { get; set; }



        [ORFieldMapping("ProcessCode")]
        public string ProcessCode { get; set; }



        [ORFieldMapping("InstanceName")]
        public string InstanceName { get; set; }



        [ORFieldMapping("NodeName")]
        public string NodeName { get; set; }



        [ORFieldMapping("PreviousNodeInstanceID")]
        public int PreviousNodeInstanceID { get; set; }

        [ORFieldMapping("NodeInstanceID")]
        public int NodeInstanceID { get; set; }

        [ORFieldMapping("NodeType")]
        public int NodeType { get; set; }



        [ORFieldMapping("CreatedTime")]
        public DateTime CreatedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }



        [ORFieldMapping("UserID")]
        public int UserID { get; set; }



        [ORFieldMapping("UserName")]
        public string UserName { get; set; }



        [ORFieldMapping("ModifyUserID")]
        public int ModifyUserID { get; set; }



        [ORFieldMapping("ModifycUserID")]
        public int ModifycUserID { get; set; }



        [ORFieldMapping("ModifyUserCode")]
        public string ModifyUserCode { get; set; }



        [ORFieldMapping("ModifyUserName")]
        public string ModifyUserName { get; set; }



        [ORFieldMapping("TodoType")]
        public int TodoType { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }



        [ORFieldMapping("CreateProcessUserID")]
        public int CreateProcessUserID { get; set; }



        [ORFieldMapping("CreateProcesscUserID")]
        public int CreateProcesscUserID { get; set; }



        [ORFieldMapping("CreateProcessUserCode")]
        public string CreateProcessUserCode { get; set; }



        [ORFieldMapping("CreateProcessUserName")]
        public string CreateProcessUserName { get; set; }



        [ORFieldMapping("CreateProcessTime")]
        public DateTime CreateProcessTime { get; set; }



        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }



        [ORFieldMapping("ProjectID")]
        public int ProjectID { get; set; }


        #endregion
    }
}
