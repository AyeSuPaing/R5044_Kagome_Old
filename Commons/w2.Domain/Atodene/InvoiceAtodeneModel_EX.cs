/*
=========================================================================================================
  Module      : Atodene後払い請求書モデル (InvoiceAtodeneModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Atodene
{
	/// <summary>
	/// Atodene後払い請求書モデル
	/// </summary>
	public partial class InvoiceAtodeneModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>請求書明細</summary>
		public InvoiceAtodeneDetailModel[] Details { get; set; }
		#endregion
	}
}
