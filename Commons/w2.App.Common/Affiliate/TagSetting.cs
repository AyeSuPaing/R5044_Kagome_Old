/*
=========================================================================================================
  Module      : タグ固有設定管理クラス(TagSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ固有設定管理クラス
	/// </summary>
	public class TagSetting
	{
		/// <summary>定数 : アクションタイプ : 注文情報利用ページ</summary>
		public const string ACTION_TYPE_ORDER = "ORDER";
		/// <summary>定数 : アクションタイプ : セッションのみ利用ページ</summary>
		public const string ACTION_TYPE_SESSION_ONLY = "SESSION_ONLY";
		/// <summary>定数 : アクションタイプ : カート内商品</summary>
		public const string ACTION_TYPE_CART_PRODUCT = "CART_PRODUCT";
		/// <summary>定数 : アクションタイプ : 複数ページ選択時のアクションタイプが異なる場合</summary>
		public const string ACTION_TYPE_MIX = "MIX";
		/// <summary>定数 : アクションタイプ : 全ページ対象</summary>
		public const string ACTION_TYPE_ALL = "ALL";
		/// <summary>定数 : アクションタイプ : ランディングカートページ対象</summary>
		public const string ACTION_TYPE_LANDING_CART = "LANDING_CART";

		/// <summary>定数 : アクションタイプ : SEO全体設定対象</summary>
		public const string ACTION_TYPE_SEO_ALL = "SEO_ALL";
		/// <summary>定数 : アクションタイプ : SEO商品一覧設定対象</summary>
		public const string ACTION_TYPE_SEO_PRODUCT_LIST = "SEO_PRODUCT_LIST";
		/// <summary>定数 : アクションタイプ : SEO商品詳細設定対象</summary>
		public const string ACTION_TYPE_SEO_PRODUCT_DETAIL = "SEO_PRODUCT_DETAIL";
		/// <summary>定数 : アクションタイプ : コーディネート一覧設定対象</summary>
		public const string ACTION_TYPE_COORDINATE_LIST = "COORDINATE_LIST";
		/// <summary>定数 : アクションタイプ : コーディネート詳細設定対象</summary>
		public const string ACTION_TYPE_COORDINATE_DETAIL = "COORDINATE_DETAIL";

		/// <summary>定数: 設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = "Settings";
		/// <summary>定数: プロジェクト用設定ファイル名</summary>
		private const string PROJECT_SETTING_FILE_NAME = "ProjectTagSetting.xml";


		/// <summary>定数: 設定Baseディレクトリ名</summary>
		private const string SETTING_FILE_BASE_DIR = @"Settings\base";
		/// <summary>定数: 設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "BaseTagSetting.xml";

		/// <summary>インスタンス</summary>
		private static TagSetting m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TagSetting()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR),
				SETTING_FILE_NAME,
				Update);
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR),
				PROJECT_SETTING_FILE_NAME,
				Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static TagSetting GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new TagSetting();
			return m_singletonInstance;
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			if (File.Exists(this.SettingFileFullPath) == false) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingFileFullPath))
				{
					this.Setting = (TagSettings)new XmlSerializer(typeof(TagSettings)).Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("タグ設定「" + (this.SettingFileFullPath) + "」の読み込みに失敗しました。", ex);
			}

			UpdateProjectConf();
		}

		/// <summary>
		/// プロジェクト別の設定更新
		/// </summary>
		private void UpdateProjectConf()
		{
			if (File.Exists(this.SettingProjectFileFullPath) == false) return;

			// 上書きする元がなければ何もしない
			if (this.Setting == null) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingProjectFileFullPath))
				{
					var projectSetting =
						(TagSettings)new XmlSerializer(typeof(TagSettings)).Deserialize(fs);

					// 上書き
					foreach (var prop in projectSetting.GetType().GetProperties().Where(
						p => Attribute.GetCustomAttribute(p, typeof(XmlArrayItemAttribute)) != null))
					{
						// コレクション要素で要素数が1つ以上あるもののみ、すなわち要素が存在する場合だけを上書き
						if (prop.GetValue(projectSetting) is ICollection
							&& ((ICollection)prop.GetValue(projectSetting)).Count > 0)
						{
							this.Setting.GetType().GetProperty(prop.Name).SetValue(
								this.Setting,
								prop.GetValue(projectSetting));
						}
					}

					foreach (var prop in projectSetting.GetType().GetProperties().Where(
						p => Attribute.GetCustomAttribute(p, typeof(XmlElementAttribute)) != null))
					{
						// Null以外、すなわち要素が存在する場合だけ上書き
						if (prop.GetValue(projectSetting) != null)
						{
							this.Setting.GetType().GetProperty(prop.Name).SetValue(
								this.Setting,
								prop.GetValue(projectSetting));
						}
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("タグ設定「" + (this.SettingProjectFileFullPath) + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>設定</summary>
		public TagSettings Setting { get; set; }
		/// <summary>設定ファイル フルパス</summary>
		private string SettingFileFullPath
		{
			get
			{
				return Path.Combine(
					Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					SETTING_FILE_BASE_DIR,
					SETTING_FILE_NAME);
			}
		}
		/// <summary>プロジェクト毎の設定ファイルのパス </summary>
		private string SettingProjectFileFullPath
		{
			get
			{
				return Path.Combine(
					Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					SETTING_FILE_DIR,
					PROJECT_SETTING_FILE_NAME);
			}
		}
	}

	/// <summary>
	/// タグ設定
	/// </summary>
	[XmlRoot("TagSettings")]
	public class TagSettings
	{
		/// <summary>ターゲットページ設定</summary>
		[XmlArray("TargetPages")]
		[XmlArrayItem("TargetPage")]
		public List<TargetPage> TargetPages { get; set; }

		/// <summary>利用不可タグリスト</summary>
		[XmlArray("NotUsedReplaceTags")]
		[XmlArrayItem("ReplaceTagKey")]
		public List<string> NotUsedReplaceTags { get; set; }
	}

	/// <summary>
	/// ターゲットページ
	/// </summary>
	[Serializable]
	public class TargetPage
	{
		/// <summary>ページパス</summary>
		[XmlAttribute("Path")]
		public string Path { get; set; }
		/// <summary>ページ名</summary>
		[XmlAttribute("Name")]
		public string Name { get; set; }
		/// <summary>アクションタイプ</summary>
		[XmlAttribute("ActionType")]
		public string ActionType { get; set; }
		/// <summary>ロギング</summary>
		[XmlAttribute("Logging")]
		public bool Logging { get; set; }
	}
}