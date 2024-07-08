/*
=========================================================================================================
  Module      : 後払い.com請求書テーブルリポジトリ (InvoiceAtobaraicomRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.InvoiceAtobaraicom
{
	/// <summary>
	/// 後払い.com請求書テーブルリポジトリ
	/// </summary>
	public class InvoiceAtobaraicomRepository : RepositoryBase
	{
		/// <summary>XML key name</summary>
		private const string XML_KEY_NAME = "InvoiceAtobaraicom";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceAtobaraicomRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public InvoiceAtobaraicomRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +InsertUpdateInvoiceAtobaraicom
		/// <summary>
		/// Insert update invoice atobaraicom
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUpdateInvoiceAtobaraicom(InvoiceAtobaraicomModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUpdateInvoiceAtobaraicom", model.DataSource);
			return result;
		}
		#endregion
	}
}
