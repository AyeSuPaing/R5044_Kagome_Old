/*
=========================================================================================================
  Module      : DSK後払い注文情報登録モデルアダプタ(DskDeferredOrderRegisterModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderRegister
{
	/// <summary>
	/// DSK後払い注文情報登録モデルアダプタ
	/// </summary>
	public class DskDeferredOrderRegisterModelAdapter : BaseDskDeferredOrderRegisterAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="apiSetting">API接続設定</param>
		public DskDeferredOrderRegisterModelAdapter(OrderModel order, DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
			this.Order = order;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public DskDeferredOrderRegisterRequest CreateRequest()
		{
			var request = new DskDeferredOrderRegisterRequest();

			request.Buyer.ShopTransactionId = this.Order.PaymentOrderId;
			request.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			request.Buyer.FullName = this.Order.Owner.OwnerName;
			request.Buyer.FullKanaName = this.Order.Owner.OwnerNameKana;
			request.Buyer.Tel = this.Order.Owner.OwnerTel1;
			request.Buyer.Email = this.Order.Owner.OwnerMailAddr;
			request.Buyer.ZipCode = this.Order.Owner.OwnerZip;
			request.Buyer.Address1 = this.Order.Owner.OwnerAddr1;
			request.Buyer.Address2 = this.Order.Owner.OwnerAddr2;
			request.Buyer.Address3 = this.Order.Owner.OwnerAddr3 + "　" + this.Order.Owner.OwnerAddr4;
			request.Buyer.CompanyName = this.Order.Owner.OwnerCompanyName;
			request.Buyer.DepartmentName = this.Order.Owner.OwnerCompanyPostName;
			request.Buyer.BilledAmount = this.Order.LastBilledAmount.ToPriceString();
			// 決済種別（請求書の同梱か別送かどうか）
			request.Buyer.PaymentType = ((this.Order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
				? DSKDeferredPaymentType.IncludeService
				: DSKDeferredPaymentType.SeparateService);

			request.Deliveries.Delivery = GetDelivaryies();

			return request;
		}

		/// <summary>
		/// 配送先取得
		/// </summary>
		/// <returns>明細要素</returns>
		private DeliveryElement[] GetDelivaryies()
		{
			var shipping = this.Order.Shippings[0];
			var delivary = new DeliveryElement();
			delivary.DeliveryCustomer.FullName = shipping.ShippingName;
			delivary.DeliveryCustomer.FullKanaName = shipping.ShippingNameKana;
			delivary.DeliveryCustomer.Tel = shipping.ShippingTel1;
			delivary.DeliveryCustomer.ZipCode = shipping.ShippingZip;
			delivary.DeliveryCustomer.Address1 = shipping.ShippingAddr1;
			delivary.DeliveryCustomer.Address2 = shipping.ShippingAddr2;
			delivary.DeliveryCustomer.Address3 = shipping.ShippingAddr3 + "　" + shipping.ShippingAddr4;

			delivary.Details = GetDetail();

			var delivariyies = new[] { delivary };
			return delivariyies;
		}

		/// <summary>
		/// 受注明細取得
		/// </summary>
		/// <returns>明細要素</returns>
		private DetailsElement GetDetail()
		{
			var detailElements = new List<DetailElement>();

			var details = GetProductInfoOfOrderDetails();
			if (details != null)
			{
				detailElements.AddRange(details);
			}
			else
			{
				detailElements.Add(CreateDetailElement(
					Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SUBTOTAL,
					(decimal)this.Order.OrderPriceSubtotal.ToPriceDecimal()));
			}

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SHIPPING,
				(decimal)this.Order.OrderPriceShipping.ToPriceDecimal()));

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_PAYMENT,
				(decimal)this.Order.OrderPriceExchange.ToPriceDecimal()));

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_DISCOUNT_ETC,
				(decimal)(this.Order.OrderPriceTotal
					- (this.Order.OrderPriceSubtotal + this.Order.OrderPriceShipping + this.Order.OrderPriceExchange)).ToPriceDecimal()));

			var result = new DetailsElement
			{
				Detail = detailElements.ToArray()
			};
			return result;
		}

		/// <summary>
		/// 受注明細取得
		/// </summary>
		/// <returns>明細要素</returns>
		private DetailElement[] GetProductInfoOfOrderDetails()
		{
			if (Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_COOPERATION && (this.Order.Items != null))
			{
				var details = new List<DetailElement>();

				var itemEtcPrice = 0m;
				for (var i = 0; i < this.Order.Items.Length; i++)
				{
					var item = this.Order.Items[i];

					if (i < DskDeferredConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_ROWS)
					{
						details.Add(CreateDetailElement(
							item.ProductName,
							(decimal)item.ProductPrice.ToPriceDecimal(),
							item.ItemQuantity));
					}
					else
					{
						itemEtcPrice += item.ProductPrice * item.ItemQuantity;
					}
				}

				// 商品項目数が100項目以上ある場合は、100項目からそれ以降の金額をまとめて連携する
				if (this.Order.Items.Length > DskDeferredConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_ROWS)
				{
					details.Add(CreateDetailElement(
						Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_ITEM_ETC,
						(decimal)itemEtcPrice.ToPriceDecimal()));
				}
				return details.ToArray();
			}

			return null;
		}

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public DskDeferredOrderRegisterResponse Execute()
		{
			var facade = (this.ApiSetting == null) ? new DskDeferredApiFacade() : new DskDeferredApiFacade(this.ApiSetting);
			this.Request = CreateRequest();
			var response = facade.OrderRegister(this.Request);
			return response;
		}

		/// <summary>注文</summary>
		private OrderModel Order { get; set; }
		/// <summary>リクエスト</summary>
		private DskDeferredOrderRegisterRequest Request { get; set; }
	}
}
