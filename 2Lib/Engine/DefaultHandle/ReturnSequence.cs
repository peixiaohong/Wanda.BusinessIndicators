using Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Engine
{
    public class ReturnSequence : ISequence
    {
        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> list, string Paramters)
        {
            List<MonthlyReportDetail> res = new List<MonthlyReportDetail>();
            List<string> TargetIDs = JsonHelper.Deserialize<List<string>>(Paramters);
            if (TargetIDs.Count <= 0)
            {
                return list;
            }

            int Index = TargetIDs.Count;
            MonthlyReportCompare defaultCompare = new MonthlyReportCompare();
            for (int i = 0; i < TargetIDs.Count; i++)
            {
                Guid TargetID = TargetIDs[i].ToGuid();
                Dictionary<Guid, decimal> CompanyIDs = new Dictionary<Guid, decimal>();
                list.FindAll(P => P.TargetID == TargetID && P.NAccumulativeDifference < 0).ForEach(C =>
                {
                    if (!CompanyIDs.ContainsKey(C.CompanyID))
                    {
                        CompanyIDs.Add(C.CompanyID, Math.Abs(C.NAccumulativeDifference));
                    }
                });
                List<Guid> Companies = new List<Guid>();
                CompanyIDs.OrderByDescending(P => P.Value).ForEach(C =>
                {
                    Companies.Add(C.Key);
                });

                for (int k = 0; k < Companies.Count; k++)
                {
                    List<MonthlyReportDetail> t = list.FindAll(R => R.CompanyID == Companies[k]);
                    t.Sort(defaultCompare);
                    res.AddRange(t);
                    list.RemoveAll(R => R.CompanyID == Companies[k]);
                }
                if (list.Count <= 0)
                    break;
            }

            //已补回,差值为正的情况处理
            if (list.Count > 0)
            {
                for (int i = 0; i < TargetIDs.Count; i++)
                {
                    Guid TargetID = TargetIDs[i].ToGuid();
                    Dictionary<Guid, decimal> CompanyIDs = new Dictionary<Guid, decimal>();
                    list.FindAll(P => P.TargetID == TargetID).ForEach(C =>
                    {
                        if (!CompanyIDs.ContainsKey(C.CompanyID))
                        {
                            CompanyIDs.Add(C.CompanyID, Math.Abs(C.NAccumulativeDifference));
                        }
                    });
                    List<Guid> Companies = new List<Guid>();
                    CompanyIDs.OrderByDescending(P => P.Value).ForEach(C =>
                    {
                        Companies.Add(C.Key);
                    });

                    for (int k = 0; k < Companies.Count; k++)
                    {
                        List<MonthlyReportDetail> t = list.FindAll(R => R.CompanyID == Companies[k]);
                        t.Sort(defaultCompare);
                        res.AddRange(t);
                        list.RemoveAll(R => R.CompanyID == Companies[k]);
                    }
                    if (list.Count <= 0)
                        break;
                }
            }

            return res;
        }

    }

    public class MonthlyReportCompare : IComparer<MonthlyReportDetail>
    {
        public int Compare(MonthlyReportDetail x, MonthlyReportDetail y)
        {
            C_Target tx = StaticResource.Instance.TargetList[x.SystemID].Find(T => T.ID == x.TargetID);
            C_Target ty = StaticResource.Instance.TargetList[y.SystemID].Find(T => T.ID == y.TargetID);
            if (tx == null && ty == null)
            {
                return 0;
            }
            else if (tx == null)
            {
                return 1;
            }
            else if (ty == null)
            {
                return -1;
            }
            else
            {
                if (tx.Sequence > ty.Sequence)
                    return 1;
                if (tx.Sequence == ty.Sequence)
                    return 0;
                else
                    return -1;
            }
        }
    }
}
