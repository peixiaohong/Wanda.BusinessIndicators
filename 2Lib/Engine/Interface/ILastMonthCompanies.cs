using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    public interface ILastMonthCompanies
    {
        List<Guid> GetLastMonthCompaniesID(object Lastlist);
    }


}
