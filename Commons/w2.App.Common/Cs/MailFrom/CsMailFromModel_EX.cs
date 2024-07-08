/*
=========================================================================================================
  Module      : メール送信元モデルのパーシャルクラス(CsMailFromModel_EX.cs)
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
	/// メール送信元モデルのパーシャルクラス
	/// </summary>
	public partial class CsMailFromModel : ModelBase<CsMailFromModel>
	{
		#region プロパティ
		/// <summary>総件数</summary>
		public int EX_RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>メールアドレス（表示名 &lt;sample@address.co.jp&gt;）</summary>
		public string EX_DisplayAddress
		{
			get { return w2.Common.Net.Mail.MailAddress.GetMailAddrString(this.DisplayName, this.MailAddress); }
		}
		/// <summary>有効フラグテキスト</summary>
		public string EX_ValidFlgText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSMAILFROM, Constants.FIELD_CSMAILFROM_VALID_FLG, this.ValidFlg); }
		}
		#endregion
	}
}
