using System;
using System.Collections.Generic;


namespace Lib.Web.Security
{
    public interface ILoginUser
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        string ApplicationID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        string LoginName { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 扮演前的登录名
        /// </summary>
        string OriginalUserID { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性集合（不入库）
        /// </summary>
        Dictionary<string, object> Properties
        {
            get;
        }


        /// <summary>
        /// 将对象信息序列化成string类型
        /// </summary>
        string Serialize();

        /// <summary>
        /// 将数据反序列化成对象
        /// </summary>
        /// <param name="dataString"></param>
        ILoginUser Deserialize(string dataString);

        /// <summary>
        /// 判断当前登录用户是否属于某角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool IsInRole(string role);

        /// <summary>
        /// 登录信息持久化
        /// </summary>
        /// <returns></returns>
        int Persist();
    }
}
