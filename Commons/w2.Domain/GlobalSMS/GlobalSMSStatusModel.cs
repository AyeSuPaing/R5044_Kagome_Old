/*
=========================================================================================================
  Module      : SMSステータスモデル (GlobalSMSStatusModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalSMS
{
	/// <summary>
	/// SMSステータスモデル
	/// </summary>
	[Serializable]
	public partial class GlobalSMSStatusModel : ModelBase<GlobalSMSStatusModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalSMSStatusModel()
		{
			this.MessageId = "";
			this.GlobalTelNo = "";
			this.SmsStatus = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSStatusModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSStatusModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>メッセージID</summary>
		public string MessageId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_MESSAGE_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_MESSAGE_ID] = value; }
		}
		/// <summary>グローバル電話番号</summary>
		public string GlobalTelNo
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_GLOBAL_TEL_NO]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_GLOBAL_TEL_NO] = value; }
		}
		/// <summary>SMSステータス</summary>
		public string SmsStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_SMS_STATUS]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_SMS_STATUS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSSTATUS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
