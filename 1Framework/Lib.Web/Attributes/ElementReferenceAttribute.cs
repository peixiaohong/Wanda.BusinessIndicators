using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    /// <summary>
    /// Specifies this property is an element reference and should be converted during serialization.
    /// The default (e.g. cases without this attribute) will generate the element's ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ElementReferenceAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new ElementReferenceAttribute
        /// </summary>
        public ElementReferenceAttribute()
        {
        }
    }
}
