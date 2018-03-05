using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanda.Financing.Web
{
    public class BaseUserControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// 控件所在页
        /// </summary>
        public BasePage CurrentPage
        {
            get
            {
                return (BasePage)this.Page;
            }
        }

        #region 获取格式化字符,超出长度加...
        /// <summary>
        /// 获取格式化字符,超出长度加...
        /// </summary>
        /// <param name="originalString">原始字符</param>
        /// <param name="limitedlength">限制长度，单位：字节,一个汉字代表2个字节，英文或者字母1个字节</param>
        /// <returns></returns>
        public string GetFormatString(string originalString, int limitedlength)
        {
            string retValue = originalString;
            if (StringLength(retValue) > limitedlength)
            {
                return StringCut(retValue, limitedlength);
            }
            return retValue;
        }

        /// <summary>
        /// 获取格式化字符,超出长度加...
        /// </summary>
        /// <param name="originalObj">原始字符</param>
        /// <param name="limitedlength">限制长度，单位：字节,一个汉字代表2个字节，英文或者字母1个字节</param>
        /// <returns></returns>
        public string GetFormatString(object originalObj, int limitedlength)
        {
            string retValue = "";
            if (originalObj != null)
            {
                retValue = originalObj.ToString();
            }

            if (StringLength(retValue) > limitedlength)
            {
                return StringCut(retValue, limitedlength);
            }
            return retValue;
        }

        /// <summary>
        /// 统计字符串长度[字节数]，对中文字符按照两个字节统计
        /// </summary>
        public static int StringLength(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            return System.Text.Encoding.GetEncoding("GB2312").GetBytes(str).Length;
        }

        /// <summary>
        /// 字符串截取，按照字节处理，解决半个汉字问题
        /// </summary>
        /// <param name="srcString">输入字符</param>
        /// <param name="byteLength">指定字节数</param>
        /// <returns></returns>
        public static string StringCut(string srcString, int byteLength)
        {
            if ((srcString ?? "") == "") return "";
            string myResult = srcString;
            if (byteLength >= 0)
            {
                byte[] bsSrcString = System.Text.Encoding.GetEncoding("GB2312").GetBytes(srcString);

                if (bsSrcString.Length >= byteLength)
                {
                    if (byteLength > 2)
                    {
                        //预留2位存放省略号…
                        byteLength = byteLength - 2;
                    }
                    int nRealLength = byteLength;
                    int[] anResultFlag = new int[byteLength];
                    byte[] bsResult = null;
                    int nFlag = 0;
                    for (int i = 0; i < byteLength; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[byteLength - 1] > 127) && (anResultFlag[byteLength - 1] == 1))
                    {
                        nRealLength = byteLength + 1;
                    }
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);
                    myResult = System.Text.Encoding.GetEncoding("GB2312").GetString(bsResult);
                    myResult = myResult + "…";
                }
            }
            return myResult;
        }
        #endregion

     }
}