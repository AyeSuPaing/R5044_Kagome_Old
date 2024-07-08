/*
=========================================================================================================
  Module      : 検索対象カラム(SearchTargetColumn.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace w2.App.Common.Cs.Search
{
	/// <summary>
	/// 検索項目となる対象カラム
	/// </summary>
	class SearchTargetColumn
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="table">対象テーブル名</param>
		/// <param name="column">対象カラム名</param>
		/// <param name="dbType">対象カラム型</param>
		/// <param name="size">対象カラムサイズ</param>
		private SearchTargetColumn(string table, string column, SqlDbType dbType, string size)
		{
			this.TableName = table;
			this.ColumnName = column;
			this.ColumnDbType = dbType;
			this.ColumnSize = size;
		}
		#endregion

		#region プロパティ
		/// <summary>テーブル名</summary>
		public string TableName { get; private set; }
		/// <summary>カラム名</summary>
		public string ColumnName { get; private set; }
		/// <summary>カラム型</summary>
		public SqlDbType ColumnDbType { get; private set; }
		/// <summary>カラムサイズ</summary>
		public string ColumnSize { get; private set; }
		#endregion

		#region 内部クラス
		/// <summary>
		/// 受信メール系カラム定義定数
		/// </summary>
		public class ReceiveMail
		{
			const string CONST_RECEIVE_MAIL = "ReceiveMail";
			public static SearchTargetColumn Body = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_BODY, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn From = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM, SqlDbType.NVarChar, "512");
			public static SearchTargetColumn To = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_TO, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Cc = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_CC, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Bcc = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_BCC, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Subject = new SearchTargetColumn(CONST_RECEIVE_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_SUBJECT, SqlDbType.NVarChar, "100");
		}

		/// <summary>
		/// 送信メール系カラム定義定数
		/// </summary>
		public class SendMail
		{
			const string CONST_SEND_MAIL = "SendMail";
			public static SearchTargetColumn Body = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_BODY, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn From = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM, SqlDbType.NVarChar, "512");
			public static SearchTargetColumn To = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_TO, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Cc = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_CC, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Bcc = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_BCC, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Subject = new SearchTargetColumn(CONST_SEND_MAIL, Constants.FIELD_CSMESSAGEMAIL_MAIL_SUBJECT, SqlDbType.NVarChar, "100");
		}

		/// <summary>
		/// メッセージ系カラム定義定数
		/// </summary>
		public class Message
		{
			public static SearchTargetColumn Name1 = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_NAME1, SqlDbType.NVarChar, "20");
			public static SearchTargetColumn Name2 = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_NAME2, SqlDbType.NVarChar, "20");
			public static SearchTargetColumn NameKana1 = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_NAME_KANA1, SqlDbType.NVarChar, "30");
			public static SearchTargetColumn NameKana2 = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_NAME_KANA2, SqlDbType.NVarChar, "30");
			public static SearchTargetColumn MailAddr = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR, SqlDbType.NVarChar, "256");
			public static SearchTargetColumn Tel1 = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_USER_TEL1, SqlDbType.NVarChar, "16");
			public static SearchTargetColumn InquiryTitle = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_INQUIRY_TITLE, SqlDbType.NVarChar, "50");
			public static SearchTargetColumn InquiryText = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_INQUIRY_TEXT, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn ReplyText = new SearchTargetColumn(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_REPLY_TEXT, SqlDbType.NVarChar, "MAX");
		}

		/// <summary>
		/// インシデント系カラム定義定数
		/// </summary>
		public class Incident
		{
			public static SearchTargetColumn IncidentId = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_INCIDENT_ID, SqlDbType.NVarChar, "30");
			public static SearchTargetColumn IncidentTitle = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_INCIDENT_TITLE, SqlDbType.NVarChar, "50");
			public static SearchTargetColumn VocMemo = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_VOC_MEMO, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn Comment = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_COMMENT, SqlDbType.NVarChar, "MAX");
			public static SearchTargetColumn UserName = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_USER_NAME, SqlDbType.NVarChar, "100");
			public static SearchTargetColumn UserContact = new SearchTargetColumn(Constants.TABLE_CSINCIDENT, Constants.FIELD_CSINCIDENT_USER_CONTACT, SqlDbType.NVarChar, "256");
		}

		/// <summary>
		/// インシデント集計区分値系カラム定義定数
		/// </summary>
		public class IncidentSummaryValue
		{
			public static SearchTargetColumn IncidentId = new SearchTargetColumn(Constants.TABLE_CSINCIDENTSUMMARYVALUE, Constants.FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID, SqlDbType.NVarChar, "30");
			public static SearchTargetColumn SummaryNo = new SearchTargetColumn(Constants.TABLE_CSINCIDENTSUMMARYVALUE, Constants.FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO, SqlDbType.Int, null);
			public static SearchTargetColumn SummaryValue = new SearchTargetColumn(Constants.TABLE_CSINCIDENTSUMMARYVALUE, Constants.FIELD_CSINCIDENTSUMMARYVALUE_VALUE, SqlDbType.NVarChar, "50");
		}
		#endregion
	}
}
