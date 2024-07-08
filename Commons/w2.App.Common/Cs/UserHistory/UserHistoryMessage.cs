/*
=========================================================================================================
  Module      : ユーザー履歴（メッセージ）クラス(UserHistoryMessage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.Common.Util;

namespace w2.App.Common.Cs.UserHistory
{
	[Serializable]
	public class UserHistoryMessage : UserHistoryBase
	{
		static string KBN_STRING = "メッセージ";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		internal UserHistoryMessage(DataRowView info)
			: base(info)
		{
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			this.DateTime = this.DateCreated;
			this.KbnString = KBN_STRING;
			this.MessageUrgencyFlg = this.UrgencyFlg;
		}

		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_INCIDENT_ID]); }
		}
		/// <summary>問合せ媒体</summary>
		public string MediaKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_MEDIA_KBN]); }
		}
		/// <summary>受発信区分</summary>
		public string DirectionKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_DIRECTION_KBN]); }
		}
		/// <summary>メッセージステータス</summary>
		public string MessageStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS]); }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CREATED]; }
		}
		/// <summary>緊急フラグ</summary>
		public string UrgencyFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_URGENCY_FLG]); }
		}
		/// <summary>問合せ件名</summary>
		public string InquiryTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TITLE]); }
		}
		/// <summary>作成オペレータ名</summary>
		public string OperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]); }
		}

		#region +GetMessageIcon メッセージアイコン取得
		/// <summary>
		/// メッセージアイコン取得
		/// </summary>
		/// <returns>メッセージアイコン</returns>
		public string GetMessageIcon()
		{
			return CsCommon.GetMessageIcon(this.MessageStatus, this.MediaKbn, this.DirectionKbn).Value;
		}
		#endregion
	}
}