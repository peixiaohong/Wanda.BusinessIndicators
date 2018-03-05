using Lib.Expression;
using Lib.Web;
using Lib.Web.Json;
using Lib.Web.MVC.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanda.BusinessIndicators.Web.AjaxHandler
{
    public class ExpressionParserDemo : BaseController
    {
        [LibAction]
        public decimal Calculate(string BizContext, string Expression)
        {
            Hashtable bizCon = JsonHelper.Deserialize<Hashtable>(BizContext);
            ExpressionParser parser = new ExpressionParser(bizCon);

            
            return parser.CacluateExpression(Expression);
        }
    }
}
