/*
=========================================================================================================
  Module      : MailReplacementTagクラス(MailReplacementTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.Common;
using w2.Common.Helper;

/// <summary>
/// メール置換タグクラス
/// </summary>
public class MailReplacementTag
{
	/// <summary>インスタンスオブジェクト</summary>
	private static readonly Lazy<MailReplacementTag> m_instance =
		new Lazy<MailReplacementTag>(() => new MailReplacementTag());
	/// <summary>全メールに設定できるタグを宣言する際のメールID</summary>
	private const string MAIL_REPLACE_TAG_MAIL_ID_ALL = "All";

	/// <summary>
	/// プライベートコンストラクタ
	/// </summary>
	private MailReplacementTag()
	{
	}

	/// <summary>
	/// インスタンス取得
	/// </summary>
	/// <returns>インスタンス</returns>
	public static MailReplacementTag GetInstance()
	{
		return m_instance.Value;
	}

	/// <summary>
	/// メールテンプレートタグをデシリアライズを取得
	/// </summary>
	/// <returns>デシリアライズされたMailTemplateTagオブジェクト</returns>
	public static MailTemplateTag GetDeserializeMailTemplateTag()
	{
		var filePath = AppDomain.CurrentDomain.BaseDirectory
			+ Constants.FILEPATH_XML_MAIL_REPLACEMENT_TAG.Replace("/", @"\");
		var attr = File.GetAttributes(filePath);
		//読み取り専用属性があるか調べる
		if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
		{
			//読み取り専用属性を削除する
			File.SetAttributes(filePath, attr & (~FileAttributes.ReadOnly));
		}
		return SerializeHelper.DeserializeFromXmlFile<MailTemplateTag>(filePath);
	}

	/// <summary>
	/// メールIDで使用可能メール置換タグを取得(オプション考慮)
	/// </summary>
	/// <param name="mailId">メールID</param>
	/// <returns>オプションを考慮した使用可能メール置換タグ一覧</returns>
	public static MailTemplateTagTagsValue[] GetAvailableMailTemplateTagArrayByMailId(string mailId)
	{
		try
		{
			var allTagList = GetDeserializeMailTemplateTag();

			var mailTemplateTagList = new List<MailTemplateTagTagsValue>();


			// メールIDがあればメールIDに応じた設定を取得
			if (HasInnternalElements(mailId))
			{
				var mailTemplateTags = allTagList.Tags.First(fv => fv.MailId.Contains(mailId));
				if (mailTemplateTags.Value != null) mailTemplateTagList.AddRange(mailTemplateTags.Value);
			}

			// 全メールIDで設定できる項目を取得
			mailTemplateTagList.AddRange(allTagList.Tags.First(fv => fv.MailId == MAIL_REPLACE_TAG_MAIL_ID_ALL).Value);

			var mailTemplateTagArray = mailTemplateTagList.Any()
				? mailTemplateTagList.Where(v => (v.MailId == null) || v.MailId.Contains(mailId) || v.MailId == MAIL_REPLACE_TAG_MAIL_ID_ALL).Where(
					v => (v.Option == null)
						|| (v.Option == "Point" && Constants.W2MP_POINT_OPTION_ENABLED)
						|| (v.Option == "Coupon" && Constants.W2MP_COUPON_OPTION_ENABLED)
						|| (v.Option == "SetPromotion" && Constants.SETPROMOTION_OPTION_ENABLED)
						|| (v.Option == "Gift" && Constants.GIFTORDER_OPTION_ENABLED)
						|| (v.Option == "Global" && Constants.GLOBAL_OPTION_ENABLE)
						|| (v.Option == "Receipt" && Constants.RECEIPT_OPTION_ENABLED)
						|| (v.Option == "OrderExtend" && Constants.ORDEREXTENDSTATUSSETTING_OPTION_ENABLED)
						|| ((v.Option == "SubscriptionBox") && Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
						|| ((v.Option == "PersonalAuthentication") && Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
						|| ((v.Option == "ProductSet") && Constants.PRODUCT_SET_OPTION_ENABLED)
						|| ((v.Option == "ListUnsubscribe") && Constants.MAIL_LISTUNSUBSCRIBE_OPTION_ENABLED)).ToArray()
				: new MailTemplateTagTagsValue[] { };

			return mailTemplateTagArray;
		}
		catch (Exception ex)
		{
			throw new w2Exception(string.Format("MailReplacementTag:{0}で例外が発生しました。", mailId), ex);
		}
	}

	/// <summary>
	/// メールIDで使用可能メール置換タグをすべて取得
	/// </summary>
	/// <param name="mailId">メールID</param>
	/// <returns>使用可能メール置換タグ一覧</returns>
	public static string GetMailTemplateTagArrayAllByMailId(string mailId)
	{
		try
		{
			var allTagList = GetDeserializeMailTemplateTag();

			var mailTemplateTagList = new List<MailTemplateTagTagsValue>();

			// メールIDに応じた設定を取得
			var mailTemplateTags = allTagList.Tags.First(fv => fv.MailId.Contains(mailId));
			if (mailTemplateTags.Value != null) mailTemplateTagList.AddRange(mailTemplateTags.Value);

			// 全メールIDで設定できる項目を取得
			mailTemplateTagList.AddRange(allTagList.Tags.First(fv => fv.MailId == MAIL_REPLACE_TAG_MAIL_ID_ALL).Value);
			
			var mailTemplateTagaArray = mailTemplateTagList.Any()
				? mailTemplateTagList.Where(v => (v.MailId == null) || v.MailId.Contains(mailId) || v.MailId == MAIL_REPLACE_TAG_MAIL_ID_ALL).ToArray()
				: new MailTemplateTagTagsValue[] { };
			var result = string.Join(",", mailTemplateTagaArray.Select(kvp => kvp.Text));

			return result;
		}
		catch (Exception ex)
		{
			throw new w2Exception(string.Format("MailReplacementTag:{0}で例外が発生しました。", mailId), ex);
		}
	}

	/// <summary>
	/// XMLの要素チェック
	/// </summary>
	/// <param name="mailId">メールID</param>
	/// <returns>要素があるか</returns>
	public static bool HasInnternalElements(string mailId)
	{
		if (string.IsNullOrEmpty(mailId)) return false;

		var result = GetDeserializeMailTemplateTag().Tags.FirstOrDefault(fv => fv.MailId.Contains(mailId));
		return (result != null);
	}

	/// <summary>
	/// メールテンプレートタグ親クラス
	/// </summary>
	[Serializable]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class MailTemplateTag
	{
		[XmlElement("Tags")]
		public MailTemplateTagTags[] Tags { get; set; }
	}

	/// <summary>
	/// メールテンプレートタグID別クラス
	/// </summary>
	[Serializable]
	[XmlType(AnonymousType = true)]
	public class MailTemplateTagTags
	{
		[XmlElement("Value")]
		public MailTemplateTagTagsValue[] Value { get; set; }

		[XmlAttribute("mailId")]
		public string MailId { get; set; }
	}

	/// <summary>
	/// メールテンプレートタグID別詳細クラス
	/// </summary>
	[Serializable]
	[XmlType(AnonymousType = true)]
	public class MailTemplateTagTagsValue
	{
		[XmlAttribute("text")]
		public string Text { get; set; }

		[XmlAttribute("value")]
		public string Value { get; set; }

		[XmlAttribute("option")]
		public string Option { get; set; }

		[XmlAttribute("mailId")]
		public string MailId { get; set; }
	}
}
