using System;
using System.Text;
using System.Collections;

namespace Lib.Expression
{
	/// <summary>
	/// Summary description for ExpParsing.
	/// </summary>
	public class ParseExpression
	{
		/// <summary>
		/// 公共变量
		/// </summary>
		public bool Optimize = true;
		public Object ParseParam = null;
		public bool OutputIdentifiers = false;
		public IExpParsing UserFunctions = null;

		/// <summary>
		/// 私有变量
		/// </summary>
		private EXP_TreeNode _Tree = null;
		private ParseIdentifier _Identifiers = null;
		private string _Expression = string.Empty;
		private int _Position = 0;
		private char[] _ExpressionChars = null;

		private ParseIdentifier _CurrentIdentifier = null;
		private ParseIdentifier _ParentIdentifier = null;

		private delegate EXP_TreeNode DoNextOP();

		/// <summary>
		/// 缺省构造函数
		/// </summary>
		public ParseExpression()
		{
		}

		//公共方法

		public EXP_TreeNode Tree
		{
			get
			{
				return _Tree;
			}
		}

		public ParseIdentifier Identifiers
		{
			get
			{
				return _Identifiers;
			}
		}

		public string Expression
		{
			get
			{
				return _Expression;
			}
		}

		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// 重新构造二叉树
		/// </summary>
		/// <param name="strExpression">表达式</param>
		public void ChangeExpression(string strExpression)
		{
			_Expression = strExpression;

			_Tree = null;
			_Position = 0;
			_Identifiers = null;
			_CurrentIdentifier = null;
			_ParentIdentifier = null;

			if (strExpression != string.Empty)
			{
				try
				{
					_ExpressionChars = new Char[strExpression.Length + 1];

					strExpression.CopyTo(0, _ExpressionChars, 0, strExpression.Length);
					_ExpressionChars[strExpression.Length] = '\0';

					_Tree = DoExpression();

					if (_ExpressionChars[_Position] != '\0')
						throw ParsingException.NewParsingException(Parse_Error.peSyntaxError, _Position);
				}
				finally
				{
					_CurrentIdentifier = null;
					_ParentIdentifier = null;
				}
			}
		}

		/// <summary>
		/// 根据已经分析过的二叉树计算出结果
		/// </summary>
		/// <returns></returns>
		public object Value()
		{
			return VExp(_Tree);
		}


		//私有方法

		/// <summary>
		/// 顺序输出标识符
		/// </summary>
		/// <param name="oID"></param>
		/// <param name="strID"></param>
		/// <param name="nPos"></param>
		private void OutputID(Operation_IDs oID, string strID, int nPos)
		{
			if (OutputIdentifiers)
			{
				ParseIdentifier pi = new ParseIdentifier(oID, strID, nPos, _CurrentIdentifier);

				if (_CurrentIdentifier == null)
				{
					if (_ParentIdentifier == null)
						_Identifiers = pi;
					else
						_ParentIdentifier._SubIdentifier = pi;						
				}
				else
					_CurrentIdentifier._NextIdentifier = pi;

				pi._ParentIdentifier = _ParentIdentifier;
				_CurrentIdentifier = pi;
			}
		}

		private void OutputIDToSubLevel()
		{
			_ParentIdentifier = _CurrentIdentifier;
			_CurrentIdentifier = null;
		}

		private void OutputIDToParentLevel()
		{
			if (_ParentIdentifier != null)
			{
				_CurrentIdentifier = _ParentIdentifier;
				_ParentIdentifier = _ParentIdentifier._ParentIdentifier;
			}
		}

		/// <summary>
		/// 开始分析一段表达式
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoExpression()
		{
			return DoLogicalOR();
		}

		private EXP_TreeNode DoLogical_AND_OR(char chSensitive, Operation_IDs oID, DoNextOP nextOP)
		{
			EXP_TreeNode node = null;
			EXP_TreeNode node1 = nextOP();
			EXP_TreeNode node2 = null;

			while (_ExpressionChars[_Position] == chSensitive)
			{
				int nPos = _Position;
				char op = _ExpressionChars[_Position++];

				if (op == chSensitive)
				{
					OutputID(oID, chSensitive.ToString() + chSensitive.ToString(), nPos);
					_Position++;
				}

				node2 = nextOP();

				node = NewTreeNode(node1, node2, oID, nPos);

				node1 = node;
			}

			return node1;
		}

