using System;
using System.Collections.Generic;
using System.Web;
using Lib.Web.Json;

namespace Lib.Web.Mvc
{
    //public abstract class ResponseWriter
    //{
    //    protected LibViewModel ViewModel { get; set; }

    //    public ResponseWriter(LibViewModel viewModel)
    //    {
    //        this.ViewModel = viewModel;
    //    }

    //    public static void Write(LibViewModel model)
    //    {
    //        ResponseWriter writer = CreateResponseWriter(model);
    //        writer.Write();
    //    }

    //    public static ResponseWriter CreateResponseWriter(LibViewModel viewModel)
    //    {
    //        switch (viewModel.ModeType)
    //        { 
    //            case LibViewModelType.Json:
    //                return new JsonResponseWriter(viewModel);
    //            case LibViewModelType.Html:
    //                return new HtmlResponseWriter(viewModel);
    //            default :
    //                return new JsonResponseWriter(viewModel);
    //        }
    //    }

    //    public void Write()
    //    {
    //        HttpResponse response = PrepareResponse();

    //        BeforeWriteResponse(response);

    //        InnerWriteResponse(response);

    //        AfterWriteResponse(response);
    //    }

    //    protected virtual void AfterWriteResponse(HttpResponse response)
    //    {
            
    //    }

    //    protected abstract void InnerWriteResponse(HttpResponse response);

    //    protected virtual HttpResponse PrepareResponse()
    //    {
    //        HttpResponse response = HttpContext.Current.Response;

    //        return response;
    //    }

    //    protected virtual void BeforeWriteResponse(HttpResponse response)
    //    { }
    //}
}
