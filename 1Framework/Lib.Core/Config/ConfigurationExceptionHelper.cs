using System;
using System.Configuration;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Config
{
    /// <summary>
    /// 和Config异常有关的异常辅助方法
    /// </summary>
    public static class ConfigurationExceptionHelper
    {
        /// <summary>
        /// 检查section是否为空，如果为空，会抛出异常
        /// </summary>
        /// <param name="section">section对象</param>
        /// <param name="sectionName">section的名称，会在异常信息中出现</param>
        public static void CheckSectionNotNull(ConfigurationSection section, string sectionName)
        {
            ExceptionHelper.FalseThrow<ConfigurationException>(section != null,
                Resource.CanNotFoundConfigSection, sectionName);
        }

        /// <summary>
        /// 检查section的source是否为空，如果为空，会抛出异常。这个检查会先执行CheckSectionNotNull
        /// </summary>
        /// <param name="section">section对象</param>
        /// <param name="sectionName">section的名称，会在异常信息中出现</param>
        public static void CheckSectionSource(ConfigurationSection section, string sectionName)
        {
            CheckSectionNotNull(section, sectionName);

            ExceptionHelper.FalseThrow<ConfigurationException>(section.ElementInformation.Source != null,
                Resource.CanNotFoundConfigSectionElement, sectionName);
        }
    }
}
