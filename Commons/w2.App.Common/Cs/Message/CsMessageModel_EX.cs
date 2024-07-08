/*
=========================================================================================================
  Module      : メッセージモデルのパーシャルクラス(CsMessageModel_EX.cs)
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
	/// メッセージモデルのパーシャルクラス
	/// </summary>
	public partial class CsMessageModel : ModelBase<CsMessageModel>
	{
		#region プロパティ
		/// <summary>総件数</summary>
		public int EX_SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>ユーザーID</summary>
		public string EX_UserId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENT_USER_ID]); }
		}
		/// <summary>作成オペレータ名</summary>
		public string EX_OperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["operator_name"]); }
		}
		/// <summary>メッセージステータス名</summary>
		public string EX_MessageStatusName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, this.MessageStatus); }
		}

		/// <summary>回答者オペレータ名</summary>
		public string EX_ReplyOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["reply_operator_name"]); }
		}
		/// <summary>メッセージメール</summary>
		public CsMessageMailModel EX_Mail
		{
			get { return (CsMessageMailModel)this.DataSource["EX_Mail"]; }
			set { this.DataSource["EX_Mail"] = value; }
		}
		/// <summary>メッセージ依頼</summary>
		public CsMessageRequestModel EX_Request
		{
			get { return (CsMessageRequestModel)this.DataSource["EX_Request"]; }
			set { this.DataSource["EX_Request"] = value; }
		}
		/// <summary>メッセージ依頼：依頼番号</summary>
		public int EX_RequestNo
		{
			get { return (int)(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_NO] ?? 0); }
		}
		/// <summary>メッセージ依頼：緊急フラグ</summary>
		public string EX_UrgencyFlg
		{
			get { return  StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_URGENCY_FLG]); }
		}
		/// <summary>依頼者オペレータID</summary>
		public string EX_RequestOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_OPERATOR_ID]); }
		}
		/// <summary>承認者オペレータID</summary>
		public string EX_ApprOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID]); }
		}
		/// <summary>問合せ・回答日（下書きまたは回答日がなければ作成日）</summary>
		public DateTime EX_InquiryReplyChangedDate
		{
			get
			{
				if (this.EX_IsDraft || (this.InquiryReplyDate.HasValue == false))
				{
					return this.DateCreated;
				}
				return this.InquiryReplyDate.Value;
			}
		}
		/// <summary>下書きか</summary>
		public bool EX_IsDraft
		{
			get { return (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT); }
		}
		/// <summary>Date send or receive</summary>
		public DateTime? EX_DateSendOrReceive
		{
			get
			{
				if (this.DataSource["DateSendOrReceive"] == DBNull.Value) return null;
				return (DateTime?)this.DataSource["DateSendOrReceive"];
			}
			set { this.DataSource["DateSendOrReceive"] = value; }
		}
		/// <summary>Reply changed date</summary>
		public DateTime EX_ReplyChangedDate
		{
			get
			{
				return (DateTime)this.DataSource["ReplyChangedDate"];
			}
			set { this.DataSource["ReplyChangedDate"] = value; }
		}
		/// <summary>リクエストか</summary>
		public bool EX_IsRequest
		{
			get { return (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL)
					|| (this.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG); }
		}
		/// <summary>有効か</summary>
		public bool EX_IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_CSMESSAGE_VALID_FLG_VALID); }
		}
		#endregion

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

		#region +GetMessageComment メッセージコメント取得
		/// <summary>
		/// メッセージコメント取得
		/// </summary>
		/// <returns>メッセージコメント</returns>
		public string GetMessageComment()
		{
			return CsCommon.GetMessageIcon(this.MessageStatus, this.MediaKbn, this.DirectionKbn).Key;
		}
		#endregion
	}
}