		private EXP_TreeNode DoLogicalOR()
		{
			return DoLogical_AND_OR('|', Operation_IDs.OI_LOGICAL_OR, new DoNextOP(DoLogicalAND));
		}

		/// <summary>
		/// 逻辑与
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoLogicalAND()
		{
			return DoLogical_AND_OR('&', Operation_IDs.OI_LOGICAL_AND, new DoNextOP(DoLogicalOP));
		}

		/// <summary>
		/// 逻辑比较运算
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoLogicalOP()
		{
			EXP_TreeNode node = null;
			EXP_TreeNode node1 = DoAddSub();
			EXP_TreeNode node2 = null;

			char op = _ExpressionChars[_Position];

			string strID = "";

			while (op == '>' || op == '<' || op == '=')
			{
				int nPos = _Position;

				Operation_IDs oID = Operation_IDs.OI_NONE;

				if (_ExpressionChars[++_Position] == '=')
				{
					switch(op)
					{
						case '>':	//>=
							oID = Operation_IDs.OI_GREATEQUAL;
							strID = ">=";
							break;
						case '<':	//<=
							oID = Operation_IDs.OI_LESSEQUAL;
							strID = "<=";
							break;
						case '=':	//==
							oID = Operation_IDs.OI_EQUAL;
							strID = "==";
							break;
						default: throw ParsingException.NewParsingException(Parse_Error.peInvalidOperator, 
										_Position, op.ToString());
					}

					_Position++;
				}
				else
				{
					if (_ExpressionChars[_Position] == '>')
					{
						if (op == '<')	//<>
						{
							strID = "<>";
							oID = Operation_IDs.OI_NOT_EQUAL;
							_Position++;
						}
						else
							throw ParsingException.NewParsingException(Parse_Error.peInvalidOperator,
									_Position, op.ToString());
					}
					else
					{
						switch(op)
						{
							case '>':
								oID = Operation_IDs.OI_GREAT;
								strID = ">";
								break;
							case '<':
								oID = Operation_IDs.OI_LESS;
								strID = "<";
								break;
							default:
								throw ParsingException.NewParsingException(Parse_Error.peInvalidOperator, 
									 _Position, op.ToString());
						}
					}
				}

				OutputID(oID, strID, nPos);

				node2 = DoAddSub();

				node = NewTreeNode(node1, node2, oID, nPos);

				node1 = node;

				op = _ExpressionChars[_Position];
			}

			return node1;
		}

		/// <summary>
		/// 处理加减运算
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoAddSub()
		{
			EXP_TreeNode node = null;
			EXP_TreeNode node1 = DoMulDiv();
			EXP_TreeNode node2 = null;

			char ch = _ExpressionChars[_Position];
			while (ch == '-' || ch == '+')
			{
				Operation_IDs oID = Operation_IDs.OI_NONE;

				oID = (ch == '-') ? Operation_IDs.OI_MINUS : Operation_IDs.OI_ADD;

				OutputID(oID, ch.ToString(), _Position);

				int nPos = _Position;

				_Position++;

				node2 = DoMulDiv();

				node = NewTreeNode(node1, node2, oID, nPos);

				node1 = node;

				ch = _ExpressionChars[_Position];
			}

			return node1;
		}

		/// <summary>
		/// 处理乘除运算
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoMulDiv()
		{
			EXP_TreeNode node = null;
			EXP_TreeNode node1 = DoSgOP();
			EXP_TreeNode node2 = null;

			char ch = _ExpressionChars[_Position];
			while (ch == '*' || ch == '/')
			{
				Operation_IDs oID = Operation_IDs.OI_NONE;

				oID = (ch == '*') ? Operation_IDs.OI_MUL : Operation_IDs.OI_DIV;

				OutputID(oID, ch.ToString(), _Position);

				int nPos = _Position;

				_Position++;
				node2 = DoSgOP();

				node = NewTreeNode(node1, node2, oID, nPos);
				node1 = node;

				ch = _ExpressionChars[_Position];
			}

			return node1;
		}

