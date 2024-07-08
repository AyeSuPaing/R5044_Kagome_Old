/*
=========================================================================================================
  Module      : インシデントマスタモデル (CsIncidentModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.CsIncident
{
	/// <summary>
	/// インシデントマスタモデル
	/// </summary>
	public partial class CsIncidentModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>総件数</summary>
		public int EX_SearchCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["row_count"], 0); }
		}
		#endregion
	}
}
