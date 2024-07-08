/*
=========================================================================================================
  Module      : ネクストエンジンAPI ネクストエンジン受注伝票モデルクラス (NextEngineOrderModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine.Response;
using w2.App.Common.OrderExtend;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.Order;

namespace w2.App.Common.NextEngine.Helper
{
	/// <summary>
	/// ネクストエンジン受注モデル
	/// </summary>
	public class NextEngineOrderModel
	{
		/// <summary>
		/// プライベートコンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="orderItem">注文商品モデル</param>
		/// <param name="orderShipping">注文配送モデル</param>
		/// <param name="shippingNo">配送先番号</param>
		/// <param name="isNeGift">ネクストエンジンギフトであるか</param>
		private NextEngineOrderModel(OrderModel order, OrderItemModel orderItem, OrderShippingModel orderShipping, int shippingNo = 1, bool isNeGift = false)
		{
			this.OrderId = CreateNeGiftOrderId(shippingNo, order.OrderId);
			this.OrderDate = order.OrderDate.HasValue
				? order.OrderDate.Value.ToString("yyyy/MM/dd HH:mm")
				: string.Empty;
			this.ShippingZipCode = orderShipping.ShippingZip.Replace("-", "");
			this.OrderZipCode = order.Owner.OwnerZip.Replace("-", "");
			this.ShippingAddress1 = orderShipping.ShippingAddr1 + orderShipping.ShippingAddr2;
			this.ShippingAddress2 = orderShipping.ShippingAddr3 + orderShipping.ShippingAddr4;
			this.OrderAddress1 = order.Owner.OwnerAddr1 + order.Owner.OwnerAddr2;
			this.OrderAddress2 = order.Owner.OwnerAddr3 + order.Owner.OwnerAddr4;
			this.OrderName = order.Owner.OwnerName;
			this.OrderNameKana = order.Owner.OwnerNameKana;
			this.OrderPhoneNumber = order.Owner.OwnerTel1.Replace("-", "");
			this.OrderMail = order.Owner.OwnerMailAddr;
			this.ShippingName = orderShipping.ShippingName;
			this.ShippingNameKana = orderShipping.ShippingNameKana;
			this.ShippingPhoneNumber = orderShipping.ShippingTel1.Replace("-", "");
			this.PaymentType = order.PaymentName;
			this.ShippingType = orderShipping.DeliveryCompanyName;
			var isChildShipping = (shippingNo > 1);
			this.ProductTotalPrice = (isChildShipping ? "0" : order.OrderPriceSubtotal.ToString());
			this.ShippingPrice = (isChildShipping ? "0" : order.OrderPriceShipping.ToString());
			this.TaxPrice = (isChildShipping ? "0" : orderItem.ItemPriceTax.ToString());
			this.PaymentPrice = (isChildShipping ? "0" : order.OrderPriceExchange.ToString());
			this.Point = (isChildShipping ? "0" : order.OrderPointUse.ToString());
			var discountTotal = -(order.FixedPurchaseDiscountPrice
				+ order.FixedPurchaseMemberDiscountAmount
				+ order.MemberRankDiscountPrice
				+ order.OrderCouponUse
				+ order.OrderDiscountSetPrice
				+ order.SetpromotionTotalDiscountAmount);
			this.OtherPrice = (isChildShipping ? "0" : (discountTotal + order.OrderPriceRegulation).ToString());
			this.TotalPrice = (isChildShipping ? "0" : order.OrderPriceTotal.ToString());
			this.FlgGift = (isNeGift ? NextEngineConstants.FLG_UPLOAD_ORDER_DATA_IS_GIFT_VALID : NextEngineConstants.FLG_UPLOAD_ORDER_DATA_IS_GIFT_INVALID);
			this.ShippingTime = orderShipping.DeliveryCompanyShippingTimeMessage;
			this.ShippingDate = orderShipping.ShippingDate.HasValue
				? orderShipping.ShippingDate.Value.ToString("yyyy年MM月dd日")
				: string.Empty;
			this.Remarks = CreateRemarks(order, orderItem);
			this.Memo = string.Empty;
			this.ProductName = orderItem.ProductName;
			this.ProductCode = orderItem.VariationId;
			this.ProductPrice = (isChildShipping ? "0" : orderItem.ProductPrice.ToString());
			this.ProductAmount = orderItem.ItemQuantitySingle.ToString();
			this.ProductOption = orderItem.ProductOptionTexts;
			this.FlgShipping = string.Empty;
			this.CustomerKbn = string.Empty;
			this.CustomerCode = order.UserId;
			this.TaxRate = orderItem.ProductTaxRate.ToString();
		}

		/// <summary>
		/// アップロード用CSV読み込み（読み込み後削除）
		/// </summary>
		/// <returns>CSVデータ文字列</returns>
		public static string GetCsvForUpload()
		{
			var result = string.Empty;

			if (File.Exists(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA) == false) return result;

			using (var sr = new StreamReader(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA))
			{
				var sb = new StringBuilder(string.Empty);
				while (sr.EndOfStream == false)
				{
					sb.AppendLine(sr.ReadLine());
				}
				result = sb.ToString();
			}

			File.Delete(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA);

			return result;
		}

		/// <summary>
		/// 指定パスのCSVファイルに追記
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="orderItem">注文商品モデル</param>
		/// <param name="orderShipping">注文配送モデル</param>
		/// <param name="shippingNo">配送先番号</param>
		/// <param name="isNeGift">ネクストエンジンギフトであるか</param>
		/// <returns></returns>
		public static bool SaveToCsv(
			OrderModel order,
			OrderItemModel orderItem,
			OrderShippingModel orderShipping,
			int shippingNo = 1,
			bool isNeGift = false)
		{
			var csvLine = new NextEngineOrderModel(order, orderItem, orderShipping, shippingNo, isNeGift)
				.CreateCsvLine();

			if (File.Exists(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA) == false)
			{
				using (var sw = new StreamWriter(
					NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA,
					false))
				{
					sw.WriteLine(NextEngineConstants.FIELDS_UPLOAD_ORDER_DATA.JoinToString(","));
				}
			}

			try
			{
				using (var sw = new StreamWriter(
					NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA,
					true))
				{
					sw.WriteLine(csvLine);
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 受注アップロード用CSVデータ文字列（1行）作成
		/// </summary>
		/// <returns>作成したCSV1行分データ</returns>
		private string CreateCsvLine()
		{
			var result = new StringBuilder(string.Format("\"{0}\"", this.OrderId))
				.AppendFormat(",\"{0}\"", this.OrderDate)
				.AppendFormat(",\"{0}\"", this.OrderZipCode)
				.AppendFormat(",\"{0}\"", this.OrderAddress1)
				.AppendFormat(",\"{0}\"", this.OrderAddress2)
				.AppendFormat(",\"{0}\"", this.OrderName)
				.AppendFormat(",\"{0}\"", this.OrderNameKana)
				.AppendFormat(",\"{0}\"", this.OrderPhoneNumber)
				.AppendFormat(",\"{0}\"", this.OrderMail)
				.AppendFormat(",\"{0}\"", this.ShippingZipCode)
				.AppendFormat(",\"{0}\"", this.ShippingAddress1)
				.AppendFormat(",\"{0}\"", this.ShippingAddress2)
				.AppendFormat(",\"{0}\"", this.ShippingName)
				.AppendFormat(",\"{0}\"", this.ShippingNameKana)
				.AppendFormat(",\"{0}\"", this.ShippingPhoneNumber)
				.AppendFormat(",\"{0}\"", this.PaymentType)
				.AppendFormat(",\"{0}\"", this.ShippingType)
				.AppendFormat(",\"{0}\"", this.ProductTotalPrice)
				.AppendFormat(",\"{0}\"", this.TaxPrice)
				.AppendFormat(",\"{0}\"", this.ShippingPrice)
				.AppendFormat(",\"{0}\"", this.PaymentPrice)
				.AppendFormat(",\"{0}\"", this.Point)
				.AppendFormat(",\"{0}\"", this.OtherPrice)
				.AppendFormat(",\"{0}\"", this.TotalPrice)
				.AppendFormat(",\"{0}\"", this.FlgGift)
				.AppendFormat(",\"{0}\"", this.ShippingTime)
				.AppendFormat(",\"{0}\"", this.ShippingDate)
				.AppendFormat(",\"{0}\"", this.Memo)
				.AppendFormat(",\"{0}\"", this.Remarks)
				.AppendFormat(",\"{0}\"", this.ProductName)
				.AppendFormat(",\"{0}\"", this.ProductCode)
				.AppendFormat(",\"{0}\"", this.ProductPrice)
				.AppendFormat(",\"{0}\"", this.ProductAmount)
				.AppendFormat(",\"{0}\"", this.ProductOption)
				.AppendFormat(",\"{0}\"", this.FlgShipping)
				.AppendFormat(",\"{0}\"", this.CustomerKbn)
				.AppendFormat(",\"{0}\"", this.CustomerCode)
				.AppendFormat(",\"{0}\"", this.TaxRate)
				.ToString();
			return result;
		}

		/// <summary>
		/// 備考欄用文字列生成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="orderItem">注文商品情報</param>
		/// <returns>備考欄用文字列</returns>
		private string CreateRemarks(OrderModel order, OrderItemModel orderItem)
		{
			var remark = new StringBuilder();

			var remarksItems = Constants.NE_REMARKS_ENABLED_ITEM.Split(',');
			foreach (var item in remarksItems)
			{
				switch (item)
				{
					case Constants.NEXTENGINE_REMARKS_ADD_ORDER_MEMO:
						remark.AppendLine(order.Memo);
						break;

					case Constants.NEXTENGINE_REMARKS_ADD_ORDER_EXTEND:
						var orderExtend = MailTemplateDataCreaterBase.GetOrderExtendAttributsText(OrderExtendCommon.ConvertOrderExtend(order));
						if (string.IsNullOrEmpty(orderExtend) == false)
						{
							remark.AppendLine(orderExtend);
						}
						break;

					case Constants.NEXTENGINE_REMARKS_ADD_PAYMENT_ORDER_ID:
						if (string.IsNullOrEmpty(order.PaymentOrderId) == false)
						{
							remark.AppendLine(string.Format("【決済注文ID1】{0}【決済注文ID2】", order.PaymentOrderId));
						}
						break;

					case Constants.NEXTENGINE_REMARKS_ADD_CARD_TRAN_ID:
						if (string.IsNullOrEmpty(order.CardTranId) == false)
						{
							remark.AppendLine(string.Format("【決済取引ID1】{0}【決済取引ID2】", order.CardTranId));
						}
						break;

					case Constants.NEXTENGINE_REMARKS_ADD_ORDER_KBN:
						if (string.IsNullOrEmpty(order.OrderKbn) == false)
						{
							remark.AppendLine(string.Format("【注文区分1】{0}【注文区分2】", order.OrderKbn));
						}
						break;

					case Constants.NEXTENGINE_REMARKS_ADD_BRAND_ID:
						if ((string.IsNullOrEmpty(orderItem.BrandId) == false) && Constants.PRODUCT_BRAND_ENABLED)
						{
							remark.AppendLine(string.Format("【ブランドID1】{0}【ブランドID2】", orderItem.BrandId));
						}
						break;
				}
			}

			var result = remark.ToString();

			return result;
		}

		/// <summary>
		/// ネクストエンジン用ギフト連携受注ID作成
		/// </summary>
		/// <param name="orderIds">ネクストエンジン連携対応受注ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>配送先ごとに作成した受注ID</returns>
		public static string[] CreateNeGiftOrderId(string[] orderIds, SqlAccessor accessor)
		{
			var newOrderIds = new List<string>();

			foreach (var orderId in orderIds)
			{
				var shippings = new OrderService().GetShippingAll(orderId, accessor);
				foreach (var shipping in shippings)
				{
					if (shipping.OrderShippingNo != 1) newOrderIds.Add(string.Format("{0}-{1}", orderId, (shipping.OrderShippingNo - 1)));
				}
			}
			newOrderIds.AddRange(orderIds);
			return newOrderIds.ToArray();
		}

		/// <summary>
		/// ネクストエンジン連携、ギフト用受注IDを作成
		/// </summary>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="orderId">受注ID</param>
		/// <returns>ネクストエンジン連携、ギフト用受注ID</returns>
		public static string CreateNeGiftOrderId(int shippingNo, string orderId)
		{
			return (shippingNo == 1)
				? orderId
				: (string.Format("{0}-{1}", orderId, (shippingNo - 1)));
		}

		/// <summary>
		/// アップロード用CSV削除
		/// </summary>
		public static void DeleteCsv()
		{
			if (File.Exists(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA) == false) return;

			try
			{
				File.Delete(NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA);
			}
			catch (Exception ex)
			{
				AppLogger.WriteError("ファイルが削除できませんでした:" + NextEngineConstants.PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA, ex);
			}
		}

		/// <summary>
		/// 更新用注文情報の取得
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="orderItem">注文商品モデル</param>
		/// <param name="orderShipping">注文配送モデル</param>
		/// <returns>更新用注文情報(XML方式)</returns>
		public static string GetOrderInfo(
			OrderModel order, 
			OrderItemModel orderItem,
			OrderShippingModel orderShipping)
		{
			var xml = new NextEngineOrderModel(order, orderItem, orderShipping).CreateUpdateData();

			var writer = new StringWriterUtf8();
			xml.Save(writer);
			var result = writer.ToString();
			writer.Close();

			return result;
		}

		/// <summary>
		/// Get orders for bulk cancel
		/// </summary>
		/// <param name="nextEngineOrders">Next Engine orders</param>
		/// <returns>XML data</returns>
		public static string GetOrdersForBulkCancel(NEOrder[] nextEngineOrders)
		{
			var xml = CreateUpdateDataRequest(nextEngineOrders);
			var writer = new StringWriterUtf8();
			xml.Save(writer);
			var result = writer.ToString();
			writer.Close();

			return result;
		}

		/// <summary>
		/// UTF-8 用 StringWriter です。
		/// </summary>
		private class StringWriterUtf8 : StringWriter
		{
			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}
		}

		/// <summary>
		/// 更新データ作成
		/// </summary>
		/// <returns>更新データ</returns>
		private XmlDocument CreateUpdateData()
		{
			var orderInfo = new XElement(
				new XElement(
					"root",
					new XElement(
						"receiveorder_base",
						new XElement(
							"receive_order_cancel_type_id",
							NextEngineConstants.FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_DUE_TO_CIRCUMSTANCES))));

			return XmlExtensions.ToXmlDocument(new XDocument(orderInfo));
		}

		/// <summary>
		/// Create update data request
		/// </summary>
		/// <param name="nextEngineOrders">Next engine orders</param>
		/// <returns>Xml data request</returns>
		private static XmlDocument CreateUpdateDataRequest(NEOrder[] nextEngineOrders)
		{
			var rootElememt = new XElement("root");
			foreach(var nextEngineOrder in nextEngineOrders)
			{
				var childElement = new XElement(
					NextEngineConstants.PARAM_SEARCH_XML_ELEMENT_RECEIVEORDER,
					new XAttribute(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_ID, nextEngineOrder.NEOrderId),
					new XAttribute(NextEngineConstants.PARAM_SEARCH_ORDER_ORDER_LAST_MODIFIED_DATE, nextEngineOrder.LastModifiedDate),
					new XElement(NextEngineConstants.PARAM_SEARCH_XML_ELEMENT_RECEIVEORDER_BASE,
						new XElement(
							NextEngineConstants.PARAM_SEARCH_ORDER_CANCEL_TYPE_ID,
							NextEngineConstants.FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_DUE_TO_CIRCUMSTANCES)));
				rootElememt.Add(childElement);
			}
			var orderInfo = new XElement(rootElememt);

			return XmlExtensions.ToXmlDocument(new XDocument(orderInfo));
		}

		/// <summary>
		/// Generate import data as CSV format for Next Engine order
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="isSuccess">Is success</param>
		/// <returns>Import data as CSV format for Next Engine order</returns>
		public static string GenerateImportDataAsCsvForNextEngineOrder(string orderId, out bool isSuccess)
		{
			isSuccess = false;
			try
			{
				var order = DomainFacade.Instance.OrderService.GetOrderInfoByOrderId(orderId);
				if (order == null) return string.Empty;

				var importData = new StringBuilder()
					.AppendLine(NextEngineConstants.FIELDS_UPLOAD_ORDER_DATA.JoinToString(","));
				if (Constants.GIFTORDER_OPTION_ENABLED
					&& Constants.NE_OPTION_ENABLED
					&& (order.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON))
				{
					foreach (var shipping in order.Shippings)
					{
						shipping.DeliveryCompanyShippingTimeMessage = DomainFacade.Instance.DeliveryCompanyService
							.Get(shipping.DeliveryCompanyId)
							.GetShippingTimeMessage(shipping.ShippingTime);

						foreach (var orderItem in shipping.Items)
						{
							var dataLine = new NextEngineOrderModel(order, orderItem, shipping, shipping.OrderShippingNo, true)
								.CreateCsvLine();
							importData.AppendLine(new NextEngineOrderModel(
								order,
								orderItem,
								shipping,
								shipping.OrderShippingNo,
								true)
								.CreateCsvLine());
						}
					}
				}

				order.Shippings[0].DeliveryCompanyShippingTimeMessage = DomainFacade.Instance.DeliveryCompanyService
					.Get(order.Shippings[0].DeliveryCompanyId)
					.GetShippingTimeMessage(order.Shippings[0].ShippingTime);
				foreach (var orderItem in order.Items)
				{
					var dataLine = new NextEngineOrderModel(order, orderItem, order.Shippings[0])
						.CreateCsvLine();
					importData.AppendLine(dataLine);
				}

				isSuccess = true;
				return importData.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>店舗伝票番号</summary>
		private string OrderId { get; set; }
		/// <summary>受注日</summary>
		private string OrderDate { get; set; }
		/// <summary>受注郵便番号</summary>
		private string OrderZipCode { get; set; }
		/// <summary>受注住所１</summary>
		private string OrderAddress1 { get; set; }
		/// <summary>受注住所2</summary>
		private string OrderAddress2 { get; set; }
		/// <summary>受注名</summary>
		private string OrderName { get; set; }
		/// <summary>受注名カナ</summary>
		private string OrderNameKana { get; set; }
		/// <summary>受注電話番号</summary>
		private string OrderPhoneNumber { get; set; }
		/// <summary>受注メールアドレス</summary>
		private string OrderMail { get; set; }
		/// <summary>発送郵便番号</summary>
		private string ShippingZipCode { get; set; }
		/// <summary>発送先住所１</summary>
		private string ShippingAddress1 { get; set; }
		/// <summary>発送先住所2</summary>
		private string ShippingAddress2 { get; set; }
		/// <summary>発送先名</summary>
		private string ShippingName { get; set; }
		/// <summary>発送先名カナ</summary>
		private string ShippingNameKana { get; set; }
		/// <summary>発送電話番号</summary>
		private string ShippingPhoneNumber { get; set; }
		/// <summary>支払方法</summary>
		private string PaymentType { get; set; }
		/// <summary>発送方法</summary>
		private string ShippingType { get; set; }
		/// <summary>商品計</summary>
		private string ProductTotalPrice { get; set; }
		/// <summary>発送料</summary>
		private string ShippingPrice { get; set; }
		/// <summary>税金</summary>
		private string TaxPrice { get; set; }
		/// <summary>手数料</summary>
		private string PaymentPrice { get; set; }
		/// <summary>ポイント</summary>
		private string Point { get; set; }
		/// <summary>その他費用</summary>
		private string OtherPrice { get; set; }
		/// <summary>合計金額</summary>
		private string TotalPrice { get; set; }
		/// <summary>ギフトフラグ</summary>
		private string FlgGift { get; set; }
		/// <summary>時間指定</summary>
		private string ShippingTime { get; set; }
		/// <summary>日付指定</summary>
		private string ShippingDate { get; set; }
		/// <summary>備考</summary>
		private string Remarks { get; set; }
		/// <summary>作業者欄</summary>
		private string Memo { get; set; }
		/// <summary>商品名</summary>
		private string ProductName { get; set; }
		/// <summary>商品コード</summary>
		private string ProductCode { get; set; }
		/// <summary>商品価格</summary>
		private string ProductPrice { get; set; }
		/// <summary>受注数量</summary>
		private string ProductAmount { get; set; }
		/// <summary>商品オプション</summary>
		private string ProductOption { get; set; }
		/// <summary>出荷済フラグ</summary>
		private string FlgShipping { get; set; }
		/// <summary>顧客区分</summary>
		private string CustomerKbn { get; set; }
		/// <summary>顧客コード</summary>
		private string CustomerCode { get; set; }
		/// <summary>消費税率（%）</summary>
		private string TaxRate { get; set; }
	}
}
