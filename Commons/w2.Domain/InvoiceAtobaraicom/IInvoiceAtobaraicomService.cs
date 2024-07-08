/*
=========================================================================================================
  Module      : Interface Invoice Atobaraicom Service (IInvoiceAtobaraicomService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.InvoiceAtobaraicom
{
	/// <summary>
	/// IInvoiceAtobaraicomService
	/// </summary>
	public interface IInvoiceAtobaraicomService : IService
	{
		/// <summary>
		/// Insert update invoice atobaraicom
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <returns>影響を受けた件数</returns>
		int InsertUpdateInvoiceAtobaraicom(InvoiceAtobaraicomModel model, SqlAccessor accessor = null);
	}
}
