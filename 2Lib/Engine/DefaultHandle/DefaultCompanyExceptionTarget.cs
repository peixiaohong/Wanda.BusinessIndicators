using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Engine
{
    /// <summary>
    /// 这里通过公司获取，该公司所在指标的异常状态==上报考核
    /// </summary>
    public class DefaultCompanyExceptionTarget : ICompanyExceptionTarget
    {


        public int GetCompanyExceptionTarget(C_Company company)
        {
          
            DateTime CurrentDate =  DateTime.Now;
            C_System SysModel = StaticResource.Instance[company.SystemID, CurrentDate];

            int result = 0;

            //代表是在项目系统里
            if (SysModel.Category == 2)
            {
                if (company.CompanyProperty1 == "尾盘")
                {
                    result = 1;
                }
                else
                {
                    result= 0;
                }
            }
            else
            {
                //上报不考核
                if (company.CompanyProperty1 == "筹备门店")
                {
                    result = 1;
                }
                else 
                {
                    result = 0;
                }
            }

            return result;
        }
    }



}
