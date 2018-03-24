using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Core;
using System.Reflection;
using System.Web;
using Lib.Web.Mvc;
using Lib.Web.Json;
using System.Configuration;
namespace Lib.Web.MVC.Controller
{
    public class BaseController
    {
        private static IDictionary<string, BaseController> _controllers = null;

        /// <summary>
        /// 根据名称找到Controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns>找不到则返回null</returns>
        internal static BaseController GetController(string controllerName)
        {
            if (_controllers == null)
            {
                InitControllers();
            }
            string key = controllerName.ToLower();
            if (key.EndsWith(controllerString))
            {
                key = key.Substring(0, key.Length - controllerString.Length);
            }

            return DictionaryHelper.GetValue<string, BaseController, BaseController>(_controllers, key, null);
        }

        private const string controllerString = "controller";

        private static void InitControllers()
        {
            // 指定的多个带有controller的assembly的名称
            string ajaxHandlerAssemblies = ConfigurationManager.AppSettings["ControllerAssemblies"];  //"LJTH.HR.KPI.Web.AjaxHandler";

            _controllers = new Dictionary<string, BaseController>();

            string[] ajaxHandlerAssemblyArray = ajaxHandlerAssemblies.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Type> controllerTypes = new List<Type>();

            foreach (var assembly in ajaxHandlerAssemblyArray)
            {
                var types = (from t in Assembly.Load(assembly).GetTypes()
                             where IsSubClassOf(t, typeof(BaseController))
                             select t).ToList();
                controllerTypes.AddRange(types);
            }



            foreach (Type type in controllerTypes)
            {
                string typeName = type.Name.ToLower();
                // todo alias

                BaseController ctrl = (BaseController)Activator.CreateInstance(type);

                if (typeName.EndsWith(controllerString))
                {
                    typeName = typeName.Substring(0, typeName.Length - controllerString.Length);
                }
                _controllers.Add(typeName, ctrl);
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

        /// <summary>
        /// Default Action, 以方便显示的提醒开发人员
        /// </summary>
        [LibAction(true)]
        public virtual string DefaultAction()
        {
            string msg = "DefaultAction Executing.";
            if (HttpContext.Current != null && HttpContext.Current.Items["action"] != null)
            {
                msg += "The expected action is " + HttpContext.Current.Items["action"].ToString();
            }

            throw new ApplicationException(msg);
            //LibViewModel model = LibViewModel.CreateFailureJSONResponseViewModel(LibViewModelCode.InvalidActionFlag, msg);
            //ResponseWriter.Write(model);
        }


        /// <summary>
        /// 反序列化Json数据， 并对重要字段检查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPostData">json数据</param>
        /// <param name="check">检查， 防止重点数据未能反序列化后加载</param>
        /// <returns></returns>
        protected static T DeserializeAndCheck<T>(string jsonPostData, Predicate<T> check)
        {
            if (string.IsNullOrEmpty(jsonPostData))
            {
                return default(T);
            }
            try
            {
                T result = JsonHelper.Deserialize<T>(jsonPostData);

                if (check != null && !check(result))
                {
                    throw new ArgumentException("数据反序列化异常：" + jsonPostData);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
            }
        }

        /// <summary>
        /// 反序列化Json对象数组， 并对重要字段检查
        /// </summary>
        /// <typeparam name="T1">集合</typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="jsonPostData">json数据</param>
        /// <param name="check">检查， 防止重点数据未能反序列化后加载； 如果为null或者Empty的话， 则不检查</param>
        /// <returns></returns>
        protected static T1 DeserializeAndCheck<T1, T2>(string jsonPostData, Predicate<T2> check)
            where T1 : IEnumerable<T2>
        {
            if (string.IsNullOrEmpty(jsonPostData))
            {
                return default(T1);
            }
            try
            {
                T1 result = JsonHelper.Deserialize<T1>(jsonPostData);
                if (((IEnumerable<T2>)result).Count() == 0)
                {
                    return result;
                }
                T2 obj = ((IEnumerable<T2>)result).ToList().First();
                if (check != null && !check(obj))
                {
                    throw new ArgumentException("数据反序列化异常：" + jsonPostData);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("数据反序列化异常：" + jsonPostData, ex);
            }
        }


    }
}
