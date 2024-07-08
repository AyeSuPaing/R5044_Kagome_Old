/*
=========================================================================================================
  Module      : メール配信　配信先テンポラリテーブルモデル (MailSendTempModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailSendTemp
{
	/// <summary>
	/// メール配信　配信先テンポラリテーブルモデル
	/// </summary>
	[Serializable]
	public partial class MailSendTempModel : ModelBase<MailSendTempModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailSendTempModel()
		{
			this.DeptId = "";
			this.MasterId = "";
			this.UserId = "";
			this.MailAddr = "";
			this.MailAddrKbn = "";
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendTempModel(DataRowView source) : this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendTempModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEMP_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_DEPT_ID] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEMP_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_MASTER_ID] = value; }
		}
		/// <summary>枝番</summary>
		public long DataNo
		{
			get { return (long)this.DataSource[Constants.FIELD_MAILSENDTEMP_DATA_NO]; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEMP_USER_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_USER_ID] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEMP_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_MAIL_ADDR] = value; }
		}
		/// <summary>メールアドレス区分</summary>
		public string MailAddrKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEMP_MAIL_ADDR_KBN]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_MAIL_ADDR_KBN] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILSENDTEMP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEMP_DATE_CREATED] = value; }
		}
		#endregion
	}
}