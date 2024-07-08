/*
=========================================================================================================
  Module      : 定期購入継続分析テーブルモデル (FixedPurchaseRepeatAnalysisModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.FixedPurchaseRepeatAnalysis
{
	/// <summary>
	/// 定期購入継続分析テーブルモデル
	/// </summary>
	public partial class FixedPurchaseRepeatAnalysisModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>離脱か</summary>
		public bool IsFallOut
		{
			get { return (this.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT); }
		}
		#endregion
	}
}