		/// <summary>
		/// 处理逻辑非"!"运算符
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoSgOP()
		{
			EXP_TreeNode node = null;
			EXP_TreeNode node2 = null;

			SkipSpaces();

			char ch = _ExpressionChars[_Position];

			if (ch == '!')
			{
				OutputID(Operation_IDs.OI_NOT, "!", _Position);

				int nPos = _Position;

				_Position++;

				node2 = DoSgOP();
				node = NewTreeNode(null, node2, Operation_IDs.OI_NOT, nPos);
			}
			else
				node = DoFactor();

			return node;
		}

		/// <summary>
		/// 处理各种系数、负数、括号等
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoFactor()
		{
			EXP_TreeNode node = null;
			EXP_TreeNode left = null;
			EXP_TreeNode node2 = null;

			SkipSpaces();

			char ch = _ExpressionChars[_Position];

			int nPos = _Position;

			if (ch == '-')	//处理负号
			{
				OutputID(Operation_IDs.OI_NEG, "-", _Position);

				_Position++;

				node2 = DoExpression();

				left = NewTreeNode();
				left._OperationID = Operation_IDs.OI_NEG;
				left._Value = (double)-1;
				left._Position = nPos;

				node = NewTreeNode(left, node2, Operation_IDs.OI_MUL, nPos);
			}
			else
			if (ch == '(')
			{
				OutputID(Operation_IDs.OI_LBRACKET, "(", _Position);
				_Position++;
				node = DoExpression();

				SkipSpaces();

				if (_ExpressionChars[_Position] != ')')
					throw ParsingException.NewParsingException(Parse_Error.peCharExpected, _Position, ")");
				else
					OutputID(Operation_IDs.OI_RBRACKET, ")", _Position);

				_Position++;
			}
			else
				node = DoIdentifier();

			SkipSpaces();

			return node;
		}

		/// <summary>
		/// 处理各种数字、标识符、自定义函数、字符串等
		/// </summary>
		/// <returns></returns>
		private EXP_TreeNode DoIdentifier()
		{
			EXP_TreeNode result = null;

			SkipSpaces();

			char ch = _ExpressionChars[_Position];

			if (ch != '\0')
			{
				if (ch == '#')	//string
					result = DoDatetime();
				else
				if (ch == '"')	//string
					result = DoString();
				else
				if (Char.IsDigit(ch) || ch == '.')
					result = DoNumber();
				else
				if (Char.IsLetter(ch) || ch == '_')
					result = DoFunctionID();
				else
					throw ParsingException.NewParsingException(Parse_Error.peInvalidOperator, _Position, ch.ToString());

				SkipSpaces();
			}

			return result;
		}

		private EXP_TreeNode DoDatetime()
		{
			int nPos = _Position;
			char ch = _ExpressionChars[++_Position];

			StringBuilder strB = new StringBuilder(256);

			strB.Append("#");

			while (ch != '\0')
			{
				strB.Append(ch);

				if (ch == '#')
				{
					_Position++;
					break;
				}

				ch = _ExpressionChars[++_Position];
			}

			if (ch == '\0')
				throw ParsingException.NewParsingException(Parse_Error.peCharExpected, _Position, "#");

			EXP_TreeNode node = NewTreeNode();

			node._Position = nPos;
			node._OperationID = Operation_IDs.OI_DATETIME;

			try
			{
				string strID = strB.ToString();
				node._Value = DateTime.Parse(strID);

				OutputID(Operation_IDs.OI_DATETIME, strID, nPos);

				return node;
			}
			catch(System.FormatException)
			{
				throw ParsingException.NewParsingException(Parse_Error.peFormatError, nPos);
			}
		}

		private EXP_TreeNode DoString()
		{
			int nPos = _Position;
			char ch = _ExpressionChars[++_Position];

			StringBuilder strB = new StringBuilder(256);
			StringBuilder strIDB = new StringBuilder(256);

			strIDB.Append('"');

			while (ch != '\0')
			{
				if (ch != '"')
				{
					strB.Append(ch);
					strIDB.Append(ch);
					_Position++;
				}
				else
				if (_ExpressionChars[_Position + 1] == '"')
				{
					strB.Append('"');
					strIDB.Append("\"\"");
					_Position += 2;
				}
				else
				{
					strIDB.Append('"');
					_Position++;
					break;
				}

				ch = _ExpressionChars[_Position];
			}

			if (ch == '\0')
				throw ParsingException.NewParsingException(Parse_Error.peCharExpected, _Position, "\"");

			string strID = strIDB.ToString();

			OutputID(Operation_IDs.OI_STRING, strID, nPos);
			EXP_TreeNode node = NewTreeNode();

			node._Position = nPos;
			node._OperationID = Operation_IDs.OI_STRING;
			node._Value = strB.ToString();

			return node;
		}

