/*
=========================================================================================================
  Module      : 定期購入電子発票情報サービス (TwFixedPurchaseInvoiceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.TwFixedPurchaseInvoice
{
	/// <summary>
	/// 定期購入電子発票情報サービス
	/// </summary>
	public class TwFixedPurchaseInvoiceService : ServiceBase, ITwFixedPurchaseInvoiceService
	{
		#region +Get Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Get Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseShippingNo">定期購入電子発票枝番</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>モデル</returns>
		public TwFixedPurchaseInvoiceModel GetTaiwanFixedPurchaseInvoice(string fixedPurchaseId, int fixedPurchaseShippingNo, SqlAccessor accessor = null)
		{
			using (var repository = new TwFixedPurchaseInvoiceRepository(accessor))
			{
				var model = repository.GetTaiwanFixedPurchaseInvoice(fixedPurchaseId, fixedPurchaseShippingNo);

				return model;
			}
		}
		#endregion

		#region +Insert Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Insert Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Accessor</param>
		public void InsertTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TwFixedPurchaseInvoiceRepository(accessor))
			{
				repository.InsertTaiwanFixedPurchaseInvoice(model);
			}
		}
		#endregion

		#region +Update Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Update Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model)
		{
			using (var repository = new TwFixedPurchaseInvoiceRepository())
			{
				repository.UpdateTaiwanFixedPurchaseInvoice(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">Sql Accessor</param>
		public bool Delete(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new TwFixedPurchaseInvoiceRepository(accessor))
			{
				var result = repository.Delete(fixedPurchaseId);
				return (result > 0);
			}
		}
		#endregion
	}
}
