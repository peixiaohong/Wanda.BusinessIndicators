using Lib.Core;
using Lib.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lib.Web.MVC
{
    internal abstract class ActionResult
    {

        //private ResponseWriter _responseWriter = null;

        protected object _data = null;

        public ActionResult(object data)
        {
            this._data = data;
        }

        public abstract void ExecuteResult(HttpContext context);
    

        internal static ActionResult Create(object data)
        {

            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "data is null!");

            if (data is ActionResult)
            {
                return (ActionResult)data;
            }
            // 当前只设置一种返回类型，即json
            ActionResult result = new JsonResult(data);
            return result;
        }

      
    }

 
   

}
