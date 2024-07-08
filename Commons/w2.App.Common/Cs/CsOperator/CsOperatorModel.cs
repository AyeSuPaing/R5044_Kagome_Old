/*
=========================================================================================================
  Module      : CSオペレータモデル(CsOperatorModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.CsOperator
{
	/// <summary>
	/// CSオペレータモデル
	/// </summary>
	[Serializable]
	public partial class CsOperatorModel : w2.Domain.ModelBase<CsOperatorModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsOperatorModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ情報</param>
		public CsOperatorModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ情報</param>
		public CsOperatorModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_DEPT_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_ID] = value; }
		}
		/// <summary>オペレータ権限ID</summary>
		public string OperatorAuthorityId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID] = value; }
		}
		/// <summary>メール送信元ID</summary>
		public string MailFromId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_FROM_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_FROM_ID] = value; }
		}
		/// <summary>情報メール通知フラグ</summary>
		public string NotifyInfoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_INFO_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_INFO_FLG] = value; }
		}
		/// <summary>警告メール通知フラグ</summary>
		public string NotifyWarnFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_WARN_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_WARN_FLG] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_ADDR]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_ADDR] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSOPERATOR_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSOPERATOR_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSOPERATOR_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
