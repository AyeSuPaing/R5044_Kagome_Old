/*
=========================================================================================================
  Module      : インシデント集計区分値モデルのパーシャルクラス(CsIncidentSummaryValueModel_EX.cs)
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

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデント集計区分値モデルのパーシャルクラス
	/// </summary>
	public partial class CsIncidentSummaryValueModel : ModelBase<CsIncidentSummaryValueModel>
	{
		#region プロパティ
		/// <summary>値のテキスト</summary>
		public string EX_Text
		{
			get { return StringUtility.ToEmpty(this.DataSource["EX_Text"]); }
			set { this.DataSource["EX_Text"] = value; }
		}
		#endregion

	}
}
