/*
=========================================================================================================
  Module      : CSオペレータモデルのパーシャルクラス(CsOperatorModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.CsOperator
{
	/// <summary>
	/// CSオペレータモデルのパーシャルクラス
	/// </summary>
	public partial class CsOperatorModel
	{
		#region w2_ShopOperator
		/// <summary>ショップオペレータ名</summary>
		public string EX_ShopOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]); }
		}
		/// <summary>メニュー権限3</summary>
		public string EX_MenuAccessLevel3
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3]); }
		}
		/// <summary>メニュー権限名</summary>
		public string EX_MenuAuthorityName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME]); }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string EX_ValidFlag
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG]); }
		}
		/// <summary>有効フラグ表示名</summary>
		public string EX_ValidFlagName
		{
			get { return ValueText.GetValueText(Constants.TABLE_SHOPOPERATOR, Constants.FIELD_SHOPOPERATOR_VALID_FLG, this.EX_ValidFlag); }
		}
		#endregion

		#region w2_CsOperator
		/// <summary>情報メール通知フラグ</summary>
		public bool EX_NotifyInfoFlg
		{
			get { return (this.NotifyInfoFlg == Constants.FLG_CSOPERATOR_NOTIFY_INFO_FLG_VALID); }
		}
		/// <summary>警告メール通知フラグ</summary>
		public bool EX_NotifyWarnFlg
		{
			get { return (this.NotifyWarnFlg == Constants.FLG_CSOPERATOR_NOTIFY_WARN_FLG_VALID); }
		}
		/// <summary>メール送信元表示名</summary>
		public string EX_MailFromDisplayName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER]) == "" ? "－" : StringUtility.ToEmpty(this.DataSource["MailFromDisplayName"]); }
			set { this.DataSource["MailFromDisplayName"] = value; }
		}
		/// <summary>情報メール通知フラグ表示名</summary>
		public string EX_NotifyInfoFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATOR, Constants.FIELD_CSOPERATOR_NOTIFY_INFO_FLG,
					(this.NotifyInfoFlg == "") ? Constants.FLG_CSOPERATOR_NOTIFY_INFO_FLG_INVALID : this.NotifyInfoFlg);
			}
		}
		/// <summary>警告メール通知フラグ表示名</summary>
		public string EX_NotifyWarnFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATOR, Constants.FIELD_CSOPERATOR_NOTIFY_WARN_FLG,
					(this.NotifyWarnFlg == "") ? Constants.FLG_CSOPERATOR_NOTIFY_WARN_FLG_INVALID : this.NotifyWarnFlg);
			}
		}
		/// <summary>メールアドレス表示文字列</summary>
		public string EX_MailAddrText
		{
			get { return this.MailAddr.ConvertIfNullEmpty("－"); }
		}
		/// <summary>表示順表示文字列</summary>
		public string EX_DisplayOrderText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER]).ConvertIfNullEmpty("－"); }
		}
		#endregion

		#region w2_CsOperatorAuthority
		/// <summary>オペレータ権限名</summary>
		public string EX_OperatorAuthorityName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER]) == "" ? "－" : StringUtility.ToNull(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME] = value; }
		}
		/// <summary>編集許可フラグ</summary>
		public bool EX_PermitEditFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_VALID); }
		}
		/// <summary>メール直接送信許可フラグ</summary>
		public bool EX_PermitMailSendFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_VALID); }
		}
		/// <summary>承認受付許可フラグ</summary>
		public bool EX_PermitApprovalFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_VALID); }
		}
		/// <summary>ロック解除許可フラグ</summary>
		public bool EX_PermitUnlockFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_VALID); }
		}
		/// <summary>共通署名編集許可フラグ</summary>
		public bool EX_PermitEditSignatureFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG_VALID); }
		}
		/// <summary>担当未設定警告メール受け取りフラグ</summary>
		public bool EX_ReceiveNoAssignWarningFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_VALID); }
		}
		/// <summary>完削除許可フラグ表示フラグ</summary>
		public bool EX_PermitPermanentDeleteFlg
		{
			get { return (StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG]) == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_VALID); }
		}
		#endregion
	}
}
