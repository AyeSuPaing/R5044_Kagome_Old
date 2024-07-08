/*
=========================================================================================================
  Module      : ユーザーインシデント履歴クラス(UserHistoryIncident.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザーインシデント履歴クラス
	/// </summary>
	public class UserHistoryIncident : UserHistoryBase
	{
		static string KBN_STRING = "インシデント";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="infos">情報</param>
		internal UserHistoryIncident(DataRowView info)
			: base(info)
		{
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			this.KbnString = KBN_STRING;
			this.IncidentId = (string)this.DataSource[Constants.FIELD_CSMESSAGE_INCIDENT_ID];
			this.IncidentTitle = (string)this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_TITLE];

			this.FirstMessageNo = (int)this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_NO];
			this.FirstMessageMediaKbn = (string)this.DataSource[Constants.FIELD_CSMESSAGE_MEDIA_KBN];
			this.FirstMessageDirectionKbn = (string)this.DataSource[Constants.FIELD_CSMESSAGE_DIRECTION_KBN];
			this.FirstMessageDateTime = (DateTime)this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CHANGED];
			if (((string)this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS] != Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT)
				&& ((this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE] != DBNull.Value)))
			{
				this.FirstMessageDateTime = (DateTime)this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE];
			}
			this.FirstMessageTitle = (string)this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TITLE];
			this.FirstMessageStatus = (string)this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS];

			this.DateTime = this.FirstMessageDateTime;
		}

		/// <summary>
		/// 情報セット（最終メッセージ）
		/// </summary>
		/// <param name="infos">情報</param>
		public void SetLastMessageInfo(DataRowView info)
		{
			this.LastMessageNo = (int)info[Constants.FIELD_CSMESSAGE_MESSAGE_NO];
			this.LastMessageMediaKbn = (string)info[Constants.FIELD_CSMESSAGE_MEDIA_KBN];
			this.LastMessageDirectionKbn = (string)info[Constants.FIELD_CSMESSAGE_DIRECTION_KBN];
			this.LastMessageDateTime = (DateTime)info[Constants.FIELD_CSMESSAGE_DATE_CHANGED];
			if (((string)info[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS] != Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT)
				&& ((info[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE] != DBNull.Value)))
			{
				this.LastMessageDateTime = (DateTime)info[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE];
			}
			this.LastMessageTitle = (string)info[Constants.FIELD_CSMESSAGE_INQUIRY_TITLE];
			this.LastMessageStatus = (string)info[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS];
		}

		/// <summary>インシデントID</summary>
		public string IncidentId { get; private set; }
		/// <summary>インシデントタイトル</summary>
		public string IncidentTitle { get; private set; }
		/// <summary>初回メッセージNO</summary>
		public int FirstMessageNo { get; private set; }
		/// <summary>初回メッセージメディア区分</summary>
		public string FirstMessageMediaKbn { get; private set; }
		/// <summary>初回メッセージ受発信区分</summary>
		public string FirstMessageDirectionKbn { get; private set; }
		/// <summary>初回メッセージ問合せ日時</summary>
		public DateTime FirstMessageDateTime { get; private set; }
		/// <summary>初回メッセージタイトル</summary>
		public string FirstMessageTitle { get; private set; }
		/// <summary>初回メッセージステータス</summary>
		public string FirstMessageStatus { get; private set; }
		/// <summary>最終メッセージNO</summary>
		public int LastMessageNo { get; private set; }
		/// <summary>最終メッセージメディア区分</summary>
		public string LastMessageMediaKbn { get; private set; }
		/// <summary>最終メッセージ受発信区分</summary>
		public string LastMessageDirectionKbn { get; private set; }
		/// <summary>最終メッセージ問合せ日時</summary>
		public DateTime LastMessageDateTime { get; private set; }
		/// <summary>最終メッセージタイトル</summary>
		public string LastMessageTitle { get; private set; }
		/// <summary>最終メッセージステータス</summary>
		public string LastMessageStatus { get; private set; }
		/// <summary>初回・最終メッセージが同一か</summary>
		public bool IsSameFirstAndLast
		{
			get { return (this.FirstMessageNo == this.LastMessageNo); }
		}
	}
}
