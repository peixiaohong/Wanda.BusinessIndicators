using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Plugin.EmploeeTransfer.Models;
using System.Data;
using System.Transactions;
using Plugin.EmployeeTransfer.Helper.DbHelper;
using Lib.Data;

namespace Plugin.EmployeeTransfer
{
    /// <summary>
    /// 基础人员及部门数据同步
    /// </summary>
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    class EmployeeTransferService : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            LogMgnt.Instance["EmployeeTransferService"].Info("------------基础人员及部门数据同步开始-------------");
            try
            {
                #region 01.生成部门数据

                #region 查询语句
                string oa_dept_sql = @"Select v_m_dept.IS_DELETE,
       v_m_dept.ID,
       v_m_dept.DESCRIPT,
       v_m_dept.PARENT_ID,
       v_m_dept.MDM_ID,
       v_m_dept.COMPANY_ID,
       v_m_dept.ORDERLY,
       v_m_dept.UPDATE_TIME
  From v_m_dept Where v_m_dept.IS_DELETE = 0 ";
                #endregion

                var dt_dept = OracleHelper.QueryData(oa_dept_sql);
                List<OA_Dept> oa_depts = ConvertOADeptList(dt_dept);


                var rootNodes = oa_depts.Where(x => x.PARENT_ID == 0 && x.COMPANY_ID == 0).OrderBy(x => x.ORDERLY).ToList();
                rootNodes.ForEach(x =>
                {
                    BuildDeptPath(oa_depts, x.ID);
                });
                //筛选掉断层的节点 说明上级节点已经被删除
                oa_depts = oa_depts.Where(x => !string.IsNullOrEmpty(x.DeptFullPath)).ToList();

                #region  生成sql语句

                List<string> dept_CmdList = new List<string>();
                dept_CmdList.Add("TRUNCATE TABLE [dbo].[Dept]");
                dept_CmdList.AddRange(oa_depts.Select(x => string.Format(
                        @"INSERT INTO[dbo].[Dept]
           ([ID]
           ,[DeptName]
           ,[OrderLevel]
           ,[ParentID]
           ,[DeptPath]
           ,[DeptFullName]
           ,[BatchTime]
           ,[CreatorLoginName]
           ,[CreatorName]
           ,[CreatorTime]
           ,[ModifierLoginName]
           ,[ModifierName]
           ,[ModifyTime]
           ,[IsDeleted])
     VALUES
           ({0}
           ,'{1}'
           ,{2}
           ,{3}
           ,'{4}'
           ,'{5}'
           ,'{6}'
           ,'{7}'
           ,'{8}'
           ,'{9}'
           ,'{10}'
           ,'{11}'
           ,'{12}'
           ,{13})"

    , x.ID
    , x.DESCRIPT
    , x.ORDERLY
    , x.REAL_PARENT_ID
    , x.DeptFullPath
    , x.DeptFullName
    , x.UPDATE_TIME
    , "EMPLOYEE_TRANSFER"
    , "EMPLOYEE_TRANSFER"
    , x.UPDATE_TIME
    , "EMPLOYEE_TRANSFER"
    , "EMPLOYEE_TRANSFER"
    , x.UPDATE_TIME
    , 0
                        )));

                #endregion

                string dept_ExecuteSqlList = string.Join("\r\n", dept_CmdList);
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    SqlHelper.ExecuteSql(dept_ExecuteSqlList);
                    scope.Complete();
                }

                #endregion

                #region 02.生成人员数据

                string oa_employee_sql = @"Select v_m_person.PERSON_ID_MDM,
       v_m_person.LAST_NAME,
       v_m_person.FIRST_NAME,
       v_m_person.ID,
       v_m_person.GENDER,
       v_m_person.BIRTHDAY,
       v_m_person.EMAIL,
       v_m_person.TEL,
       v_m_person.MOBILE,
       v_m_person.ADDR,
       v_m_person.TITLE,
       v_m_person.DEPT_ID,
       v_m_person.JOB_LEVEL,
       v_m_person.AVATAR,
       v_m_person.AVATAR_BIG,
       v_m_person.IS_DELETE,
       v_m_person.UPDATE_TIME,
       v_m_person.ORDERBY
  From v_m_person";

