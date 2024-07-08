/*
=========================================================================================================
  Module      : NP After Pay Utility(NPAfterPayUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.NPAfterPay
{
	/// <summary>
	/// NP After Pay Utility
	/// </summary>
	public class NPAfterPayUtility
	{
		#region +Constants
		/// <summary>NP After Pay : api date format</summary>
		private const string NPAFTERPAY_API_DATE_FORMAT = "yyyy-MM-dd";
		/// <summary>NP After Pay : settlement type invoice separated</summary>
		private const string FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_SEPARATED = "02";
		/// <summary>NP After Pay : settlement type invoice bundled</summary>
		private const string FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_BUNDLED = "03";
		#endregion

		#region +Methods
		/// <summary>
		/// Create Order Request Data
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="order">Order</param>
		/// <param name="paymentOrderId">Payment Order Id</param>
		/// <param name="isUpdate">Is Update</param>
		/// <param name="isNeedUpdateInvoiceBundleFlgOff">Is Need Update Invoice Bundle Flg Off</param>
		/// <returns>NP After Pay Request</returns>
		public static NPAfterPayRequest CreateOrderRequestData(
			CartObject cart,
			Hashtable order,
			string paymentOrderId,
			bool isUpdate = false,
			bool isNeedUpdateInvoiceBundleFlgOff = false)
		{
			var invoiceBundleFlg = isNeedUpdateInvoiceBundleFlgOff
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF
				: cart.GetInvoiceBundleFlg();
			var cardTranId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]);

			// Create Products
			var goods = CreateProducts(cart);

			var transaction = new Transaction()
			{
				ShopTransactionId = paymentOrderId,
				ShopOrderDate = DateTime.Now.ToString(NPAFTERPAY_API_DATE_FORMAT),
				SettlementType = (invoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
					? FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_BUNDLED
					: FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_SEPARATED,
				BilledAmount = (int)cart.PriceTotal,
				Customer = CreateCustomer(cart),
				DestCustomer = CreateDestCustomer(cart, invoiceBundleFlg),
				Goods = goods,
			};

			// For case update order
			if (isUpdate)
			{
				transaction.NpTransactionId = cardTranId;
			}

			var request = new NPAfterPayRequest()
			{
				Transactions = new[] { transaction }
			};
			return request;
		}

		/// <summary>
		/// 注文情報連携データ作成
		/// </summary>
		/// <param name="order">OrderModel(更新時は変更前注文情報)</param>
		/// <param name="paymentOrderId">決済取引ID</param>
		/// <param name="isUpdate">更新か</param>
		/// <param name="isNeedUpdateInvoiceBundleFlgOff">請求書同梱フラグの更新は必要か</param>
		/// <param name="orderNew">変更後注文情報</param>
		/// <returns>NP後払い連携データ</returns>
		public static NPAfterPayRequest CreateOrderRequestData(
			OrderModel order,
			string paymentOrderId,
			bool isUpdate = false,
			bool isNeedUpdateInvoiceBundleFlgOff = false,
			OrderModel orderNew = null)
		{
			var invoiceBundleFlg = isNeedUpdateInvoiceBundleFlgOff
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF
				: orderNew.InvoiceBundleFlg;
			var cardTranId = StringUtility.ToEmpty(order.CardTranId);

			// 商品データ作成
			if ((IsReturnOrExchange == false) && (orderNew != null))
			{
				// 注文情報更新(order：注文情報、orderNew：利用しない)
				order = orderNew;
				orderNew = null;
			}
			var goods = CreateProducts(order, orderNew);

			var transaction = new Transaction
			{
				ShopTransactionId = paymentOrderId,
				ShopOrderDate = DateTime.Now.ToString(NPAFTERPAY_API_DATE_FORMAT),
				SettlementType = (invoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
					? FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_BUNDLED
					: FLG_NPAFTERPAY_SETTLEMENT_TYPE_INVOICE_SEPARATED,
				BilledAmount = (IsReturnOrExchange) ? (int)orderNew.LastBilledAmount : (int)order.LastBilledAmount,
				Customer = new Customer
				{
					CustomerName = order.Owner.OwnerName.Replace(" ", ""),
					CustomerNameKana = order.Owner.OwnerNameKana.Replace(" ", ""),
					CompanyName = order.Owner.OwnerCompanyName,
					DepartmentName = order.Owner.OwnerCompanyPostName,
					ZipCode = order.Owner.OwnerZip,
					Address = order.Owner.ConcatenateAddressWithoutCountryName().Replace(" ", string.Empty),
					Tel = order.Owner.OwnerTel1,
					Email = order.Owner.OwnerMailAddr
				},
				DestCustomer = CreateDestCustomer(order, invoiceBundleFlg),
				Goods = goods,
			};

			// 更新処理
			if (isUpdate)
			{
				transaction.NpTransactionId = cardTranId;
			}

			var request = new NPAfterPayRequest
			{
				Transactions = new[] { transaction }
			};

			IsReturnOrExchange = false;
			return request;
		}

		/// <summary>
		/// Create Cancel Or Get Payment Request Data
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <returns>NP After Pay Request</returns>
		public static NPAfterPayRequest CreateCancelOrGetPaymentRequestData(string cardTranId)
		{
			var transaction = new Transaction()
			{
				NpTransactionId = cardTranId
			};

			var request = new NPAfterPayRequest()
			{
				Transactions = new[] { transaction }
			};
			return request;
		}

		/// <summary>
		/// Create Shipment Request Data
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="deliveryCompanyType">Delivery Company Type</param>
		/// <param name="billIssuedDate">Bill Issued Date</param>
		/// <returns>NP After Pay Request</returns>
		public static NPAfterPayRequest CreateShipmentRequestData(
			string cardTranId,
			string shippingCheckNo,
			string deliveryCompanyType,
			string billIssuedDate)
		{
			var transaction = new Transaction()
			{
				NpTransactionId = cardTranId,
				PdCompanyCode = deliveryCompanyType,
				SlipNo = shippingCheckNo,
				BillIssuedDate = string.IsNullOrEmpty(billIssuedDate)
					? null
					: billIssuedDate
			};

			var request = new NPAfterPayRequest()
			{
				Transactions = new[] { transaction }
			};
			return request;
		}

		/// <summary>
		/// Is NP After Pay Has Paid
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <returns>True: Payment has paid, otherwise: false</returns>
		public static bool IsNPAfterPayHasPaid(string cardTranId)
		{
			var request = CreateCancelOrGetPaymentRequestData(cardTranId);
			var result = NPAfterPayApiFacade.GetPaymentOrder(request);
			return result.HasPaid;
		}

		/// <summary>
		/// Check Bill Issued Date
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="billIssuedDate">Bill Issued Date</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Error Message</returns>
		public static string CheckBillIssuedDate(
			OrderModel order,
			out string billIssuedDate,
			SqlAccessor accessor = null)
		{
			billIssuedDate = GetBillIssuedDate(order, accessor);
			if ((order != null)
				&& (order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
				&& string.IsNullOrEmpty(billIssuedDate))
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CANNOT_SHIPMENT_ERROR);
			}
			return string.Empty;
		}

		/// <summary>
		/// Get Bill Issued Date
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>Bill Issued Date</returns>
		private static string GetBillIssuedDate(OrderModel order, SqlAccessor accessor = null)
		{
			if ((order == null)
				|| (order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)) return string.Empty;

			if (order.IsReturnOrder && (string.IsNullOrEmpty(order.OrderIdOrg) == false))
			{
				var orderOriginal = new OrderService().Get(order.OrderIdOrg, accessor);
				if (orderOriginal != null)
				{
					order.OrderShippedDate = orderOriginal.OrderShippedDate;
					order.OrderShippingDate = orderOriginal.OrderShippingDate;
					order.OrderDeliveringDate = orderOriginal.OrderDeliveringDate;
				}
			}

			var billIssuedDate = string.Empty;
			if (order.OrderDeliveringDate.HasValue)
			{
				billIssuedDate = order.OrderDeliveringDate.Value.ToString(NPAFTERPAY_API_DATE_FORMAT);
			}
			else if (order.OrderShippedDate.HasValue)
			{
				billIssuedDate = order.OrderShippedDate.Value.ToString(NPAFTERPAY_API_DATE_FORMAT);
			}
			else if (order.OrderShippingDate.HasValue)
			{
				billIssuedDate = order.OrderShippingDate.Value.ToString(NPAFTERPAY_API_DATE_FORMAT);
			}
			return billIssuedDate;
		}

		/// <summary>
		/// Create products
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <returns>Products</returns>
		private static Goods[] CreateProducts(CartObject cart)
		{
			// Create product list
			var result = cart.Items
				.Where(product => (Constants.PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED
					|| (product.IsBundle == false)
					|| (product.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)))
				.Select(cartProduct => CreateProduct(cartProduct)).ToList();

			// 割引額
			var discount = cart.MemberRankDiscount
				+ cart.UseCouponPrice
				+ cart.SetPromotions.ProductDiscountAmount
				+ cart.SetPromotions.ShippingChargeDiscountAmount
				+ cart.SetPromotions.PaymentChargeDiscountAmount
				+ cart.FixedPurchaseDiscount
				+ cart.FixedPurchaseMemberDiscountAmount;
			if (discount > 0)
			{
				var item = CreateProduct("割引額", 1, -(int)discount);
				result.Add(item);
			}

			// ポイント利用額
			if (cart.UsePointPrice > 0)
			{
				var item = CreateProduct("ポイント利用額", 1, -(int)cart.UsePointPrice);
				result.Add(item);
			}

			// 調整金額
			if (cart.PriceRegulationTotal != 0)
			{
				var item = CreateProduct("調整金額", 1, (int)cart.PriceRegulationTotal);
				result.Add(item);
			}

			// 決済手数料
			if ((cart.Payment != null)
				&& (cart.Payment.PriceExchange != 0))
			{
				var item = CreateProduct("決済手数料", 1, (int)cart.Payment.PriceExchange);
				result.Add(item);
			}

			// 配送料
			if (cart.PriceShipping > 0)
			{
				var item = CreateProduct("配送料", 1, (int)cart.PriceShipping);
				result.Add(item);
			}
			return result.ToArray();
		}

		/// <summary>
		/// 商品データ作成
		/// </summary>
		/// <param name="order">OrderModel</param>
		/// <param name="orderNew">変更後注文情報</param>
		/// <returns>商品データ</returns>
		private static Goods[] CreateProducts(OrderModel order, OrderModel orderNew)
		{
			// 商品名・注文数・商品単価
			var result = new List<Goods>();
			if (IsReturnOrExchange)
			{
				var resultOld = order.Items
					.Where(product => (Constants.PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED
						|| (product.IsProductBundleItem == false)
						|| (product.BundleItemDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)))
					.Select(product => CreateProduct(product.VariationId, product.ProductName, product.ItemQuantity, (int)product.ProductPrice))
					.ToList();
				var resultNew = orderNew.Items
					.Where(product => (Constants.PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED
						|| (product.IsProductBundleItem == false)
						|| (product.BundleItemDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)))
					.Select(product => CreateProduct(product.VariationId, product.ProductName, product.ItemQuantity, (int)product.ProductPrice))
					.ToList();

				// 返品データから返品分を引きresultに追加
				foreach (var product in resultOld.Union(resultNew))
				{
					var sameProduct = result.FirstOrDefault(good => good.VariationId == product.VariationId);
					if (sameProduct != null)
					{
						sameProduct.Quantity += product.Quantity;
						continue;
					}
					result.Add(product);
				}

				// バリエーションIDを連携しないようにする
				foreach (var good in result)
				{
					good.VariationId = null;
				}
			}
			else
			{
				result = order.Items
					.Where(product => (Constants.PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED
						|| (product.IsProductBundleItem == false)
						|| (product.BundleItemDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)))
					.Select(product => CreateProduct(product.ProductName, product.ItemQuantity, (int)product.ProductPrice))
					.ToList();
			}

			// 割引額
			var couponDiscount = 0m;
			foreach (var coupon in order.Coupons)
			{
				if (coupon.CouponDiscountPrice != null)
				{
					couponDiscount += (decimal)coupon.CouponDiscountPrice;
					if (IsReturnOrExchange) break;
				}
			}
			var productDiscount = 0m;
			var shippingDiscount = 0m;
			var paymentDiscount = 0m;
			foreach (var price in order.SetPromotions)
			{
				productDiscount += price.ProductDiscountAmount;
				shippingDiscount += price.ShippingChargeDiscountAmount;
				paymentDiscount += price.PaymentChargeDiscountAmount;
			}
			var discount = order.MemberRankDiscountPrice
				+ couponDiscount
				+ productDiscount
				+ shippingDiscount
				+ paymentDiscount
				+ order.FixedPurchaseDiscountPrice
				+ order.FixedPurchaseMemberDiscountAmount;
			if (discount > 0)
			{
				var item = CreateProduct("割引額", 1, -(int)discount);
				result.Add(item);
			}

			// ポイント利用額
			if (IsReturnOrExchange)
			{
				if (orderNew.LastOrderPointUseYen != 0)
				{
					var item = CreateProduct("ポイント利用額", 1, -(int)orderNew.LastOrderPointUseYen);
					result.Add(item);
				}
			}
			else
			{
				if (order.OrderPointUseYen > 0)
				{
					var item = CreateProduct("ポイント利用額", 1, -(int)order.OrderPointUseYen);
					result.Add(item);
				}
			}

			// 調整金額
			var totalRegulationPrice = order.OrderPriceRegulationTotal;
			if (IsReturnOrExchange)
			{
				totalRegulationPrice += orderNew.OrderPriceByTaxRates.Sum(price => price.ReturnPriceCorrectionByRate);
			}
			if (totalRegulationPrice != 0)
			{
				var item = CreateProduct("調整金額", 1, (int)totalRegulationPrice);
				result.Add(item);
			}

			// 決済手数料
			if (order.OrderPriceExchange > 0)
			{
				var item = CreateProduct("決済手数料", 1, (int)order.OrderPriceExchange);
				result.Add(item);
			}

			// 配送料
			if (order.OrderPriceShipping > 0)
			{
				var item = CreateProduct("配送料", 1, (int)order.OrderPriceShipping);
				result.Add(item);
			}
			return result.ToArray();
		}

		/// <summary>
		/// Create Customer
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <returns>Customer Object</returns>
		private static Customer CreateCustomer(CartObject cart)
		{
			var result = new Customer()
			{
				CustomerName = cart.Owner.Name,
				CustomerNameKana = cart.Owner.NameKana,
				CompanyName = cart.Owner.CompanyName,
				DepartmentName = cart.Owner.CompanyPostName,
				ZipCode = cart.Owner.Zip,
				Address = cart.Owner.ConcatenateAddressWithoutCountryName().Replace(" ", string.Empty),
				Tel = cart.Owner.Tel1,
				Email = cart.Owner.MailAddr
			};

			return result;
		}
		/// <summary>
		/// Create Dest Customer
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="invoiceBundleFlg">Invoice Bundle Flag</param>
		/// <returns>Dest Customer Object</returns>
		private static DestCustomer CreateDestCustomer(CartObject cart, string invoiceBundleFlg)
		{
			DestCustomer result;
			if (invoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
			{
				result = new DestCustomer()
				{
					CustomerName = string.Empty,
					CustomerNameKana = string.Empty,
					CompanyName = string.Empty,
					DepartmentName = string.Empty,
					ZipCode = string.Empty,
					Address = string.Empty,
					Tel = string.Empty
				};
			}
			else
			{
				var cartShipping = cart.GetShipping();
				result = new DestCustomer()
				{
					CustomerName = cartShipping.Name,
					CustomerNameKana = cartShipping.NameKana,
					CompanyName = cartShipping.CompanyName,
					DepartmentName = cartShipping.CompanyPostName,
					ZipCode = cartShipping.Zip,
					Address = cartShipping.ConcatenateAddressWithoutCountryName().Replace(" ", string.Empty),
					Tel = cartShipping.Tel1,

				};
			}
			return result;
		}

		/// <summary>
		/// 配送先データ作成
		/// </summary>
		/// <param name="order">Order Model</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <returns>配送先データ</returns>
		private static DestCustomer CreateDestCustomer(OrderModel order, string invoiceBundleFlg)
		{
			if (order.Shippings.Length == 0)
			{
				return new DestCustomer
				{
					CustomerName = string.Empty,
					CustomerNameKana = string.Empty,
					CompanyName = string.Empty,
					DepartmentName = string.Empty,
					ZipCode = string.Empty,
					Address = string.Empty,
					Tel = string.Empty
				};
			}
			var cartShipping = order.Shippings[0];
			return new DestCustomer
			{
				CustomerName = cartShipping.ShippingName,
				CustomerNameKana = cartShipping.ShippingNameKana,
				CompanyName = cartShipping.ShippingCompanyName,
				DepartmentName = cartShipping.ShippingCompanyPostName,
				ZipCode = cartShipping.ShippingZip,
				Address = cartShipping.ConcatenateAddressWithoutCountryName().Replace(" ", string.Empty),
				Tel = cartShipping.ShippingTel1,
			};
		}

		/// <summary>
		/// Create product object
		/// </summary>
		/// <param name="cartProduct">Cart Product</param>
		/// <returns>A product object</returns>
		private static Goods CreateProduct(CartProduct cartProduct)
		{
			var result = new Goods()
			{
				GoodName = cartProduct.ProductJointName,
				GoodPrice = (int)cartProduct.Price,
				Quantity = cartProduct.Count
			};
			return result;
		}

		/// <summary>
		/// Create product object
		/// </summary>
		/// <param name="productName">Product Name</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="price">Price</param>
		/// <returns>A product object</returns>
		private static Goods CreateProduct(
			string productName,
			int quantity,
			int price)
		{
			var result = new Goods()
			{
				GoodName = productName,
				GoodPrice = price,
				Quantity = quantity
			};
			return result;
		}
		/// <summary>
		/// Create product object
		/// </summary>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="productName">Product Name</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="price">Price</param>
		/// <returns>A product object</returns>
		private static Goods CreateProduct(
			string variationId,
			string productName,
			int quantity,
			int price)
		{
			var result = new Goods()
			{
				VariationId = variationId,
				GoodName = productName,
				GoodPrice = price,
				Quantity = quantity
			};
			return result;
		}

		/// <summary>
		/// Create Cart For Order Modify
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Cart Object</returns>
		public static CartObject CreateCartForOrderModify(OrderModel order)
		{
			// Create cart and recalculate cart
			var cart = CartObject.CreateCartByOrder(order);
			cart.SetPriceShipping(order.OrderPriceShipping);
			cart.Payment.PriceExchange = order.OrderPriceExchange;
			cart.Calculate(false);
			return cart;
		}

		/// <summary>
		/// Create Cart For Order Return Exchange
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order New</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Cart Object</returns>
		public static CartObject CreateCartForOrderReturnExchange(
			OrderModel orderOld,
			OrderModel orderNew,
			SqlAccessor accessor = null)
		{
			var orderIdOrg = string.IsNullOrEmpty(orderOld.OrderIdOrg)
				? orderOld.OrderId
				: orderOld.OrderIdOrg;
			var orderService = new OrderService();
			var order = orderService.Get(orderIdOrg, accessor);
			var orderItems = orderService.GetRelatedOrderItems(
				orderIdOrg,
				accessor);

			var modelList = new List<OrderItemModel>();
			foreach (var item in orderItems)
			{
				if (modelList.Any(data => (data.ProductId == item.ProductId)
					&& (data.VariationId == item.VariationId)) == false)
				{
					modelList.Add(item);
				}
				else
				{
					modelList.Where(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId))
						.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
				}
			}

			// For return exchange order
			if (orderOld.OrderId != orderNew.OrderId)
			{
				foreach (var item in orderNew.Shippings[0].Items)
				{
					if (modelList.Any(data => (data.ProductId == item.ProductId)
						&& (data.VariationId == item.VariationId)) == false)
					{
						modelList.Add(item);
					}
					else
					{
						modelList.Where(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId))
							.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
					}
				}

				// Set order point use for return exchange order
				order.OrderPointUse = order.LastOrderPointUse + orderNew.OrderPointUse;
			}
			else
			{
				// Set order point use for reauth order exchange order
				order.OrderPointUse = order.LastOrderPointUse;
			}

			// Set items
			order.Shippings[0].Items = modelList.Where(data => (data.ItemQuantity > 0)).ToArray();

			// Create cart and recalculate cart because this cart has item changed
			var cart = CartObject.CreateCartByOrder(order);
			cart.SetPriceShipping(order.OrderPriceShipping);
			cart.Payment.PriceExchange = order.OrderPriceExchange;
			cart.PriceRegulation = orderNew.LastBilledAmount - cart.PriceTotal;
			cart.Calculate(false, true);
			return cart;
		}

		/// <summary>
		/// Get Error Messages
		/// </summary>
		/// <param name="errorCode">Error Code</param>
		/// <returns>Error message</returns>
		public static string GetErrorMessages(string errorCode)
		{
			if (string.IsNullOrEmpty(errorCode)) return string.Empty;
			var document = XDocument.Parse(Properties.Resources.NPAfterPayErrorMessages);
			var errorMessage = document.Root.Elements("Message")
				.Where(element => element.Attributes("code").First().Value == errorCode)
				.Select(element => element.Value).FirstOrDefault()
				?? (string)document.Root
					.Elements("Message")
					.FirstOrDefault(x => x.Attributes("code").First().Value
						== Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_UNEXPECTED_ERROR);
			return errorMessage;
		}

		/// <summary>
		/// 外部連携ステータスが入金済みかどうか
		/// </summary>
		/// <param name="externalPaymentStatus">外部連携ステータス</param>
		/// <returns>true：入金済み、false：未入金</returns>
		public static bool CheckIfExternalPaymentStatusHasBeenPaid(string externalPaymentStatus)
		{
			if ((externalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
				|| (externalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
				|| (externalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP)
				|| (externalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)) return true;

			return false;
		}

		/// <summary>返品交換画面からの遷移か</summary>
		public static bool IsReturnOrExchange { get; set; }
		#endregion
	}
}
