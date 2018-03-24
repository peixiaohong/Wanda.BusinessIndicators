using System.Data.SqlClient;
using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using LJTH.Lib.AuthCenter.Model;
using LJTH.Lib.Data.AppBase;
using Lib.Core;
namespace LJTH.Lib.AuthCenter.DAL
{


    sealed class WD_UserAdapter : AuthCommonAdapter
    {
        public static WD_UserAdapter Instance = new WD_UserAdapter();


        //        /// <summary>
        //        /// 根据条件查询出万达用户列表
        //        /// </summary>
        //        /// <param name="filter">过滤条件</param>

        //        /// <returns></returns>
        //        public PartlyCollection<BUserinfo> GetWDUserinfoList(UserFilter filter)
        //        {
        ////            string sql = @"select employeeName,username,unitName,jobName,gender,mobile,employeeStatus,employeeCode from WD_User where employeeCode in (
        ////                                                select employeeCode from
        ////                                                (select employeeCode,row_number() over (order by employeeCode) as
        ////                                                num from WD_User where 1=1 {2}) 
        ////                                                as settable
        ////                                                where num between ({0}-1)*{1}+1 and {0}*{1})";
        ////            string strwhere = "";
        ////            strwhere += !string.IsNullOrEmpty(filter.Name) ? " and (employeeName like '%" + SqlTextHelper.EscapeLikeString(filter.Name) + "%' or username like '%" + SqlTextHelper.EscapeLikeString(filter.Name) + "%') " : string.Empty;
        ////            strwhere += !string.IsNullOrEmpty(filter.Department) ? " and unitName like '%" + SqlTextHelper.EscapeLikeString(filter.Department) + "%'" : string.Empty;
        ////            //strwhere += !string.IsNullOrEmpty(filter.Status) ? " and employeeStatusName='" +  SqlTextHelper.SafeQuote(filter.Status) + "'" : string.Empty;


        ////            DataTable dt = ExecuteReturnTable(string.Format(sql, filter.PageIndex, filter.PageSize, strwhere));
        ////            List<BUserinfo> result = new List<BUserinfo>();
        ////            BUserinfo model = null;
        ////            if (dt != null)
        ////            {
        ////                for (int i = 0; i < dt.Rows.Count; i++)
        ////                {
        ////                    model = new BUserinfo();
        ////                    model.Name = !string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? dt.Rows[i][0].ToString() : "";
        ////                    model.LoginName = !string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? dt.Rows[i][1].ToString() : "";
        ////                    model.Department = !string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? dt.Rows[i][2].ToString() : "";
        ////                    model.JobTitle = !string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? dt.Rows[i][3].ToString() : "";
        ////                    //model.Gender = !string.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? (dt.Rows[i][4].ToString()=="1"?"男":"女"): "";
        ////                    //model.Phone = !string.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? dt.Rows[i][5].ToString() : "";
        ////                    model.IsForbidden = dt.Rows[i][6].ToString().Trim() == "2" ? "False" : "True";
        ////                    //model.WD_UserID = !string.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? dt.Rows[i][7].ToString() : "";
        ////                    BUserinfo userInfo = new UserinfoAdapter().GetUserInfoByWDUserID(model.WD_UserID);

        ////                    //ID 为自增列
        ////                    //model.ID = userInfo != null ? userInfo.ID : dt.Rows[i][7].ToString();
        ////                    if (userInfo != null)
        ////                    {
        ////                        model.ID = userInfo.ID;
        ////                    }
        ////                    // List<Roleinfo> RoleList= new RoleinfoAdapter().GetBelongsToRolesByWD(model.UserID).ToList();


        ////                    //model.Roles = new RoleinfoAdapter().GetBelongsToRoles(new UserinfoAdapter().GetUserInfoByWDUserID(dt.Rows[i][7].ToString()).ID).ToList();
        ////                    result.Add(model);
        ////                }
        ////            }
        ////            DataTable numdt = ExecuteReturnTable("Select Count(0) From WD_User where 1=1 " + strwhere + "");
        ////            if (numdt != null)
        ////            {
        ////                itemCount = Convert.ToInt32(numdt.Rows[0][0]);
        ////            }
        ////            else
        ////            {
        ////                itemCount = 0;
        ////            }

        //   //         return result;
        //            throw new NotImplementedException();

        //        }


