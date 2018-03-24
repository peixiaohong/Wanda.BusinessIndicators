using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Engine
{
    public class TargetEvaluationBuilder : BaseBuilder<ITargetEvaluation>
    {
        private static TargetEvaluationBuilder _Instance = new TargetEvaluationBuilder();

        public static TargetEvaluationBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }

    }
}
