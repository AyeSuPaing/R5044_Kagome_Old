/*
=========================================================================================================
  Module      : メッセージ依頼モデルのパーシャルクラス(CsMessageRequestModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージ依頼モデルのパーシャルクラス
	/// </summary>
	public partial class CsMessageRequestModel : ModelBase<CsMessageRequestModel>
	{
		#region プロパティ
		/// <summary>承認方法名</summary>
		public string EX_ApprovalTypeName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSMESSAGEREQUEST, Constants.FIELD_CSMESSAGEREQUEST_APPROVAL_TYPE, this.ApprovalType); }
		}
		/// <summary>メッセージ依頼アイテム</summary>
		public CsMessageRequestItemModel[] EX_Items
		{
			get { return (CsMessageRequestItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>緊急度テキスト</summary>
		public string UrgencyFlgText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSMESSAGEREQUEST, Constants.FIELD_CSMESSAGEREQUEST_URGENCY_FLG, this.UrgencyFlg); }
		}
		/// <summary>依頼者オペレータ名</summary>
		public string EX_RequestOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["request_operator_name"]); }
		}
		/// <summary>作業中オペレータ名</summary>
		public string EX_WorkingOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["working_operator_name"]); }
			set { this.DataSource["working_operator_name"] = value; }
		}
		#endregion
	}
}
