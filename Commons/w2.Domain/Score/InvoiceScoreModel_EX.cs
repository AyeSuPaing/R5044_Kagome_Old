/*
=========================================================================================================
  Module      : スコア後払い請求書明細拡張モデル (InvoiceScoreDetailModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Score
{
	/// <summary>
	/// スコア後払い請求書明細拡張モデル
	/// </summary>
	partial class InvoiceScoreModel
	{
		/// <summary>請求書明細</summary>
		public InvoiceScoreDetailModel[] Details { get; set; }
	}
}
