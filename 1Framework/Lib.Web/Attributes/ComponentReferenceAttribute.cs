using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    /// <summary>
    /// Signifies that this property references a ScriptComponent
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ComponentReferenceAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ComponentReferenceAttribute()
        {
        }
    }
}
