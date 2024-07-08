/*
=========================================================================================================
  Module      : ベリトランス請求書サービスインターフェース (IInvoiceVeritransService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.InvoiceVeritrans
{
	/// <summary>
	/// ベリトランス請求書サービスインターフェース
	/// </summary>
	public interface IInvoiceVeritransService : IService
	{
		#region Get
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセッサ</param>
		/// <returns>ベリトランス請求書モデル</returns>
		InvoiceVeritransModel Get(string orderId, SqlAccessor accessor = null);
		#endregion

		#region Insert
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセッサ</param>
		/// <returns>影響を受けた件数</returns>
		int InsertUpdate(InvoiceVeritransModel model, SqlAccessor accessor = null);
		#endregion
	}
}
