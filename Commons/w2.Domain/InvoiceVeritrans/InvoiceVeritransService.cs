/*
=========================================================================================================
  Module      : ベリトランス請求書サービス (InvoiceVeritransService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.InvoiceVeritrans
{
	/// <summary>
	/// ベリトランス請求書サービス
	/// </summary>
	public class InvoiceVeritransService : ServiceBase, IInvoiceVeritransService
	{
		#region Get
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセッサ</param>
		/// <returns>ベリトランス請求書モデル</returns>
		public InvoiceVeritransModel Get(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceVeritransRepository(accessor))
			{
				var model = repository.Get(orderId);
				return model;
			}
		}
		#endregion

		#region InsertUpdate
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセッサ</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUpdate(InvoiceVeritransModel model, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceVeritransRepository(accessor))
			{
				var result = repository.InsertUpdate(model);
				foreach (var detail in model.Details)
				{
					repository.InsertUpdateDetail(detail);
				}
				return result;
			}
		}
		#endregion
	}
}
