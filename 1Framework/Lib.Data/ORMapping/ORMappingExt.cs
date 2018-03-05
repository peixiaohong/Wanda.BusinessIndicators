using System;
using System.Data.Common;
using Lib.Core;
using System.Collections.Generic;

namespace Lib.Data
{
    public static partial class ORMapping
    {
        private delegate DbParameter DoSqlClauseBuilderWithParams<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph);



        public static string GetInsertSql<T>(T graph, ISqlBuilder builder, ref List<DbParameter> dbParams, params string[] ignorProperties)
        {
            ORMappingItemCollection mapping = InnerGetMappingInfoByObject(graph);

            return GetInsertSql<T>(graph, mapping, builder, ref dbParams, ignorProperties);
        }

        public static string GetInsertSql<T>(T graph, ORMappingItemCollection mapping, ISqlBuilder builder, ref List<DbParameter> dbParams, params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");
            ExceptionHelper.FalseThrow<ArgumentNullException>(builder != null, "builder");

            InsertSqlClauseBuilder insertBuilder = GetInsertSqlClauseBuilder(graph, mapping, ref dbParams, ignorProperties);

            return string.Format("INSERT INTO {0} {1}", mapping.TableName, insertBuilder.ToSqlString(builder));
        }

        public static InsertSqlClauseBuilder GetInsertSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, ref List<DbParameter> dbParams, params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
            ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

            InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

            FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Insert,
                new DoSqlClauseBuilderWithParams<T>(DoInsertUpdateSqlWithParamBuilder<T>), ignorProperties);

            return builder;
        }

        private static void FillSqlClauseBuilder<T>(
               SqlClauseBuilderIUW builder,
               T graph,
               ORMappingItemCollection mapping,
               ClauseBindingFlags bindingFlags,
               DoSqlClauseBuilderWithParams<T> builderDelegate,
               params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            foreach (ORMappingItem item in mapping)
            {
                if (Array.Exists<string>(ignorProperties,
                                            delegate(string target)
                                            {
                                                return (string.Compare(target, item.PropertyName, true) == 0);
                                            }) == false)
                {
                    if ((item.BindingFlags & bindingFlags) != ClauseBindingFlags.None)
                        builderDelegate(builder, item, graph);
                }
            }
        }

        private static DbParameter DoInsertUpdateSqlWithParamBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
        {
            DbParameter param = TSqlBuilder.Instance.CreateDbParameter();

            if (item.IsIdentity == false)
            {
                object data = GetValueFromObject(item, graph);

                if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) &&
                        string.IsNullOrEmpty(item.DefaultExpression) == false)
                    builder.AppendItem(item.DataFieldName, item.DefaultExpression, SqlClauseBuilderBase.EqualTo, true);
                else
                    builder.AppendItem(item.DataFieldName, data);
            }

            return param;
        }
    }
}
