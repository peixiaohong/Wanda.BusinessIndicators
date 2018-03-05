using Lib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Lib.Web.Download
{
    /// <summary>
    /// 数据处理分发器， 负责根据数据的类型， 将请求交给不同的ExcelController处理
    /// </summary>
    public static class ExcelGenerator
    {
        internal static void GenerateExcel(HttpContext context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(context != null, "HttpContext");

            ExcelCategoryHandler categoryHandler = ParseCategoryHandler(context); // throw ContrllerParseFailException or ControllerNotFoundException


            if (categoryHandler.CheckBeforeDownload(context))
            {

                Object queryResult = categoryHandler.Query(context);

                if (categoryHandler.OutputType == ExcelOutputEnum.FileStream)
                {
                    MemoryStream stream = categoryHandler.ToStream(queryResult);
                    if (stream == null)
                    {
                        context.Response.Write("无内容。");
                        context.Response.End();
                    }
                    else
                    {


                        context.Response.Clear();
                        context.Response.Buffer = true;
                        context.Response.Charset = "utf-8";
                        context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(categoryHandler.FileName, System.Text.Encoding.UTF8) + ".xls");
                        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                        context.Response.ContentType = "application/ms-excel";
                        context.Response.BinaryWrite(stream.ToArray());
                        context.Response.End();
                    }
                }
                else if (categoryHandler.OutputType == ExcelOutputEnum.HtmlPage)
                {
                    // 返回显示， 调试用
                    context.Items["queryResult"] = queryResult;
                    context.Server.Execute("~/_pagelet/excelhtml.aspx");
                }

            }
            else
            {
                HttpContext.Current.Response.Clear();
                context.Response.Write("未通过检查！请与系统管理员联系！");
                context.Response.End();
            }
        }

        private static ExcelCategoryHandler ParseCategoryHandler(HttpContext context)
        {
            string url = context.Request.Path;
            ExcelOutputEnum outputType = ParseUrlSuffix(url);
            //必须是.jx结尾
            if (outputType == ExcelOutputEnum.Unknown)
            {
                throw new ApplicationException("不是有效的Excel文件类型。 url=" + url);
            }


            string[] parts = GetUrlDivision(context);

            if (parts.Length == 0)
            {
                throw new ApplicationException("未指定Excel文档类型。 url=" + url); //无Controller
            }

            ExcelCategoryHandler controller = ExcelCategoryHandler.GetCategoryHandler(parts[0]); // parts[0] should be controller name
            if (controller == null)
            {
                throw new ApplicationException("未找到Excel文档的处理对象。 url=" + url); //无Controller
            }


            return controller;
        }

        private static string[] GetUrlDivision(HttpContext context)
        {
            string url = context.Request.Path;
            string[] parts = url
                            .Substring(0, url.LastIndexOf('.')) //删除尾部
                            .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries); // 分割
            return parts;
        }

        private static ExcelOutputEnum ParseUrlSuffix(string url)
        {
            string suffix = url.Substring(url.LastIndexOf('.')).ToLower();

            ExcelOutputEnum result;
            if (suffix == excel_suffix)
            {
                result = ExcelOutputEnum.FileStream;
            }
            else if (suffix == excel_html_suffix)
            {
                result = ExcelOutputEnum.HtmlPage;
            }
            else
            {
                result = ExcelOutputEnum.Unknown;
            }
            return result;
        }

        private static string excel_suffix = ".xlsh";

        private static string excel_html_suffix = ".xlshtml";
    }
}
