using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators
{
    public interface ITargetEvaluation
    {
        object Calculation(object obj);
    }
}
