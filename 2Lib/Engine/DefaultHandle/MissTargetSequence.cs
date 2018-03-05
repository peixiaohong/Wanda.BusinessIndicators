using Lib.Core;
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
    public class MissTargetSequence : ISequence
    {

        //Update 2015-5-22  添加新的规则
        //1 ： 到期未补回
        //2 :  补回中->预计无法按期补回
        //3 :  本月新增/补回中 ->预计年内无法补回
        //4 :  其它的参照原来的排序
        //Update End

        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> RptDetailList, string Paramters)
        {
            List<Guid> i = new List<Guid>();
            List<Guid> ii_1 = new List<Guid>();
            List<Guid> ii_2 = new List<Guid>();
            List<Guid> ii_3 = new List<Guid>();
            List<Guid> ii_4 = new List<Guid>();
            List<Guid> iii = new List<Guid>();

            //i.	先排“到期未补回”的（任何一个指标存“在到期未补回”都在此类）
            List<MonthlyReportDetail> NotReturn_i=new List<MonthlyReportDetail>();
            NotReturn_i.AddRange(RptDetailList.FindAll(R=>R.ReturnType==(int)Common.EnumReturnType.NotReturn));
            NotReturn_i.ForEach(P=>{
                if(!i.Contains(P.CompanyID)){
                    i.Add(P.CompanyID);
                }
            } );

            //ii.	再排“预计无法按期补回”及“预计年度内无法回补”的（任何一个指标存“预计无法按期补回”或“预计年度内无法回补”都在此类）
            List<MonthlyReportDetail> CanNotReturn_ii_1=new List<MonthlyReportDetail>();
            CanNotReturn_ii_1.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.UnableReturnByMonth)));
            CanNotReturn_ii_1.ForEach(P =>
            {
                if (!ii_1.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_1.Add(P.CompanyID);
                }
            });
            List<MonthlyReportDetail> CanNotReturn_ii_2 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_2.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.UnableReturnByYear)));
            CanNotReturn_ii_2.ForEach(P =>
            {
                if (!ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_2.Add(P.CompanyID);
                }
            });


            //ii.   这里重新排序，排的是“补回中->预计无法按期补回”,(任何一个指标存,都算是)   Update 2015-5-22
            List<MonthlyReportDetail> CanNotReturn_ii_3 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_3.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.Returning
                                       && R.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(Common.EnumReturnType_Sub),
                                       (int)Common.EnumReturnType_Sub.Sub_UnableReturnByMonth) ) ) );
            CanNotReturn_ii_3.ForEach(P => {

                if (!ii_3.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_3.Add(P.CompanyID);
                }
            });


            //ii.  这里重新排序,排的是“本月新增/补回中 ->预计年内无法补回”,(任何一个指标存,都算是)
            List<MonthlyReportDetail> CanNotReturn_ii_4 = new List<MonthlyReportDetail>();
           // CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => (R.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(Common.EnumReturnType_Sub), (int)Common.EnumReturnType_Sub.Sub_UnableReturnByMonth))));

            CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => (  (R.ReturnType_Sub == "T4" 
                && R.ReturnType == (int)Common.EnumReturnType.New) || 
                (R.ReturnType == (int)Common.EnumReturnType.Returning 
                && R.ReturnType_Sub =="T4" )  )));

            CanNotReturn_ii_4.ForEach(P =>
            {
                if (!ii_4.Contains(P.CompanyID) && !ii_3.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_4.Add(P.CompanyID);
                }
            });

            //Update 2015-5-22 End 


            //iii.	最后排其他类型的（包括“补回中”及“本月新增”）
            List<MonthlyReportDetail> Ohter_iii=new List<MonthlyReportDetail>();
            Ohter_iii.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.New || R.ReturnType == (int)Common.EnumReturnType.Returning)));
            Ohter_iii.ForEach(P =>
            {
                if (!iii.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID) &&!ii_4.Contains(P.CompanyID) && !ii_3.Contains(P.CompanyID) )
                {
                    iii.Add(P.CompanyID);
                }
            });



            //iv.	在每一类中，按净利润差额的绝对值由大到小排列
            NotReturn_i = new List<MonthlyReportDetail>();
            CanNotReturn_ii_1 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_2 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_3 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_4 = new List<MonthlyReportDetail>();
            Ohter_iii = new List<MonthlyReportDetail>();
            NotReturn_i.AddRange(RptDetailList.FindAll(R => i.Contains(R.CompanyID)));
            CanNotReturn_ii_1.AddRange(RptDetailList.FindAll(R => ii_1.Contains(R.CompanyID)));
            CanNotReturn_ii_2.AddRange(RptDetailList.FindAll(R => ii_2.Contains(R.CompanyID)));
            CanNotReturn_ii_3.AddRange(RptDetailList.FindAll(R => ii_3.Contains(R.CompanyID)));
            CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => ii_4.Contains(R.CompanyID)));

            Ohter_iii.AddRange(RptDetailList.FindAll(R => iii.Contains(R.CompanyID)));

            List<MonthlyReportDetail> notMissed = new List<MonthlyReportDetail>();

            notMissed.AddRange(
                RptDetailList.FindAll(R => !NotReturn_i.Exists(NI => NI.ID == R.ID) && 
                !CanNotReturn_ii_1.Exists(CII1 => CII1.ID == R.ID) && 
                !CanNotReturn_ii_2.Exists(CII2 => CII2.ID == R.ID) &&
                !CanNotReturn_ii_3.Exists(CII3 => CII3.ID == R.ID) &&
                !CanNotReturn_ii_4.Exists(CII4 => CII4.ID == R.ID) && 
                !Ohter_iii.Exists(OIII => OIII.ID == R.ID)
                )  );

            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();

            result.AddRange(Sort(NotReturn_i,Paramters));
            result.AddRange(Sort(CanNotReturn_ii_1, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_2, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_3, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_4, Paramters));

            result.AddRange(Sort(Ohter_iii, Paramters));
            result.AddRange(Sort(notMissed, Paramters));
            return result;
        }

     


        //iv.	在每一类中，按净利润差额的绝对值由大到小排列
        //iv.	在每一类中，按净利润差额的绝对值由大到小排列
        //iv.	在每一类中，先按净收入差额的绝对值由大到小排列，如果净收入指标完成（即差额是正数），则按综合收入差额的绝对值由大到小排列
        //已补回：按净利润差额（正数）由大到小排列
        //iv.	在每一类中按指标差额的绝对值由大到小排列
        //已补回：按指标差额（正数）由大到小排列
        List<MonthlyReportDetail> Sort(List<MonthlyReportDetail> list, string Paramters)
        {
            List<MonthlyReportDetail> res = new List<MonthlyReportDetail>();
            List<string> TargetIDs = JsonHelper.Deserialize<List<string>>(Paramters);
            if (TargetIDs.Count <= 0)
            {
                return list;
            }

            int Index = TargetIDs.Count;
            MonthlyReportCompare defaultCompare=new MonthlyReportCompare();
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
                CompanyIDs.OrderByDescending(P => P.Value).ForEach(C => {
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


    public class CurrentMissTargetSequence : ISequence
    {

        //Update 2015-5-22  添加新的规则
        //1 ： 到期未补回
        //2 :  补回中->预计无法按期补回
        //3 :  本月新增/补回中 ->预计年内无法补回
        //4 :  其它的参照原来的排序
        //Update End

        public List<MonthlyReportDetail> GetSequence(List<MonthlyReportDetail> RptDetailList, string Paramters)
        {
            List<Guid> i = new List<Guid>();
            List<Guid> ii_1 = new List<Guid>();
            List<Guid> ii_2 = new List<Guid>();
            List<Guid> ii_3 = new List<Guid>();
            List<Guid> ii_4 = new List<Guid>();
            List<Guid> iii = new List<Guid>();

            //i.	先排“到期未补回”的（任何一个指标存“在到期未补回”都在此类）
            List<MonthlyReportDetail> NotReturn_i = new List<MonthlyReportDetail>();
            NotReturn_i.AddRange(RptDetailList.FindAll(R => R.ReturnType == (int)Common.EnumReturnType.NotReturn));
            NotReturn_i.ForEach(P => {
                if (!i.Contains(P.CompanyID))
                {
                    i.Add(P.CompanyID);
                }
            });

            //ii.	再排“预计无法按期补回”及“预计年度内无法回补”的（任何一个指标存“预计无法按期补回”或“预计年度内无法回补”都在此类）
            List<MonthlyReportDetail> CanNotReturn_ii_1 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_1.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.UnableReturnByMonth)));
            CanNotReturn_ii_1.ForEach(P =>
            {
                if (!ii_1.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_1.Add(P.CompanyID);
                }
            });
            List<MonthlyReportDetail> CanNotReturn_ii_2 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_2.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.UnableReturnByYear)));
            CanNotReturn_ii_2.ForEach(P =>
            {
                if (!ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_2.Add(P.CompanyID);
                }
            });


            //ii.   这里重新排序，排的是“补回中->预计无法按期补回”,(任何一个指标存,都算是)   Update 2015-5-22
            List<MonthlyReportDetail> CanNotReturn_ii_3 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_3.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.Returning
                                       && R.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(Common.EnumReturnType_Sub),
                                       (int)Common.EnumReturnType_Sub.Sub_UnableReturnByMonth))));
            CanNotReturn_ii_3.ForEach(P => {

                if (!ii_3.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_3.Add(P.CompanyID);
                }
            });


            //ii.  这里重新排序,排的是“本月新增/补回中 ->预计年内无法补回”,(任何一个指标存,都算是)
            List<MonthlyReportDetail> CanNotReturn_ii_4 = new List<MonthlyReportDetail>();
            // CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => (R.ReturnType_Sub == EnumHelper.GetEnumDescription(typeof(Common.EnumReturnType_Sub), (int)Common.EnumReturnType_Sub.Sub_UnableReturnByMonth))));

            CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => ((R.ReturnType_Sub == "T4"
                && R.ReturnType == (int)Common.EnumReturnType.New) ||
                (R.ReturnType == (int)Common.EnumReturnType.Returning
                && R.ReturnType_Sub == "T4"))));

            CanNotReturn_ii_4.ForEach(P =>
            {
                if (!ii_4.Contains(P.CompanyID) && !ii_3.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID))
                {
                    ii_4.Add(P.CompanyID);
                }
            });

            //Update 2015-5-22 End 


            //iii.	最后排其他类型的（包括“补回中”及“本月新增”）
            List<MonthlyReportDetail> Ohter_iii = new List<MonthlyReportDetail>();
            Ohter_iii.AddRange(RptDetailList.FindAll(R => (R.ReturnType == (int)Common.EnumReturnType.New || R.ReturnType == (int)Common.EnumReturnType.Returning)));
            Ohter_iii.ForEach(P =>
            {
                if (!iii.Contains(P.CompanyID) && !ii_1.Contains(P.CompanyID) && !ii_2.Contains(P.CompanyID) && !i.Contains(P.CompanyID) && !ii_4.Contains(P.CompanyID) && !ii_3.Contains(P.CompanyID))
                {
                    iii.Add(P.CompanyID);
                }
            });



            //iv.	在每一类中，按净利润差额的绝对值由大到小排列
            NotReturn_i = new List<MonthlyReportDetail>();
            CanNotReturn_ii_1 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_2 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_3 = new List<MonthlyReportDetail>();
            CanNotReturn_ii_4 = new List<MonthlyReportDetail>();
            Ohter_iii = new List<MonthlyReportDetail>();
            NotReturn_i.AddRange(RptDetailList.FindAll(R => i.Contains(R.CompanyID)));
            CanNotReturn_ii_1.AddRange(RptDetailList.FindAll(R => ii_1.Contains(R.CompanyID)));
            CanNotReturn_ii_2.AddRange(RptDetailList.FindAll(R => ii_2.Contains(R.CompanyID)));
            CanNotReturn_ii_3.AddRange(RptDetailList.FindAll(R => ii_3.Contains(R.CompanyID)));
            CanNotReturn_ii_4.AddRange(RptDetailList.FindAll(R => ii_4.Contains(R.CompanyID)));

            Ohter_iii.AddRange(RptDetailList.FindAll(R => iii.Contains(R.CompanyID)));

            List<MonthlyReportDetail> notMissed = new List<MonthlyReportDetail>();

            notMissed.AddRange(
                RptDetailList.FindAll(R => !NotReturn_i.Exists(NI => NI.ID == R.ID) &&
                !CanNotReturn_ii_1.Exists(CII1 => CII1.ID == R.ID) &&
                !CanNotReturn_ii_2.Exists(CII2 => CII2.ID == R.ID) &&
                !CanNotReturn_ii_3.Exists(CII3 => CII3.ID == R.ID) &&
                !CanNotReturn_ii_4.Exists(CII4 => CII4.ID == R.ID) &&
                !Ohter_iii.Exists(OIII => OIII.ID == R.ID)
                ));

            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();

            result.AddRange(Sort(NotReturn_i, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_1, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_2, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_3, Paramters));
            result.AddRange(Sort(CanNotReturn_ii_4, Paramters));

            result.AddRange(Sort(Ohter_iii, Paramters));
            result.AddRange(Sort(notMissed, Paramters));
            return result;
        }




        //iv.	在每一类中，按净利润差额的绝对值由大到小排列
        //iv.	在每一类中，按净利润差额的绝对值由大到小排列
        //iv.	在每一类中，先按净收入差额的绝对值由大到小排列，如果净收入指标完成（即差额是正数），则按综合收入差额的绝对值由大到小排列
        //已补回：按净利润差额（正数）由大到小排列
        //iv.	在每一类中按指标差额的绝对值由大到小排列
        //已补回：按指标差额（正数）由大到小排列
        List<MonthlyReportDetail> Sort(List<MonthlyReportDetail> list, string Paramters)
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
                list.FindAll(P => P.TargetID == TargetID && P.NDifference < 0).ForEach(C =>
                {
                    if (!CompanyIDs.ContainsKey(C.CompanyID))
                    {
                        CompanyIDs.Add(C.CompanyID, Math.Abs(C.NDifference));
                    }
                });

                List<Guid> Companies = new List<Guid>();
                CompanyIDs.OrderByDescending(P => P.Value).ForEach(C => {
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
                            CompanyIDs.Add(C.CompanyID, Math.Abs(C.NDifference));
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





}
