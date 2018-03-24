using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.Lib.Data.AppBase
{
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
        /// 分页的页面数据行数
        /// </summary>
        public int PageSize { set; get; }

        /// <summary>
        /// 分页序号，基于1
        /// </summary>
        public int PageIndex { set; get; }


        /// <summary>
        /// 行序号， 基于0
        /// </summary>
        public int RowIndex { get { return (PageIndex > 0) ? ((PageIndex - 1) * PageSize) : 0; } }

        /// <summary>
        /// 排序字段接Orderby
        /// </summary>

        private string _sortKey = null;

        public string SortKey
        {
            get { return _sortKey; }
            set
            {
                if (value == null)
                {
                    _sortKey = "";
                }
                _sortKey = SqlTextHelper.SafeQuote(value); //要做防注入
            }
        }
    }

    [Serializable]
    public class PagenationDataFilter : PagenationFilter, IDataFilter
    {
        private Dictionary<string, FieldAndOperator> _filterDict = null;
        public Dictionary<string, FieldAndOperator> FilterDict
        {
            get
            {
                if (_filterDict == null)
                {
                    _filterDict = new Dictionary<string, FieldAndOperator>();
                }
                return _filterDict;
            }
        }


    }
}
