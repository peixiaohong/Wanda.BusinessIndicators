using Lib.Data;
using System;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.LightWorkflow.Entities
{
    [ORViewMapping(@"SELECT DISTINCT * FROM (SELECT a.ID,a.ProcessID,a.ProcessCode,a.BizProcessID,a.InstanceName,a.StartTime,a.FinishTime,a.LastUpdatedTime,
                        a.Status,a.UserName,a.BizProcessContext,a.IsDeleted,a.CreateTime,a.CreatorName,b.UserID,c.Name+'('+c.LoginName+')' as UserLoginName
                    FROM lwf_ProcessInstance a INNER JOIN LWF_ApprovalLog b ON a.id=b.ProcessInstanceID
                    INNER JOIN PB_UserInfo c on c.ID=a.UserID 
                    WHERE a.IsDeleted=0 and b.IsDeleted=0 
                    UNION ALL 
                    SELECT a.ID,a.ProcessID,a.ProcessCode,a.BizProcessID,a.InstanceName,a.StartTime,a.FinishTime,a.LastUpdatedTime,
                            a.Status,a.UserName,a.BizProcessContext,a.IsDeleted,a.CreateTime,a.CreatorName,a.UserID,a.UserName+'('+LoginName+')' as UserLoginName
                    FROM lwf_ProcessInstance a INNER JOIN PB_UserInfo b ON a.UserID=b.ID
                    WHERE a.IsDeleted=0 and b.IsDeleted=0 ) AS TEMP", "VPInstanceAndStakeHolder")]
    public class VPInstanceAndStakeHolder : IBaseComposedModel
    {
//        private readonly string _adminViewSql = @"
//                (
//                    SELECT DISTINCT a.ID,a.ProcessID,a.ProcessCode,a.BizProcessID,a.InstanceName,a.StartTime,a.FinishTime,a.LastUpdatedTime,
//                            a.Status,a.UserName,a.BizProcessContext,a.IsDeleted,a.CreateTime,a.CreatorName,b.ID as userID,a.UserName+'('+LoginName+')' as UserLoginName
        //                    FROM lwf_ProcessInstance a INNER JOIN PB_UserInfo b ON a.UserID=b.ID
//                    WHERE a.IsDeleted=0 and b.IsDeleted=0
//                ) AS VPInstanceAndStakeHolder";

        [ORFieldMapping("ID", PrimaryKey = true)]
        public int ID { get; set; }


        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }


        /// <summary>
        /// 标示是否已删除（标记删除）
        /// </summary>
        [ORFieldMapping("IsDeleted")]
        public virtual bool IsDeleted { get; set; }

        [ORFieldMapping("ProcessID")]
        public int ProcessID { get; set; }



        [ORFieldMapping("ProcessCode")]
        public string ProcessCode { get; set; }



        [ORFieldMapping("BizProcessID")]
        public int BizProcessID { get; set; }



        [ORFieldMapping("InstanceName")]
        public string InstanceName { get; set; }



        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }



        [ORFieldMapping("FinishTime")]
        public DateTime FinishTime { get; set; }



        [ORFieldMapping("LastUpdatedTime")]
        public DateTime LastUpdatedTime { get; set; }



        [ORFieldMapping("Status")]
        public int Status { get; set; }


        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        [ORFieldMapping("UserLoginName")]
        public string UserLoginName { get; set; }

        [ORFieldMapping("BizProcessContext")]
        public string BizProcessContext { get; set; }
    }
}
