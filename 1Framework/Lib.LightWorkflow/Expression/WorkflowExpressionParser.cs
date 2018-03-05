using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

//using Capital.iWorkflow.Framework.Tools;
//using Capital.iWorkflow.Framework.ExpParsing;

//using Capital.iWorkflow.PortalLibrary.DataObject;

namespace Wanda.Lib.LightWorkflow.Expression
{
    /// <summary>
    /// 工作流条件表达式解析器
    /// </summary>
    public class WorkflowExpressionParser : IExpParsing
    {
        public WorkflowExpressionParser(string expressionStr)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            try
            {
                _BizObject = js.Deserialize<Hashtable>(expressionStr);
            }
            catch
            {
                throw new Exception("表达式解析出错。");
            }
        }

        public WorkflowExpressionParser(object expressionDictionary)
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
            ParseExpression pe = new ParseExpression();
            pe.UserFunctions = (IExpParsing)this;

            pe.ChangeExpression(parseCondition);
            object condValue = pe.Value();

            return (bool)condValue;
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
                    returnValue = GetValueFunction((string)paramObject[0].Value);           //取数值
                    break;
                case "abs":
                    returnValue = GetABSFunction((string)paramObject[0].Value);             //取绝对值
                    break;
                case "getstringvalue":
                    returnValue = GetStringValueFunction((string)paramObject[0].Value);     //取字符串值
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
        private double GetABSFunction(string propertyName)
        {
            double returnValue = 0;

            ParseExpression pe = new ParseExpression();
            pe.OutputIdentifiers = true;
            pe.UserFunctions = new WorkflowExpressionParser(this._BizObject);
            pe.ChangeExpression(propertyName);
            string s = pe.Value().ToString();
            returnValue = Math.Abs(Double.Parse(s));
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
