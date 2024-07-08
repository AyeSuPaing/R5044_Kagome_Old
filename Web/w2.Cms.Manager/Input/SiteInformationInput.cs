/*
=========================================================================================================
  Module      : サイト情報入力クラス(ShopInformationInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using w2.App.Common;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// サイト情報入力クラス
	/// </summary>
	public class SiteInformationInput
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SiteInformationInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="xmlDocument">SHOPメッセージXML</param>
		public SiteInformationInput(XmlDocument xmlDocument)
		{
			SetShopMessage(xmlDocument);
		}
		#endregion

		/// <summary>
		/// 表示情報をセット
		/// </summary>
		/// <param name="xmlDocument">SHOPメッセージXML</param>
		private void SetShopMessage(XmlDocument xmlDocument)
		{
			var lXmlInfo = new List<SiteInformation>();
			foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Comment)
				{
					continue;
				}

				lXmlInfo.Add(new SiteInformation(xmlNode));
			}

			this.XmlInfo = lXmlInfo.ToArray();

			if (Constants.GLOBAL_OPTION_ENABLE == false) return;
			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
				MasterId1 = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
				MasterId2 = string.Empty,
				MasterId3 = string.Empty,
			};

			var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
			this.SiteInformationTranslation = translationData.GroupBy(d => new { d.LanguageCode, d.LanguageLocaleId })
				.Select(groupingData => new SiteInformationTranslation(groupingData)).ToArray();
		}

		#region プロパティ
		/// <summary>サイト情報リスト</summary>
		public SiteInformation[] XmlInfo { get; set; }
		/// <summary>サイト基本情報翻訳設定情報</summary>
		public SiteInformationTranslation[] SiteInformationTranslation { get; set; }
		#endregion

		/// <summary>
		/// サイト情報クラス
		/// </summary>
		public class SiteInformation
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public SiteInformation()
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="xnShopMessage">ショップメッセージのノード</param>
			public SiteInformation(XmlNode xnShopMessage)
			{
				this.NodeName = xnShopMessage.Name;
				this.InputNodeName = xnShopMessage.Name;
				foreach (XmlNode xn in xnShopMessage.ChildNodes)
				{
					switch (xn.Name)
					{
						case "Name":
							this.TagName = xn.InnerText;
							break;

						case "Text":
							this.Text = xn.InnerText;
							break;

						case "TextBoxRows":
							this.TextBoxRows = int.Parse(xn.InnerText);
							break;

						case "Description":
							this.Description = xn.InnerText;
							break;

						case "AddTag":
							this.AddTag = xn.InnerText;
							break;

						case "IsEditHtml":
							this.IsEditHtml = xn.InnerText;
							break;

						default:
							break;
					}
				}
			}

			#region プロパティ
			/// <summary>ノード名</summary>
			public string NodeName { get; set; }
			/// <summary>タグ名</summary>
			public string TagName { get; set; }
			/// <summary>設定値</summary>
			public string Text { get; set; }
			/// <summary>行数</summary>
			public int TextBoxRows { get; set; }
			/// <summary>説明文</summary>
			public string Description { get; set; }
			/// <summary>追加タグ用</summary>
			public string AddTag { get; set; }
			/// <summary>HTMLエディタを表示するか</summary>
			public string IsEditHtml { get; set; }
			/// <summary>ノード名（入力用）</summary>
			public string InputNodeName { get; set; }
			#endregion
		}
	}
}