                var dt_employee = OracleHelper.QueryData(oa_employee_sql);
                List<OA_Employee> oa_employees = ConvertOAEmployeeList(dt_employee);

                #region  生成sql语句

                List<string> employee_CmdList = new List<string>();
                employee_CmdList.Add("TRUNCATE TABLE [dbo].[Employee]");
                employee_CmdList.AddRange(oa_employees.Select(x =>
                string.Format(@"
INSERT INTO [dbo].[Employee]
           ([ID]
           ,[OAID]
           ,[LoginName]
           ,[EmployeeName]
           ,[Gender]
           ,[BrithDay]
           ,[Email]
           ,[Tel]
           ,[Mobile]
           ,[Address]
           ,[JobTitle]
           ,[DeptID]
           ,[JobLevel]
           ,[OrderLevel]
           ,[CreatorLoginName]
           ,[CreatorName]
           ,[CreateTime]
           ,[ModifierLoginName]
           ,[ModifierName]
           ,[ModifyTime]
           ,[IsDeleted]
           ,[EmployeeStatus]
           ,[AvatarPath]
           ,[Thumb]

)
     VALUES
           ('{0}'
           ,{1}
           ,'{2}'
           ,'{3}'
           ,{4}
           ,'{5}'
           ,'{6}'
           ,'{7}'
           ,'{8}'
           ,'{9}'
           ,'{10}'
           ,{11}
           ,{12}
           ,{13}
           ,'{14}'
           ,'{15}'
           ,'{16}'
           ,'{17}'
           ,'{18}'
           ,'{19}'
           ,{20}
           ,{21}
           ,'{22}'
           ,'{23}')"
    , Guid.NewGuid()
    , x.ID
    , x.PERSON_ID_MDM
    , x.FIRST_NAME + x.LAST_NAME
    , x.GENDER
    , x.BIRTHDAY
    , x.EMAIL
    , x.TEL
    , x.MOBILE
    , x.ADDR
    , x.TITLE
    , x.DEPT_ID
    , x.JOB_LEVEL
    , x.ORDERBY
    , "EMPLOYEE_TRANSFER"
    , "EMPLOYEE_TRANSFER"
    , x.UPDATE_TIME
    , "EMPLOYEE_TRANSFER"
    , "EMPLOYEE_TRANSFER"
    , x.UPDATE_TIME
    , 0
    , x.IS_DELETE == 0 ? 2 : 3   //2在职 3离职
    , x.AVATAR_BIG
    , x.AVATAR
    )
                ));

                #endregion

