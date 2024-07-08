/*
=========================================================================================================
  Module      : DSK後払い請求書サービス (InvoiceDskDeferredService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.InvoiceDskDeferred
{
	/// <summary>
	/// DSK後払い請求書サービス
	/// </summary>
	public class InvoiceDskDeferredService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <returns>モデル</returns>
		public InvoiceDskDeferredModel Get(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new InvoiceDskDeferredRepository(accessor))
			{
				var model = repository.Get(orderId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(InvoiceDskDeferredModel model, SqlAccessor accessor　= null)
		{
			using (var repository = new InvoiceDskDeferredRepository(accessor))
			{
				repository.Insert(model);
				foreach (var detail in model.Details)
				{
					repository.InsertDetail(detail);
				}
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public void Delete(string orderId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new InvoiceDskDeferredRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				repository.Delete(orderId);
				DeleteDetail(orderId, accessor);

				accessor.CommitTransaction();
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
			using (var repository = new InvoiceDskDeferredRepository(accessor))
			{
				repository.DeleteDetail(orderId);
			}
		}
		#endregion
	}
}
