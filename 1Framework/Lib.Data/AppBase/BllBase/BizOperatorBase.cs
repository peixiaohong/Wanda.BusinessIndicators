using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Validation;
using Lib.Data;
using System.Data;
using Lib.Core;
using System.Web;

namespace LJTH.Lib.Data.AppBase
{

    public abstract class BizOperatorBase : MarshalByRefObject
    {
        public event Func<string> GetUserName;

        /// <summary>
        /// 如果使用了指定的委托， 则通过委托返回； 否则使用内置的通过HttpContext.Current来取登录用户名（或SSO用户名）
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentUserName()
        {
            if (GetUserName == null)
            {
                return GetWebLoginUserName();
            }

            return GetUserName();
        }

        private string GetWebLoginUserName()
        {
            string result = "unknown";
            if (HttpContext.Current == null)
            {
                return result;
            }
            // WandaSSO用法
            string ssoUsername = HttpContext.Current.Items["WD_SSO_UserName"] != null ? HttpContext.Current.Items["WD_SSO_UserName"].ToString() : string.Empty;
            string strUserName = HttpContext.Current.User.Identity != null ? HttpContext.Current.User.Identity.Name : ssoUsername;

            if (string.IsNullOrEmpty(strUserName))
            {
                return result;
            }
            result = strUserName;
            return result;
        }

        public event Func<DateTime> GetNowTime;

        protected DateTime GetDateTimeNow()
        {
            if (GetNowTime == null)
            {
                return DateTime.Now;
            }

            return GetNowTime();
        }


    }
    /// <summary>
    /// 封装常见的增删查改的处理。 避免在Operator中写大量重复代码.
    /// 这些增删查改操作， 依靠在Adapter定义的接口IBasicDataAccess<T>实现
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class BizOperatorBase<TModel> : BizOperatorBase
        where TModel : BaseModel, new()
    {

        protected TModel GetModelObject(Guid id)
        {
            TModel result = GetAdapter().GetModelByID(id);

            if (result == null)
            {
                throw new ArgumentException(
                    string.Format("Cannot find '{0}' object by {0}ID. {0}ID={1}", typeof(TModel), id));
            }

            return result;
        }



        /// <summary>
        /// 根据多个ID返回多个结果
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">
        /// 如果返回结果的数量不和传入参数的数量一致， 则抛出异常ApplicationException
        /// </exception> 
        protected IList<TModel> GetBatchModelObjects(params  Guid[] ids)
        {

            ExceptionHelper.TrueThrow<ArgumentException>(ids == null || ids.Length == 0, "请至少输入一个值");

            IList<TModel> result = GetAdapter().GetBatchModelObjects(ids);

            if (result.Count != ids.Count())
            {
                throw new ApplicationException(
                    string.Format("Some id(s) are invalid for the return length are not matched! return {0}, expected {1}", result.Count, ids.Count()));
            }

            return result;
        }

        protected Guid AddNewModel(TModel data, params IValidator<TModel>[] validators)
        {
            var adapter = GetAdapter();

            ValidateResult validateResult = ValidateResult.CreateNormal();

            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    validateResult.AddErrors(validator.Validate(data).AbnormalMessages);
                }
            }


            if (validateResult.IsPass)
            {

                if (string.IsNullOrEmpty(data.CreatorName))
                {
                    data.CreatorName = GetCurrentUserName();
                }
                Guid newID = data.ID;
                if (newID == null || newID == Guid.Empty)
                {
                    newID=Guid.NewGuid();
                    data.ID = newID;
                }

                data.CreateTime = GetDateTimeNow();
                data.IsDeleted = false;
                data.ModifierName = data.CreatorName;
                data.ModifyTime = data.CreateTime;

                if (adapter.Insert(data) > 0)
                {
                    return data.ID;  //没有必要返回整个Entity，带来不必要的序列化麻烦
                }

                throw new ApplicationException(typeof(TModel).Name + " Data inserted but there is no return count!");
            }
            else
            {
                throw new ValidationException(validateResult.AbnormalSummary);
            }


        }

        protected Guid UpdateModelObject(TModel model, params IValidator<TModel>[] validators)
        {
            var adapter = GetAdapter();

            ValidateResult validateResult = ValidateResult.CreateNormal();

            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    validateResult.AddErrors(validator.Validate(model).AbnormalMessages);
                }
            }


            if (validateResult.IsPass)
            {
                model.ModifierName = GetCurrentUserName();
                model.ModifyTime = GetDateTimeNow();
                int count = adapter.Update(model);

                if (count == 0)
                {
                    throw new ArgumentException(
                        string.Format("Cannot find '{0}' object by {0}ID. {0}ID={1}", typeof(TModel), model.ID));
                }
            }
            else
            {
                throw new ValidationException(validateResult.AbnormalSummary);
            }

            return model.ID;
        }

        public Guid RemoveObject(Guid id)
        {
            var adapter = GetAdapter();
            TModel model = GetModelObject(id);

            if (adapter is IUsage)
            {
                bool IsInUse = ((IUsage)adapter).UsageCount(id) > 0;
                if (IsInUse)
                {
                    throw new ApplicationException(
                        string.Format(" '{0}' object could not been removed in case of being in using.. {0}ID={1}", typeof(TModel), id));
                }
            }
            model.ModifierName = GetCurrentUserName();
            model.ModifyTime = GetDateTimeNow();
            if (adapter.Remove(model) > 0)
            {
                return id;
            }
            throw new ApplicationException(
                string.Format(" '{0}' object could not been removed by {0}ID. {0}ID={1}", typeof(TModel), id));
        }

        protected abstract BaseAdapterT<TModel> GetAdapter();

    }

    public class BizOperatorFactory
    {
        public static T Create<T>()
            where T : BizOperatorBase
        {
            //return PolicyInjection.Create<T>();
            T result = Activator.CreateInstance<T>();

            return result;
        }

        public static T Create<T>(Func<string> getUser)
            where T : BizOperatorBase
        {
            T result = Activator.CreateInstance<T>();
            result.GetUserName += getUser;
            return result;
        }

        public static T Create<T>(Func<string> getUser, Func<DateTime> getNowTime)
            where T : BizOperatorBase
        {
            T result = Activator.CreateInstance<T>();
            result.GetUserName += getUser;
            result.GetNowTime += getNowTime;
            return result;
        }
    }
}
