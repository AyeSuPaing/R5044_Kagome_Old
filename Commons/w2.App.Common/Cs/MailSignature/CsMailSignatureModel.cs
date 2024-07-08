/*
=========================================================================================================
  Module      : メール署名モデル(CsMailSignatureModel.cs)
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

namespace w2.App.Common.Cs.MailSignature
{
	/// <summary>
	/// メール署名モデル
	/// </summary>
	[Serializable]
	public partial class CsMailSignatureModel : ModelBase<CsMailSignatureModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMailSignatureModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール署名情報</param>
		public CsMailSignatureModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール署名情報</param>
		public CsMailSignatureModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DEPT_ID] = value; }
		}
		/// <summary>メール署名ID</summary>
		public string MailSignatureId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID] = value; }
		}
		/// <summary>署名タイトル</summary>
		public string SignatureTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TITLE] = value; }
		}
		/// <summary>署名本文</summary>
		public string SignatureText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TEXT] = value; }
		}
		/// <summary>所有オペレータID</summary>
		public string OwnerId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_OWNER_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_OWNER_ID] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILSIGNATURE_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMAILSIGNATURE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
