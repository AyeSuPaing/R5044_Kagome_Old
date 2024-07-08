/*
=========================================================================================================
  Module      : DSK後払い請求書モデル (InvoiceDskDeferredModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.InvoiceDskDeferredDetail;

namespace w2.Domain.InvoiceDskDeferred
{
	/// <summary>
	/// DSK後払い請求書モデル
	/// </summary>
	public partial class InvoiceDskDeferredModel
	{
		#region プロパティ
		/// <summary>請求書明細</summary>
		public InvoiceDskDeferredDetailModel[] Details { get; set; }
		#endregion
	}
}
