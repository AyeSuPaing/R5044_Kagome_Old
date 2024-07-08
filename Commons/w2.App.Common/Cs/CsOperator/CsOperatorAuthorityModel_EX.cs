/*
=========================================================================================================
  Module      : CSオペレータ権限モデルのパーシャルクラス(CsOperatorAuthorityModel_EX.cs)
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
	/// CSオペレータ権限モデルのパーシャルクラス
	/// </summary>
	public partial class CsOperatorAuthorityModel : ModelBase<CsOperatorAuthorityModel>
	{
		/// <summary>編集許可フラグ表示テキスト</summary>
		public string EX_PermitEditFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG, this.PermitEditFlg);
			}
		}
		/// <summary>メール直接送信許可フラグ表示テキスト</summary>
		public string EX_PermitMailSendFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG, this.PermitMailSendFlg);
			}
		}
		/// <summary>承認受付許可フラグ表示テキスト</summary>
		public string EX_PermitApprovalFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG, this.PermitApprovalFlg);
			}
		}
		/// <summary>ロック解除許可フラグ表示テキスト</summary>
		public string EX_PermitUnlockFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG, this.PermitUnlockFlg);
			}
		}
		/// <summary>共通署名編集許可フラグ表示テキスト</summary>
		public string EX_PermitEditSignatureFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG, this.PermitEditSignatureFlg);
			}
		}
		/// <summary>担当未設定警告メール受け取りフラグ表示テキスト</summary>
		public string EX_ReceiveNoAssignWarningFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG, this.ReceiveNoAssignWarningFlg);
			}
		}
		/// <summary>完削除許可フラグ表示テキスト</summary>
		public string EX_PermitPermanentDeleteFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSOPERATORAUTHORITY, Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG, this.PermitPermanentDeleteFlg);
			}
		}
	}
}
