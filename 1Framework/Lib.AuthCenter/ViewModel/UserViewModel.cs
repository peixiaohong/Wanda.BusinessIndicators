using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Wanda.Lib.AuthCenter.Model;

namespace Wanda.Lib.AuthCenter.ViewModel
{
    //public class UserViewModel
    //{
    //    public int UserID { get; set; }
    //    public string LoginName { get; set; }
    //    public string UserName { get; set; }
    //    public string JobTitle { get; set; }
    //    public string Department { get; set; }
    //    public string Status { get; set; }
    //    public string Phone { get; set; }
    //    public string Gender { get; set; }
    //    public string DataAuthorization { get; set; }
    //    /// <summary>
    //    /// 名字首字母
    //    /// </summary>
    //    public string Letter { get; set; }
    //    public List<RoleViewModel> Roles { get; set; }
    //    public string RolesString
    //    {
    //        get
    //        {
    //            return Roles != null ? string.Join(",", Roles.Select(r => r.RoleName).ToArray()) : "";
    //        }
    //    }

    //    public List<PrivilegeViewModel> BPrivileges { get; set; }
    //    public string BPrivilegesString { get { return BPrivileges != null ? string.Join(",", BPrivileges.Select(p => p.BPrivilegeName).ToArray()) : ""; } }


    //    public UserViewModel()
    //    {

    //    }

    //    public static UserViewModel Create(BUserinfo user)
    //    {
    //        UserViewModel result = new UserViewModel
    //        {
    //            UserID = user.ID,
    //            LoginName = user.LoginName,
    //            UserName = user.Name,
    //            JobTitle = user.JobTitle,
    //            Department = user.Department,
    //            Phone = user.Phone,
    //            // Gender = user.Gender,
    //            Status = user.IsForbidden == "False" ? "正式" : "离职"
    //        };
    //        return result;
    //    }
    //    /// <summary>
    //    /// 创建User对象， 附带角色信息
    //    /// </summary>
    //    /// <param name="user"></param>
    //    /// <param name="getUserRoles">实现一个获得用户所属角色的方法</param>
    //    /// <returns></returns>
    //    public static UserViewModel CreateWithRoles(BUserinfo user, Func<int, List<BRoleinfo>> getUserRoles)
    //    {
    //        UserViewModel result = Create(user);
    //        if (user.ID > 0)
    //        {
    //            result.Roles = (getUserRoles(user.ID)).Select(r => RoleViewModel.Create(r)).ToList();
    //        }
    //        return result;
    //    }


    //    /// <summary>
    //    /// 创建User对象， 附带BPrivilege信息
    //    /// </summary>
    //    /// <param name="role"></param>
    //    /// <param name="getBPrivileges"></param>
    //    /// <returns></returns>
    //    public static UserViewModel CreateWithBPrivileges(BUserinfo role, Func<BUserinfo, List<BPrivilege>> getBPrivileges)
    //    {
    //        UserViewModel result = Create(role);
    //        result.BPrivileges = getBPrivileges(role)
    //                            .Select(p => PrivilegeViewModel.Create(p))
    //                            .ToList();
    //        return result;
    //    }
    //}

    //public class RoleViewModel
    //{
    //    public int RoleID { get; set; }
    //    public string RoleName { get; set; }
    //    public string Comment { get; set; }
    //    public string Status { get; set; }
    //    public string ScopeID { get; set; }
    //    public string ScopeName { get; set; }

    //    public List<UserViewModel> Users { get; set; }
    //    public string UsersString { get { return Users != null ? string.Join(",", Users.Select(u => u.UserName).ToArray()) : ""; } }

    //    public List<PrivilegeViewModel> BPrivileges { get; set; }
    //    public string BPrivilegesString { get { return BPrivileges != null ? string.Join(",", BPrivileges.Select(p => p.BPrivilegeName).ToArray()) : ""; } }

    //    //public RoleViewModel()
    //    //{

    //    //}

