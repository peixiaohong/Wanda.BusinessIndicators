using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Validation
{
    public interface IValidator<T>
    {
        ValidateResult Validate(T data);
    }

    //TODO, 初步想法是使用Model 的Attribute定制一些服务器端的验证
    public class ModelValidatorBase<T> : IValidator<T>
    {
        public virtual ValidateResult Validate(T data)
        {
            return ValidateResult.CreateNormal();
        }
    }

    /// <summary>
    /// 常用的一些验证方法
    /// </summary>
    public static class ValidatorHelper
    {
        /// <summary>
        /// 必填项验证
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Required(object field, string fieldName, ref ValidateResult result)
        {
            if (field == null
                || (field is string && string.IsNullOrWhiteSpace((string)field)))
            {
                result.AddError(string.Format("{0} 不能为空", fieldName));
                return false;
            }
            return true;
        }

        /// <summary>
        /// string 长度控制
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool LengthControl(string field, string fieldName, int minLength, int maxLength, ref ValidateResult result)
        {
            if (!Required(field, fieldName, ref result))
            {
                return false;
            }

            if (minLength >= maxLength)
            {
                throw new ArgumentException(string.Format("minLength {0} > maxLength {1}", minLength, maxLength));
            }


            int length = field.Length;

            if (length > maxLength || length < minLength)
            {
                result.AddError(string.Format("{0} 长度不在范围内({1} ~ {2})", fieldName, minLength, maxLength));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 唯一性验证
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fieldName"></param>
        /// <param name="CheckNameUnique">委托， 传入一个验证model是否唯一的方法</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool UniqueField(object model, string fieldName, Predicate<object> CheckNameUnique, ref ValidateResult result)
        {
            if (CheckNameUnique==null)
            {
                throw new ArgumentException("CheckNameUnique委托不能为空！");
            }
            if (!CheckNameUnique(model))
            {
                result.AddError(string.Format("{0} 值已存在，不能重复 ", fieldName));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 值范围控制验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool RangeControl<T>(T field, string fieldName, T min, T max, ref ValidateResult result)
            where T : IComparable
        {


            if (min.CompareTo(max) != -1)
            {
                throw new ArgumentException(string.Format("min  {0} > max  {1}", min, max));
            }


            if (field.CompareTo(max) > 0 || field.CompareTo(min) < 0)  // field > max or < min
            {
                result.AddError(string.Format("{0} 值不在范围内({1} ~ {2})", fieldName, min, max));
                return false;
            }

            return true;
        }

    }
}
