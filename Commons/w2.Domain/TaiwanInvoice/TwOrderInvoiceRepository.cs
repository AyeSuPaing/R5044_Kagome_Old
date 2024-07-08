/*
=========================================================================================================
  Module      : 注文電子発票情報リポジトリ (TwOrderInvoiceRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.TwOrderInvoice
{
	/// <summary>
	/// 注文電子発票情報リポジトリ
	/// </summary>
	internal class TwOrderInvoiceRepository : RepositoryBase
	{
		/// <returns>Xml key name</returns>
		private const string XML_KEY_NAME = "TwOrderInvoice";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwOrderInvoiceRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">Accessor</param>
		public TwOrderInvoiceRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TwOrderInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Get Order Invoice
		/// <summary>
		/// Get Order Invoice
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <returns>モデル</returns>
		public TwOrderInvoiceModel GetOrderInvoice(string orderId, int orderShippingNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWORDERINVOICE_ORDER_ID, orderId },
				{ Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO, orderShippingNo }
			};
			var data = Get(XML_KEY_NAME, "GetOrderInvoice", input);
			if (data.Count == 0) return null;

			return new TwOrderInvoiceModel(data[0]);
		}
		#endregion

		#region ~Update Taiwan Order Invoice
		/// <summary>
		/// Update Taiwan Order Invoice
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>result</returns>
		internal int UpdateTwOrderInvoice(TwOrderInvoiceModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateTwOrderInvoice", model.DataSource);

			return result;
		}
		#endregion

		#region ~UpdateTwOrderInvoiceStatus
		/// <summary>
		/// Update Taiwan Order Invoice Status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="invoiceStatus">Invoice Status</param>
		/// <param name="invoiceNo">Invoice No</param>
		/// <returns>result</returns>
		internal int UpdateTwOrderInvoiceStatus(string orderId, int orderShippingNo, string invoiceStatus, string invoiceNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWORDERINVOICE_ORDER_ID, orderId },
				{ Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO, orderShippingNo },
				{ Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS, invoiceStatus },
				{ Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO, invoiceNo }
			};

			var result = Exec(XML_KEY_NAME, "UpdateInvoiceStatus", input);

			return result;
		}
		#endregion

		#region~UpdateInvoiceNoByOrderId
		/// <summary>
		/// Update Invoice No By Order Id
		/// </summary>
		/// <param name="invoiceNo">Invoice no</param>
		/// <param name="orderId">Order id</param>
		public void UpdateInvoiceNoByOrderId(string invoiceNo, string orderId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWORDERINVOICE_ORDER_ID, orderId },
				{ Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO, invoiceNo }
			};

			Exec(XML_KEY_NAME, "UpdateInvoiceNoByOrderId", input);
		}
		#endregion

		#region +DELETE 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public int Delete(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TWORDERINVOICE_ORDER_ID, orderId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