    //    /// <summary>
    //    /// 简单转换
    //    /// </summary>
    //    /// <param name="role"></param>
    //    /// <returns></returns>
    //    public static RoleViewModel Create(BRoleinfo role)
    //    {
    //        RoleViewModel result = new RoleViewModel();
    //        result.RoleID = role.ID;
    //        result.RoleName = role.Name;
    //        result.Comment = role.Comment;
    //        result.ScopeID = role.ScopeID;
    //        result.Status = role.IsForbidden ? "禁用" : "活动";
    //        return result;
    //    }

    //    /// <summary>
    //    /// 带User信息
    //    /// </summary>
    //    /// <param name="role"></param>
    //    /// <returns></returns>
    //    public static RoleViewModel CreateWithUsers(BRoleinfo role, Func<BRoleinfo, List<BUserinfo>> getUsers)
    //    {
    //        RoleViewModel result = Create(role);
    //        result.Users = getUsers(role)
    //                        .Select(u => UserViewModel.Create(u))
    //                        .ToList();
    //        return result;
    //    }

    //    /// <summary>
    //    /// 创建Role对象， 附带对应BPrivilege
    //    /// </summary>
    //    /// <param name="role"></param>
    //    /// <param name="getBPrivileges">委托，根据角色找到对应的BPrivilege</param>
    //    /// <returns></returns>
    //    public static RoleViewModel CreateWithBPrivileges(BRoleinfo role, Func<BRoleinfo, List<BPrivilege>> getBPrivileges)
    //    {
    //        RoleViewModel result = Create(role);
    //        result.BPrivileges = getBPrivileges(role)
    //                            .Select(p => PrivilegeViewModel.Create(p))
    //                            .ToList();
    //        return result;
    //    }
    //}

    //public class PrivilegeViewModel
    //{
    //    public int BPrivilegeID { get; set; }
    //    public string BPrivilegeName { get; set; }
    //    public string BPrivilegeType { get; set; }
    //    public string GroupName { get; set; }
    //    public int GroupID { get; set; }

    //    public List<RoleViewModel> Roles { get; set; }
    //    public string RolesString
    //    {
    //        get
    //        {
    //            return Roles != null ? string.Join(",", Roles.Select(r => r.RoleName).ToArray()) : "";
    //        }
    //    }

    //    public List<UserViewModel> Users { get; set; }
    //    public string UsersString { get { return Users != null ? string.Join(",", Users.Select(u => u.UserName).ToArray()) : ""; } }


    //    public PrivilegeViewModel()
    //    {

    //    }

    //    public static PrivilegeViewModel Create(BPrivilege data)
    //    {
    //        PrivilegeViewModel result = new PrivilegeViewModel();
    //        result.BPrivilegeID = data.ID;
    //        //result.GroupID = data.GroupID;
    //        result.GroupName = data.GroupName;
    //        result.BPrivilegeName = data.Name;
    //        result.BPrivilegeType = EnumHelper.GetEnumDescription(typeof(PrivilegeType), data.PrivilegeType);
    //        return result;
    //    }

    //    /// <summary>
    //    /// 带User信息
    //    /// </summary>
    //    /// <param name="role"></param>
    //    /// <returns></returns>
    //    public static PrivilegeViewModel CreateWithUsers(BPrivilege data, Func<BPrivilege, List<BUserinfo>> getUsers)
    //    {
    //        PrivilegeViewModel result = Create(data);
    //        result.Users = getUsers(data)
    //                        .Select(u => UserViewModel.Create(u))
    //                        .ToList();
    //        return result;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="data"></param>
    //    /// <param name="getRoles">实现一个获得权限所属角色的方法</param>
    //    /// <returns></returns>
    //    public static PrivilegeViewModel CreateWithRoles(BPrivilege data, Func<BPrivilege, List<BRoleinfo>> getRoles)
    //    {
    //        PrivilegeViewModel result = Create(data);
    //        result.Roles = (getRoles(data))
    //                        .Select(r => RoleViewModel.Create(r))
    //                        .ToList();
    //        return result;
    //    }
    //}





