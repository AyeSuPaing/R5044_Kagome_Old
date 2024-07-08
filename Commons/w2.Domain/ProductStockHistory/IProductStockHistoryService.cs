/*
=========================================================================================================
  Module      : 商品在庫履歴サービスのインターフェース (IProductStockHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductStockHistory
{
	/// <summary>
	/// 商品在庫履歴サービスのインターフェース
	/// </summary>
	public interface IProductStockHistoryService : IService
	{
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Insert(ProductStockHistoryModel model, SqlAccessor accessor = null);
	}
}
