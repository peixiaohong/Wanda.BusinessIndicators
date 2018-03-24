using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Engine
{
    public interface IGetTemplate
    {
        string GetTemplate(Guid SystemID);
    }
}
