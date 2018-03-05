using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using Lib.Core;
using Lib.Web.MVC.Controller;

namespace Lib.Web.MVC
{
    internal class ControllerInfo
    {
        private MethodInfo defaultMethod = null;
        private BaseController controller = null;

        private List<MethodInfo> methods = null;
        //private List<MethodInfo> postMethods = null;

        public ControllerInfo(BaseController controller)
        {
            this.controller = controller;
            ParseController( );
        }

        private void ParseController( )
        {
            MethodInfo[] mis = this.controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
             
            foreach (MethodInfo mi in mis)
            {
                LibActionAttribute attr = AttributeHelper.GetCustomAttribute<LibActionAttribute>(mi);
                if (attr != null)
                {
                    this.Methods.Add(mi);
                    if (this.DefaultMethod == null && attr.Default)
                    { this.DefaultMethod = mi; }
                }
            }
        }

        public List<MethodInfo> Methods
        {
            get
            {
                if (this.methods == null)
                    this.methods = new List<MethodInfo>();
                return this.methods;
            }
        }

        public MethodInfo DefaultMethod
        {
            get
            {
                return defaultMethod;
            }
            set
            {
                this.defaultMethod = value;
            }
        }


        public BaseController Controller { get { return this.controller; } }


        public JxTypeEnum JxType { get; set; }
    }
}
