/*
=========================================================================================================
  Module      : メール署名モデルのパーシャルクラス(CsMailSignatureModel_EX.cs)
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
	/// メール署名モデルのパーシャルクラス
	/// </summary>
	public partial class CsMailSignatureModel : ModelBase<CsMailSignatureModel>
	{
		#region プロパティ
		/// <summary>総件数</summary>
		public int EX_RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>有効フラグ文字列</summary>
		public string EX_ValidFlgText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSMAILSIGNATURE, Constants.FIELD_CSMAILSIGNATURE_VALID_FLG, this.ValidFlg); }
		}
		#endregion
	}
}
