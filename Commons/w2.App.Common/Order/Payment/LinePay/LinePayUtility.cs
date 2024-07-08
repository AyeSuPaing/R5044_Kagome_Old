/*
=========================================================================================================
  Module      : Line Pay Utility (LinePayUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Option;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.LinePay
{
	/// <summary>
	/// LINE Pay Utility
	/// </summary>
	public class LinePayUtility
	{
		#region Constants
		/// <summary>API call back: Confirm Order</summary>
		public const string API_CALLBACK_REQUEST_FOR_ORDER = "1";
		/// <summary>API call back: Confirm Order</summary>
		public const string API_CALLBACK_CONFIRM_FOR_ORDER = "2";
		/// <summary>API call back: Confirm Order</summary>
		public const string API_CALLBACK_REQUEST_FOR_MODIFY = "3";
		/// <summary>API call back: Cancel Order</summary>
		public const string API_CALLBACK_CANCEL_ORDER = "E";
		/// <summary>Pay type: Preapproved</summary>
		private const string PAY_TYPE_PREAPPROVED = "PREAPPROVED";
		/// <summary>Package default ID</summary>
		private const string PACKAGE_DEFAULT_ID = "1";
		/// <summary>Shipping type: fixed address</summary>
		private const string SHIPPING_TYPE_FIXED_ADDRESS = "FIXED_ADDRESS";
		/// <summary>Shipping type: no shipping</summary>
		private const string SHIPPING_TYPE_NO_SHIPPING = "NO_SHIPPING";
		/// <summary>Shipping fee inquiry type: fixed</summary>
		private const string SHIPPING_FEE_INQUIRY_TYPE_FIXED = "FIXED";
		#endregion

		#region Methods
		/// <summary>
		/// Create request payment
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="redurectUrls">リダイレクトURL</param>
		/// <returns>A request payment object</returns>
		public static LinePayRequestPayment CreateRequestPayment(CartObject cart, RedirectUrls redurectUrls)
		{
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			var isFreeShipping = (cart.IsFreeShipping()
				|| cart.IsFreeShippingCouponUse()
				|| ((cart.SetPromotions != null)
					&& cart.SetPromotions.IsShippingChargeFree));

			var shippingFee = (isFreeShipping
				? 0
				: (int)cart.PriceShipping);
			var products = CreateProducts(cart, shippingFee);
			var priceTotal = (int)cart.SettlementAmount;

			var request = new LinePayRequestPayment
			{
				Amount = priceTotal,
				Currency = cart.SettlementCurrency,
				PaymentOrderId = paymentOrderId,
				Packages = CreatePackages(products, priceTotal, shippingFee),
				Options = CreateOptions(cart, shippingFee),
				RedirectUrls = redurectUrls,
			};

			return request;
		}

		/// <summary>
		/// Create options
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="shippingFee">Shipping fee</param>
		/// <returns>An options object</returns>
		private static Options CreateOptions(CartObject cart, int shippingFee)
		{
			var options = new Options
			{
				Payment = new Payment
				{
					Capture = Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW,
					PayType = PAY_TYPE_PREAPPROVED
				},
				Shipping = new Shipping
				{
					Type = (IsMerchantCountryJapan
						? ((shippingFee == 0)
							? SHIPPING_TYPE_NO_SHIPPING
							: SHIPPING_TYPE_FIXED_ADDRESS)
						: SHIPPING_TYPE_NO_SHIPPING),
					FeeAmount = shippingFee,
					FeeInquiryType = SHIPPING_FEE_INQUIRY_TYPE_FIXED,
					Address = new Address
					{
						Country = (Constants.GLOBAL_OPTION_ENABLE)
							? cart.Owner.AddrCountryIsoCode
							: Constants.COUNTRY_ISO_CODE_JP,
						PostalCode = cart.Owner.Zip,
						State = cart.Owner.Addr1,
						City = cart.Owner.Addr2,
						Detail = cart.Owner.Addr3,
						Optional = string.Format("{0}{1}", cart.Owner.Addr4, cart.Owner.Addr5),
						Recipient = new Recipient
						{
							FirstName = cart.Owner.Name1,
							LastName = cart.Owner.Name2,
							FirstNameOptional = cart.Owner.NameKana1,
							LastNameOptional = cart.Owner.NameKana2,
							Email = cart.Owner.MailAddr,
							PhoneNo = cart.Owner.Tel1,
						},
					}
				},
				Display = new Display
				{
					CheckConfirmUrlBrowser = true,
				},
			};
			return options;
		}

		/// <summary>
		/// Create package
		/// </summary>
		/// <param name="products">A collection of products</param>
		/// <param name="amount">Total amount of products</param>
		/// <returns>A package object</returns>
		private static Package CreatePackage(IEnumerable<Product> products, int amount)
		{
			var package = new Package
			{
				Id = PACKAGE_DEFAULT_ID,
				Amount = amount,
				Products = products.ToArray()
			};

			return package;
		}

		/// <summary>
		/// Create packages
		/// </summary>
		/// <param name="products">A collection of products</param>
		/// <param name="priceTotal">A price total of order</param>
		/// <param name="shippingFee">A shipping fee</param>
		/// <returns>A collection of packages</returns>
		private static Package[] CreatePackages(IEnumerable<Product> products, int priceTotal, int shippingFee)
		{
			var totalAmountOfProducts = GetTotalAmount(products);
			var feeAndDiscountAmount = (priceTotal - totalAmountOfProducts);
			var amount = (totalAmountOfProducts + feeAndDiscountAmount);

			// APIのoptions.shippingが利用する場合、商品合計金額に配送料を引く（日本以外の場合はoptions.shippingが利用不可ので配送料も商品合計金額に入る）
			if (IsMerchantCountryJapan)
			{
				amount -= shippingFee;
			}

			var packages = new Package[]
			{
				CreatePackage(products, amount)
			};

			return packages;
		}

		/// <summary>
		/// Create products
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="shippingFee">Shipping fee</param>
		/// <returns>A collection of LINE Pay product objects</returns>
		private static List<Product> CreateProducts(CartObject cart, int shippingFee)
		{
			// Calculate member rank discount per product
			var memberRankItems = cart.Items.Where(item => item.IsMemberRankDiscount).ToList();
			var totalMemberRankProductQuantity = memberRankItems.Sum(product => product.Count);
			var memberRankDiscount = (int)cart.MemberRankDiscount;
			var memberRankDiscountPerProduct = GetDiscountPricePerProduct(
				memberRankDiscount,
				totalMemberRankProductQuantity);
			var totalFeesAndDiscountsPriceWithoutShipping = cart.PriceTotal
				- TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax)
				- shippingFee;

			// Calculate fee and discount per product
			var feeAndDiscount = (int)(totalFeesAndDiscountsPriceWithoutShipping
				+ cart.MemberRankDiscount
				+ cart.SetPromotions.ProductDiscountAmount
				+ (cart.SettlementAmount - cart.PriceTotal));

			var totalProductQuantity = cart.Items.Sum(product => product.Count);
			var feeAndDiscountPerProduct = GetDiscountPricePerProduct(feeAndDiscount, totalProductQuantity);

			// Create LINE Pay products
			var result = CreateProducts(cart, feeAndDiscountPerProduct, memberRankDiscountPerProduct);

			// Handle remainder
			if (result.Count > 0)
			{
				var memberRankItem = memberRankItems.LastOrDefault();
				var lastMemberRankProductId = (memberRankItem != null)
					? memberRankItem.ProductId
					: string.Empty;
				var memberRankDiscountRemainder = GetDiscountRemainderPrice(
					memberRankDiscount,
					totalMemberRankProductQuantity);

				HandleMemberRankDiscountRemainder(result, memberRankDiscountRemainder, lastMemberRankProductId);

				// Fee and discount remainder
				var remainder = GetDiscountRemainderPrice(feeAndDiscount, totalProductQuantity);
				HandleFeeAndDiscountRemainder(result, remainder);
			}

			return result;
		}

		/// <summary>
		/// Create products
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="feeAndDiscountPerProduct">Fee and discount amount per product</param>
		/// <param name="memberRankDiscountPerProduct">Member discount amount per product</param>
		/// <returns>A collection of LINE Pay product objects</returns>
		private static List<Product> CreateProducts(
			CartObject cart,
			int feeAndDiscountPerProduct,
			int memberRankDiscountPerProduct)
		{
			var products = CreateNormalProducts(
				cart,
				feeAndDiscountPerProduct,
				memberRankDiscountPerProduct);
			var setPromotionProducts = CreateSetPromotionProducts(
				cart,
				feeAndDiscountPerProduct,
				memberRankDiscountPerProduct);

			products.AddRange(setPromotionProducts);

			// 日本LINEPayではない場合、APIのoptions.shippingが利用不可のため、配送料を仮商品として追加する
			if ((IsMerchantCountryJapan == false) && (cart.PriceShipping > 0))
			{
				var dummyShippingFeeProduct = CreateDummyShippingFeeProduct(cart);
				products.Add(dummyShippingFeeProduct);
			}

			return products;
		}

		/// <summary>
		/// Create normal products
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="feeAndDiscountPerProduct">Fee and discount amount per product</param>
		/// <param name="memberRankDiscountPerProduct">Member discount amount per product</param>
		/// <returns>A collection of LINE Pay product objects</returns>
		private static List<Product> CreateNormalProducts(
			CartObject cart,
			int feeAndDiscountPerProduct,
			int memberRankDiscountPerProduct)
		{
			var items = new List<Product>();
			foreach (var cartProduct in cart.Items.Where(cartProduct => (cartProduct.QuantitiyUnallocatedToSet != 0)))
			{

				// Create LINE Pay product object
				var quantity = cartProduct.IsSetItem
					? cartProduct.Count
					: cartProduct.QuantitiyUnallocatedToSet;

				var productPrice = GetProductPrice(
					cartProduct,
					feeAndDiscountPerProduct,
					memberRankDiscountPerProduct);

				var item = CreateProduct(cartProduct, quantity, productPrice);

				items.Add(item);
			}

			return items;
		}

		/// <summary>
		/// Create set promotion products
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="feeAndDiscountPerProduct">Fee and discount amount per product</param>
		/// <param name="memberRankDiscountPerProduct">Member discount amount per product</param>
		/// <returns>A collection of LINE Pay product objects</returns>
		private static List<Product> CreateSetPromotionProducts(
			CartObject cart,
			int feeAndDiscountPerProduct,
			int memberRankDiscountPerProduct)
		{
			var result = new List<Product>();
			foreach (var setpromotion in cart.SetPromotions.Items)
			{
				// Calculate set promotion discount per product and remainder
				var totalSetPromotionProductQuantity = setpromotion.Items.Sum(product =>
					product.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo]);

				var setPromotionDiscountPerProduct = GetDiscountPricePerProduct(
					(int)setpromotion.ProductDiscountAmount,
					totalSetPromotionProductQuantity);
				var setPromotionDiscountRemainder = GetDiscountRemainderPrice(
					(int)setpromotion.ProductDiscountAmount,
					totalSetPromotionProductQuantity);
				var hasRemainder = (setPromotionDiscountRemainder != 0);

				// Create LINE Pay products
				var products = CreateSetPromotionProducts(
					setpromotion,
					(feeAndDiscountPerProduct - setPromotionDiscountPerProduct),
					memberRankDiscountPerProduct,
					hasRemainder);
				result.AddRange(products);

				// Hanlde set promotion remainder
				if (hasRemainder == false) continue;
				var cartProduct = setpromotion.Items[setpromotion.Items.Count - 1];
				var quantity = cartProduct.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo];
				var productPrice = GetProductPrice(
					cartProduct,
					(feeAndDiscountPerProduct - setPromotionDiscountPerProduct),
					memberRankDiscountPerProduct);
				if (quantity > 1)
				{
					var item = CreateProduct(
						cartProduct,
						(quantity - 1),
						productPrice);
					result.Add(item);
				}
				var lastItem = CreateProduct(
					cartProduct,
					1,
					(productPrice - setPromotionDiscountRemainder));
				result.Add(lastItem);
			}

			return result;
		}

		/// <summary>
		/// Create set promotion products
		/// </summary>
		/// <param name="setpromotion">Cart set promotion</param>
		/// <param name="feeAndDiscountPerProduct">Fee and discount amount per product</param>
		/// <param name="memberRankDiscountPerProduct">Member discount amount per product</param>
		/// <param name="hasRemainder">Has remainder of fee and discount per product</param>
		/// <returns>A collection of LINE Pay product objects</returns>
		private static List<Product> CreateSetPromotionProducts(
			CartSetPromotion setpromotion,
			int feeAndDiscountPerProduct,
			int memberRankDiscountPerProduct,
			bool hasRemainder)
		{
			var count = setpromotion.Items.Count;
			var lastIndex = (count - 1);

			var result = new List<Product>();
			for (var index = 0; index < count; index++)
			{
				// Don't add last item if has remainder
				if ((index == lastIndex) && hasRemainder) break;

				// Create LINE Pay product object
				var cartProduct = setpromotion.Items[index];
				var quantity = cartProduct.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo];
				var productPrice = GetProductPrice(
					cartProduct,
					feeAndDiscountPerProduct,
					memberRankDiscountPerProduct);

				var item = CreateProduct(cartProduct, quantity, productPrice);

				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// 配送料を仮商品として一つを追加する
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>配送料で作った仮商品</returns>
		private static Product CreateDummyShippingFeeProduct(CartObject cart)
		{
			var result = new Product
			{
				Id = "ShippingFee",
				ImageUrl = string.Empty,
				Name = "Shipping Fee",
				Price = (int)cart.PriceShipping,
				Quantity = 1
			};

			return result;
		}

		/// <summary>
		/// Create product object
		/// </summary>
		/// <param name="cartProduct">The cart product</param>
		/// <param name="quantity">A product quantity</param>
		/// <param name="price">A product price</param>
		/// <returns>A LINE Pay product object</returns>
		private static Product CreateProduct(
			CartProduct cartProduct,
			int quantity,
			int price)
		{
			var result = new Product
			{
				Id = cartProduct.ProductId,
				ImageUrl = string.Empty,
				Name = cartProduct.ProductJointName,
				Price = price,
				Quantity = quantity
			};

			return result;
		}


		/// <summary>
		/// Create product object
		/// </summary>
		/// <param name="product">The cart product</param>
		/// <param name="quantity">A product quantity</param>
		/// <param name="price">A product price</param>
		/// <returns>A LINE Pay product object</returns>
		private static Product CreateProduct(
			Product product,
			int quantity,
			int price)
		{
			var result = new Product
			{
				Id = product.Id,
				ImageUrl = string.Empty,
				Name = product.Name,
				Price = price,
				Quantity = quantity
			};

			return result;
		}

		/// <summary>
		/// Handle member rank discount remainder
		/// </summary>
		/// <param name="products">A collection of LINE Pay product objects</param>
		/// <param name="remainder">The member rank discount remainder</param>
		/// <param name="lastMemberRankProductId">Last member rank product ID</param>
		private static void HandleMemberRankDiscountRemainder(
			List<Product> products,
			int remainder,
			string lastMemberRankProductId)
		{
			if ((remainder == 0) || string.IsNullOrEmpty(lastMemberRankProductId)) return;

			var lastIndex = products.FindLastIndex(item => (item.Id == lastMemberRankProductId));

			if (lastIndex < 0) return;

			var lastProduct = products[lastIndex];
			products.RemoveAt(lastIndex);

			if (lastProduct.Quantity > 1)
			{
				products.Add(CreateProduct(
					lastProduct,
					(lastProduct.Quantity - 1),
					lastProduct.Price));
			}

			products.Add(CreateProduct(
				lastProduct,
				1,
				(lastProduct.Price - remainder)));
		}

		/// <summary>
		/// Handle fee and discount remainder
		/// </summary>
		/// <param name="products">A collection of LINE Pay product objects</param>
		/// <param name="remainder">The fee and discount remainder</param>
		private static void HandleFeeAndDiscountRemainder(List<Product> products, int remainder)
		{
			if (remainder == 0) return;

			var lastProduct = products.LastOrDefault();

			if (lastProduct == null) return;

			products.Remove(lastProduct);

			if (lastProduct.Quantity > 1)
			{
				products.Add(CreateProduct(
					lastProduct,
					(lastProduct.Quantity - 1),
					lastProduct.Price));
			}

			products.Add(CreateProduct(
				lastProduct,
				1,
				(lastProduct.Price + remainder)));
		}

		/// <summary>
		/// Get product price
		/// </summary>
		/// <param name="cartProduct">The cart product</param>
		/// <param name="feeAndDiscount">Fee and discount amount</param>
		/// <param name="memberRankDiscount">Member discount amount</param>
		/// <returns>An product price</returns>
		private static int GetProductPrice(
			CartProduct cartProduct,
			int feeAndDiscount,
			int memberRankDiscount)
		{
			var result = (int)cartProduct.Price
				+ feeAndDiscount
				- (cartProduct.IsMemberRankDiscount ? memberRankDiscount : 0);

			return result;
		}

		/// <summary>
		/// Get discount remainder price
		/// </summary>
		/// <param name="discountPrice">A discount price</param>
		/// <param name="totalProduct">A total product count</param>
		/// <returns>A discount remainder price</returns>
		private static int GetDiscountRemainderPrice(int discountPrice, int totalProduct)
		{
			var result = (totalProduct != 0)
				? (discountPrice % totalProduct)
				: 0;

			return result;
		}

		/// <summary>
		/// Get discount price per product
		/// </summary>
		/// <param name="discountPrice">A discount price</param>
		/// <param name="totalProduct">A total product count</param>
		/// <returns>A discount price per product</returns>
		private static int GetDiscountPricePerProduct(int discountPrice, int totalProduct)
		{
			var result = (totalProduct != 0)
				? (discountPrice / totalProduct)
				: 0;

			return result;
		}

		/// <summary>
		/// Get total amount
		/// </summary>
		/// <param name="products">A collection of products</param>
		/// <returns>A total amount</returns>
		private static int GetTotalAmount(IEnumerable<Product> products)
		{
			var amount = products.Sum(product => (product.Quantity * product.Price));

			return amount;
		}

		/// <summary>
		/// Create Confirm Request
		/// </summary>
		/// <param name="order">Order data</param>
		/// <returns>Line Pay Confirm Payment Request</returns>
		public static LinePayConfirmPaymentRequest CreateConfirmRequest(OrderModel order)
		{
			var result =CreateConfirmRequest(
				order.SettlementAmount,
				order.SettlementCurrency);
			return result;
		}
		/// <summary>
		/// Create Confirm Request
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <returns>Line Pay Confirm Payment Request</returns>
		public static LinePayConfirmPaymentRequest CreateConfirmRequest(CartObject cart)
		{
			var result = CreateConfirmRequest(
				cart.SettlementAmount,
				cart.SettlementCurrency);
			return result;
		}
		/// <summary>
		/// Create Confirm Request
		/// </summary>
		/// <param name="amount">Amount</param>
		/// <param name="currency">Currency</param>
		/// <returns>Line Pay Confirm Payment Request</returns>
		private static LinePayConfirmPaymentRequest CreateConfirmRequest(decimal amount, string currency)
		{
			var result = new LinePayConfirmPaymentRequest
			{
				Amount = amount,
				Currency = currency,
			};
			return result;
		}
		#endregion

		/// <summary>日本のLINEPayであるか</summary>
		private static bool IsMerchantCountryJapan
		{
			get{ return (Constants.PAYMENT_LINEPAY_MERCHANT_COUNTRY == "JP"); }
		}
	}
}
