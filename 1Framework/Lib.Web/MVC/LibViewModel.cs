using System;
using System.ComponentModel;
using Lib.Core;

namespace Lib.Web.Mvc
{
    [Serializable]
    public class LibViewModel
    {
        public LibViewModel()
        { }

        public LibViewModel(LibViewModelType modelType)
            : this(modelType, LibViewModelCode.Success)
        { }

        public LibViewModel(LibViewModelType modelType, LibViewModelCode resultCode)
            : this(modelType, resultCode, string.Empty)
        { }

        public LibViewModel(LibViewModelType resultModelType, LibViewModelCode resultCode, string errorMessage)
        {
            this.ModelType = resultModelType;
            this.ModelCode = resultCode;
            this.ErrorMessage = errorMessage;
        }

        public LibViewModelType ModelType { get; set; }

        public LibViewModelCode ModelCode { get; set; }

        public string ErrorMessage { get; set; }

        private bool isView = true;
        public bool IsView
        {
            get { return this.isView; }
            set { this.isView = value; }
        }

        public object ResultData { get; set; }



        public static LibViewModel CreateFailureJSONResponseViewModel(LibViewModelCode errorCode)
        {
            return new LibViewModel(LibViewModelType.Json, errorCode, errorCode.ToString());
        }

        public static LibViewModel CreateFailureJSONResponseViewModel(LibViewModelCode errorCode, string errorMesage)
        {
            return new LibViewModel(LibViewModelType.Json, errorCode, errorMesage);
        }
        public static LibViewModel CreateFailureJSONResponseViewModel(string errorMesage)
        {
            return new LibViewModel(LibViewModelType.Json, LibViewModelCode.UnhandledErrors, errorMesage);
        }
        public static LibViewModel CreateSuccessJSONResponseViewModel()
        {
            return new LibViewModel(LibViewModelType.Json, LibViewModelCode.Success);
        }

        public static LibViewModelCode GetExceptionCode(Exception ex)
        {
            if (ex is LoginUserNullException) { return LibViewModelCode.RequestLogin; }
            if (ex is InvalidActionException) { return LibViewModelCode.InvalidActionFlag; }
            if (ex is System.Data.SqlClient.SqlException) { return LibViewModelCode.DatabaseFailed; }
            if (ex is UnauthorizedAccessException) { return LibViewModelCode.NoPrivilege; }
            if (ex is JsonConvertFailException) { return LibViewModelCode.ConvertJsonFailed; }
            if (ex is MissingParameterException) { return LibViewModelCode.MissingParameter; }
            if (ex is ValidationException) { return LibViewModelCode.ValidationFailed; }

            return LibViewModelCode.UnhandledErrors;
        }
    }

    public enum LibViewModelCode : int
    {
        #region 系统错误 ( 小于零 0)

        [Description("没有登录或者会话已过期")]
        RequestLogin = 0,

        [Description("请求被拒绝")]
        RejectAccessPage = 1,

        [Description("没有权限访问此方法")]
        NoPrivilege = 2,

        [Description("数据库操作失败")]
        DatabaseFailed = 3,

        [Description("结果数据转化为JSON失败")]
        ConvertJsonFailed = 4,

        [Description("非法的Ajax Action")]
        InvalidActionFlag = 5,

        [Description("缺少某些参数")]
        MissingParameter = 6,

        [Description("没找到匹配的数据")]
        NoMatchedData = 7,


        [Description("数据验证出错")]
        ValidationFailed = 8,

        [Description("未知错误")]
        UnhandledErrors = 99,
        #endregion

        #region 成功

        Success = 200,

        #endregion

    }

    public enum LibViewModelType
    {
        /// <summary>
        /// 返回Json字符串
        /// </summary>
        Json = 0,

        /// <summary>
        /// 返回Html字符串
        /// </summary>
        Html = 1,

        /// <summary>
        /// 返回文件流
        /// </summary>
        File = 2,

        /// <summary>
        /// 拒绝访问
        /// </summary>
        Detect = 3
    }
}