		private EXP_TreeNode DoNumber()
		{
			int nPos = _Position;
			char ch = _ExpressionChars[_Position];

			while (Char.IsDigit(ch) || (ch == '.'))
				ch = _ExpressionChars[++_Position];

			EXP_TreeNode node = NewTreeNode();

			node._Position = nPos;
			node._OperationID = Operation_IDs.OI_NUMBER;

			string ns = new String(_ExpressionChars, nPos, _Position - nPos);
			node._Value = double.Parse(ns);

			OutputID(Operation_IDs.OI_NUMBER, ns, nPos);

			return node;
		}

		private EXP_TreeNode DoFunctionID()
		{
			int nPos = _Position;
			char ch = _ExpressionChars[_Position];

			while (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
				ch = _ExpressionChars[++_Position];

			string strID = new string(_ExpressionChars, nPos, _Position - nPos);

			return DoFunction(strID);
		}

		private EXP_TreeNode DoFunction(string strID)
		{
			Operation_IDs oID = Operation_IDs.OI_USERDEFINE;

			EXP_TreeNode node = null;

			string strLower = strID.ToLower();

			if (strLower == "true" || strLower == "false")
			{
				node = NewTreeNode();
				node._Position = _Position - strID.Length;
				node._OperationID = Operation_IDs.OI_BOOLEAN;
				node._Value = bool.Parse(strLower);

				OutputID(Operation_IDs.OI_BOOLEAN, strID, node._Position);
			}
			else
				node = GetFunctionNode(oID, strID);

			return node;
		}

		EXP_TreeNode GetFunctionNode(Operation_IDs funcID, string strID)
		{
			EXP_TreeNode node = null;
			EXP_TreeNode nodeTemp = null;
			ArrayList paramBase = new ArrayList(4);

			int nStartFunction = _Position - strID.Length;

			OutputID(Operation_IDs.OI_USERDEFINE, strID, nStartFunction);

			if (_ExpressionChars[_Position] == '(')	//有参数
			{
				OutputIDToSubLevel();
				OutputID(Operation_IDs.OI_LBRACKET, "(", _Position);

				do
				{
					_Position++;
					SkipSpaces();

					nodeTemp = DoExpression();

					if (nodeTemp != null)
					{
						paramBase.Add(nodeTemp);

						SkipSpaces();
					}
					else
						break;

					if (_ExpressionChars[_Position] == ',')
						OutputID(Operation_IDs.OI_COMMA, ",", _Position);
				}
				while(_ExpressionChars[_Position] == ',');

				if (_ExpressionChars[_Position] == ')')
				{
					OutputID(Operation_IDs.OI_RBRACKET, ")", _Position);
					OutputIDToParentLevel();

					_Position++;
					node = NewTreeNode();
					node._Position = nStartFunction;
					node._Params = paramBase;
					node._OperationID = funcID;

					if (funcID == Operation_IDs.OI_USERDEFINE)
						node._FunctionName = strID;
				}
				else
					throw ParsingException.NewParsingException(Parse_Error.peCharExpected, _Position, ")");

				SkipSpaces();
			}
			else	//没有参数
			{
				node = NewTreeNode();
				node._Position = nStartFunction;
				node._Params = paramBase;
				node._OperationID = funcID;

				if (funcID == Operation_IDs.OI_USERDEFINE)
					node._FunctionName = strID;
			}

			return node;
		}

		private object VExp(EXP_TreeNode node)
		{
			object oValue = null;

			if (node != null)
			{
				try
				{
					switch(node._OperationID)
					{
						case Operation_IDs.OI_NUMBER:
						case Operation_IDs.OI_STRING:
						case Operation_IDs.OI_NEG:
						case Operation_IDs.OI_BOOLEAN:
						case Operation_IDs.OI_DATETIME:
							oValue = node._Value;
							break;
						case Operation_IDs.OI_ADD:
							oValue = AddOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_MINUS:
							{
								object p1 = VExp(node._Left);
								object p2 = VExp(node._Right);

								CheckOperand(p1, p2, node._Position);
								oValue = (double)p1 - (double)p2;
							}
							break;
						case Operation_IDs.OI_MUL:
							{
								object p1 = VExp(node._Left);
								object p2 = VExp(node._Right);

								CheckOperand(p1, p2, node._Position);
								oValue = (double)p1 * (double)p2;
							}
							break;
						case Operation_IDs.OI_DIV:
							{
								object p1 = VExp(node._Left);
								object p2 = VExp(node._Right);

								CheckOperand(p1, p2, node._Position);
								oValue = (double)p1 / (double)p2;
							}
							break;
						case Operation_IDs.OI_LOGICAL_OR:
							{
								oValue = (bool)VExp(node._Left);
								object oRight = (bool)false;

								if (Optimize == false || (bool)oValue == false)
									oRight = VExp(node._Right);

								CheckOperand(oValue, oRight, node._Position);
								oValue = (bool)oValue || (bool)oRight;
							}
							break;
						case Operation_IDs.OI_LOGICAL_AND:
							{
								oValue = (bool)VExp(node._Left);
								object oRight = (bool)true;

								if (Optimize == false || (bool)oValue == true)
									oRight = VExp(node._Right);

								CheckOperand(oValue, oRight, node._Position);
								oValue = (bool)oValue && (bool)oRight;
							}
							break;
						case Operation_IDs.OI_NOT:
							oValue = VExp(node._Right);
							CheckOperand(oValue, node._Position);
							oValue = !(bool)oValue;
							break;
						case Operation_IDs.OI_GREAT:
							oValue = CompareGreatOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_GREATEQUAL:
							oValue = CompareGreatEqualOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_LESS:
							oValue = CompareLessOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_LESSEQUAL:
							oValue = CompareLessEqualOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_NOT_EQUAL:
							oValue = CompareNotEqualOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_EQUAL:
							oValue = CompareEqualOP(VExp(node._Left), VExp(node._Right), node._Position);
							break;
						case Operation_IDs.OI_USERDEFINE:
							{
								ParamObject[] po = GetParamArray(node._Params);
								oValue = CalculateFunction(node._FunctionName, po, this);
							}
							break;
						default:
							throw ParsingException.NewParsingException(Parse_Error.peInvalidOperator, node._Position);
					}
				}
				catch//(System.InvalidCastException ex)
				{
					throw ParsingException.NewParsingException(Parse_Error.peTypeMismatch, node._Position);
				}
			}

			return oValue;
		}

		private object CalculateFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj)
		{
			object oValue = null;

