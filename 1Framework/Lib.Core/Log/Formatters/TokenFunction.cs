using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Lib.Core.Properties;

namespace Lib.Log
{
    /// <summary>
    /// 抽象基类，各种格式化函数
    /// </summary>
    internal abstract class TokenFunction
    {
        private string startDelimiter = string.Empty;
        private string endDelimiter = ")}";

        /// <summary>
        /// Initializes an instance of a TokenFunction with a start delimiter and the default end delimiter.
        /// </summary>
        /// <param name="tokenStartDelimiter">Start delimiter.</param>
        protected TokenFunction(string tokenStartDelimiter)
        {
            if (tokenStartDelimiter == null || tokenStartDelimiter.Length == 0)
            {
                throw new ArgumentNullException("tokenStartDelimiter");
            }

            this.startDelimiter = tokenStartDelimiter;
        }

        /// <summary>
        /// Initializes an instance of a TokenFunction with a start and end delimiter.
        /// </summary>
        /// <param name="tokenStartDelimiter">Start delimiter.</param>
        /// <param name="tokenEndDelimiter">End delimiter.</param>
        protected TokenFunction(string tokenStartDelimiter, string tokenEndDelimiter)
        {
            if (tokenStartDelimiter == null || tokenStartDelimiter.Length == 0)
            {
                throw new ArgumentNullException("tokenStartDelimiter");
            }
            if (tokenEndDelimiter == null || tokenEndDelimiter.Length == 0)
            {
                throw new ArgumentNullException("tokenEndDelimiter");
            }

            this.startDelimiter = tokenStartDelimiter;
            this.endDelimiter = tokenEndDelimiter;
        }

        /// <summary>
        /// Searches for token functions in the message and replace all with formatted values.
        /// </summary>
        /// <param name="messageBuilder">Message template containing tokens.</param>
        /// <param name="log">Log entry containing properties to format.</param>
        public virtual void Format(StringBuilder messageBuilder, LogEntity log)
        {
            int pos = 0;
            while (pos < messageBuilder.Length)
            {
                string messageString = messageBuilder.ToString();
                if (messageString.IndexOf(this.startDelimiter) == -1)
                {
                    break;
                }

                string tokenTemplate = GetInnerTemplate(pos, messageString);
                string tokenToReplace = this.startDelimiter + tokenTemplate + this.endDelimiter;
                pos = messageBuilder.ToString().IndexOf(tokenToReplace);

                string tokenValue = FormatToken(tokenTemplate, log);

                messageBuilder.Replace(tokenToReplace, tokenValue);
            }
        }

        /// <summary>
        /// Abstract method to process the token value between the start and end delimiter.
        /// </summary>
        /// <param name="tokenTemplate">Token value between the start and end delimiters.</param>
        /// <param name="log">Log entry to process.</param>
        /// <returns>Formatted value to replace the token.</returns>
        public abstract string FormatToken(string tokenTemplate, LogEntity log);

        /// <summary>
        /// Returns the template in between the paratheses for a token function.
        /// Expecting tokens in this format: {keyvalue(myKey1)}.
        /// </summary>
        /// <param name="startPos">Start index to search for the next token function.</param>
        /// <param name="message">Message template containing tokens.</param>
        /// <returns>Inner template of the function.</returns>
        protected virtual string GetInnerTemplate(int startPos, string message)
        {
            int tokenStartPos = message.IndexOf(this.startDelimiter, startPos) + this.startDelimiter.Length;
            int endPos = message.IndexOf(this.endDelimiter, tokenStartPos);
            return message.Substring(tokenStartPos, endPos - tokenStartPos);
        }
    }

    internal class TimeStampToken : TokenFunction
    {
        private const string LocalStartDelimiter = "local";
        private const string LocalStartDelimiterWithFormat = "local:";

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeStampToken"/>.
        /// </summary>
        public TimeStampToken()
            : base("{timestamp(")
        {
        }

