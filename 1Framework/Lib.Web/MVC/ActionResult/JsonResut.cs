using Lib.Core;
using Lib.Web.Json;
using Lib.Web.Mvc;
using System;
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

            //var json = JsonHelper.Serialize(viewModel);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel);
            if (context.Request.Headers["UseGZip"] == ((int)UseGZip.ReturnData).ToString() || context.Request.Headers["UseGZip"] == ((int)UseGZip.Both).ToString())
            {
                DateTime start = DateTime.Now;
                var base64 = GZipHelper.Compress(json);
                DateTime end = DateTime.Now;
                TimeSpan span = end - start;
                response.Headers.Add("GZip-Time", span.TotalMilliseconds.ToString());
                response.Write(base64);
            }
            else
            {
                response.Write(json);
            }
            
            HttpContext.Current.ApplicationInstance.CompleteRequest();
           // response.End(); 
        }
        
    }



}
