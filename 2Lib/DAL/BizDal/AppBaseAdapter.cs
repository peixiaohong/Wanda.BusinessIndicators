using Lib.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
using System.Data;
using System.Reflection;

namespace LJTH.BusinessIndicators.DAL
{
    public class AppBaseCompositionAdapterT<T> : BaseCompositionAdapterT<T>
         where T : IBaseComposedModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }
    }

    public class AppCommonAdapter : CommonAdapter
    {
        protected override string ConnectionName
        {
            get { return ""; }
        }
    }

    public class AppBaseAdapterT<T> : BaseAdapterT<T>
     where T : BaseModel, new()
    {
        protected override string ConnectionName
        {
            get { return "WandaJYZBConnectionString"; }
        }

        /// <summary>
        /// DataTable 转List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected List<T> DataTableToListT(DataTable dt)
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value == DBNull.Value)
                            continue;
                        if (pi.PropertyType == typeof(string))
                        {
                            pi.SetValue(t, value.ToString(), null);
                        }
                        else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                        {
                            pi.SetValue(t, int.Parse(value.ToString()), null);
                        }
                        else if (pi.PropertyType == typeof(DateTime?) || pi.PropertyType == typeof(DateTime))
                        {
                            pi.SetValue(t, DateTime.Parse(value.ToString()), null);
                        }
                        else if (pi.PropertyType == typeof(float))
                        {
                            pi.SetValue(t, float.Parse(value.ToString()), null);
                        }
                        else if (pi.PropertyType == typeof(double))
                        {
                            pi.SetValue(t, double.Parse(value.ToString()), null);
                        }
                        else if (pi.PropertyType == typeof(Guid))
                        {
                            pi.SetValue(t, value.ToString().ToGuid(), null);
                        }
                        else
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }
    }
}
