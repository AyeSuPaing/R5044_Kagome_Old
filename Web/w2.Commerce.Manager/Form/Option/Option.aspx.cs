/*
=========================================================================================================
  Module      : オプション訴求ページ処理(Option.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Xml.Linq;
using OptionAppeal;
using w2.App.Common;
using w2.App.Common.OptionAppeal;

public partial class Form_Option_Option : BasePage
{
	/// <summary>お客様向けメールフラグ</summary>
	private const int MAILE_TO_CUSTOMER = 0;
	/// <summary>管理者向けメールフラグ</summary>
	private const int MAILE_TO_ADMINISTER = 1;

	/// <summary>静的オプションリスト</summary>
	private static List<OptionItem> _optionItems;
	/// <summary>静的カテゴリリスト</summary>
	private static List<OptionCategory> _categorys;
	/// <summary>静的メールテンプレート</summary>
	private static readonly string _mailSetting = new OptionContentsGetter().GetMailSetting();

	/// <summary>オプション訴求：メールテンプレートタイトル</summary>
	private const string OPTIONAPPEAL_MAIL_TEMPLATE_TITLE = "Title";
	/// <summary>オプション訴求：メールテンプレート本文</summary>
	private const string OPTIONAPPEAL_MAIL_TEMPLATE_BODY = "Body";
	/// <summary>Cookieキー:オプション訴求ユーザー名保持用クッキー変数</summary>
	protected const string COOKIE_KEY_OPTIONAPPEAL_USER_NAME = "w2Manager_OptionAppeal_UserName";
	/// <summary>Cookieキー:オプション訴求ユーザーメール保持用クッキー変数</summary>
	protected const string COOKIE_KEY_OPTIONAPPEAL_USER_EMAIL = "w2Manager_OptionAppeal_UserEmail";
	/// <summary>Cookieキー:オプション訴求ユーザー電話番号保持用クッキー変数</summary>
	protected const string COOKIE_KEY_OPTIONAPPEAL_USER_TEL_NUMBER = "w2Manager_OptionAppeal_UserTelNumber";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// オプションxml取得
		this.OptionInfo = new OptionXmlReader().Read();

		_optionItems = this.OptionInfo.OptionList;
		_categorys = this.OptionInfo.ContainIntroducedOptionCategoryList;

		// オプション情報データバインド
		rOptionCategoryList.DataSource = this.OptionInfo.OptionCategoryList;
		rOptionCategoryList.DataBind();

		// オプション分類データバインド
		rNarrow.DataSource = this.OptionInfo.OptionCategoryList;
		rNarrow.DataBind();

		// スライダーデータバインド
		rSlider.DataSource = this.OptionInfo.OptionSliderList;
		rSlider.DataBind();

		// 人気オプションスライダーデータバインド
		rPopularOptionSlider.DataSource = this.OptionInfo.PopularOptionSliderList;
		rPopularOptionSlider.DataBind();
	}

	/// <summary>
	/// 問い合わせ連絡先情報を取得
	/// </summary>
	/// <param name="xml">xml</param>
	/// <returns>問い合わせ連絡先情報</returns>
	private static Dictionary<string, string> GetInquiryInformation(string xml)
	{
		// オプション訴求：XMLファイルデフォルトセッティングタグ
		const string OPTIONAPPEAL_DISPLAY_SETTINGS = "OptionAppealDisplaySettings";
		// オプション訴求：XMLファイル問合せいタグ
		const string OPTIONAPPEAL_INQUIRY = "Inquiry";
		// オプション訴求：電話番号
		const string OPTIONAPPEAL_TEL_NUMBER = "TelNumber";
		// オプション訴求：受付時間
		const string OPTIONAPPEAL_RECEPTION_TIME = "ReceptionTime";

		var optionXDocument = XDocument.Parse(xml);
		var inquiryInformation = optionXDocument.Descendants(OPTIONAPPEAL_DISPLAY_SETTINGS).Nodes()
			.OfType<XElement>().FirstOrDefault(node => node.Name.LocalName ==OPTIONAPPEAL_INQUIRY);
		var telNumber = inquiryInformation.Element(OPTIONAPPEAL_TEL_NUMBER).Value;
		var receptionTime = inquiryInformation.Element(OPTIONAPPEAL_RECEPTION_TIME).Value;
		var result = new Dictionary<string, string>
		{
			{ OPTIONAPPEAL_TEL_NUMBER, telNumber },
			{ OPTIONAPPEAL_RECEPTION_TIME, receptionTime },
		};
		return result;
	}

	/// <summary>
	/// メールテンプレート取得
	/// </summary>
	/// <param name="type">メールタイプ</param>
	/// <returns>メールテンプレート</returns>
	private static Dictionary<string, string> GetMailContent(int type)
	{
		// オプション訴求：お客様向けメールテンプレートタグ
		const string OPTIONAPPEAL_CUSTOMER_MAIL_TEMPLATE = "InquiryCompletionMail";
		// オプション訴求：管理者向けメールテンプレートタグ
		const string OPTIONAPPEAL_ADMINISTER_MAIL_TEMPLATE = "InquiryMail";
		// オプション訴求：メールテンプレートタグ
		const string OPTIONAPPEAL_MAIL_TEMPLATE_SETTING = "MailSettings";

		var mailType = (type == MAILE_TO_ADMINISTER)
			? OPTIONAPPEAL_ADMINISTER_MAIL_TEMPLATE
			: OPTIONAPPEAL_CUSTOMER_MAIL_TEMPLATE;
		var mailSettingXDocument = XDocument.Parse(_mailSetting);
		var mailSettings = mailSettingXDocument.Descendants(OPTIONAPPEAL_MAIL_TEMPLATE_SETTING).Nodes()
			.OfType<XElement>().ToArray();
		var result = new Dictionary<string, string>();
		foreach (var mailSetting in mailSettings)
		{
			if (mailSetting.Name.LocalName == mailType)
			{
				var title = mailSetting.Element(OPTIONAPPEAL_MAIL_TEMPLATE_TITLE).Value;
				var body = mailSetting.Element(OPTIONAPPEAL_MAIL_TEMPLATE_BODY).Value;
				result.Add(OPTIONAPPEAL_MAIL_TEMPLATE_TITLE, title);
				result.Add(OPTIONAPPEAL_MAIL_TEMPLATE_BODY, body);
			}
		}

		return result;
	}

	/// <summary>
	/// メールテンプレートタグ置換
	/// </summary>
	/// <param name="mailTemplate">メールテンプレート</param>
	/// <param name="replaceTags">置換タグ</param>
	/// <returns>置換済メールテンプレート</returns>
	private static Dictionary<string, string> ReplaceTag(
		Dictionary<string, string> mailTemplate,
		Dictionary<string, string> replaceTags)
	{
		var result = new Dictionary<string, string>();
		foreach (var key in mailTemplate.Keys)
		{
			var replaceContent = string.Empty;
			foreach (var item in replaceTags.Keys)
			{
				var tag = string.Format("@@ {0} @@", item);
				if (StringUtility.ToEmpty(mailTemplate[key]).Contains(tag))
				{
					var replacement = (string.IsNullOrEmpty(replaceContent))
						? StringUtility.ToEmpty(mailTemplate[key])
						: replaceContent;
					replaceContent = replacement.Replace(
						tag,
						StringUtility.ToEmpty(replaceTags[item]));
				}
			}
			result.Add(key, replaceContent);
		}
		return result;
	}

	/// <summary>
	/// モーダルデータ
	/// </summary>
	/// <param name="optionId">オプションId</param>
	/// <returns>オプション情報</returns>
	[WebMethod]
	public static string SetOptionItem(string optionId)
	{
		var optionItem = _optionItems.FirstOrDefault(item => item.OptionId == optionId);
		if (optionItem == null) return "";
		var categoryId = optionItem.CategoryId;
		var category = _categorys.FirstOrDefault(item => item.CategoryId == categoryId);
		if (category == null) return "";
		optionItem.CategoryName = category.CategoryName;
		optionItem.ParentCategoryId = category.CategoryParent;
		var inquiryInformations = GetInquiryInformation(new OptionContentsGetter().GetSliderBase());
		var result = BasePageHelper.ConvertObjectToJsonString(
			new
			{
				Option = optionItem,
				Inquiry = inquiryInformations,
			});
		return result;
	}

	/// <summary>
	/// 問い合わせメール送信
	/// </summary>
	/// <param name="name">氏名</param>
	/// <param name="optionName">オプション名</param>
	/// <param name="mailAddress">メールアドレス</param>
	/// <param name="telNumber">電話番号</param>
	/// <param name="inquiryTypeName">問い合わせ区分</param>
	/// <param name="content">問い合わせ内容</param>
	[WebMethod]
	public static void SendMail(
		string name,
		string optionName,
		string mailAddress,
		string telNumber,
		string inquiryTypeName,
		string content)
	{
		SendMailTo(
			name,
			optionName,
			mailAddress,
			telNumber,
			inquiryTypeName,
			content,
			MAILE_TO_CUSTOMER);
		SendMailTo(
			name,
			optionName,
			mailAddress,
			telNumber,
			inquiryTypeName,
			content,
			MAILE_TO_ADMINISTER);
	}

	/// <summary>
	/// メール送信
	/// </summary>
	/// <param name="name">氏名</param>
	/// <param name="optionName">オプション名</param>
	/// <param name="mailAddress">メールアドレス</param>
	/// <param name="telNumber">電話番号</param>
	/// <param name="inquiryTypeName">問い合わせ区分</param>
	/// <param name="content">問い合わせ内容</param>
	/// <param name="target">送信対象</param>
	/// <returns>結果</returns>
	public static bool SendMailTo(
		string name,
		string optionName,
		string mailAddress,
		string telNumber,
		string inquiryTypeName,
		string content,
		int target)
	{
		// オプション訴求：メール置換タグオプション名
		const string OPTIONAPPEAL_REPLACEMENT_OPTION_NAME = "option_name";
		// オプション訴求：メール置換タグプロジェクト番号
		const string OPTIONAPPEAL_REPLACEMENT_PROJECT_NO = "project_no";
		// オプション訴求：メール置換タグフルネーム
		const string OPTIONAPPEAL_REPLACEMENT_FULL_NAME = "full_name";
		// オプション訴求：メール置換タグお問い合わせタイプ
		const string OPTIONAPPEAL_REPLACEMENT_INQUIRY_TYPE = "inquiry_selected_radio_button";
		// オプション訴求：メール置換タグお問い合わせ内容
		const string OPTIONAPPEAL_REPLACEMENT_INQUIRY_TEXT = "inquiry_text";
		// オプション訴求：メール置換タグメールアドレス
		const string OPTIONAPPEAL_REPLACEMENT_MAIL_ADDRESS = "mail_address";
		// オプション訴求：メール置換タグ電話番号
		const string OPTIONAPPEAL_REPLACEMENT_TEL_NUMBER = "tel_number";

		using (var smsMailSend = new MailSendUtility(Constants.MailSendMethod.Manual))
		{
			var projectNo = Constants.OPTIONAPPEAL_PROJECT_NO;
			var match = Regex.Match(Constants.OPTIONAPPEAL_INQUIRY_MAIL_FROM, "<.*>");
			var nameFrom = match.Success ? Constants.OPTIONAPPEAL_INQUIRY_MAIL_FROM.Replace(match.Value, "") : string.Empty;
			var emailAddress = match.Success ? match.Value.Replace("<", "").Replace(">", "") : Constants.OPTIONAPPEAL_INQUIRY_MAIL_FROM;
			smsMailSend.SetFrom(emailAddress, nameFrom);
			if (target == MAILE_TO_ADMINISTER)
			{
				var mailAddressForAdminister = Constants.OPTIONAPPEAL_INQUIRY_MAIL_TO;
				smsMailSend.AddTo(mailAddressForAdminister);
			}
			else
			{
				smsMailSend.AddTo(mailAddress);
			}
			var mailContents = GetMailContent(target);
			var replaceTags = new Dictionary<string, string> 
			{ 
				{OPTIONAPPEAL_REPLACEMENT_OPTION_NAME, optionName},
				{OPTIONAPPEAL_REPLACEMENT_PROJECT_NO, projectNo},
				{OPTIONAPPEAL_REPLACEMENT_FULL_NAME, name},
				{OPTIONAPPEAL_REPLACEMENT_INQUIRY_TYPE, inquiryTypeName},
				{OPTIONAPPEAL_REPLACEMENT_INQUIRY_TEXT, content},
				{OPTIONAPPEAL_REPLACEMENT_MAIL_ADDRESS, mailAddress},
				{OPTIONAPPEAL_REPLACEMENT_TEL_NUMBER, telNumber},
			};
			var mailSettingsAfterReplace = ReplaceTag(mailContents, replaceTags);
			var mailSubject = StringUtility.ToEmpty(mailSettingsAfterReplace[OPTIONAPPEAL_MAIL_TEMPLATE_TITLE]);
			var mailSubjectBody = StringUtility.ToEmpty(mailSettingsAfterReplace[OPTIONAPPEAL_MAIL_TEMPLATE_BODY]);
			smsMailSend.SetSubject(mailSubject);
			smsMailSend.SetBody(mailSubjectBody);
			// メール送信
			var result = smsMailSend.SendMail();
			return result;
		}
	}

	/// <summary> オプション情報 </summary>
	protected OptionXmlReader OptionInfo
	{
		get { return (OptionXmlReader)ViewState["OptionInfo"]; }
		set { ViewState["OptionInfo"] = value; }
	}
}