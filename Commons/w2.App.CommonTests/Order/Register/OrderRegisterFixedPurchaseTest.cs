using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Register;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests.Order.Cart;
using w2.Common.Sql;
using w2.Common.Util;
using w2.CommonTests._Helper;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ShopShipping;

namespace w2.App.CommonTests.Order.Register
{
	[TestClass()]
	public class OrderRegisterFixedPurchaseTest : AppTestClassBase
	{
		/// <summary>
		/// 注文チェックテスト
		/// </summary>
		/// <param name="config">コンフィグ設定</param>
		/// <param name="data">データ</param>
		/// <param name="expected">期待結果</param>
		/// <param name="msg">メッセージ</param>
		[DataTestMethod()]
		[DynamicData("TdCheckForOrderTest")]
		public void CheckForOrderTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg
			)
		{
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), config.GlobalOptionEnable))
			{
				var orderHt = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, "N01" },
					{ Constants.FIELD_ORDER_USER_ID, "U01" },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, "P01" }
				};

				var cart = CartTestHelper.CreateCart();
				cart.Owner = data.TestCartOwner;
				var cartShipping = new CartShipping(cart);
				cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);
				cartShipping.UpdateCartPriceShipping((decimal)data.PriceShipping);
				cart.Shippings[0] = cartShipping;
				cart.Items.Add(CartTestHelper.CreateCartProduct());
				cart.Payment = CartTestHelper.CreateCartPayment(
					paymentId : data.PaymentId,
					priceExchange : data.PriceExchange);

				typeof(CartObject).GetProperty("PriceSubtotal", BindingFlags.Instance | BindingFlags.Public)
					.SetValue(cart, data.PriceSubtotal);

				// 支払方法と上下限金額設定するモック
				var mockPayment = new Mock<IPaymentService>();
				mockPayment.Setup(s => s.Get(It.IsAny<string>(),It.IsAny<string>())).Returns(new PaymentModel()
				{
					PaymentId = data.PaymentId,
					UsablePriceMax = data.PaymentMaxAmount,
					UsablePriceMin = data.PaymentMinAmount,
				});
				Domain.DomainFacade.Instance.PaymentService = mockPayment.Object;

				// 商品取得するモック
				var productDr = CartTestHelper.CreateProductDataRowView();
				productDr[Constants.FIELD_PRODUCTSTOCK_STOCK] = data.ProductStockCount;
				productDr[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = data.ProductStockManagementKbn;
				
				var mockProduct = new Mock<IProductService>();
				mockProduct.Setup(s => s.GetProductVariationAtDataRowView(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>())).Returns(productDr);
				Domain.DomainFacade.Instance.ProductService = mockProduct.Object;

				// 配送不可エリアモック(沖縄：9010154)
				var mockShippingZip = new Mock<IShopShippingService>();
				var returnsResult = mockShippingZip.Setup(s => s.GetUnavailableShippingZipFromShippingDelivery(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<SqlAccessor>())).Returns("9010154");
				Domain.DomainFacade.Instance.ShopShippingService = mockShippingZip.Object;

				var fixedPurchaseMailSendingTime = new FixedPurchaseMailSendTiming(string.Empty);
				var orderRegisterFixedPurchase = new OrderRegisterFixedPurchase(
					lastChanged: "",
					canSendFixedPurchaseMailToUser : true,
					canUpdateShippingDate : true,
					fixedPurchaseMailSendTiming : fixedPurchaseMailSendingTime);
				var orderRegitserFixedPurchaseInner = new OrderRegisterFixedPurchaseInner(
					isUser: true,
					lastChanged: "",
					canSendFixedPurchaseMailToUser: true,
					fixedPurchaseId: "FP001",
					fixedPurchaseMailSendTiming: fixedPurchaseMailSendingTime);

				((bool)new PrivateObject(orderRegisterFixedPurchase)
					.Invoke("CheckForOrder",args : new object[] { orderHt, cart, orderRegitserFixedPurchaseInner, (bool)data.IsMypage }))
					.Should().Be(expected.ExcuteResult, msg);

				orderRegitserFixedPurchaseInner.ErrorMessages.Should().BeEquivalentTo(expected.ErrorMessage);
				orderRegitserFixedPurchaseInner.FixedPurchaseStatus.Should().Be(expected.FixedPurchaseStatus);
				orderRegitserFixedPurchaseInner.FixedPurchaseHistoryKbn.Should().Be(expected.FixedPurchaseHistoryKbn);
			}
		}

		/// <summary>
		/// テストデータ
		/// </summary>
		private static IEnumerable<object[]> TdCheckForOrderTest => new List<object[]>
		{
		
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"通常成功パターン(クレジットカード)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { OrderCommon.GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOutOfRangeError,
						StringUtility.ToPrice(1100m),
						StringUtility.ToPrice(100m),
						StringUtility.ToPrice(1000m)) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				}
				,
				"決済上限金額超えたパターン(クレジットカード)"
			}, 
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 10m,
					PriceExchange = 10m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { OrderCommon.GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOutOfRangeError,
						StringUtility.ToPrice(20m),
						StringUtility.ToPrice(100m),
						StringUtility.ToPrice(1000m)) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				}
				,
				"決済下限金額未満パターン(クレジットカード)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 900m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"決済上限金額同じパターン(クレジットカード)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 90m,
					PriceExchange = 10m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"決済下限金額同じパターン(クレジットカード)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 0m,
					PriceExchange = 0m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { OrderCommon.GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOverUnselectableError,
						StringUtility.ToPrice(1m) + " 以上") },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				}
				,
				"決済金額0円パターン(クレジットカード)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 0m,
					PriceExchange = 0m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"決済金額0円パターン(決済無し)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"定期利用可キャリア決済(SBPS)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXED_PURCHASE_PAYMENT_CAREER_ERROR) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
				}
				,
				"定期利不可キャリア決済(ドコモケータイ払い)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = true,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(
						addrCountryIsoCode :  Constants.COUNTRY_ISO_CODE_TW),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				}
				,
				"Paidy決済の日本住所のチェック(台湾)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = true,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"Paidy決済の日本住所のチェック(日本)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(
						addrCountryIsoCode :  Constants.COUNTRY_ISO_CODE_TW),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"Paidy決済の日本住所のチェック(グローバルオプションOFF台湾)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = true,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(
						addrCountryIsoCode :  Constants.COUNTRY_ISO_CODE_TW),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				}
				,
				"Np後払い決済の日本住所のチェック(台湾)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = true,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"Np後払い決済の日本住所のチェック(日本)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY,
					PaymentMaxAmount = 9999m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 1000m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(
						addrCountryIsoCode :  Constants.COUNTRY_ISO_CODE_TW),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"Np後払い決済の日本住所のチェック(グローバルオプションOFF台湾)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = -1,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"在庫管理チェック(在庫管理なし＋在庫なし)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = -1,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = true,
					ErrorMessage = new object[] { },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				}
				,
				"在庫管理チェック(在庫管理購入可能＋在庫なし)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = -1,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK_ORDER_NOW_FROM_MYPAGE)
							.Replace("@@ 1 @@", string.Empty) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_NOSTOCK,
				}
				,
				"在庫管理チェック(在庫管理購入不可＋在庫なし)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = -1,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG,
					TestCartOwner = CartTestHelper.CreateCartOwner(),
					IsMypage = false,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { CommerceMessages.GetMessages(
						OrderCommon.GetErrorMessage(OrderErrorcode.ProductNoStock, string.Empty)) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_NOSTOCK,
				}
				,
				"在庫管理チェック(在庫管理購入不可＋在庫なし+マイページ以外)"
			},
			new object[]
			{
				new
				{
					GlobalOptionEnable = false,
				},
				new
				{
					PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentMaxAmount = 1000m,
					PaymentMinAmount = 100m,
					PriceSubtotal = 800m,
					PriceExchange = 100m,
					PriceShipping = 0m,
					ProductStockCount = 100,
					ProductStockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED,
					TestCartOwner = CartTestHelper.CreateCartOwner(zip: "9010154"),
					IsMypage = true,
				},
				new
				{
					ExcuteResult = false,
					ErrorMessage = new object[] { CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR) },
					FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA,
					FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_UNAVAILABLE_SHIPPING_AREA,
				}
				,
				"配送不可エリアチェック(沖縄)"
			},
		};
	}
}
