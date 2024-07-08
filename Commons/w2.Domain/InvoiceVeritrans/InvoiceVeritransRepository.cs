/*
=========================================================================================================
  Module      : ベリトランス請求書リポジトリ (InvoiceVeritransRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.InvoiceVeritrans
{
	/// <summary>
	/// ベリトランス請求書リポジトリ
	/// </summary>
	public class InvoiceVeritransRepository : RepositoryBase
	{
		/// <summary>XML key name</summary>
		private const string XML_KEY_NAME = "InvoiceVeritrans";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceVeritransRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public InvoiceVeritransRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region Get
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>ベリトランス請求書モデル</returns>
		public InvoiceVeritransModel Get(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICEVERITRANS_ORDER_ID, orderId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var model = dv.Count > 0 ? new InvoiceVeritransModel(dv[0]) : null;
			return model;
		}
		#endregion

		#region +InsertUpdate
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUpdate(InvoiceVeritransModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUpdate", model.DataSource);
			return result;
		}

		/// <summary>
		/// 請求書明細登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertUpdateDetail(InvoiceVeritransDetailModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUpdateDetail", model.DataSource);
			return result;
		}
		#endregion
	}
}
