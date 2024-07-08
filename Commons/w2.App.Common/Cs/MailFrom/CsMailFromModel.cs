/*
=========================================================================================================
  Module      : メール送信元モデル(CsMailFromModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.MailFrom
{
	/// <summary>
	/// メール送信元モデル
	/// </summary>
	[Serializable]
	public partial class CsMailFromModel : ModelBase<CsMailFromModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMailFromModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール送信元情報</param>
		public CsMailFromModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール送信元情報</param>
		public CsMailFromModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DEPT_ID] = value; }
		}
		/// <summary>送信元ID</summary>
		public string MailFromId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_MAIL_FROM_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_MAIL_FROM_ID] = value; }
		}
		/// <summary>送信元アドレス</summary>
		public string MailAddress
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_MAIL_ADDRESS]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_MAIL_ADDRESS] = value; }
		}
		/// <summary>送信元表示名</summary>
		public string DisplayName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_DISPLAY_NAME]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DISPLAY_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMAILFROM_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILFROM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILFROM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILFROM_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMAILFROM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
