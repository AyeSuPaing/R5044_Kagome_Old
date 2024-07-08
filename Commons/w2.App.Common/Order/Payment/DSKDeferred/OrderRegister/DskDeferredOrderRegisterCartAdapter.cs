/*
=========================================================================================================
  Module      : DSK後払い注文情報登録カートアダプタ(DskDeferredOrderRegisterAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderRegister
{
	/// <summary>
	/// DSK後払い注文情報登録カートアダプタ
	/// </summary>
	public class DskDeferredOrderRegisterCartAdapter : BaseDskDeferredOrderRegisterAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="apiSetting">API接続設定</param>
		public DskDeferredOrderRegisterCartAdapter(CartObject cart, DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
			this.Cart = cart;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>リクエスト</returns>
		public DskDeferredOrderRegisterRequest CreateRequest()
		{
			var request = new DskDeferredOrderRegisterRequest();

			request.Buyer.ShopTransactionId = OrderCommon.CreatePaymentOrderId(this.Cart.ShopId);
			request.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			request.Buyer.FullName = this.Cart.Owner.Name;
			request.Buyer.FullKanaName = this.Cart.Owner.NameKana;
			request.Buyer.Tel = this.Cart.Owner.Tel1;
			request.Buyer.Email = this.Cart.Owner.MailAddr;
			request.Buyer.ZipCode = this.Cart.Owner.Zip;
			request.Buyer.Address1 = this.Cart.Owner.Addr1;
			request.Buyer.Address2 = this.Cart.Owner.Addr2;
			request.Buyer.Address3 = this.Cart.Owner.Addr3 + "　" + this.Cart.Owner.Addr4;
			request.Buyer.CompanyName = this.Cart.Owner.CompanyName;
			request.Buyer.DepartmentName = StringUtility.ToEmpty(this.Cart.Owner.CompanyPostName);
			request.Buyer.BilledAmount = this.Cart.PriceTotal.ToPriceString();
			// 決済種別（請求書の同梱か別送かどうか）
			request.Buyer.PaymentType = ((this.Cart.GetInvoiceBundleFlg() == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
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
			var shipping = this.Cart.Shippings[0];
			var delivary = new DeliveryElement();
			delivary.DeliveryCustomer.FullName = shipping.Name;
			delivary.DeliveryCustomer.FullKanaName = shipping.NameKana;
			delivary.DeliveryCustomer.Tel = shipping.Tel1;
			delivary.DeliveryCustomer.ZipCode = shipping.Zip;
			delivary.DeliveryCustomer.Address1 = shipping.Addr1;
			delivary.DeliveryCustomer.Address2 = shipping.Addr2;
			delivary.DeliveryCustomer.Address3 = shipping.Addr3 + '　' + shipping.Addr4;

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
					(decimal)this.Cart.PriceSubtotal.ToPriceDecimal()));
			}

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SHIPPING,
				(decimal)this.Cart.PriceShipping.ToPriceDecimal()));

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_PAYMENT,
				(decimal)this.Cart.Payment.PriceExchange.ToPriceDecimal()));

			detailElements.Add(CreateDetailElement(
				Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_DISCOUNT_ETC,
				(decimal)(this.Cart.PriceTotal - (this.Cart.PriceSubtotal + this.Cart.PriceShipping + this.Cart.Payment.PriceExchange)).ToPriceDecimal()));

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
			if (Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_COOPERATION && (this.Cart != null))
			{
				var details = new List<DetailElement>();

				var itemEtcPrice = 0m;
				for (var i = 0; i < this.Cart.Items.Count; i++)
				{
					var item = this.Cart.Items[i];

					if (i < DskDeferredConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_ROWS)
					{
						details.Add(CreateDetailElement(
							item.ProductName,
							item.Price,
							item.Count));
					}
					else
					{
						itemEtcPrice += item.Price * item.Count;
					}
				}

				// 商品項目数が100項目以上ある場合は、100項目からそれ以降の金額をまとめて連携する
				if (this.Cart.Items.Count > DskDeferredConst.MAXIMUM_NUMBER_OF_PRODUCT_ITEM_ROWS)
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

		/// <summary>カート</summary>
		private CartObject Cart { get; set; }
		/// <summary>リクエスト</summary>
		public DskDeferredOrderRegisterRequest Request { get; set; }
	}
}
