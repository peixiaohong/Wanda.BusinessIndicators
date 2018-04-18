using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Engine
{
    /// <summary>
    /// 明细项
    /// </summary>
    public class ReportInstanceDetailBuilder : BaseBuilder<IReportInstanceDetail>
    {
        private static ReportInstanceDetailBuilder _Instance = new ReportInstanceDetailBuilder();

        public static ReportInstanceDetailBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
    /// <summary>
    /// 经营报告明细项
    /// </summary>
    public class ReportInstanceManageDetailBuilder : BaseBuilder<IReportInstanceManageDetail>
    {
        private static ReportInstanceManageDetailBuilder _Instance = new ReportInstanceManageDetailBuilder();

        public static ReportInstanceManageDetailBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
    /// <summary>
    /// 月度报告
    /// </summary>
    public class ReportInstanceSummaryBuilder : BaseBuilder<IReportInstanceSummary>
    {
        private static ReportInstanceSummaryBuilder _Instance = new ReportInstanceSummaryBuilder();

        public static ReportInstanceSummaryBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }

    /// <summary>
    /// 累计未完成
    /// </summary>
    public class ReportInstanceMisstargetBuilder : BaseBuilder<IReportInstanceMissTarget>
    {
        private static ReportInstanceMisstargetBuilder _Instance = new ReportInstanceMisstargetBuilder();

        public static ReportInstanceMisstargetBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }


    /// <summary>
    /// 当月未完成
    /// </summary>
    public class ReportInstanceCurrentMisstargetBuilder : BaseBuilder<IReportInstanceCurrentMissTarget>
    {
        private static ReportInstanceCurrentMisstargetBuilder _Instance = new ReportInstanceCurrentMisstargetBuilder();

        public static ReportInstanceCurrentMisstargetBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }


    /// <summary>
    /// 补回情况
    /// </summary>
    public class ReportInstanceReturnBuilder : BaseBuilder<IReportInstanceReturn>
    {
        private static ReportInstanceReturnBuilder _Instance = new ReportInstanceReturnBuilder();

        public static ReportInstanceReturnBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
}