        /// <summary>
        /// Formats the timestamp property with the specified date time format string.
        /// </summary>
        /// <param name="tokenTemplate">Date time format string.</param>
        /// <param name="log">Log entry containing the timestamp.</param>
        /// <returns>Returns the formatted time stamp.</returns>
        public override string FormatToken(string tokenTemplate, LogEntity log)
        {
            string result = null;
            if (tokenTemplate.Equals(TimeStampToken.LocalStartDelimiter, System.StringComparison.InvariantCultureIgnoreCase))
            {
                System.DateTime localTime = log.TimeStamp.ToLocalTime();
                result = localTime.ToString();
            }
            else if (tokenTemplate.StartsWith(TimeStampToken.LocalStartDelimiterWithFormat, System.StringComparison.InvariantCultureIgnoreCase))
            {
                string formatTemplate = tokenTemplate.Substring(TimeStampToken.LocalStartDelimiterWithFormat.Length);
                System.DateTime localTime = log.TimeStamp.ToLocalTime();
                result = localTime.ToString(formatTemplate, CultureInfo.CurrentCulture);
            }
            else
            {
                result = log.TimeStamp.ToString(tokenTemplate, CultureInfo.CurrentCulture);
            }
            return result;
        }
    }

    internal class DictionaryToken : TokenFunction
    {
        private const string DictionaryKeyToken = "{key}";
        private const string DictionaryValueToken = "{value}";

        /// <summary>
        /// Initializes a new instance of a <see cref="DictionaryToken"/>.
        /// </summary>
        public DictionaryToken()
            : base("{dictionary(")
        {
        }

        /// <summary>
        /// Iterates through each entry in the dictionary and display the key and/or value.
        /// </summary>
        /// <param name="tokenTemplate">Template to repeat for each key/value pair.</param>
        /// <param name="log">Log entry containing the extended properties dictionary.</param>
        /// <returns>Repeated template for each key/value pair.</returns>
        public override string FormatToken(string tokenTemplate, LogEntity log)
        {
            StringBuilder dictionaryBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> entry in log.ExtendedProperties)
            {
                StringBuilder singlePair = new StringBuilder(tokenTemplate);
                string keyName = string.Empty;
                if (entry.Key != null)
                {
                    keyName = entry.Key.ToString();
                }
                singlePair.Replace(DictionaryToken.DictionaryKeyToken, keyName);

                string keyValue = string.Empty;
                if (entry.Value != null)
                {
                    keyValue = entry.Value.ToString();
                }
                singlePair.Replace(DictionaryToken.DictionaryValueToken, keyValue);

                dictionaryBuilder.Append(singlePair.ToString());
            }

            return dictionaryBuilder.ToString();
        }
    }

    internal class ReflectedPropertyToken : TokenFunction
    {
        private const string StartDelimiter = "{property(";

        /// <summary>
        /// Constructor that initializes the token with the token name
        /// </summary>
        public ReflectedPropertyToken()
            : base(ReflectedPropertyToken.StartDelimiter)
        {
        }

        /// <summary>
        /// Searches for the reflected property and returns its value as a string
        /// </summary>
        public override string FormatToken(string tokenTemplate, LogEntity log)
        {
            Type logType = log.GetType();
            PropertyInfo property = logType.GetProperty(tokenTemplate);
            if (property != null)
            {
                object value = property.GetValue(log, null);
                return value != null ? value.ToString() : string.Empty;
            }
            else
            {
                return String.Format(Resource.Culture, Resource.ReflectedPropertyTokenNotFound, tokenTemplate);
            }
        }
    }

    internal class KeyValueToken : TokenFunction
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="TimeStampToken"/>.
        /// </summary>
        public KeyValueToken()
            : base("{keyvalue(")
        {
        }

        /// <summary>
        /// Gets the value of a property from the log entry.
        /// </summary>
        /// <param name="tokenTemplate">Dictionary key name.</param>
        /// <param name="log">Log entry containing with extended properties dictionary values.</param>
        /// <returns>The value of the key from the extended properties dictionary, or <see langword="null"/> 
        /// (Nothing in Visual Basic) if there is no entry with that key.</returns>
        public override string FormatToken(string tokenTemplate, LogEntity log)
        {
            string propertyString = string.Empty;
            object propertyObject;

            if (log.ExtendedProperties.TryGetValue(tokenTemplate, out propertyObject))
            {
                propertyString = propertyObject.ToString();
            }

            return propertyString;
        }
    }
}
