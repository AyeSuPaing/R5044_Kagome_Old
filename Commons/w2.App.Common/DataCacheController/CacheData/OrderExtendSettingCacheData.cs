/*
=========================================================================================================
  Module      : 注文拡張項目設定キャッシュコントローラ(OrderExtendSettingCacheData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.OrderExtendSetting;
using Validator = w2.App.Common.Util.Validator;

namespace w2.App.Common.DataCacheController.CacheData
{
	/// <summary>
	/// 注文拡張項目設定キャッシュコントローラ
	/// </summary>
	public class OrderExtendSettingCacheData
	{
		/// <summary>バリューテキスト ルート名</summary>
		public const string VALIDATE_NAME = "OrderExtendSetting";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderExtendSettingCacheData()
		{
			this.ValidaterMaster = new ValidatorXml();
			this.SettingModels = new OrderExtendSettingModel[] { };
		}

		/// <summary>
		/// プロパティセット
		/// </summary>
		public void SetProperty()
		{
			this.SettingModels = DomainFacade.Instance.OrderExtendSettingService.GetAll().Select(
				m =>
				{
					m.OutlineHtmlEncode = GetOrderExtendSettingOutLine(m.OutlineKbn, m.Outline);
					m.ValidatorXmlColumn = GetValidatorXmlColumn(m.Validator);
					return m;
				}).ToArray();

			this.SettingModelsForFront = this.SettingModels.Where(m => m.CanUseFront).ToArray();
			this.ValidaterMasterForFrontRegister = GetValidatorXmlDocument(this.SettingModelsForFront);

			var settingModelsForFrontModify = this.SettingModelsForFront.Where(m => m.CanUseModify).ToArray();
			this.ValidaterMasterForFrontModify = GetValidatorXmlDocument(settingModelsForFrontModify);

			this.SettingModelsForEcManager = this.SettingModels.Where(m => (m.CanUseEc)).ToArray();
			this.ValidaterMasterForEcManager = GetValidatorXmlDocument(this.SettingModelsForEcManager);
		}

		/// <summary>
		/// バリデーション内容の取得
		/// </summary>
		/// <param name="models">注文拡張項目設定モデル</param>
		/// <returns>バリデーション内容</returns>
		private XmlDocument GetValidatorXmlDocument(OrderExtendSettingModel[] models)
		{
			var validaterText = string.Format(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?><{0}>{1}</{0}>",
				VALIDATE_NAME,
				string.Join("", models.Select(m => m.Validator)));
			var result = (XmlDocument)Validator.GetValidateXmlDocument(VALIDATE_NAME, validaterText);
			return result;
		}

		/// <summary>
		/// バリデーション内容 カラム内容の取得
		/// </summary>
		/// <param name="xml">バリデーションXML</param>
		/// <returns>バリデーション内容</returns>
		private ValidatorXmlColumn GetValidatorXmlColumn(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var newNode = (XmlNode)doc.DocumentElement;
			var result = new ValidatorXmlColumn(newNode);
			return result;
		}

		/// <summary>
		/// 概要取得(Text,Html判定）
		/// </summary>
		/// <param name="type">HTML区分（TEXT/HTML）</param>
		/// <param name="content">概要内容</param>
		/// <returns>区分に合わせた概要内容</returns>
		private string GetOrderExtendSettingOutLine(string type, string content)
		{
			switch (type)
			{
				case Constants.FLG_ORDEREXTENDSETTING_OUTLINE_HTML:
				{
					// 相対パスを絶対パスに置換(aタグ、imgタグのみ）
					var relativePath = Regex.Matches(
						content,
						"(a[\\s]+href=|img[\\s]+src=)([\"|']([^/|#][^\\\"':]+)[\"|'])",
						RegexOptions.IgnoreCase);
					foreach (Match match in relativePath)
					{
						var resourceUri = new Uri(HttpContext.Current.Request.Url, match.Groups[3].ToString());
						content = content.Replace(
							match.Groups[2].ToString(),
							"\"" + resourceUri.PathAndQuery + resourceUri.Fragment + "\"");
					}

					return content;
				}

				case Constants.FLG_ORDEREXTENDSETTING_OUTLINE_TEXT:
					return HtmlSanitizer.HtmlEncodeChangeToBr(content);

				default:
					return string.Empty;
			}
		}

		/// <summary>バリデーション内容 全部</summary>
		public XmlDocument ValidaterMaster { get; private set; }
		/// <summary>バリデーション内容 Front登録</summary>
		public XmlDocument ValidaterMasterForFrontRegister { get; private set; }
		/// <summary>バリデーション内容 Front編集</summary>
		public XmlDocument ValidaterMasterForFrontModify { get; private set; }
		/// <summary>バリデーション内容 管理画面</summary>
		public XmlDocument ValidaterMasterForEcManager { get; private set; }
		/// <summary>注文拡張項目 全部</summary>
		public OrderExtendSettingModel[] SettingModels { get; private set; }
		/// <summary>注文拡張項目 Front登録</summary>
		public OrderExtendSettingModel[] SettingModelsForFront { get; private set; }
		/// <summary>注文拡張項目 管理画面</summary>
		public OrderExtendSettingModel[] SettingModelsForEcManager { get; private set; }
	}
}
