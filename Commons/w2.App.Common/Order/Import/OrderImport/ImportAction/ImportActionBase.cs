/*
=========================================================================================================
  Module      : 注文取り込み基底クラス(ImportActionBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order.Import.OrderImport.CreateModel;
using w2.App.Common.Order.Import.OrderImport.Entity;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.App.Common.Order.Import.OrderImport.ImportAction
{
	/// <summary>
	/// 注文取り込み基底
	/// </summary>
	public abstract class ImportActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderData">取り込み注文データ</param>
		protected ImportActionBase(OrderData orderData)
		{
			this.ImportData = orderData;

			try
			{
				this.ImportData.Order = CreateOrder();
				this.ImportData.User = CreateUser();
				this.ImportData.Cart = CreateCart();
				this.ImportData.Coupon = CreateCoupon();
			}
			catch (Exception e)
			{
				FileLogger.WriteError(e);
				this.ErrorMessage.AppendLine(
					ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_CREATE_ORDER_INFORMATION));
			}
		}

		/// <summary>
		/// 取り込み処理
		/// </summary>
		public abstract void Import();

		/// <summary>
		/// 初期値設定
		/// </summary>
		protected abstract void SetDefaultValue();

		/// <summary>
		/// 注文作成
		/// </summary>
		/// <returns>注文モデル</returns>
		private OrderModel CreateOrder()
		{
			var order = this.ImportData.CsvOrderData[0];
			var model = new OrderModel();

			model.DateCreated = DateTime.Now;
			model.DateChanged = DateTime.Now;

			var createOrder = new CreateModelOrder();
			createOrder.SetData(model, order);

			model.Items = CreateOrderItems();
			model.Owner = CreateOrderOwner();
			model.Shippings = new[] { CreateOrderShipping() };
			model.OrderPriceByTaxRates = new OrderPriceByTaxRateModel[] { };
			var user = new UserService().Get(model.UserId);

			if (order.ContainsKey("order_count_order"))
			{
				model.OrderCountOrder = int.Parse((string)order["order_count_order"]);
			}
			else if (user != null)
			{
				model.OrderCountOrder = user.OrderCountOrderRealtime;

				if (model.IsCancelled == false)
				{
					model.OrderCountOrder++;
				}
			}
			else
			{
				model.OrderCountOrder = 1;
			}

			if (order.ContainsKey(Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT))
			{
				int parsedSubscriptionBoxOrderCount;
				model.OrderSubscriptionBoxOrderCount = int.TryParse(
					(string)order[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT],
					out parsedSubscriptionBoxOrderCount)
					? parsedSubscriptionBoxOrderCount
					: 0;
			}

			foreach (var orderShippingModel in model.Shippings)
			{
				orderShippingModel.Items = model.Items
					.Where(item => item.OrderShippingNo == orderShippingModel.OrderShippingNo).ToArray();

				// 商品消費税額を計算
				foreach (var orderItemModel in orderShippingModel.Items)
				{
					var productTaxPrice = TaxCalculationUtility.GetTaxPrice(
						orderItemModel.ProductPrice,
						orderItemModel.ProductTaxRate,
						orderShippingModel.ShippingCountryIsoCode,
						orderShippingModel.ShippingAddr5,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
					if (orderItemModel.ProductPricePretax == 0)
					{
						orderItemModel.ProductPricePretax = TaxCalculationUtility.GetPriceTaxIncluded(
							orderItemModel.ProductPrice,
							productTaxPrice);
					}

					if (orderItemModel.ItemPriceTax == 0)
					{
						orderItemModel.ItemPriceTax = productTaxPrice * orderItemModel.ItemQuantity;
					}
				}
			}
			
			// 消費税額を自動計算
			OrderCommon.SetCalculateTax(model);

			// 請求書同梱フラグセット
			model.InvoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(model.OrderPaymentKbn)
				? OrderCommon.JudgmentInvoiceBundleFlg(model)
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;

			return model;
		}

		/// <summary>
		/// 注文商品作成
		/// </summary>
		/// <returns>注文商品モデル</returns>
		private OrderItemModel[] CreateOrderItems()
		{
			var result = new List<OrderItemModel>();

			foreach (var order in this.ImportData.CsvOrderData)
			{
				var model = new OrderItemModel();
				model.DateCreated = DateTime.Now;
				model.DateChanged = DateTime.Now;

				var createOrderItem = new CreateModelOrderItem();
				createOrderItem.SetData(model, order);

				model.SubscriptionBoxCourseId = order.ContainsKey(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX)
					? StringUtility.ToEmpty(order[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX])
					: string.Empty;
				model.SubscriptionBoxFixedAmount =
					order.ContainsKey(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX)
						&& decimal.TryParse(
								StringUtility.ToEmpty(order[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX]),
								out var fixedAmount)
							? (decimal?)fixedAmount
							: null;

				result.Add(model);
			}
			return result.ToArray();
		}

		/// <summary>
		/// 注文者作成
		/// </summary>
		/// <returns>注文者モデル</returns>
		private OrderOwnerModel CreateOrderOwner()
		{
			var order = this.ImportData.CsvOrderData[0];
			var model = new OrderOwnerModel();
			model.DateCreated = DateTime.Now;
			model.DateChanged = DateTime.Now;

			var createOrderOwner = new CreateModelOrderOwner();
			createOrderOwner.SetData(model, order);

			return model;
		}

		/// <summary>
		/// 注文配送先作成
		/// </summary>
		/// <returns>注文配送先モデル</returns>
		private OrderShippingModel CreateOrderShipping()
		{
			var order = this.ImportData.CsvOrderData[0];
			var model = new OrderShippingModel();
			model.DateCreated = DateTime.Now;
			model.DateChanged = DateTime.Now;

			var createOrderShipping = new CreateModelOrderShipping();
			createOrderShipping.SetData(model, order);

			return model;
		}

		/// <summary>
		/// ユーザー作成
		/// </summary>
		/// <returns>ユーザーモデル</returns>
		private UserModel CreateUser()
		{
			var order = this.ImportData.Order;
			var orderOwner = order.Owner;

			var zip = (orderOwner.OwnerZip + "-").Split('-');
			var tel1 = (orderOwner.OwnerTel1 + "--").Split('-');
			var tel2 = (orderOwner.OwnerTel2 + "--").Split('-');

			var model = new UserModel
			{
				UserId = order.UserId,
				UserKbn = orderOwner.OwnerKbn,
				Name = orderOwner.OwnerName,
				Name1 = orderOwner.OwnerName1,
				Name2 = orderOwner.OwnerName2,
				NameKana = orderOwner.OwnerNameKana,
				NameKana1 = orderOwner.OwnerNameKana1,
				NameKana2 = orderOwner.OwnerNameKana2,
				MailAddr = orderOwner.OwnerMailAddr,
				MailAddr2 = orderOwner.OwnerMailAddr2,
				Zip = orderOwner.OwnerZip,
				Zip1 = zip[0],
				Zip2 = zip[1],
				Addr = orderOwner.ConcatenateAddressWithoutCountryName(),
				Addr1 = orderOwner.OwnerAddr1,
				Addr2 = orderOwner.OwnerAddr2,
				Addr3 = orderOwner.OwnerAddr3,
				Addr4 = orderOwner.OwnerAddr4,
				CompanyName = orderOwner.OwnerCompanyName,
				CompanyPostName = orderOwner.OwnerCompanyPostName,
				Tel1 = orderOwner.OwnerTel1,
				Tel1_1 = tel1[0],
				Tel1_2 = tel1[1],
				Tel1_3 = tel1[2],
				Tel2 = orderOwner.OwnerTel2,
				Tel2_1 = tel2[0],
				Tel2_2 = tel2[1],
				Tel2_3 = tel2[2],
				Sex = orderOwner.OwnerSex,
				Birth = orderOwner.OwnerBirth,
				BirthYear = (orderOwner.OwnerBirth.HasValue) ? ((DateTime)orderOwner.OwnerBirth).Year.ToString() : "",
				BirthMonth = (orderOwner.OwnerBirth.HasValue) ? ((DateTime)orderOwner.OwnerBirth).Month.ToString() : "",
				BirthDay = (orderOwner.OwnerBirth.HasValue) ? ((DateTime)orderOwner.OwnerBirth).Day.ToString() : "",
				MemberRankId = order.MemberRankId,
			};

			return model;
		}

		/// <summary>
		/// カート作成
		/// </summary>
		/// <returns>カート</returns>
		private CartObject CreateCart()
		{
			var user = this.ImportData.User;
			var order = this.ImportData.Order;
			var orderShipping = order.Shippings.First();

			var cart = new CartObject(order.UserId, order.OrderKbn, order.ShopId, order.ShippingId, false, false, order.MemberRankId);
			cart.OrderId = order.OrderId;

			foreach (var item in order.Items)
			{
				// 商品情報が存在しない場合はエラーとする
				var product = new ProductService().GetProductVariationAtDataRowView(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					order.MemberRankId);
				if (product == null)
				{
					var messages = string.Format("商品情報（shop_id:{0},product_id:{1},variation_id:{2}）が存在しないため、注文登録できません。",
						item.ShopId,
						item.ProductId,
						item.VariationId);
					this.ErrorMessage.AppendLine(messages);
					continue;
				}
				var cartProduct = new CartProduct(
					product,
					this.ImportData.IsRegistFixedPurchase ? Constants.AddCartKbn.FixedPurchase : Constants.AddCartKbn.Normal,
					item.ProductsaleId,
					item.ItemQuantity,
					false);
				cartProduct.SetPrice(item.ProductPrice);
				cart.AddVirtural(cartProduct, false);
			}

			// カートの領収書情報をセット
			if (Constants.RECEIPT_OPTION_ENABLED && (order.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON))
			{
				cart.ReceiptFlg = order.ReceiptFlg;
				cart.ReceiptAddress = order.ReceiptAddress;
				cart.ReceiptProviso = order.ReceiptProviso;
			}

			var cartList = new CartObjectList(user.UserId, order.OrderKbn, false, memberRankId: order.MemberRankId);
			cartList.AddCartVirtural(cart);

			var payment = new CartPayment
			{
				PaymentId = order.OrderPaymentKbn,
				PriceExchange = order.OrderPriceExchange,
			};
			cartList.Items.ForEach(cartObject => cartObject.Payment = payment);

			cartList.SetOwner(new CartOwner(user));

			var zip = (orderShipping.ShippingZip + "-").Split('-');
			var tel = (orderShipping.ShippingTel1 + "--").Split('-');

			cartList.Items.First().Shippings.First().UpdateShippingAddr(
				orderShipping.ShippingName1,
				orderShipping.ShippingName2,
				orderShipping.ShippingNameKana1,
				orderShipping.ShippingNameKana2,
				orderShipping.ShippingZip,
				zip[0],
				zip[1],
				orderShipping.ShippingAddr1,
				orderShipping.ShippingAddr2,
				orderShipping.ShippingAddr3,
				orderShipping.ShippingAddr4,
				orderShipping.ShippingAddr5,
				orderShipping.ShippingCountryIsoCode,
				orderShipping.ShippingCountryName,
				string.Empty,
				string.Empty,
				"",
				tel[0],
				tel[1],
				tel[2],
				true,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);

			if (this.ImportData.IsRegistFixedPurchase)
			{
				var shopShipping = new ShopShippingService().Get(order.ShopId, order.ShippingId);
				var cartShipping = cartList.Items.First().Shippings.First();

				cartShipping.UpdateFixedPurchaseSetting(
					this.ImportData.FixedPurchaseKbn,
					this.ImportData.FixedPurchaseSetting1,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);

				cartShipping.UpdateShippingMethod(
					orderShipping.ShippingMethod,
					orderShipping.DeliveryCompanyId);

				if (string.IsNullOrEmpty(orderShipping.ShippingTime) == false)
				{
					cartShipping.UpdateShippingTime(orderShipping.ShippingTime);
				}

				cartShipping.CalculateNextShippingDates();

				if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
				{
					var cartFixedPurchaseNextShippingProduct = cartList.Items.First().Items
						.FirstOrDefault(cartProduct => cartProduct.CanSwitchProductFixedPurchaseNextShippingSecondTime());
					if (cartFixedPurchaseNextShippingProduct != null)
					{
						cartShipping.CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
						cartShipping.UpdateNextShippingItemFixedPurchaseInfos(
							cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
							cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
						cartShipping.CalculateNextShippingItemNextNextShippingDate();
					}
				}
			}

			cartList.Items.First().UpdateAnotherShippingFlag();
			cartList.CalculateAllCart();

			//再計算で書き換わってしまうと不都合のある値を再設定
			var result = cartList.Items.First();
			result.Payment.PriceExchange = order.OrderPriceExchange;
			result.SetPriceShipping(order.OrderPriceShipping);
			result.SetPriceSubtotal(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax);
			result.SetPriceTotal(order.OrderPriceTotal);
			result.SettlementCurrency = CurrencyManager.GetSettlementCurrency(result.Payment.PaymentId);
			result.SettlementRate = CurrencyManager.GetSettlementRate(result.SettlementCurrency);
			result.SettlementAmount = CurrencyManager.GetSettlementAmount(
				result.PriceTotal,
				result.SettlementRate,
				result.SettlementCurrency);
			result.PriceRegulation = order.OrderPriceRegulation;
			result.MemberRankDiscount = order.MemberRankDiscountPrice;
			result.UseCouponPrice = order.OrderCouponUse;
			result.FixedPurchaseDiscount = order.FixedPurchaseDiscountPrice;
			result.FixedPurchaseMemberDiscountAmount = order.FixedPurchaseMemberDiscountAmount;
			result.UsePointPrice = order.OrderPointUseYen;

			return result;
		}

		/// <summary>
		/// クーポン作成
		/// </summary>
		/// <returns>クーポンモデル</returns>
		private OrderCouponModel CreateCoupon()
		{
			var order = this.ImportData.CsvOrderData[0];

			var model = new OrderCouponModel()
			{
				OrderId = (string)order[Constants.FIELD_ORDERCOUPON_ORDER_ID],
				OrderCouponNo = 1,
				DeptId = (string)order[Constants.FIELD_ORDERCOUPON_DEPT_ID],
				CouponId = (string)order[Constants.FIELD_ORDERCOUPON_COUPON_ID],
				CouponNo = (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDERCOUPON_COUPON_NO])) ? 1 : int.Parse((string)order[Constants.FIELD_ORDERCOUPON_COUPON_NO]),
				CouponCode = (string)order[Constants.FIELD_ORDERCOUPON_COUPON_CODE],
				CouponName = (string)order[Constants.FIELD_ORDERCOUPON_COUPON_NAME],
				CouponDispName = (string)order[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME],
				CouponType = (string)order[Constants.FIELD_ORDERCOUPON_COUPON_TYPE],
				CouponDiscountPrice = (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE])) ? (decimal?)null : decimal.Parse((string)order[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE]),
				CouponDiscountRate = (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE])) ? (decimal?)null : decimal.Parse((string)order[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE]),
				DateCreated = DateTime.Parse(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_DATE])),
				DateChanged = DateTime.Parse(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_DATE])),
				LastChanged = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED])
			};

			var createOrder = new CreateModelOrder();
			createOrder.SetData(model, order);

			return model;
		}

		/// <summary>取り込みデータ</summary>
		public OrderData ImportData { get; set; }
		/// <summary>エラーメッセージ</summary>
		public StringBuilder ErrorMessage { get { return this.m_errorMessage; } }
		private readonly StringBuilder m_errorMessage = new StringBuilder();
		/// <summary>注文アイテム生成数</summary>
		public int OrderItemImportCount { get; protected set; }
		/// <summary>定期台帳生成数</summary>
		public int FixedPurchaseRegistCount { get; protected set; }
	}
}