    //[Serializable]
    //public class UserPair
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //}

    //public class UserModelTransfer
    //{
    //    public static UserPair ToPair(BUserinfo user)
    //    {
    //        if (user != null)
    //        {
    //            UserPair pair = new UserPair();

    //            pair.ID = user.ID;
    //            pair.Name = user.Name;
    //            return pair;
    //        }
    //        return null;
    //    }

    //    public static UserViewModel UserInfoToView(BUserinfo userinfo)
    //    {
    //        if (userinfo != null)
    //        {
    //            UserViewModel view = new UserViewModel();

    //            view.UserID = userinfo.ID;
    //            view.LoginName = userinfo.LoginName;
    //            view.UserName = userinfo.Name;
    //            view.Department = userinfo.Department;
    //            view.JobTitle = userinfo.JobTitle;
    //            view.Status = userinfo.IsForbidden;
    //            return view;
    //        }
    //        return null;
    //    }


    //    /// <summary>
    //    /// 进行搜索用转换
    //    /// </summary>
    //    /// <param name="userinfo"></param>
    //    /// <returns></returns>
    //    public static UserViewModel UserNameToView(BUserinfo userinfo)
    //    {
    //        if (userinfo != null)
    //        {
    //            UserViewModel view = new UserViewModel();

    //            view.UserID = userinfo.ID;
    //            view.LoginName = userinfo.LoginName;
    //            view.UserName = userinfo.Name;


    //            return view;
    //        }
    //        return null;
    //    }


    //}


    //[Serializable]
    //public class RolePair
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //}



    //[Serializable]
    //public class BPrivilegePair
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //}

    ///// <summary>
    ///// 权限试图转换 li jing guang 2013-05-14
    ///// </summary>
    //public class BPrivilegeTransfer
    //{
    //    public static BPrivilege ViewToBPrivilege(PrivilegeViewModel view)
    //    {
    //        BPrivilege data = new BPrivilege();
    //        if (view != null)
    //        {
    //            data.ID = view.BPrivilegeID;
    //            data.Name = view.BPrivilegeName;
    //            data.GroupName = data.GroupName;
    //            data.PrivilegeType = EnumHelper.GetEnumValue(typeof(PrivilegeType), view.BPrivilegeType);
    //            //data.GroupID = view.GroupID;

    //        }
    //        return data;
    //    }

    //    public static PrivilegeViewModel BPrivilegeToView(BPrivilege data, Func<BPrivilege, string> GetGroupName)
    //    {
    //        PrivilegeViewModel view = new PrivilegeViewModel();
    //        if (data != null)
    //        {
    //            view.BPrivilegeID = data.ID;
    //            view.BPrivilegeName = data.Name;
    //            view.BPrivilegeType = EnumHelper.GetEnumDescription(typeof(PrivilegeType), data.PrivilegeType);
    //            //view.GroupID = data.GroupID;
    //            view.GroupName = GetGroupName(data);
    //        }
    //        return view;
    //    }
    //}


    //public class RoleInfoTransfer
    //{
    //    public static BRoleinfo VMToModel(RoleViewModel view)
    //    {
    //        BRoleinfo data = new BRoleinfo();
    //        if (view != null)
    //        {
    //            data.ID = view.RoleID;
    //            data.Name = view.RoleName;
    //            data.Comment = view.Comment;
    //            data.ScopeID = view.ScopeID;
    //            data.IsForbidden = view.Status == "活动" ? false : true;
    //        }
    //        return data;
    //    }

    //    public static RoleViewModel ModelToVm(BRoleinfo data)
    //    {
    //        RoleViewModel view = new RoleViewModel();
    //        if (data != null)
    //        {
    //            view.RoleID = data.ID;
    //            view.RoleName = data.Name;
    //            view.Comment = data.Comment;
    //            view.ScopeID = data.ScopeID;
    //            view.Status = data.IsForbidden ? "禁用" : "活动";
    //        }
    //        return view;
    //    }
    //}
}
