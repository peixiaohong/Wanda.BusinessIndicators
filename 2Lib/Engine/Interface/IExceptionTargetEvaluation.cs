﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators
{
    public interface IExceptionTargetEvaluation
    {
        B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true);

        bool Calculation(Guid SystemID,Guid TargetID,Guid CompanyID, bool WithCounter = true);
    }

}
