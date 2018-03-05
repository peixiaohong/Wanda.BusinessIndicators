using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Lib.Cache;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Config
{
    /// <summary>
    /// <remarks>
    /// Broker类管理所有本地配置文件和映射配置文件。
    /// 远程配置文件映射由ConfigurationFileMap和ConfigurationManager.OpenMappedMachineConfiguration处理.
    /// 
    /// 约束:
    ///     <list type="bullet">
    ///         <item>
    ///         映射文件必须以ConfigurationSectionGroup或ConfigurationSection开始.
    ///         </item>
    ///         <item>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// </summary>
    public static class ConfigurationBroker
    {
        #region private const and field
        /// <summary>
        /// Private const
        /// </summary>
        private const string LocalItem = "local";
        private const string MetaConfigurationItem = "MCS.MetaConfiguration";
        private const string MetaConfigurationSectionGroupItem = "mcs.library.metaConfig";


        /// <summary>
        /// Private static field
        /// </summary>
        private static readonly string MachineConfigurationFile = ConfigurationManager.OpenMachineConfiguration().FilePath;
        private static readonly string LocalConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        private static readonly string GlobalConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
        // 缓存失效时间
        private static readonly TimeSpan SlidingTime = TimeSpan.FromSeconds(15.0); //TimeSpan.FromMinutes(5.0);

        #endregion private const and field

        /// <summary>
        /// 构造函数
        /// </summary>
        static ConfigurationBroker()
        {

        }

        /// <summary>
        /// meta配置文件位置枚举
        /// </summary>
        private enum MetaConfigurationPosition
        {
            LocalFile,
            MetaFile
        }

        /// <summary>
        /// 内部类，用于存放、传递machine、local、meta、global配置文件的地址和
        /// meta文件位置（枚举） 
        /// </summary>
        private class ConfigFilesSetting
        {
            private string machineConfigurationFile = string.Empty;
            private string localConfigurationFile = string.Empty;
            private string metaConfigurationFile = string.Empty;
            private string globalConfigurationFile = string.Empty;
            private MetaConfigurationPosition metaFilePosition = MetaConfigurationPosition.LocalFile;

            public MetaConfigurationPosition MetaFilePosition
            {
                get { return this.metaFilePosition; }
                set { this.metaFilePosition = value; }
            }

            public string GlobalConfigurationFile
            {
                get { return this.globalConfigurationFile; }
                set { this.globalConfigurationFile = value; }
            }

            public string MetaConfigurationFile
            {
                get { return this.metaConfigurationFile; }
                set { this.metaConfigurationFile = value; }
            }

            public string LocalConfigurationFile
            {
                get { return this.localConfigurationFile; }
                set { this.localConfigurationFile = value; }
            }

            public string MachineConfigurationFile
            {
                get { return this.machineConfigurationFile; }
                set { this.machineConfigurationFile = value; }
            }
        }



        #region private static method

        /// <summary>
        /// 生成configuration对象的缓存key值
        /// </summary>
        /// <param name="fileNames">文件列表</param>
        /// <returns>cache key</returns>
        private static string CreateConfigurationCacheKey(params string[] fileNames)
        {
            StringBuilder key = new StringBuilder(256);

            for (int i = 0; i < fileNames.Length; i++)
            {
                // 只取文件名，去掉完整路径
                key.Append(Path.GetFileName(fileNames[i]).ToLower());
            }

            return key.ToString();
        }

        /// <summary>
        /// 加载machine、local配置文件，meta配置文件，meta中的配置节，将其缓存并建立缓存失效依赖。
        /// 查找并在 ConfigFilesSetting 类实例中记录machine、local、meta和global配置文件的地址和
        /// meta配置文件位置（枚举）
        /// </summary>
        /// <returns>ConfigFilesSetting 类实例</returns>
        private static ConfigFilesSetting LoadFilesSetting()
        {
            ConfigFilesSetting settings = new ConfigFilesSetting();
            settings.MachineConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
            settings.LocalConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
            settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;

            MetaConfigurationSourceInstanceSection metaSection = ConfigurationBroker.GetMetaSourceInstanceSection(settings);

            if (metaSection != null)
            {
                string currPath;

                if (EnvironmentHelper.Mode == InstanceMode.Web)
                    currPath = HttpContext.Current.Request.Url.AbsoluteUri;
                else
                    currPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

                settings.GlobalConfigurationFile = ConfigurationBroker.ReplaceEnvironmentVariablesInFilePath(metaSection.Instances.GetMatchedPath(currPath));

                if (string.IsNullOrEmpty(settings.GlobalConfigurationFile))
                    settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;
                else
                {
                    if (false == Path.IsPathRooted(settings.GlobalConfigurationFile))
                        settings.GlobalConfigurationFile =
                            AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + settings.GlobalConfigurationFile;

                    ExceptionHelper.FalseThrow(File.Exists(settings.GlobalConfigurationFile), Resource.GlobalFileNotFound, settings.GlobalConfigurationFile);
                }
            }

            return settings;
        }

        /// <summary>
        /// 获取meta配置中的 sourceMappings 节点
        /// </summary>
        /// <param name="fileSettings">ConfigFilesSetting 类实例</param>
        /// <returns>meta配置中的 sourceMappings 节点</returns>
        private static MetaConfigurationSourceInstanceSection GetMetaSourceInstanceSection(ConfigFilesSetting fileSettings)
        {
            ConfigurationSection section;

            string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileSettings.MachineConfigurationFile,
                MetaConfigurationSourceInstanceSection.Name);

            if (ConfigurationSectionCache.Instance.TryGetValue(cacheKey, out section) == false)
            {
                ConfigurationBroker.GetMetaFileSettings(fileSettings);

                if (fileSettings.MetaFilePosition == MetaConfigurationPosition.LocalFile)
                    section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromLocal(fileSettings);
                else
                    section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromMetaFile(fileSettings);

                FileCacheDependency fileDependency = new FileCacheDependency(
                    true,
                    fileSettings.MachineConfigurationFile,
                    fileSettings.LocalConfigurationFile,
                    fileSettings.MetaConfigurationFile);

                SlidingTimeDependency timeDependency = new SlidingTimeDependency(ConfigurationBroker.SlidingTime);

                ConfigurationSectionCache.Instance.Add(cacheKey, section, new MixedDependency(fileDependency, timeDependency));
            }

            return (MetaConfigurationSourceInstanceSection)section;

        }

        /// <summary>
        /// 从本地config文件中读取meta配置
        /// </summary>
        /// <param name="fileSettings">ConfigFilesSetting 类实例</param>
        /// <returns>MetaConfigurationSourceInstanceSection 实体</returns>
        private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromLocal(ConfigFilesSetting fileSettings)
        {
            System.Configuration.Configuration config;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                config = ConfigurationBroker.GetStandardWebConfiguration(fileSettings.MachineConfigurationFile, true);
            else
                config = ConfigurationBroker.GetStandardExeConfiguration(fileSettings.MachineConfigurationFile, fileSettings.LocalConfigurationFile, true);

            MetaConfigurationSectionGroup group =
                (MetaConfigurationSectionGroup)config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem);
            MetaConfigurationSourceInstanceSection section = null;

            if (group != null)
                section = group.SourceConfigurationMapping;

            return section;
        }

        /// <summary>
        /// 从单独的meta.config文件中读取meta配置
        /// </summary>
        /// <param name="fileSettings">ConfigFilesSetting 实体</param>
        /// <returns>MetaConfigurationSourceInstanceSection 实体</returns>
        private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromMetaFile(ConfigFilesSetting fileSettings)
        {
            System.Configuration.Configuration config = ConfigurationBroker.GetSingleFileConfiguration(
                    fileSettings.MetaConfigurationFile,
                    true,
                    fileSettings.MachineConfigurationFile,
                    fileSettings.LocalConfigurationFile);

            MetaConfigurationSectionGroup group =
                config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem) as MetaConfigurationSectionGroup;

            MetaConfigurationSourceInstanceSection section = null;

            if (group != null)
                section = group.SourceConfigurationMapping;

            return section;
        }

        /// <summary>
        /// 获取meta文件的地址和位置
        /// </summary>
        /// <param name="fileSettings">ConfigFilesSetting 类实例</param>
        private static void GetMetaFileSettings(ConfigFilesSetting fileSettings)
        {
            AppSettingsSection section = ConfigurationBroker.GetLocalAppSettingsSection();

            if (section != null)
            {
                if (section.Settings[ConfigurationBroker.MetaConfigurationItem] == null)
                    fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
                else
                    fileSettings.MetaConfigurationFile =
                        ConfigurationBroker.ReplaceEnvironmentVariablesInFilePath(section.Settings[ConfigurationBroker.MetaConfigurationItem].Value);
            }

            if (string.IsNullOrEmpty(fileSettings.MetaConfigurationFile) == true)
            {
                fileSettings.MetaFilePosition = MetaConfigurationPosition.LocalFile;
                fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
            }
            else
            {
                fileSettings.MetaFilePosition = MetaConfigurationPosition.MetaFile;

                if (false == Path.IsPathRooted(fileSettings.MetaConfigurationFile))
                    fileSettings.MetaConfigurationFile =
                        AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + fileSettings.MetaConfigurationFile;

                ExceptionHelper.FalseThrow(File.Exists(fileSettings.MetaConfigurationFile), Resource.MetaFileNotFound, fileSettings.MetaConfigurationFile);
            }
        }

        /// <summary>
        /// 获取最终 local 和 global 合并后的 Configuration
        /// </summary>
        /// <param name="fileSettings">ConfigFilesSetting 类实例</param>
        /// <returns>local 和 global 合并后的 Configuration</returns>
        private static System.Configuration.Configuration GetFinalConfiguration(ConfigFilesSetting fileSettings)
        {
            System.Configuration.Configuration config;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                config = ConfigurationBroker.GetStandardWebConfiguration(
                            fileSettings.GlobalConfigurationFile,
                            true,
                            fileSettings.LocalConfigurationFile,
                            fileSettings.MachineConfigurationFile,
                            fileSettings.MetaConfigurationFile);
            else
                config = ConfigurationBroker.GetStandardExeConfiguration(fileSettings.GlobalConfigurationFile,
                            fileSettings.LocalConfigurationFile,
                            true,
                            fileSettings.MachineConfigurationFile,
                            fileSettings.MetaConfigurationFile);

            return config;
        }

        /// <summary>
        /// 获取本地config的AppSettings节点
        /// </summary>
        /// <returns>AppSettingsSection</returns>
        private static AppSettingsSection GetLocalAppSettingsSection()
        {
            System.Configuration.Configuration config = null;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                config = ConfigurationBroker.GetStandardWebConfiguration(ConfigurationBroker.MachineConfigurationFile, true);
            else
                config = ConfigurationBroker.GetStandardExeConfiguration(ConfigurationBroker.MachineConfigurationFile, ConfigurationBroker.LocalConfigurationFile, true);

            return config.AppSettings;
        }

        /// <summary>
        /// 取得单独config文件中的 Configuration
        /// </summary>
        /// <param name="fileName">文件地址</param>
        /// <param name="fileDependencies">缓存依赖文件</param>
        /// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
        /// <returns>Configuration对象</returns>
        private static System.Configuration.Configuration GetSingleFileConfiguration(string fileName, bool ignoreFileNotExist, params string[] fileDependencies)
        {
            string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileName);

            System.Configuration.Configuration config;

            if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
            {
                config = ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(fileName));

                Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
                fileDependencies[fileDependencies.Length - 1] = fileName;

                ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
            }

            return config;
        }

        /// <summary>
        /// 获取标准Web应用程序的配置信息，合并Web.config和global配置文件
        /// </summary>
        /// <param name="machineConfigPath">global配置文件地址</param>
        /// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
        /// <param name="fileDependencies">缓存依赖文件</param>
        /// <returns>Web.config和global配置文件合并后的Configuration对象</returns>
        private static System.Configuration.Configuration GetStandardWebConfiguration(string machineConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
        {
            string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath);

            System.Configuration.Configuration config;

            if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
            {
                WebConfigurationFileMap fileMap = new WebConfigurationFileMap();

                fileMap.MachineConfigFilename = machineConfigPath;
                VirtualDirectoryMapping vDirMap = new VirtualDirectoryMapping(
                        HttpContext.Current.Request.PhysicalApplicationPath,
                        true);

                fileMap.VirtualDirectories.Add("/", vDirMap);

                config = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/",
                    HttpContext.Current.Request.ServerVariables["INSTANCE_ID"]);

                Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
                fileDependencies[fileDependencies.Length - 1] = machineConfigPath;

                ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromFile;
#endif
            }
            else
            {
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromCache;
#endif
            }

            return config;

        }

        /// <summary>
        /// 获取标准WinForm应用程序的配置信息，合并App.config和global配置文件
        /// </summary>
        /// <param name="machineConfigPath">global配置文件地址</param>
        /// <param name="localConfigPath">本地应用程序配置文件地址</param>
        /// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
        /// <param name="fileDependencies">缓存依赖文件</param>
        /// <returns>App.config和global配置文件合并后的Configuration对象</returns>
        private static System.Configuration.Configuration GetStandardExeConfiguration(string machineConfigPath, string localConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
        {
            string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath, localConfigPath);

            System.Configuration.Configuration config;

            if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.MachineConfigFilename = machineConfigPath;
                fileMap.ExeConfigFilename = localConfigPath;

                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 2);
                fileDependencies[fileDependencies.Length - 2] = machineConfigPath;
                fileDependencies[fileDependencies.Length - 1] = localConfigPath;

                ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromFile;