			try
			{
				switch(strFuncName.ToLower())
				{
					case "now":
						oValue = DateTime.Now;
						break;
					case "today":
						oValue = DateTime.Today;
						break;
					case "dateinterval.day":
						oValue = "d";
						break;
					case "dateinterval.hour":
						oValue = "h";
						break;
					case "dateinterval.minute":
						oValue = "n";
						break;
					case "dateinterval.second":
						oValue = "s";
						break;
					case "dateinterval.millisecond":
						oValue = "ms";
						break;
                    case "datediff":
                        oValue = DoDateDiff(arrParams);
                        break;
                    case "mindate":
						oValue = DateTime.MinValue;
						break;
					case "maxdate":
						oValue = DateTime.MaxValue;
						break;
					default:
					{
						if (UserFunctions != null)
							oValue = UserFunctions.CalculateUserFunction(strFuncName, arrParams, this);

						break;
					}
				}

				return oValue;
			}
			catch(System.Exception ex)
			{
				throw new ApplicationException(string.Format("函数{0}错误，{1}", strFuncName, ex.Message));
			}
		}

		private object DoDateDiff(ParamObject[] arrParams)
		{
			CheckParamsLength(arrParams, 3);

			CheckParameterType(arrParams, 0, typeof(System.String));
			CheckParameterType(arrParams, 1, typeof(System.DateTime));
			CheckParameterType(arrParams, 2, typeof(System.DateTime));

			DateTime startTime = (DateTime)arrParams[1].Value;
			DateTime endTime = (DateTime)arrParams[2].Value;

			TimeSpan ts = endTime - startTime;

			double result = 0;

			string intervalType = arrParams[0].Value.ToString().ToLower();

