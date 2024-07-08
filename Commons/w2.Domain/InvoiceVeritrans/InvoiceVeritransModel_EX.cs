/*
=========================================================================================================
  Module      : ベリトランス請求書モデル (InvoiceVeritransModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.InvoiceVeritrans
{
	/// <summary>
	/// ベリトランス請求書モデル
	/// </summary>
	public partial class InvoiceVeritransModel
	{
		#region プロパティ
		/// <summary>ベリトランス請求書明細モデル</summary>
		public InvoiceVeritransDetailModel[] Details { get; set; }
		#endregion
	}
}
