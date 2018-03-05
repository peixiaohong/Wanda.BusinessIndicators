using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml.Linq;

namespace Lib.Data
{
    /// <summary>
    /// 带数据的Sql语句构造项的基类
    /// </summary>
    [Serializable]
    public class SqlCaluseBuilderItemWithData : SqlClauseBuilderItemBase
    {
        private object data = null;
        private bool isExpression = false;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        /// <summary>
        /// 构想项中的Data是否是sql表达式
        /// </summary>
        public bool IsExpression
        {
            get { return this.isExpression; }
            set { this.isExpression = value; }
        }

        /// <summary>
        /// 得到Data的Sql字符串描述
        /// </summary>
        /// <param name="builder">构造器</param>
        /// <returns>返回将data翻译成sql语句的结果</returns>
        public override string GetDataDesp(ISqlBuilder builder)
        {
            string result = string.Empty;

            if (this.data == null || this.data is DBNull)
                result = "NULL";
            else
            {
                if (this.data is DateTime)
                {
                    if ((DateTime)this.data == DateTime.MinValue)
                        result = "NULL";
                    else
                        result = builder.FormatDateTime((DateTime)this.data);
                }
                else if (this.data is System.Guid)
                {
                    if ((Guid)this.data == Guid.Empty)
                        result = "NULL";
                    else
                        result = builder.CheckQuotationMark(this.data.ToString(), true);
                }
                else if (this.data is XElement) //Xml类型
                {
                    result = builder.CheckQuotationMark(this.data.ToString(), true);
                }
                else if (this.data is byte[]) //二进制类型
                {
                    result = "0x" + string.Join("", ((byte[])this.data).Select(n => n.ToString("x2")));
                }
                else if (this.data is IEnumerable && this.data.GetType().IsGenericType) //泛型集合, 使用于 where 子句 in 结构的判断
                {
                    int count = 0;
                    Type itemType = null;
                    foreach (var item in (IEnumerable)this.data)
                    {
                        count += 1;
                        if (count == 1)
                        {
                            itemType = item.GetType();
                        }
                        break;

                    }

                    if (count == 0)
                    {
                        result = "(null)";

                    }
                    else
                    {

                        if (itemType.IsValueType  && itemType.IsPrimitive)
                          //  && Convert.ChangeType(Activator.CreateInstance(itemType),  0)) // 存在类型匹配问题 (0.0).Equals(0)=false
                        {
                            result = string.Format("({0})", JoinItems((IEnumerable)this.data, false, builder));
                        }
                        else
                        {
                            result = string.Format("({0})", JoinItems((IEnumerable)this.data, true, builder));
                        }
                    }
                }

                else
                {
                    if (this.isExpression == false && (this.data is string || this.data.GetType().IsEnum))
                        result = builder.CheckQuotationMark(this.data.ToString(), true);
                    else
                        if (this.data is bool)
                            result = ((int)Convert.ChangeType(this.data, typeof(int))).ToString();
                        else
                            result = this.data.ToString();
                }
            }

            return result;
        }

        private static string JoinItems(IEnumerable items, bool useQuote, ISqlBuilder builder)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in items)
            {
                result.Append(builder.CheckQuotationMark(item.ToString(), useQuote)).Append(",");
            }

            result.Length = result.Length - 1;
            return result.ToString();
        }
    }
}
