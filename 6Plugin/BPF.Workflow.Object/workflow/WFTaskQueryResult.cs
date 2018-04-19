using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 待办已办查询返回结果
    /// </summary>
    [Serializable]
    public class WFTaskQueryResult
    {
        /// <summary>
        /// 待办/已办列表
        /// </summary>
        public List<UserTask> TaskList { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 待办/已办查询条件
    /// </summary>
    [Serializable]
    public class WFTaskQueryFilter : PagenationFilter
    {
        /// <summary>
        /// 标题Like查询
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 流程编码（完全匹配）
        /// </summary>
        public string FlowCode { get; set; }
        /// <summary>
        /// 发送人登陆账号（完全匹配），名字（模糊匹配）
        /// </summary>
        public string RequestUser { get; set; }
        /// <summary>
        /// 审批人登陆账号（完全匹配），名字（模糊匹配）
        /// </summary>
        public string TaskUser { get; set; }
        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime? RequestStart { get; set; }
        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime? RequestEnd { get; set; }
        /// <summary>
        /// 完成开始时间
        /// </summary>
        public DateTime? FinishStart { get; set; }
        /// <summary>
        /// 完成结束时间
        /// </summary>
        public DateTime? FinishEnd { get; set; }
    }

    /// <summary>
    /// 带分页的查询
    /// </summary>
    [Serializable]
    public class PagenationFilter
    {
        public PagenationFilter()
        {
            PageIndex = 1; //默认为1
        }
        /// <summary>
        /// 分页的页面数据行数，默认为0(如果PageSize和PageIndex都为0，则认为是查询所有数据)
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 分页序号，基于1，默认为1(如果PageSize和PageIndex都为0，则认为是查询所有数据)
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// 行序号， 基于0
        /// </summary>
        public int RowIndex { get { return (PageIndex - 1) * PageSize; } }
    }
}
