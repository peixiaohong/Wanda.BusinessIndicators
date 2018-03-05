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
    /// �������������ʽ������
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
                throw new Exception("���ʽ��������");
            }
        }

        public WorkflowExpressionParser(object expressionDictionary)
        {
            _BizObject = expressionDictionary;
        }

        private object _BizObject = null;

        /// <summary>
        /// �����û�������������ʽ
        /// </summary>
        /// <param name="parseCondition">�������ʽ</param>
        /// <returns>�������</returns>
        public bool CacluateCondition(string parseCondition)
        {
            ParseExpression pe = new ParseExpression();
            pe.UserFunctions = (IExpParsing)this;

            pe.ChangeExpression(parseCondition);
            object condValue = pe.Value();

            return (bool)condValue;
        }

        /// <summary>
        /// ������ʽ
        /// </summary>
        /// <param name="strFuncName">������</param>
        /// <param name="paramObject">����</param>
        /// <param name="parseObj">ParseExpression����</param>
        /// <returns>������</returns>
        public object CalculateExpression(string strFuncName, ParamObject[] paramObject, ParseExpression parseObj)
        {
            object returnValue = null;

            switch (strFuncName.ToLower())
            {
                case "getvalue":
                    returnValue = GetValueFunction((string)paramObject[0].Value);           //ȡ��ֵ
                    break;
                case "abs":
                    returnValue = GetABSFunction((string)paramObject[0].Value);             //ȡ����ֵ
                    break;
                case "getstringvalue":
                    returnValue = GetStringValueFunction((string)paramObject[0].Value);     //ȡ�ַ���ֵ
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="propertyName">��������</param>
        /// <param name="operatorString">������</param>
        /// <param name="expectativeValue">�ڴ�ֵ</param>
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
        /// ����������
        /// </summary>
        /// <param name="propertyName">��������</param>
        /// <param name="operatorString">������</param>
        /// <param name="expectativeValue">�ڴ�ֵ</param>
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
                throw new Exception("��ȡ���������Ե�ֵ����");
            }

            return returnValue;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="propertyName">��������</param>
        /// <param name="operatorString">������</param>
        /// <param name="expectativeValue">�ڴ�ֵ</param>
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
                throw new Exception("��ȡ���������Ե�ֵ����");
            }

            return returnValue;
        }


        #region IExpParsing ��Ա

        /// <summary>
        /// ʵ��IExpParsing�ӿ�CheckUserFunction����
        /// </summary>
        /// <param name="strFuncName">�Զ��庯������</param>
        /// <param name="arrParams">������������</param>
        /// <param name="parseObj">ParseExpression����</param>
        /// <returns>������õĽ������</returns>
        public object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
        {
            return CalculateExpression(strFuncName, arrParams, parseObj);
        }

        /// <summary>
        /// ʵ��IExpParsing�ӿ�CalculateUserFunction������Ŀǰδ��
        /// </summary>
        /// <param name="strFuncName" >��������</param>
        /// <param name="arrParams" >��������</param>
        /// <param name="parseObj" >���ʽ����</param>
        /// <returns>������õĽ������</returns>
        public object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
        {
            return CalculateExpression(strFuncName, arrParams, parseObj);
        }

        #endregion
    }
}
