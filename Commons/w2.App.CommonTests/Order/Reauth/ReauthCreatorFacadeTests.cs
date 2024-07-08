using Amazon.Pay.API.WebStore.Charge;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Reauth.Actions;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests.Order.Reauth.Actions;
using w2.Common.Sql;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.Order;
using w2.DomainTests.Order;
using static w2.App.Common.Order.Reauth.ReauthCreatorFacade;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Order.Reauth
{
	/// <summary>
	/// 再与信インスタンス作成Facade ユニットテスト
	/// </summary>
	[TestClass]
	public class ReauthCreatorFacadeTests : AppTestClassBase
	{
		/// <summary>注文サービスモック</summary>
		private Mock<IOrderService> _orderServiceMock;
		/// <summary>AmazonCv2Apiファサードモック</summary>
		private Mock<IAmazonCv2ApiFacade> _amazonCv2ApiFacadeMock;
		/// <summary>ReauthExecuter 与信アクション プロパティ名称</summary>
		private const string CREDIT_CARD_REAUTH_ACTION_PROPERTY_NAME = "ReauthAction";
		/// <summary>ReauthCreditCardAction 再与信アクションパラメタ プロパティ名称</summary>
		private const string CREDIT_CARD_REAUTH_ACTION_PARAMS_PROPERTY_NAME = "ReauthActionParams";

		/// <summary>
		/// 初期化
		/// </summary>
		[TestInitialize]
		public void Init()
		{
			_orderServiceMock = new Mock<IOrderService>();
			DomainFacade.Instance.OrderService = _orderServiceMock.Object;
			_amazonCv2ApiFacadeMock = new Mock<IAmazonCv2ApiFacade>();
			ExternalApiFacade.Instance.AmazonCv2ApiFacade = _amazonCv2ApiFacadeMock.Object;
		}

		/// <summary>
		/// 再与信インスタンス作成テスト
		/// </summary>
		/// <param name="oldOrder">変更前注文</param>
		/// <param name="newOrder">変更後注文</param>
		/// <param name="executeType">処理区分</param>
		/// <param name="orderActionType">注文処理区分</param>
		/// <param name="isReturnAllItems">全返品か</param>
		/// <param name="configs">設定値群</param>
		/// <param name="expected">期待結果</param>
		/// <param name="msg">メッセージ</param>
		[DataTestMethod]
		[DynamicData(nameof(TdCreateReauthTest))]
		public void CreateReauthTest(
			OrderModel oldOrder,
			OrderModel newOrder,
			ExecuteTypes executeType,
			OrderActionTypes orderActionType,
			bool isReturnAllItems,
			CreateReauthTestConfigParams configs,
			ReauthExecuter expected,
			string msg)
		{
			_orderServiceMock
				.Setup(service => service.GetRelatedOrders(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
				.Returns(new[] { oldOrder });
			_orderServiceMock
				.Setup(service => service.InspectReturnAllItems(It.IsAny<OrderModel[]>(), It.IsAny<SqlAccessor>()))
				.Returns(isReturnAllItems);
			_orderServiceMock
				.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
				.Returns(new OrderModel
				{
					OnlinePaymentStatus = configs.LinePayRefundOrgOrderOnlinePaymentStatus,
				});
			_orderServiceMock
				.Setup(service => service.GetTaxRateIncludeReturnExchange(It.IsAny<string>()))
				.Returns(new List<OrderPriceByTaxRateModel> { new OrderPriceByTaxRateModel(), new OrderPriceByTaxRateModel() });
			_amazonCv2ApiFacadeMock
				.Setup(facade => facade.GetCharge(It.IsAny<string>()))
				.Returns((string input) =>
				{
					var privateResponse = new PrivateObject(new ChargeResponse());
					privateResponse.SetProperty("ChargePermissionId", input);
					return (ChargeResponse)privateResponse.Target;
				});

			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CARD_KBN), configs.PaymentCard))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_SETTING_GMO_3DSECURE), configs.Gmo3dSecureEnabled))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_SETTING_CREDIT_RETURN_AUTOSALES_ENABLED), configs.CreditReturnAutoSalesEnabled))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD), configs.GmoCreditCardPaymentMethod))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CVS_DEF_KBN), configs.PaymentCvsDef))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_PAYPAY_KBN), configs.PaymentPayPayKbn))
			using (new TestConfigurator(Member.Of(() => Constants.AMAZON_PAYMENT_CV2_ENABLED), configs.AmazonPaymentCv2Enabled))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW), configs.AmazonPaymentCaptureNow))
			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED), configs.AmazonPaymentReturnAutoSalesEnabled))
			{
				var reauthCreatorFacade = new ReauthCreatorFacade(
					oldOrder,
					newOrder,
					executeType,
					orderActionType);

				var actual = reauthCreatorFacade.CreateReauth();

				// 全ての再与信アクションの型を検証する
				var actualPrivateObject = new PrivateObject(actual);
				var expectedPrivateObject = new PrivateObject(expected);
				foreach (var propertyName in GetReauthActionPropertyNames())
				{
					actualPrivateObject
						.GetProperty(propertyName)
						.Should()
						.BeOfType(expectedPrivateObject.GetProperty(propertyName).GetType(), msg);
				}
			}

			// 再与信アクションのプロパティ名称を取得
			IEnumerable<string> GetReauthActionPropertyNames()
			{
				yield return CREDIT_CARD_REAUTH_ACTION_PROPERTY_NAME;
				yield return "CancelAction";
				yield return "ReduceAction";
				yield return "UpdateAction";
				yield return "ReprintAction";
				yield return "SalesAction";
				yield return "RefundAction";
				yield return "BillingAction";
			}
		}

		/// <summary>
		/// 再与信インスタンス作成テスト
		/// クレジットカード再与信において、編集による実行かどうかの検証
		/// </summary>
		/// <param name="paymentCard">カード決済区分</param>
		/// <param name="orderActionType">注文処理区分</param>
		/// <param name="expected">期待結果</param>
		/// <param name="msg">メッセージ</param>
		/// <remarks>新決済がクレカ以外の場合は想定する必要がないため、新決済がクレカかどうかの条件は見ない</remarks>
		[DataTestMethod]
		[DataRow(Constants.PaymentCard.Rakuten, OrderActionTypes.Modify, true, "楽天, 編集")]
		[DataRow(Constants.PaymentCard.Rakuten, OrderActionTypes.Return, false, "楽天, 編集以外")]
		[DataRow(Constants.PaymentCard.SBPS, OrderActionTypes.Modify, false, "楽天以外, 編集")]
		public void CreateReauthTest_IsExecModifyByReauthCreditCard(
			Constants.PaymentCard paymentCard,
			OrderActionTypes orderActionType,
			bool expected,
			string msg)
		{
			_orderServiceMock
				.Setup(service => service.GetRelatedOrders(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
				.Returns(Array.Empty<OrderModel>());
			_orderServiceMock
				.Setup(service => service.InspectReturnAllItems(It.IsAny<OrderModel[]>(), It.IsAny<SqlAccessor>()))
				.Returns(false);

			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CARD_KBN), paymentCard))
			{
				// とりあえずクレジットカード再与信アクションができるパターンであれば何でもいい
				var reauthCreatorFacade = new ReauthCreatorFacade(
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					orderActionType);

				var actual = reauthCreatorFacade.CreateReauth();

				var actualReauthActionParams = new PrivateObject(
						new PrivateObject(actual).GetProperty<ReauthCreditCardAction>(CREDIT_CARD_REAUTH_ACTION_PROPERTY_NAME))
					.GetProperty<ReauthCreditCardAction.ReauthActionParams>(CREDIT_CARD_REAUTH_ACTION_PARAMS_PROPERTY_NAME);
				actualReauthActionParams.IsExecModify.Should().Be(expected, msg);
			}
		}

		/// <summary>
		/// 再与信インスタンス作成テスト
		/// クレジットカード再与信において、旧決済IDがセットされるかの検証
		/// </summary>
		/// <param name="paymentCard">カード決済区分</param>
		/// <param name="expected">期待結果</param>
		/// <param name="msg">メッセージ</param>
		[DataTestMethod]
		[DataRow(Constants.PaymentCard.VeriTrans, Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT, "ベリトランスの場合")]
		[DataRow(Constants.PaymentCard.SBPS, null, "ベリトランス以外の場合")]
		public void CreateReauthTest_SetOldPaymentKbnByReauthCreditCard(
			Constants.PaymentCard paymentCard,
			string expected,
			string msg)
		{
			_orderServiceMock
				.Setup(service => service.GetRelatedOrders(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
				.Returns(Array.Empty<OrderModel>());
			_orderServiceMock
				.Setup(service => service.InspectReturnAllItems(It.IsAny<OrderModel[]>(), It.IsAny<SqlAccessor>()))
				.Returns(false);

			using (new TestConfigurator(Member.Of(() => Constants.PAYMENT_CARD_KBN), paymentCard))
			{
				// とりあえずクレジットカード再与信アクションができるパターンであれば何でもいい
				var reauthCreatorFacade = new ReauthCreatorFacade(
					OrderModelFactoryForTest.CreateCollectOnDelivery(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return);

				var actual = reauthCreatorFacade.CreateReauth();

				var actualReauthActionParams = new PrivateObject(
						new PrivateObject(actual).GetProperty<ReauthCreditCardAction>(CREDIT_CARD_REAUTH_ACTION_PROPERTY_NAME))
					.GetProperty<ReauthCreditCardAction.ReauthActionParams>(CREDIT_CARD_REAUTH_ACTION_PARAMS_PROPERTY_NAME);
				actualReauthActionParams.OldPaymentKbn.Should().Be(expected, msg);
			}
		}

		/// <summary>再与信インスタンス作成テストデータ</summary>
		public static IEnumerable<object[]> TdCreateReauthTest
		{
			get
			{
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDなし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCardWithoutAuthDate(
						lastBilledAmount: 1000m,
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = true,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDはあるが、取引日時がないパターン, 外部決済ステータス：連携なし, GMO3DSecureのみ発生"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCardWithoutAuthDate(
						lastBilledAmount: 1000m,
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = true,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDはあるが、取引日時がないパターン, 外部決済ステータス：未決済, GMO3DSecureのみ発生"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCardWithoutAuthDate(
						lastBilledAmount: 1000m,
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = true,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDはあるが、取引日時がないパターン, 外部決済ステータス：与信済み, GMO3DSecureのみ発生"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCardWithoutAuthDate(
						lastBilledAmount: 1000m,
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDはあるが、取引日時がないパターン, 外部決済ステータス：未決済, GMO3DSecureOFFの場合（ありえるのか？）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCardWithoutAuthDate(
						lastBilledAmount: 1000m,
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 全返品, 元注文の決済取引IDはあるが、取引日時がないパターン, 外部決済ステータス：未決済, GMO以外の場合（ありえるのか？）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesCreditCard(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 全返品, 一部返金"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesCreditCard(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 一部返品"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = false,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 一部返品, 自動売上設定OFF"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.YamatoKwc,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 一部返品, 決済代行会社がヤマト"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Capture
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 一部返品, GMO即時売上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.Gmo,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesCreditCard(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 一部返品, GMO仮売上/実売上（即時売上ではない場合）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false, // 返品以外は全返品フラグFALSE固定でOK（ロジック上もそうなっている）
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 交換, 金額変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 1000m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false, // 返品以外は全返品フラグFALSE固定でOK（ロジック上もそうなっている）
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 交換, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					new OrderModel(OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 1000m).DataSource)
					{
						CreditBranchNo = 2
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false, // 返品以外は全返品フラグFALSE固定でOK（ロジック上もそうなっている）
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 交換, 金額変更なし, カード枝番に変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					new OrderModel(OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 1000m).DataSource)
					{
						CardInstallmentsCode = "2"
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false, // 返品以外は全返品フラグFALSE固定でOK（ロジック上もそうなっている）
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 交換, 金額変更なし, 支払い回数に変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: -500m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false, // 返品以外は全返品フラグFALSE固定でOK（ロジック上もそうなっている）
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 交換, 最終請求金額がマイナス"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 編集, 金額変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ -> クレカ, 編集, 金額変更あり, 強制与信"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 交換キャンセル, 最後が通常注文（元注文）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					new OrderModel(OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m).DataSource)
					{
						OrderId = "TEST-ORDER-00001-002",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesCreditCard(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 交換キャンセル, 最後が返品注文"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					new OrderModel(OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m).DataSource)
					{
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesCreditCard(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 交換キャンセル, 最後がキャンセル中の交換注文（注文IDが同じ）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 1000m),
					new OrderModel(OrderModelFactoryForTest.CreateUnauthorizeCreditCard(lastBilledAmount: 400m).DataSource)
					{
						OrderId = "TEST-ORDER-00001-002",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCard = Constants.PaymentCard.SBPS,
						Gmo3dSecureEnabled = false,
						CreditReturnAutoSalesEnabled = true,
						GmoCreditCardPaymentMethod = Constants.GmoCreditCardPaymentMethod.Auth
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCreditCard(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"クレカ, 交換キャンセル, 最後が別の交換注文"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い, 全返品"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(paymentOrderId: ""),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い, 全返品, 決済注文IDが空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い, 全返品, 外部決済ステータスが入金済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(
						orderPaymentStatus: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い, 全返品, 入金ステータスが入金済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(
						externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP,
						orderPaymentStatus: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い, 全返品, 外部決済ステータスが入金済み, 入金ステータスが入金済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ以外, 処理区分：強制与信"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.None,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ, 処理区分：何もしない"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtodeneExcludingAuthExtensionDef(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ, 処理区分：システム連動, 金額変更あり, 最終請求金額0より↑"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ, 処理区分：システム連動, 金額変更なし, 最終請求金額0より↑"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ, 処理区分：システム連動, 金額変更あり, 最終請求金額0以下"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m, orderId: "test-order-00002"),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtodeneExcludingAuthExtensionDef(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, アトディーネ, 処理区分：強制与信, 注文IDが新旧で異なる"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtodeneExcludingAuthExtensionDef(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 交換（返品以外）, アトディーネ, 処理区分：システム連動, 注文に変更あり（金額）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(orderPaymentStatus: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 交換（返品以外）, アトディーネ, 処理区分：システム連動, 注文に変更あり（金額）, 元注文が入金済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atodene
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 交換（返品以外）, アトディーネ, 処理区分：システム連動, 注文に変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.None,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Dsk
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, DSK, 処理区分：何もしない"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Dsk
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, DSK, 処理区分：システム連動, 最終請求金額が0円より上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m, orderId: "test-order-00002"),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Dsk
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, DSK, 処理区分：システム連動, 最終請求金額が0円以下, 注文IDが新旧で異なる"
					+ "（if分岐で入れてるが、0円の時点でreauthは何もしないの確定なのでいらない気も）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(
						lastBilledAmount: 0m,
						orderId: "test-order-00002",
						hasRecommendItem: true),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Dsk
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, DSK, 処理区分：システム連動, 最終請求金額が0円以下, 注文IDが新旧で同じ, レコメンド商品含む"
					+ "（if分岐で入れてるが、0円の時点でreauthは何もしないの確定なのでいらない気も）"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m, orderId: "test-order-00002"),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Dsk
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, DSK, 処理区分：システム連動, 最終請求金額が0円以下, 注文IDが新旧で同じ, レコメンド商品含まない"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.None,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Veritrans
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, ベリトランス, 処理区分：何もしない"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Veritrans
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, ベリトランス, 処理区分：システム連動"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Veritrans
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ベリトランス, 処理区分：システム連動, 金額変更あり, 最終請求金額 > 0円"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Veritrans
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyUpdateVeritransAfterPay(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ベリトランス, 処理区分：システム連動, 金額変更なし, 最終請求金額 > 0円"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Veritrans
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyUpdateVeritransAfterPay(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ベリトランス, 処理区分：システム連動, 金額変更あり, 最終請求金額 = 0円"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.None,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Score
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, スコア@払い, 処理区分：何もしない"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Score
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, スコア@払い, 処理区分：システム連動"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Score
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyUpdateScoreAfterPay(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, スコア@払い, 処理区分：システム連動"
				};
				// NOTE: 現状バグと思しきロジックが埋め込まれているため、この条件は 2024/4/3時点 ではNGになる（バグ起票 #26501）
				// 仕様の可能性もあるため、バグ管理チームの判断次第で修正する必要あり
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Score
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyUpdateScoreAfterPay(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, スコア@払い, 処理区分：システム連動, 請求金額増額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Gmo
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, GMO, 処理区分：システム連動, 注文情報変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 400m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.Atobaraicom
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, 後払い.com, 処理区分：システム連動, 注文情報変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ヤマト(GMO以外), 処理区分：システム連動, 請求金額UP"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(ownerName: "テスト注文999"),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ヤマト(GMO以外), 処理区分：システム連動, 注文者情報に変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(shippingDate: DateTime.Now),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ヤマト(GMO以外), 処理区分：システム連動, 配送希望日に変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 120m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 一部返品, ヤマト(GMO以外), 処理区分：システム連動"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 120m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 交換, ヤマト(GMO以外), 処理区分：システム連動, 請求金額変更あり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 120m),
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCvsDef(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelCvsDef(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 交換キャンセル, ヤマト(GMO以外), 処理区分：システム連動, 金額残りあり"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateCvsDef(),
					OrderModelFactoryForTest.CreateCvsDef(lastBilledAmount: 120m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentCvsDef = Constants.PaymentCvsDef.YamatoKa,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: ReauthActionFactoryForTest.CreateEmptyReduceCvsDef(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"コンビニ後払い -> コンビニ後払い, 編集, ヤマト(GMO以外), 処理区分：システム連動, 請求金額DOWN"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction:  ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし1, 強制与信, 決済注文IDが空, 返品交換ではなく、減額ではなく、最終金額0以上"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし2, 強制与信, 減額, 最終金額0以下"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction:  ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし3, 強制与信, 減額, 最終金額0以上"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし4, 強制与信, 交換, 減額ではなく"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし5, 強制与信, 交換, 減額"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし6, 強制与信, 返品, 減額ではなく"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし7, 強制与信, 返品, 減額"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし8, 強制与信, 減額ではく, 返品交換ではなく, 0円以上"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし9, 強制与信ではなく,金額変更なし,決済取引IDはある"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし10, 強制与信ではなく,金額変更なし,返品交換ではなく、決済注文IDは空"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし11, 強制与信ではなく,金額変更,返品交換ではなく、決済取引IDはある"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし12, 強制与信ではなく,金額変更,返品交換ではなく,決済注文IDはある,最終金額0以下"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction:  ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし13, 強制与信ではなく,金額変更,返品交換ではなく,決済注文IDはある,最終金額0以上"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction:  ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし14, 強制与信ではなく,金額変更,交換,減額ではない,決済注文IDは空"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし15, 強制与信ではなく,金額変更,交換,減額,決済取引IDは空"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesLinePay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし16, 強制与信ではなく,金額変更,交換,減額ではなく,決済取引IDはある,オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
						OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						CardTranId = "test-tran-id1",
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() { LinePayRefundOrgOrderOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED },
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundLinePay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし17, 強制与信ではなく,金額変更,交換,減額ではなく,決済取引IDはある,オンライン決済ステータス：売上確定済"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし18, 強制与信ではなく,金額変更,返品,減額,決済注文IDは空"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし19, 強制与信ではなく,金額変更,返品,減額ではなく,決済注文IDは空"
				};

				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし20, 強制与信ではなく,金額変更,返品,減額ではなく決済取引IDはある,オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1100m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
						ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() { LinePayRefundOrgOrderOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED },
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthLinePay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundLinePay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし21, 強制与信ではなく,金額変更,返品,減額ではなく,決済取引IDはある,オンライン決済ステータス：連携済み"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし22, 全返品,減額ではなく,決済取引IDはなし"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelLinePay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"LinePay変更なし23, 全返品,減額ではなく,決済取引IDはある"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(
						lastBilledAmount: 0m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelGmoPost(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: ReauthActionFactoryForTest.CreateEmptyModifyCancelBillingGmoPost()),
					"GMO掛け払い(都度与信), 全返品, オンライン決済ステータス：売上確定済"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(
						lastBilledAmount: 0m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelGmoPost(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信), 全返品, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 交換, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 交換, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: ReauthActionFactoryForTest.CreateEmptyReduceGmoPost(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 一部返品, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 一部返品, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 編集, 金額変更あり, 1円以上, 未出荷"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(orderStatus: Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 編集, 金額変更あり, 1円以上, 出荷済"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 編集, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(),
					OrderModelFactoryForTest.CreatePayAsYouGoOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(都度与信) -> GMO掛け払い(都度与信), 交換キャンセル, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(
						lastBilledAmount: 0m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelGmoPost(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: ReauthActionFactoryForTest.CreateEmptyModifyCancelBillingGmoPost()),
					"GMO掛け払い(枠保証), 全返品, オンライン決済ステータス：売上確定済"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(
						lastBilledAmount: 0m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelGmoPost(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証), 全返品, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 交換, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 交換, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: ReauthActionFactoryForTest.CreateEmptyReduceGmoPost(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 一部返品, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 一部返品, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 編集, 金額変更あり, 1円以上, 未出荷"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(orderStatus: Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 編集, 金額変更あり, 1円以上, 出荷済"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 編集, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(),
					OrderModelFactoryForTest.CreateFrameGuaranteeOrder(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.ExchangeCancel,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyEditGmoPost(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMO掛け払い(枠保証) -> GMO掛け払い(枠保証), 交換キャンセル, 金額変更あり, 1円以上"
				};
				yield return new object[]
				{
					new OrderModel
					{
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
					},
					new OrderModel
					{
						LastBilledAmount = 0m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: ReauthActionFactoryForTest.CreateEmptyUpdateGmoAtokara(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMOアトカラ, 全返品 (決済変更なしと列挙型同じ)"
				};
				yield return new object[]
				{
					new OrderModel
					{
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA,
					},
					OrderModelFactoryForTest.CreateAuthorizedCreditCard(lastBilledAmount: 500m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthCreditCard(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelGmoAtokara(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"GMOアトカラ -> クレカ, 一部返品"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし1, 強制与信,マイページで,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし2, 強制与信,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし3, 強制与信,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし4, 強制与信,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし5, 返品,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし6, 返品,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtoneReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし7, 返品,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtoneReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし8, 返品,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし9, 交換,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし10, 交換,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtoneReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし11, 交換,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtoneReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし12, 交換,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						IsUpdateAtonePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし13, 金額変更,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし14, 金額変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし15, 金額変更,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし16, 金額変更,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingName1 = "TESTER",
							},
						},
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingName1 = "TESTER_CHANGED",
							},
						},
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし17, 配送先情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[]{ new OrderShippingModel() },
						Owner = new OrderOwnerModel
						{
							OwnerName1 = "テスト",
						},
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[]{ new OrderShippingModel() },
						Owner = new OrderOwnerModel
						{
							OwnerName1 = "テスト テスト",
						},
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし18, 注文者情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingNameKana = "テスト",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingNameKana = "テストテスト",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし19, 配送先氏名かな変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingTel1 = "080",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingTel1 = "090",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし20, 配送先TEL変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								Items = new OrderItemModel[]
								{
									new OrderItemModel
									{
										ProductId = "TEST",
										VariationId = "01",
										ProductName = "商品",
										ProductNameKana = "テスト",
										ItemQuantity = 1,
										ItemPrice = 1000m,
									}
								}
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								Items = new OrderItemModel[]
								{
									new OrderItemModel
									{
										ProductId = "TEST2",
										VariationId = "02",
										ProductName = "商品二",
										ProductNameKana = "テストテスト",
										ItemQuantity = 2,
										ItemPrice = 500m,
									}
								}
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAtone(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtone(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし21, 商品情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[]
						{
							new OrderShippingModel()
							{
								Items = new OrderItemModel[] { new OrderItemModel() },
							}
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
						Shippings = new OrderShippingModel[]
						{
							new OrderShippingModel()
							{
								Items = new OrderItemModel[] { new OrderItemModel() },
							}
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし22, 変更なし,マイページではなく,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAtoneReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし23, 全返品,マイページではなく,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAtonePaymentFromMyPage = false,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Atone変更なし24, 全返品,マイページではなく,決済取引IDはなし,請求額<0"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 強制与信, 最終請求金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 強制与信, 増額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 強制与信, 減額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: -100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 強制与信, 減額, 最終請求金額変更0未満"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 最終請求金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 増額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 減額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: -100m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 減額, 最終請求金額変更0未満"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 最終請求金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 増額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 減額, 最終請求金額変更0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: -100m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 減額, 最終請求金額変更0未満"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(cardTranId: string.Empty),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 編集, 減額, 決済カード取引ID：空"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 編集, 減額, 決済注文ID：空"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(cardTranId: string.Empty),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 減額, 決済カード取引ID：空"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 減額, 決済注文ID：空"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesPaypay(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 減額, 外部決済ステータス：連携無し"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesPaypay(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 減額, オンライン決済ステータス：未連携"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 全返品, 減額, 最終請求金額0以上"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: -100m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 全返品, 減額, 最終請求金額0未満"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentPayPayKbn = Constants.PaymentPayPayKbn.VeriTrans,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 編集, 減額, 外部決済ステータス：連携無し, PayPay決済区分：ベリトランス"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentPayPayKbn = Constants.PaymentPayPayKbn.VeriTrans,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 交換, 減額, 外部決済ステータス：連携無し, PayPay決済区分：ベリトランス"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(externalPaymentStatus: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreatePayPayOrder(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						PaymentPayPayKbn = Constants.PaymentPayPayKbn.VeriTrans,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundPaypay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> PayPay, 返品, 減額, 外部決済ステータス：連携無し, PayPay決済区分：ベリトランス"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					new OrderModel
					{
						LastBilledAmount = 0m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelPaypay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> 決済なし, 編集, 最終請求金額0"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					new OrderModel
					{
						LastBilledAmount = 0m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelPaypay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> 決済なし, 交換, 最終請求金額0"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreatePayPayOrder(),
					new OrderModel
					{
						LastBilledAmount = 0m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams(),
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelPaypay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"PayPay -> 決済なし, 返品, 最終請求金額0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし1, 強制与信,マイページで,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし2, 強制与信,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし3, 強制与信,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし4, 強制与信,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし5, 返品,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし6, 返品,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAfteeReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし7, 返品,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAfteeReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし8, 返品,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし9, 交換,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし10, 交換,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAfteeReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし11, 交換,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAfteeReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし12, 交換,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						IsUpdateAfteePaymentFromMyPage = true,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし13, 金額変更,マイページ,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 900m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし14, 金額変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 0m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし15, 金額変更,マイページではなく,決済取引IDはある,請求額は0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = -100m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし16, 金額変更,マイページではなく,決済取引IDはある,請求額<0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingName1 = "TESTER",
							},
						},
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingName1 = "TESTER_CHANGED",
							},
						},
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし17, 配送先情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[]{ new OrderShippingModel() },
						Owner = new OrderOwnerModel
						{
							OwnerName1 = "テスト",
						},
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[]{ new OrderShippingModel() },
						Owner = new OrderOwnerModel
						{
							OwnerName1 = "テスト テスト",
						},
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし18, 注文者情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingNameKana = "テスト",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingNameKana = "テストテスト",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし19, 配送先氏名かな変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingTel1 = "080",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								ShippingTel1 = "090",
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし20, 配送先TEL変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								Items = new OrderItemModel[]
								{
									new OrderItemModel
									{
										ProductId = "TEST",
										VariationId = "01",
										ProductName = "商品",
										ProductNameKana = "テスト",
										ItemQuantity = 1,
										ItemPrice = 1000m,
									}
								}
							},
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[] {
							new OrderShippingModel()
							{
								Items = new OrderItemModel[]
								{
									new OrderItemModel
									{
										ProductId = "TEST2",
										VariationId = "02",
										ProductName = "商品二",
										ProductNameKana = "テストテスト",
										ItemQuantity = 2,
										ItemPrice = 500m,
									}
								}
							},
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAftee(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAftee(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし21, 商品情報変更,マイページではなく,決済取引IDはなし,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[]
						{
							new OrderShippingModel()
							{
								Items = new OrderItemModel[] { new OrderItemModel() },
							}
						},
						Owner = new OrderOwnerModel(),
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
						Shippings = new OrderShippingModel[]
						{
							new OrderShippingModel()
							{
								Items = new OrderItemModel[] { new OrderItemModel() },
							}
						},
						Owner = new OrderOwnerModel(),
					},
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし22, 変更なし,マイページではなく,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						CardTranId = "test-tran-id1",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAfteeReturnOrExchange(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし23, 全返品,マイページではなく,決済取引IDはある,請求額>0"
				};
				yield return new object[]
				{
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					},
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						LastBilledAmount = 1000m,
						IsUpdateAfteePaymentFromMyPage = false,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams() {},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"Aftee変更なし24, 全返品,マイページではなく,決済取引IDはなし,請求額<0"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 返品交換ではない, 最終請求金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: -100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 減額, 最終請求金額0以下"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 900m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 減額, 最終請求金額0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 増額, 最終請求金額0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 増額, 即時売上OFF"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 減額, 即時売上OFF"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 増額, 即時売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 減額, 即時売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 増額, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 減額, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 増額, 即時売上ON, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentCaptureNow = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 減額, 即時売上ON, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay,返品, 増額, オンライン決済ステータス：連携済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundAmazonPay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 返品, 減額, オンライン決済ステータス：連携済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 返品, 増額, オンライン決済ステータス：連携済み, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundAmazonPay(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 返品, 減額, オンライン決済ステータス：連携済み, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay,返品, 増額, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 返品, 減額, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 編集, 増額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 編集, 減額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPay(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 返品, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 編集, 増額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 編集, 減額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 編集, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 全返品, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPay(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 全返品, 減額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 強制与信, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPay(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 交換, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPay(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPay(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = false,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPay -> AmazonPay, 全返品, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 返品交換ではない, 最終請求金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: -100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 減額, 最終請求金額0以下"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 900m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 減額, 最終請求金額0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 増額, 最終請求金額0以上"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 増額, 即時売上OFF"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = false,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 減額, 即時売上OFF"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPayCv2(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 増額, 即時売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPayCv2(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 減額, 即時売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 増額, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 減額, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPayCv2(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 増額, 即時売上ON, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentCaptureNow = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPayCv2(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 減額, 即時売上ON, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 増額, オンライン決済ステータス：連携済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 減額, オンライン決済ステータス：連携済み"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: ReauthActionFactoryForTest.CreateEmptySalesAmazonPayCv2(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 増額, オンライン決済ステータス：連携済み, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
						AmazonPaymentReturnAutoSalesEnabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 減額, オンライン決済ステータス：連携済み, 自動売上ON"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 増額, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 900m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 減額, オンライン決済ステータス：未連携"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 増額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 減額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						paymentOrderId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1100m,
						returnExchangeKbn: Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 返品, 決済注文ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 増額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: ReauthActionFactoryForTest.CreateEmptyReauthAmazonPayCv2(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 減額"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 全返品, 金額変更なし"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 全返品, 最終請求金額0"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m),
					ExecuteTypes.ForcedAuth,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 強制与信, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 900m),
					ExecuteTypes.System,
					OrderActionTypes.Exchange,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 交換, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						cartdTranId: string.Empty),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 0m),
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 全返品, 決済カード取引ID：空"
				};
				yield return new object[]
				{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1100m, cartdTranId: "test-transaction-id2"),
					ExecuteTypes.System,
					OrderActionTypes.Modify,
					false,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: DoNothingAction.CreateEmpty(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> AmazonPayCv2, 編集, 決済カード取引ID：異なる決済カード取引ID"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreateAmazonPayCv2(lastBilledAmount: 1000m),
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
						LastBilledAmount = 0m,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: DoNothingAction.CreateEmpty(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> 決済なし, 全返品, オンライン決済ステータス：未連携"
				};
				yield return new object[]
{
					OrderModelFactoryForTest.CreateAmazonPayCv2(
						lastBilledAmount: 1000m,
						onlinePaymentStatus: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED),
					new OrderModel
					{
						OrderId = "TEST-ORDER-00001",
						OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
						LastBilledAmount = 0m,
					},
					ExecuteTypes.System,
					OrderActionTypes.Return,
					true,
					new CreateReauthTestConfigParams
					{
						AmazonPaymentCv2Enabled = true,
					},
					new ReauthExecuter(
						reauthAction: DoNothingAction.CreateEmpty(),
						cancelAction: ReauthActionFactoryForTest.CreateEmptyCancelAmazonPayCv2(),
						reduceAction: DoNothingAction.CreateEmpty(),
						updateAction: DoNothingAction.CreateEmpty(),
						reprintAction: DoNothingAction.CreateEmpty(),
						salesAction: DoNothingAction.CreateEmpty(),
						refundAction: ReauthActionFactoryForTest.CreateEmptyRefundAmazonPayCv2(),
						billingAction: DoNothingAction.CreateEmpty()),
					"AmazonPayCv2 -> 決済なし, 全返品, オンライン決済ステータス：売上確定済"
				};
			}
		}

		/// <summary>
		/// <see cref="ReauthCreatorFacade.CreateReauth" /> テスト用設定値パラメータ
		/// </summary>
		/// <remarks>
		/// 決済関連の設定値多すぎだし、再与信の決済パターンによっては考慮不要な設定値が多いのでクラスとしてまとめる<br />
		/// とりあえず当たり障りのなさそうな設定値をデフォルト値として突っ込んでおく
		/// </remarks>
		public class CreateReauthTestConfigParams
		{
			/// <summary>カード決済区分</summary>
			public Constants.PaymentCard? PaymentCard { get; set; } = Constants.PaymentCard.SBPS;
			/// <summary>Gmo：3Dセキュア対応</summary>
			public bool Gmo3dSecureEnabled { get; set; } = false;
			/// <summary>カード決済：返品時クレジットカードの自動売上確定</summary>
			public bool CreditReturnAutoSalesEnabled { get; set; } = true;
			/// <summary>GMO：決済区分</summary>
			public Constants.GmoCreditCardPaymentMethod? GmoCreditCardPaymentMethod { get; set; } = Constants.GmoCreditCardPaymentMethod.Auth;
			/// <summary>後払い区分</summary>
			public Constants.PaymentCvsDef? PaymentCvsDef { get; set; } = Constants.PaymentCvsDef.Gmo;
			/// <summary> LinePay返金用元注文のオンライン決済ステータス </summary>
			public string LinePayRefundOrgOrderOnlinePaymentStatus { get; set; } = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
			/// <summary>PayPay決済区分</summary>
			public Constants.PaymentPayPayKbn? PaymentPayPayKbn { get; set; } = Constants.PaymentPayPayKbn.GMO;
			/// <summary>AmazonPayCv2が有効</summary>
			public bool AmazonPaymentCv2Enabled { get; set; } = false;
			/// <summary>AmazonPay：即時売上かどうか</summary>
			public bool AmazonPaymentCaptureNow { get; set; } = false;
			/// <summary>AmazonPay：返品時Amazon Payの自動売上確定</summary>
			public bool AmazonPaymentReturnAutoSalesEnabled { get; set; } = false;
		}
	}
}
