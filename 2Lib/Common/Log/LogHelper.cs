using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using Lib.Log;

namespace LJTH.BusinessIndicators.Common
{

    //日志的分类
    // 1 开发日志
    //      此类日志的特点是是开发阶段， 开发人员为方便跟踪程序的运行情况而记录的日志
    //      A BLL日志（autolog) -- 保存在数据库中
    //      B 自定义日志 --  保存在数据库中
    //      * 如果数据库保存失败， 需要有本地的备用文本日志
    // 2 系统管理日志
    //      A 用户登录、退出
    //      B 用户操作
    //      C 管理员操作

    public static class LogHelper
    {
        /// <summary>
        /// 开发期间的BLL运行日志
        /// </summary>
        private static Logger _autoLogger = null;
        public static Logger AutoLogger
        {
            get
            {
                if (_autoLogger == null)
                {
                    _autoLogger = LoggerFactory.Create("AutoLog");
                }
                return _autoLogger;
            }
        }

        /// <summary>
        /// 开发期间的自定义日志
        /// </summary>
        private static Logger _devLogger = null;
        public static Logger DevLogger
        {
            get
            {
                if (_devLogger == null)
                {
                    _devLogger = LoggerFactory.Create("DevLog");
                }
                return _devLogger;
            }
        }


        /// <summary>
        /// 运营期间的自定义日志
        /// </summary>
        private static Logger _operatorLogger = null;
        public static Logger OperatorLogger
        {
            get
            {
                if (_operatorLogger == null)
                {
                    _operatorLogger = LoggerFactory.Create("SysLog");
                }
                return _operatorLogger;
            }
        }
    }
}
