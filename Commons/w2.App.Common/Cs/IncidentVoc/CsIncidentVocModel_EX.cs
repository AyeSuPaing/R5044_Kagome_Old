/*
=========================================================================================================
  Module      : インシデントVOCモデルのパーシャルクラス(CsIncidentVocModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.IncidentVoc
{
	/// <summary>
	/// インシデントVOCモデルのパーシャルクラス
	/// </summary>
	public partial class CsIncidentVocModel : ModelBase<CsIncidentVocModel>
	{
		/// <summary>
		/// 行数
		/// </summary>
		public int EX_RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
	}
}
