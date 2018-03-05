using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Lib.Data
{
    /// <summary>
    /// 将IDataFilter对象转换为QueryCondition对象
    /// </summary>
    public static class DataFilterHelper
    {
        public static WhereSqlClauseBuilder ConvertToWhereBuilder(this IDataFilter filter)
        {

            Type type = filter.GetType();

            PropertyInfo[] propertyInfos = type.GetProperties();

            WhereSqlClauseBuilder sqlBuilder = new WhereSqlClauseBuilder();
            foreach (PropertyInfo p in propertyInfos)
            {
                FieldAndOperator f_a_o = null;
                if (filter.FilterDict != null && filter.FilterDict.Keys.Contains(p.Name))
                {
                    f_a_o = filter.FilterDict[p.Name];
                }
                else
                {
                    FilterFieldAttribute filterFieldAttr = (FilterFieldAttribute)Attribute.GetCustomAttribute(p, typeof(FilterFieldAttribute));
                    if (filterFieldAttr == null)
                    {
                        continue;
                    }
                    filterFieldAttr.Fao.DefaultV = filterFieldAttr.DefaultV;
                    f_a_o = filterFieldAttr.Fao;
                }
                object value = p.GetValue(filter, null);

                var defaultV = f_a_o.DefaultV ?? GetDefaultValue(p.PropertyType);

                if (value == null || value.Equals(defaultV))
                {
                    continue;
                }


                sqlBuilder.AppendItem(f_a_o.TableFieldName, value, f_a_o.Operator);
            }
            return sqlBuilder;

        }

        private static object GetDefaultValue(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
