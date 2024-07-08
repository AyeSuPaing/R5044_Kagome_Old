/*
=========================================================================================================
  Module      : CS共通処理クラス(CsCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Cs
{
	public class CsCommon
	{
		/// <summary>
		/// アイコン区分
		/// </summary>
		private enum IconKbn
		{
			// <summary>送信/承認依頼済み</summary>
			MailReq,
			// <summary>承認済み</summary>
			MailResOk,
			// <summary>送信/承認差し戻し</summary>
			MailResNg,
			// <summary>送信/承認依頼取り下げ</summary>
			MailReqCan,
			// <summary>メール下書き</summary>
			MailDraft,
			// <summary>電話問い合わせ下書き</summary>
			TelDraft,
			// <summary>メール送信済み</summary>
			MailOut,
			// <summary>電話問い合わせ送信済み</summary>
			TelOut,
			// <summary>メール受信済み</summary>
			MailIn,
			// <summary>電話問い合わせ受信済み</summary>
			TelIn,
			// <summary>メール</summary>
			Mail,
			// <summary>電話問い合わせ</summary>
			Tel,
		}

		/// <summary>
		/// アイコン情報
		/// </summary>
		private static Dictionary<IconKbn, KeyValuePair<string, string>> ICON_INFO = new Dictionary<IconKbn, KeyValuePair<string, string>>
		{
			{IconKbn.MailReq, new KeyValuePair<string, string>("送信/承認依頼済み", "icon_mail_req.png")},
			{IconKbn.MailResOk, new KeyValuePair<string, string>("承認済み", "icon_mail_res_ok.png")},
			{IconKbn.MailResNg, new KeyValuePair<string, string>("送信/承認差し戻し", "icon_mail_res_ng.png")},
			{IconKbn.MailReqCan, new KeyValuePair<string, string>("送信/承認依頼取り下げ", "icon_mail_req_can.png")},
			{IconKbn.MailDraft, new KeyValuePair<string, string>("メール下書き", "icon_mail_draft.png")},
			{IconKbn.TelDraft, new KeyValuePair<string, string>("電話問い合わせ下書き", "icon_tel_draft.png")},
			{IconKbn.MailOut, new KeyValuePair<string, string>("メール送信済み", "icon_mail_out.png")},
			{IconKbn.TelOut, new KeyValuePair<string, string>("電話問い合わせ送信済み", "icon_tel_out.png")},
			{IconKbn.MailIn, new KeyValuePair<string, string>("メール受信済み", "icon_mail_in.png")},
			{IconKbn.TelIn, new KeyValuePair<string, string>("電話問い合わせ受信済み", "icon_tel_in.png")},
			{IconKbn.Mail, new KeyValuePair<string, string>("メール" ,"icon_mail.png")},
			{IconKbn.Tel, new KeyValuePair<string, string>("電話問い合わせ", "icon_tel.png")},
		};

		#region +GetMessageIcon メッセージアイコン情報取得
		/// <summary>
		/// メッセージアイコン情報取得
		/// </summary>
		/// <param name="messageStatus">メッセージテータス</param>
		/// <param name="mediaKbn">メディア区分</param>
		/// <param name="directionKbn">受発信区分</param>
		/// <returns>メッセージアイコン情報</returns>
		public static KeyValuePair<string, string> GetMessageIcon(string messageStatus, string mediaKbn, string directionKbn)
		{
			bool isMail = (mediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);

			switch (messageStatus)
			{
				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ:
				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ:
					return ICON_INFO[IconKbn.MailReq];

				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK:
					return ICON_INFO[IconKbn.MailResOk];

				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG:
				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG:
					return ICON_INFO[IconKbn.MailResNg];

				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL:
				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL:
					return ICON_INFO[IconKbn.MailReqCan];

				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT:
					return isMail ? ICON_INFO[IconKbn.MailDraft] : ICON_INFO[IconKbn.TelDraft];

				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE:
				case Constants.FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED:
					switch (directionKbn)
					{
						case Constants.FLG_CSMESSAGE_DIRECTION_KBN_SEND:
							return isMail ? ICON_INFO[IconKbn.MailOut] : ICON_INFO[IconKbn.TelOut];

						case Constants.FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE:
							return isMail ? ICON_INFO[IconKbn.MailIn] : ICON_INFO[IconKbn.TelIn];
					}
					break;

				default:
					return isMail ? ICON_INFO[IconKbn.Mail] : ICON_INFO[IconKbn.Tel];
			}
			return new KeyValuePair<string, string>();
		}
		#endregion
	}
}
