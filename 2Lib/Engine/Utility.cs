using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Engine
{
    public class Utility
    {
        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static Utility Instance
        {
            get
            {
                return _instance;
            }
        }private static Utility _instance = new Utility();

        public string CalculateDisplayRate(decimal ActualRate, decimal MeasureRate, string strUnit, int DisplayType = 1)
        {
            return CalculateDisplayRate(double.Parse(ActualRate.ToString()), MeasureRate, strUnit, DisplayType);
        }

        public string CalculateDisplayRate(double ActualRate, decimal _measureRate, string strUnit, int DisplayType = 1)
        {

            double MeasureRate = Convert.ToDouble(_measureRate);
            int DigitLen = 2;
            if (MeasureRate < 1 && DisplayType != 3)
            {
                DigitLen = 4;
            }
            double DisplayRate = Math.Abs(ActualRate);
            if (DisplayType != 3)
            {
                DisplayRate = Math.Round(DisplayRate, DigitLen, MidpointRounding.AwayFromZero);
            }
            //去除1
            if (DisplayRate == 1 && ActualRate < 1)
            {
                double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                while (t == 1)
                {
                    DigitLen++;
                    if (DigitLen > 15)
                    {
                        DigitLen = 15;
                        t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        break;
                    }

                    t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                }
                DisplayRate = t;
            }

            //去除0
            if (DisplayRate == 0 && ActualRate != 0)
            {
                double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                while (t == 0)
                {
                    DigitLen++;
                    if (DigitLen > 15)
                    {
                        DigitLen = 15;
                        t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        break;
                    }
                    t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                }
                DisplayRate = t;
            }

            if (DisplayType == 3)
            {
                if (DisplayRate.ToString().IndexOf(".") > 0)
                {
                    double tempDisplayRate = Math.Abs(DisplayRate); //首先绝对值

                    if (tempDisplayRate >= 1) //判断绝对值是 > 1
                    {
                        return Math.Round(DisplayRate, 0, MidpointRounding.AwayFromZero).ToString() + strUnit;
                    }
                    else //反之 <1的
                    {
                        string tempStr = string.Empty;

                        for (int i = 1; i <= DisplayRate.ToString().Length; i++)
                        {
                            if ((tempDisplayRate * 10) > 1) //乘以10 计算四舍五入的位数
                            {
                                tempStr = Math.Round(DisplayRate, i, MidpointRounding.AwayFromZero).ToString() + strUnit;
                                break;
                            }
                            else
                            {
                                tempDisplayRate = tempDisplayRate * 10;
                                continue;
                            }
                        }

                        return tempStr;
                    }
                }
                else
                {
                    return DisplayRate.ToString() + strUnit;
                }
            }
            else
            {

                string NDisplayRate = (DisplayRate * 100).ToString();

                if (DisplayType == 1)
                {
                    if (DisplayRate == MeasureRate && ActualRate < MeasureRate)
                    {
                        double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        while (t == MeasureRate)
                        {
                            DigitLen++;
                            if (DigitLen > 15)
                            {
                                DigitLen = 15;
                                t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                                break;
                            }
                            t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        }
                        DisplayRate = t;
                    }
                    NDisplayRate = (DisplayRate * 100).ToString();
                }

                if (NDisplayRate.IndexOf(".") > 0)
                {
                    NDisplayRate = NDisplayRate.TrimEnd('0').TrimEnd('.');
                }
                NDisplayRate = NDisplayRate + "%";

                return NDisplayRate;
            }
        }




        /// <summary>
        /// 项目公司用
        /// </summary>
        /// <param name="ActualRate">实际完成率</param>
        /// <param name="MeasureRate">考核阀值</param>
        /// <param name="strUnit">指标单位</param>
        /// <param name="DisplayType">指标类型</param>
        /// <param name="Number">完成率保留几位小数</param>
        /// <returns></returns>
        public string Pro_CalculateDisplayRate(decimal ActualRate, decimal MeasureRate, string strUnit, int DisplayType = 1, int Number = 3)
        {
            return Pro_CalculateDisplayRate(double.Parse(ActualRate.ToString()), MeasureRate, strUnit, DisplayType, Number);
        }


        /// <summary>
        /// 项目公司用
        /// </summary>
        /// <param name="ActualRate"></param>
        /// <param name="_measureRate"></param>
        /// <param name="strUnit"></param>
        /// <param name="DisplayType"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        public string Pro_CalculateDisplayRate(double ActualRate, decimal _measureRate, string strUnit, int DisplayType = 1, int Number = 3)
        {

            double MeasureRate = Convert.ToDouble(_measureRate);
            int DigitLen = Number;
            if (MeasureRate < 1 && DisplayType != 3)
            {
                DigitLen = Number;
            }
            double DisplayRate = Math.Abs(ActualRate);
            if (DisplayType != 3)
            {
                DisplayRate = Math.Round(DisplayRate, DigitLen, MidpointRounding.AwayFromZero);
            }
            //去除1
            if (DisplayRate == 1 && ActualRate < 1)
            {
                double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                while (t == 1)
                {
                    DigitLen++;
                    if (DigitLen > 15)
                    {
                        DigitLen = 15;
                    }
                    t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                }
                DisplayRate = t;
            }

            //去除0
            if (DisplayRate == 0 && ActualRate != 0)
            {
                double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                while (t == 0)
                {
                    DigitLen++;
                    if (DigitLen > 15)
                    {
                        DigitLen = 15;
                        t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        break;
                    }
                    t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                }
                DisplayRate = t;
            }

            if (DisplayType == 3)
            {
                if (DisplayRate.ToString().IndexOf(".") > 0)
                {
                    double tempDisplayRate = Math.Abs(DisplayRate); //首先绝对值

                    if (tempDisplayRate >= 1) //判断绝对值是 > 1
                    {
                        return Math.Round(DisplayRate, 0, MidpointRounding.AwayFromZero).ToString() + strUnit;
                    }
                    else //反之 <1的
                    {
                        string tempStr = string.Empty;

                        for (int i = 1; i <= DisplayRate.ToString().Length; i++)
                        {
                            if ((tempDisplayRate * 10) > 1) //乘以10 计算四舍五入的位数
                            {
                                tempStr = Math.Round(DisplayRate, i, MidpointRounding.AwayFromZero).ToString() + strUnit;
                                break;
                            }
                            else
                            {
                                tempDisplayRate = tempDisplayRate * 10;
                                continue;
                            }
                        }

                        return tempStr;
                    }
                }
                else
                {
                    return DisplayRate.ToString() + strUnit;
                }
            }
            else
            {

                string NDisplayRate = (DisplayRate * 100).ToString();

                if (DisplayType == 1)
                {
                    if (DisplayRate == MeasureRate && ActualRate < MeasureRate)
                    {
                        double t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        while (t == MeasureRate)
                        {
                            DigitLen++;

                            if (DigitLen > 15)
                            {
                                DigitLen = 15;
                                t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                                break;
                            }
                            t = Math.Round(ActualRate, DigitLen, MidpointRounding.AwayFromZero);
                        }
                        DisplayRate = t;
                    }
                    NDisplayRate = DisplayRate.ToString("P1");    //因是项目系统单独用到，其它系统请不要使用该方法
                }

                return NDisplayRate;
            }
        }

    }
}