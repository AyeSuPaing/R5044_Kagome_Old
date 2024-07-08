/*
=========================================================================================================
  Module      : CPM（顧客ポートフォリオマネジメント）クラスタ設定クラス(CpmClusterSettings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// CPMクラスタ設定
	/// </summary>
	public class CpmClusterSettings
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settingFilePath">設定ファイルパス</param>
		public CpmClusterSettings(string settingFilePath)
		{
			Initialize(settingFilePath);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="settingFilePath">設定ファイルパス</param>
		private void Initialize(string settingFilePath)
		{
			var xdoc = XDocument.Load(settingFilePath);
			var clusterSettingElements = xdoc.Root.Element("ClusterSettings").Elements("Setting");
			var activeSettingElements = xdoc.Root.Element("ClusterActiveSettings").Elements("Setting");
			this.ClusterNameSeparator = xdoc.Root.Element("ClusterNameSeparator").Value;

			this.Settings1 = clusterSettingElements.Select(s =>
				new CpmClusterSetting1
				{
					Id = s.Attribute("id").Value,
					Name = s.Element("Name").Value,
					BuyCount = (string.IsNullOrEmpty(s.Element("BuyCount").Value) ? null : (int?)int.Parse(s.Element("BuyCount").Value)),
                    BuyAmount = (string.IsNullOrEmpty(s.Element("BuyAmount").Value) ? null : (decimal?)decimal.Parse(s.Element("BuyAmount").Value)),
                    EnrollmentDays = (string.IsNullOrEmpty(s.Element("EnrollmentDays").Value) ? null : (int?)int.Parse(s.Element("EnrollmentDays").Value)),
				}).ToArray();

			this.Settings2 = activeSettingElements.Select(s =>
				new CpmClusterSetting2
				{
					Id = s.Attribute("id").Value,
					Name = s.Element("Name").Value,
					AwayDays = (string.IsNullOrEmpty(s.Element("AwayDays").Value) ? null : (int?)int.Parse(s.Element("AwayDays").Value)),
				}).ToArray();

			this.ClusterIdList = new List<string>();
			this.ClusterIds = new Dictionary<string, string>();
			this.ClusterNames = new Dictionary<string, string>();
			foreach (var setting2 in this.Settings2)
			{
				foreach (var setting1 in this.Settings1)
				{
					var id = setting1.Id + setting2.Id;
					var name = CreateClusterName(setting1, setting2);
					if (this.ClusterIds.ContainsKey(name) == false) this.ClusterIds.Add(name, id);
					if (this.ClusterNames.ContainsKey(id) == false) this.ClusterNames.Add(id, name);
					ClusterIdList.Add(id);
				}
			}
		}

		/// <summary>
		/// クラスタ名作成
		/// </summary>
		/// <param name="setting1">クラスタ設定1</param>
		/// <param name="setting2">クラスタ設定2</param>
		/// <returns>クラスタ名</returns>
		public string CreateClusterName(CpmClusterSetting1 setting1, CpmClusterSetting2 setting2)
		{
			return setting1.Name + this.ClusterNameSeparator + setting2.Name;
		}

		/// <summary>クラスタIDリスト（レポートの並び順になります）</summary>
		public List<string> ClusterIdList { get; private set; }
		/// <summary>クラスタ設定ディクショナリ</summary>
		public CpmClusterSetting1[] Settings1 { get; private set; }
		/// <summary>クラスタアクティブ設定ディクショナリ</summary>
		public CpmClusterSetting2[] Settings2 { get; private set; }
		/// <summary>クラスタ名区切り文字</summary>
		public string ClusterNameSeparator { get; private set; }
		/// <summary>クラスタID（キーはクラスタ名）</summary>
		public Dictionary<string, string> ClusterIds { get; set; }
		/// <summary>クラスタ名（キーはID）</summary>
		public Dictionary<string, string> ClusterNames { get; set; }
	}

	/// <summary>
	/// CPMクラスタ設定1
	/// </summary>
	public class CpmClusterSetting1
	{
		/// <summary>ID</summary>
		public string Id { get; set; }
		/// <summary>名称</summary>
		public string Name { get; set; }
		/// <summary>購入回数</summary>
		public int? BuyCount { get; set; }
		/// <summary>購入金額</summary>
		public decimal? BuyAmount { get; set; }
		/// <summary>在籍期間</summary>
		public int? EnrollmentDays { get; set; }
	}

	/// <summary>
	/// CPMクラスタ設定2
	/// </summary>
	public class CpmClusterSetting2
	{
		/// <summary>ID</summary>
		public string Id { get; set; }
		/// <summary>名称</summary>
		public string Name { get; set; }
		/// <summary>離脱期間</summary>
		public int? AwayDays { get; set; }
	}
}
