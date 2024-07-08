/*
=========================================================================================================
  Module      : モール系マッピング設定管理(MallMappingManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Xml;
using System.Xml.XPath;
using w2.Common.Util;

namespace w2.App.Common.Mall.Mapping
{
	/// <summary>
	/// モール系マッピング設定管理
	/// </summary>
	public abstract class MallMappingManager
	{
		/// <summary>定数: 設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = "Settings";
		/// <summary>定数: 設定Baseディレクトリ名</summary>
		private const string SETTING_FILE_BASE_DIR = @"Settings\base";
		/// <summary>定数: デフォルト設定ノード名</summary>
		private const string DEFAULT_NODE = "Default";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="defaultSettingFileName">デフォルト設定ファイル名 拡張子含む</param>
		/// <param name="projectSettingFileName">プロジェクト設定ファイル名 拡張子含む</param>
		/// <param name="rootNodeName">ルートノード名</param>
		protected MallMappingManager(string defaultSettingFileName, string projectSettingFileName, string rootNodeName)
		{
			this.SettingFileFullPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR, defaultSettingFileName);
			this.SettingProjectFileFullPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR, projectSettingFileName);
			this.RootNodeName = rootNodeName;

			Update();
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR), defaultSettingFileName, Update);
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR), projectSettingFileName, Update);
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			this.DefaultNode = this.DefaultNode ?? GetMappingXmlNode(this.SettingFileFullPath);
			this.ProjectNode = this.ProjectNode ?? GetMappingXmlNode(this.SettingProjectFileFullPath);
		}

		/// <summary>
		/// モールIDに紐づく設定を取得 設定が存在しない場合はデフォルトを返す
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="targetColumnName">取得対象カラム名</param>
		/// <returns>設定</returns>
		protected XmlNodeList GetNodeList(string mallId, string targetColumnName)
		{
			if (this.DefaultNode == null || this.ProjectNode == null) return null;

			var defaultNode = this.DefaultNode.Clone();
			var projectNode = this.ProjectNode.Clone();

			bool useProjectFileRootNode;
			try
			{
				useProjectFileRootNode = (projectNode.SelectNodes(mallId).Count > 0);
			}
			catch (XPathException)
			{
				useProjectFileRootNode = false;
			}

			var rootNode = (useProjectFileRootNode) ? projectNode : defaultNode;

			// 各項目マッピング設定を取得する モールIDごとの設定が存在する場合はモールIDごとの設定を優先
			var nodeList = (useProjectFileRootNode)
				? rootNode.SelectSingleNode(mallId).SelectSingleNode(targetColumnName).ChildNodes
				: rootNode.SelectSingleNode(DEFAULT_NODE).SelectSingleNode(targetColumnName).ChildNodes;
			return nodeList;
		}

		/// <summary>
		/// 設定ファイルの内容取得
		/// </summary>
		/// <param name="filePath">マッピングファイルパス</param>
		/// <returns>設定ファイルの内容取得</returns>
		private XmlNode GetMappingXmlNode(string filePath)
		{
			if (File.Exists(filePath) == false) return null;

			// ルートノード読み込み
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			var rootNode = xmlDocument.SelectSingleNode(this.RootNodeName);
			return rootNode;
		}

		/// <summary>デフォルト設定ファイル フルパス</summary>
		private string SettingFileFullPath { get; set; }
		/// <summary>プロジェクト設定ファイルのパス</summary>
		private string SettingProjectFileFullPath { get; set; }
		/// <summary>ルートノード名</summary>
		private string RootNodeName { get; set; }
		/// <summary>デフォルト設定</summary>
		private XmlNode DefaultNode { get; set; }
		/// <summary>プロジェクト設定</summary>
		private XmlNode ProjectNode { get; set; }
	}
}
