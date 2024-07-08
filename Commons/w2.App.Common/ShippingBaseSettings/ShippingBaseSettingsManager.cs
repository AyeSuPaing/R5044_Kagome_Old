/*
=========================================================================================================
  Module      : 配送拠点設定読み取り処理(ShippingBaseSettingsManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.ShippingBaseSettings
{
	/// <summary>
	/// 配送拠点の管理
	/// </summary>
	public sealed class ShippingBaseSettingsManager
	{
		/// <summary>配送拠点設定ファイル名</summary>
		private readonly string CONST_SHIPPING_BASE_XML_FILE_NAME = @"Settings\ShippingBaseSettings.xml";

		/// <summary>インスタンス</summary>
		private static ShippingBaseSettingsManager s_instance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ShippingBaseSettingsManager()
		{
			Update();
			FileUpdateObserver.GetInstance()
				.AddObservation(
					Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					CONST_SHIPPING_BASE_XML_FILE_NAME,
					Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static ShippingBaseSettingsManager GetInstance()
		{
			return s_instance = new ShippingBaseSettingsManager();
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			if (File.Exists(this.SettingFileFullPath) == false) return;
			try
			{
				var xDocument = XDocument.Load(this.SettingFileFullPath);
				var setting = new List<ShippingBaseSettings>();
				foreach (var element in xDocument.Root.Descendants("ShippingBase"))
				{
					setting.Add(
						new ShippingBaseSettings
						{
							Id = element.Attribute("id").Value,
							Name = element.Attribute("name").Value,
							EastWestFlg = element.Attribute("eastWestFlg").Value,
						});
				}
				this.Settings = setting.ToArray();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError($"配送拠点設定XML「{this.SettingFileFullPath}」の読み込みに失敗しました。", ex);
			}
		}

		#region プロパティ
		/// <summary>配送拠点設定</summary>
		public ShippingBaseSettings[] Settings { get; private set; }
		/// <summary>設定ファイルディレクトリ</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, CONST_SHIPPING_BASE_XML_FILE_NAME); }
		}
		#endregion
	}

	/// <summary>
	/// 配送拠点設定
	/// </summary>
	[Serializable]
	public sealed class ShippingBaseSettings
	{
		/// <summary>配送拠点ID</summary>
		public string Id { get; set; }
		/// <summary>配送拠点名</summary>
		public string Name { get; set; }
		/// <summary>東西拠点フラグ（東日本：0　西日本：1）</summary>
		public string EastWestFlg { get; set; }
	}
}
