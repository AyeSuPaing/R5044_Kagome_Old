/*
=========================================================================================================
  Module      : メール送信ログ確認ページ処理(MailSendLogConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using w2.App.Common.Mail;
using w2.App.Common.ShopMessage;
using w2.Domain.MailSendLog;
using w2.Domain.User;
using w2.Domain.User.Helper;

public partial class Form_Message_MailSendLogConfirm : BasePageCs
{
	#region メンバ変数
	/// <summary>絵文字タグ開始</summary>
	protected const string m_tagEmojiHead = "<@@emoji:";
	/// <summary>絵文字タグ終了</summary>
	protected const string m_tagEmojiFoot = "@@>";
	/// <summary>デコメイメージタグ開始</summary>
	protected const string m_tagDecomeImageHead = "<@@image:";
	/// <summary>デコメイメージタグ終了</summary>
	protected const string m_tagDecomeImageFoot = "@@>";
	#endregion

	#region メソッド
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// メール送信ログ表示
			DisplayMailSendLog();
		}
	}
	#endregion

	#region -DisplayMailSendLog メール送信ログ表示
	/// <summary>
	/// メール送信ログ表示
	/// </summary>
	private void DisplayMailSendLog()
	{
		var existMailSendLog = false;
		if (this.LogNo != null)
		{
			var mailSendLog = new MailSendLogService().GetForDisplay(this.LogNo.Value);
			if (mailSendLog != null)
			{
				var user = new UserService().GetUserForMailSend(mailSendLog.UserId);
				var languageCode = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
				var languageLocalId = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];

				// メール送信ログ情報表示
				lLogNo.Text = WebSanitizer.HtmlEncode(mailSendLog.LogNo);
				checkBoxReadFlg.Checked = (mailSendLog.ReadFlg == Constants.FLG_MAILSENDLOG_READ_FLG_READ);
				lMailTextId.Text = WebSanitizer.HtmlEncode(mailSendLog.MailtextId);
				lMailTextName.Text = WebSanitizer.HtmlEncode(mailSendLog.MailtextName);
				lMailDistId.Text = WebSanitizer.HtmlEncode(mailSendLog.MaildistId);
				lMailDistName.Text = WebSanitizer.HtmlEncode(mailSendLog.MaildistName);
				this.IsMailTemplate = mailSendLog.IsMailTemplate;
				lMailId.Text = WebSanitizer.HtmlEncode(mailSendLog.MailId);
				lMailName.Text = WebSanitizer.HtmlEncode(mailSendLog.MailName);

				lMailTo.Text = WebSanitizer.HtmlEncode(mailSendLog.MailTo);
				lMailCc.Text = WebSanitizer.HtmlEncode(mailSendLog.MailCc);
				lMailBcc.Text = WebSanitizer.HtmlEncode(mailSendLog.MailBcc);
				lMailFrom.Text = WebSanitizer.HtmlEncode(mailSendLog.MailFrom
					+ ((string.IsNullOrEmpty(mailSendLog.MailFromName) == false) ? " ( " + mailSendLog.MailFromName + " )" : ""));
				lDateSendMail.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						mailSendLog.DateSendMail,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lDateReadMail.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						mailSendLog.DateReadMail,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				mailSendLog.MailSubject = ShopMessageUtil.ConvertShopMessage(
					mailSendLog.MailSubject,
					languageCode,
					languageLocalId,
					false);
				mailSendLog.MailBody = ShopMessageUtil.ConvertShopMessage(
					mailSendLog.MailBody,
					languageCode,
					languageLocalId,
					false);
				mailSendLog.MailBodyHtml = ShopMessageUtil.ConvertShopMessage(
					mailSendLog.MailBodyHtml,
					languageCode,
					languageLocalId,
					true);

				if (string.IsNullOrEmpty(mailSendLog.MailtextReplaceTags) == false)
				{
					var replaceTags = StringUtility.DecompressString(mailSendLog.MailtextReplaceTags);
					var mailTextReplaces = JsonConvert.DeserializeObject<Dictionary<string, string>>(replaceTags);

					mailSendLog.MailSubject = GetMailTemplateUtility.ConvertMailContentsForDisplay(
						mailTextReplaces,
						mailSendLog.MailSubject);

					mailSendLog.MailBody = GetMailTemplateUtility.ConvertMailContentsForDisplay(
						mailTextReplaces,
						mailSendLog.MailBody);

					mailSendLog.MailBodyHtml = GetMailTemplateUtility.ConvertMailContentsForDisplay(
						mailTextReplaces,
						mailSendLog.MailBodyHtml);
				}

				this.HtmlForPreviewList = new List<string> { };
				if (mailSendLog.MailAddrKbn == Constants.FLG_MAILSENDLOG_MAIL_ADDR_KBN_PC)
				{
					lMailSubject.Text = WebSanitizer.HtmlEncode(mailSendLog.MailSubject);
					lMailBody.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(mailSendLog.MailBody));
					this.HtmlForPreviewList.Add(mailSendLog.MailBodyHtml);
				}
				else
				{
					lMailSubject.Text = ReplaceEmojiTagToHtml(mailSendLog.MailSubject);
					lMailBody.Text = this.IsMailTemplate
						? StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(mailSendLog.MailBody))
						: StringUtility.ChangeToBrTag(ReplaceEmojiTagToHtml(mailSendLog.MailBody));
					this.HtmlForPreviewList
						.Add(ReplaceEmojiTagToHtml(ReplaceImageTagToHtml(mailSendLog.MailBodyHtml)).Replace("\r", "").Replace("\n", ""));
				}
				existMailSendLog = true;

				// メール本文(テキスト)を初期表示
				lbDisplayText.Enabled = false;
				lbDisplayText.Font.Bold = true;
				trMailBodyHtml.Visible = false;
				dMessage.Visible = (this.IsMailTemplate == false);

				// メール本文(テキスト、HTML)の設定有無で表示制御
				if (string.IsNullOrEmpty(mailSendLog.MailBodyHtml) == false)
				{
					lbDisplayHtml.Enabled = false;
					lbDisplayHtml.Font.Bold = true;
					lbDisplayHtml.Visible = trMailBodyHtml.Visible = true;
					lbDisplayText.Visible = trMailBody.Visible = false;
				}
				else
				{
					lbDisplayHtml.Visible = trMailBodyHtml.Visible = false;
				}
			}
		}
		this.ExistMailSendLog = existMailSendLog;
		// メール送信ログが存在しない場合はエラーメッセージ表示
		if (this.ExistMailSendLog == false)
		{
			lNotExistErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
		}
	}
	#endregion

	#region -ConvertMailContentsForDisplay 表示用メール内容変換
	/// <summary>
	/// 表示用メール内容変換
	/// </summary>
	/// <param name="convertTarget">変換対象</param>
	/// <param name="user">ユーザ情報</param>
	/// <returns>変換後</returns>
	private string ConvertMailContentsForDisplay(string convertTarget, UserForMailSend user)
	{
		var matchCollection = Regex.Matches(convertTarget, "<@@user:((?!@@>).)*@@>");
		foreach (Match match in matchCollection)
		{
			var key = match.Value.Replace("<@@user:", "").Replace("@@>", "");
			var value = ((user != null) ? StringUtility.ToEmpty(ConvertUserInfoTag(user, key)) : "");
			convertTarget = convertTarget.Replace(match.Value, value);
		}
		return convertTarget;
	}
	#endregion

	#region -ConvertUserInfoTag ユーザ情報タグ変換
	/// <summary>
	/// ユーザ情報タグ変換
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="tagKey">タグのキー</param>
	/// <returns>変換後ユーザ情報</returns>
	private string ConvertUserInfoTag(UserForMailSend user, string tagKey)
	{
		// 返却用のユーザ情報
		string userInfo = "";
		switch (tagKey)
		{
			// 名前
			case Constants.FIELD_USER_NAME:
				userInfo = UserModel.CreateComplementUserName(
					(string)user.DataSource[tagKey],
					(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR],
					(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR2]);
				break;

			// 氏名（姓）
			case Constants.FIELD_USER_NAME1:
				userInfo =
					(string.IsNullOrEmpty((string)user.DataSource[tagKey]) && string.IsNullOrEmpty((string)user.DataSource[Constants.FIELD_USER_NAME2]))
						? UserModel.CreateComplementUserName(
							(string)user.DataSource[tagKey],
							(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR],
							(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR2])
						: (string)user.DataSource[tagKey];
				break;

			// 性別（区分）
			case Constants.FIELD_USER_SEX:
				userInfo = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, (string)user.DataSource[tagKey]);
				break;

			// 生年月日
			// ポイント有効期限
			case Constants.FIELD_USER_BIRTH:
			case Constants.FIELD_USERPOINT_POINT_EXP:
				if (user.DataSource[tagKey] != System.DBNull.Value)
				{
					userInfo = DateTimeUtility.ToStringForManager(
						user.DataSource[tagKey],
						DateTimeUtility.FormatType.ShortDate2Letter);
				}
				break;

			// パスワード
			case Constants.FIELD_USER_PASSWORD:
				// ユーザーパスワードが空文字以外の場合、復号化
				var password = (string)user.DataSource[Constants.FIELD_USER_PASSWORD];
				if (password != "")
				{
					w2.Common.Util.Security.RijndaelCrypto rcUserPassword
						= new w2.Common.Util.Security.RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
					userInfo = rcUserPassword.Decrypt(password);
				}
				// それ以外の場合、値をそのまま設定
				else
				{
					userInfo = password;
				}
				break;

			// メール配信区分（区分）
			case Constants.FIELD_USER_MAIL_FLG:
				userInfo = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, (string)user.DataSource[tagKey]);
				break;

			// 本ポイント数
			case Constants.FIELD_USERPOINT_POINT:
				userInfo = user.DataSource[tagKey].ToString();
				break;

			default:
				userInfo = (tagKey.StartsWith(Constants.FLG_USEREXTENDSETTING_PREFIX_KEY))
					? ((user.DataSource.Contains(tagKey)) ? StringUtility.ToEmpty(user.DataSource[tagKey]) : "")
					: StringUtility.ToEmpty(user.DataSource[tagKey]);
				break;
		}
		return userInfo;
	}
	#endregion

	#region #lbDisplay_Click 表示ボタンクリック
	/// <summary>
	/// 表示ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplay_Click(object sender, CommandEventArgs e)
	{
		var displayHtml = (e.CommandName == "DisplayHtml");
		ChangeDisplayButtonStyle(lbDisplayText, (displayHtml == false));
		ChangeDisplayButtonStyle(lbDisplayHtml, displayHtml);

		if (displayHtml)
		{
			trMailBody.Visible = false;
			trMailBodyHtml.Visible = true;
		}
		else
		{
			trMailBody.Visible = true;
			trMailBodyHtml.Visible = false;
		}
	}
	#endregion

	#region #ChangeDisplayButtonStyle 表示ボタンスタイル切り替え
	/// <summary>
	/// 表示ボタンスタイル切り替え
	/// </summary>
	/// <param name="lb">リンクボタン</param>
	/// <param name="enabled">有効にするか</param>
	protected void ChangeDisplayButtonStyle(LinkButton lb, bool enabled)
	{
		lb.Enabled = (enabled == false);
		lb.Font.Bold = enabled;
	}
	#endregion

	#region -ReplaceEmojiTagToHtml 絵文字タグをimgタグに変換
	/// <summary>
	/// 絵文字タグをimgタグに変換する
	/// </summary>
	/// <param name="target">変換対象</param>
	/// <returns>変換後imgタグ</returns>
	private string ReplaceEmojiTagToHtml(string target)
	{
		return ReplaceEmojiTag(target, (string match) =>
		{
			var emojiTag = match.Split(':')[1].Replace(m_tagEmojiFoot, "");
			var imgUrl = GetEmojiImageSrcUrl(emojiTag);
			var imgTag = string.Format(
				"<img src='{0}' Title='{1}' Alt='{2}' />",
				imgUrl,
				emojiTag,
				imgUrl);
			return imgTag;
		});
	}
	#endregion

	#region -GetEmojiImageSrcUrl 絵文字タグ名からimg参照先urlを取得
	/// <summary>
	/// 絵文字タグ名からimg参照先urlを取得
	/// </summary>
	/// <param name="emojiTagName">絵文字タグ名</param>
	/// <returns>絵文字画像参照先URL</returns>
	private static string GetEmojiImageSrcUrl(string emojiTagName)
	{
		if (File.Exists(Constants.EMOJI_IMAGE_DIRPATH + emojiTagName + ".png") == false)
		{
			emojiTagName = "no_emoji";
		}
		return Constants.EMOJI_IMAGE_URL + emojiTagName + ".png";
	}
	#endregion

	#region -ReplaceImageTagToHtml デコメイメージタグをimgタグに変換
	/// <summary>
	/// デコメイメージタグをimgタグに変換
	/// </summary>
	/// <param name="target">置換対象文字列</param>
	/// <returns>置換結果文字列</returns>
	private static string ReplaceImageTagToHtml(string target)
	{
		return ReplaceImageTag(target, ((Func<string, string>)((string match) =>
		{
			string strTag = match.Split(':')[1].Replace(m_tagDecomeImageFoot, "");

			return "<img src='" + Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_URL + match.Split(':')[1].Replace(m_tagDecomeImageFoot, "") + "' />";
		})));
	}
	#endregion

	#region -ReplaceEmojiTag 絵文字タグ置換
	/// <summary>
	/// 絵文字タグ置換
	/// </summary>
	/// <param name="target">処理対象</param>
	/// <param name="GetReplaceString">タグを引数に渡される置換処理</param>
	/// <returns>置換結果文字列</returns>
	private static string ReplaceEmojiTag(string target, Func<string, string> GetReplaceString)
	{
		return ReplaceEachTag(target, m_tagEmojiHead, m_tagEmojiFoot, GetReplaceString);
	}
	#endregion

	#region -ReplaceImageTag デコメイメージタグ置換
	/// <summary>
	/// デコメイメージタグ置換
	/// </summary>
	/// <param name="target">処理対象</param>
	/// <param name="GetReplaceFunction">タグを引数に渡される置換処理</param>
	/// <returns>置換結果文字列</returns>
	private static string ReplaceImageTag(string target, Func<string, string> GetReplaceString)
	{
		return ReplaceEachTag(target, m_tagDecomeImageHead, m_tagDecomeImageFoot, GetReplaceString);
	}
	#endregion

	#region -ReplaceEachTag タグ毎に置換
	/// <summary>
	/// タグ毎に置換を行う
	/// </summary>
	/// <param name="target">処理対象</param>
	/// <param name="tagHead">タグ先頭</param>
	/// <param name="tagFoot">タグ終端</param>
	/// <param name="GetReplaceString">タグを引数に渡される置換処理</param>
	/// <returns>置換結果文字列</returns>
	private static string ReplaceEachTag(string target, string tagHead, string tagFoot, Func<string, string> GetReplaceString)
	{
		var replaceResult = string.Copy(target);
		ForEachTag(replaceResult, tagHead, tagFoot, (match) =>
		{
			var tag = match.Split(':')[1].Replace(tagFoot, "");
			replaceResult = replaceResult.Replace(match, GetReplaceString.Invoke(match));
		});
		return replaceResult;
	}
	#endregion

	#region -ForEachTag タグ毎に処理
	/// <summary>
	/// タグ毎に処理を行う
	/// </summary>
	/// <param name="target">処理対象</param>
	/// <param name="tagHead">タグ先頭</param>
	/// <param name="tagFoot">タグ終端</param>
	/// <param name="actionForTag">タグを引数に渡されるデリゲート</param>
	private static void ForEachTag(string target, string tagHead, string tagFoot, Action<string> actionForTag)
	{
		foreach (Match mFind in Regex.Matches(target, tagHead + "((?!" + tagFoot + ").)*" + tagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase))
		{
			actionForTag.Invoke(mFind.Value);
		}
	}
	#endregion

	#endregion

	#region プロパティ
	/// <summary>ログNO</summary>
	protected long? LogNo
	{
		get
		{
			long logNo = 0;
			if (long.TryParse(Request[Constants.REQUEST_KEY_MAILSENDLOG_LOG_NO], out logNo))
			{
				return logNo;
			}
			return null;
		}
	}
	/// <summary>メールテンプレート？</summary>
	protected bool IsMailTemplate
	{
		get { return (bool)ViewState["IsMailTemplate"]; }
		set { ViewState["IsMailTemplate"] = value; }
	}
	/// <summary>メール送信ログが存在する？</summary>
	protected bool ExistMailSendLog
	{
		get { return (bool)ViewState["ExistMailSendLog"]; }
		set { ViewState["ExistMailSendLog"] = value; }
	}
	#endregion
}