/*
=========================================================================================================
  Module      : コンテンツマネージャー設定管理(ContentsManagerSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using w2.Common.Util;

namespace w2.Cms.Manager.Codes.Settings
{
	/// <summary>
	/// コンテンツマネージャー設定管理
	/// </summary>
	public class ContentsManagerSetting
	{
		/// <summary>設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = @"Xml\Setting\";
		/// <summary>設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "ContentsManagerSetting.xml";
		/// <summary>インスタンス</summary>
		private static ContentsManagerSetting m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ContentsManagerSetting()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, SETTING_FILE_DIR),
				SETTING_FILE_NAME,
				Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static ContentsManagerSetting GetInstance()
		{
			var result = m_singletonInstance ?? (m_singletonInstance = new ContentsManagerSetting());
			return result;
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			var refuseDirList = new List<string>();
			var refuseFileList = new List<string>();

			// 設定XML読み込み
			var xd = new XmlDocument();
			xd.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTING_FILE_DIR, SETTING_FILE_NAME));

			var xnl = xd.SelectSingleNode("ContentsManagerSetting/Refuse").ChildNodes;
			// 拒否リスト（ディレクトリ・ファイル）
			var tempRefuseList = new List<string>();
			foreach (XmlNode xn in xnl)
			{
				// コメントは無視する
				if (xn.NodeType == XmlNodeType.Comment) continue;

				switch (xn.Name)
				{
					case "Dir":
						refuseDirList.Add(xn.InnerText.ToLower());
						break;
					case "File":
						refuseFileList.Add(xn.InnerText.ToLower());
						break;
				}

				tempRefuseList.Add(xn.InnerText.ToLower());
			}

			this.RefuseList = tempRefuseList.ToArray();
			this.RefuseListIncludeContentsRoot = tempRefuseList
				.Select(x => Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, x)).ToArray();

			if (Constants.REPEATPLUSONE_OPTION_ENABLED)
			{
				var contentsManager = Constants.REPEATPLUSONE_CONFIGS.RepeatPlusOneSettings.ContentsManager;
				foreach (var setting in contentsManager)
				{
					if (string.IsNullOrEmpty(setting.Dir) == false) refuseDirList.Add(setting.Dir.ToLower());
					if (string.IsNullOrEmpty(setting.FileName) == false) refuseFileList.Add(setting.FileName.ToLower());
				}
			}

			this.RefuseDirList = refuseDirList.ToArray();
			this.RefuseFileList = refuseFileList.ToArray();

			var notDelete = xd.SelectSingleNode("ContentsManagerSetting/NotDelete").ChildNodes;
			var notDeleteList = new List<string>();

			foreach (XmlNode nd in notDelete)
			{
				if (nd.NodeType == XmlNodeType.Comment) continue;
				switch (nd.Name)
				{
					case "Dir":
						notDeleteList.Add(nd.InnerText.ToLower());
						break;
				}
			}

			this.NotDeleteList = notDeleteList.ToArray();
		}

		/// <summary>拒否リスト</summary>
		public string[] RefuseList { get; private set; }
		/// <summary>拒否リスト(ルート含む)</summary>
		public string[] RefuseListIncludeContentsRoot { get; private set; }
		/// <summary>拒否ディレクトリリスト</summary>
		public string[] RefuseDirList { get; private set; }
		/// <summary>拒否ファイルリスト</summary>
		public string[] RefuseFileList { get; private set; }
		/// <summary>削除不可リスト</summary>
		public string[] NotDeleteList { get; private set; }
	}
}