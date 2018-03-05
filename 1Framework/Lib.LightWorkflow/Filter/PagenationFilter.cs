using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanda.Lib.LightWorkflow.Filter
{
    /// <summary>
    /// 带分页的查询
    /// </summary>
    public class PagenationFilter
    {

        public PagenationFilter()
        {
            PageIndex = 1; //默认为1
        }
        /// <summary>
        /// 分页的页面数据行数
        /// </summary>
        public int PageSize { set; get; }

        /// <summary>
        /// 分页序号，基于1
        /// </summary>
        public int PageIndex { set; get; }


        /// <summary>
        /// 行序号， 基于1
        /// </summary>
        public int RowIndex { get { return (PageIndex - 1) * PageSize; } }

    }
}
