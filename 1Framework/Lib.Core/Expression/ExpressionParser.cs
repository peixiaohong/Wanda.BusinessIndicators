using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lib.Expression
{
    /// <summary>
    /// 条件表达式解析器
    /// </summary>
    public class ExpressionParser : IExpParsing
    {

        public ExpressionParser(object expressionDictionary)
        {
            _BizObject = expressionDictionary;
        }

        private object _BizObject = null;

        /// <summary>
        /// 解析用户输入的条件表达式
        /// </summary>
        /// <param name="parseCondition">条件表达式</param>
        /// <returns>解析结果</returns>
        public bool CacluateCondition(string parseCondition)
        {
            if (string.IsNullOrEmpty(parseCondition)) return true;
            ParseExpression pe = new ParseExpression();
            pe.UserFunctions = (IExpParsing)this;

            pe.ChangeExpression(parseCondition);
            object condValue = pe.Value();

            return (bool)condValue;
        }

        /// <summary>
        /// 解析用户输入的条件表达式
        /// </summary>
        /// <param name="parseCondition">条件表达式</param>
        /// <returns>解析结果</returns>
        public decimal CacluateExpression(string Expression)
        {
            if (string.IsNullOrEmpty(Expression)) throw new Exception("Expression is null");
            ParseExpression pe = new ParseExpression();
            pe.UserFunctions = (IExpParsing)this;

            pe.ChangeExpression(Expression);
            object condValue = pe.Value();
            decimal result = decimal.MinValue;
            decimal.TryParse(condValue.ToString(), out result);

            return result;
        }


        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="strFuncName">函数名</param>
        /// <param name="paramObject">参数</param>
        /// <param name="parseObj">ParseExpression变量</param>
        /// <returns>计算结果</returns>
        public object CalculateExpression(string strFuncName, ParamObject[] paramObject, ParseExpression parseObj)
        {
            object returnValue = null;

            switch (strFuncName.ToLower())
            {
                case "getvalue":
                    returnValue = GetValueFunction((string)paramObject[0].Value);     //属性名称
                    break;
                case "getstringvalue":
                    returnValue = GetStringValueFunction((string)paramObject[0].Value);     //属性名称
                    break;
                case "istrue":
                    returnValue = IsTrue((string)paramObject[0].Value);     //属性名称
                    break;
                case "getdatevalue":
                    returnValue = GetDateValueFunction((string)paramObject[0].Value);     //属性名称
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// 检查基本属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="operatorString">操作符</param>
        /// <param name="expectativeValue">期待值</param>
        /// <returns></returns>
        private DateTime GetDateValueFunction(string propertyName)
        {
            DateTime returnValue = DateTime.MinValue;

            try
            {
                if (this._BizObject is Hashtable)
                {
                    Hashtable ht = (Hashtable)this._BizObject;
                    if (ht[propertyName] != null)
                    {
                        returnValue = Convert.ToDateTime(ht[propertyName]);
                    }
                }
                else
                {
                    PropertyInfo p = this._BizObject.GetType().GetProperty(propertyName);

                    if (p != null)
                    {
                        returnValue = Convert.ToDateTime(p.GetValue(this._BizObject, null));
                    }
                }

            }
            catch
            {
                throw new Exception("获取对象中属性的值错误！");
            }
            return returnValue;
        }


        /// <summary>
        /// 检查基本属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="operatorString">操作符</param>
        /// <param name="expectativeValue">期待值</param>
        /// <returns></returns>
        private double GetValueFunction(string propertyName)
        {
            double returnValue = 0;

            try
            {
                if (this._BizObject is Hashtable)
                {
                    Hashtable ht = (Hashtable)this._BizObject;
                    if (ht[propertyName] != null)
                    {
                        returnValue = Convert.ToDouble(ht[propertyName]);
                    }
                }
                else
                {
                    PropertyInfo p = this._BizObject.GetType().GetProperty(propertyName);

                    if (p != null)
                    {
                        returnValue = Convert.ToDouble(p.GetValue(this._BizObject, null));
                    }
                }

            }
            catch
            {
                throw new Exception("获取对象中属性的值错误！");
            }

            return returnValue;
        }

        /// <summary>
        /// 检查基本属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="operatorString">操作符</param>
        /// <param name="expectativeValue">期待值</param>
        /// <returns></returns>
        private string GetStringValueFunction(string propertyName)
        {
            string returnValue = string.Empty;

            try
            {
                if (this._BizObject is Hashtable)
                {
                    Hashtable ht = (Hashtable)this._BizObject;
                    if (ht[propertyName] != null)
                    {
                        returnValue = Convert.ToString(ht[propertyName]);
                    }
                }
                else
                {
                    PropertyInfo p = this._BizObject.GetType().GetProperty(propertyName);

                    if (p != null)
                    {
                        returnValue = Convert.ToString(p.GetValue(this._BizObject, null));
                    }
                }

            }
            catch
            {
                throw new Exception("获取对象中属性的值错误！");
            }

            return returnValue;
        }

        /// <summary>
        /// 检查基本属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="operatorString">操作符</param>
        /// <param name="expectativeValue">期待值</param>
        /// <returns></returns>
        private bool IsTrue(string propertyName)
        {
            bool returnValue = false;

            try
            {
                if (this._BizObject is Hashtable)
                {
                    Hashtable ht = (Hashtable)this._BizObject;
                    if (ht[propertyName] != null)
                    {
                        returnValue = Convert.ToBoolean(ht[propertyName]);
                    }
                }
                else
                {
                    PropertyInfo p = this._BizObject.GetType().GetProperty(propertyName);

                    if (p != null)
                    {
                        returnValue = Convert.ToBoolean(p.GetValue(this._BizObject, null));
                    }
                }

            }
            catch
            {
                throw new Exception("获取对象中属性的值错误！");
            }

            return returnValue;
        }


        #region IExpParsing 成员

        /// <summary>
        /// 实现IExpParsing接口CheckUserFunction函数
        /// </summary>
        /// <param name="strFuncName">自定义函数名称</param>
        /// <param name="arrParams">函数变量数组</param>
        /// <param name="parseObj">ParseExpression变量</param>
        /// <returns>解析获得的结果对象</returns>
        public object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
        {
            return CalculateExpression(strFuncName, arrParams, parseObj);
        }

        /// <summary>
        /// 实现IExpParsing接口CalculateUserFunction函数，目前未用
        /// </summary>
        /// <param name="strFuncName" >函数名称</param>
        /// <param name="arrParams" >参数数组</param>
        /// <param name="parseObj" >表达式对象</param>
        /// <returns>解析获得的结果对象</returns>
        public object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
        {
            return CalculateExpression(strFuncName, arrParams, parseObj);
        }

        #endregion
    }
}
