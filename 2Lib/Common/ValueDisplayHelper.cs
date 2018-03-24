using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.BusinessIndicators.Common
{
    //public static class ValueDisplayHelper
    //{
    //    #region Amount
    //    private const decimal WANYUAN_RATE = 0.0001M;
    //    private const decimal YIYUAN_RATE = 0.00000001M;

    //    //public static decimal ToUnit(this decimal yuan, AmountUnit targetUnit)
    //    //{
    //    //    if (yuan == 0m)
    //    //    {
    //    //        return 0m;
    //    //    }
    //    //    switch (targetUnit)
    //    //    {
    //    //        case AmountUnit.Yuan:
    //    //            return yuan;
    //    //        case AmountUnit.WanYuan:
    //    //            return yuan * WANYUAN_RATE;
    //    //        case AmountUnit.YiYuan:
    //    //            return yuan * YIYUAN_RATE;
    //    //            break;
    //    //        default:
    //    //            throw new NotSupportedException(targetUnit.ToString());
    //    //    }
    //    //}

    //    //public static decimal ToUnit(this decimal amount, AmountUnit targetUnit, AmountUnit thisUnit)
    //    //{
    //    //    decimal yuan = 0m;
    //    //    switch (thisUnit)
    //    //    {
    //    //        case AmountUnit.Yuan:
    //    //            yuan = amount;
    //    //            break;
    //    //        case AmountUnit.WanYuan:
    //    //            yuan = amount / WANYUAN_RATE;
    //    //            break;
    //    //        case AmountUnit.YiYuan:
    //    //            yuan = amount / YIYUAN_RATE;
    //    //            break;
    //    //        default:
    //    //            break;
    //    //    }

    //    //    return yuan.ToUnit(targetUnit);
    //    //}

    //    /// <summary>
    //    /// 显示千分位转换
    //    /// </summary>
    //    /// <param name="amount"></param>
    //    /// <param name="decimalPoints">小数点后的位数（默认为2）</param>
    //    /// <returns></returns>
    //    public static string ToThounsandizeText(this decimal amount, int decimalPoints = 2)
    //    {
    //        if (decimalPoints > 0)
    //        {
    //            return amount.ToString("###,##0" + ".".PadLeft(decimalPoints, '0'));
    //        }
    //        else { return amount.ToString("###,##0"); }
    //    }

    //    public static string Segmenta(this string s,string signChar, int length)
    //    {
    //        int l = (s.Length-1)/4;
    //        for (int i = 0; i < l; i++)
    //        {
    //            s = s.Insert((i + 1) * 4 + i, signChar);
    //        }
    //        return s;
    //    }

    //    public static string ToThounsandizeTextOrEmpty(this decimal amount, int decimalPoints = 2, string emptyText = "--")
    //    {
    //        if (amount == 0m)
    //        {
    //            return emptyText;
    //        }

    //        if (decimalPoints > 0)
    //        {
    //            return amount.ToString("###,##0" + ".".PadRight(decimalPoints + 1, '0'));
    //        }
    //        else { return amount.ToString("###,##0"); }
    //    }
    //    #endregion

    //    /// <summary>
    //    /// 日期显示格式
    //    /// </summary>
    //    /// <param name="date"></param>
    //    /// <returns></returns>
    //    public static string ToDateString(this DateTime date)
    //    {
    //        return date.ToString("yyyy-MM-dd");
    //    }

    //    /// <summary>
    //    /// 日期时间显示格式
    //    /// </summary>
    //    /// <param name="date"></param>
    //    /// <returns></returns>
    //    public static string ToDateTimeString(this DateTime date)
    //    {
    //        return date.ToString("yyyy-MM-dd HH:mm:ss");

    //    }

    //    /// <summary>
    //    /// 日期时间显示格式, 方便js new Date()构造Date对象
    //    /// </summary>
    //    /// <param name="date"></param>
    //    /// <returns></returns>
    //    public static string ToJsDateTimeString(this DateTime date)
    //    {
    //        return date.ToString("MM/dd/yyyy HH:mm:ss");

    //    }
    //}
}