        /// <summary>
        /// 获取用户详细信息根据用户Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BUserinfo GetWDUserInfoByID(int userId)
        {
            string sql = @"
SELECT employeeName,
        username,
        unitName,
        jobName,
        gender,
        mobile,
        employeeStatus as Status ,
        employeeCode 
FROM dbo.WD_User 
    where employeeCode=" + userId + " ";
            DataTable dt = ExecuteReturnTable(sql);
            BUserinfo model = null;
            if (dt != null)
            {

                model = new BUserinfo();
                model.Name = !string.IsNullOrEmpty(dt.Rows[0][0].ToString()) ? dt.Rows[0][0].ToString() : "";
                model.LoginName = !string.IsNullOrEmpty(dt.Rows[0][1].ToString()) ? dt.Rows[0][1].ToString() : "";
                model.DisplayName = !string.IsNullOrEmpty(dt.Rows[0][0].ToString()) ? dt.Rows[0][0].ToString() : "";
                model.Department = !string.IsNullOrEmpty(dt.Rows[0][2].ToString()) ? dt.Rows[0][2].ToString() : "";
                model.JobTitle = !string.IsNullOrEmpty(dt.Rows[0][3].ToString()) ? dt.Rows[0][3].ToString() : "";
                //model.Gender = !string.IsNullOrEmpty(dt.Rows[0][4].ToString()) ? (dt.Rows[0][4].ToString() == "1" ? "男" : "女") : "";
                model.Phone = !string.IsNullOrEmpty(dt.Rows[0][5].ToString()) ? dt.Rows[0][5].ToString() : "";
                model.Status = GetEmployeeStatus(DataConverter.ChangeType<object, string>(dt.Rows[0][6]) );
                model.WD_UserID = DataConverter.ChangeType<object, int>(dt.Rows[0][7]);
            }
            return model;

        }
        /// <summary>
        /// 获取用户详细信息根据用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BUserinfo GetWDUserInfoByUserName(string userName)
        {
            string sql = @"
SELECT 
        employeeName,
        username,
        unitName,
        jobName,
        gender,
        mobile,
        employeeStatus as Status ,
        employeeCode 
    FROM dbo.WD_User
where username='" + SqlTextHelper.SafeQuote(userName) + "'";

            DataTable dt = ExecuteReturnTable(sql);
            BUserinfo model = null;
            if (dt != null)
            {

                model = new BUserinfo();
                model.Name = !string.IsNullOrEmpty(dt.Rows[0][0].ToString()) ? dt.Rows[0][0].ToString() : "";
                model.LoginName = !string.IsNullOrEmpty(dt.Rows[0][1].ToString()) ? dt.Rows[0][1].ToString() : "";
                model.DisplayName = !string.IsNullOrEmpty(dt.Rows[0][0].ToString()) ? dt.Rows[0][0].ToString() : "";
                model.Department = !string.IsNullOrEmpty(dt.Rows[0][2].ToString()) ? dt.Rows[0][2].ToString() : "";
                model.JobTitle = !string.IsNullOrEmpty(dt.Rows[0][3].ToString()) ? dt.Rows[0][3].ToString() : "";
                //model.Gender = !string.IsNullOrEmpty(dt.Rows[0][4].ToString()) ? (dt.Rows[0][4].ToString() == "1" ? "男" : "女") : "";
                model.Phone = !string.IsNullOrEmpty(dt.Rows[0][5].ToString()) ? dt.Rows[0][5].ToString() : "";
                model.Status = GetEmployeeStatus(DataConverter.ChangeType<object, string>(dt.Rows[0][6]) );
                model.WD_UserID = DataConverter.ChangeType<object, int>(dt.Rows[0][7]);
            }
            return model;

        }

        private static Dictionary<string, string> _employStatusDict = null;

        private static Dictionary<string, string> EmployStatusDict
        {
            get
            {
                if (_employStatusDict == null)
                {
                    _employStatusDict = new Dictionary<string, string>();
                    _employStatusDict["2"] = "正式";
                    _employStatusDict["3"] = "离职";
                    _employStatusDict["4"] = "退休";
                    _employStatusDict["5"] = "内退";
                    _employStatusDict["6"] = "离休";
                    _employStatusDict["9"] = "实习";
                    _employStatusDict["10"] = "实习结束";
                    _employStatusDict["11"] = "调动中";

                }
                return _employStatusDict;
            }
        }

        private static string GetEmployeeStatus(string status)
        {
            string result = "unknown";
            EmployStatusDict.TryGetValue(status, out result);
            return result;
        }
    }
}
