
using System;
using System.Collections.Generic;
using System.IO;
using Lib.Core.Properties;

namespace Lib.Config
{
	/// <summary>
	/// 寻找最匹配的文件算法
	/// </summary>
	internal sealed class BestFileNameMatchStrategy : BestPathMatchStrategyBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
        /// <param name="instances">MetaConfigurationSourceInstanceElementCollection对象（集合）</param>
		public BestFileNameMatchStrategy(MetaConfigurationSourceInstanceElementCollection instances)
		{
			if (instances == null) throw new NullReferenceException("instances");

			path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
			path = FormatPath(path);
			candidates = FileterPath(instances, false);
		}



		/// <summary>
		/// 从路径列表里选择与目标路径最贴近的一项。
		/// </summary>
		/// <param name="items">路径列表</param>
		/// <returns>最贴近项目</returns>
		public override string Calculate(IList<KeyValuePair<string, string>> items)
		{
			if ((items == null) || (items.Count <= 0) || string.IsNullOrEmpty(path))
				return string.Empty;

			string metaConfig = string.Empty;
			foreach (KeyValuePair<string, string> item in items)
			{
				if (string.Equals(item.Key, path))
				{
					if (string.IsNullOrEmpty(metaConfig))
						metaConfig = item.Value;
					#region confilit dispose
					else
					{
						if (false == string.Equals(metaConfig, item.Value))
						{
							string message = string.Format(Resource.ExceptionConflictPathDefinition,
								item.Key, metaConfig, item.Value);
							throw new Exception(message);
						}
					}
					#endregion
				}
			}

			return metaConfig;
		}

	}
}
