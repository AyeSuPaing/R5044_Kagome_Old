/*
=========================================================================================================
  Module      : メールテンプレート取得ユーティリティ(GetMailTemplateUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using w2.Common.Extensions;
using w2.Domain.MailTemplate;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// メールテンプレート取得ユーティリティ
	/// </summary>
	public class GetMailTemplateUtility
	{
		/// <summary>
		/// 全てのメールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateAll(string shopId)
		{
			return new MailTemplateService().GetAll(shopId);
		}

		/// <summary>
		/// 受注情報出力用メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateForOrder(string shopId)
		{
			var mailCategorys = new List<string>()
			{
				Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_ORDER,
				Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM,
				Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER,
			};
			if (Constants.STORE_PICKUP_OPTION_ENABLED && Constants.REALSHOP_OPTION_ENABLED)
			{
				mailCategorys.Add(Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP);
				mailCategorys.Add(Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP);
			}

			// 取得するメールテンプレートのカテゴリを指定
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				mailCategorys.ToArray());
			return models;
		}

		/// <summary>
		/// Get mail template for store pick up order
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <returns>Email template model</returns>
		public static MailTemplateModel[] GetMailTemplateForStorePickUpOrder(string shopId)
		{
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				new[]
				{
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP,
				});

			return models;
		}

		/// <summary>
		/// 定期購入情報出力用メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateForFixedPurchase(string shopId)
		{
			// 取得するメールテンプレートのカテゴリを指定
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				new[]
				{
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FIXEDPURCHASE,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE,
				});
			return models;
		}

		/// <summary>
		/// 入荷通知メール情報出力用メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateForProductArrival(string shopId)
		{
			// 取得するメールテンプレートのカテゴリを指定
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				new[]
				{
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_PRODUCTARRIVAL,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM,
				});
			return models;
		}

		/// <summary>
		/// 在庫減少アラートメール情報出力用メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateForStockAlertMail(string shopId)
		{
			// 取得するメールテンプレートのカテゴリを指定
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				new[]
				{
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_PRODUCTARRIVAL,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM,
				});
			return models;
		}

		/// <summary>
		/// カスタムメール出力用メールテンプレート情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>メールテンプレートモデル列</returns>
		public static MailTemplateModel[] GetMailTemplateForCustom(string shopId)
		{
			// 取得するメールテンプレートのカテゴリを指定
			var models = new MailTemplateService().GetMailTemplateByCategory(
				shopId,
				new[]
				{
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE,
					Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER,
				});
			return models;
		}

		/// <summary>
		/// 表示用メール内容変換
		/// </summary>
		/// <param name="mailTextReplaces">解凍したタグ置換情報</param>
		/// <param name="convertTarget">変換対象</param>
		/// <returns>変換後のメール内容</returns>
		public static string ConvertMailContentsForDisplay(
			Dictionary<string, string> mailTextReplaces,
			string convertTarget)
		{
			var matchCollection = Regex.Matches(convertTarget, "<@@user:((?!@@>).)*@@>");

			foreach (Match match in matchCollection)
			{
				var key = match.Value.Replace("<@@user:", "").Replace("@@>", "");
				if (mailTextReplaces.ContainsKey(key))
				{
					convertTarget = convertTarget.Replace(
						match.Value,
						mailTextReplaces[key]);
				}
			}

			return convertTarget;
		}
	}
}
