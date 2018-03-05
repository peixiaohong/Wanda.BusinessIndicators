using Lib.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Filter;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal class VPInstanceAndApprovalLogAdapter : LwfBaseCompositionAdapterT<VPInstanceAndApprovalLog>
    {
        //add czq 2013-06-17
        public static VPInstanceAndApprovalLogAdapter Instance = new VPInstanceAndApprovalLogAdapter();


        /// <summary>
        /// 根据条件查询我的已办
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        internal PartlyCollection<VPInstanceAndApprovalLog> LoadList(WorkFlowFilter filter)
        {
            return base.GetList(filter, filter.SortKey);
        }

    }
}
