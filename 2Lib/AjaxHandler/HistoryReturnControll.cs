using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Common;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;

namespace Wanda.BusinessIndicators.Web.AjaxHandler
{
    public class HistoryReturnControll : BaseController
    {

         [LibAction]
        public List<HistoryReturnDateVModel> GetHistoryReturnList(string SystemID, int Year)
        {
            V_HistoryReturnDateOperator _historyReturn = new V_HistoryReturnDateOperator();
            return _historyReturn.GetList(SystemID.ToGuid(), Year);
        }





    }
}
