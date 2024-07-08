/*
=========================================================================================================
  Module      : CSオペレータ権限モデル(CsOperatorAuthorityModel.cs)
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

namespace w2.App.Common.Cs.CsOperator
{
	/// <summary>
	/// CSオペレータ権限モデル
	/// </summary>
	[Serializable]
	public partial class CsOperatorAuthorityModel : ModelBase<CsOperatorAuthorityModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsOperatorAuthorityModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ権限情報</param>
		public CsOperatorAuthorityModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ権限情報</param>
		public CsOperatorAuthorityModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID] = value; }
		}
		/// <summary>オペレータ権限ID</summary>
		public string OperatorAuthorityId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID] = value; }
		}
		/// <summary>オペレータ権限名</summary>
		public string OperatorAuthorityName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME] = value; }
		}
		/// <summary>編集許可フラグ</summary>
		public string PermitEditFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG] = value; }
		}
		/// <summary>メール直接送信許可フラグ</summary>
		public string PermitMailSendFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG] = value; }
		}
		/// <summary>承認受付許可フラグ</summary>
		public string PermitApprovalFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG] = value; }
		}
		/// <summary>ロック解除許可フラグ</summary>
		public string PermitUnlockFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG] = value; }
		}
		/// <summary>共通署名編集許可フラグ</summary>
		public string PermitEditSignatureFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG] = value; }
		}
		/// <summary>担当未設定警告メール受け取りフラグ</summary>
		public string ReceiveNoAssignWarningFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_LAST_CHANGED] = value; }
		}
		/// <summary>完全削除許可フラグ</summary>
		public string PermitPermanentDeleteFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG]; }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG] = value; }
		}
		#endregion
	}
}