			switch(intervalType)
			{
				case "d":
					result = ts.TotalDays;
					break;
				case "h":
					result = ts.TotalHours;
					break;
				case "n":
					result = ts.TotalMinutes;
					break;
				case "s":
					result = ts.TotalSeconds;
					break;
				case "ms":
					result = ts.TotalMilliseconds;
					break;
				default:
					throw new System.ApplicationException(string.Format("\"{0}\"不是DateDiff所支持的时间间隔类型", intervalType));
			}

			return result;
		}


		private void CheckParamsLength(ParamObject[] arrParams, int nLimit)
		{
			if (arrParams.Length < nLimit)
				throw new System.ApplicationException(string.Format("参数个数应该不少于{0}个", nLimit));
		}

		private void CheckParameterType(ParamObject[] arrParams, int paramIndex, System.Type expectedType)
		{
			ParamObject po = arrParams[paramIndex];

			if (po.Value != null)
				if (po.Value.GetType().IsSubclassOf(expectedType) == false && po.Value.GetType() != expectedType)
					throw new System.ApplicationException(string.Format("第{0}的参数类型错误，应该是{1}类型", paramIndex + 1, expectedType.Name));
		}

		private ParamObject[] GetParamArray(ArrayList arrParams)
		{
			ParamObject[] result = new ParamObject[arrParams.Count];

			for (int i = 0; i < arrParams.Count; i++)
			{
				EXP_TreeNode node = (EXP_TreeNode) arrParams[i];

				result[i] = new ParamObject(VExp(node), node._Position);
			}

			return result;
		}

		private void CheckOperand(object p, int nPos)
		{
			if (p == null)
				throw ParsingException.NewParsingException(Parse_Error.peNeedOperand, nPos);
		}

		private void CheckOperand(object p1, object p2, int nPos)
		{
			if (p1 == null || p2 == null)
				throw ParsingException.NewParsingException(Parse_Error.peNeedOperand, nPos);
		}

		private object AddOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString() + p2.ToString();
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 + (double)p2;
			}
		}

		private object CompareGreatOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString().CompareTo(p2.ToString()) > 0;
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 > (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 > (double)p2;
			}
		}

		private object CompareLessOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString().CompareTo(p2.ToString()) < 0;
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 < (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 < (double)p2;
			}
		}

		private object CompareEqualOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString() == p2.ToString();
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 == (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 == (double)p2;
			}
		}

		private object CompareNotEqualOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString() != p2.ToString();
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 != (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 != (double)p2;
			}
		}

		private object CompareGreatEqualOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString().CompareTo(p2.ToString()) >= 0;
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 >= (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 >= (double)p2;
			}
		}

		private object CompareLessEqualOP(object p1, object p2, int nPos)
		{
			if (p1 is System.String || p2 is System.String)
				return p1.ToString().CompareTo(p2.ToString()) <= 0;
			else
			if (p1 is System.DateTime && p2 is System.DateTime)
			{
				CheckOperand(p1, p2, nPos);
				return (DateTime)p1 <= (DateTime)p2;
			}
			else
			{
				CheckOperand(p1, p2, nPos);
				return (double)p1 <= (double)p2;
			}
		}

		/// <summary>
		/// 生成一个新的二叉树节点
		/// </summary>
		private EXP_TreeNode NewTreeNode()
		{
			EXP_TreeNode node = new EXP_TreeNode();

			node._Position = _Position;

			return node;
		}

		/// <summary>
		/// 生成一个新的二叉树节点
		/// </summary>
		/// <param name="left">左子树</param>
		/// <param name="right">右子树</param>
		/// <param name="oID">操作类型</param>
		/// <returns></returns>
		private EXP_TreeNode NewTreeNode(EXP_TreeNode left, EXP_TreeNode right, Operation_IDs oID)
		{
			EXP_TreeNode node = NewTreeNode();

			node._Left = left;
			node._Right = right;
			node._OperationID = oID;

			return node;
		}

		private EXP_TreeNode NewTreeNode(EXP_TreeNode left, EXP_TreeNode right, Operation_IDs oID, int nPosition)
		{
			EXP_TreeNode node = NewTreeNode(left, right, oID);

			node._Position = nPosition;

			return node;
		}

		void SkipSpaces()
		{
			while(_ExpressionChars[_Position] <= ' ' && _ExpressionChars[_Position] != '\0')
				_Position++;
		}
	}
}
