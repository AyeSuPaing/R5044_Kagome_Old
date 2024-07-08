/*
=========================================================================================================
  Module      : 通知メール送信クラス(NotificationMailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using w2.App.Common;
using w2.App.Common.Cs.CsOperator;

/// <summary>
/// 通知メール送信クラス
/// </summary>
public class NotificationMailSender
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	public NotificationMailSender(string shopId)
	{
		this.ShopId = shopId;
		this.SiteRootPath = HttpContext.Current.Request.Url.Scheme + Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Authority + Constants.PATH_ROOT;
	}
	#endregion

	#region #SendNotificationMailForRequest 依頼通知メール送信
	/// <summary>
	/// 依頼通知メール送信
	/// </summary>m_siteRootPath
	/// <param name="incidentId">インシデントID</param>
	/// <param name="mailAddrTo">送信先メールアドレス</param>
	/// <param name="fromOperatorName">依頼元オペレータ名</param>
	/// <param name="mailActionString">メールアクション文字列</param>
	/// <param name="comment">依頼文言</param>
	/// <param name="nextUrl">遷移先URL</param>
	/// <returns>送信結果</returns>
	public bool SendNotificationMailForRequest(string incidentId, string mailAddrTo, string fromOperatorName, string mailActionString, string comment, string nextUrl)
	{
		string url = this.SiteRootPath + Constants.PAGE_W2CS_MANAGER_LOGIN
			+ "?" + Constants.REQUEST_KEY_MANAGER_NEXTURL + "=" + HttpUtility.UrlEncode(nextUrl);

		return SendNotificationMail(
			mailAddrTo,
			mailActionString + "通知 [" + incidentId + "]",
			string.Format("{0}さんからの{1}があります。\r\n\r\n[コメント]\r\n{2}\r\n\r\n[URL]\r\n{3}", fromOperatorName, mailActionString, comment, url)); ;
	}
	#endregion

	#region #SendNotificationMailForIncidentOperatorChanged インシデントオペレータ変更通知メール送信
	/// <summary>
	/// インシデントオペレータ変更通知メール送信
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="operatorId">担当オペレータID</param>
	/// <returns>送信結果（送信しなかった、送信失敗したらfalse）</returns>
	public bool SendNotificationMailForIncidentOperatorChanged(string deptId, string incidentId, string operatorId)
	{
		var csOperatorService = new CsOperatorService(new CsOperatorRepository());
		var ope = csOperatorService.Get(deptId, operatorId);
		if (ope == null) return false;

		string mailActionString = "インシデント担当変更";
		return SendNotificationMail(
			ope.MailAddr,
			mailActionString + "通知 [" + incidentId + "]",
			string.Format("インシデントID「{0}」の担当者があなたに変更されました。", incidentId)); ;
	}
	#endregion

	#region #SendNotificationMail 通知メール送信
	/// <summary>
	/// 通知メール送信
	/// </summary>
	/// <param name="mailAddrTo">送信先メールアドレス</param>
	/// <param name="title">タイトル</param>
	/// <param name="message">メッセージ</param>
	/// <returns>送信結果</returns>
	private bool SendNotificationMail(string mailAddrTo, string title, string message)
	{
		var mailSender = new MailSendUtility(this.ShopId, Constants.CONST_MAIL_ID_CS_NOTIFICATION, "", new Hashtable(), true, Constants.MailSendMethod.Auto);
		mailSender.AddTo(mailAddrTo);
		mailSender.SetSubject(mailSender.Subject.Replace("@@ title @@", title));
		mailSender.SetBody(mailSender.Body.Replace("@@ message @@", message));

		return mailSender.SendMail();
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	private string ShopId { get; set; }
	/// <summary>サイトルートパス</summary>
	private string SiteRootPath { get; set; }
	#endregion
}
