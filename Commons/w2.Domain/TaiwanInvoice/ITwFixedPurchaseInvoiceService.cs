/*
=========================================================================================================
  Module      : 定期購入電子発票情報サービスのインタフェース(ITwFixedPurchaseInvoiceService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.TwFixedPurchaseInvoice
{
	/// <summary>
	/// 定期購入電子発票情報サービスのインタフェース
	/// </summary>
	public interface ITwFixedPurchaseInvoiceService : IService
	{
		/// <summary>
		/// Get Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseShippingNo">定期購入電子発票枝番</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>モデル</returns>
		TwFixedPurchaseInvoiceModel GetTaiwanFixedPurchaseInvoice(string fixedPurchaseId, int fixedPurchaseShippingNo, SqlAccessor accessor = null);

		/// <summary>
		/// Insert Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Accessor</param>
		void InsertTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		void UpdateTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">Sql Accessor</param>
		bool Delete(string fixedPurchaseId, SqlAccessor accessor = null);
	}
}