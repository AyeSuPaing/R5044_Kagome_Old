/*
=========================================================================================================
  Module      : スコア後払い請求書サービス (InvoiceScoreService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.Score
{
	/// <summary>
	/// スコア後払い請求書サービス
	/// </summary>
	public class InvoiceScoreService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <returns>モデル</returns>
		public InvoiceScoreModel Get(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceScoreRepository(accessor))
			{
				var model = repository.Get(orderId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 削除してから登録する
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(InvoiceScoreModel model, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceScoreRepository(accessor))
			{
				repository.Accessor.OpenConnection();
				repository.Accessor.BeginTransaction();

				// 登録する前に削除する
				repository.Delete(model.OrderId);
				repository.DeleteDetail(model.OrderId);

				repository.Insert(model);
				foreach (var detail in model.Details)
				{
					repository.InsertDetail(detail);
				}

				repository.Accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		public void Delete(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceScoreRepository(accessor))
			{
				repository.Delete(orderId);
				DeleteDetail(orderId, accessor);
			}
		}
		#endregion

		#region +DeleteDetail 請求書明細削除
		/// <summary>
		/// 請求書明細削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		private void DeleteDetail(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceScoreRepository(accessor))
			{
				repository.DeleteDetail(orderId);
			}
		}
		#endregion
	}
}
