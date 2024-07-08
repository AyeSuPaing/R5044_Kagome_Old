/*
=========================================================================================================
  Module      : 配送拠点エリア設定読み取り処理(ShippingBaseAreaSettingsManager.cs)
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
	/// 配送拠点エリアの管理
	/// </summary>
	public sealed class ShippingBaseAreaSettingsManager
	{
		/// <summary>配送拠点設定ファイル名</summary>
		private readonly string CONST_SHIPPING_BASE_AREA_XML_FILE_NAME = @"Settings\ShippingBaseAreaSettings.xml";

		/// <summary>インスタンス</summary>
		private static ShippingBaseAreaSettingsManager s_instance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ShippingBaseAreaSettingsManager()
		{
			Update();
			FileUpdateObserver.GetInstance()
				.AddObservation(
					Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					CONST_SHIPPING_BASE_AREA_XML_FILE_NAME,
					Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static ShippingBaseAreaSettingsManager GetInstance()
		{
			return s_instance = new ShippingBaseAreaSettingsManager();
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
				var setting = new List<ShippingBaseAreaSettings>();
				foreach (var element in xDocument.Root.Descendants("Prefecture"))
				{
					setting.Add(
						new ShippingBaseAreaSettings
						{
							Id = element.Attribute("id").Value,
							Name = element.Attribute("name").Value,
							ShippingBaseId = element.Attribute("shippingBaseId").Value,
						});
				}
				this.Settings = setting.ToArray();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError($"配送拠点エリア設定XML「{this.SettingFileFullPath}」の読み込みに失敗しました。", ex);
			}
		}

		#region プロパティ
		/// <summary>配送拠点エリア設定</summary>
		public ShippingBaseAreaSettings[] Settings { get; private set; }
		/// <summary>設定ファイルディレクトリ</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, CONST_SHIPPING_BASE_AREA_XML_FILE_NAME); }
		}
		#endregion
	}

	/// <summary>
	/// 配送拠点エリア設定
	/// </summary>
	[Serializable]
	public sealed class ShippingBaseAreaSettings
	{
		/// <summary>配送拠点エリアID</summary>
		public string Id { get; set; }
		/// <summary>配送拠点エリア名</summary>
		public string Name { get; set; }
		/// <summary>配送拠点ID</summary>
		public string ShippingBaseId { get; set; }
	}
}
