using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Cache
{
    /// <summary>
    /// 文件依赖，客户端代码在初始化此类的对象时，需要提供一个文件名数组，
    /// 当此数组中的任何一个文件发生变化时，认为与此依赖项相关的Cache项过期。
    /// </summary>
    public sealed class FileCacheDependency : DependencyBase
    {
        /// <summary>
        /// 为了实现大小写问题，
        /// </summary>
        public class FileDependencyReadOnlyCollection : ReadOnlyCollection<string>
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="ls">初始化用的List</param>
            public FileDependencyReadOnlyCollection(List<string> ls)
                : base(ls)
            { }

            /// <summary>
            /// 判断集合中是否含有某个字符串，但在判断之前将字符串转化为大写
            /// </summary>
            /// <param name="key">需要搜索的字符串</param>
            /// <returns>是否存在</returns>
            public new bool Contains(string key)
            {
                key = key.ToUpper();
                return base.Contains(key);
            }

            /// <summary>
            /// 返回指定键的索引
            /// </summary>
            /// <param name="key">键值</param>
            /// <returns>键值索引</returns>
            public new int IndexOf(string key)
            {
                key = key.ToUpper();
                return base.IndexOf(key);
            }
        }

        #region 私有类/结构定义
        //定义了目录和FileSystemWatcher的映射
        private class DirToFileSystemWatcherItem
        {
            private int referenceCount = 0;
            private string directory = string.Empty;
            private FileSystemWatcher watcher = null;
            /// <summary>
            /// 查看系统文件组
            /// </summary>
            /// <param name="directory">目录</param>
            /// <param name="fse">文件系统事件句柄</param>
            /// <param name="re">Reamed事件句柄</param>
            public DirToFileSystemWatcherItem(string directory, FileSystemEventHandler fse, RenamedEventHandler re)
            {
                this.directory = directory;

                this.watcher = new FileSystemWatcher(directory);
                this.watcher.Changed += fse;
                this.watcher.Deleted += fse;
                this.watcher.Renamed += re;
                this.watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;

                this.watcher.EnableRaisingEvents = true;
            }

            public FileSystemWatcher Watcher
            {
                get { return this.watcher; }
            }

            public string Directory
            {
                get { return this.directory; }
            }

            public int ReferenceCount
            {
                get { return this.referenceCount; }
                set { this.referenceCount = value; }
            }
        }

        /// <summary>
        /// 文件名称的FileDependency的映射
        /// </summary>
        private class FileNameToFileCacheDependenciesItem
        {
            private string fileName = string.Empty;
            private Dictionary<FileCacheDependency, FileCacheDependency> dependencies = new Dictionary<FileCacheDependency, FileCacheDependency>();

            /// <summary>
            /// 获取文件名称
            /// </summary>
            /// <param name="dependenceFileName">缓存依赖文件的名称</param>
            public FileNameToFileCacheDependenciesItem(string dependenceFileName)
            {
                this.fileName = dependenceFileName;
            }

            public string FileName
            {
                get { return this.fileName; }
            }

            public Dictionary<FileCacheDependency, FileCacheDependency> Dependencies
            {
                get { return this.dependencies; }
            }
        }
        #endregion 私有类定义

        #region 私有静态成员
        private static Dictionary<string, DirToFileSystemWatcherItem>
            dirToFileSystemWatcherDict = new Dictionary<string, DirToFileSystemWatcherItem>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, FileNameToFileCacheDependenciesItem>
            fileNameToFileCacheDependenciesDict = new Dictionary<string, FileNameToFileCacheDependenciesItem>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 同步对象
        /// </summary>
        private static object syncObject = new object();

        #endregion 私有静态成员

        #region 私有成员

        private string[] dirs;
        private FileDependencyReadOnlyCollection fileNameCollection;
        private bool changed = false;
        private bool ignoreFileNotExist = false;

#if DELUXEWORKSTEST
		/// <summary>
		/// 取内部集合的个数，用于测试
		/// </summary>
		private int DirToFileSystemWatcherDictCount
		{
			get
			{
				return FileCacheDependency.dirToFileSystemWatcherDict.Count;
			}
		}

		/// <summary>
		/// 取内部集合的个数，用于测试
		/// </summary>
		private int FileNameToFileCacheDependenciesDictCount
		{
			get
			{
				return FileCacheDependency.fileNameToFileCacheDependenciesDict.Count;
			}
		}
#endif
        #endregion 私有属性

        #region 公有方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="fileNames">Cache项依赖的文件名称</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileCacheDependencyConstructorTest" lang="cs" title="指定依赖的文件名列表" />
        /// </remarks>
        public FileCacheDependency(params string[] fileNames)
        {
            Init(fileNames);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ifne">是否忽略不存在的文件，如置为false，则文件不存在会抛出异常</param>
        /// <param name="fileNames">Cache项依赖的文件名称</param>
        public FileCacheDependency(bool ifne, params string[] fileNames)
        {
            this.ignoreFileNotExist = ifne;

            Init(fileNames);
        }

        /// <summary>
        /// 文件名集合
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileCacheDependencyFileNamesTest" lang="cs" title="文件名列表，只读" />
        /// </remarks>
        public FileDependencyReadOnlyCollection FileNames
        {
            get
            {
                return this.fileNameCollection;
            }
        }
        /// <summary>
        /// 是否发生改变
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\FileCacheDependencyTest.cs" region="FileDependencyChangeContentTest" lang="cs" title="获取本Dependency是否过期" />
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return this.changed;
            }
        }
        #endregion 公有方法

        #region 保护的方法
        /// <summary>
        /// 
        /// </summary>
        internal protected override void SetChanged()
        {
            this.changed = true;
        }

        /// <summary>
        /// 释放资源，包括内部的FileSystemWatcher
        /// </summary>
        public override void Dispose()
        {
            lock (FileCacheDependency.syncObject)
            {
                DirToFileSystemWatcherItem item;

                foreach (string dir in this.dirs)
                {
                    if (FileCacheDependency.dirToFileSystemWatcherDict.TryGetValue(dir, out item))
                    {
                        item.ReferenceCount--;

                        if (item.ReferenceCount == 0)
                        {
                            item.Watcher.Dispose();
                            FileCacheDependency.dirToFileSystemWatcherDict.Remove(dir);
                        }
                    }
                }

                foreach (string file in this.fileNameCollection)
                {
                    FileNameToFileCacheDependenciesItem f2dItem;

                    if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(file, out f2dItem))
                    {
                        f2dItem.Dependencies.Remove(this);

                        if (f2dItem.Dependencies.Count == 0)
                            FileCacheDependency.fileNameToFileCacheDependenciesDict.Remove(file);
                    }
                }
            }
        }

        /// <summary>
        /// 当此Dependency绑定到CacheItem时
        /// </summary>
        protected internal override void CacheItemBinded()
        {
            InitFileWatcher();
        }

        #endregion 保护的方法

        #region 私有方法
        private void Init(string[] fileNames)
        {
            this.UtcLastModified = DateTime.UtcNow;
            this.UtcLastAccessTime = DateTime.UtcNow;

            this.InitFileNames(fileNames);
            this.InitDirs(this.fileNameCollection);
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            DoFileChange(e.FullPath);
        }

        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            DoFileChange(e.OldFullPath);
        }

        private static void DoFileChange(string fullPath)
        {
            try
            {
                lock (FileCacheDependency.syncObject)
                {
                    FileNameToFileCacheDependenciesItem f2dItem;
                    List<CacheItemBase> list = new List<CacheItemBase>();

                    if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(fullPath, out f2dItem))
                    {
                        foreach (KeyValuePair<FileCacheDependency, FileCacheDependency> kp in f2dItem.Dependencies)
                        {
                            kp.Value.changed = true;

                            if (kp.Value.CacheItem != null)
                                list.Add(kp.Value.CacheItem);
                        }

                        /*
                        for (int index = 0; index < f2dItem.Dependencies.Count; index++)
                        {
                            f2dItem.Dependencies[index].changed = true;

                            if (f2dItem.Dependencies[index].CacheItem != null)
                                list.Add(f2dItem.Dependencies[index].CacheItem);
                        }*/
                    }

                    /*不需要主动清除item，等待内存回收
                    foreach (CacheItemBase item in list)
                        item.RemoveCacheItem();
                     */

                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
        }

        private void InitFileNames(string[] fileNames)
        {
            List<string> list = new List<string>(fileNames.Length);

            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i].ToUpper();

                if (string.IsNullOrEmpty(fileName) == false)
                {
                    if (Path.IsPathRooted(fileName) == false)
                        fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + fileName;

                    if (File.Exists(fileName) == false)
                        ExceptionHelper.FalseThrow<FileNotFoundException>(this.ignoreFileNotExist, Resource.FileNotFound, fileName);
                    else
                        list.Add(fileName);
                }
            }

            this.fileNameCollection = new FileDependencyReadOnlyCollection(list);
        }

        private void InitDirs(ReadOnlyCollection<string> fileNames)
        {
            Dictionary<string, string> dirDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string fileName in fileNames)
            {
                if (string.IsNullOrEmpty(fileName) == false)
                {
                    string dir = Path.GetDirectoryName(fileName);

                    ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(dir == null, "不能使用根目录作为Cache项的依赖");

                    if (dirDict.ContainsKey(dir) == false)
                        dirDict.Add(dir, dir);
                }
            }

            string[] dires = new string[dirDict.Count];

            int j = 0;

            foreach (KeyValuePair<string, string> kp in dirDict)
                dires[j++] = kp.Value;

            this.dirs = dires;
        }

        private void InitFileWatcher()
        {
            lock (FileCacheDependency.syncObject)
            {
                foreach (string dir in this.dirs)
                {
                    DirToFileSystemWatcherItem item;

                    if (FileCacheDependency.dirToFileSystemWatcherDict.TryGetValue(dir, out item) == false)
                    {
                        item = new DirToFileSystemWatcherItem(dir,
                            new FileSystemEventHandler(FileSystemWatcher_Changed),
                            new RenamedEventHandler(FileSystemWatcher_Renamed));
                        FileCacheDependency.dirToFileSystemWatcherDict.Add(dir, item);
                    }

                    item.ReferenceCount++;
                }

                foreach (string fileName in this.fileNameCollection)
                {
                    FileNameToFileCacheDependenciesItem item;

                    if (FileCacheDependency.fileNameToFileCacheDependenciesDict.TryGetValue(fileName, out item) == false)
                    {
                        item = new FileNameToFileCacheDependenciesItem(fileName);
                        FileCacheDependency.fileNameToFileCacheDependenciesDict.Add(fileName, item);
                    }

                    item.Dependencies[this] = this;
                }
            }
        }
        #endregion 私有方法
    }
}
