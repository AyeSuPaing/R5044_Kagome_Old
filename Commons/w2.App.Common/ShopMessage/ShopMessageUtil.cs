/*
=========================================================================================================
  Module      : ショップメッセージユーティリティーモジュール(ShopMessageUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Translation;
using w2.Common.Web;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

namespace w2.App.Common.ShopMessage
{
	/// <summary>
	/// ショップメッセージユーティリティーモジュール
	/// </summary>
	public class ShopMessageUtil
	{
		/// <summary>ショップメッセージタグフォーマット</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG = "@@ ShopMessage.{0} @@";
		/// <summary>ショップメッセージタグフォーマット前半空白入り</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG_FIRST_PART_WITH_SPACE = "@@ ShopMessage";
		/// <summary>ショップメッセージタグフォーマット前半空白なし</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG_FIRST_PART_WITHOUT_SPACE = "@@ShopMessage";
		/// <summary>空白入り埋め込みタグ形式</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG_WITH_SPACE = "@@ ShopMessage[.][a-zA-Z0-9]+ @@";
		/// <summary>空白なし埋め込みタグ形式</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG_WITHOUT_SPACE = "@@ShopMessage[.][a-zA-Z0-9]+@@";
		/// <summary>空白入り埋め込みタグ形式後半タグなし</summary>
		public static string FORMAT_SHOP_MESSAGE_TAG_WITHOUT_LAST_TAG = "@@ ShopMessage[.][a-zA-Z0-9]+";
		/// <summary>振り分けタグ：決済種別ID</summary>
		private const string TAG_ORDER_ORDER_PAYMENT_KBN = "OrderPaymentKbn";

		/// <summary>
		/// ショップメッセージタグ置換
		/// </summary>
		/// <param name="replaceText">置換前文字列</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="doHtmlEncodeChangeToBr">改行をbrにするか</param>
		/// <returns>置換後文字列</returns>
		public static string ConvertShopMessage(
			string replaceText,
			string languageCode,
			string languageLocaleId,
			bool doHtmlEncodeChangeToBr)
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var translationSettings =
					new NameTranslationSettingService().GetTranslationSettingsByMasterIdAndLanguageCode(
						new NameTranslationSettingSearchCondition
						{
							DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
							MasterId1 = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
							LanguageCode = languageCode,
							LanguageLocaleId = languageLocaleId
						});

				// 名称翻訳設定で置換
				foreach (var translationSetting in translationSettings)
				{
					foreach (Match shopMessageTag in Regex.Matches(
						replaceText,
						string.Format(FORMAT_SHOP_MESSAGE_TAG, translationSetting.TranslationTargetColumn)))
					{
						replaceText = replaceText.Replace(
							shopMessageTag.Value,
							doHtmlEncodeChangeToBr
								? HtmlSanitizer.HtmlEncodeChangeToBr(translationSetting.AfterTranslationalName)
								: translationSetting.AfterTranslationalName);
					}
				}
			}

			var shopMessage = GetShopMessage();

			// ShopMessage.xmlで置換
			foreach (XmlNode xmlNode in shopMessage.DocumentElement.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Comment) continue;

				foreach (Match shopMessageTag in Regex.Matches(
					replaceText,
					string.Format(FORMAT_SHOP_MESSAGE_TAG, xmlNode.Name)))
				{
					var textNode = xmlNode.SelectSingleNode("Text");
					replaceText = replaceText.Replace(
						shopMessageTag.Value,
						doHtmlEncodeChangeToBr
							? HtmlSanitizer.HtmlEncodeChangeToBr(textNode.InnerText)
							: textNode.InnerText);
				}
			}

			return replaceText;
		}
		/// <summary>
		/// ショップメッセージタグ置換
		/// </summary>
		/// <param name="replaceText">置換前文字列</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <param name="doHtmlEncodeChangeToBr">改行をbrにするか</param>
		/// <returns>置換後文字列</returns>
		public static StringBuilder ConvertShopMessage(
			StringBuilder replaceText,
			string languageCode,
			string languageLocaleId,
			bool doHtmlEncodeChangeToBr)
		{
			var after = ConvertShopMessage(
				replaceText.ToString(),
				languageCode,
				languageLocaleId,
				doHtmlEncodeChangeToBr);
			return new StringBuilder(after);
		}

		/// <summary>
		/// ショップメッセージのxmlの値を取得
		/// </summary>
		/// <returns>ショップメッセージ</returns>
		public static XmlDocument GetShopMessage()
		{
			var shopMessage = new XmlDocument();
			shopMessage.Load(
				Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.FILE_XML_FRONT_SHOP_MESSAGE));

			return shopMessage;
		}

		/// <summary>
		/// メッセージ取得
		/// </summary>
		/// <param name="messageType">メッセージタイプ</param>
		/// <returns>メッセージ</returns>
		public static string GetMessage(string messageType)
		{
			try
			{
				var shopMessage = GetShopMessage();
				var message = shopMessage.SelectSingleNode(string.Format("ShopMessage/{0}/Text", messageType))
					.InnerText;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					message = NameTranslationCommon.GetTranslationName(
						Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
						Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION,
						messageType,
						message);
				}
				return message;
			}
			catch
			{
				// 何もしない //
			}

			return string.Empty;
		}

		/// <summary>
		/// サイト基本設定ににあるタイトルとメールタグを一緒に取得
		/// </summary>
		/// <returns>タイトルとメールタグを含む配列</returns>
		public static KeyValuePair<string, string>[] GetMailTagArrayByShopMassage()
		{
			var result = GetShopMessage().SelectSingleNode("ShopMessage").ChildNodes
				.Cast<XmlNode>().Where(node => (node.NodeType == XmlNodeType.Element)).Select(
					v => new KeyValuePair<string, string>(
						string.Format("@@ ShopMessage.{0} @@", v.Name),
						v.FirstChild.InnerText)).ToArray();

			return result;
		}

		/// <summary>
		/// サイト基本設定にあるメールタグをカンマ区切りで取得
		/// </summary>
		/// <returns>カンマ区切りのメールタグ一覧</returns>
		public static string GetMailTagByShopMessage()
		{
			var result = string.Join(",", GetShopMessage().SelectSingleNode("ShopMessage").ChildNodes.Cast<XmlNode>()
				.Where(node => (node.NodeType == XmlNodeType.Element))
				.Select(v => string.Format("@@ ShopMessage.{0} @@", v.Name)));

			return result;
		}

		/// <summary>
		/// サイト基本設定にある特商法に基づく記載を決済種別ごとに取得
		/// </summary>
		/// <param name="paymentId">決済種別id</param>
		/// <returns>ショップメッセージ</returns>
		public static string GetMessageByPaymentId(string paymentId = "")
		{
			var message = GetMessage("SpecifiedCommercialTransactions");
			// 決済種別の指定が無ければ全て表示する
			if (string.IsNullOrEmpty(paymentId))
			{
				var validPayments = DataCacheControllerFacade.GetPaymentCacheController().GetValidAllWithPrice();
				foreach (var validPayment in validPayments)
				{
					message = SetTagEnabled(message, TAG_ORDER_ORDER_PAYMENT_KBN + ":" + validPayment.PaymentId);
				}
			}
			else
			{
				// 該当決済種別ID有効
				message = SetTagEnabled(message, TAG_ORDER_ORDER_PAYMENT_KBN + ":" + paymentId);
				// その他の決済種別ID無効
				message = SetTagDisabled(message, TAG_ORDER_ORDER_PAYMENT_KBN + ":" + "((?!@@>).)*");
			}

			return message;
		}

		/// <summary>
		/// タグ有効化
		/// </summary>
		/// <param name="message">置換対象メッセージ</param>
		/// <param name="strTagName">タグ名</param>
		/// <returns>ショップメッセージ</returns>
		private static string SetTagEnabled(string message, string strTagName)
		{
			string strTagBgn = "<@@" + strTagName + "@@>";
			string strTagEnd = "</@@" + strTagName + "@@>";
			message = Regex.Replace(message, strTagBgn, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			message = Regex.Replace(message, strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			return message;
		}

		/// <summary>
		/// タグ無効化
		/// </summary>
		/// <param name="message">置換対象メッセージ</param>
		/// <param name="strTagName">タグ名</param>
		/// <returns>ショップメッセージ</returns>
		private static string SetTagDisabled(string message, string strTagName)
		{
			string strTagBgn = "<@@" + strTagName + "@@>";
			string strTagEnd = "</@@" + strTagName + "@@>";
			var result = Regex.Replace(message, strTagBgn + "(?:(?!" + strTagBgn + "|" + strTagEnd + ").)*" + strTagEnd, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			return result;
		}
	}
}
