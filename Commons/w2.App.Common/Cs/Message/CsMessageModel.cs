/*
=========================================================================================================
  Module      : メッセージモデル(CsMessageModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージモデル
	/// </summary>
	[Serializable]
	public partial class CsMessageModel : ModelBase<CsMessageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">1メール振分項目情報</param>
		public CsMessageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール振分項目情報</param>
		public CsMessageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_DEPT_ID] = value; }
		}
		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_INCIDENT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_INCIDENT_ID] = value; }
		}
		/// <summary>メッセージ番号</summary>
		public int MessageNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_NO] = value; }
		}
		/// <summary>問合せ媒体</summary>
		public string MediaKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_MEDIA_KBN]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_MEDIA_KBN] = value; }
		}
		/// <summary>受発信区分</summary>
		public string DirectionKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_DIRECTION_KBN]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_DIRECTION_KBN] = value; }
		}
		/// <summary>作成オペレータID</summary>
		public string OperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_OPERATOR_ID] = value; }
		}
		/// <summary>問合せ・回答日時</summary>
		public DateTime? InquiryReplyDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE];
			}
			set { this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE] = value; }
		}
		/// <summary>メッセージステータス</summary>
		public string MessageStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_MESSAGE_STATUS] = value; }
		}
		/// <summary>顧客氏名1</summary>
		public string UserName1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME1]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME1] = value; }
		}
		/// <summary>顧客氏名2</summary>
		public string UserName2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME2]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME2] = value; }
		}
		/// <summary>顧客氏名かな1</summary>
		public string UserNameKana1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME_KANA1]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME_KANA1] = value; }
		}
		/// <summary>顧客氏名かな2</summary>
		public string UserNameKana2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME_KANA2]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_NAME_KANA2] = value; }
		}
		/// <summary>顧客メールアドレス</summary>
		public string UserMailAddr
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR] = value; }
		}
		/// <summary>顧客電話番号1</summary>
		public string UserTel1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_USER_TEL1]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_USER_TEL1] = value; }
		}
		/// <summary>問合せ件名</summary>
		public string InquiryTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TITLE] = value; }
		}
		/// <summary>問合せ内容</summary>
		public string InquiryText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_TEXT] = value; }
		}
		/// <summary>回答内容</summary>
		public string ReplyText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_REPLY_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_REPLY_TEXT] = value; }
		}
		/// <summary>回答オペレータID</summary>
		public string ReplyOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_REPLY_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_REPLY_OPERATOR_ID] = value; }
		}
		/// <summary>受信メール識別ID</summary>
		public string ReceiveMailId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_RECEIVE_MAIL_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_RECEIVE_MAIL_ID] = value; }
		}
		/// <summary>送信メール識別ID</summary>
		public string SendMailId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_SEND_MAIL_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_SEND_MAIL_ID] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGE_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGE_LAST_CHANGED] = value; }
		}
		#endregion

	}
}
