using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Filter;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VBizProcessDraftAdapter : LwfBaseCompositionAdapterT<VBizProcess>
    {
        public static VBizProcessDraftAdapter Instance = new VBizProcessDraftAdapter();


        internal PartlyCollection<BBizProcess> LoadList(WorkFlowFilter filter)
        {
            StringBuilder strSql = new StringBuilder();

            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            where.AppendItem("Isdeleted", 0);
            where.AppendItem("ProcessType",DBNull.Value);
            //string sql =new VTodoWorkAndUser().TodoViewSql;

            QueryCondition qc = new QueryCondition(
                filter.RowIndex,
                filter.PageSize,
                "*",
                ORMapping.GetTableName(typeof(BBizProcess)),
                " Createtime desc",
                where.ToSqlString(TSqlBuilder.Instance)
                );
              
            PartlyCollection<VBizProcess> tuple = GetPageSplitedCollection(qc);
            PartlyCollection<BBizProcess> result = new PartlyCollection<BBizProcess>();
            foreach (VBizProcess vBizProcess in tuple)
            {
                result.Add(new BBizProcess
                    {
                        TaskID = vBizProcess.TaskID,
                        ProcessType = vBizProcess.ProcessType,
                        Title = vBizProcess.Title,
                        ParentID = vBizProcess.ParentID,
                        Status = vBizProcess.Status,
                        IsCurrentVersion = vBizProcess.IsCurrentVersion,
                        AlarmAction = vBizProcess.AlarmAction,
                        AlarmPlanSplitDate = vBizProcess.AlarmPlanSplitDate
                    });
            }
            return result;
        }

          
    }
}
