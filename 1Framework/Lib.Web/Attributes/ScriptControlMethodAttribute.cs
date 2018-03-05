using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    /// <summary>
    /// 标志此控件方法可以被客户端回调
    /// Signifies that this method should be exposed as a client callback 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptControlMethodAttribute : Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Exposing this for user convenience")]
        private static ScriptControlMethodAttribute Yes = new ScriptControlMethodAttribute(true);
        private static ScriptControlMethodAttribute No = new ScriptControlMethodAttribute(false);
        private static ScriptControlMethodAttribute Default = No;

        #region [ Fields ]

        private bool _isScriptMethod;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new ScriptControlMethodAttribute
        /// </summary>
        public ScriptControlMethodAttribute()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new ScriptControlMethodAttribute
        /// </summary>
        /// <param name="isScriptMethod"></param>
        public ScriptControlMethodAttribute(bool isScriptMethod)
        {
            _isScriptMethod = isScriptMethod;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Whether this is a valid ScriptMethod
        /// </summary>
        public bool IsScriptMethod
        {
            get { return _isScriptMethod; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tests for object equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            ScriptControlMethodAttribute other = obj as ScriptControlMethodAttribute;
            if (other != null)
            {
                return other._isScriptMethod == _isScriptMethod;
            }
            return false;
        }

        /// <summary>
        /// Gets a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _isScriptMethod.GetHashCode();
        }

        /// <summary>
        /// Gets whether this is the default value for this attribute
        /// </summary>
        /// <returns></returns>
        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }

        #endregion
    }
}
