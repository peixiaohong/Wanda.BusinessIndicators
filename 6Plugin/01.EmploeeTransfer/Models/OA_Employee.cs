using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EmploeeTransfer.Models
{
    public class OA_Employee
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 登陆名
        /// </summary>
        public string PERSON_ID_MDM { get; set; }
        /// <summary>
        /// 姓 ???应该是名?
        /// </summary>
        public string FIRST_NAME { get; set; }
        /// <summary>
        /// 名 ???应该是姓?
        /// </summary>
        public string LAST_NAME { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int GENDER { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BIRTHDAY { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string EMAIL { get; set; }
        /// <summary>
        /// 固话
        /// </summary>
        public string TEL { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string MOBILE { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string TITLE { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public int DEPT_ID { get; set; }
        /// <summary>
        /// 职级
        /// </summary>
        public int JOB_LEVEL { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ORDERBY { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UPDATE_TIME { get; set; }
        /// <summary>
        /// 员工状态 0在职 1 离职
        /// </summary>
        public int IS_DELETE { get; set; }
        /// <summary>
        /// 头像大图
        /// </summary>
        public string AVATAR_BIG { get; set; }
        /// <summary>
        ///头像缩略图
        /// </summary>
        public string AVATAR { get; set; }
    }
}
