using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Engine;

namespace Wanda.BusinessIndicators.Engine
{
    public class BizContextBuilder : BaseBuilder<IBizContext>
    {
        private static BizContextBuilder _Instance = new BizContextBuilder();

        public static BizContextBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }

    /// <summary>
    /// 获取上个月的系统公司
    /// </summary>
    public class LastMonthCompaniesBuilder : BaseBuilder<ILastMonthCompanies>
    {
        private static LastMonthCompaniesBuilder _Instance = new LastMonthCompaniesBuilder();

        public static LastMonthCompaniesBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }


    /// <summary>
    /// 获取相关系统
    /// </summary>
    public class SystemBuilder : BaseBuilder<ISystem>
    {
        private static SystemBuilder _Instance = new SystemBuilder();

        public static SystemBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

    /// <summary>
    /// 获取指标分解
    /// </summary>
    public class TargetPlanBuilder : BaseBuilder<ITargetPlanInstance>
    {
        private static TargetPlanBuilder _Instance = new TargetPlanBuilder();

        public static TargetPlanBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

    /// <summary>
    /// 获取明细项的指标数
    /// </summary>
    public class SystemTargetCountBuilder : BaseBuilder<ISystemTargetCount>
    {
        private static SystemTargetCountBuilder _Instance = new SystemTargetCountBuilder();

        public static SystemTargetCountBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

    
    
    public class TemplateBuilder : BaseBuilder<IGetTemplate>
    {
        private static TemplateBuilder _Instance = new TemplateBuilder();

        public static TemplateBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }

    public class SequenceBuilder : BaseBuilder<ISequence>
    {
        private static SequenceBuilder _Instance = new SequenceBuilder();

        public static SequenceBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }

    public class CalculationEvaluationBuilder : BaseBuilder<ICalculationEvaluation>
    {
        private static CalculationEvaluationBuilder _Instance = new CalculationEvaluationBuilder();

        public static CalculationEvaluationBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }


    public class ResetCalculationEvationBuilder : BaseBuilder<IResetCalculationEvaluation>
    {
        private static ResetCalculationEvationBuilder _Instance = new ResetCalculationEvationBuilder();

        public static ResetCalculationEvationBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }


    public class ExceptionTargetEvaluationBuilder : BaseBuilder<IExceptionTargetEvaluation>
    {
        private static ExceptionTargetEvaluationBuilder _Instance = new ExceptionTargetEvaluationBuilder();

        public static ExceptionTargetEvaluationBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }



    public class CompanyExceptionTargetBuilder : BaseBuilder<ICompanyExceptionTarget>
    {
        private static CompanyExceptionTargetBuilder _Instance = new CompanyExceptionTargetBuilder();

        public static CompanyExceptionTargetBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }


}
