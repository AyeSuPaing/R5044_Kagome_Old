/*
=========================================================================================================
  Module      : メールエラーアドレスマスタモデル (MailErrorAddrModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailErrorAddr
{
	/// <summary>
	/// メールエラーアドレスマスタモデル
	/// </summary>
	[Serializable]
	public partial class MailErrorAddrModel : ModelBase<MailErrorAddrModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailErrorAddrModel()
		{
			this.ErrorPoint = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailErrorAddrModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailErrorAddrModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>対象メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILERRORADDR_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_MAIL_ADDR] = value; }
		}
		/// <summary>エラーポイント</summary>
		public int ErrorPoint
		{
			get { return (int)this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT]; }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_ERROR_POINT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILERRORADDR_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILERRORADDR_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILERRORADDR_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
