/*
=========================================================================================================
  Module      : 定期購入電子発票情報リポジトリ (TwFixedPurchaseInvoiceRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.TwFixedPurchaseInvoice
{
	/// <summary>
	/// 定期購入電子発票情報リポジトリ
	/// </summary>
	internal class TwFixedPurchaseInvoiceRepository : RepositoryBase
	{
		/// <returns>Xml Key Name</returns>
		private const string XML_KEY_NAME = "TwFixedPurchaseInvoice";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwFixedPurchaseInvoiceRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TwFixedPurchaseInvoiceRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Get Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseShippingNo">定期購入電子発票枝番</param>
		/// <returns>モデル</returns>
		public TwFixedPurchaseInvoiceModel GetTaiwanFixedPurchaseInvoice(string fixedPurchaseId, int fixedPurchaseShippingNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID, fixedPurchaseId },
				{ Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO, fixedPurchaseShippingNo }
			};
			var data = Get(XML_KEY_NAME, "GetTaiwanFixedPurchaseInvoice", input);
			if (data.Count == 0) return null;

			return new TwFixedPurchaseInvoiceModel(data[0]);
		}
		#endregion

		#region +Insert Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Insert Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "InsertTaiwanFixedPurchaseInvoice", model.DataSource);
		}
		#endregion

		#region +Update Taiwan Fixed Purchase Invoice
		/// <summary>
		/// Update Taiwan Fixed Purchase Invoice
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateTaiwanFixedPurchaseInvoice(TwFixedPurchaseInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "UpdateTaiwanFixedPurchaseInvoice", model.DataSource);
		}
		#endregion

		#region +DELETE 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		public int Delete(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID, fixedPurchaseId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
