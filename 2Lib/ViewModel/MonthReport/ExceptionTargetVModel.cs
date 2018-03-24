using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.Lib.Data.AppBase;
using LJTH.BusinessIndicators.Model;
using Lib.Data;

namespace LJTH.BusinessIndicators.ViewModel
{
    public class ExceptionTargetVModel : BaseModel, IBaseComposedModel
    {

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }
        [ORFieldMapping("TargetName")]
        public string TargetName { get; set; }

    }
    public class ExceptionCompanyVModel : IBaseComposedModel
    {

        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("ExceptionTargetID")]
        public Guid ExceptionTargetID { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("ModifierName")]
        public string ModifierName { get; set; }

        [ORFieldMapping("ModifyTime")]
        public DateTime? ModifyTime { get; set; }

        [ORFieldMapping("OpeningTime")]
        public DateTime? OpeningTime { get; set; }


        [ORFieldMapping("ExceptionType")]
        public int ExceptionType { get; set; }
    }
    public class LastExceptionCompanyVModel : BaseModel, IBaseComposedModel
    {

        [ORFieldMapping("ID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("OpeningTime")]
        public DateTime? OpeningTime { get; set; }

        //[ORFieldMapping("ModifyTime")]
        //public DateTime? ModifyTime { get; set; }

        //[ORFieldMapping("CreatorName")]
        //public DateTime? CreatorName { get; set; }

    }

    public class ExceptionTargetUpdateList
    {
        public Guid TargetID { get; set; }

        public List<ExceptionCompanyVModel> ExcepListA { get; set; }

        public List<ExceptionCompanyVModel> ExcepListB { get; set; }

        public List<LastExceptionCompanyVModel> ExcepListC { get; set; }
    }
    public class ExceptionReplaceList
    {
        public Guid TargetID { get; set; }
        public List<ExceptionCompanyVModel> ExcepListA { get; set; }
        public List<LastExceptionCompanyVModel> ExcepListB { get; set; }
    }



    public class ContrastCompanyVModel : IBaseComposedModel
    {
        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("NActualAmmount")]
        public decimal NActualAmmount { get; set; }

        [ORFieldMapping("TargetID")]
        public Guid TargetID { get; set; }

    }
    public class ContrastAllCompanyVM : IBaseComposedModel
    {
        [ORFieldMapping("CompanyID")]
        public Guid CompanyID { get; set; }

        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        [ORFieldMapping("OpeningTime")]
        public DateTime OpeningTime { get; set; }
                [ORFieldMapping("NAccumulativeActualAmmount")]
        public decimal NAccumulativeActualAmmount { get; set; }

    }



}
