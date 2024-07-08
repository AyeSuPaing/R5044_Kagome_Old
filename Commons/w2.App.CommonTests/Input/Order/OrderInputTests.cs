using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Common.Util;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.MallCooperationSetting;
using w2.Domain.MemberRank;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderInputTests : AppTestClassBase
	{
		private static readonly string[] m_excludedPropertyForUpdateOrder = new string[]
		{
			Constants.FIELD_ORDER_ORDER_ID,
			Constants.FIELD_ORDER_ORDER_ID_ORG,
			Constants.FIELD_ORDER_ORDER_GROUP_ID,
			Constants.FIELD_ORDER_ORDER_NO,
			Constants.FIELD_ORDER_USER_ID,
			Constants.FIELD_ORDER_SHOP_ID,
			Constants.FIELD_ORDER_SUPPLIER_ID,
			Constants.FIELD_ORDER_MALL_ID,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME,
			Constants.FIELD_ORDER_ORDER_STATUS,
			Constants.FIELD_ORDER_ORDER_DATE,
			Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE,
			Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS,
			Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE,
			Constants.FIELD_ORDER_ORDER_SHIPPING_DATE,
			Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS,
			Constants.FIELD_ORDER_ORDER_SHIPPED_DATE,
			Constants.FIELD_ORDER_ORDER_DELIVERING_DATE,
			Constants.FIELD_ORDER_ORDER_CANCEL_DATE,
			Constants.FIELD_ORDER_ORDER_RETURN_DATE,
			Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
			Constants.FIELD_ORDER_ORDER_PAYMENT_DATE,
			Constants.FIELD_ORDER_DEMAND_STATUS,
			Constants.FIELD_ORDER_DEMAND_DATE,
			Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS,
			Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE,
			Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE,
			Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE,
			Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
			Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE,
			Constants.FIELD_ORDER_ORDER_ITEM_COUNT,
			Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT,
			Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL,
			Constants.FIELD_ORDER_ORDER_PRICE_PACK,
			Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT,
			Constants.FIELD_ORDER_ORDER_PRICE_TOTAL,
			Constants.FIELD_ORDER_ORDER_PRICE_TOTAL + "_old",
			Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE,
			Constants.FIELD_ORDER_ORDER_POINT_USE,
			Constants.FIELD_ORDER_ORDER_POINT_USE + "_old",
			Constants.FIELD_ORDER_ORDER_POINT_USE_YEN,
			Constants.FIELD_ORDER_ORDER_POINT_ADD,
			Constants.FIELD_ORDER_ORDER_POINT_RATE,
			Constants.FIELD_ORDER_ORDER_COUPON_USE,
			Constants.FIELD_ORDER_CARD_KBN,
			Constants.FIELD_ORDER_FIXED_PURCHASE_ID,
			Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN,
			Constants.FIELD_ORDER_ATTRIBUTE1,
			Constants.FIELD_ORDER_ATTRIBUTE2,
			Constants.FIELD_ORDER_ATTRIBUTE3,
			Constants.FIELD_ORDER_ATTRIBUTE4,
			Constants.FIELD_ORDER_ATTRIBUTE5,
			Constants.FIELD_ORDER_ATTRIBUTE6,
			Constants.FIELD_ORDER_ATTRIBUTE7,
			Constants.FIELD_ORDER_ATTRIBUTE8,
			Constants.FIELD_ORDER_ATTRIBUTE9,
			Constants.FIELD_ORDER_ATTRIBUTE10,
			Constants.FIELD_ORDER_EXTEND_STATUS1,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE1,
			Constants.FIELD_ORDER_EXTEND_STATUS2,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE2,
			Constants.FIELD_ORDER_EXTEND_STATUS3,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE3,
			Constants.FIELD_ORDER_EXTEND_STATUS4,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE4,
			Constants.FIELD_ORDER_EXTEND_STATUS5,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE5,
			Constants.FIELD_ORDER_EXTEND_STATUS6,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE6,
			Constants.FIELD_ORDER_EXTEND_STATUS7,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE7,
			Constants.FIELD_ORDER_EXTEND_STATUS8,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE8,
			Constants.FIELD_ORDER_EXTEND_STATUS9,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE9,
			Constants.FIELD_ORDER_EXTEND_STATUS10,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE10,
			Constants.FIELD_ORDER_CAREER_ID,
			Constants.FIELD_ORDER_MOBILE_UID,
			Constants.FIELD_ORDER_REMOTE_ADDR,
			Constants.FIELD_ORDER_WRAPPING_MEMO,
			Constants.FIELD_ORDER_DEL_FLG,
			Constants.FIELD_ORDER_DATE_CREATED,
			Constants.FIELD_ORDER_DATE_CHANGED,
			Constants.FIELD_ORDER_LAST_CHANGED,
			Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE,
			Constants.FIELD_ORDER_MEMBER_RANK_ID,
			Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME,
			Constants.FIELD_ORDER_CREDIT_BRANCH_NO,
			Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1,
			Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1,
			Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2,
			Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2,
			Constants.FIELD_ORDER_USER_AGENT,
			Constants.FIELD_ORDER_GIFT_FLG,
			Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG,
			Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID,
			Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL,
			Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY,
			Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG,
			Constants.FIELD_ORDER_ORDER_TAX_RATE,
			Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE,
			Constants.FIELD_ORDER_EXTEND_STATUS11,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE11,
			Constants.FIELD_ORDER_EXTEND_STATUS12,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE12,
			Constants.FIELD_ORDER_EXTEND_STATUS13,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE13,
			Constants.FIELD_ORDER_EXTEND_STATUS14,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE14,
			Constants.FIELD_ORDER_EXTEND_STATUS15,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE15,
			Constants.FIELD_ORDER_EXTEND_STATUS16,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE16,
			Constants.FIELD_ORDER_EXTEND_STATUS17,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE17,
			Constants.FIELD_ORDER_EXTEND_STATUS18,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE18,
			Constants.FIELD_ORDER_EXTEND_STATUS19,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE19,
			Constants.FIELD_ORDER_EXTEND_STATUS20,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE20,
			Constants.FIELD_ORDER_EXTEND_STATUS21,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE21,
			Constants.FIELD_ORDER_EXTEND_STATUS22,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE22,
			Constants.FIELD_ORDER_EXTEND_STATUS23,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE23,
			Constants.FIELD_ORDER_EXTEND_STATUS24,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE24,
			Constants.FIELD_ORDER_EXTEND_STATUS25,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE25,
			Constants.FIELD_ORDER_EXTEND_STATUS26,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE26,
			Constants.FIELD_ORDER_EXTEND_STATUS27,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE27,
			Constants.FIELD_ORDER_EXTEND_STATUS28,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE28,
			Constants.FIELD_ORDER_EXTEND_STATUS29,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE29,
			Constants.FIELD_ORDER_EXTEND_STATUS30,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE30,
			Constants.FIELD_ORDER_EXTEND_STATUS31,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE31,
			Constants.FIELD_ORDER_EXTEND_STATUS32,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE32,
			Constants.FIELD_ORDER_EXTEND_STATUS33,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE33,
			Constants.FIELD_ORDER_EXTEND_STATUS34,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE34,
			Constants.FIELD_ORDER_EXTEND_STATUS35,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE35,
			Constants.FIELD_ORDER_EXTEND_STATUS36,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE36,
			Constants.FIELD_ORDER_EXTEND_STATUS37,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE37,
			Constants.FIELD_ORDER_EXTEND_STATUS38,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE38,
			Constants.FIELD_ORDER_EXTEND_STATUS39,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE39,
			Constants.FIELD_ORDER_EXTEND_STATUS40,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE40,
			Constants.FIELD_ORDER_EXTEND_STATUS41,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE41,
			Constants.FIELD_ORDER_EXTEND_STATUS42,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE42,
			Constants.FIELD_ORDER_EXTEND_STATUS43,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE43,
			Constants.FIELD_ORDER_EXTEND_STATUS44,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE44,
			Constants.FIELD_ORDER_EXTEND_STATUS45,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE45,
			Constants.FIELD_ORDER_EXTEND_STATUS46,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE46,
			Constants.FIELD_ORDER_EXTEND_STATUS47,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE47,
			Constants.FIELD_ORDER_EXTEND_STATUS48,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE48,
			Constants.FIELD_ORDER_EXTEND_STATUS49,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE49,
			Constants.FIELD_ORDER_EXTEND_STATUS50,
			Constants.FIELD_ORDER_EXTEND_STATUS_DATE50,
			Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS,
			Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT,
			Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT,
			Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE,
			Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT,
			Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
			Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE,
			Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE,
			Constants.FIELD_ORDER_LAST_BILLED_AMOUNT,
			Constants.FIELD_ORDER_LAST_ORDER_POINT_USE,
			Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old",
			Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN,
			Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS,
			Constants.FIELD_ORDER_LAST_AUTH_FLG,
			Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN,
			Constants.FIELD_ORDER_MALL_LINK_STATUS,
			Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX,
			Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX,
			Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG,
			Constants.FIELD_ORDER_ORDER_COUNT_ORDER,
			Constants.FIELD_ORDER_DELIVERY_TRAN_ID,
			Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS,
		};

		private static readonly string[] m_excludedPropertyForUpdateOrderOwner = new string[]
		{
			Constants.FIELD_ORDEROWNER_OWNER_TEL2,
			Constants.FIELD_ORDEROWNER_OWNER_TEL3,
			Constants.FIELD_ORDEROWNER_OWNER_FAX,
			Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME,
			Constants.FIELD_ORDEROWNER_DEL_FLG,
			Constants.FIELD_ORDEROWNER_DATE_CREATED,
			Constants.FIELD_ORDEROWNER_DATE_CHANGED,
		};

		/// <summary>
		/// OrderInputコンストラクタ実行
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderModelからのOrderInputインスタンス生成
		/// ・モデルから生成したOrderInputと、引数無しで生成したOrderInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderInputTest_CreatedByModel()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), true))
			using (new TestConfigurator(Member.Of(() => Constants.MALLCOOPERATION_OPTION_ENABLED), true))
			{
				const string ORDER_ID = "DEV001";
				const string MALL_NAME = "MALL001";
				const string MALL_KBN = "KBN001";
				const string PAYMENT_NAME = "PAYMENT001";
				const string MEMBER_RANK_ID = "RANKID01";
				const string MEMBER_RANK_NAME = "RANKNAME01";
				var excludedPropertyReplication = new[]
				{
					Constants.FIELD_ORDER_ORDER_PRICE_TOTAL + "_old",
					Constants.FIELD_ORDER_ORDER_POINT_USE + "_old",
					Constants.FIELD_ORDER_LAST_BILLED_AMOUNT + "_old",
					Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old",
					Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX,
					Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX,
				};

				// モックによるドメイン層偽装
				var paymentServiceMock = new Mock<IPaymentService>();
				paymentServiceMock
					.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>()))
					.Returns(new PaymentModel
					{
						PaymentName = PAYMENT_NAME,
					});
				var mallCooperationSettingServiceMock = new Mock<IMallCooperationSettingService>();
				mallCooperationSettingServiceMock
					.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>()))
					.Returns(new MallCooperationSettingModel
					{
						MallName = MALL_NAME,
						MallKbn = MALL_KBN,
					});
				var memberRankServiceMock = new Mock<IMemberRankService>();
				memberRankServiceMock.Setup(s => s.GetMemberRankList()).Returns(new[]{ new MemberRankModel
				{
					MemberRankId = MEMBER_RANK_ID,
					MemberRankName = MEMBER_RANK_NAME
				}});

				DomainFacade.Instance.PaymentService = paymentServiceMock.Object;
				DomainFacade.Instance.MallCooperationSettingService = mallCooperationSettingServiceMock.Object;
				DomainFacade.Instance.MemberRankService = memberRankServiceMock.Object;

				var val = 1;
				var orderModel = new OrderModel
				{
					OrderId = (val++).ToString(),
					OrderIdOrg = (val++).ToString(),
					OrderGroupId = (val++).ToString(),
					OrderNo = (val++).ToString(),
					BundleChildOrderIds = (val++).ToString(),
					BundleParentOrderId = (val++).ToString(),
					BundleOrderBak = (val++).ToString(),
					UserId = (val++).ToString(),
					ShopId = (val++).ToString(),
					SupplierId = (val++).ToString(),
					OrderKbn = (val++).ToString(),
					MallId = (val++).ToString(),
					OrderPaymentKbn = (val++).ToString(),
					OrderStatus = (val++).ToString(),
					OrderDate = DateTime.Parse("2020/01/01"),
					OrderRecognitionDate = DateTime.Parse("2020/01/01"),
					OrderStockreservedStatus = (val++).ToString(),
					OrderStockreservedDate = DateTime.Parse("2020/01/01"),
					OrderShippingDate = DateTime.Parse("2020/01/01"),
					OrderShippedStatus = (val++).ToString(),
					OrderShippedDate = DateTime.Parse("2020/01/01"),
					OrderDeliveringDate = DateTime.Parse("2020/01/01"),
					OrderCancelDate = DateTime.Parse("2020/01/01"),
					OrderReturnDate = DateTime.Parse("2020/01/01"),
					OrderPaymentStatus = (val++).ToString(),
					OrderPaymentDate = DateTime.Parse("2020/01/01"),
					DemandStatus = (val++).ToString(),
					DemandDate = DateTime.Parse("2020/01/01"),
					OrderReturnExchangeStatus = (val++).ToString(),
					OrderReturnExchangeReceiptDate = DateTime.Parse("2020/01/01"),
					OrderReturnExchangeArrivalDate = DateTime.Parse("2020/01/01"),
					OrderReturnExchangeCompleteDate = DateTime.Parse("2020/01/01"),
					OrderRepaymentStatus = (val++).ToString(),
					OrderRepaymentDate = DateTime.Parse("2020/01/01"),
					OrderItemCount = val++,
					OrderProductCount = val++,
					OrderPriceSubtotal = val++,
					OrderPricePack = val++,
					OrderPriceTax = val++,
					OrderPriceShipping = val++,
					OrderPriceShippingTax = val++,
					OrderPriceExchange = val++,
					OrderPriceExchangeTax = val++,
					OrderPriceRegulation = val++,
					OrderPriceRepayment = val++,
					OrderPriceTotal = val++,
					OrderDiscountSetPrice = val++,
					OrderPointUse = val++,
					OrderPointUseYen = val++,
					OrderPointAdd = val++,
					OrderPointRate = val++,
					OrderCouponUse = val++,
					CardKbn = (val++).ToString(),
					CardInstruments = (val++).ToString(),
					CardTranId = (val++).ToString(),
					ShippingId = (val++).ToString(),
					FixedPurchaseId = (val++).ToString(),
					AdvcodeFirst = (val++).ToString(),
					AdvcodeNew = (val++).ToString(),
					InflowContentsType = (val++).ToString(),
					InflowContentsId = (val++).ToString(),
					ShippedChangedKbn = (val++).ToString(),
					ReturnExchangeKbn = (val++).ToString(),
					ReturnExchangeReasonKbn = (val++).ToString(),
					Attribute1 = (val++).ToString(),
					Attribute2 = (val++).ToString(),
					Attribute3 = (val++).ToString(),
					Attribute4 = (val++).ToString(),
					Attribute5 = (val++).ToString(),
					Attribute6 = (val++).ToString(),
					Attribute7 = (val++).ToString(),
					Attribute8 = (val++).ToString(),
					Attribute9 = (val++).ToString(),
					Attribute10 = (val++).ToString(),
					ExtendStatus1 = (val++).ToString(),
					ExtendStatusDate1 = DateTime.Parse("2020/01/01"),
					ExtendStatus2 = (val++).ToString(),
					ExtendStatusDate2 = DateTime.Parse("2020/01/01"),
					ExtendStatus3 = (val++).ToString(),
					ExtendStatusDate3 = DateTime.Parse("2020/01/01"),
					ExtendStatus4 = (val++).ToString(),
					ExtendStatusDate4 = DateTime.Parse("2020/01/01"),
					ExtendStatus5 = (val++).ToString(),
					ExtendStatusDate5 = DateTime.Parse("2020/01/01"),
					ExtendStatus6 = (val++).ToString(),
					ExtendStatusDate6 = DateTime.Parse("2020/01/01"),
					ExtendStatus7 = (val++).ToString(),
					ExtendStatusDate7 = DateTime.Parse("2020/01/01"),
					ExtendStatus8 = (val++).ToString(),
					ExtendStatusDate8 = DateTime.Parse("2020/01/01"),
					ExtendStatus9 = (val++).ToString(),
					ExtendStatusDate9 = DateTime.Parse("2020/01/01"),
					ExtendStatus10 = (val++).ToString(),
					ExtendStatusDate10 = DateTime.Parse("2020/01/01"),
					CareerId = (val++).ToString(),
					MobileUid = (val++).ToString(),
					RemoteAddr = (val++).ToString(),
					Memo = (val++).ToString(),
					WrappingMemo = (val++).ToString(),
					PaymentMemo = (val++).ToString(),
					ManagementMemo = (val++).ToString(),
					RelationMemo = (val++).ToString(),
					ReturnExchangeReasonMemo = (val++).ToString(),
					RegulationMemo = (val++).ToString(),
					RepaymentMemo = (val++).ToString(),
					DelFlg = (val++).ToString(),
					LastChanged = (val++).ToString(),
					MemberRankDiscountPrice = val++,
					MemberRankId = MEMBER_RANK_ID,
					CreditBranchNo = (val++),
					AffiliateSessionName1 = (val++).ToString(),
					AffiliateSessionValue1 = (val++).ToString(),
					AffiliateSessionName2 = (val++).ToString(),
					AffiliateSessionValue2 = (val++).ToString(),
					UserAgent = (val++).ToString(),
					GiftFlg = (val++).ToString(),
					DigitalContentsFlg = (val++).ToString(),
					Card_3dsecureTranId = (val++).ToString(),
					Card_3dsecureAuthUrl = (val++).ToString(),
					Card_3dsecureAuthKey = (val++).ToString(),
					ShippingPriceSeparateEstimatesFlg = (val++).ToString(),
					OrderTaxIncludedFlg = (val++).ToString(),
					OrderTaxRate = val++,
					OrderTaxRoundType = (val++).ToString(),
					ExtendStatus11 = (val++).ToString(),
					ExtendStatusDate11 = DateTime.Parse("2020/01/01"),
					ExtendStatus12 = (val++).ToString(),
					ExtendStatusDate12 = DateTime.Parse("2020/01/01"),
					ExtendStatus13 = (val++).ToString(),
					ExtendStatusDate13 = DateTime.Parse("2020/01/01"),
					ExtendStatus14 = (val++).ToString(),
					ExtendStatusDate14 = DateTime.Parse("2020/01/01"),
					ExtendStatus15 = (val++).ToString(),
					ExtendStatusDate15 = DateTime.Parse("2020/01/01"),
					ExtendStatus16 = (val++).ToString(),
					ExtendStatusDate16 = DateTime.Parse("2020/01/01"),
					ExtendStatus17 = (val++).ToString(),
					ExtendStatusDate17 = DateTime.Parse("2020/01/01"),
					ExtendStatus18 = (val++).ToString(),
					ExtendStatusDate18 = DateTime.Parse("2020/01/01"),
					ExtendStatus19 = (val++).ToString(),
					ExtendStatusDate19 = DateTime.Parse("2020/01/01"),
					ExtendStatus20 = (val++).ToString(),
					ExtendStatusDate20 = DateTime.Parse("2020/01/01"),
					ExtendStatus21 = (val++).ToString(),
					ExtendStatusDate21 = DateTime.Parse("2020/01/01"),
					ExtendStatus22 = (val++).ToString(),
					ExtendStatusDate22 = DateTime.Parse("2020/01/01"),
					ExtendStatus23 = (val++).ToString(),
					ExtendStatusDate23 = DateTime.Parse("2020/01/01"),
					ExtendStatus24 = (val++).ToString(),
					ExtendStatusDate24 = DateTime.Parse("2020/01/01"),
					ExtendStatus25 = (val++).ToString(),
					ExtendStatusDate25 = DateTime.Parse("2020/01/01"),
					ExtendStatus26 = (val++).ToString(),
					ExtendStatusDate26 = DateTime.Parse("2020/01/01"),
					ExtendStatus27 = (val++).ToString(),
					ExtendStatusDate27 = DateTime.Parse("2020/01/01"),
					ExtendStatus28 = (val++).ToString(),
					ExtendStatusDate28 = DateTime.Parse("2020/01/01"),
					ExtendStatus29 = (val++).ToString(),
					ExtendStatusDate29 = DateTime.Parse("2020/01/01"),
					ExtendStatus30 = (val++).ToString(),
					ExtendStatusDate30 = DateTime.Parse("2020/01/01"),
					CardInstallmentsCode = (val++).ToString(),
					SetpromotionProductDiscountAmount = val++,
					SetpromotionShippingChargeDiscountAmount = val++,
					SetpromotionPaymentChargeDiscountAmount = val++,
					OnlinePaymentStatus = (val++).ToString(),
					FixedPurchaseOrderCount = val++,
					FixedPurchaseShippedCount = val++,
					FixedPurchaseDiscountPrice = val++,
					PaymentOrderId = (val++).ToString(),
					FixedPurchaseMemberDiscountAmount = val++,
					CombinedOrgOrderIds = (val++).ToString(),
					LastBilledAmount = val++,
					ExternalPaymentStatus = (val++).ToString(),
					ExternalPaymentErrorMessage = (val++).ToString(),
					ExternalPaymentAuthDate = DateTime.Parse("2020/01/01"),
					ExtendStatus31 = (val++).ToString(),
					ExtendStatusDate31 = DateTime.Parse("2020/01/01"),
					ExtendStatus32 = (val++).ToString(),
					ExtendStatusDate32 = DateTime.Parse("2020/01/01"),
					ExtendStatus33 = (val++).ToString(),
					ExtendStatusDate33 = DateTime.Parse("2020/01/01"),
					ExtendStatus34 = (val++).ToString(),
					ExtendStatusDate34 = DateTime.Parse("2020/01/01"),
					ExtendStatus35 = (val++).ToString(),
					ExtendStatusDate35 = DateTime.Parse("2020/01/01"),
					ExtendStatus36 = (val++).ToString(),
					ExtendStatusDate36 = DateTime.Parse("2020/01/01"),
					ExtendStatus37 = (val++).ToString(),
					ExtendStatusDate37 = DateTime.Parse("2020/01/01"),
					ExtendStatus38 = (val++).ToString(),
					ExtendStatusDate38 = DateTime.Parse("2020/01/01"),
					ExtendStatus39 = (val++).ToString(),
					ExtendStatusDate39 = DateTime.Parse("2020/01/01"),
					ExtendStatus40 = (val++).ToString(),
					ExtendStatusDate40 = DateTime.Parse("2020/01/01"),
					ExtendStatus41 = (val++).ToString(),
					ExtendStatusDate41 = DateTime.Parse("2020/01/01"),
					ExtendStatus42 = (val++).ToString(),
					ExtendStatusDate42 = DateTime.Parse("2020/01/01"),
					ExtendStatus43 = (val++).ToString(),
					ExtendStatusDate43 = DateTime.Parse("2020/01/01"),
					ExtendStatus44 = (val++).ToString(),
					ExtendStatusDate44 = DateTime.Parse("2020/01/01"),
					ExtendStatus45 = (val++).ToString(),
					ExtendStatusDate45 = DateTime.Parse("2020/01/01"),
					ExtendStatus46 = (val++).ToString(),
					ExtendStatusDate46 = DateTime.Parse("2020/01/01"),
					ExtendStatus47 = (val++).ToString(),
					ExtendStatusDate47 = DateTime.Parse("2020/01/01"),
					ExtendStatus48 = (val++).ToString(),
					ExtendStatusDate48 = DateTime.Parse("2020/01/01"),
					ExtendStatus49 = (val++).ToString(),
					ExtendStatusDate49 = DateTime.Parse("2020/01/01"),
					ExtendStatus50 = (val++).ToString(),
					ExtendStatusDate50 = DateTime.Parse("2020/01/01"),
					LastOrderPointUse = val++,
					LastOrderPointUseYen = val++,
					ExternalOrderId = (val++).ToString(),
					ExternalImportStatus = (val++).ToString(),
					LastAuthFlg = (val++).ToString(),
					MallLinkStatus = (val++).ToString(),
					FixedPurchaseKbn = (val++).ToString(),
					FixedPurchaseSetting1 = (val++).ToString(),
					OrderPriceSubtotalTax = val++,
					SettlementCurrency = (val++).ToString(),
					SettlementRate = 1,
					SettlementAmount = val++,
					ShippingMemo = (val++).ToString(),
					ShippingTaxRate = val++,
					PaymentTaxRate = val++,
					InvoiceBundleFlg = (val++).ToString(),
					ReceiptFlg = (val++).ToString(),
					ReceiptOutputFlg = (val++).ToString(),
					ReceiptAddress = (val++).ToString(),
					ReceiptProviso = (val++).ToString(),
					DeliveryTranId = (val++).ToString(),
					OnlineDeliveryStatus = (val++).ToString(),
					ExternalPaymentType = (val++).ToString(),
					LogiCooperationStatus = (val++).ToString(),
					DateCreated = DateTime.Parse("2020/01/01"),
					DateChanged = DateTime.Parse("2020/01/01"),
					Owner = new OrderOwnerModel
					{
						OrderId = ORDER_ID,
						DateCreated = DateTime.Parse("2020/01/01"),
						DateChanged = DateTime.Parse("2020/01/01"),
					},
					Shippings = new[]
					{
						new OrderShippingModel
						{
							OrderId = ORDER_ID,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						}
					},
					SetPromotions = new[]
					{
						new OrderSetPromotionModel
						{
							OrderId = ORDER_ID,
						}
					},
					OrderPriceByTaxRates = new[]
					{
						new OrderPriceByTaxRateModel
						{
							OrderId = ORDER_ID,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						}
					}
				};

				var orderInputEmpty = new OrderInput();
				var orderInputByModel = new OrderInput(orderModel);

				// ・モデルから生成したOrderInputと、引数無しで生成したOrderInputの各プロパティ値が異なること
				foreach (var key in orderInputByModel.DataSource.Keys)
				{
					if (excludedPropertyReplication.Contains(key)
						|| (orderInputByModel.DataSource[key] is string == false)) continue;

					orderInputByModel.DataSource[key].Should().NotBe(orderInputEmpty.DataSource[key], " 値チェック：" + key);
				}

				// 子要素はオブジェクトが作られていることをチェックする
				orderInputByModel.Owner.OrderId.Should().Be(ORDER_ID, " 値チェック：" + Constants.TABLE_ORDEROWNER);
				orderInputByModel.Shippings[0].OrderId.Should().Be(ORDER_ID, " 値チェック：" + Constants.TABLE_ORDERSHIPPING);
				orderInputByModel.SetPromotions[0].OrderId.Should().Be(ORDER_ID, " 値チェック：" + Constants.TABLE_ORDERSETPROMOTION);
				orderInputByModel.OrderPriceByTaxRates[0].OrderId.Should().Be(
					ORDER_ID,
					" 値チェック：" + Constants.TABLE_ORDERPRICEBYTAXRATE);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成
		/// ・生成したOrderModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			string[] excludedPropertyReplication =
			{
				Constants.FIELD_ORDER_BUNDLE_ORDER_BAK,
				Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS,
				Constants.FIELD_ORDER_BUNDLE_PARENT_ORDER_ID,
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				Constants.FIELD_ORDER_BUNDLE_CHILD_ORDER_IDS,
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
			};
			var val = 1;
			var input = new OrderInput()
			{
				OrderId = (val++).ToString(),
				PaymentName = (val++).ToString(),
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
				CardTranId = (val++).ToString(),
				PaymentOrderId = (val++).ToString(),
				OrderPriceShipping = (val++).ToString(),
				OrderPriceExchange = (val++).ToString(),
				OrderPriceRegulation = (val++).ToString(),
				Memo = (val++).ToString(),
				PaymentMemo = (val++).ToString(),
				ManagementMemo = (val++).ToString(),
				ShippingMemo = (val++).ToString(),
				RelationMemo = (val++).ToString(),
				RegulationMemo = (val++).ToString(),
				CardInstruments = (val++).ToString(),
				CardInstallmentsCode = (val++).ToString(),
				SetpromotionProductDiscountAmount = (val++).ToString(),
				SetpromotionShippingChargeDiscountAmount = (val++).ToString(),
				SetpromotionPaymentChargeDiscountAmount = (val++).ToString(),
				ShippingPriceSeparateEstimatesFlg = (val++).ToString(),
				OrderKbn = (val++).ToString(),
				ReturnExchangeKbn = (val++).ToString(),
				ReturnExchangeReasonKbn = (val++).ToString(),
				ReturnExchangeReasonMemo = (val++).ToString(),
				RepaymentMemo = (val++).ToString(),
				AdvcodeFirst = (val++).ToString(),
				AdvcodeNew = (val++).ToString(),
				InflowContentsType = (val++).ToString(),
				InflowContentsId = (val++).ToString(),
				OldLastBilledAmount = (val++).ToString(),
				ShippingId = (val++).ToString(),
				OrderPriceTax = (val++).ToString(),
				OrderPriceSubtotalTax = (val++).ToString(),
				SettlementCurrency = (val++).ToString(),
				SettlementRate = (val++).ToString(),
				SettlementAmount = (val++).ToString(),
				ExternalOrderId = (val++).ToString(),
				ShippingTaxRate = (val++).ToString(),
				PaymentTaxRate = (val++).ToString(),
				InvoiceBundleFlg = (val++).ToString(),
				ReceiptFlg = (val++).ToString(),
				ReceiptOutputFlg = (val++).ToString(),
				ReceiptAddress = (val++).ToString(),
				ReceiptProviso = (val++).ToString(),
				ExternalPaymentType = (val++).ToString(),
				OrderPointAdd = (val++).ToString(),
				OrderPointUse = (val++).ToString(),
				OrderPointUseYen = (val++).ToString(),
				LastOrderPointUse = (val++).ToString(),
				LastOrderPointUseYen = (val++).ToString(),
				OldLastOrderPointUse = (val++).ToString(),
				OrderCouponUse = (val++).ToString(),
				MemberRankDiscountPrice = (val++).ToString(),
				FixedPurchaseDiscountPrice = (val++).ToString(),
				FixedPurchaseMemberDiscountAmount = (val++).ToString(),
				OrderIdOrg = (val++).ToString(),
				OrderGroupId = (val++).ToString(),
				OrderNo = (val++).ToString(),
				UserId = (val++).ToString(),
				ShopId = (val++).ToString(),
				SupplierId = (val++).ToString(),
				MallId = (val++).ToString(),
				MallName = (val++).ToString(),
				MallKbn = (val++).ToString(),
				OrderStatus = (val++).ToString(),
				OrderDate = "2020/01/01",
				OrderRecognitionDate = "2020/01/01",
				OrderStockreservedStatus = (val++).ToString(),
				OrderStockreservedDate = "2020/01/01",
				OrderShippingDate = "2020/01/01",
				OrderShippedStatus = (val++).ToString(),
				OrderShippedDate = "2020/01/01",
				OrderDeliveringDate = "2020/01/01",
				OrderCancelDate = "2020/01/01",
				OrderReturnDate = "2020/01/01",
				OrderPaymentStatus = (val++).ToString(),
				OrderPaymentDate = "2020/01/01",
				DemandStatus = (val++).ToString(),
				DemandDate = "2020/01/01",
				OrderReturnExchangeStatus = (val++).ToString(),
				OrderReturnExchangeReceiptDate = "2020/01/01",
				OrderReturnExchangeArrivalDate = "2020/01/01",
				OrderReturnExchangeCompleteDate = "2020/01/01",
				OrderRepaymentStatus = (val++).ToString(),
				OrderRepaymentDate = "2020/01/01",
				OrderItemCount = (val++).ToString(),
				OrderProductCount = (val++).ToString(),
				OrderPriceSubtotal = (val++).ToString(),
				OrderPricePack = (val++).ToString(),
				OrderPriceRepayment = (val++).ToString(),
				OrderPriceTotal = (val++).ToString(),
				OrderDiscountSetPrice = (val++).ToString(),
				OrderPointRate = (val++).ToString(),
				CardKbn = (val++).ToString(),
				FixedPurchaseId = (val++).ToString(),
				ShippedChangedKbn = (val++).ToString(),
				Attribute1 = (val++).ToString(),
				Attribute2 = (val++).ToString(),
				Attribute3 = (val++).ToString(),
				Attribute4 = (val++).ToString(),
				Attribute5 = (val++).ToString(),
				Attribute6 = (val++).ToString(),
				Attribute7 = (val++).ToString(),
				Attribute8 = (val++).ToString(),
				Attribute9 = (val++).ToString(),
				Attribute10 = (val++).ToString(),
				ExtendStatus1 = (val++).ToString(),
				ExtendStatusDate1 = "2020/01/01",
				ExtendStatus2 = (val++).ToString(),
				ExtendStatusDate2 = "2020/01/01",
				ExtendStatus3 = (val++).ToString(),
				ExtendStatusDate3 = "2020/01/01",
				ExtendStatus4 = (val++).ToString(),
				ExtendStatusDate4 = "2020/01/01",
				ExtendStatus5 = (val++).ToString(),
				ExtendStatusDate5 = "2020/01/01",
				ExtendStatus6 = (val++).ToString(),
				ExtendStatusDate6 = "2020/01/01",
				ExtendStatus7 = (val++).ToString(),
				ExtendStatusDate7 = "2020/01/01",
				ExtendStatus8 = (val++).ToString(),
				ExtendStatusDate8 = "2020/01/01",
				ExtendStatus9 = (val++).ToString(),
				ExtendStatusDate9 = "2020/01/01",
				ExtendStatus10 = (val++).ToString(),
				ExtendStatusDate10 = "2020/01/01",
				CareerId = (val++).ToString(),
				MobileUid = (val++).ToString(),
				RemoteAddr = (val++).ToString(),
				WrappingMemo = (val++).ToString(),
				DelFlg = (val++).ToString(),
				LastChanged = (val++).ToString(),
				MemberRankId = (val++).ToString(),
				MemberRankName = (val++).ToString(),
				CreditBranchNo = (val++).ToString(),
				AffiliateSessionName1 = (val++).ToString(),
				AffiliateSessionValue1 = (val++).ToString(),
				AffiliateSessionName2 = (val++).ToString(),
				AffiliateSessionValue2 = (val++).ToString(),
				UserAgent = (val++).ToString(),
				GiftFlg = (val++).ToString(),
				DigitalContentsFlg = (val++).ToString(),
				Card_3dsecureTranId = (val++).ToString(),
				Card_3dsecureAuthUrl = (val++).ToString(),
				Card_3dsecureAuthKey = (val++).ToString(),
				OrderTaxIncludedFlg = (val++).ToString(),
				OrderTaxRate = (val++).ToString(),
				OrderTaxRoundType = (val++).ToString(),
				ExtendStatus11 = (val++).ToString(),
				ExtendStatusDate11 = "2020/01/01",
				ExtendStatus12 = (val++).ToString(),
				ExtendStatusDate12 = "2020/01/01",
				ExtendStatus13 = (val++).ToString(),
				ExtendStatusDate13 = "2020/01/01",
				ExtendStatus14 = (val++).ToString(),
				ExtendStatusDate14 = "2020/01/01",
				ExtendStatus15 = (val++).ToString(),
				ExtendStatusDate15 = "2020/01/01",
				ExtendStatus16 = (val++).ToString(),
				ExtendStatusDate16 = "2020/01/01",
				ExtendStatus17 = (val++).ToString(),
				ExtendStatusDate17 = "2020/01/01",
				ExtendStatus18 = (val++).ToString(),
				ExtendStatusDate18 = "2020/01/01",
				ExtendStatus19 = (val++).ToString(),
				ExtendStatusDate19 = "2020/01/01",
				ExtendStatus20 = (val++).ToString(),
				ExtendStatusDate20 = "2020/01/01",
				ExtendStatus21 = (val++).ToString(),
				ExtendStatusDate21 = "2020/01/01",
				ExtendStatus22 = (val++).ToString(),
				ExtendStatusDate22 = "2020/01/01",
				ExtendStatus23 = (val++).ToString(),
				ExtendStatusDate23 = "2020/01/01",
				ExtendStatus24 = (val++).ToString(),
				ExtendStatusDate24 = "2020/01/01",
				ExtendStatus25 = (val++).ToString(),
				ExtendStatusDate25 = "2020/01/01",
				ExtendStatus26 = (val++).ToString(),
				ExtendStatusDate26 = "2020/01/01",
				ExtendStatus27 = (val++).ToString(),
				ExtendStatusDate27 = "2020/01/01",
				ExtendStatus28 = (val++).ToString(),
				ExtendStatusDate28 = "2020/01/01",
				ExtendStatus29 = (val++).ToString(),
				ExtendStatusDate29 = "2020/01/01",
				ExtendStatus30 = (val++).ToString(),
				ExtendStatusDate30 = "2020/01/01",
				ExtendStatus31 = (val++).ToString(),
				ExtendStatusDate31 = "2020/01/01",
				ExtendStatus32 = (val++).ToString(),
				ExtendStatusDate32 = "2020/01/01",
				ExtendStatus33 = (val++).ToString(),
				ExtendStatusDate33 = "2020/01/01",
				ExtendStatus34 = (val++).ToString(),
				ExtendStatusDate34 = "2020/01/01",
				ExtendStatus35 = (val++).ToString(),
				ExtendStatusDate35 = "2020/01/01",
				ExtendStatus36 = (val++).ToString(),
				ExtendStatusDate36 = "2020/01/01",
				ExtendStatus37 = (val++).ToString(),
				ExtendStatusDate37 = "2020/01/01",
				ExtendStatus38 = (val++).ToString(),
				ExtendStatusDate38 = "2020/01/01",
				ExtendStatus39 = (val++).ToString(),
				ExtendStatusDate39 = "2020/01/01",
				ExtendStatus40 = (val++).ToString(),
				ExtendStatusDate40 = "2020/01/01",
				ExtendStatus41 = (val++).ToString(),
				ExtendStatusDate41 = "2020/01/01",
				ExtendStatus42 = (val++).ToString(),
				ExtendStatusDate42 = "2020/01/01",
				ExtendStatus43 = (val++).ToString(),
				ExtendStatusDate43 = "2020/01/01",
				ExtendStatus44 = (val++).ToString(),
				ExtendStatusDate44 = "2020/01/01",
				ExtendStatus45 = (val++).ToString(),
				ExtendStatusDate45 = "2020/01/01",
				ExtendStatus46 = (val++).ToString(),
				ExtendStatusDate46 = "2020/01/01",
				ExtendStatus47 = (val++).ToString(),
				ExtendStatusDate47 = "2020/01/01",
				ExtendStatus48 = (val++).ToString(),
				ExtendStatusDate48 = "2020/01/01",
				ExtendStatus49 = (val++).ToString(),
				ExtendStatusDate49 = "2020/01/01",
				ExtendStatus50 = (val++).ToString(),
				ExtendStatusDate50 = "2020/01/01",
				OnlinePaymentStatus = (val++).ToString(),
				OnlineDeliveryStatus = (val++).ToString(),
				LastBilledAmount = (val++).ToString(),
				LastAuthFlg = (val++).ToString(),
				FixedPurchaseOrderCount = (val++).ToString(),
				FixedPurchaseShippedCount = (val++).ToString(),
				ExternalPaymentAuthDate = "2020/01/01",
				ExternalPaymentStatus = (val++).ToString(),
				ExternalPaymentErrorMessage = (val++).ToString(),
				ExternalImportStatus = (val++).ToString(),
				MallLinkStatus = (val++).ToString(),
				DeliveryTranId = (val++).ToString(),
				LogiCooperationStatus = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
				Owner = new OrderOwnerInput
				{
					OrderId = (val++).ToString(),
				},
				Shippings = new[]
				{
					new OrderShippingInput
					{
						OrderId = (val++).ToString(),
					}
				},
				SetPromotions = new[]
				{
					new OrderSetPromotionInput
					{
						OrderId = (val++).ToString(),
					}
				},
				Coupons = new[]
				{
					new OrderCouponInput
					{
						OrderId = (val++).ToString(),
					}
				},
				OrderPriceByTaxRates = new[]
				{
					new OrderPriceByTaxRateInput
					{
						OrderId = (val++).ToString(),
					}
				},
			};
			var orderModelEmpty = new OrderModel();

			var orderModelByInput = input.CreateModel();

			// ・生成したOrderModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderModelByInput.DataSource.Keys)
			{
				if (excludedPropertyReplication.Contains(key) || ((string)key == Constants.TABLE_ORDEROWNER)
					|| ((string)key == Constants.TABLE_ORDERSHIPPING)
					|| ((string)key == Constants.TABLE_ORDERSETPROMOTION)
					|| ((string)key == Constants.TABLE_ORDERCOUPON)
					|| ((string)key == Constants.TABLE_ORDERPRICEBYTAXRATE)) continue;

				orderModelByInput.DataSource[key].Should().NotBe(orderModelEmpty.DataSource[key], " 値チェック：" + key);
			}

			// 子要素オブジェクトが作られていることをチェックする
			orderModelByInput.Owner.OrderId.Should().NotBe("", " 値チェック：" + Constants.TABLE_ORDEROWNER);
			orderModelByInput.Shippings[0].OrderId.Should().NotBe("", " 値チェック：" + Constants.TABLE_ORDERSHIPPING);
			orderModelByInput.SetPromotions[0].OrderId.Should().NotBe("", " 値チェック：" + Constants.TABLE_ORDERSETPROMOTION);
			orderModelByInput.Coupons[0].OrderId.Should().NotBe("", " 値チェック：" + Constants.TABLE_ORDERCOUPON);
			orderModelByInput.OrderPriceByTaxRates[0].OrderId.Should().NotBe("", " 値チェック：" + Constants.TABLE_ORDERPRICEBYTAXRATE);
		}

		/// <summary>
		/// 別送フラグ判定
		/// ・OrderShippingInputとOrderOwnerInputのプロパティが同一ならばfalse、異なるならばtrueが取得できること
		/// ※チェック対象プロパティ：Name1,Name2,Zip,Addr1,Addr2,Addr3,Addr4,Addr5,CountryIsoCode,CompanyName,CompanyPostName,Tel1
		/// </summary>
		[DataTestMethod()]
		[DataRow("OwnerName1", "あ", "ShippingName1", "あ", "Name1", false)]
		[DataRow("OwnerName1", "あ", "ShippingName1", "い", "Name1", true)]
		[DataRow("OwnerName2", "あ", "ShippingName2", "あ", "Name2", false)]
		[DataRow("OwnerName2", "あ", "ShippingName2", "い", "Name2", true)]
		[DataRow("OwnerZip", "あ", "ShippingZip", "あ", "Zip", false)]
		[DataRow("OwnerZip", "あ", "ShippingZip", "い", "Zip", true)]
		[DataRow("OwnerAddr1", "あ", "ShippingAddr1", "あ", "Addr1", false)]
		[DataRow("OwnerAddr1", "あ", "ShippingAddr1", "い", "Addr1", true)]
		[DataRow("OwnerAddr2", "あ", "ShippingAddr2", "あ", "Addr2", false)]
		[DataRow("OwnerAddr2", "あ", "ShippingAddr2", "い", "Addr2", true)]
		[DataRow("OwnerAddr3", "あ", "ShippingAddr3", "あ", "Addr3", false)]
		[DataRow("OwnerAddr3", "あ", "ShippingAddr3", "い", "Addr3", true)]
		[DataRow("OwnerAddr4", "あ", "ShippingAddr4", "あ", "Addr4", false)]
		[DataRow("OwnerAddr4", "あ", "ShippingAddr4", "い", "Addr4", true)]
		[DataRow("OwnerAddr5", "あ", "ShippingAddr5", "あ", "Addr5", false)]
		[DataRow("OwnerAddr5", "あ", "ShippingAddr5", "い", "Addr5", true)]
		[DataRow("OwnerAddrCountryIsoCode", "あ", "ShippingCountryIsoCode", "あ", "CountryIsoCode", false)]
		[DataRow("OwnerAddrCountryIsoCode", "あ", "ShippingCountryIsoCode", "い", "CountryIsoCode", true)]
		[DataRow("OwnerCompanyName", "あ", "ShippingCompanyName", "あ", "CompanyName", false)]
		[DataRow("OwnerCompanyName", "あ", "ShippingCompanyName", "い", "CompanyName", true)]
		[DataRow("OwnerCompanyPostName", "あ", "ShippingCompanyPostName", "あ", "CompanyPostName", false)]
		[DataRow("OwnerCompanyPostName", "あ", "ShippingCompanyPostName", "い", "CompanyPostName", true)]
		[DataRow("OwnerTel1", "あ", "ShippingTel1", "あ", "Tel1", false)]
		[DataRow("OwnerTel1", "あ", "ShippingTel1", "い", "Tel1", true)]
		public void IsAnotherShippingFlagValidTest(
			string ownerPropertyName,
			string ownerValue,
			string shippingPropertyName,
			string shippingValue,
			string msg,
			bool resultAnotherShippingFlg)
		{
			var orderInput = new OrderInput
			{
				Owner = new OrderOwnerInput(),
			};
			var orderShippingInput = new OrderShippingInput();

			var ownerProperty = typeof(OrderOwnerInput).GetProperty(ownerPropertyName);
			var shippingProperty = typeof(OrderShippingInput).GetProperty(shippingPropertyName);

			// 指定したプロパティがOwnerとShippingに存在すること
			ownerProperty.Should().NotBeNull("引数に指定したOwnerプロパティがNULL:" + msg);
			shippingProperty.Should().NotBeNull("引数に指定したShippingプロパティがNULL:" + msg);

			// ・OrderShippingInputとOrderOwnerInputのプロパティが同一ならばfalse、異なるならばtrueが取得できること
			ownerProperty.SetValue(orderInput.Owner, ownerValue);
			shippingProperty.SetValue(orderShippingInput, shippingValue);
			orderInput.IsAnotherShippingFlagValid(orderShippingInput).Should().Be(
				resultAnotherShippingFlg,
				"別送フラグ値チェック:" + msg);
		}

		/// <summary>
		/// セットプロモーション名の取得テスト
		/// ・注文のセットプロモーションに空の配列が設定されている場合
		/// 　・引数の枝番「1」 → NULLが取得される
		/// ・注文のセットプロモーションが2件存在する場合
		/// 　・引数の枝番「1」 → 該当するセットプロモーション名が取得できる
		/// 　・引数の枝番「3」 → NULLが取得される
		/// 　・引数の枝番「」  → NULLが取得される
		/// </summary>
		[DataTestMethod()]
		[DataRow(0, "1", null, "セットプロモーションが空")]
		[DataRow(2, "1", "SetPromotion1", "存在するセットプロモーション名を取得")]
		[DataRow(2, "3", null, "引数の枝番のセットプロモーションが存在しない")]
		[DataRow(2, "", null, "引数の枝番に空文字を指定")]
		public void GetOrderSetPromotionNameTest(
			int numberOfSetPromotion,
			string targetSetPromotionNo,
			string setPromotionNameExcepted,
			string msg)
		{
			var orderInput = new OrderInput();
			var setPromotionList = new List<OrderSetPromotionInput>();
			for (var i = 1; i <= numberOfSetPromotion; i++)
			{
				setPromotionList.Add(new OrderSetPromotionInput
				{
					OrderSetpromotionNo = i.ToString(),
					SetpromotionName = "SetPromotion" + i,
				});
			}
			orderInput.SetPromotions = setPromotionList.ToArray();

			// ・注文のセットプロモーションに空の配列が設定されている場合
			// 　・引数の枝番「1」 → NULLが取得される
			// ・注文のセットプロモーションが2件存在する場合
			// 　・引数の枝番「1」 → 該当するセットプロモーション名が取得できる
			// 　・引数の枝番「3」 → NULLが取得される
			// 　・引数の枝番「」  → NULLが取得される
			orderInput.GetOrderSetPromotionName(targetSetPromotionNo).Should().Be(setPromotionNameExcepted, msg);
		}

		/// <summary>
		/// 新規配送先追加テスト
		/// ・新規追加された配送先の枝番が「2」になっていること
		/// ・新規追加された配送先に、空の注文商品が1件存在すること
		/// ・新規追加された配送先の以下の項目が先頭の配送先と同じになること
		///  チェック対象項目：「配送先国ISOコード」「配送先国名」「配送元国ISOコード」「配送元国名」「配送会社ID」
		/// </summary>
		[DataTestMethod()]
		public void AddShippingTest()
		{
			var orderInput = new OrderInput
			{
				OrderId = "ODR001",
				Shippings = new[]
				{
					new OrderShippingInput
					{
						OrderShippingNo = "1",
						ShippingCountryIsoCode = "US",
						ShippingCountryName = "United States",
						SenderCountryIsoCode = "JP",
						SenderCountryName = "Japan",
						DeliveryCompanyId = "001",
					},
				},
			};

			orderInput.AddShipping();

			// 追加した結果、配送先が2件になっていること
			orderInput.Shippings.Length.Should().Be(2, "配送先数");
			// 追加した2件目の配送先の「枝番」が2になっていること
			orderInput.Shippings[1].OrderShippingNo.Should().Be("2", "配送先枝番");
			// 追加した配送先に商品が一件存在していること
			orderInput.Shippings[1].Items.Length.Should().Be(1, "item追加");
			// 追加した配送先の「配送先国ISOコード」が1番目の配送先と同一になっていること
			orderInput.Shippings[1].ShippingCountryIsoCode.Should().Be(
				orderInput.Shippings[0].ShippingCountryIsoCode,
				"配送先国ISOコード");
			// 追加した配送先の「配送先国名」が1番目の配送先と同一になっていること
			orderInput.Shippings[1].ShippingCountryName.Should().Be(
				orderInput.Shippings[0].ShippingCountryName,
				"配送先国名");
			// 追加した配送先の「配送元国ISOコード」が1番目の配送先と同一になっていること
			orderInput.Shippings[1].SenderCountryIsoCode.Should().Be(
				orderInput.Shippings[0].SenderCountryIsoCode,
				"配送元国ISOコード");
			// 追加した配送先の「配送元国名」が1番目の配送先と同一になっていること
			orderInput.Shippings[1].SenderCountryName.Should().Be(
				orderInput.Shippings[0].SenderCountryName,
				"配送元国名");
			// 追加した配送先の「配送会社ID」が1番目の配送先と同一になっていること
			orderInput.Shippings[1].DeliveryCompanyId.Should().Be(
				orderInput.Shippings[0].DeliveryCompanyId,
				"配送会社ID");
		}

		/// <summary>
		/// 配送先削除テスト
		/// ・配送先が3件存在する注文に対して以下の引数の枝番を指定し、指定した配送先が削除されていること
		/// ・対象の枝番以外の配送先が削除されていないこと
		/// ・引数の枝番「2」 → 枝番を指定した配送先が削除されていること
		/// ・引数の枝番「5」 → 配送先が一件も削除されていないこと
		/// ・引数の枝番「」  → 配送先が一件も削除されていないこと
		/// </summary>
		[DataTestMethod()]
		[DataRow("2", "削除成功")]
		[DataRow("5", "削除対象が存在しない")]
		[DataRow("", "枝番に空文字を指定")]
		public void DeleteShippingTest(
			string targetShippingNo,
			string msg)
		{
			// 3件配送先があるOrderInputを作成
			var orderInput = new OrderInput();
			var shippingList = new List<OrderShippingInput>();
			for (var i = 1; i <= 3; i++)
			{
				shippingList.Add(new OrderShippingInput
				{
					OrderShippingNo = i.ToString(),
					ShippingName = "Shipping" + i,
				});
			}
			orderInput.Shippings = shippingList.ToArray();
			var shippingCountExceptTarget =
				orderInput
					.Shippings
					.Count(shipping => (shipping.OrderShippingNo != targetShippingNo));

			orderInput.DeleteShipping(targetShippingNo);

			// ・対象の枝番の配送先が削除されていること
			orderInput
				.Shippings
				.Any(shipping => (shipping.ShippingName == "Shipping" + targetShippingNo))
				.Should().Be(false, "対象が削除されている：" + msg);
			// ・対象の枝番以外の配送先が削除されていないこと
			orderInput.Shippings.Length.Should().Be(shippingCountExceptTarget, "対象以外が削除されていない：" + msg);
		}

		/// <summary>
		/// 商品追加テスト：対象配送先に商品が存在しない場合
		/// 商品が存在しない配送先に対して商品追加を行う
		/// ・引数に指定した枝番の配送先に新規に商品が追加される
		/// ・追加された商品の注文IDが、注文情報の注文IDと一致すること
		/// ・追加された商品の削除可能フラグにFALSEが設定されていること
		/// </summary>
		[DataTestMethod()]
		public void AddItemTest_OrderDoesNotHaveItems()
		{
			var orderInput = new OrderInput
			{
				OrderId = "ODR001",
				Shippings = new[]
				{
					new OrderShippingInput
					{
						OrderShippingNo = "1"
					}
				},
			};

			var setPromotion = orderInput.SetPromotions.FirstOrDefault();

			orderInput.AddItem("1");

			// ・引数に指定した枝番の配送先に新規に商品が追加される
			orderInput.Shippings[0].Items.Length.Should().Be(1, "商品追加");
			// ・追加された商品の注文IDが、注文情報の注文IDと一致すること
			orderInput.Shippings[0].Items[0].OrderId.Should().Be(orderInput.OrderId, "追加商品明細の注文ID");
			// ・追加された商品の削除可能フラグにFALSEが設定されていること
			orderInput.Shippings[0].Items[0].CanDelete.Should().Be(false, "追加商品の削除可能フラグにFALSEが設定されている");
		}

		/// <summary>
		/// 商品追加テスト：対象配送先に商品が存在する場合
		/// 商品が1件存在する配送先に対して商品追加を行う
		/// ・対象の配送先の商品全ての削除可能フラグにTRUEが設定されていること
		/// </summary>
		[DataTestMethod()]
		public void AddItemTest_OrderHasOneItem()
		{
			var orderInput = new OrderInput
			{
				OrderId = "ODR001",
				Shippings = new[]
				{
					new OrderShippingInput
					{
						OrderShippingNo = "1",
						Items = new[]
						{
							new OrderItemInput
							{
								OrderItemNo = "1"
							}
						}
					}
				},
			};

			orderInput.AddItem("1");

			// ・対象の配送先の商品全ての削除可能フラグにTRUEが設定されていること
			orderInput.Shippings[0].Items.All(item => item.CanDelete).Should().Be(true, "追加商品の削除可能フラグにTRUEが設定されている");
		}

		/// <summary>
		/// 商品削除テスト
		/// 配送先が1件商品が2件存在する注文に対して引数「1,0」を指定して商品削除を実行する
		/// ・1番目(注文商品配列のIndex0)の商品が削除されていること
		/// ・1番目(注文商品配列のIndex0)以外の商品が削除されていないこと
		/// ・枝番「1」の配送先の商品の削除可能フラグがFALSEになっていること
		/// </summary>
		[DataTestMethod()]
		public void RemoveItemTest()
		{
			var orderInput = new OrderInput
			{
				OrderId = "ODR001",
				Shippings = new[]
				{
					new OrderShippingInput
					{
						OrderShippingNo = "1",
						Items = new[]
						{
							new OrderItemInput
							{
								ProductId = "A",
							},
							new OrderItemInput
							{
								ProductId = "B",
							},
						}
					},
				},
			};

			orderInput.RemoveItem("1", 0);

			// ・1番目(注文商品配列のIndex0)の商品が削除されていること
			orderInput.Shippings[0].Items.Any(item => (item.ProductId == "A")).Should().Be(false, "対象の商品が削除されている");
			// ・1番目(注文商品配列のIndex0)以外の商品が削除されていないこと
			orderInput.Shippings[0].Items[0].ProductId.Should().Be("B", "対象でない商品が削除されていない");
			// ・枝番「1」の配送先の商品の削除可能フラグがFALSEになっていること
			orderInput.Shippings[0].Items[0].CanDelete.Should().Be(false, "商品削除可否フラグがFALSE");
		}

		/// <summary>
		/// 注文入力情報更新テスト
		/// ・各子要素を更新するサブメソッドが呼び出されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateTest()
		{
			const string VALUE_NEW = "NEW";
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), true))
			{
				// モックによるドメイン層偽装
				var mock = new Mock<IPointService>();
				mock.Setup(s => s.GetPointMaster()).Returns(new []{ new PointModel
				{
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					UsableUnit = 1,
					ExchangeRate = 1
				}});
				DomainFacade.Instance.PointService = mock.Object;

				var orderInput = new OrderInput
				{
					Owner = new OrderOwnerInput(),
					Shippings = new[] { new OrderShippingInput(), new OrderShippingInput(), },
					OrderPriceByTaxRates = new[] { new OrderPriceByTaxRateInput(), new OrderPriceByTaxRateInput(), },
				};
				orderInput.Update(
					new OrderInput
					{
						ShippingId = VALUE_NEW,
						Owner = new OrderOwnerInput
						{
							OwnerName1 = VALUE_NEW
						},
						Shippings = new[]
						{
							new OrderShippingInput()
							{
								ShippingName1 = VALUE_NEW
							}
						},
						OrderPriceByTaxRates = new[]
						{
							new OrderPriceByTaxRateInput
							{
								KeyTaxRate = VALUE_NEW
							},
						},
						SetPromotions = new[]
						{
							new OrderSetPromotionInput
							{
								SetpromotionName = VALUE_NEW,
							},
						},
						Coupons = new[]
						{
							new OrderCouponInput
							{
								CouponName = VALUE_NEW,
							},
						}
					});

				// ・各子要素を更新するサブメソッドが呼び出されていること
				orderInput.ShippingId.Should().Be(VALUE_NEW, "OrderのShippingId");
				orderInput.Owner.OwnerName1.Should().Be(VALUE_NEW, "Owner");
				orderInput.Shippings.Length.Should().Be(1, "ShippingsのLength");
				orderInput.Shippings[0].ShippingName1.Should().Be(VALUE_NEW, "ShippingsのShippingName1");
				orderInput.OrderPriceByTaxRates.Length.Should().Be(1, "OrderPriceByTaxRatesのLength");
				orderInput.OrderPriceByTaxRates[0].KeyTaxRate.Should().Be(VALUE_NEW, "OrderPriceByTaxRatesのKeyTaxRate");
				orderInput.SetPromotions.Length.Should().Be(1, "SetPromotionsのLength");
				orderInput.SetPromotions[0].SetpromotionName.Should().Be(VALUE_NEW, "SetPromotionsのSetpromotionName");
				orderInput.Coupons.Length.Should().Be(1, "CouponsのLength");
				orderInput.Coupons[0].CouponName.Should().Be(VALUE_NEW, "CouponsのCouponName");
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：割引系オプションがすべてOFF
		/// ・OrderInputの更新対象の各プロパティの内容が更新されていること
		/// ・更新対象外のプロパティは更新されていないこと
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_VariousDiscountOptionOff()
		{
			var discountProperty = new[]
			{
				Constants.FIELD_ORDER_ORDER_POINT_ADD,
				Constants.FIELD_ORDER_ORDER_POINT_USE,
				Constants.FIELD_ORDER_ORDER_POINT_USE_YEN,
				Constants.FIELD_ORDER_LAST_ORDER_POINT_USE,
				Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN,
				Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old",
				Constants.FIELD_ORDER_ORDER_COUPON_USE,
				Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE,
				Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE,
				Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT,
			};
			var orderInput = new OrderInput
			{
				DataSource = new Hashtable()
			};
			var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

			orderInput.As<dynamic>().UpdateOrder(orderInputTmp);

			foreach (var key in orderInput.DataSource.Keys)
			{
				var value = orderInput.DataSource[key];
				if ((discountProperty.Contains(key))
					|| ((string)key == Constants.TABLE_ORDEROWNER)
					|| ((string)key == "TwOrderInvoiceModel")
					|| (value != null) && value.GetType().IsArray) continue;

				// ・OrderInputの更新対象の各プロパティの内容が更新されていること
				// ・更新対象外のプロパティは更新されていないこと
				if (m_excludedPropertyForUpdateOrder.Contains(key))
				{
					value.Should().BeNull(" 更新対象外プロパティ値チェック：" + key);
				}
				else
				{
					value.Should().Be(orderInputTmp.DataSource[key], " 更新対象プロパティ値チェック：" + key);
				}
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：「ポイントOP」ON
		/// ・ポイント割引に関連するOrderInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_PointOptionOn()
		{
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_POINT_OPTION_ENABLED), true))
			{
				var pointDiscountOptionProperty = new[]
				{
					Constants.FIELD_ORDER_ORDER_POINT_ADD,
					Constants.FIELD_ORDER_ORDER_POINT_USE,
					Constants.FIELD_ORDER_ORDER_POINT_USE_YEN,
					Constants.FIELD_ORDER_LAST_ORDER_POINT_USE,
					Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN,
					Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old",
				};
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				orderInput.As<dynamic>().UpdateOrder(orderInputTmp);

				// ・ポイント割引に関連するOrderInputの各プロパティの内容が更新されていること
				foreach (var key in orderInputTmp.DataSource.Keys)
				{
					var value = orderInput.DataSource[key];

					if (pointDiscountOptionProperty.Contains(key) == false) continue;

					if ((string)key == Constants.FIELD_ORDER_LAST_ORDER_POINT_USE)
					{
						value.Should().Be(orderInputTmp.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE], " 値チェック：" + key);
					}
					else if ((string)key == Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN)
					{
						value.Should().Be(orderInputTmp.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN], " 値チェック：" + key);
					}
					else
					{
						value.Should().Be(orderInputTmp.DataSource[key], " 値チェック：" + key);
					}
				}
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：「クーポンOP」ON
		/// ・クーポン割引に関連するOrderInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_CouponOptionOn()
		{
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), true))
			{
				var couponDiscountOptionProperty = new[]
				{
					Constants.FIELD_ORDER_ORDER_COUPON_USE,
				};
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				orderInput.As<dynamic>().UpdateOrder(orderInputTmp);

				// ・ポイント割引に関連するOrderInputの各プロパティの内容が更新されていること
				foreach (var key in orderInputTmp.DataSource.Keys)
				{
					var value = orderInput.DataSource[key];
					if (couponDiscountOptionProperty.Contains(key) == false) continue;

					value.Should().Be(orderInputTmp.DataSource[key], " 値チェック：" + key);
				}
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：「会員ランクOP」ON
		/// ・会員ランク割引に関連するOrderInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_MemberRankOptionOn()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), true))
			{
				var memberRankDiscountOptionProperty = new[]
				{
					Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE,
				};
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				orderInput.As<dynamic>().UpdateOrder(orderInputTmp);

				// ・会員ランク割引に関連するOrderInputの各プロパティの内容が更新されていること
				foreach (var key in orderInputTmp.DataSource.Keys)
				{
					var value = orderInput.DataSource[key];
					if (memberRankDiscountOptionProperty.Contains(key) == false) continue;

					value.Should().Be(orderInputTmp.DataSource[key], " 値チェック：" + key);
				}
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：「定期購入OP」ON
		/// ・定期購入割引に関連するOrderInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_FiexedPurchaseOptionOn()
		{
			using (new TestConfigurator(Member.Of(() => Constants.FIXEDPURCHASE_OPTION_ENABLED), true))
			{
				var fixedPurchaseDiscountOptionProperty = new[]
				{
					Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE,
				};
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				orderInput.As<dynamic>().UpdateOrder(orderInputTmp);

				// ・定期購入割引に関連するOrderInputの各プロパティの内容が更新されていること
				foreach (var key in orderInputTmp.DataSource.Keys)
				{
					var value = orderInput.DataSource[key];
					if (fixedPurchaseDiscountOptionProperty.Contains(key) == false) continue;

					value.Should().Be(orderInputTmp.DataSource[key], " 値チェック：" + key);
				}
			}
		}

		/// <summary>
		/// 注文入力情報更新テスト：「会員ランクOP」ONかつ「定期購入OP」ON
		/// ・定期会員割引に関連するOrderInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateOrderTest_MemberRankOptionOnAndFiexedPurchaseOptionOn()
		{
			using (new TestConfigurator(Member.Of(() => Constants.FIXEDPURCHASE_OPTION_ENABLED), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), true))
			{
				var fixedPurchaseMemberDiscountOptionProperty = new[]
				{
					Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT,
				};
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				new PrivateType(typeof(OrderInput)).InvokeStatic("UpdateOrder", new object[] { orderInputTmp });

				// ・定期購入割引に関連するOrderInputの各プロパティの内容が更新されていること
				foreach (var key in orderInputTmp.DataSource.Keys)
				{
					var value = orderInput.DataSource[key];
					if (fixedPurchaseMemberDiscountOptionProperty.Contains(key) == false) continue;

					value.Should().Be(orderInputTmp.DataSource[key], " 値チェック：" + key);
				}
			}
		}

		/// <summary>
		/// 注文者情報更新テスト
		/// ・OrderOwnerInputの各プロパティの内容が更新されていること
		/// ・注文IDはOrderInputクラスの注文IDで更新されること
		/// ・氏名/氏名カナプロパティは、「姓」「名」プロパティが連結されてコピーされること
		/// ・配送先国が「JP」の場合、電話番号と郵便番号がハイフン区切りのプロパティが連結されてコピーされること
		/// ・配送先国が「JP以外」の場合、電話番号と郵便番号がそのままコピーされること
		/// </summary>
		[DataTestMethod()]
		[DataRow(false, Constants.COUNTRY_ISO_CODE_JP, "グローバルOP:OFF JP配送")]
		[DataRow(false, Constants.COUNTRY_ISO_CODE_US, "グローバルOP:OFF US配送")]
		[DataRow(true, Constants.COUNTRY_ISO_CODE_JP, "グローバルOP:ON JP配送")]
		[DataRow(true, Constants.COUNTRY_ISO_CODE_US, "グローバルOP:ON US配送")]
		public void UpdateOrderOwnerTest(bool globalOptionEnableFlg, string countryIsoCode, string msg)
		{
			const string ORDER_ID = "DEV001";
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), globalOptionEnableFlg))
			{
				var orderInput = new OrderInput
				{
					OrderId = ORDER_ID,
					Owner = new OrderOwnerInput
					{
						DataSource = new Hashtable()
					},
				};
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();
				orderInputTmp.Owner.OwnerAddrCountryIsoCode = countryIsoCode;

				orderInput.As<dynamic>().UpdateOrderOwner(orderInputTmp);

				foreach (var key in orderInputTmp.Owner.DataSource.Keys)
				{
					if (((string)key == Constants.FIELD_ORDEROWNER_ORDER_ID)
						|| ((string)key == Constants.FIELD_ORDEROWNER_OWNER_TEL1)
						|| ((string)key == Constants.FIELD_ORDEROWNER_OWNER_ZIP)
						|| ((string)key == Constants.FIELD_ORDEROWNER_OWNER_NAME)
						|| ((string)key == Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA)) continue;
					var value = orderInput.Owner.DataSource[key];

					// ・OrderOwnerInputの各プロパティの内容が更新されていること
					// ・更新対象外のプロパティは更新されていないこと
					if (m_excludedPropertyForUpdateOrderOwner.Contains(key))
					{
						value.Should().BeNull(" 更新対象外プロパティ値チェック：" + key);
					}
					else
					{
						value.Should().Be(orderInputTmp.Owner.DataSource[key], " 更新対象プロパティ値チェック：" + key);
					}
				}
				// ・注文IDはOrderInputクラスの注文IDで更新されること
				orderInput.Owner.OrderId.Should().Be(
					orderInput.OrderId,
					msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_ORDER_ID);
				// ・氏名/氏名カナプロパティは、「姓」「名」プロパティが連結されてコピーされること
				orderInput.Owner.OwnerName.Should().Be(
					orderInputTmp.Owner.OwnerName1 + orderInputTmp.Owner.OwnerName2,
					msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_NAME);
				orderInput.Owner.OwnerNameKana.Should().Be(
					orderInputTmp.Owner.OwnerNameKana1 + orderInputTmp.Owner.OwnerNameKana2,
					msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA);
				// ・配送先国が「JP」の場合、電話番号と郵便番号がハイフン区切りのプロパティが連結されてコピーされること
				if ((globalOptionEnableFlg == false) || (countryIsoCode == Constants.COUNTRY_ISO_CODE_JP))
				{
					orderInput.Owner.OwnerTel1.Should().Be(
						string.Join(
							"-",
							orderInputTmp.Owner.OwnerTel1_1,
							orderInputTmp.Owner.OwnerTel1_2,
							orderInputTmp.Owner.OwnerTel1_3),
						msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_TEL1);
					orderInput.Owner.OwnerZip.Should().Be(
						string.Join("-", orderInputTmp.Owner.OwnerZip1, orderInputTmp.Owner.OwnerZip2),
						msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_ZIP);
				}
				// ・配送先国が「JP以外」の場合、電話番号と郵便番号がそのままコピーされること
				else
				{
					orderInput.Owner.OwnerTel1.Should().Be(
						orderInputTmp.Owner.OwnerTel1,
						msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_TEL1);
					orderInput.Owner.OwnerZip.Should().Be(
						orderInputTmp.Owner.OwnerZip,
						msg + " 値チェック：" + Constants.FIELD_ORDEROWNER_OWNER_ZIP);
				}
			}
		}

		/// <summary>
		/// 配送先情報更新テスト：コンビニ受け取りフラグON/OFFの場合分け
		/// ・OrderShippingInputの各プロパティの内容が更新されていること
		/// ・配送先情報のコンビニ受取フラグがONの場合、氏名と電話番号と郵便番号が元のままコピーされること
		/// ・配送先情報のコンビニ受取フラグがOFFの場合、氏名と電話番号と郵便番号がハイフン区切りのプロパティが連結されてコピーされること
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF, "コンビニ受け取りフラグOFF")]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON, "コンビニ受け取りフラグON")]
		public void UpdateOrderShippingsTest_RecieveConvinienceStore(string shippingAddrKbnConvinienceStore, string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), true))
			{
				var orderInput = new OrderInput()
				{
					Owner = new OrderOwnerInput(),
					Shippings = new[]
					{
								new OrderShippingInput()
							}
				};
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();
				orderInputTmp.Shippings[0].ShippingReceivingStoreFlg = shippingAddrKbnConvinienceStore;

				orderInput.As<dynamic>().UpdateOrderShippings(orderInputTmp);

				// ・OrderShippingInputの各プロパティの内容が更新されていること
				foreach (var key in orderInput.Shippings[0].DataSource.Keys)
				{
					if (((string)key == Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SENDER_NAME)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SENDER_TEL1)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_SENDER_ZIP)
						|| ((string)key == Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG)) continue;

					var value = orderInput.Shippings[0].DataSource[key];
					if ((value != null) && value.GetType().IsArray) continue; // 配列要素はスキップ（Itemsなどはコピーされないため）

					value.Should().Be(orderInputTmp.Shippings[0].DataSource[key], msg + " 値チェック：" + key);
				}

				var shippingInput = orderInput.Shippings[0];

				// ・配送先情報のコンビニ受取フラグがONの場合、氏名と電話番号と郵便番号が元のままコピーされること
				if (shippingAddrKbnConvinienceStore
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				{
					shippingInput.ShippingName.Should().Be(
						orderInputTmp.Shippings[0].ShippingName,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_ORDER_ID);
					shippingInput.ShippingNameKana.Should().Be(
						orderInputTmp.Shippings[0].ShippingNameKana,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA);
					shippingInput.SenderName.Should().Be(
						orderInputTmp.Shippings[0].SenderName,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_NAME);
					shippingInput.SenderNameKana.Should().Be(
						orderInputTmp.Shippings[0].SenderNameKana,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA);
					shippingInput.ShippingTel1.Should().Be(
						orderInputTmp.Shippings[0].ShippingTel1,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
					shippingInput.ShippingZip.Should().Be(
						orderInputTmp.Shippings[0].ShippingZip,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);
					shippingInput.SenderTel1.Should().Be(
						orderInputTmp.Shippings[0].SenderTel1,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
					shippingInput.SenderZip.Should().Be(
						orderInputTmp.Shippings[0].SenderZip,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				}
				// ・配送先情報のコンビニ受取フラグがOFFの場合、氏名と電話番号と郵便番号がハイフン区切りのプロパティが連結されてコピーされること
				else
				{
					shippingInput.ShippingName.Should().Be(
						orderInputTmp.Shippings[0].ShippingName1 + orderInputTmp.Shippings[0].ShippingName2,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_ORDER_ID);
					shippingInput.ShippingNameKana.Should().Be(
						orderInputTmp.Shippings[0].ShippingNameKana1 + orderInputTmp.Shippings[0].ShippingNameKana2,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA);
					shippingInput.SenderName.Should().Be(
						orderInputTmp.Shippings[0].SenderName1 + orderInputTmp.Shippings[0].SenderName2,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_NAME);
					shippingInput.SenderNameKana.Should().Be(
						orderInputTmp.Shippings[0].SenderNameKana1 + orderInputTmp.Shippings[0].SenderNameKana2,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA);
					shippingInput.ShippingTel1.Should().Be(
						string.Join(
							"-",
							orderInputTmp.Shippings[0].ShippingTel1_1,
							orderInputTmp.Shippings[0].ShippingTel1_2,
							orderInputTmp.Shippings[0].ShippingTel1_3),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
					shippingInput.ShippingZip.Should().Be(
						string.Join("-", orderInputTmp.Shippings[0].ShippingZip1, orderInputTmp.Shippings[0].ShippingZip2),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);
					shippingInput.SenderTel1.Should().Be(
						string.Join(
							"-",
							orderInputTmp.Shippings[0].SenderTel1_1,
							orderInputTmp.Shippings[0].SenderTel1_2,
							orderInputTmp.Shippings[0].SenderTel1_3),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
					shippingInput.SenderZip.Should().Be(
						string.Join("-", orderInputTmp.Shippings[0].SenderZip1, orderInputTmp.Shippings[0].SenderZip2),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				}
			}
		}

		/// <summary>
		/// 配送先情報更新テスト：配送先国による場合分け
		/// ・OrderShippingInputの各プロパティの内容が更新されていること
		/// ・配送先国が「JP」の場合、電話番号と郵便番号がハイフン区切りのプロパティが連結されてコピーされること
		/// ・配送先国が「JP以外」の場合、電話番号とと郵便番号がそのままコピーされること
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.COUNTRY_ISO_CODE_JP, "JP配送")]
		[DataRow(Constants.COUNTRY_ISO_CODE_US, "海外配送")]
		public void UpdateOrderShippingsTest_ShippingCountry(string shippingCountryIsoCode, string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), true))
			{
				var orderInput = new OrderInput()
				{
					Owner = new OrderOwnerInput(),
					Shippings = new[]
					{
								new OrderShippingInput()
							}
				};
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();
				orderInputTmp.Shippings[0].ShippingCountryIsoCode = shippingCountryIsoCode;

				orderInput.As<dynamic>().UpdateOrderShippings(orderInputTmp);

				// ・ OrderShippingInputの各プロパティの内容が更新されていること
				// ・配送先国が「JP」の場合、電話番号と住所がハイフン区切りのプロパティが連結されてコピーされること
				var shippingInput = orderInput.Shippings[0];
				if (orderInputTmp.Shippings[0].IsShippingAddrJp)
				{
					shippingInput.ShippingTel1.Should().Be(
						string.Join(
							"-",
							orderInputTmp.Shippings[0].ShippingTel1_1,
							orderInputTmp.Shippings[0].ShippingTel1_2,
							orderInputTmp.Shippings[0].ShippingTel1_3),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
					shippingInput.ShippingZip.Should().Be(
						string.Join("-", orderInputTmp.Shippings[0].ShippingZip1, orderInputTmp.Shippings[0].ShippingZip2),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);
					shippingInput.SenderTel1.Should().Be(
						string.Join(
							"-",
							orderInputTmp.Shippings[0].SenderTel1_1,
							orderInputTmp.Shippings[0].SenderTel1_2,
							orderInputTmp.Shippings[0].SenderTel1_3),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
					shippingInput.SenderZip.Should().Be(
						string.Join("-", orderInputTmp.Shippings[0].SenderZip1, orderInputTmp.Shippings[0].SenderZip2),
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				}
				// ・配送先国が「JP以外」の場合、電話番号と住所がそのままコピーされること
				else
				{
					shippingInput.ShippingTel1.Should().Be(
						orderInputTmp.Shippings[0].ShippingTel1,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
					shippingInput.ShippingZip.Should().Be(
						orderInputTmp.Shippings[0].ShippingZip,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);
					shippingInput.SenderTel1.Should().Be(
						orderInputTmp.Shippings[0].SenderTel1,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
					shippingInput.SenderZip.Should().Be(
						orderInputTmp.Shippings[0].SenderZip,
						msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				}
			}
		}

		/// <summary>
		/// 配送先情報更新テスト(別送フラグ更新)：注文者と配送先が異なる
		/// ・コンビニ受け取りフラグがON → 別送フラグが「コンビニ受け取り」となること
		/// ・コンビニ受け取りフラグがOFF → 別送フラグ「有効」となること
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF, Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID, "コンビニ受け取りフラグOFF")]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON, Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG, "コンビニ受け取りフラグON")]
		public void UpdateOrderShippingsTest_ShippingAnotherAddress(
			string shippingAddrKbnConvinienceStore,
			string anotherShippingFlgResult,
			string msg)
		{
			var orderInput = new OrderInput()
			{
				Owner = new OrderOwnerInput(),
				Shippings = new[]
				{
							new OrderShippingInput()
						}
			};
			var orderInputTmp = CreateTemplateOrderInputForUpdateTest();
			orderInputTmp.Shippings[0].ShippingReceivingStoreFlg = shippingAddrKbnConvinienceStore;

			orderInput.As<dynamic>().UpdateOrderShippings(orderInputTmp);

			var shippingInput = orderInput.Shippings[0];

			// ・コンビニ受け取りフラグがON → 別送フラグが「コンビニ受け取り」となること
			// ・コンビニ受け取りフラグがOFF → 別送フラグ「有効」となること
			shippingInput.AnotherShippingFlg.Should().Be(
				anotherShippingFlgResult,
				msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG);
		}

		/// <summary>
		/// 配送先情報更新テスト(別送フラグ更新)：注文者と配送先が同一
		/// ・コンビニ受け取りフラグがON → 別送フラグが「コンビニ受け取り」となること
		/// ・コンビニ受け取りフラグがOFF → 別送フラグ「無効」となること
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF, Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID, "コンビニ受け取りフラグOFF")]
		[DataRow(Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON, Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG, "コンビニ受け取りフラグON")]
		public void UpdateOrderShippingsTest_ShippingSameAddress(
					string shippingAddrKbnConvinienceStore,
					string anotherShippingFlgResult,
					string msg)
		{
			var orderInput = new OrderInput();
			var orderShippingInput = new OrderShippingInput();
			var orderOwnerInput = new OrderOwnerInput
			{
				OwnerName1 = "a",
				OwnerName2 = "a",
				OwnerZip = "111-1111",
				OwnerZip1 = "111",
				OwnerZip2 = "1111",
				OwnerAddr1 = "a",
				OwnerAddr2 = "a",
				OwnerAddr3 = "a",
				OwnerAddr4 = "a",
				OwnerCompanyName = "a",
				OwnerCompanyPostName = "a",
				OwnerTel1 = "000-0000-0000",
				OwnerTel1_1 = "000",
				OwnerTel1_2 = "0000",
				OwnerTel1_3 = "0000",
			};
			orderInput.Owner = orderOwnerInput;
			orderInput.Shippings = new[] { orderShippingInput };
			var orderInputTmp = new OrderInput();
			var orderShippingTmp = new OrderShippingInput()
			{
				ShippingName1 = "a",
				ShippingName2 = "a",
				ShippingZip = "111-1111",
				ShippingZip1 = "111",
				ShippingZip2 = "1111",
				ShippingAddr1 = "a",
				ShippingAddr2 = "a",
				ShippingAddr3 = "a",
				ShippingAddr4 = "a",
				ShippingCompanyName = "a",
				ShippingCompanyPostName = "a",
				ShippingTel1 = "000-0000-0000",
				ShippingTel1_1 = "000",
				ShippingTel1_2 = "0000",
				ShippingTel1_3 = "0000",
				ShippingReceivingStoreFlg = shippingAddrKbnConvinienceStore,
			};
			orderInputTmp.Shippings = new[] { orderShippingTmp };
			orderInput.As<dynamic>().UpdateOrderShippings(orderInputTmp);

			var shippingInput = orderInput.Shippings[0];

			// ・コンビニ受け取りフラグがON → 別送フラグが「コンビニ受け取り」となること
			// ・コンビニ受け取りフラグがOFF → 別送フラグ「無効」となること
			shippingInput.AnotherShippingFlg.Should().Be(
				anotherShippingFlgResult,
				msg + " 値チェック：" + Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG);
		}

		/// <summary>
		/// 商品情報更新テスト
		/// ・OrderItemInputの各プロパティの内容が更新されていること
		/// ・注文情報の返品交換区分により、商品の返品交換区分が更新されること
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN, "注文返品区分「指定無し」注文商品返品区分「指定無し」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE, "注文返品区分「指定無し」注文商品返品区分「交換」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN, "注文返品区分「指定無し」注文商品返品区分「返品」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN, "注文返品区分「交換」注文商品返品区分「指定無し」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE, "注文返品区分「交換」注文商品返品区分「交換」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN, "注文返品区分「交換」注文商品返品区分「返品」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN, "注文返品区分「返品」注文商品返品区分「指定無し」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE, "注文返品区分「返品」注文商品返品区分「交換」")]
		[DataRow(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN, "注文返品区分「返品」注文商品返品区分「返品」")]
		public void UpdateOrderItemsTest(string returnExchangeKbn, string returnExchangeKbnItem, string msg)
		{
			var orderInput = new OrderInput()
			{
				ReturnExchangeKbn = returnExchangeKbn,
				Owner = new OrderOwnerInput(),
				Shippings = new[]
				{
							new OrderShippingInput
							{
								Items = new[]
								{
									new OrderItemInput()
								}
							}
						}
			};
			var orderInputTmp = CreateTemplateOrderInputForUpdateTest();
			orderInputTmp.Shippings[0].Items[0].ItemReturnExchangeKbn = returnExchangeKbnItem;

			orderInput.As<dynamic>().UpdateOrderItems(orderInputTmp.Shippings[0].Items, orderInput.Shippings[0]);

			// ・OrderItemInputの各プロパティの内容が更新されていること
			foreach (var key in orderInput.Shippings[0].Items[0].DataSource.Keys)
			{
				if ((string)key == Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN) continue;
				var value = orderInput.Shippings[0].Items[0].DataSource[key];
				if ((value != null) && value.GetType().IsArray) continue;

				value.Should().Be(orderInputTmp.Shippings[0].Items[0].DataSource[key], msg + " 値チェック：" + key);
			}

			// ・注文情報の返品交換区分により、商品の返品交換区分が更新されること
			if (orderInputTmp.Shippings[0].Items[0].IsReturnItem)
			{
				orderInput.Shippings[0].Items[0].ItemReturnExchangeKbn.Should().Be(
					orderInputTmp.Shippings[0].Items[0].ItemReturnExchangeKbn,
					msg + " 値チェック：" + Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN);
			}
			else
			{
				orderInput.Shippings[0].Items[0].ItemReturnExchangeKbn.Should().Be(
					orderInput.IsNotReturnExchangeOrder
						? Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN
						: Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE,
					msg + " 値チェック：" + Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN);
			}
		}

		/// <summary>
		/// セットプロモーション更新テスト
		/// ・OrderSetPromotionInputの各プロパティの内容が更新されていること
		/// </summary>
		[DataTestMethod()]
		public void UpdateSetPromotionTest()
		{
			var orderInput = new OrderInput()
			{
				Owner = new OrderOwnerInput(),
				SetPromotions = new[]
				{
					new OrderSetPromotionInput()
				}
			};
			var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

			orderInput.As<dynamic>().UpdateSetPromotion(orderInputTmp);

			// ・OrderSetPromotionInputの各プロパティの内容が更新されていること
			foreach (var key in orderInput.SetPromotions[0].DataSource.Keys)
			{
				var value = orderInput.SetPromotions[0].DataSource[key];
				if ((value != null) && value.GetType().IsArray) continue;

				value.Should().Be(orderInputTmp.SetPromotions[0].DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// 注文クーポン情報更新テスト
		/// ・OrderCouponInputの各プロパティの内容が更新されていること
		/// ・クーポンOPがOFFの場合、クーポン情報が空の配列となること
		/// </summary>
		[DataTestMethod()]
		[DataRow(true, "クーポンOP:ON")]
		[DataRow(false, "クーポンOP:OFF")]
		public void UpdateOrderCouponTest(bool couponOptionEnableFlg, string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), couponOptionEnableFlg))
			{
				var orderInput = new OrderInput();
				var orderInputTmp = CreateTemplateOrderInputForUpdateTest();

				orderInput.As<dynamic>().UpdateOrderCoupon(orderInputTmp);

				// ・OrderCouponInputの各プロパティの内容が更新されていること
				if (couponOptionEnableFlg)
				{
					foreach (var key in orderInput.Coupons[0].DataSource.Keys)
					{
						if (((string)key == Constants.FIELD_ORDERCOUPON_ORDER_ID)
							|| ((string)key == Constants.FIELD_ORDERCOUPON_DATE_CREATED)
							|| ((string)key == Constants.FIELD_ORDERCOUPON_DATE_CHANGED)) continue;
						var value = orderInput.Coupons[0].DataSource[key];
						if ((value != null) && value.GetType().IsArray) continue; // 配列要素はスキップ（Itemsなどはコピーされないため）

						value.Should().Be(orderInputTmp.Coupons[0].DataSource[key], msg + " 値チェック：" + key);
					}
				}
				// ・クーポンOPがOFFの場合、クーポン情報が空の配列となること
				else
				{
					orderInput.Coupons.Any().Should().Be(false, msg);
				}
			}
		}

		/// <summary>
		/// 再計算テスト；注文に削除対象商品・返品商品が存在しない
		/// ・注文の全ての商品価格と割引額を含んだ金額が計算されていること
		/// </summary>
		[DataTestMethod()]
		public void RecalculateTest_OrderDoesNotHaveDeleteItemAndReturnItem()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			{
				var orderInput = CreateTemplateOrderInputForRecalculateTest();
				// モックによるドメイン層偽装
				var mock = new Mock<IPointService>();
				mock.Setup(s => s.GetPointMaster()).Returns(new[]{ new PointModel
				{
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					UsableUnit = 1,
					ExchangeRate = 1
				}});
				DomainFacade.Instance.PointService = mock.Object;

				orderInput.As<dynamic>().Recalculate();

				// ・注文の全ての商品価格と割引額を含んだ金額が計算されていること
				orderInput.OrderItemCount.Should().Be("2", "明細数");
				orderInput.OrderProductCount.Should().Be("3", "商品個数");
				orderInput.OrderPriceSubtotal.Should().Be("210000000000000", "商品小計");
				orderInput.SetpromotionProductDiscountAmount.Should().Be("10000000", "セットプロモーション割引(商品価格)");
				orderInput.SetpromotionShippingChargeDiscountAmount.Should().Be("100000000", "セットプロモーション割引(配送料)");
				orderInput.SetpromotionPaymentChargeDiscountAmount.Should().Be("1000000000", "セットプロモーション割引(決済手数料)");
				orderInput.OrderPriceTotal.Should().Be("211088888888890", "金額合計");
				orderInput.LastBilledAmount.Should().Be("211088888888890", "最終請求金額");
				orderInput.LastOrderPointUse.Should().Be("10000", "最終利用ポイント数");
				orderInput.LastOrderPointUseYen.Should().Be("10000", "最終利用ポイント割引額");
			}
		}

		/// <summary>
		/// 再計算テスト；注文に削除対象商品が存在する
		/// ・注文の削除対象の商品価格を除いて合計金額・商品個数が計算されていること
		/// </summary>
		[DataTestMethod()]
		public void RecalculateTest_OrderHasDeleteItem()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			{
				var orderInput = CreateTemplateOrderInputForRecalculateTest();
				orderInput.Shippings[0].Items[0].DeleteTarget = true;
				// モックによるドメイン層偽装
				var mock = new Mock<IPointService>();
				mock.Setup(s => s.GetPointMaster()).Returns(new[]{ new PointModel
				{
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					UsableUnit = 1,
					ExchangeRate = 1
				}});
				DomainFacade.Instance.PointService = mock.Object;

				orderInput.As<dynamic>().Recalculate();

				// ・注文の削除対象の商品価格を除いて合計金額・商品個数が計算されていること
				orderInput.OrderItemCount.Should().Be("1", "明細数");
				orderInput.OrderProductCount.Should().Be("2", "商品個数");
				orderInput.OrderPriceSubtotal.Should().Be("200000000000000", "商品小計");
				orderInput.SetpromotionProductDiscountAmount.Should().Be("10000000", "セットプロモーション割引(商品価格)");
				orderInput.SetpromotionShippingChargeDiscountAmount.Should().Be("100000000", "セットプロモーション割引(配送料)");
				orderInput.SetpromotionPaymentChargeDiscountAmount.Should().Be("1000000000", "セットプロモーション割引(決済手数料)");
				orderInput.OrderPriceTotal.Should().Be("201088888888890", "金額合計");
				orderInput.LastBilledAmount.Should().Be("201088888888890", "最終請求金額");
				orderInput.LastOrderPointUse.Should().Be("10000", "最終利用ポイント数");
				orderInput.LastOrderPointUseYen.Should().Be("10000", "最終利用ポイント割引額");
			}
		}

		/// <summary>
		/// 再計算テスト；注文に返品商品が存在する
		/// ・商品個数に返品商品の個数が含まれていないこと
		/// ・注文の全ての商品価格と割引額を含んだ金額が計算されていること
		/// </summary>
		[DataTestMethod()]
		public void RecalculateTest_OrderHasReturnItem()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			{
				const int POINT_USE = -200;
				// モックによるドメイン層偽装
				var mock = new Mock<IPointService>();
				mock.Setup(s => s.GetPointMaster()).Returns(new[]{ new PointModel
				{
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					UsableUnit = 1,
					ExchangeRate = 1
				}});
				DomainFacade.Instance.PointService = mock.Object;
				var orderInput = new OrderInput
				{
					ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					OrderPointUse = POINT_USE.ToString(),
					OrderPointUseYen = POINT_USE.ToString(),
					OldLastBilledAmount = "900",
					OldOrderPriceTotal = "900",
					OldLastOrderPointUse = (POINT_USE * -1).ToString(),
					OldOrderPointUse = (POINT_USE * -1).ToString(),
					Shippings = new[]
					{
					new OrderShippingInput
					{
						Items = new[]
						{
							new OrderItemInput
							{
								OrderId = "test",
								DeleteTarget = false,
								ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE,
								ShopId = "0",
								ProductId = "XXX",
								VariationId = "_1",
								ProductPrice = "1000",
								ItemQuantity = "2",
								ItemPrice = "2000"
							},
							new OrderItemInput
							{
								OrderId = "test",
								DeleteTarget = false,
								ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN,
								ShopId = "0",
								ProductId = "YYY",
								VariationId = "_1",
								ProductPrice = "1100",
								ItemQuantity = "-1",
								ItemPrice = "-1100"
							}
						}
					}
				},
					OrderPriceByTaxRates = new[]
					{
						new OrderPriceByTaxRateInput
						{
							PriceCorrectionByRate = "-100"
						}
					}
				};

				orderInput.As<dynamic>().Recalculate();

				// ・商品個数に返品商品の個数が含まれていないこと
				orderInput.OrderItemCount.Should().Be("1", "明細数");
				orderInput.OrderProductCount.Should().Be("2", "商品個数");
				// ・注文の全ての商品価格と割引額を含んだ金額が計算されていること
				orderInput.OrderPriceSubtotal.Should().Be("900", "商品小計");
				orderInput.OrderPriceTotal.Should().Be("1000", "金額合計");
				orderInput.LastBilledAmount.Should().Be("1000", "最終請求金額");
				orderInput.LastOrderPointUse.Should().Be("-200", "最終利用ポイント数");
				orderInput.LastOrderPointUseYen.Should().Be("-200", "最終利用ポイント割引金額");
			}
		}
		
		/// <summary>
		/// 注文合計金額の再計算テスト
		/// ・注文合計金額が「税込商品小計 + 配送料 + 決済手数料 + 調整金額 - 会員ランク割引 - クーポン割引 - ポイント割引 - セットプロモーション割引 - 定期会員割引 - 定期購入割引」となっていること
		/// </summary>
		[DataTestMethod()]
		public void RecalculatePriceTotalTest()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			{
				var orderInput = CreateTemplateOrderInputForRecalculateTest();
				// モックによるドメイン層偽装
				var mock = new Mock<IPointService>();
				mock.Setup(s => s.GetPointMaster()).Returns(new[]{ new PointModel
				{
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					UsableUnit = 1,
					ExchangeRate = 1
				}});
				DomainFacade.Instance.PointService = mock.Object;

				orderInput.As<dynamic>().Recalculate();

				orderInput.OrderPriceTotal.Should().Be("211088888888890", "金額合計");
			}
		}

		/// <summary>
		/// Recalculateテスト用のテンプレートOrderInputインスタンス生成
		/// </summary>		
		private OrderInput CreateTemplateOrderInputForRecalculateTest()
		{
			// OrderInput.Recalculateメソッドで計算に使用される金額項目を設定する
			var orderInput = new OrderInput
			{
				ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN,
				OrderPriceRegulation = "-10",
				MemberRankDiscountPrice = "100",
				OrderCouponUse = "1000",
				OrderPointUse = "10000",
				OrderPointUseYen = "10000",
				FixedPurchaseMemberDiscountAmount = "100000",
				FixedPurchaseDiscountPrice = "1000000",
				SetPromotions = new[]
				{
					new OrderSetPromotionInput
					{
						ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON,
						ProductDiscountAmount = "10000000",
					},
					new OrderSetPromotionInput
					{
						ShippingChargeFreeFlg = Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON,
						ShippingChargeDiscountAmount = "100000000",
					},
					new OrderSetPromotionInput
					{
						PaymentChargeFreeFlg = Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON,
						PaymentChargeDiscountAmount = "1000000000",
					},
				},
				OrderPriceByTaxRates = new []
				{
					new OrderPriceByTaxRateInput
					{
						PriceCorrectionByRate = "-10000000000"
					}
				},
				OrderPriceShipping = "100000000000",
				OrderPriceExchange = "1000000000000",
			};
			var itemInputs = new[]
			{
				new OrderItemInput
				{
					OrderId = "test",
					DeleteTarget = false,
					ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN,
					ShopId = "0",
					ProductId = "YYY",
					VariationId = "_1",
					ProductPrice = "10000000000000",
					ItemQuantity = "1",
					ItemPrice = "10000000000000"
				},
				new OrderItemInput
				{
					OrderId = "test",
					DeleteTarget = false,
					ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN,
					ShopId = "0",
					ProductId = "XXX",
					VariationId = "_1",
					ProductPrice = "100000000000000",
					ItemQuantity = "2",
					ItemPrice = "200000000000000"
				}
			};

			// 上記以外の金額項目に「1」を設定する
			// ※追加金額項目などの処理への影響確認のため
			foreach (var item in itemInputs)
			{
				var newItemDataSource = new Hashtable();
				foreach (var key in item.DataSource.Keys)
				{
					if ((item.DataSource[key] is string)
						&& ((string)item.DataSource[key] == "0"))
					{
						newItemDataSource[key] = "1";
					}
					else
					{
						newItemDataSource[key] = item.DataSource[key];
					}
				}

				item.DataSource = newItemDataSource;
			}
			var newOrderDataSource = new Hashtable();
			foreach (var key in orderInput.DataSource.Keys)
			{
				if ((orderInput.DataSource[key] is string)
					&& ((string)orderInput.DataSource[key] == "0"))
				{
					newOrderDataSource[key] = "1";
				}
				else
				{
					newOrderDataSource[key] = orderInput.DataSource[key];
				}
			}

			orderInput.DataSource = newOrderDataSource;

			orderInput.Shippings = new []
			{
				new OrderShippingInput
				{
					Items = itemInputs
				}
			};
			return orderInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderInputインスタンス生成
		/// </summary>		
		private OrderInput CreateTemplateOrderInputForUpdateTest()
		{
			var val = 1;
			var orderIuput = new OrderInput()
			{
				PaymentName = (val++).ToString(),
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
				CardTranId = (val++).ToString(),
				PaymentOrderId = (val++).ToString(),
				OrderPriceShipping = (val++).ToString(),
				OrderPriceExchange = (val++).ToString(),
				OrderPriceRegulation = (val++).ToString(),
				Memo = (val++).ToString(),
				PaymentMemo = (val++).ToString(),
				ManagementMemo = (val++).ToString(),
				ShippingMemo = (val++).ToString(),
				RelationMemo = (val++).ToString(),
				RegulationMemo = (val++).ToString(),
				CardInstruments = (val++).ToString(),
				CardInstallmentsCode = (val++).ToString(),
				SetpromotionProductDiscountAmount = (val++).ToString(),
				SetpromotionShippingChargeDiscountAmount = (val++).ToString(),
				SetpromotionPaymentChargeDiscountAmount = (val++).ToString(),
				ShippingPriceSeparateEstimatesFlg = (val++).ToString(),
				OrderKbn = (val++).ToString(),
				ReturnExchangeKbn = (val++).ToString(),
				ReturnExchangeReasonKbn = (val++).ToString(),
				ReturnExchangeReasonMemo = (val++).ToString(),
				RepaymentMemo = (val++).ToString(),
				AdvcodeFirst = (val++).ToString(),
				AdvcodeNew = (val++).ToString(),
				InflowContentsType = (val++).ToString(),
				InflowContentsId = (val++).ToString(),
				OldLastBilledAmount = (val++).ToString(),
				ShippingId = (val++).ToString(),
				OrderPriceTax = (val++).ToString(),
				OrderPriceSubtotalTax = (val++).ToString(),
				SettlementCurrency = (val++).ToString(),
				SettlementRate = (val++).ToString(),
				SettlementAmount = (val++).ToString(),
				ExternalOrderId = (val++).ToString(),
				ShippingTaxRate = (val++).ToString(),
				PaymentTaxRate = (val++).ToString(),
				InvoiceBundleFlg = (val++).ToString(),
				ReceiptFlg = (val++).ToString(),
				ReceiptOutputFlg = (val++).ToString(),
				ReceiptAddress = (val++).ToString(),
				ReceiptProviso = (val++).ToString(),
				ExternalPaymentType = (val++).ToString(),
				OrderPointAdd = (val++).ToString(),
				OrderPointUse = (val++).ToString(),
				OrderPointUseYen = (val++).ToString(),
				LastOrderPointUse = (val++).ToString(),
				LastOrderPointUseYen = (val++).ToString(),
				OldLastOrderPointUse = (val++).ToString(),
				OrderCouponUse = (val++).ToString(),
				MemberRankDiscountPrice = (val++).ToString(),
				FixedPurchaseDiscountPrice = (val++).ToString(),
				FixedPurchaseMemberDiscountAmount = (val++).ToString(),
				Owner = CreateTemplateOrderOwnerInputForUpdateTest(),
				Shippings = new[]
				{
					CreateTemplateOrderShippingInputForUpdateTest(),
				},
				SetPromotions = new[]
				{
					CreateTemplateOrderSetPromotionInputForUpdateTest(),
				},
				Coupons = new[]
				{
					CreateTemplateOrderCouponInputForUpdateTest(),
				},
				OrderPriceByTaxRates = new[]
				{
					CreateTemplateOrderPriceByTaxRateInputForUpdateTest(),
				}
			};

			return orderIuput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderOwnerInputインスタンス生成
		/// </summary>		
		private OrderOwnerInput CreateTemplateOrderOwnerInputForUpdateTest()
		{
			var val = 1;
			var orderOwnerInput = new OrderOwnerInput()
			{
				OrderId = (val++).ToString(),
				OwnerKbn = (val++).ToString(),
				OwnerName = (val++).ToString(),
				OwnerName1 = (val++).ToString(),
				OwnerName2 = (val++).ToString(),
				OwnerNameKana = (val++).ToString(),
				OwnerNameKana1 = (val++).ToString(),
				OwnerNameKana2 = (val++).ToString(),
				OwnerMailAddr = (val++).ToString(),
				OwnerMailAddr2 = (val++).ToString(),
				OwnerAddrCountryIsoCode = (val++).ToString(),
				OwnerAddrCountryName = (val++).ToString(),
				OwnerZip = (val++).ToString(),
				OwnerZip1 = (val++).ToString(),
				OwnerZip2 = (val++).ToString(),
				OwnerAddr1 = (val++).ToString(),
				OwnerAddr2 = (val++).ToString(),
				OwnerAddr3 = (val++).ToString(),
				OwnerAddr4 = (val++).ToString(),
				OwnerAddr5 = (val++).ToString(),
				OwnerTel1 = (val++).ToString(),
				OwnerTel1_1 = (val++).ToString(),
				OwnerTel1_2 = (val++).ToString(),
				OwnerTel1_3 = (val++).ToString(),
				OwnerSex = (val++).ToString(),
				OwnerBirth = "2020/01/01",
				OwnerCompanyName = (val++).ToString(),
				OwnerCompanyPostName = (val++).ToString(),
				UserMemo = (val++).ToString(),
				UserManagementLevelId = (val++).ToString(),
				AccessCountryIsoCode = (val++).ToString(),
				DispLanguageCode = (val++).ToString(),
				DispLanguageLocaleId = (val++).ToString(),
				DispCurrencyCode = (val++).ToString(),
				DispCurrencyLocaleId = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			return orderOwnerInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderShippingInputインスタンス生成
		/// </summary>		
		private OrderShippingInput CreateTemplateOrderShippingInputForUpdateTest()
		{
			var val = 1;
			var orderShippingInput = new OrderShippingInput
			{
				OrderId = (val++).ToString(),
				OrderShippingNo = (val++).ToString(),
				ShippingName = (val++).ToString(),
				ShippingName1 = (val++).ToString(),
				ShippingName2 = (val++).ToString(),
				ShippingNameKana = (val++).ToString(),
				ShippingNameKana1 = (val++).ToString(),
				ShippingNameKana2 = (val++).ToString(),
				ShippingAddr1 = (val++).ToString(),
				ShippingAddr2 = (val++).ToString(),
				ShippingAddr3 = (val++).ToString(),
				ShippingAddr4 = (val++).ToString(),
				ShippingTel1 = (val++).ToString(),
				ShippingTel1_1 = (val++).ToString(),
				ShippingTel1_2 = (val++).ToString(),
				ShippingTel1_3 = (val++).ToString(),
				ShippingTel2 = (val++).ToString(),
				ShippingTel3 = (val++).ToString(),
				ShippingFax = (val++).ToString(),
				ShippingCompany = (val++).ToString(),
				ShippingDate = "2020/01/01",
				ShippingTime = (val++).ToString(),
				ShippingCheckNo = (val++).ToString(),
				DelFlg = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
				SenderName = (val++).ToString(),
				SenderName1 = (val++).ToString(),
				SenderName2 = (val++).ToString(),
				SenderNameKana = (val++).ToString(),
				SenderNameKana1 = (val++).ToString(),
				SenderNameKana2 = (val++).ToString(),
				SenderZip = (val++).ToString(),
				SenderZip1 = (val++).ToString(),
				SenderZip2 = (val++).ToString(),
				SenderAddr1 = (val++).ToString(),
				SenderAddr2 = (val++).ToString(),
				SenderAddr3 = (val++).ToString(),
				SenderAddr4 = (val++).ToString(),
				SenderTel1 = (val++).ToString(),
				SenderTel1_1 = (val++).ToString(),
				SenderTel1_2 = (val++).ToString(),
				SenderTel1_3 = (val++).ToString(),
				WrappingPaperType = (val++).ToString(),
				WrappingPaperName = (val++).ToString(),
				WrappingBagType = (val++).ToString(),
				ShippingCompanyName = (val++).ToString(),
				ShippingCompanyPostName = (val++).ToString(),
				SenderCompanyName = (val++).ToString(),
				SenderCompanyPostName = (val++).ToString(),
				AnotherShippingFlg = (val++).ToString(),
				ShippingMethod = (val++).ToString(),
				DeliveryCompanyId = (val++).ToString(),
				ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
				ShippingCountryName = (val++).ToString(),
				ShippingAddr5 = (val++).ToString(),
				SenderCountryIsoCode = (val++).ToString(),
				SenderCountryName = (val++).ToString(),
				SenderAddr5 = (val++).ToString(),
				ScheduledShippingDate = "2020/01/01",
				ExternalShipmentEntry = false,
				OldShippingCheckNo = (val++).ToString(),
				ShippingReceivingStoreId = (val++).ToString(),
				ShippingExternalDelivertyStatus = (val++).ToString(),
				ShippingStatus = (val++).ToString(),
				ShippingStatusUpdateDate = "2020/01/01",
				ShippingReceivingMailDate = "2020/01/01",
				ShippingReceivingStoreType = (val++).ToString(),
				Items = new[]
				{
					CreateTemplateOrderItemInputForUpdateTest(),
				}
			};

			return orderShippingInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderItemInputインスタンス生成
		/// </summary>		
		private OrderItemInput CreateTemplateOrderItemInputForUpdateTest()
		{
			var val = 1;
			var orderItemInput = new OrderItemInput
			{
				DeleteTargetSet = false,
				OrderId = (val++).ToString(),
				OrderItemNo = (val++).ToString(),
				OrderShippingNo = (val++).ToString(),
				ShopId = (val++).ToString(),
				ProductId = (val++).ToString(),
				VariationId = (val++).ToString(),
				VId = (val++).ToString(),
				ProductName = (val++).ToString(),
				ProductNameKana = (val++).ToString(),
				ProductPrice = (val++).ToString(),
				ProductPriceOrg = (val++).ToString(),
				ProductPoint = (val++).ToString(),
				ProductTaxIncludedFlg = (val++).ToString(),
				ProductTaxRate = (val++).ToString(),
				ProductTaxRoundType = (val++).ToString(),
				ProductPricePretax = (val++).ToString(),
				ProductPointKbn = (val++).ToString(),
				ItemRealstockReserved = (val++).ToString(),
				ItemRealstockShipped = (val++).ToString(),
				ItemQuantity = (val++).ToString(),
				ItemQuantitySingle = (val++).ToString(),
				ItemPrice = (val++).ToString(),
				ItemPriceSingle = (val++).ToString(),
				ItemPriceTax = (val++).ToString(),
				ProductSetId = (val++).ToString(),
				ItemReturnExchangeKbn = (val++).ToString(),
				ItemMemo = (val++).ToString(),
				ItemPoint = (val++).ToString(),
				ItemCancelFlg = (val++).ToString(),
				ItemReturnFlg = (val++).ToString(),
				DelFlg = (val++).ToString(),
				ProductOptionTexts = (val++).ToString(),
				BrandId = (val++).ToString(),
				DownloadUrl = (val++).ToString(),
				ProductsaleId = (val++).ToString(),
				CooperationId1 = (val++).ToString(),
				CooperationId2 = (val++).ToString(),
				CooperationId3 = (val++).ToString(),
				CooperationId4 = (val++).ToString(),
				CooperationId5 = (val++).ToString(),
				StockReturnedFlg = (val++).ToString(),
				NoveltyId = (val++).ToString(),
				RecommendId = (val++).ToString(),
				FixedPurchaseProductFlg = (val++).ToString(),
				ProductBundleId = (val++).ToString(),
				BundleItemDisplayType = (val++).ToString(),
				ItemIndex = (val++).ToString(),
				ShippingSizeKbn = (val++).ToString(),
				ColumnForMallOrder = (val++).ToString(),
				GiftWrappingId = (val++).ToString(),
				GiftMessage = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			return orderItemInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderSetPromotionInputインスタンス生成
		/// </summary>		
		private OrderSetPromotionInput CreateTemplateOrderSetPromotionInputForUpdateTest()
		{
			var val = 1;
			var orderSetPromotionInput = new OrderSetPromotionInput
			{
				OrderId = (val++).ToString(),
				OrderSetpromotionNo = (val++).ToString(),
				SetpromotionId = (val++).ToString(),
				SetpromotionName = (val++).ToString(),
				SetpromotionDispName = (val++).ToString(),
				UndiscountedProductSubtotal = (val++).ToString(),
				ProductDiscountFlg = (val++).ToString(),
				ProductDiscountAmount = (val++).ToString(),
				ShippingChargeFreeFlg = (val++).ToString(),
				ShippingChargeDiscountAmount = (val++).ToString(),
				PaymentChargeFreeFlg = (val++).ToString(),
				PaymentChargeDiscountAmount = (val++).ToString(),
			};

			return orderSetPromotionInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderCouponInputインスタンス生成
		/// </summary>		
		private OrderCouponInput CreateTemplateOrderCouponInputForUpdateTest()
		{
			var val = 1;

			var orderCouponInput = new OrderCouponInput()
			{
				OrderId = (val++).ToString(),
				OrderCouponNo = (val++).ToString(),
				DeptId = (val++).ToString(),
				CouponId = (val++).ToString(),
				CouponNo = (val++).ToString(),
				CouponCode = (val++).ToString(),
				CouponName = (val++).ToString(),
				CouponDispName = (val++).ToString(),
				CouponType = (val++).ToString(),
				CouponDiscountPrice = (val++).ToString(),
				CouponDiscountRate = (val++).ToString(),
				LastChanged = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			return orderCouponInput;
		}

		/// <summary>
		/// Updateテスト用のテンプレートOrderPriceByTaxRateInputインスタンス生成
		/// </summary>		
		private OrderPriceByTaxRateInput CreateTemplateOrderPriceByTaxRateInputForUpdateTest()
		{
			var val = 1;
			var orderPriceBytaxRateInput = new OrderPriceByTaxRateInput
			{
				OrderId = (val++).ToString(),
				PriceSubtotalByRate = (val++).ToString(),
				PriceShippingByRate = (val++).ToString(),
				PricePaymentByRate = (val++).ToString(),
				PriceTotalByRate = (val++).ToString(),
				PriceCorrectionByRate = (val++).ToString(),
				TaxPriceByRate = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			return orderPriceBytaxRateInput;
		}
	}
}
