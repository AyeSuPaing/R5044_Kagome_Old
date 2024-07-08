/*
=========================================================================================================
  Module      : メールエラーアドレスモデル(CsMailErrorAddrModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.ErrorPoint
{
	[Serializable]
	public partial class CsMailErrorAddrModel : ModelBase<CsMailErrorAddrModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMailErrorAddrModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">エラーポイント情報</param>
		public CsMailErrorAddrModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">エラーポイント情報</param>
		public CsMailErrorAddrModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>対象メールアドレス</summary>
		public string MailAddr
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_MAILERRORADDR_MAIL_ADDR]); }
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
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_MAILERRORADDR_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_MAILERRORADDR_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
