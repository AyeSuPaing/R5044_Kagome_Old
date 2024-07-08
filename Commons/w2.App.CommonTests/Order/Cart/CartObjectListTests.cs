using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests._Helper.DataCacheConfigurator;
using w2.Common.Sql;
using w2.Common.Wrapper;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.MemberRank;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ProductTag;
using w2.Domain.ShopShipping;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Order.Cart
{
	/// <summary>
	/// CartObjectListのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class CartObjectListTests : AppTestClassBase
	{
		///// <summary>
		///// ユーザーデフォルト注文設定のチェック(全てのカート)テスト
		///// ・引数のユーザーデフォルト注文設定の決済種別情報と、各カートのCartPayment情報から利用可否をチェックしエラーメッセージのキーを返すこと
		///// 　エラーメッセージは以下の通りになること
		///// 　　・TriLink決済利用不可の場合：ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2
		///// 　　・利用不可能決済種別指定の場合：ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT
		///// ・ユーザーデフォルト注文設定の決済種別が「ECPAY」「NEWEBPAY」の場合、CartPayment.ExternalPaymentTypeにデフォルト値が設定されること
		///// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCheckDefaultOrderSettingAllCartTest")]
		//public void CheckDefaultOrderSettingAllCartTest(
		//	dynamic config,
		//	dynamic data,
		//	dynamic expected,
		//	string msg)
		//{
		//			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), true))
		//			using (new TestConfigurator(Member.Of(() => Constants.ECPAY_PAYMENT_OPTION_ENABLED), true))
		//			using (new TestConfigurator(Member.Of(() => Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED), true))
		//			using (new TestConfigurator(Member.Of(() => Constants.TWOCLICK_OPTION_ENABLE), config.TwoClickOptionEnable))
		//			using (new ShopShippingDataCacheConfigurator(new ShopShippingModel[0]))
		//			using (new MemberRankDataCacheConfigurator(new MemberRankModel[0]))
		//			using (new PaymentDataCacheConfigurator((PaymentModel[])data.ValidPaymentCachData))
		//			{
		//				var userDefaultOrderSettingServiceMock = new Mock<IUserDefaultOrderSettingService>();
		//				userDefaultOrderSettingServiceMock
		//					.Setup(s => s.Get(
		//						It.IsAny<string>(),
		//						It.IsAny<SqlAccessor>()))
		//					.Returns((UserDefaultOrderSettingModel)data.UserDefaultOrderSettingModel);
		//				DomainFacade.Instance.UserDefaultOrderSettingService = userDefaultOrderSettingServiceMock.Object;

		//				var userServiceMock = new Mock<IUserService>();
		//				userServiceMock
		//					.Setup(s => s.Get(
		//						It.IsAny<string>(),
		//						It.IsAny<SqlAccessor>()))
		//					.Returns(new UserModel());
		//				DomainFacade.Instance.UserService = userServiceMock.Object;

		//				var deliveryCompanyServiceMock = new Mock<IDeliveryCompanyService>();
		//				deliveryCompanyServiceMock
		//					.Setup(s => s.Get(It.IsAny<string>()))
		//					.Returns(new DeliveryCompanyModel());
		//				DomainFacade.Instance.DeliveryCompanyService = deliveryCompanyServiceMock.Object;

		//				var productServiceMock = new Mock<IProductService>();
		//				productServiceMock
		//					.Setup(s => s.GetCartProducts(It.IsAny<string>()))
		//					.Returns(new []
		//					{
		//						new ProductModel
		//						{
		//							LimitedPaymentIds = ""
		//						}
		//					});
		//				DomainFacade.Instance.ProductService = productServiceMock.Object;

		//				var productTagServiceMock = new Mock<IProductTagService>();
		//				productTagServiceMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
		//				DomainFacade.Instance.ProductTagService = productTagServiceMock.Object;

		//				var nameTranslationSettingServiceMock = new Mock<INameTranslationSettingService>();
		//				nameTranslationSettingServiceMock
		//					.Setup(s => s.GetTranslationSettingsByMultipleMasterId1(It.IsAny<NameTranslationSettingSearchCondition>()))
		//					.Returns(new NameTranslationSettingModel[0]);
		//				DomainFacade.Instance.NameTranslationSettingService = nameTranslationSettingServiceMock.Object;

		//				var cartList = new CartObjectList(
		//					"test001",
		//					Constants.FLG_ORDER_ORDER_KBN_PC,
		//					false,
		//					"",
		//					"Rank001");
		//				foreach (var cartParam in data.CartParams)
		//				{
		//					var cart = CartTestHelper.CreateCart();

		//					cart.Owner = CartTestHelper.CreateCartOwner();
		//					cart.Owner.AddrCountryIsoCode = cartParam.OwnerAddrCountryIsoCode;
		//					cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
		//					cart.GetShipping().ShippingCountryIsoCode = cartParam.ShippingCountryIsoCode;
		//					cart.GetShipping().DeliveryCompanyId = "test001";
		//					cart.Items.Add(CartTestHelper.CreateCartProduct());
		//					cart.Payment = cartParam.CreateCartPaymentFunc();

		//					cartList.AddCartVirtural(cart);
		//				}
		//				cartList.CalculateAllCart();

		//				var result = cartList.CheckDefaultOrderSettingAllCart("test001");

		//				// 商品金額合計再計算：配送先一つのみ
		//				// ・商品単価と購入個数からカート商品小計が計算されること
		//				// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
		//				// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
		//				var index = 0;
		//				foreach (var cart in cartList.Items.Where(cart => (cart.Payment != null)))
		//				{
		//					var externalPaymentTypeExpected = expected.ExternalPaymentTypeExpectedList[index];
		//					cart.Payment.ExternalPaymentType.Should().Be((string)externalPaymentTypeExpected,
		//						msg + " : カート決済方法：ExternalPaymentType");
		//					index++;
		//				}

		//				result.Should().Be((string)expected.ErrorMessageKey, msg + "エラーメッセージキー");
		//			}
		//}

		public static object[] m_tdCheckDefaultOrderSettingAllCartTest = new[]
		{
			// 2クリック決済オプションOFF
			new object[]
			{
				new
				{
					TwoClickOptionEnable = false
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = "",
				},
				"2クリック決済オプションOFF"
			},
			// ユーザーデフォルト注文設定：NULL
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = (UserDefaultOrderSettingModel)null,
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = "",
				},
				"ユーザーデフォルト注文設定：NULL"
			},
			// カート決済情報：NULL
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() => null)
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = "",
				},
				"カート決済方法：NULL"
			},
			// 後付款(TriLink後払い)が利用可能判定
			// 利用可能
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = "",
				},
				"後付款(TriLink後払い)利用可能"
			},
			// 後付款(TriLink後払い)が利用可能判定
			// 利用不可能
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2,
				},
				"後付款(TriLink後払い)利用不可能"
			},
			// デフォルト外部決済種別設定処理：ECPAY
			// 外部決済種別未設定
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT,
					},
					ErrorMessageKey = "",
				},
				"外部決済種別設定処理:外部決済種別未設定:ECPAY"
			},
			// デフォルト外部決済種別設定処理：ECPAY
			// 外部決済種別設定済み
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
								cartPayment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_ECPAY_WEBATM;
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						Constants.FLG_PAYMENT_TYPE_ECPAY_WEBATM,
					},
					ErrorMessageKey = "",
				},
				"外部決済種別設定処理:外部決済種別設定済み:ECPAY"
			},
			// デフォルト外部決済種別設定処理：NEWEBPAY
			// 外部決済種別未設定
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT,
					},
					ErrorMessageKey = "",
				},
				"外部決済種別設定処理:外部決済種別未設定:NEWEBPAY"
			},
			// デフォルト外部決済種別設定処理：NEWEBPAY
			// 外部決済種別未設定
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY);
								cartPayment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM;
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM,
					},
					ErrorMessageKey = "",
				},
				"外部決済種別設定処理:外部決済種別設定済み:NEWEBPAY"
			},
			// デフォルト決済種別利用可能判定
			// 決済種別利用可能〇
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}
						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = "",
				},
				"デフォルト決済種別利用可能判定：〇"
			},
			// デフォルト決済種別利用可能判定
			// 決済種別利用可能✕
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CartParams = new[]
					{
						new
						{
							OwnerAddrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							CreateCartPaymentFunc = new Func<CartPayment>(() =>
							{
								var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
								cartPayment.ExternalPaymentType = "";
								return cartPayment;
							})
						},
					},
				},
				new
				{
					ExternalPaymentTypeExpectedList = new[]
					{
						"",
					},
					ErrorMessageKey = CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT,
				},
				"デフォルト決済種別利用可能判定：×"
			},
		};

		/// <summary>
		/// クーポン利用可否のチェック(全てのカート)テスト
		/// ・各カートに以下のいずれかのクーポン情報が存在した場合、falseを返すこと
		/// 　・使用済みクーポン
		/// 　・利用不可クーポン
		/// 　・最新のDBから取得したクーポンマスタ情報と「割引額」「割引率」が異なるクーポン
		/// ・上記に当てはまるクーポン情報が存在しない、もしくはクーポンオプションがOFFの場合は、trueを返すこと
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCheckCouponUseInfoAllCartTest")]
		public void CheckCouponUseInfoAllCartTest(
			dynamic config,
			dynamic data,
			bool expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), config.CouponOptionEnabled))
			using (new TestConfigurator(Member.Of(() => Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE), config.CouponUseUserBlackListCouponUsedUserJudgeType))
			{
				var couponServiceMock = new Mock<ICouponService>();
				couponServiceMock
					.Setup(s => s.GetAllUserCouponsFromCouponId(
						It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<int>()))
					.Returns((UserCouponDetailInfo[])data.DbUserCouponDetailInfo);
				var ownerMailAddress = (data.CartOwner != null)
					? ((CartOwner)data.CartOwner).MailAddr
					: "";
				var mailAddressArg = (string)data.MailAddress;
				couponServiceMock
					.Setup(s => s.CheckUsedCoupon(
						It.IsAny<string>(),
						It.Is<string>(param => (param == ownerMailAddress))))
					.Returns(false);
				couponServiceMock
					.Setup(s => s.CheckUsedCoupon(
						It.IsAny<string>(),
						It.Is<string>(param => (param == mailAddressArg))))
					.Returns(true);
				DomainFacade.Instance.CouponService = couponServiceMock.Object;

				var productTagServiceMock = new Mock<IProductTagService>();
				productTagServiceMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = productTagServiceMock.Object;

				var dateTimeWrapperMock = new Mock<DateTimeWrapper>();
				dateTimeWrapperMock.Setup(s => s.Now).Returns((DateTime)data.DateTimeNow);
				DateTimeWrapper.Instance = dateTimeWrapperMock.Object;

				var cartList = new CartObjectList(
					"test001",
					Constants.FLG_ORDER_ORDER_KBN_PC,
					false,
					"",
					"Rank001");
				foreach (var cartParam in data.CartParams)
				{
					var cart = CartTestHelper.CreateCart();

					cart.Owner = data.CartOwner;
					cart.Items.Add(CartTestHelper.CreateCartProduct());
					cart.Payment = CartTestHelper.CreateCartPayment();
					cart.Coupon = cartParam.CartCoupon;

					cartList.AddCartVirtural(cart);
				}

				var result = cartList.CheckCouponUseInfoAllCart((string)data.UserId, (string)data.MailAddress);

				// 商品金額合計再計算：配送先一つのみ
				// ・商品単価と購入個数からカート商品小計が計算されること
				// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
				// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
				result.Should().Be(expected, msg + "：クーポン利用可否判定");
			}
		}

		public static object[] m_tdCheckCouponUseInfoAllCartTest = new[]
		{
			// クーポンオプション：OFF
			new object[]
			{
				new
				{
					CouponOptionEnabled = false,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
							CouponCount = 1,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 1,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				true,
				"クーポンオプション：OFF"
			},
			// カートのクーポン無し
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[0],
					CartParams = new[]
					{
						new
						{
							CartCoupon = (CartCoupon)null,
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				true,
				"カートのクーポン無し"
			},
			// DBにクーポン情報無し
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[0],
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 1,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				false,
				"DBにクーポン情報無し"
			},
			// 有効なクーポン情報
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
							CouponCount = 1,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 1,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				true,
				"有効なクーポン情報"
			},
			// 無効なクーポン情報
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
							CouponCount = 1,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ValidFlg = Constants.FLG_COUPON_VALID_FLG_INVALID,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 1,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_INVALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				false,
				"無効なクーポン情報"
			},
			// 使用済みクーポン
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
							CouponCount = 0,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 0,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				false,
				"使用済みクーポン"
			},
			// カートクーポン割引額と、最新のクーポン情報の割引額がことなるクーポン情報
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
							CouponCount = 1,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT,
									CouponCount = 1,
									DiscountPrice = 101,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = CartTestHelper.CreateCartOwner(),
				},
				false,
				"カートクーポン割引額と、最新のクーポン情報の割引額がことなるクーポン情報"
			},
			// カートオーナー情報が未設定（エラーとなる引数のメールアドレスが使用される）
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
					CouponUseUserBlackListCouponUsedUserJudgeType = Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS
				},
				new
				{
					UserId = "",
					MailAddress = "test@w2.xyz",
					DateTimeNow = DateTime.Parse("2021/09/01"),
					DbUserCouponDetailInfo = new UserCouponDetailInfo[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER,
							CouponCount = 1,
							DiscountPrice = 100,
							UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
							UserCouponCount = 0,
							UsablePrice = 0,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExpireBgn = DateTime.Parse("2021/08/01"),
							ExpireEnd = DateTime.Parse("2021/09/02"),
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
							ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
							ExceptionalProduct = "test",
						}
					},
					CartParams = new[]
					{
						new
						{
							CartCoupon = new CartCoupon(
								new UserCouponDetailInfo
								{
									CouponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER,
									CouponCount = 1,
									DiscountPrice = 100,
									UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
									UserCouponCount = 0,
									UsablePrice = 0,
									PublishDateBgn = DateTime.Now,
									PublishDateEnd = DateTime.Now,
									DateCreated = DateTime.Now,
									DateChanged = DateTime.Now,
									ExpireBgn = DateTime.Parse("2021/08/01"),
									ExpireEnd = DateTime.Parse("2021/09/02"),
									ExceptionalIcon1 = 0,
									ExceptionalIcon2 = 0,
									ExceptionalIcon3 = 0,
									ExceptionalIcon4 = 0,
									ExceptionalIcon5 = 0,
									ExceptionalIcon6 = 0,
									ExceptionalIcon7 = 0,
									ExceptionalIcon8 = 0,
									ExceptionalIcon9 = 0,
									ExceptionalIcon10 = 0,
									ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET,
									ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID,
									ExceptionalProduct = "test",
								})
						},
					},
					CartOwner = (CartOwner)null,
				},
				false,
				"カートオーナー情報が未設定（エラーとなる引数のメールアドレスが使用される）"
			},
		};
	}
}
