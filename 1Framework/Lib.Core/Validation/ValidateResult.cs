using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Validation
{
 

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidateResult
    {
        public ValidateResult()
        {
            IsPass = true;
            AbnormalMessages = new List<string>();
        }

        /// <summary>
        /// true represent validation pass
        /// </summary>
        public bool IsPass { get; private set; }

        /// <summary>
        /// all validation failure messages
        /// </summary>
        public IList<string> AbnormalMessages { get; private set; }

        /// <summary>
        /// validation failed times
        /// </summary>
        public int ErrorsCount
        {
            get { return AbnormalMessages.Count; }
        }


        /// <summary>
        /// all validation failure messages combined
        /// </summary>
        public string AbnormalSummary
        {
            get { return string.Join("\r\n", AbnormalMessages.ToArray()); }
        }

        /// <summary>
        /// 当添加error时， IsPass会设为false
        /// </summary>
        /// <param name="errorMsg"></param>
        public void AddError(string errorMsg)
        {
            IsPass = false;
            AbnormalMessages.Add(errorMsg);

        }

        /// <summary>
        /// 当添加error时， IsPass会设为false
        /// </summary>
        /// <param name="errorMsg"></param>
        public void AddErrors(IEnumerable<string> errorMsgs)
        {
            foreach (string errorMsg in errorMsgs)
            {
                AddError(errorMsg);
            }

        }

        /// <summary>
        /// represent the PASSED result
        /// </summary>
        public static ValidateResult CreateNormal()
        {
            return  new ValidateResult();
        }
    }

     
}
