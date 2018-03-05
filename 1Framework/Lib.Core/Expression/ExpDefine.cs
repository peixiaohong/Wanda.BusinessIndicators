using System;
using System.Xml;
using System.Resources;
using System.Reflection;
using System.Collections;

namespace Lib.Expression
{
	/// <summary>
	/// 错误类别
	/// </summary>
	public enum Parse_Error
	{
		peNone = 0,

		/// <summary>
		/// 非法字符
		/// </summary>
		peInvalidChar,

		/// <summary>
		/// 非法的字符串
		/// </summary>
		peInvalidString,

		/// <summary>
		/// 非法操作符
		/// </summary>
		peInvalidOperator,

		/// <summary>
		/// 类型不匹配
		/// </summary>
		peTypeMismatch,

		/// <summary>
		/// 非法的参数
		/// </summary>
		peInvalidParam,

		/// <summary>
		/// 非法的用户自定义函数返回值
		/// </summary>
		peInvalidUFValue,

		/// <summary>
		/// 语法错误
		/// </summary>
		peSyntaxError,

		/// <summary>
		/// 浮点运算溢出
		/// </summary>
		peFloatOverflow,

		/// <summary>
		/// 需要某个字符
		/// </summary>
		peCharExpected,

		/// <summary>
		/// 函数错误
		/// </summary>
		peFuncError,

		/// <summary>
		/// 需要操作数
		/// </summary>
		peNeedOperand,

		/// <summary>
		/// 格式错误
		/// </summary>
		peFormatError,
	}

	/// <summary>
	/// 为表达式识别错误封装的异常
	/// </summary>
	public class ParsingException: System.Exception
	{
		private Parse_Error _Reason = Parse_Error.peNone;
		private int _Position = -1;

		/// <summary>
		/// 构造函数，根据错误类型、出错位置构造异常
		/// </summary>
		/// <param name="pe"></param>
		/// <param name="nPosition"></param>
		/// <param name="strMsg"></param>
		public ParsingException(Parse_Error pe, int nPosition, string strMsg):base(strMsg)
		{
			_Reason = pe;
			_Position = nPosition;
		}

		/// <summary>
		/// 错误原因
		/// </summary>
		public Parse_Error Reason
		{
			get
			{
				return _Reason;
			}
		}

		/// <summary>
		/// 出错位置
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// 产生一个新的表达式识别异常
		/// </summary>
		/// <param name="pe">错误原因</param>
		/// <param name="nPosition">出错位置</param>
		/// <param name="strParams">在错误信息中的参数</param>
		/// <returns>表达式识别异常对象</returns>
		static public ParsingException NewParsingException(Parse_Error pe, int nPosition, params string[] strParams)
		{
            try
            {
                ResourceManager rm = new ResourceManager("Lib.LightWorkflow.Tools", Assembly.GetExecutingAssembly());

                string strID = pe.ToString();

                string strText = rm.GetString(strID);

                strText = string.Format(strText, strParams);

                if (nPosition >= 0)
                {
                    strText = string.Format(rm.GetString("position"), nPosition + 1) + ", " + strText;
                }

                return new ParsingException(pe, nPosition, strText);
            }
            catch
            {
                string msg = string.Empty;
                if (nPosition >= 0)
                {
                    msg = string.Format("字符位置{0},", nPosition);
                }
                foreach (var item in strParams)
                {
                    msg += "操作符:";
                    msg += item + ",";
                }

                throw new Exception(string.Format("产生一个表达式识别异常,{0}", msg.TrimEnd(',')));
            }
		}
	}

	/// <summary>
	/// 操作类型
	/// </summary>
	public enum Operation_IDs
	{
		OI_NONE = 0,
		OI_NOT = 120,
		OI_ADD,
		OI_MINUS,
		OI_MUL,
		OI_DIV,
		OI_NEG,
		//
		OI_EQUAL,
		OI_NOT_EQUAL,
		OI_GREAT,
		OI_GREATEQUAL,
		OI_LESS,
		OI_LESSEQUAL,
		OI_LOGICAL_AND,
		OI_LOGICAL_OR,
		//
		OI_LBRACKET,
		OI_RBRACKET,
		OI_COMMA,
		//
		OI_USERDEFINE,
		OI_STRING,
		OI_NUMBER,
		OI_BOOLEAN,
		OI_DATETIME,
	}

	public class ParseIdentifier
	{
		internal Operation_IDs _OperationID = Operation_IDs.OI_NONE;
		internal string _Identifier = string.Empty;
		internal int _Position = -1;
		internal ParseIdentifier _PrevIdentifier = null;
		internal ParseIdentifier _NextIdentifier = null;
		internal ParseIdentifier _SubIdentifier = null;
		internal ParseIdentifier _ParentIdentifier = null;

		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		public string Identifier
		{
			get
			{
				return _Identifier;
			}
		}

		public int Position
		{
			get
			{
				return _Position;
			}
		}

		public ParseIdentifier PrevIdentifier
		{
			get
			{
				return _PrevIdentifier;
			}
		}

		public ParseIdentifier NextIdentifier
		{
			get
			{
				return _NextIdentifier;
			}
		}

		public ParseIdentifier SubIdentifier
		{
			get
			{
				return _SubIdentifier;
			}
		}

		public ParseIdentifier ParentIdentifier
		{
			get
			{
				return _ParentIdentifier;
			}
		}

		public ParseIdentifier()
		{
		}

		public ParseIdentifier(Operation_IDs oID, string strID, int nPos, ParseIdentifier prev)
		{
			_OperationID = oID;
			_Identifier = strID;
			_Position = nPos;
			_PrevIdentifier = prev;
		}
	}

	public class ParamObject
	{
		internal object _Value = null;
		internal int _Position = -1;

		public ParamObject()
		{
		}

		public ParamObject(object v, int nPos)
		{
			_Value = v;
			_Position = nPos;
		}

		public object Value
		{
			get
			{
				return _Value;
			}
		}

		public int Position
		{
			get
			{
				return _Position;
			}
		}
	}

	public interface IExpParsing
	{
		object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);
		object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);
	}

	public interface IExpEditor
	{
		XmlNode GetNameSpaceNode();
	}

	/// <summary>
	/// 二叉树的节点
	/// </summary>
	public class EXP_TreeNode
	{
		internal EXP_TreeNode _Left = null;
		internal EXP_TreeNode _Right = null;
		internal int _Position = 0;
		internal Operation_IDs _OperationID = Operation_IDs.OI_NONE;
		internal Object _Value = null;
		internal ArrayList _Params = null;
		internal string _FunctionName = string.Empty;

		public EXP_TreeNode Left
		{
			get
			{
				return _Left;
			}
		}

		public EXP_TreeNode Right
		{
			get
			{
				return _Right;
			}
		}

		public int Position
		{
			get
			{
				return _Position;
			}
		}

		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		public Object Value
		{
			get
			{
				return _Value;
			}
		}

		public ArrayList Params
		{
			get
			{
				return _Params;
			}
		}

		public string FunctionName
		{
			get
			{
				return _FunctionName;
			}
		}
	}
}
