using Lib.Web;
using Lib.Web.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Wanda.HR.Common;

namespace Wanda.BusinessIndicators.Common
{
    public class DemoExcelHandler : ExcelCategoryHandler
    {

        public DemoExcelHandler()
        {
            // 条件成熟的时候， 考虑将下面的这些参数从配置中读取

            this.FileName = "Demo";
            this.ListName = "Number";
            this.TmplName = "DemoTmpl.xlsx";
            this.TmplPath = WebUtility.MapPath("~/app_data/ExcelTmpl/");
        }
        public string TmplPath { get; set; }
        public string TmplName { get; set; }
        public string ListName { get; set; }

        public override object Query(HttpContext context)
        {
            List<int> result = new List<int>();
            result.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7 });
            return result;
        }

        public override MemoryStream ToStream(object queryResult)
        {
            if (queryResult == null)
            {
                throw new ArgumentNullException("queryResult");
            }

            var list = ((List<int>)queryResult).Select(p => new MyClass { index = p, name = "huwz" }).ToList();

            ExcelEngine engine = new ExcelEngine();
            return engine.ExportExcel<MyClass>(list, ListName, TmplPath, TmplName);

        }
    }

    public class MyClass
    {
        public int index { get; set; }
        public string name { get; set; }
    }
}
