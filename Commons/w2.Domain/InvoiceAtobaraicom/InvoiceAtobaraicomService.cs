/*
=========================================================================================================
  Module      : 後払い.com請求書テーブルサービス (InvoiceAtobaraicomService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.InvoiceAtobaraicom
{
	/// <summary>
	/// 後払い.com請求書テーブルサービス
	/// </summary>
	public class InvoiceAtobaraicomService : ServiceBase, IInvoiceAtobaraicomService
	{
		#region +InsertUpdateInvoiceAtobaraicom
		/// <summary>
		/// Insert update invoice atobaraicom
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUpdateInvoiceAtobaraicom(InvoiceAtobaraicomModel model, SqlAccessor accessor)
		{
			using (var repository = new InvoiceAtobaraicomRepository(accessor))
			{
				var result = repository.InsertUpdateInvoiceAtobaraicom(model);
				return result;
			}
		}
		#endregion
	}
}
