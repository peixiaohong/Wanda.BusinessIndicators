using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators
{
    public interface ITargetEvaluation
    {
        object Calculation(object obj);
    }
}