#endif
            }
            else
            {
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromCache;
#endif
            }

            return config;

        }

        /// <summary>
        /// 把Configuration对象放入缓存，建立时间和文件的混合依赖
        /// </summary>
        /// <param name="cacheKey">cache key</param>
        /// <param name="config">待缓存的Configuration对象</param>
        /// <param name="ignoreFileNotExist">是否忽略不存在的文件</param>
        /// <param name="files">缓存依赖的文件</param>
        private static void AddConfigurationToCache(string cacheKey, System.Configuration.Configuration config, bool ignoreFileNotExist, params string[] files)
        {
            MixedDependency dependency = new MixedDependency(
                new FileCacheDependency(ignoreFileNotExist, files),
                new SlidingTimeDependency(ConfigurationBroker.SlidingTime));

            ConfigurationCache.Instance.Add(cacheKey, config, dependency);
        }

        /// <summary>
        /// 从SectionGroup中读取Section。在Section写在Group里时使用
        /// </summary>
        /// <param name="sectionName">section name</param>
        /// <param name="groups">SectionGroup</param>
        /// <returns>ConfigurationSection</returns>
        private static ConfigurationSection GetSectionFromGroups(string sectionName, ConfigurationSectionGroupCollection groups)
        {
            ConfigurationSection section = null;

            for (int i = 0; i < groups.Count; i++)
            {
                try
                {
                    ConfigurationSectionGroup group = groups[i];

                    if (group.SectionGroups.Count > 0)
                        section = ConfigurationBroker.GetSectionFromGroups(sectionName, group.SectionGroups);
                    else
                        section = group.Sections[sectionName];

                    if (section != null)
                        break;
                }
                catch (System.IO.FileNotFoundException)
                {
                }

            }

            return section;
        }

        private static string ReplaceEnvironmentVariablesInFilePath(string filePath)
        {
            string result = filePath;

            Regex r = new Regex(@"%\S+?%");

            foreach (Match m in r.Matches(filePath))
            {
                string variableName = m.Value.Trim('%');
                string variableValue = Environment.GetEnvironmentVariable(variableName);

                if (variableValue != null)
                    result = result.Replace(m.Value, variableValue);
            }

            return result;
        }

        #endregion private static method

        #region Public static method

        /// <summary>
        /// 按节点名称从配置信息中取得节点，并将节点信息缓存，建立文件依赖
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <returns>配置节点</returns>
        /// <remarks>
        /// 按名称获取配置节点信息。返回ConfigurationSection的派生类实体，实体类需由用户自定义。
        /// <code source="..\Framework\TestProjects\Deluxeworks.Library.WebTest\Configuration\Default.aspx.cs" region="Using Broker" lang="cs" title="使用配置管理" />
        /// </remarks>
        public static ConfigurationSection GetSection(string sectionName)
        {

            ConfigurationSection section;

            if (false == ConfigurationSectionCache.Instance.TryGetValue(sectionName, out section))
            {
                ConfigFilesSetting settings = LoadFilesSetting();

                System.Configuration.Configuration config = GetFinalConfiguration(settings);
                section = config.GetSection(sectionName);

                // 在Configuration对象中不能直接拿到Section对象时，遍历所有Group查找Section
                if (section == null || section is DefaultSection)
                    section = GetSectionFromGroups(sectionName, config.SectionGroups);

                FileCacheDependency dependency = new FileCacheDependency(
                                                    true,
                                                    settings.MachineConfigurationFile,
                                                    settings.LocalConfigurationFile,
                                                    settings.MetaConfigurationFile,
                                                    settings.GlobalConfigurationFile);

                ConfigurationSectionCache.Instance.Add(sectionName, section, dependency);
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.sectionReadFrom = ReadFrom.ReadFromFile;
#endif
            }
            else
            {
#if DELUXEWORKSTEST
				// 测试使用
				ConfigurationBroker.sectionReadFrom = ReadFrom.ReadFromCache;
#endif
            }

            return section;

        }

        #endregion

#if DELUXEWORKSTEST
        #region static property and method for test

        public enum ReadFrom
        {
            ReadFromFile, ReadFromCache
        }

        private static ReadFrom configurationReadFrom;
        private static ReadFrom sectionReadFrom;

        public static ReadFrom SectionReadFrom
        {
            get { return ConfigurationBroker.sectionReadFrom; }
        }

        public static ReadFrom ConfigurationReadFrom
        {
            get { return ConfigurationBroker.configurationReadFrom; }
        }

        public static ConfigurationSection GetSectionForTimeDependencyTest(string sectionName)
        {
            ConfigurationSection section;

            ConfigFilesSetting settings = LoadFilesSetting();

            System.Configuration.Configuration config = GetFinalConfiguration(settings);
            section = config.GetSection(sectionName);

            if (section == null)
                section = GetSectionFromGroups(sectionName, config.SectionGroups);

            return section;
        }
        #endregion static property and method for test
#endif
    } // class end
}
