/*
=========================================================================================================
  Module      : SMS配信文言モデル (GlobalSMSDistTextModel.cs)
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
	/// SMS配信文言モデル
	/// </summary>
	[Serializable]
	public partial class GlobalSMSDistTextModel : ModelBase<GlobalSMSDistTextModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalSMSDistTextModel()
		{
			this.DeptId = "";
			this.MailtextId = "";
			this.PhoneCarrier = "";
			this.SmsText = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSDistTextModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalSMSDistTextModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID] = value; }
		}
		/// <summary>キャリア</summary>
		public string PhoneCarrier
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_PHONE_CARRIER]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_PHONE_CARRIER] = value; }
		}
		/// <summary>SMS本文</summary>
		public string SmsText
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_SMS_TEXT]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_SMS_TEXT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
