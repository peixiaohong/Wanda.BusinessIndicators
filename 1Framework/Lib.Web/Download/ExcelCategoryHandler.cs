using Lib.Config;
using Lib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lib.Web.Download
{
    public abstract class ExcelCategoryHandler
    {

        public ExcelOutputEnum OutputType { get; set; }

        public virtual bool CheckBeforeDownload(System.Web.HttpContext context)
        {
            string userName = "";
            bool result = true;
            try
            {
               userName = context.User.Identity.Name;
            }
            catch
            {
                result= false;
            }

            string content = string.Format("RequestUrl:{0}\r\nIP:{1}\r\nUser:{2}",
                context.Request.Url.ToString(),
                WebUtility.GetClientIP(),
                userName);

            Log(content);
            return result;
        }

        public abstract object Query(System.Web.HttpContext context);


        public abstract MemoryStream ToStream(object queryResult);


        internal static ExcelCategoryHandler GetCategoryHandler(string category)
        {
            return ExcelCategoryDict.GetCategoryHandler(category);
        }

        public string FileName { get; set; }

        private static Logger _autoLogger = null;
        public static Logger AutoLogger
        {
            get
            {
                if (_autoLogger == null)
                {
                    _autoLogger = LoggerFactory.Create("AutoLog");
                }
                return _autoLogger;
            }
        }

        private static string Log(string content)
        {

            try
            {
                if (AutoLogger != null)
                    AutoLogger.Write(content, "ExcelCategoryHandler", LogPriority.AboveNormal, 0, System.Diagnostics.TraceEventType.Error, "ExcelCategoryHandler");
            }
            catch { }
            return content;
        }
    }

    public enum ExcelOutputEnum
    {
        FileStream = 0,
        HtmlPage = 1,
        Unknown = 2
    }

    internal class ExcelCategoryDict
    {

        private static IDictionary<string, ExcelCategoryHandler> _excelHandlers = null;


        internal static ExcelCategoryHandler GetCategoryHandler(string category)
        {
            if (_excelHandlers == null)
            {
                InitExcelHandlers();
            }

            category = category.ToLower();

            if (_excelHandlers.ContainsKey(category))
            {
                return _excelHandlers[category];
            }

            return null;
        }

        private static void InitExcelHandlers()
        {
            // 指定的多个带有controller的assembly的名称
            string handlerAssemblies = AppSettingConfig.GetSetting("ExcelHandlerAssemblies", "");  //"LJTH.HR.KPI.Web.Common";

            _excelHandlers = new Dictionary<string, ExcelCategoryHandler>();

            string[] assemblyArray = handlerAssemblies.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Type> handlerTypes = new List<Type>();

            foreach (var assembly in assemblyArray)
            {
                var types = (from t in Assembly.Load(assembly).GetTypes()
                             where IsSubClassOf(t, typeof(ExcelCategoryHandler))
                             select t).ToList();
                handlerTypes.AddRange(types);
            }



            foreach (Type type in handlerTypes)
            {
                string typeName = type.Name.ToLower();
                // todo alias

                ExcelCategoryHandler handler = (ExcelCategoryHandler)Activator.CreateInstance(type);

                _excelHandlers.Add(typeName, handler);
            }

        }

        private static bool IsSubClassOf(Type type, Type baseType)
        {
            var b = type.BaseType;
            while (b != null)
            {
                if (b.Equals(baseType))
                {
                    return true;
                }
                b = b.BaseType;
            }
            return false;
        }

    }
}