                string employee_ExecuteSqlList = string.Join("\r\n", employee_CmdList);
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    SqlHelper.ExecuteSql(employee_ExecuteSqlList);
                    scope.Complete();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogMgnt.Instance["EmployeeTransferService"].Error("任务执行异常，错误信息{0}，错误堆栈{1}", ex.Message, ex.StackTrace);
            }
            LogMgnt.Instance["EmployeeTransferService"].Info("------------基础人员及部门数据同步完成-------------");
        }

        /// <summary>
        /// 将OA同步过来的人员数据转换为List 集合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<OA_Employee> ConvertOAEmployeeList(DataTable source)
        {
            List<OA_Employee> employees = new List<OA_Employee>();
            OA_Employee item = null;
            foreach (DataRow row in source.Rows)
            {
                item = new OA_Employee();
                item.ADDR = row["ADDR"].ToString();
                item.BIRTHDAY = row["BIRTHDAY"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["BIRTHDAY"]);
                item.DEPT_ID = Convert.ToInt32(row["DEPT_ID"]);
                item.EMAIL = row["EMAIL"].ToString();
                item.FIRST_NAME = row["FIRST_NAME"].ToString();
                item.GENDER = row["GENDER"] == DBNull.Value ? 0 : Convert.ToInt32(row["GENDER"]);
                item.ID = Convert.ToInt32(row["ID"]);
                item.JOB_LEVEL = row["JOB_LEVEL"] == DBNull.Value ? 0 : Convert.ToInt32(row["JOB_LEVEL"]);
                item.LAST_NAME = row["LAST_NAME"].ToString();
                item.MOBILE = row["MOBILE"].ToString();
                item.ORDERBY = row["ORDERBY"] == DBNull.Value ? 0 : Convert.ToInt32(row["ORDERBY"]);
                item.PERSON_ID_MDM = row["PERSON_ID_MDM"].ToString();
                item.TEL = row["TEL"].ToString();
                item.TITLE = row["TITLE"].ToString();
                item.UPDATE_TIME = Convert.ToDateTime(row["UPDATE_TIME"]);
                item.IS_DELETE = Convert.ToInt32(row["IS_DELETE"]);
                item.AVATAR_BIG = row["AVATAR_BIG"].ToString();
                item.AVATAR = row["AVATAR"].ToString();
                employees.Add(item);
            }

            return employees;
        }

        /// <summary>
        /// 将OA同步来的部门数据转换为List 集合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<OA_Dept> ConvertOADeptList(DataTable source)
        {
            List<OA_Dept> depts = new List<OA_Dept>();
            OA_Dept item = null;
            foreach (DataRow row in source.Rows)
            {
                item = new OA_Dept();
                item.COMPANY_ID = Convert.ToInt32(row["COMPANY_ID"]);
                item.DESCRIPT = row["DESCRIPT"].ToString();
                item.ID = Convert.ToInt32(row["ID"]);
                item.IS_DELETE = Convert.ToInt32(row["IS_DELETE"]);
                item.MDM_ID = Convert.ToInt32(row["MDM_ID"]);
                item.ORDERLY = Convert.ToInt32(row["ORDERLY"] == DBNull.Value ? "0" : row["ORDERLY"]);
                item.PARENT_ID = Convert.ToInt32(row["PARENT_ID"]);
                item.UPDATE_TIME = Convert.ToDateTime(row["UPDATE_TIME"]);
                depts.Add(item);
            }
            return depts;
        }

        /// <summary>
        /// 组织部门路径
        /// </summary>
        /// <param name="oa_depts"></param>
        /// <param name="startId"></param>
        private static void BuildDeptPath(List<OA_Dept> oa_depts, int startId)
        {
            var start = oa_depts.FirstOrDefault(x => x.ID == startId);
            if (start != null)
            {
                var parent = oa_depts.FirstOrDefault(x => x.ID == (start.PARENT_ID == 0 ? start.COMPANY_ID : start.PARENT_ID));
                var parentPath = "";
                var parentPathName = "";
                int parentId = 0;
                if (parent != null)
                {
                    parentId = parent.ID;
                    parentPath = parent.DeptFullPath;
                    parentPathName = parent.DeptFullName;
                }
                var fullPath = string.Format("{0}/{1}", parentPath, start.ID);
                var fullPathName = string.Format("{0}/{1}", parentPathName, start.DESCRIPT);

                start.DeptFullPath = fullPath;
                start.DeptFullName = fullPathName;
                start.REAL_PARENT_ID = parentId;

                List<OA_Dept> children = oa_depts.Where(x => x.PARENT_ID == start.ID || (x.PARENT_ID == 0 && x.COMPANY_ID == start.ID)).OrderBy(x => x.ORDERLY).ToList();
                if (children.Count > 0)
                {
                    children.ForEach(x => BuildDeptPath(oa_depts, x.ID));
                }
            }
        }
    }
}
