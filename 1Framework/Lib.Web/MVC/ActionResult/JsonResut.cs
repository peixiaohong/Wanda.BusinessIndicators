using Lib.Core;
using Lib.Web.Json;
using Lib.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lib.Web.MVC
{
    internal sealed class JsonResult : ActionResult
    {

        public JsonResult(object data)
            : base(data)
        {

        }

        private string _contentType = "text/plain";
        private Encoding _contentEncoding = Encoding.UTF8;

        public override void ExecuteResult(HttpContext context)
        {

            LibViewModel viewModel = LibViewModel.CreateSuccessJSONResponseViewModel();
            viewModel.ResultData = this._data;

            ExceptionHelper.TrueThrow<ArgumentNullException>(context == null, "context is null!");
            HttpResponse response = context.Response;

            response.ContentType = _contentType;
            response.ContentEncoding = _contentEncoding;

            response.Write(JsonHelper.Serialize(viewModel));
            HttpContext.Current.ApplicationInstance.CompleteRequest();
           // response.End(); 
        }

    }



}
