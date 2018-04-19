using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 待办/已办SDK
    /// </summary>
    public class WFTaskSDK
    {
        /// <summary>
        /// 查询待办
        /// </summary>
        /// <param name="condition">查询条件（PageSize和PageIndex同时为0表示查询所有）</param>
        /// <returns>待办查询结果</returns>
        public static WFTaskQueryResult QueryToDo(WFTaskQueryFilter condition)
        {
            return WFClientProcess.QureyTask(condition, AppSettingInfo.CONST_OtherMethod_QueryToDo);
        }
        /// <summary>
        /// 查询已办
        /// </summary>
        /// <param name="condition">查询条件（PageSize和PageIndex同时为0表示查询所有）</param>
        /// <returns>已办查询结果</returns>
        public static WFTaskQueryResult QueryDone(WFTaskQueryFilter condition)
        {
            return WFClientProcess.QureyTask(condition, AppSettingInfo.CONST_OtherMethod_QueryDone);
        }
        /// <summary>
        /// 根据业务ID查询待办数据
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>待办查询结果</returns>
        public static WFTaskQueryResult QueryToDoByBusinessID(string businessID)
        {
            return WFClientProcess.QueryToDoByBusinessID(businessID);
        }
        /// <summary>
        /// 根据业务ID查询已办数据
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>已办查询结果</returns>
        public static WFTaskQueryResult QueryDoneByBusinessID(string businessID)
        {
            return WFClientProcess.QueryDoneByBusinessID(businessID);
        }
        /// <summary>
        /// 查询已办（同一流程同一个人只会查询到一条最新的已办信息）
        /// </summary>
        /// <param name="condition">查询条件（PageSize和PageIndex同时为0表示查询所有）</param>
        /// <returns>已办查询结果(同一流程同一个人只会查询到一条最新的已办信息)</returns>
        public static WFTaskQueryResult QueryDoneDistinct(WFTaskQueryFilter condition)
        {
            return WFClientProcess.QueryDoneDistinct(condition);
        }
        /// <summary>
        /// 根据业务ID查询已办数据（同一流程同一个人只会查询到一条最新的已办信息）
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>已办查询结果(同一流程同一个人只会查询到一条最新的已办信息)</returns>
        public static WFTaskQueryResult QueryDoneDistinctByBusinessID(string businessID)
        {
            return WFClientProcess.QueryDoneDistinctByBusinessID(businessID);
        }
    }
}
