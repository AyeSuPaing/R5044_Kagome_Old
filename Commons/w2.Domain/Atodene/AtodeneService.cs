/*
=========================================================================================================
  Module      : Atodene後払い請求書サービス (AtodeneService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.Atodene
{
	/// <summary>
	/// Atodene後払い請求書サービス
	/// </summary>
	public class AtodeneService : ServiceBase
	{
		/// <summary>
		/// 請求書を登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">アクセサ</param>
		public void Insert(InvoiceAtodeneModel model, SqlAccessor accessor)
		{
			using (var repository = new AtodeneRepository(accessor))
			{
				repository.Delete(model.OrderId);
				repository.DeleteDetails(model.OrderId);

				repository.Insert(model);
				foreach (var detail in model.Details)
				{
					repository.InsertDetail(detail);
				}
			}
		}

		/// <summary>
		/// 請求書取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>請求書モデル</returns>
		public InvoiceAtodeneModel Get(string orderId)
		{
			using (var repository = new AtodeneRepository())
			{
				return repository.Get(orderId);
			}
		}
	}
}
