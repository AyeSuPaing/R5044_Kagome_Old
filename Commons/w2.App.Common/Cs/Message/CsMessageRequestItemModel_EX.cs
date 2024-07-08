/*
=========================================================================================================
  Module      : メッセージ依頼アイテムモデルのパーシャルクラス(CsMessageRequestItemModel_EX.cs)
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
	/// メッセージ依頼アイテムモデルのパーシャルクラス
	/// </summary>
	public partial class CsMessageRequestItemModel : ModelBase<CsMessageRequestItemModel>
	{
		#region プロパティ
		/// <summary>承認者オペレータ名</summary>
		public string EX_ApprOperatorName
		{
			get { return StringUtility.ToEmpty(this.DataSource["appr_operator_name"]); }
			set { this.DataSource["appr_operator_name"] = value; }
		}
		/// <summary>結果ステータス文字列（依頼ステータスによって内容が変わるためプロパティを作成）</summary>
		public string EX_ResultStatusString
		{
			get { return StringUtility.ToEmpty(this.DataSource["EX_ResultStatusString"]); }
			set { this.DataSource["EX_ResultStatusString"] = value; }
		}
		#endregion
	}
}
