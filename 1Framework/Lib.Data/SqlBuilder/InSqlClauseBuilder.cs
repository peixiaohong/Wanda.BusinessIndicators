using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{
    /// <summary>
    /// In操作的Sql语句构造器
    /// </summary>
    [Serializable]
    public class InSqlClauseBuilder
    {
        public InSqlClauseBuilder()
        {
            throw new NotSupportedException("");
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Lib.Data 
//{
//    /// <summary>
//    /// In操作的Sql语句构造器
//    /// </summary>
//    [Serializable]
//    public class InSqlClauseBuilder : SqlClauseBuilderBase, IConnectiveSqlClause
//    {
//        private string _DataField = string.Empty;

//        /// <summary>
//        /// 构造方法
//        /// </summary>
//        public InSqlClauseBuilder()
//        {
//        }

//        /// <summary>
//        /// 数据字段
//        /// </summary>
//        /// <param name="dataField"></param>
//        public InSqlClauseBuilder(string dataField)
//        {
//            this._DataField = dataField;
//        }

//        /// <summary>
//        /// 对应的数据字段，如果不为空，那么构造的时候，会自动带上字段名
//        /// </summary>
//        public string DataField
//        {
//            get { return this._DataField; }
//            set { this._DataField = value; }
//        }

//        /// <summary>
//        /// 添加一个构造项
//        /// </summary>SqlCaluseBuilderBase
//        /// <typeparam name="T">数据的类型</typeparam>
//        /// <param name="data">In操作的数据</param>
//        public void AppendItem<T>(params T[] data)
//        {
//            AppendItem<T>(false, data);
//        }

//        /// <summary>
//        /// 添加一个构造项
//        /// </summary>
//        /// <typeparam name="T">数据的类型</typeparam>
//        /// <param name="data">In操作的数据</param>
//        /// <param name="isExpression">是否是表达式</param>
//        public void AppendItem<T>(bool isExpression, params T[] data)
//        {
//            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

//            for (int i = 0; i < data.Length; i++)
//            {
//                SqlCaluseBuilderItemInOperator item = new SqlCaluseBuilderItemInOperator();

//                item.IsExpression = isExpression;
//                item.Data = data[i];

//                List.Add(item);
//            }
//        }

//        /// <summary>
//        /// 生成Sql语句（格式为：数据1,数据2，...）
//        /// </summary>
//        /// <param name="builder">Sql语句构造器</param>
//        /// <returns>生成Sql语句（格式为：数据1,数据2，...）</returns>
//        public override string ToSqlString(ISqlBuilder builder)
//        {
//            string result = string.Empty;

//            if (this._DataField.IsNotEmpty())
//                result = ToSqlStringWithInOperator(builder);
//            else
//                result = InnerToSqlString(builder);

//            return result;
//        }

//        /// <summary>
//        /// 生成Sql语句，加上In操作符。如果没有数据，In操作符也不生成
//        /// </summary>
//        /// <param name="builder">Sql语句构造器</param>
//        /// <returns>构造的In语句</returns>
//        public string ToSqlStringWithInOperator(ISqlBuilder builder)
//        {
//            string result = string.Empty;
//            string inClause = InnerToSqlString(builder);

//            if (string.IsNullOrEmpty(inClause) == false)
//            {
//                if (this._DataField.IsNotEmpty())
//                    result = string.Format("{0} IN ({1})", this._DataField, inClause);
//                else
//                    result = string.Format("IN ({0})", inClause);
//            }

//            return result;
//        }

//        private string InnerToSqlString(ISqlBuilder builder)
//        {
//            StringBuilder strB = new StringBuilder(256);

//            for (int i = 0; i < List.Count; i++)
//            {
//                if (strB.Length > 0)
//                    strB.Append(", ");

//                strB.Append(((SqlCaluseBuilderItemInOperator)List[i]).GetDataDesp(builder));
//            }

//            return strB.ToString();
//        }

//        /// <summary>
//        /// 是否为空
//        /// </summary>
//        public bool IsEmpty
//        {
//            get
//            {
//                return this.Count == 0;
//            }
//        }

//        public LogicOperatorDefine LogicOperator
//        {
//            get
//            {
//                throw new NotSupportedException("LogicOperator is not supported!");
//            }
//            set
//            {
//                throw new NotSupportedException("LogicOperator is not supported!");
//            }
//        }
//    }
//}
