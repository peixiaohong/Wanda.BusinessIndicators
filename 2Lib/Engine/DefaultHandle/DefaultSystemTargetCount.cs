using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.ViewModel;

namespace LJTH.BusinessIndicators.Engine
{
    public class DefaultSystemTargetCount : ISystemTargetCount
    {

        public object GetSystemTargetCount(List<MonthlyReportDetail> MissTargetList, List<MonthlyReportDetail> LastMissTargetList, List<MonthlyReportDetail> SingleMissTargetList, List<MonthlyReportDetail> DoubleMissTargetList, List<MonthlyReportDetail> FilterMissTargetList)
        {
            MissTargetDataSource model = new MissTargetDataSource();
            model.MissTargetDataSource1 = MissTargetList;
            model.MissTargetDataSource2 = LastMissTargetList;
            model.MissTargetDataSource3 = DoubleMissTargetList;
            model.MissTargetDataSource4 = SingleMissTargetList;
            model.MissTargetDataSource5= FilterMissTargetList;

            return model;
        }
    }


    /// <summary>
    /// 商管系统这里不做处理
    /// </summary>
    public class SystemTargetCount_SG : ISystemTargetCount
    {

        public object GetSystemTargetCount(List<MonthlyReportDetail> MissTargetList, List<MonthlyReportDetail> LastMissTargetList, List<MonthlyReportDetail> SingleMissTargetList, List<MonthlyReportDetail> DoubleMissTargetList, List<MonthlyReportDetail> FilterMissTargetList)
        {
            MissTargetDataSource model = new MissTargetDataSource();
            //model.MissTargetDataSource1 = MissTargetList;
            //model.MissTargetDataSource2 = LastMissTargetList;
            model.MissTargetDataSource3 = MissTargetList;
            model.MissTargetDataSource4 = LastMissTargetList;
            model.MissTargetDataSource5 = MissTargetList;
            return model;
        }
    }

}
