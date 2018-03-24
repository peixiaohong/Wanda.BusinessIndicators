using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Lib.Xml;

namespace LJTH.BusinessIndicators.Engine
{
    public class DefaultExceptionTargetEvaluation : IExceptionTargetEvaluation
    {
        public B_MonthlyReportDetail Calculation(B_MonthlyReportDetail RptDetail, bool WithCounter = true)
        {
            C_Target _CTarget = new C_Target();
            if (RptDetail!=null)
            {
                _CTarget = StaticResource.Instance.GetTargetList(RptDetail.SystemID, RptDetail.CreateTime).Where(p => p.ID == RptDetail.TargetID).FirstOrDefault();

            }
             //判断当前公司和指标是否为异常指标。
                List<C_ExceptionTarget> listExceptionTarget = StaticResource.Instance.GetExceptionTargetList(RptDetail.CompanyID, RptDetail.TargetID);
                if (listExceptionTarget.Count > 0)
                {
                    if (listExceptionTarget.FirstOrDefault().ExceptionType == (int)ExceptionTargetType.HaveDeTailNONeedEvaluation)
                    {
                        RptDetail.IsMissTarget = false;
                        RptDetail.Counter = 0;
                        RptDetail.IsMissTargetCurrent = false;
                        RptDetail.ReturnType = 0; //补回状态是0 ,设置为特殊的异常考核实体是，它的未完成，补回都需要设置为0

                    }
                }
           
            return RptDetail;
        }

        public bool Calculation(Guid SystemID,Guid TargetID, Guid CompanyID, bool WithCounter = true)
        {
            bool isException = false;
            C_Target _CTarget = StaticResource.Instance.GetTargetList(SystemID,DateTime.Now).Where(p => p.ID == TargetID).FirstOrDefault();
            //判断是否为明细指标
            List<C_ExceptionTarget> listExceptionTarget = new List<C_ExceptionTarget>();
            if (_CTarget!=null)
            {
                listExceptionTarget=StaticResource.Instance.GetExceptionTargetList(CompanyID,TargetID);
            }
            if(listExceptionTarget.Count>0)
            {
                if (_CTarget.HaveDetail == true)
                {
                    //判断当前公司和指标是否为异常指标。（明细指标不上报）

                    if (listExceptionTarget.FirstOrDefault().ExceptionType == (int)ExceptionTargetType.HaveDetailNONeedReport)
                    {
                        isException = true;
                    }
                }
                else
                {
                    //判断当前公司和指标是否为异常指标。（非明细指标不上报）
                    if (listExceptionTarget.FirstOrDefault().ExceptionType == (int)ExceptionTargetType.HavenotDetailNONeedEvaluation)
                    {
                        isException = true;
                    }
                }
            }
            return isException;
        }
    }
}
