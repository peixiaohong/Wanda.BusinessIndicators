using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.DAL;
using Wanda.BusinessIndicators.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.BLL
{
    public class B_SubscriptionOperator : BizOperatorBase<B_Subscription>
    {
        public static readonly B_SubscriptionOperator Instance = PolicyInjection.Create<B_SubscriptionOperator>();

        private static B_SubscriptionAdapter _bSubscriptionAdapter = AdapterFactory.GetAdapter<B_SubscriptionAdapter>();

        protected override BaseAdapterT<B_Subscription> GetAdapter()
        {
            return _bSubscriptionAdapter;
        }

        public IList<B_Subscription> GetSubscriptionList()
        {
            IList<B_Subscription> result = _bSubscriptionAdapter.GeSubscriptionList();
            return result;
        }

        public Guid AddSubscription(B_Subscription data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_Subscription GetSubscription(Guid SubscriptionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SubscriptionID == null, "Argument Subscription is Empty");
            return base.GetModelObject(SubscriptionID);
        }
        public Guid UpdateSubscription(B_Subscription data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }
        public Guid RemoveSubscription(Guid SubscriptionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SubscriptionID == null ? true : false, "Argument Subscription is Empty");
            Guid result = base.RemoveObject(SubscriptionID);
            return result;
        }
        public IList<B_Subscription> GeSubscriptionListByFile(string id, int month, int year, string name)
        {
            IList<B_Subscription> result = _bSubscriptionAdapter.GeSubscriptionListByFile(id.ToGuid(),month,year,name);
            return result;
        }
        public IList<B_Subscription> GetallSubscriptionList()
        {
            //int year = DateTime.Now.AddMonths(-1).Year;
            //int month = DateTime.Now.AddMonths(-1).Month;
            //string newtime = ConfigurationManager.AppSettings["CheckTime"];
            //if (newtime!=null)
            //{
            //    if (newtime.Trim()!="")
            //    {
            //        DateTime Time = DateTime.Parse(newtime.Trim());
            //        year = Time.Year;
            //        month = Time.Month;
            //    }
            //}
            int FinYear = 0;
            int FinMonth = 0;

            //DateTime datetime = new DateTime();
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CheckTime"]))
            //{
            //    datetime = DateTime.Parse(ConfigurationManager.AppSettings["CheckTime"]);
            //    FinYear = datetime.Year;
            //    FinMonth = datetime.Month;
            //}
            //else
            //{
            //    FinYear = DateTime.Now.AddMonths(-1).Year;
            //    FinMonth = DateTime.Now.AddMonths(-1).Month;
            //}
            DateTime datetime = StaticResource.Instance.GetReportDateTime();
            FinMonth = datetime.Month;
            FinYear = datetime.Year;
            IList<B_Subscription> result = _bSubscriptionAdapter.GetAllSubscriptionList(FinMonth, FinYear);
            return result;
        }
    }
}
