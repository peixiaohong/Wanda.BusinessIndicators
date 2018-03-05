using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class LibActionAttribute : Attribute
    {
        private bool isDefault = false;

        /// <summary>
        /// 构造方法
        /// </summary>
        public LibActionAttribute()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="isdefault"></param>
        public LibActionAttribute(bool isdefault)
        {
            this.isDefault = isdefault;
        }

        //public ActionAttribute(string actionKey)
        //{
        //    this.actionKey = actionKey;
        //}

        //public ActionAttribute(bool isdefault, string actionKey)
        //{
        //    this.isDefault = isdefault;
        //    this.actionKey = actionKey;
        //}

        /// <summary>
        /// 是否是缺省的方法
        /// </summary>
        public bool Default
        {
            get { return this.isDefault; }
            set { this.isDefault = value; }
        }


        //public string ActionKey
        //{
        //    get { return this.actionKey; }
        //    set { this.actionKey = value; }
        //}
    }
}
